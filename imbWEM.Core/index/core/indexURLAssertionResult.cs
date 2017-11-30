// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexURLAssertionResult.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.core
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
    using imbCommonModels.webPage;
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
    using imbSCI.Core.math;
    using imbSCI.Core.math.aggregation;
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
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Structure with URL assertion result set
    /// </summary>
    /// <seealso cref="aceEnumListSet{indexPageEvaluationEntryState, System.String}" />
    public class indexURLAssertionResult : indexAssertionBase<indexPageEvaluationEntryState, string>
    {
        public indexURLAssertionResult():base()
        {

        }

        /// <summary>
        /// Reconstructs flags for the link supplied, if not found returns sets <see cref="indexPageEvaluationEntryState.notInTheIndex"/> new links
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns></returns>
        public indexPageEvaluationEntryState GetFlagsFor(string link)
        {
            indexPageEvaluationEntryState fl = indexPageEvaluationEntryState.notInTheIndex;
            if (items.Contains(link))
            {
                foreach (indexPageEvaluationEntryState flag in Keys)
                {
                    if (this[flag].Contains(link))
                    {
                        if (fl == indexPageEvaluationEntryState.notInTheIndex)
                        {
                            fl = flag;
                        }
                        else
                        {
                            fl |= flag;
                        }
                    }
                }
            } 
            return fl;
        }

        /// <summary>
        /// Gets the sub assertion: uses only links that have already here, sets <see cref="indexPageEvaluationEntryState.notInTheIndex"/> new links
        /// </summary>
        /// <param name="links">The links.</param>
        /// <returns></returns>
        public indexURLAssertionResult GetSubAssertion(IEnumerable<string> links, bool useIndex=true)
        {
            indexURLAssertionResult output = new indexURLAssertionResult();
            List<string> failed = new List<string>();

            foreach (string lnk in links) {

                
                if (flagsByItem.ContainsKey(lnk))
                {
                    output.Add(flagsByItem[lnk], lnk);
                }
                else
                {
                    if (useIndex)
                    {
                        failed.Add(lnk);
                    } else
                    {
                        output.Add(indexPageEvaluationEntryState.notInTheIndex, lnk);
                    }
                    

                   
                }
            }

            
            if (useIndex)
            {
                imbWEMManager.index.pageIndexTable.GetUrlAssertion(failed, output);
            } 

            return output;
        }
        
        public override indexPageEvaluationEntryState FlagEvaluated
        {
            get
            {
                return indexPageEvaluationEntryState.haveEvaluationEntry;
            }
        }

        public override indexPageEvaluationEntryState FlagRelevant
        {
            get
            {
                return indexPageEvaluationEntryState.isRelevant;
            }
        }

        public override indexPageEvaluationEntryState FlagIndexed
        {
            get
            {
                return indexPageEvaluationEntryState.inTheIndex;
            }
        }

        /// <summary>
        /// Performs the information gain estimation.
        /// </summary>
        /// <param name="onFirstNTargets">The on first n targets.</param>
        public void performInfoGainEstimation(int onFirstNTargets = -1)
        {
            int ec = this[indexPageEvaluationEntryState.haveEvaluationEntry].Count;
            if (onFirstNTargets == -1)
            {
                onFirstNTargets = ec;
            }
            else
            {
                onFirstNTargets = Math.Min(ec, onFirstNTargets);
            }

            Targets = onFirstNTargets;

            List<string> urls = this[indexPageEvaluationEntryState.haveEvaluationEntry].Take(Targets).ToList();

            List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesForUrls(urls);

            IPnominal = pages.Sum(x => x.InfoPrize).GetRatio(Targets);
            Lm_gain = pages.Sum(x => x.Lemmas).GetRatio(Targets);

        }


        /// <summary> Number of targets included in the information gain estimation </summary>
        [Category("Result")]
        [DisplayName("Targets")]
        [imb(imbAttributeName.measure_letter, "|T?|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of targets included in the information gain estimation")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Targets { get; set; } = 0;



        [Category("Result")]
        [DisplayName("Lm gain")]
        [imb(imbAttributeName.measure_letter, "Lm")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/|T?|]")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Nominal lemma count of links in this set, by number of targets")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Lm_gain { get; set; } = 0;


        [Category("Result")]
        [DisplayName("IP nominal")]
        [imb(imbAttributeName.measure_letter, "IPn")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/|T?|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("InfoPoints nominal value of targets in this set - as sum of targets' IP factors, by number of targets")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IPnominal { get; set; } = 0;

        protected override void recalculateCustom()
        {
            
        }
    }

}