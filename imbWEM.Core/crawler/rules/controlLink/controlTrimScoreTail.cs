// --------------------------------------------------------------------------------------------------------------------
// <copyright file="controlTrimScoreTail.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.controlLink
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

    public class controlTrimScoreTail : controlLinkRuleBase
    {
        public controlTrimScoreTail(spiderEvaluatorSimpleBase __parent) : base(__parent, spiderObjectiveStatus.unknown, spiderObjectiveStatus.aborted,
            "Trim score tail", "Once active links count reaches the treshold _n_ the rule becomes active. For each iteration it calculates the first score (Q1) quartile in active links and removes all below the value. ", 20)
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


        public override void onStartIteration()
        {
            q1 = int.MinValue;
            q3 = int.MinValue;
            scoreList.Clear();

        }
        

        public override spiderObjectiveSolution evaluate(spiderLink element, modelSpiderSiteRecord sRecord, params object[] resources)
        {
            spiderObjectiveSolution sol = new spiderObjectiveSolution();
            if (scoreList.Count < 2) return sol;
            if (wRecord.web.webActiveLinks.Count > treshold)
            {
                if (q1 == int.MinValue)
                {
                    double __q1;
                    double __q3;
                    Measures.Quartiles(scoreList.ToArray(), out __q1, out __q3, true);
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

        public override void learn(spiderLink element, modelSpiderSiteRecord sRecord, params object[] resources)
        {
            scoreList.Add(Convert.ToDouble(element.marks.score));
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
            
            data.Add("quartile_q1", q1, "Score Q1", "The first score quartile of active links");
            data.Add("quartile_q3", q3, "Score Q3", "The first score quartile of active links");
			
            return data;
        }


    }

}