// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pageruleTitleUnique.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.pagerules
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
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Rule for page evaluation
    /// </summary>
    /// <seealso cref="spiderEvalRuleForLinkBase" />
    public class pageruleTitleUnique : spiderEvalRuleForPageBase
    {
        /// <summary>
        /// 
        /// </summary>
        public instanceCountCollection<string> pageTitleCount { get; set; } = new instanceCountCollection<string>();


        /// <summary>
        /// Initializes a new instance of the <see cref="pageruleTitleUnique"/> class.
        /// </summary>
        /// <param name="__parent">The parent.</param>
        public pageruleTitleUnique(spiderEvaluatorSimpleBase __parent) : base("Title unique", "If the page title is unique for the gets score award, in other case score penalty", 5, -5, __parent)
        {
        }

        /// <summary>
        /// Evaluates the specified link.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public override spiderEvalRuleResult evaluate(spiderPage page)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this,mode);

            bool score = false;

            output.comment = " freq:[" + pageTitleCount[page.webpage.pageCaption] + "] ";

            if (pageTitleCount[page.webpage.pageCaption] == 1)
            {
                score = true;
            }

            if (score)
            {
                output.score = scoreUnit;
            }
            else
            {
                output.score = penaltyUnit;
            }

            return output;
        }

        /// <summary>
        /// Takes information from page - called before evaluation
        /// </summary>
        /// <param name="page">The page.</param>
        public override void learn(spiderPage page)
        {
            //
            //pageTitles.Add(page.webpage.pageCaption);
            pageTitleCount.AddInstance(page.webpage.pageCaption, "webpage.pageCaption @ learn() in pageruleTitleUnique");
        }

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>
        /// Updated or newly created property collection
        /// </returns>
        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();

            data.Add("titleavgfrequency", pageTitleCount.avgFreq, "Title avg. frequency", "Arithmetic average frequency of page titles in the page set");
            data.Add("titleuniquecount", pageTitleCount.Keys.Count, "Title unique count", "Count of unique page titles in the page set");
            
            return data;
        }

        /// <summary>
        /// Prepares this instance - clears temporary data
        /// </summary>
        public override void prepare()
        {
            pageTitleCount = new instanceCountCollection<string>();
            pageTitleCount.compareModeDefault = instanceCountCollectionFormulae.keyCount;

            // -- nothing to prepare
        }
    }

}