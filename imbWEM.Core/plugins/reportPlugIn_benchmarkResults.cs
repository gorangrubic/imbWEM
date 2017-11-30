// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_benchmarkResults.cs" company="imbVeles" >
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
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class reportPlugIn_benchmarkResults:dataBindableBase,IReportBenchmark
    {
        public reportPlugIn_benchmarkResults()
        {
            TestID = md5.GetMd5Hash(GetHashCode().ToString());
        }

        //public void SetTestIDAndSignature(ISpiderEvaluatorBase evaluator, analyticConsoleState state, modelSpiderTestRecord tRecord)
        //{
        //    if (evaluator != null)
        //    {
        //        TestID = md5.GetMd5Hash(objectSerialization.ObjectToXML(imbWEMManager.settings)) + "-" + md5.GetMd5Hash(objectSerialization.ObjectToXML(evaluator.settings));
        //    } else if (state != null)
        //    {
        //        TestID = state.setupHash_global + "-" + state.setupHash_crawler;
        //    } else
        //    {
        //        TestID = md5.GetMd5Hash(GetHashCode().ToString());
        //    }
            
        //    TestSignature = evaluator.name + "| DC:" + state.sampleList.Count + "| PL:" + evaluator.settings.limitTotalPageLoad + "| LT:" + evaluator.settings.limitIterationNewLinks + "| IID:" + imbWEMManager.index.current_indexID + "|SID:" + imbWEMManager.index.indexSessionEntry.SessionID;

        //    Crawler = tRecord.instance.name;
        //}


        /// <summary> ID of the test run </summary>
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


        /// <summary> Average dataload per minute, counting bytes of HTML source code </summary>
        [Category("Resources")]
        [DisplayName("DataLoad")]
        [imb(imbAttributeName.measure_letter, "DL")]
        [imb(imbAttributeName.measure_setUnit, "MiB/min")]
        [imb(imbAttributeName.reporting_valueformat, "#,###.##")]
        [Description("Average dataload per minute, counting bytes of HTML source code")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double DataLoad { get; set; } = default(double);


        /// <summary> Use of CPU resource measured by the last sample acquired </summary>
        [Category("Resources")]
        [DisplayName("CPU")]
        [imb(imbAttributeName.measure_letter, "CPU")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [Description("Use of CPU resource measured by the last sample acquired")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double CPU { get; set; } = default(double);

        [Category("Resources")]
        [DisplayName("Memory")]
        [imb(imbAttributeName.measure_letter, "RAM")]
        [imb(imbAttributeName.measure_setUnit, "MiB")]
        [imb(imbAttributeName.reporting_valueformat, "#,###.##")]
        [Description("Average RAM used by the crawler")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double RAM { get; set; } = default(double);

        /// <summary> Crawl job execution duration, expressed in minutes. </summary>
        [Category("Result")]
        [DisplayName("Crawl Time")]
        [imb(imbAttributeName.measure_letter, "T")]
        [imb(imbAttributeName.measure_setUnit, "min")]
        [Description("Crawl job execution duration, expressed in minutes.")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CrawlTime { get; set; } = default(double);


       

        [Category("Result")]
        [DisplayName("Page Precision (avg)")]
        [imb(imbAttributeName.measure_letter, "E_pp")]
        [Description("Crawl precision measure, DLC average")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public double E_PP { get; set; }

        [Category("Result")]
        [DisplayName("Term Precision (avg)")]
        [imb(imbAttributeName.measure_letter, "E_tp")]
        [Description("Measure of term harvest precision")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public double E_TP { get; set; }


        /// <summary> Collected InfoPrize (calculated by relevant terms) score by page loaded  </summary>
        [Category("Result")]
        [DisplayName("IP per PL (avg)")]
        [imb(imbAttributeName.measure_letter, "IPh")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i∙Lm_i}/|P_all|")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected InfoPrize (calculated by relevant terms) score by page loaded ")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IP_collected { get; set; } = 0;


        [Category("Result")]
        [DisplayName("IP (avg)")]
        [imb(imbAttributeName.measure_letter, "IP")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i∙Lm_i}")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected InfoPrize (calculated by relevant terms) score")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IP { get; set; } = 0;


        /// <summary> Collected Lemmas's (counted by relevant terms) score by page loaded </summary>
        [Category("Result")]
        [DisplayName("Lm harvest (avg)")]
        [imb(imbAttributeName.measure_letter, "Lm")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/|P_all|")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected Lemmas's (counted by relevant terms) score by page loaded")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Lm_collected { get; set; } = 0;



        /// <summary> Lemma recall rate - domain level, recall rateo </summary>
        [Category("Result")]
        [DisplayName("Lm recall (avg)")]
        [imb(imbAttributeName.measure_letter, "E_r")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Lemma recall rate - domain level, recall rateo")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Lm_recall { get; set; } = 0;



        /// <summary> InfoPoints nominal harvest - as sum of IP factors of crawled pages, by page loaded </summary>
        [Category("Result")]
        [DisplayName("IP nominal")]
        [imb(imbAttributeName.measure_letter, "IPn")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/|P_all|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("InfoPoints nominal harvest - as sum of IP factors of crawled pages, by page loaded")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IPnominal { get; set; } = default(double);


        /// <summary> Percentage of collected nominal IP for the domain </summary>
        [Category("Result")]
        [DisplayName("IP recall")]
        [imb(imbAttributeName.measure_letter, "IP_r")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Percentage of collected nominal IP for the domain")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IP_recall { get; set; } = default(double);


        /// <summary> Percentage of collected distinct relevant term </summary>
        [Category("Result")]
        [DisplayName("Term recall")]
        [imb(imbAttributeName.measure_letter, "E_tr")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Percentage of collected distinct relevant term")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Term_recall { get; set; } = default(double);


        /// <summary> Percentage of crawled relevant pages - compared to relevant page count in the index database </summary>
        [Category("Result")]
        [DisplayName("Page recall")]
        [imb(imbAttributeName.measure_letter, "E_pr")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Percentage of crawled relevant pages - compared to relevant page count in the index database")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Page_recall { get; set; } = default(double);





    }

}