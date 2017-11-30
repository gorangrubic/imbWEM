// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderEvaluatorSimpleBase.cs" company="imbVeles" >
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
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webStructure;
    using imbNLP.Data;
    using imbNLP.Data.basic;
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
    using imbSCI.Core.reporting.render.builders;
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
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.control;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// base class for spider evaluator classes
    /// </summary>
    public abstract class spiderEvaluatorSimpleBase : spiderEvaluatorBase
    {



        public override void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            
        }

        public override void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord)
        {
            
        }

        public override void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            
        }


        /// <summary>
        /// Set of passive link rules
        /// </summary>
        public spiderEvalRuleCollection linkActiveRules { get; protected set; } = new spiderEvalRuleCollection();


        /// <summary> </summary>
        public spiderEvalRuleCollection linkPassiveRules { get; protected set; } = new spiderEvalRuleCollection();


        /// <summary>
        /// Set of passive link rules
        /// </summary>
        public spiderEvalRuleCollection pageClassificationRules { get; protected set; } = new spiderEvalRuleCollection();

        /// <summary>
        /// Set of passive link rules
        /// </summary>
        public spiderEvalRuleCollection pageContentScoreRules { get; protected set; } = new spiderEvalRuleCollection();


        /// <summary>
        /// Prepares all rules for new case
        /// </summary>
        public override void prepareAll()
        {
            base.prepareAll();

            pageScoreRules.prepare();
            linkActiveRules.prepare();
            linkPassiveRules.prepare();

            pageClassificationRules.prepare();
            pageContentScoreRules.prepare();

            builderForMarkdown bl = new builderForMarkdown();
            this.Describe(bl);
            FullDescription = bl.ToString();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="spiderEvaluatorSimpleBase"/> class.
        /// </summary>
        /// <param name="__name">The name.</param>
        /// <param name="__description">The description.</param>
        /// <param name="__aboutfile">The aboutfile.</param>
        /// <param name="__parent">The parent.</param>
        /// <param name="__doTokenization">if set to <c>true</c> [do tokenization].</param>
        public spiderEvaluatorSimpleBase(string __name, string __description, string __aboutfile, spiderUnit __parent, bool doTokenization) : base(__name, __description, __aboutfile, __parent)
        {

            if (doTokenization) settings.flags |= spiderEvaluatorExecutionFlags.doTokenization;

            language = imbLanguageFrameworkManager.serbian.basic;
        }




        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public override PropertyCollectionExtended AppendDataFields(PropertyCollection data = null)
        {
            PropertyCollectionExtended dataExtended = new PropertyCollectionExtended();

            dataExtended.Add("name", name, "Name", "Spider algorithm name");
            dataExtended.Add("description", description, "Description", "Short description");
            dataExtended.Add("linkrules", linkActiveRules.Count(), "Link rules", "Number of evaluation rules for links");
            dataExtended.Add("pagerules", pageScoreRules.Count(), "Page rules", "Number of evaluation rules for pages");
            dataExtended.Add("controlrules", controlRules.Count(), "Control rules", "Execution flow control rules");
            dataExtended.Add("controlpagerules", controlPageRules.Count(), "Control page rules", "Number of control page rules");
            dataExtended.Add("controllinkrules", controlLinkRules.Count(), "Control link rules", "Number of control link rules");
            dataExtended.Add("dotokenization", settings.flags.HasFlag(spiderEvaluatorExecutionFlags.doTokenization), "Tokenization", "If content tokenization is performed for each page loaded");

            dataExtended.Add("iterationlimit", settings.limitIterations, "Iteration limit", "Maximum number of iterations allowed");
            dataExtended.Add("iterationnewlinks", settings.limitIterationNewLinks, "New links limit", "Maximum number of new links allowed per iteration");
            dataExtended.Add("iterationtotallinks", settings.limitTotalLinks, "Total links limit", "Maximum number of links allowed for consideration");
            dataExtended.Add("pageloadlimit", settings.limitTotalPageLoad, "Total pages load limit", "Maximum number of links allowed for consideration");


            dataExtended.Add("language_native", language.languageNativeName, "Language native name", "Native name of the language used by the spider");
            dataExtended.Add("language_english", language.languageEnglishName, "Language english name", "English name of the language used by the spider");
            dataExtended.Add("language_iso", language.iso2Code, "Language code", "Language ISO 2-letter code");


            return dataExtended;
        }


        #region execution context


        private basicLanguage _language;
        /// <summary>
        /// 
        /// </summary>
        public basicLanguage language
        {
            get {
                if (_language == null)
                {
                    return imbLanguageFrameworkManager.serbian.basic;
                }
                return _language;
            }
            set {
                if (value == null)
                {
                    throw new aceGeneralException("Someone sent null to Language", null, this, "Null language");
                }

                _language = value;
            }
        }


        #endregion



        


        /// <summary>
        /// E2: Applies passive link rules to new Active links
        /// </summary>
        /// <param name="wRecord">The s record.</param>
        public override spiderObjectiveSolutionSet operation_applyLinkRules(modelSpiderSiteRecord wRecord)
        {
            spiderObjectiveSolutionSet output = new spiderObjectiveSolutionSet();

            int c = 0;
            foreach (spiderLink sLink in Enumerable.Where(wRecord.web.webActiveLinks, x => !x.flags.HasFlag(spiderLinkFlags.passiveEvaluated)))
            {
                foreach (spiderEvalRuleForLinkBase rule in linkPassiveRules)
                {
                    sLink.marks.deploy(rule.evaluate(sLink));
                }
                c++;
                sLink.flags |= spiderLinkFlags.passiveEvaluated;
            }
            if (c>0)
            {
                wRecord.log("Passive evaluation of [" + c + "] new links");
            }

            /// cleaning rule memory
            foreach (ruleActiveBase aRule in linkActiveRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
            }

            /// perceiving current situation
            foreach (spiderLink sLink in wRecord.web.webActiveLinks)
            {
                sLink.linkAge++; // <---------------------------------------------------- adding link age points
                foreach (ruleActiveBase aRule in linkActiveRules)
                {
                    aRule.learn(sLink);
                }
            }

            /// apply update on results
            foreach (spiderLink sLink in wRecord.web.webActiveLinks)
            {
                foreach (ruleActiveBase aRule in linkActiveRules)
                {
                    sLink.marks.deploy(aRule.evaluate(sLink));
                }
                sLink.marks.calculate(wRecord.iteration);
            }


            // <----------------------------sorts the links
            wRecord.web.webActiveLinks.items.Sort((x, y) => y.marks.score.CompareTo(x.marks.score));


            foreach (controlObjectiveRuleBase aRule in controlRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
                output.listen(aRule.evaluate(wRecord));
            }



            return output;
        }

      



       
    }
}


