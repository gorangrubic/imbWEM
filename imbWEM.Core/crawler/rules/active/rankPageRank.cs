// --------------------------------------------------------------------------------------------------------------------
// <copyright file="rankPageRank.cs" company="imbVeles" >
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
    using imbCommonModels;
    using imbCommonModels.webStructure;
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

    public class rankPageRank:ruleActiveBase
    {
        public rankPageRank(ISpiderEvaluatorBase __parent, double __alpha = 0.85, double __convergence = 0.0001, int __checkSteps = 10)
            : base("PageRankRule", "Integrates PageRank algorithm included from C# open source project [https://github.com/jeffersonhwang/pagerank]. Dumping value (d) is set to:" + __alpha.ToString()+", convergence (c): "
                  + __convergence + ", check steps: " + __checkSteps + ". Targets are ranked by associated page rank multiplied to integer factor: {0}. When no pages were loaded it assigns maximum score."
                  , 1000, 10, __parent)
        {
            description = string.Format(description, scoreUnit);
            alpha = __alpha;
            convergence = __convergence;
            checkSteps = __checkSteps;
        }


        /// <summary> </summary>
        public double alpha { get; protected set; } = 0;


        /// <summary> </summary>
        public double convergence { get; protected set; } = 0;


        /// <summary> </summary>
        public int checkSteps { get; protected set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public PageRank pageRank { get; set; }


        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }

        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            PropertyCollectionExtended pce = new PropertyCollectionExtended();

            return pce;
        }

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this);

            spiderTarget target = wRecord.context.targets.GetOrCreateTarget(link, false, false);

            if (!ranks.Any())
            {
                output.score = scoreUnit;
                return output;
            }

            if (ranks.ContainsKey(target))
            {
                output.score = ranks[target];
            } else
            {
                output.score = penaltyUnit;
            }
            

            return output;
        }

        public override void learn(spiderLink page)
        {
            
        }


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<ISpiderTarget, int> ranks { get; set; }


        public override void onStartIteration()
        {
            var matrix = wRecord.context.targets.GetLinkMatrixRotated();
            if (matrix != null)
            {
                pageRank = new PageRank(matrix, alpha, convergence, checkSteps);

                double[] dbl = pageRank.ComputePageRank();
                List<int> pri = new List<int>();
                foreach (double db in dbl)
                {
                    pri.Add(Convert.ToInt32(db * scoreUnit));
                }

                ranks = wRecord.context.targets.linkMatrix.MapToX(pri);
            }
        }

        public override void prepare()
        {
            ranks = new Dictionary<ISpiderTarget, int>();
        }
    }

}