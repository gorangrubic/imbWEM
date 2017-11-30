// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ruleActiveLinkStructure.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.active
{
    using System;
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
    using imbSCI.DataComplex.linknode;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Rule for page evaluation
    /// </summary>
    /// <seealso cref="spiderEvalRuleForLinkBase" />
    public class ruleActiveLinkStructure : ruleActiveBase
    {

        public ruleActiveLinkStructure(spiderEvaluatorSimpleBase __parent)
            : base("Structure node score", "Queries domain's `link tree-structure` for matching node and awards the link with part of score unit reflecting proportion between 0 and root node score." +
                                     "`Link tree-structure` node score actually represents the number of downstream (children) nodes where nodes are parts of URLs (directories, filenames and query parameters)." +
                                     "Purpose of this rule is to promote parent pages against instance pages: i.e. main `corporate news` URL is mapped to a node with score equal to number of detected news articles." +
                                     "Pages mapped to the root node are ignored to prevent intro page promotion. Read more about the model to learn more on this.", 10, 0, __parent)
        {
        }


        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }


        /// <summary> </summary>
        public int rootScore { get; protected set; } = new int();


        public override void onStartIteration()
        {
            rootScore = wRecord.linkHierarchy.root.score;
        }

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this);

            output.score = 0;

            double coeficient = 0;
            linknodeElement node = wRecord.linkHierarchy.GetByOriginalPath(link.link.originalUrl);
            if (node == null) return output;
            if (node.level == 0) return output;
            if (rootScore > 0)
            {
                coeficient = ((double)node.score) / ((double)rootScore);
                output.score = Convert.ToInt32(coeficient * scoreUnit);
            }
            
            return output;
        }


        /// <summary>
        /// Takes information from page - called before evaluation
        /// </summary>
        /// <param name="page">The page.</param>
        public override void learn(spiderLink link)
        {
            //if (link.flags.HasFlag(spiderLinkFlags.newlinkTarget) || sln.flags.HasFlag(spiderLinkFlags.newlinkVector))
            //{
            //    wRecord.linkHierarchy.Add(link.link.originalUrl, link); // <------------------------------------------------------------- izgradjuje hijerarhiju na nivou domena
            //}

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

            return data;
        }

        /// <summary>
        /// Prepares this instance - clears temporary data
        /// </summary>
        public override void prepare()
        {
            // -- nothing to prepare
        }


    }

}