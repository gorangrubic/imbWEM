// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderModuleBase.cs" company="imbVeles" >
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
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.rules.layerRules;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// The basic class for spiderModules
    /// </summary>
    /// <seealso cref="IObjectWithNameAndDescription" />
    public abstract class spiderModuleBase:IObjectWithNameAndDescription, ISpiderModuleBase
    {
        public abstract void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        public abstract void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        public abstract void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord);


        public abstract void setup();

        public abstract ISpiderModuleData evaluate(ISpiderModuleData input, modelSpiderSiteRecord wRecord);

        public abstract int CountElements();

        public int CountRules(spiderEvalRuleResultEnum mode = spiderEvalRuleResultEnum.none, spiderEvalRuleRoleEnum role = spiderEvalRuleRoleEnum.none, spiderEvalRuleSubjectEnum subject = spiderEvalRuleSubjectEnum.none)
        {
            var rls = rules.GetRules(mode, role, subject);
            return rls.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<IRuleActiveBase> rankingTargetActiveRules { get; set; } = new List<IRuleActiveBase>();


        /// <summary>
        /// 
        /// </summary>
        public List<IRuleForTarget> rankingTargetPassiveRules { get; set; } = new List<IRuleForTarget>();


        /// <summary>
        /// Ranks the links.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<spiderLink> rankLinks(List<spiderLink> input, int iteration)
        {
            List<spiderLink> output = new List<spiderLink>();

            foreach (spiderLink link in output)
            {

                foreach (layerDistributionActiveRuleBase activeRule in rankingTargetActiveRules)
                {
                    activeRule.learn(link);
                }
            }

            foreach (spiderLink link in input)
            {
                foreach (spiderEvalRuleForLinkBase passiveRule in rankingTargetPassiveRules)
                {
                    spiderEvalRuleResult lres = link.marks[passiveRule];
                    if (lres == null)
                    {
                        link.marks.deploy(passiveRule.evaluate(link));
                        
                   }
                }

                foreach (IRuleForTarget rule in rankingTargetActiveRules)
                {
                    link.marks.deploy(rule.evaluate(link));
                    
                }

                link.marks.calculate(iteration);
                
                output.Add(link);
            }

            output.Sort((x, y) => y.marks.score.CompareTo(x.marks.score));
            return output;
        }


        public abstract void AddRule(IRuleBase rule);

        public abstract void prepare();
        
       // public abstract void startIteration(int currentIteration, modelSpiderSiteRecord __wRecord);

        /// <summary> </summary>
        public spiderEvalRuleCollection rules { get; protected set; } = new spiderEvalRuleCollection();

        public void onStartIteration()
        {
            foreach (IRuleBase rule in rules)
            {
                if (rule is IRuleActiveBase)
                {
                    IRuleActiveBase rule_IRuleActiveBase = (IRuleActiveBase)rule;
                    rule_IRuleActiveBase.onStartIteration();
    
                }
            }
        }

        public virtual void startIteration(int currentIteration, modelSpiderSiteRecord __wRecord)
        {
            foreach (IRuleBase rule in rules)
            {
                if (rule is IRuleActiveBase)
                {
                    IRuleActiveBase rule_IRuleActiveBase = (IRuleActiveBase)rule;
                    rule_IRuleActiveBase.startIteration(currentIteration, __wRecord);

                } else if (rule is layerDistributionRuleBase)
                {
                    layerDistributionRuleBase rule_layerDistributionRuleBase = (layerDistributionRuleBase)rule;
                    rule_layerDistributionRuleBase.startIteration(currentIteration, __wRecord);
    
                }

            }
        }


        /// <summary>
        /// Code name used to describe the module
        /// </summary>
        public string code { get; set; }


        protected spiderModuleBase(string __name, string __desc, ISpiderEvaluatorBase __parent)
        {
            name = __name;
            code = name[0].ToString().ToUpper();
            description = __desc;
            _parent = __parent;


          
        }

        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description { get; set; } = "";


        protected ISpiderEvaluatorBase _parent; // = "";
        /// <summary>
        /// Evaluator parent
        /// </summary>
        public ISpiderEvaluatorBase parent
        {
            get { return _parent; }
        }


       
    }

}