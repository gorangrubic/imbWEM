// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ruleActiveInflows.cs" company="imbVeles" >
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
    using Accord.Statistics;
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
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Rule for page evaluation
    /// </summary>
    /// <seealso cref="spiderEvalRuleForLinkBase" />
    public class ruleActiveInflows : ruleActiveBase
    {

        public ruleActiveInflows(spiderEvaluatorSimpleBase __parent)
            : base("Inflow links", "Calculates Q1 and Q3 `count on domain` quartiles for active links, for links above Q3 rewards the score unit, for links below Q1 applies penalty.", 2, -1, __parent)
        {
        }

        /// <summary> </summary>
        public int q1 { get; protected set; } = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        public int q3 { get; set; }


        /// <summary> </summary>
        public List<double> scoreList { get; protected set; } = new List<double>();

        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }

        public override void onStartIteration()
        {
            q1 = int.MinValue;
            q3 = int.MinValue;
            scoreList.Clear();

        }


        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this);
            if (scoreList.Count() > 0) return null;
            if (q1 == int.MinValue)
            {
                double __q1;
                double __q3;
                Measures.Quartiles(scoreList.ToArray(), out __q1, out __q3, true);
                q1 = Convert.ToInt32(__q1);
                q3 = Convert.ToInt32(__q3);
            }

            if (link.countOnTheDomain > q3)
            {
                output.score = scoreUnit;
            } else if (link.countOnTheDomain <= q1)
            {
                output.score = penaltyUnit;
            } 
            
            return output;
        }


        /// <summary>
        /// Takes information from page - called before evaluation
        /// </summary>
        /// <param name="page">The page.</param>
        public override void learn(spiderLink link)
        {
            scoreList.Add(link.countOnTheDomain);
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