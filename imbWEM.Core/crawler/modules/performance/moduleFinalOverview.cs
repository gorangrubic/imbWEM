// --------------------------------------------------------------------------------------------------------------------
// <copyright file="moduleFinalOverview.cs" company="imbVeles" >
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
    using imbSCI.Core.enums;
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
    using imbSCI.Data.enums.fields;
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
    using imbWEM.Core.plugins;
    using imbWEM.Core.stage;

    public class moduleFinalOverview: imbBindable,IReportBenchmark
    {

        [Category("Label")]
        [DisplayName("TestID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the test run: MD5(global settings) - MD5(crawler settings)")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 70)]
        public string TestID { get; set; } = "";



        /// <summary> Test signature with the most important configuration values </summary>
        [Category("Label")]
        [DisplayName("TestSignature")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Test signature with the most important configuration values")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 60)]
        public string TestSignature { get; set; } = "";




        /// <summary> Name of the crawler design that was tested </summary>
        [Category("Label")]
        [DisplayName("Crawler")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Name of the crawler design that was tested")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string Crawler { get; set; } = default(string);



        public moduleFinalOverview() { }

        public void deploySum(string crawlerName, IEnumerable<moduleFinalOverview> source)
        {
            ModuleName = crawlerName + "-" + moduleIterationRecordSummary.all.ToString();
            Module = moduleIterationRecordSummary.all.ToString().imbTitleCamelOperation(); //.ToTitleCase();

            InputTargets = source.Sum(x => x.InputTargets);
            ReductionTotal = source.Sum(x => x.ReductionTotal);
            ReductionAvg = source.Sum(x => x.ReductionAvg);

            potentialPrecissionChangeTotal = source.Sum(x => x.potentialPrecissionChangeTotal);
            potentialPrecissionChangeAvg = source.Sum(x => x.potentialPrecissionChangeAvg);

            processedTotal = source.Sum(x => x.processedTotal);
            processedAvg = source.Sum(x => x.processedAvg);


            PotChangeIP = source.Sum(x => x.PotChangeIP);

            durationTotal = source.Sum(x => x.durationTotal);
            durationAvgDLC = source.Sum(x => x.durationAvgDLC);
            durationAvgIteration = source.Sum(x => x.durationAvgIteration);

            reductionRate = ReductionTotal.GetRatio(InputTargets);
            processedRate = 1;
            timeConsumption = 1;

            foreach (moduleFinalOverview modFin in source)
            {
                //modFin.reductionRate = modFin.ReductionTotal / ReductionTotal;
                modFin.processedRate = modFin.processedTotal / processedTotal;
                modFin.timeConsumption = modFin.durationTotal / durationTotal;

            }
        }

        public void deploy(string crawlerName, moduleIterationRecordSummary modEnum, aceConcurrentBag<moduleIterationRecord> records, int DLCsCount)
        {
            ModuleName = crawlerName + "-" + modEnum.ToString();
            Module = modEnum.ToString().imbTitleCamelOperation(true);
            double c = records.Count;
            iterationRecords = records.Count;

            InputTargets = records.Sum(x => x.inputTargets);

            ReductionTotal = records.Sum(x => x.output - x.inputTargets);
            ReductionAvg = ReductionTotal / c;

            reductionRate = ReductionTotal.GetRatio(InputTargets);


            potentialPrecissionChangeTotal = records.Sum(x => x.potentialPrecissionChange);
            potentialPrecissionChangeAvg = potentialPrecissionChangeTotal / c;

            processedTotal = records.Sum(x => x.processed);
            processedAvg = processedTotal / c;

            PotChangeIP = records.Sum(x => x.PotChangeIP).GetRatio(c);

          //  PotOutputIP = records.Sum(x => x.PotOutputIP) / c;

            durationTotal = records.Sum(x => x.duration);

            durationAvgDLC = durationTotal / ((double)DLCsCount);

            durationAvgIteration = durationTotal.GetRatio(processedTotal);

            
        }


        /// <summary> Name of the module of the Frontier Ranking Algorithm </summary>
        [Category("Module")]
        [DisplayName("Name")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [imb(imbAttributeName.collectionPrimaryKey)]
        [Description("Name of the module of the Frontier Ranking Algorithm")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string ModuleName { get; set; } = default(string);


        /// <summary>  </summary>
        [Category("Module")]
        [DisplayName("Module")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string Module { get; set; } = default(string);



        /// <summary>
        /// Percentage of input targets that the module took out in this iteration (output / input) - i.e. amount of work reduction, only pozitive value is allowed
        /// </summary>
        [Category("Result")]
        [DisplayName("Reduction (avg/iteration)")]
        [imb(imbAttributeName.measure_letter, "φ(MT)")]
        [imb(imbAttributeName.measure_setUnit, "n of targets")]
        //[imb(imbAttributeName.viewPriority, 29)]
        [imb(imbAttributeName.reporting_valueformat, "F2")]
      //  [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        [Description("Targets workload reduced: output - input")]
        public double ReductionAvg { get; set; } = 0;


        /// <summary> Number of targets sent to the module </summary>
        [Category("Count")]
        [DisplayName("Input Targets")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of targets sent to the module")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        [imb(imbAttributeName.reporting_hide)]
        private int InputTargets { get; set; } = 0;





        [Category("Result")]
        [DisplayName("Reduction (total)")]
        [imb(imbAttributeName.measure_letter, "φ(MT)")]
        [imb(imbAttributeName.measure_setUnit, "n of targets")]
        //[imb(imbAttributeName.viewPriority, 29)]
        [imb(imbAttributeName.reporting_valueformat, "F2")]
        //  [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        [Description("Total workload reduction provided by this module: output - input")]
        [imb(imbAttributeName.reporting_hide)]
        private double ReductionTotal { get; set; } = 0;

        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Reduction (%)")]
        [Description("Reduction rate relative to module input")]
        [imb(imbAttributeName.measure_letter, "φ(MT)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double reductionRate { get; set; } = 0;


/// <summary>
/// Gets or sets the potential precission change average.
/// </summary>
/// <value>
/// The potential precission change average.
/// </value>
[Category("Result")]
[DisplayName("Pot. Prec. Change (avg)")]
[Description("What difference the frontier layers made from MT_ipp to MT_opp. Positive value means the pot. precision  is increased.")]
[imb(imbAttributeName.measure_setUnit, "%")]
[imb(imbAttributeName.reporting_valueformat, "P2")]
[imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
//[imb(imbAttributeName.viewPriority, 31)]
[imb(imbAttributeName.measure_important, dataPointImportance.important)]
public double potentialPrecissionChangeAvg { get; set; } = 0;

        [Category("Result")]
        [DisplayName("Pot. Prec. Change (total)")]
        [Description("What difference the frontier layers made from MT_ipp to MT_opp. Positive value means the pot. precision  is increased.")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 31)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        [imb(imbAttributeName.reporting_hide)]
        public double potentialPrecissionChangeTotal { get; set; } = 0;





        [Category("Result")]
        [DisplayName("→ IP Change →")]
        [imb(imbAttributeName.measure_letter, "∆γ(IPn)")]
        [imb(imbAttributeName.measure_setUnit, "IPn_o - IPn_i")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score difference, between input and the output, for MIN(∆|PL|,|LT|) targets")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotChangeIP { get; set; } = 0;


        /// <summary>
        /// Number of targets processed by this module so far
        /// </summary>
        [Category("Workload")]
        [DisplayName("Processed (avg/iteration)")]
        [Description("Average number of targets processed by the module, per each iteration")]
        [imb(imbAttributeName.measure_letter, "Φ(T)")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        //[imb(imbAttributeName.viewPriority, 4)]
        public double processedAvg { get; set; } = 0;

        /// <summary>
        /// Number of targets processed by this module so far
        /// </summary>
        [Category("Workload")]
        [DisplayName("Processed (total)")]
        [Description("Total number of targets processed by the module during the crawl")]
        [imb(imbAttributeName.measure_letter, "Φ(T)")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        [imb(imbAttributeName.reporting_hide)]
        //[imb(imbAttributeName.viewPriority, 4)]
        public double processedTotal { get; set; } = 0;


        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Processed (%)")]
        [Description("Participation of this module in total number of processed links")]
        [imb(imbAttributeName.measure_letter, "MT_c")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double processedRate { get; set; } = 0;

        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Time / WL")]
        [Description("Average time consumed by the module, per each workload unit")]
        [imb(imbAttributeName.measure_letter, "MT/WL")]
        [imb(imbAttributeName.measure_setUnit, "s per workload unit")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double durationAvgIteration { get; set; } = 0;

        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Duration (avg/DLC)")]
        [Description("Average time per DLC consumed by the module")]
        [imb(imbAttributeName.measure_letter, "MT_t")]
        [imb(imbAttributeName.measure_setUnit, "s per DLC")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(imbAttributeName.reporting_hide)]
        //[imb(imbAttributeName.viewPriority, 3)]
        private double durationAvgDLC { get; set; } = 0;

        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Duration (total)")]
        [Description("Total time (in all threads) consumed by the module during the crawl")]
        [imb(imbAttributeName.measure_letter, "MT_t")]
        [imb(imbAttributeName.measure_setUnit, "s in total")]
        [imb(imbAttributeName.reporting_valueformat, "#,###.##")]
        [imb(imbAttributeName.reporting_hide)]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double durationTotal { get; set; } = 0;


        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Time Consumption")]
        [Description("Participation of this module in total execution time consumption")]
        [imb(imbAttributeName.measure_letter, "MT_c")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double timeConsumption { get; set; } = 0;




        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Iteration records")]
        [Description("Total count of iteration records for this module, i.e. how many iterations this module was active")]
        [imb(imbAttributeName.measure_letter, "i")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_valueformat, "#,###")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public int iterationRecords { get; set; } = 0;

    }
}
