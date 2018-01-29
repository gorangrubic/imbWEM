// --------------------------------------------------------------------------------------------------------------------
// <copyright file="rankTargetUrlGraph.cs" company="imbVeles" >
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
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Ranking implementation of the targeturl graph
    /// </summary>
    /// <seealso cref="ruleActiveBase" />
    public class rankTargetUrlGraph : ruleActiveBase
    {
        /// <summary>
        /// 
        /// </summary>
        public linknodeFrontierGraph tree { get; set; } = new linknodeFrontierGraph();

        public rankTargetUrlGraph(ISpiderEvaluatorBase __parent, int __scoreUnit = 100)
            : base("Target URL Graph", "In the Learning phase creates link-path graph out of all links sent for evaluation, according to normalized score of matched node it will assign proportion of [{0}].", __scoreUnit, 0, __parent)
        {
            description = string.Format(description, __scoreUnit);
        }

        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }

        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();


            return data;
        }

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult result = new spiderEvalRuleResult(this);

            if (tree.Gd == null)
            {

                tree.buildGd();

            }
            int max = tree.bestNode.score;

            linknodeElement linkNode = tree.Gd.sourceNodes[link.url];

            double score = ((double)scoreUnit) * ((double)linkNode.score / ((double)max));
            result.score = Convert.ToInt32(score);

            return result;
        }

        public override void learn(spiderLink page)
        {
            tree.learn(page);
        }

        public override void onStartIteration()
        {
            tree.onStartIteration(wRecord);
        }

        public override void prepare()
        {
            tree = new linknodeFrontierGraph();
            tree.prepare();
        }
    }

}