// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_workloadEntry.cs" company="imbVeles" >
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
namespace imbWEM.Core.plugins
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
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class reportPlugIn_workloadEntry : dataBindableBase, IReportBenchmark
    {
        public reportPlugIn_workloadEntry() { }


        [Category("Count")]
        [DisplayName("RecordID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Ordinal ID number of the entry")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int RecordID { get; set; } = 0;

        [Category("Label")]
        [DisplayName("EntryID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the Entry ID")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 70)]
        
        public string EntryID { get; set; } = "";


        [Category("Label")]
        [DisplayName("TestID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the test run: MD5(global settings) - MD5(crawler settings) - [extra 4 characters]")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 70)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.groupCaption)]
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
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.groupCaption)]
        public string Crawler { get; set; } = default(string);


        /// <summary> Number of content pages processed </summary>
        [Category("Processed")]
        [DisplayName("Pages")]
        [imb(imbAttributeName.measure_letter, "WL_p")]
        [imb(imbAttributeName.measure_setUnit, "n/min")]
        [imb(imbAttributeName.reporting_valueformat, "0.##")]
        [Description("Average number of pages processed in one minute")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public double ContentPages { get; set; } = 0;
        

        /// <summary> Number of crawler iterations </summary>
        [Category("Processed")]
        [DisplayName("Iterations")]
        [imb(imbAttributeName.measure_letter, "WL_i")]
        [imb(imbAttributeName.measure_setUnit, "n/min")]
        [imb(imbAttributeName.reporting_valueformat, "0.##")]
        [Description("Average number of crawler iterations processed in one minute")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public double CrawlerIterations { get; set; } = 0;


        [Category("Processed")]
        [DisplayName("Data Load")]
        [imb(imbAttributeName.measure_letter, "WL_d")]
        [imb(imbAttributeName.measure_setUnit, "MiB/min")]
        [imb(imbAttributeName.reporting_valueformat, "0.##")]
        [Description("Average MiB of HTML source code processed in one minute")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public double DataLoad { get; set; } = 0;

        [Category("Resources")]
        [Description("Average MiB of physical memory allocated")]
        [DisplayName("RAM Allocated")]
        [imb(imbAttributeName.measure_setUnit, "MiB")]
        [imb(imbAttributeName.measure_letter, "MEM_ram")]
        [imb(imbAttributeName.reporting_valueformat, "#,###.##")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        public double physicalMemory { get; set; }

        [Category("Resources")]
        [Description("Percentage of available memory")]
        [DisplayName("RAM Available")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.measure_letter, "MEM_free")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public double availableMemory { get; set; }

        [Category("Resources")]
        [Description("Average processor utilization by the crawler process ")]
        [DisplayName("Process CPU")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.measure_letter, "CPU_p")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public double cpuRateOfProcess { get; set; }

        [Category("Resources")]
        [Description("Average processor utilizationon the machine")]
        [DisplayName("Machine CPU")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.measure_letter, "CPU_m")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.avg)]
        public double cpuRateOfMachine { get; set; }

        [Category("Crawl")]
        [Description("Number of domain level crawls done")]
        [DisplayName("Done")]
        [imb(imbAttributeName.measure_setUnit, "n of DLC threads")]
        [imb(imbAttributeName.measure_letter, "TC_d")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        public int dlcDone { get; set; }

        [Category("Crawl")]
        [Description("Number of domain level crawls running at the moment")]
        [DisplayName("Running")]
        [imb(imbAttributeName.measure_setUnit, "n of DLC threads")]
        [imb(imbAttributeName.measure_letter, "TC_r")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        
        public int dlcRunning { get; set; }


        [Category("Crawl")]
        [Description("Number of domain level crawls waiting in the Crawl Job")]
        [DisplayName("Waiting")]
        [imb(imbAttributeName.measure_setUnit, "n of DLC threads")]
        [imb(imbAttributeName.measure_letter, "TC_w")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        
        public int dlcWaiting { get; set; }


        [Category("Crawl")]
        [Description("Number of domain level crawls waiting in the Crawl Job")]
        [DisplayName("DLC allowed")]
        [imb(imbAttributeName.measure_setUnit, "n of DLC threads")]
        [imb(imbAttributeName.measure_letter, "TC_max")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.firstEntry)]
        public int dlcMaximum { get; set; }



        /// <summary> State of the reporting plugin </summary>
        [Category("State")]
        [DisplayName("Status")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("State of the reporting plugin")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string pluginState { get; set; } = default(string);

        [Category("State")]
        [Description("Ordinal number of measure group, -1 means the plugin is not in the active state")]
        [DisplayName("Group")]
        [imb(imbAttributeName.measure_setUnit, "#")]
        [imb(imbAttributeName.measure_letter, "ID")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.firstEntry)]
        [imb(dataPointAggregationAspect.lateralMultiTable, dataPointAggregationType.rowSnap)]
        public int measureGroup { get; set; }

        [Category("State")]
        [Description("Number of termination warnings issued")]
        [DisplayName("Warning Count")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [imb(imbAttributeName.measure_letter, "-")]
        [imb(dataPointAggregationAspect.subSetOfRows, dataPointAggregationType.max)]
        public int terminationWarning { get; set; }

        [Category("State")]
        [DisplayName("Comment")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Additional information on the status")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string Comment { get; set; } = default(string);


    }

}