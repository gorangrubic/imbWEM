// --------------------------------------------------------------------------------------------------------------------
// <copyright file="rankDiversityALink.cs" company="imbVeles" >
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
    using imbNLP.Data.evaluate;
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
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex;
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

    public class rankDiversityALink : ruleActiveBase
    {
        public rankDiversityALink(ISpiderEvaluatorBase __parent, double __target_sd = 0.5, double __page_sd = 0.5, int __expansionSteps = 3)
            : base("TokenDiversity", "Ranks links by semantic diversity (inverted similarity) against Target tokens (" + __target_sd.ToString("P") + ") and crawled pages tokens (" + __page_sd.ToString("P") + ")."
                  + "Target tokens are semantically expanded in [" + __expansionSteps + "] steps using Semantic Lexicon"
                  , 100000, 0, __parent)
        {
            expansionSteps = __expansionSteps;
            target_sd = __target_sd;
            page_sd = __page_sd;
            //subject = spiderEvalRuleSubjectEnum.targets;
            mode = spiderEvalRuleResultEnum.active;
            doAdjustScoreByLanguageDetection = imbWEMManager.settings.crawlAdHok.FLAG_doAdjustDiversityScore;

            __parent.settings.doEnableDLC_TFIDF = true;
        }


        /// <summary>
        /// 
        /// </summary>
        public double target_sd { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double page_sd { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int expansionSteps { get; set; }


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

        /// <summary>
        /// Gets or sets a value indicating whether Diversity score should be adjusted by language estimation
        /// </summary>
        /// <value>
        /// <c>true</c> if [do adjust score by language detection]; otherwise, <c>false</c>.
        /// </value>
        public  bool doAdjustScoreByLanguageDetection { get; set; } = false;

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult result = new spiderEvalRuleResult(this);
            spiderTarget target = wRecord.context.targets.GetOrCreateTarget(link, false, false);

            termQueryDocument query = target.getQuery(expansionSteps, wRecord.logBuilder);


            wRecord.logBuilder.AppendLine("Target [" + link.url + "] query => [" + query.GetAllTermString().toCsvInLine(",") + "]");

            weightTableMatchCollection<termSpark, termSpark> matchLinks = query.GetSparkMatchAgainst<termSpark>((termDocument)wRecord.context.targets.dlTargetLinkTokens.AggregateDocument);
            weightTableMatchCollection<termSpark, termSpark> matchPage = query.GetSparkMatchAgainst<termSpark>((termDocument)wRecord.context.targets.dlTargetPageTokens.AggregateDocument);

            if ((!matchLinks.Any()) && (!matchPage.Any()))
            {
                result.score = scoreUnit;
                wRecord.logBuilder.AppendLine("D[" + link.url + "][" + target.tokens.GetAllTermString().toCsvInLine(",") + "] = no matches with query");
                return result;
            } else
            {
                wRecord.logBuilder.AppendLine("matchLinks => " + matchLinks.ToString());

                wRecord.logBuilder.AppendLine("matchPage => " + matchPage.ToString());
            }


            double pLSim = matchLinks.GetSemanticSimilarity() * target_sd;
            double pPSim = matchPage.GetSemanticSimilarity() * page_sd;

            double sim = (pLSim + pPSim);

            double sc = sim * (double)scoreUnit;

            double score = ((double)scoreUnit) - sc;

            if (doAdjustScoreByLanguageDetection)
            {

                
                // < ---- modification of diversity score
                List<string> tkns = new List<string>();
                foreach (IWeightTableTerm spark in query)
                {
                    tkns.Add(spark.nominalForm);
                }
                textEvaluation evaluation = new textEvaluation(wRecord.aJob.langTextEvaluator, null);
                evaluation.evaluateTokens(tkns, null, false);

                double evalAdj = Math.Pow(evaluation.ratioA, 2);
                result.score = Convert.ToInt32((double)score * evalAdj ); //Convert.ToInt32(sim_inv * (Double) scoreUnit);
                wRecord.logBuilder.AppendLine();
                wRecord.logBuilder.AppendLine("Score is adjusted by language evaluation ratioA ^ 2: " + evalAdj);
            }

            wRecord.logBuilder.AppendLine("D[" + link.url + "][" + target.tokens.GetAllTermString().toCsvInLine(",") + "]=[pL:" + pLSim.ToString("P2") + "][pP:" + pPSim.ToString("P2") + "]=" + sim.ToString("#0.0000") + " (" + result.score + ")");
           

            return result;
        }

        public override void learn(spiderLink page)
        {
            
        }

        public override void onStartIteration()
        {
            
        }

        public override void prepare()
        {
            //
        }
    }

}