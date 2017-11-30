// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrawlerJobEngineConfiguration.cs" company="imbVeles" >
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
namespace imbWEM.Core.settings
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
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary> Variables controling crawl job execution process, multi-threading, and monitoring features of the Crawl Job Engine </summary>
    [Category("Configuration")]
    [DisplayName("CrawlJobEngine")]
    [Description("Variables controling crawl job execution process, multi-threading, and monitoring features of the Crawl Job Engine")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]	
    public class CrawlerJobEngineConfiguration:imbBindable
    {
        public CrawlerJobEngineConfiguration() { }

        public void prepare()
        {

        }

        /// <summary> It will automatically increase TC_max parameter if CPU utilization lower then set </summary>
        [Category("Multi-threading")]
        [DisplayName("Do TC optimization")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("It will automatically increase TC_max parameter according to measured CPU utilization and behaviour of the crawler")]
        public bool doAutoAdjustTC { get; set; } = true;


        /// <summary> It will try to prevent task timeout by pushing threads priorities to improve its progress </summary>
        [Category("Monitoring")]
        [DisplayName("doTaskTimeOutPrevention")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("It will try to prevent task timeout by pushing threads priorities to improve its progress")]
        public bool doTaskTimeOutPrevention { get; set; } = true;



        /// <summary> It will try to push CPU utilization by adjusting LT </summary>
        [Category("Multi-threading")]
        [DisplayName("doDLCMulticoreOprimization")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("It will try to push CPU utilization by adjusting LT")]
        public bool doDLCMulticoreOprimization { get; set; } = false;




        /// <summary> What is CPU utilization target ratio - for TC_max auto adjustment </summary>
        [Category("Multi-threading")]
        [DisplayName("CPUTarget")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("What is CPU utilization target ratio - for TC_max auto adjustment")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CPUTarget { get; set; } = 0.6;


        /// <summary> On what CPU utilization level it should reduce TC_max </summary>
        [Category("Multi-threading")]
        [DisplayName("CPULimit")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("On what CPU utilization level it should reduce TC_max")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CPULimit { get; set; } = 0.9;




        /// <summary> On what number of samples automatic TC_max should be based </summary>
        [Category("Multi-threading")]
        [DisplayName("CPUSampleForAutoAdjust")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("On what number of samples automatic TC_max should be based")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int CPUSampleForAutoAdjust { get; set; } = 5;



        /// <summary> Maximal number of sample takes to consider for trend estimationx </summary>
        [Category("Multi-threading")]
        [DisplayName("CPUSampleForAutoAdjustMax")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Maximal number of sample takes to consider for trend estimationx")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int CPUSampleForAutoAdjustMax { get; set; } = 15;


        /// <summary> CPU rate tolerance for the adjustment </summary>
        [Category("Multi-threading")]
        [DisplayName("CPUMargin")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("CPU rate tolerance for the adjustment")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CPUMargin { get; set; } = 0.05;




        /// <summary> Maximum TC for autoadjustment </summary>
        [Category("Multi-threading")]
        [DisplayName("TCAutoLimit")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Maximum TC for autoadjustment")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int TCAutoLimit { get; set; } = 128;




        /// <summary>
        /// Miliseconds between two status checks of multithreading operation status
        /// </summary>
        [Category("Monitoring")]
        [DisplayName("DLC Check Tick")]
        [Description("Miliseconds between two status checks of multithreading operation status")]
        [imb(imbAttributeName.measure_setUnit, "ms")]
        public int crawlerDomainCheckTickMs { get; set; } = 250;


        /// <summary>
        /// Enables Parallel.ForEach execution of WebLoader Tasks
        /// </summary>
        [Category("Multi-threading")]
        [DisplayName("Allow parallel execution")]
        [Description("Enables Parallel  execution on DLC and JLC levels")]
        public bool crawlerDoParallelTaskLoads { get; set; } = true;


        /// <summary>
        /// Number of web loader loads to trigger Garbage Collection - memory clean up
        /// </summary>
        [Category("Memory")]
        [DisplayName("Loads For GC")]
        [Description("Number of web loader loads to trigger Garbage Collection - memory clean up")]
        public int loadCountForGC { get; set; } = 20;


        /// <summary>
        /// Description of $property$
        /// </summary>
        [Category("Memory")]
        [DisplayName("DLC done for GC")]
        [Description("Number of DLCs done to trigger Garbage Collection - memory clean up")]
        public int domainCountDoneForGC { get; set; } = 5;




        /// <summary> If <c>true</c> it will reorder the sample list before job run using random number generator </summary>
        [Category("Sample")]
        [DisplayName("Randomize Order")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will reorder the sample list before job run using random number generator")]
        public bool doRandomizeSampleOrder { get; set; } = true;


        /// <summary> If <c>true</c> and in cases where sample size limit is smaller than sample source list - it will take sample entries randomly </summary>
        [Category("Sample")]
        [DisplayName("Randomize Take")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will reorder the sample list before job run using random number generator")]
        public bool doRandomizeSampleTake { get; set; } = true;


    }

}