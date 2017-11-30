// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderLayerModuleBase.cs" company="imbVeles" >
//
// Copyright (C) 2017 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
// <summary>
// Project: imbWEM.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------

namespace imbWEM.Core.crawler.modules
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.morphology;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.Data.semanticLexicon.term;
    using imbSCI.Core.attributes;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.layers;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.implementation;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.rules.layerRules;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public abstract class spiderLayerModuleBase : spiderModuleBase
    {

        public override void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            /*
            var itfolder = reporter.getIterationFolder(wRecord.iteration, wRecord);

            fileunit layerInfo = new fileunit(itfolder.pathFor("layers.txt"));
            
            layerInfo.Append("Module: " + name + " id(" + GetHashCode() + ")");

            if (wRecord.lastInput != null)
            {
                layerInfo.Append(wRecord.lastInput.GetInlineDescription("Input"));
            }

            layerInfo.Append(layers.GetInlineDescription(), true);


            if (wRecord.lastOutput != null)
            {
                layerInfo.Append(wRecord.lastOutput.GetInlineDescription("Output"));


                layerInfo.Append("------------    ------------------");

                foreach (spiderLink sl in wRecord.lastOutput.active)
                {
                    layerInfo.Append(sl.url);
                    layerInfo.Append(sl.marks.GetActiveResults());
                }
                


                layerInfo.Append("------------    ------------------");
                

            }

            layerInfo.Save();
            */



            
        }


        public override void startIteration(int currentIteration, modelSpiderSiteRecord __wRecord)
        {
            layerActiveRules.ForEach(x => x.startIteration(currentIteration, __wRecord));
            layerPassiveRules.ForEach(x => x.startIteration(currentIteration, __wRecord));
            rankingTargetActiveRules.ForEach(x => x.startIteration(currentIteration, __wRecord));

            //--- currentIteration start

            //rankingTargetPassiveRules.ForEach(x=>x.p)
        }


        public override void AddRule(IRuleBase rule)
        {

            if (rule is layerDistributionActiveRuleBase)
            {
                layerDistributionActiveRuleBase rule_layerDistributionActiveRuleBase = (layerDistributionActiveRuleBase)rule;
                layerActiveRules.Add(rule_layerDistributionActiveRuleBase);

            } else if (rule is layerDistributionPassiveRuleBase) {
                layerPassiveRules.Add(rule as layerDistributionPassiveRuleBase);
            } else if (rule is IRuleActiveBase)
            {
                rankingTargetActiveRules.Add(rule as IRuleActiveBase);
            } else
            {
                rankingTargetPassiveRules.Add(rule as IRuleForTarget);
            }

            rules.Add(rule);
            rule.tagName = rule.GetType().Name + "_" + rules.Count().ToString("D2");
            
        }

        /// <summary> </summary>
        public layerStack layers { get; protected set; } = new layerStack();


        public override ISpiderModuleData evaluate(ISpiderModuleData input, modelSpiderSiteRecord wRecord)
        {
            return evaluate(input as spiderModuleData<spiderLink>, wRecord);
        }




        /// <summary>
        /// Evaluates the specified input with links
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public spiderModuleData<spiderLink> evaluate(spiderModuleData<spiderLink> input, modelSpiderSiteRecord wRecord)
        {
            // wRecord.lastInput = input.Clone();

            //  moduleDLCRecord moduleLevelReportTable = wRecord.frontierDLC[this.name];

            //     moduleIterationRecord moduleDLCRecordTableEntry = null;

            moduleIterationRecord moduleDLCRecordTableEntry = input.moduleDLCRecordTableEntry;

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                moduleDLCRecordTableEntry.reportEvaluateStart(input, wRecord, this);
            }

            wRecord.logBuilder.Append(input.GetInlineDescription("Input (" + name + ") "));

            spiderModuleData<spiderLink> output = new spiderModuleData<spiderLink>();
            output.inactive.AddRange(input.inactive);

            List<spiderLink> rest = new List<spiderLink>();

            // < --- starting iteration for the layer rules
            foreach (layerDistributionActiveRuleBase aRule in layerActiveRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
            }


            // << ---- learning about the links
            foreach (spiderLink link in input.active)
            {
                foreach (layerDistributionActiveRuleBase activeRule in layerActiveRules)
                {
                    activeRule.learn(link);
                }
            }

            // <<----- evaluation of the links
            foreach (spiderLink link in input.active)
            {
                bool assigned = false;
                layerCollection assignedTo = null;
                layerDistributionRuleBase layerRule = null;
                foreach (layerDistributionPassiveRuleBase passiveRule in layerPassiveRules)
                {
                    spiderEvalRuleResult lres = link.marks[passiveRule];
                    if (lres == null)
                    {
                        lres = passiveRule.evaluate(link);
                        link.marks.deploy(lres);
                    }

                    if (lres.layer > -1)
                    {
                        assignedTo = layers[lres.layer];
                        assigned = true;
                        layers[lres.layer].Push<spiderLink>(link);
                        layerRule = passiveRule;
                        break;
                    }
                }
                if (!assigned)
                {
                    foreach (layerDistributionActiveRuleBase activeRule in layerActiveRules)
                    {
                        spiderEvalRuleResult lres = activeRule.evaluate(link);
                        link.marks.deploy(lres);

                        if (lres.layer > -1)
                        {
                            assignedTo = layers[lres.layer];
                            assigned = true;
                            layers[lres.layer].Push<spiderLink>(link);
                            layerRule = activeRule;
                            break;
                        }
                    }
                }

                if (!assigned)
                {
                    rest.Add(link);
                } else
                {
                    wRecord.logBuilder.AppendLine("Link [" + link.url + "] => " + assignedTo.name + "(" + assignedTo.Count + ") [" + layerRule.tagName + "]");
                }
            }

            switch (restPolicy)
            {
                case spiderLayerModuleEvaluationRestPolicy.assignToTheInactive:
                    output.inactive.AddRange(rest);
                    break;
                case spiderLayerModuleEvaluationRestPolicy.assignToTheDeepestLayer:
                    output.inactive.AddRange(layers.Deepest.Push<spiderLink>(rest));
                    break;
            }


          //  wRecord.logBuilder.Append(layers.GetInlineDescription());


            
            List<spiderLink> result = layers.Pull<spiderLink>(pullLimit, doTakeFromLower);

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                moduleDLCRecordTableEntry.reportEvaluateEnd(result, wRecord, this);
              //  input.moduleDLC.reportEvaluateEnd(result, wRecord, moduleDLCRecordTableEntry, this);
            }

            wRecord.logBuilder.AppendLine("Module output => layers[" + layers.layer_id + "].Pull(" + pullLimit + ", " + doTakeFromLower + ") => " + result.Count);

            // <<----- ranking
            result = rankLinks(result, wRecord.iteration);

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                moduleDLCRecordTableEntry.reportEvaluateAlterRanking(result, wRecord, this);
                //moduleLevelReportTable.reportEvaluateAlterRanking(result, wRecord, moduleDLCRecordTableEntry, this); // ------ module level report -- after ranking
               // moduleLevelReportTable.AddOrUpdate(moduleDLCRecordTableEntry);
            }

            if (result.Any())
            {
                output.active.AddRange(result);
                output.isModuleGaveUp = false;
            } else
            {
                output.active.AddRange(input.active);
                output.isModuleGaveUp = true;
            }

            wRecord.logBuilder.Append(output.GetInlineDescription("Output"));

            

            return output;
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderLayerModuleEvaluationRestPolicy restPolicy { get; set; } = spiderLayerModuleEvaluationRestPolicy.assignToTheDeepestLayer;


        /// <summary>
        /// Number of the links returned
        /// </summary>
        public int pullLimit { get; set; } = -1;


        #region ----------- Boolean [ doTakeFromLower ] -------  [if true it will pull from lower than the best layer in order to reach the pull limit]

        /// <summary>
        /// if true it will pull from lower than the best layer in order to reach the pull limit
        /// </summary>
        [Category("Switches")]
        [DisplayName("doTakeFromLower")]
        [Description("if true it will pull from lower than the best layer in order to reach the pull limit")]
        public bool doTakeFromLower { get; set; } = false;

        #endregion


        /// <summary> </summary>
        public List<layerDistributionActiveRuleBase> layerActiveRules { get; protected set; } = new List<layerDistributionActiveRuleBase>();


        /// <summary> </summary>
        public List<layerDistributionPassiveRuleBase> layerPassiveRules { get; protected set; } = new List<layerDistributionPassiveRuleBase>();

        /// <summary>
        /// 
        /// </summary>
        public List<IRuleActiveBase> rankingTargetActiveRules { get; set; } = new List<IRuleActiveBase>();


        /// <summary>
        /// 
        /// </summary>
        public List<IRuleForTarget> rankingTargetPassiveRules { get; set; } = new List<IRuleForTarget>();


        public override int CountElements()
        {
            return layers.CountAll;
        }

        /// <summary>
        /// Prepares this instance.
        /// </summary>
        public override void prepare()
        {
            rules.prepare();
            layers.Clear();
            rankingTargetActiveRules.ForEach(x => x.prepare());
            rankingTargetPassiveRules.ForEach(x => x.prepare());
            layerPassiveRules.ForEach(x => x.prepare());
            layerActiveRules.ForEach(x => x.prepare());
            
        }


        protected spiderLayerModuleBase(string __name, string __desc, ISpiderEvaluatorBase __parent) : base(__name, __desc, __parent)
        {
        }
    }

}