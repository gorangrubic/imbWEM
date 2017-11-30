// --------------------------------------------------------------------------------------------------------------------
// <copyright file="controlPageCrossQ1.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.rules.controlPage
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.control;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class controlPageCrossQ1 : controlPageRuleBase
    {
        public controlPageCrossQ1(spiderEvaluatorSimpleBase __parent, int __treshold) : base(__parent, spiderObjectiveStatus.aborted, spiderObjectiveStatus.unknown,
            "Page crosslinks Q1", 
            "Once the highest count of crosslinks per page reaches the treshold _n_ the rule becomes active."+
            "Any page with in the first crosslink count quartile is dropped out of consideration", __treshold)
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

        /// <summary> </summary>
        public int min { get; protected set; } = int.MaxValue;


        /// <summary> </summary>
        public int max { get; protected set; } = int.MinValue;

        public override void onStartIteration()
        {
            min = int.MaxValue;
            max = int.MinValue;
            q1 = int.MinValue;
            q3 = int.MinValue;
            scoreList.Clear();
        }


        public override spiderObjectiveSolution evaluate(spiderPage element, modelSpiderSiteRecord sRecord, params object[] resources)
        {
            spiderObjectiveSolution sol = new spiderObjectiveSolution();
            if (max < treshold) return null;

            if (element.relationship.crossLinks.Count() > treshold)
            {
                if (q1 == int.MinValue)
                {
                    double __q1;
                    double __q3;
                    Measures.Quartiles(scoreList.ToArray(), out __q1, out __q3, false);
                    q1 = Convert.ToInt32(__q1);
                    q3 = Convert.ToInt32(__q3);
                }

                if (element.marks.score <= q1)
                {
                    sol = new spiderObjectiveSolution(element, spiderObjectiveStatus.aborted);
                }
                else
                {

                }
            }
            return sol;
        }

        public override void learn(spiderPage element, modelSpiderSiteRecord sRecord, params object[] resources)
        {
            int cross = element.relationship.crossLinks.Count();
            min = Math.Min(cross, min);
            max = Math.Max(cross, max);
            scoreList.Add(Convert.ToDouble(cross));
        }

        public override void prepare()
        {
            q1 = int.MinValue;
            scoreList.Clear();
        }


        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();

            data.Add("pageCross_q1", q1, "Crosslinks Q1", "The first crosslink count quartile of pages loaded");
            data.Add("pageCross_q3", q3, "Crosslinks Q3", "The third crosslink count quartile of pages loaded");
            data.Add("pageCross_min", min, "Crosslinks Min.", "The lowest count of crosslinks in pages loaded");
            data.Add("pageCross_max", max, "Crosslinks Max.", "The highest count of crosslink in pages loaded");

            return data;
        }

        
    }

}