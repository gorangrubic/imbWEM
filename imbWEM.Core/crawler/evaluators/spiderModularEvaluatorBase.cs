// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderModularEvaluatorBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.evaluators
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
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
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.control;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public abstract class spiderModularEvaluatorBase : spiderEvaluatorBase, ISpiderEvaluatorBase
    {
        public spiderModularEvaluatorBase(string __name, string __description, string __aboutfile, spiderUnit __parent) : base(__name, __description, __aboutfile, __parent)
        {
        }


        public override void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            foreach (ISpiderModuleBase module in modules)
            {
                module.reportIteration(reporter, wRecord);
            }
        }

        public override void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord)
        {
            foreach (ISpiderModuleBase module in modules)
            {
                module.reportCrawlFinished(reporter, tRecord);
            }
        }

        public override void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            foreach (ISpiderModuleBase module in modules)
            {
                module.reportDomainFinished(reporter, wRecord);
            }
        }


        /// <summary> </summary>
        public moduleCollection modules { get; protected set; } = new moduleCollection();


        public override PropertyCollectionExtended AppendDataFields(PropertyCollection data = null)
        {
            PropertyCollectionExtended dataExtended = new PropertyCollectionExtended();


            dataExtended.Add("name", name, "Name", "Spider algorithm name");
            dataExtended.Add("description", description, "Description", "Short description");

            dataExtended.Add("modules", modules.Count(), "Frontier modules", "Number of frontier management modules");

            dataExtended.Add("pagerules", pageScoreRules.Count(), "Page rules", "Number of evaluation rules for pages");
            dataExtended.Add("controlrules", controlRules.Count(), "Control rules", "Execution flow control rules");
            dataExtended.Add("controlpagerules", controlPageRules.Count(), "Control page rules", "Number of control page rules");
            dataExtended.Add("controllinkrules", controlLinkRules.Count(), "Control link rules", "Number of control link rules");
            dataExtended.Add("dotokenization", settings.flags.HasFlag(spiderEvaluatorExecutionFlags.doTokenization), "Tokenization", "If content tokenization is performed for each page loaded");

            dataExtended.Add("iterationlimit", settings.limitIterations, "Iteration limit", "Maximum number of iterations allowed");
            dataExtended.Add("iterationnewlinks", settings.limitIterationNewLinks, "New links limit", "Maximum number of new links allowed per iteration");
            dataExtended.Add("iterationtotallinks", settings.limitTotalLinks, "Total links limit", "Maximum number of links allowed for consideration");
            dataExtended.Add("pageloadlimit", settings.limitTotalPageLoad, "Total pages load limit", "Maximum number of links allowed for consideration");

            /*
            dataExtended.Add("language_native", language.languageNativeName, "Language native name", "Native name of the language used by the spider");
            dataExtended.Add("language_english", language.languageEnglishName, "Language english name", "English name of the language used by the spider");
            dataExtended.Add("language_iso", language.iso2Code, "Language code", "Language ISO 2-letter code");
            */


            return dataExtended;
        }

        public override spiderObjectiveSolutionSet operation_applyLinkRules(modelSpiderSiteRecord wRecord)
        {
            spiderModuleData<spiderLink> dataInput = new spiderModuleData<spiderLink>();
            dataInput.iteration = wRecord.iteration;
            dataInput.active.AddRange(wRecord.web.webActiveLinks);

            frontierRankingAlgorithmIterationRecord frontierReportEntry = null;
            
             
            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                frontierReportEntry = wRecord.frontierDLC.reportStartOfFRA(wRecord.iteration, wRecord, dataInput); // <----------------- reporting on module activity -- START
            }

            foreach (ISpiderModuleBase module in modules)
            {
                module.startIteration(wRecord.iteration, wRecord);
            }


            bool breakExecution = false;
            foreach (ISpiderModuleBase module in modules)
            {
                if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
                {
                    dataInput.moduleDLC = wRecord.frontierDLC.modRecords[module.GetType().Name];
                    dataInput.moduleDLCRecordTableEntry = dataInput.moduleDLC.StartNewRecord(wRecord.iteration);
                }

                spiderModuleData<spiderLink> dataOutput = null;
                if (!breakExecution)
                {
                     dataOutput = module.evaluate(dataInput, wRecord) as spiderModuleData<spiderLink>;
                }

                //dataInput.moduleDLC.reportEvaluateAlterRanking(dataOutput.active, wRecord, dataInput.moduleDLCRecordTableEntry, module as spiderModuleBase);

                if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
                {
                   
                    dataInput.moduleDLC.AddOrUpdate(dataInput.moduleDLCRecordTableEntry);
                    dataInput.moduleDLCRecordTableEntry.disposeResources();

                }

                if (!breakExecution)
                {
                    dataInput = dataOutput.CreateNext();

                    if (dataInput.active.Count == 1)
                    {
                        wRecord.log("Module " + module.name + " returned single link instance -- skipping other modules");
                        breakExecution = true;
                    }
                }
            }

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                frontierReportEntry = wRecord.frontierDLC.reportEndOfFRA(wRecord, frontierReportEntry, dataInput); // <--------------------------------------------- reporting on module activity -- END
            }
            wRecord.currentModuleData = dataInput;



            // <------------------ Objective control rules

            spiderObjectiveSolutionSet output = new spiderObjectiveSolutionSet();

            foreach (controlObjectiveRuleBase aRule in controlRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
                output.listen(aRule.evaluate(wRecord));
            }

           

            return output;
        }


        public override spiderTask operation_GetLoadTask(modelSpiderSiteRecord wRecord)
        {
            //base.operation_GetLoadTask(wRecord);

            //operation_doControlAndStats(wRecord);

            //Int32 toLimit = settings.limitTotalPageLoad - (wRecord.context.targets.GetLoaded().Count - wRecord.duplicateCount);

            //Int32 n = Math.Min(wRecord.currentModuleData.active.Count, settings.limitIterationNewLinks); //, untillLimit);
            //n = Math.Min(n, toLimit);

            //wRecord.logBuilder.log("Creating new spiderTask for iteration " + wRecord.iteration + " with " + n + " links to load. Pageloads until limit: " + toLimit);



            

            spiderTask outputTask = __operation_GetLoadTaskCommon(wRecord, wRecord.currentModuleData.active);
            


            int c = 0;
            foreach (var task in wRecord.currentModuleData.active)
            {
                string lAge = task.linkAge.ToString("D2");
                string lUrl = task.url;
                string lScore = task.marks.calculate(wRecord.iteration).ToString(); //.score.ToString();

                string lineFormat = c.ToString("D2") + " {0,4} | {1, 30} | {2,6}";

                if (outputTask.Contains(task))
                {
                    lineFormat += " (selected)";
                }

                wRecord.logBuilder.AppendLine(string.Format(lineFormat, lAge, lUrl, lScore));
                c++;
            }


            return outputTask;
        }


        public override void prepareAll()
        {
            base.prepareAll();

            foreach (ISpiderModuleBase module in modules)
            {
                module.prepare();
            }
        }
    }

}