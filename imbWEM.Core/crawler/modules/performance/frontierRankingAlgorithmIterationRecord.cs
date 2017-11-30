// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frontierRankingAlgorithmIterationRecord.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.modules.performance
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
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Iteration state of ranking algorithm --- holding data that is not relevant at particular module level
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    [imb(imbAttributeName.reporting_categoryOrder, "report,execution,input,workload,distribution,output,result")]
    public class frontierRankingAlgorithmIterationRecord : frontierEntryCommonBase
    {

        public frontierRankingAlgorithmIterationRecord()
        {
            start = DateTime.Now;
        }

        [Category("Execution")]
        [DisplayName("Duration")]
        [Description("Duration of the complete FRA, in seconds")]
        [imb(imbAttributeName.measure_letter, "M_t")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        [imb(imbAttributeName.reporting_valueformat, "F5")]
        public double duration { get; set; } = 0;


        /// <summary> Pages to be loaded until end of the crawl - i.e. to reach page load limit </summary>
        [Category("Output")]
        [DisplayName("PL left")]
        [imb(imbAttributeName.measure_letter, "∆|PL|")]
        [imb(imbAttributeName.measure_setUnit, "n of pages")]
        [Description("Pages to be loaded until end of the crawl - i.e. to reach page load limit")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int PLleft { get; set; } = 0;




        [Category("Input")]
        [DisplayName("Active Targets")]
        [Description("Number of active targets delivered to the FRA as input")]
        [imb(imbAttributeName.measure_letter, "|AT|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int inputTargets { get; set; } = 0;
      


        [Category("Input")]
        [DisplayName("AT New")]
        [Description("Just harvested links")]
        [imb(imbAttributeName.measure_letter, "|AT_new|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int newTargets { get; set; } = 0;


        [Category("Input")]
        [DisplayName("AT Old")]
        [imb(imbAttributeName.measure_letter, "|AT_old|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [Description("Targets that were in the AT before the last link harvest")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int oldTargets { get; set; } = 0;

        [Category("Input")]
        [DisplayName("All Targets")]
        [imb(imbAttributeName.measure_letter, "|T_all|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [Description("All targets discovered by the crawl - including loaded ones")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int allTargets { get; set; } = 0;


        [Category("Input")]
        [DisplayName("Eval. known")]
        [imb(imbAttributeName.measure_letter, "γ(AT)≠Ø")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [Description("Number of targets having their relevance evaluation record in the Global index database")]
        public int evaluationKnown { get; set; } = 0;
        

        
        [Category("Input")]
        [DisplayName("Eval. unknown")]
        [Description("Number of targets not having their relevance evaluation record")]
        [imb(imbAttributeName.measure_letter, "γ(AT)=Ø")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int evaluationUnknown { get; set; } = 0;

        [Category("Input")]
        [DisplayName("Certainty")]
        [Description("Measure of potential precision certainty: T_known / T_evl (targets having evaluation meta data attached)")]
        [imb(imbAttributeName.measure_letter, "≈γ")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double evaluationCertainty { get; set; } = 0;

        [Category("Input")]
        [DisplayName("→ Pot. Prec.")]
        [Description("Potential Precision value of targets that entered the module")]
        [imb(imbAttributeName.measure_letter, "γ(AT)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double inputPotentialPrecission { get; set; } = 0;


        /// <summary> Potential nominal IP score of the output, average on all targets in the output </summary>
        [Category("Input")]
        [DisplayName("→ Potential IP")]
        [imb(imbAttributeName.measure_letter, "IPn_i")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i}/∆|PL|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score of the input, average on MIN(∆|PL|,|AT|) targets in the output")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotInputIP { get; set; } = 0;



        [Category("Distribution")]
        [DisplayName("Accumulation")]
        [Description("Number of targets that are accumulated in the modules' layers")]
        [imb(imbAttributeName.measure_letter, "∑{M_n ∑{|FL|}}")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int accumulation { get; set; } = 0;


        [Category("Distribution")]
        [DisplayName("01 Language")]
        [Description("Number of targets that are accumulated in the Language module layers in this iteration")]
        [imb(imbAttributeName.measure_letter, "L ∑{|FL|}")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int accumulatedLanguage { get; set; } = 0;

        [Category("Distribution")]
        [DisplayName("02 Template")]
        [Description("Number of targets that are accumulated in the Template module layers in this iteration")]
        [imb(imbAttributeName.measure_letter, "T ∑{|FL|}")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int accumulatedTemplate { get; set; } = 0;

        [Category("Distribution")]
        [DisplayName("03 Structure")]
        [Description("Number of targets that are accumulated in the Structure module layers in this iteration")]
        [imb(imbAttributeName.measure_letter, "S ∑{|FL|}")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int accumulatedStructure { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("Module use")]
        [Description("Number of modules used in this iteration -- it is smaller than number of modules if one (but not the last) module returned single target.")]
        [imb(imbAttributeName.measure_letter, "M_use")]
        [imb(imbAttributeName.measure_setUnit, "n of modules")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int moduleUse { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("Drain")]
        [Description("when no targets at input - the modules will release their targets from secundary and reserve module layers - this number is> output - input, when output > input")]
        [imb(imbAttributeName.measure_letter, "∆|MT|→")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int drain { get; set; } = 0;
      


        
        
        [Category("Result")]
        [DisplayName("Pot. Prec. Change")]
        [Description("What difference the frontier layers made from →AT to MT→. Positive value means the pot. precision  is increased.")]
        [imb(imbAttributeName.measure_letter, "∆γ(MT)")]
        [imb(imbAttributeName.measure_setUnit, "γ(MT)-γ(AT)")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double potentialPrecissionChange { get; set; } = 0;


        /// <summary> Ratio </summary>
        [Category("Result")]
        [DisplayName("Pot. Prec. →")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.measure_letter, "γ(LT_src)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [Description("Potential precision of the Targets forwarded as Load Take input")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double outputPotentialPrecission { get; set; } = 0;

        [Category("Result")]
        [DisplayName("→ IP Change →")]
        [imb(imbAttributeName.measure_letter, "∆γ(IPn)")]
        [imb(imbAttributeName.measure_setUnit, "IPn_o - IPn_i")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score difference, between input and the output, for MIN(∆|PL|,|LT|) targets")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotChangeIP { get; set; } =0;



        /// <summary> Potential nominal IP score of the output, average on all targets in the output </summary>
        [Category("Output")]
        [DisplayName("Potential IP →")]
        [imb(imbAttributeName.measure_letter, "IPn_o")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/∆|PL|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score of the output, average on ∆|PL| targets in the output")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotOutputIP { get; set; } = 0;

        [Category("Output")]
        [DisplayName("Output")]
        [Description("Number of targets sent for the ranking and LT selection")]
        [imb(imbAttributeName.measure_letter, "|LT|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int output { get; set; } = 0;



        



        [Category("Result")]
        [DisplayName("Success")]
        [Description("How many iterations FRA output to the Loader component confirmed as relevant?")]
        [imb(imbAttributeName.reporting_valueformat, "#.##")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [imb(imbAttributeName.measure_letter, "LT_rel")]
        [imb(imbAttributeName.measure_setUnit, "n of Iterations")]
        [imb(imbAttributeName.reporting_hide)]
        public int success { get; set; } = 0;


        [Category("Result")]
        [DisplayName("Fail")]
        [Description("How many iterations FRA output to the Loader component confirmed as irrelevant?")]
        [imb(imbAttributeName.reporting_valueformat, "#.##")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [imb(imbAttributeName.measure_letter, "λ(LT)")]
        [imb(imbAttributeName.measure_setUnit, "n of Iterations")]
        [imb(imbAttributeName.reporting_hide)]
        public int fail { get; set; } = 0;
        


    }

}