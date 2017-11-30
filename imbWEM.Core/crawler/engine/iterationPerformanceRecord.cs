// --------------------------------------------------------------------------------------------------------------------
// <copyright file="iterationPerformanceRecord.cs" company="imbVeles" >
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

// using imbAnalyticsEngine.webSiteComplexCrawler;

namespace imbWEM.Core.crawler.engine
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
    using imbSCI.Data.enums.fields;
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
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    /// <summary>
    /// Data object to keep information on each iteration performance 
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />

    /// <summary> Iteration Performance record </summary>
    [Category("Record")]
    [DisplayName("Iteration record")]
    [imb(imbAttributeName.measure_letter)]
    //[imb(templateFieldDataTable.col_group, templateFieldDataTable.col_caption, templateFieldDataTable.col_desc)]
   // [imb(imbAttributeName.reporting_categoryOrder, "Report,Workload,Result,Iteration,Time")]
    [Description("Iteration Performance record")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]	
    public class iterationPerformanceRecord:imbBindable
    {
        /// <summary>
        /// Blank initiation for deserialization
        /// </summary>
        public iterationPerformanceRecord()
        {
            creationTime = DateTime.Now;
        }

        /// <summary>
        /// Proper constructor for new instance creation witn immediate data population
        /// </summary>
        /// <param name="wRecord">The w record.</param>
        public iterationPerformanceRecord(modelSpiderSiteRecord wRecord)
        {
            creationTime = DateTime.Now;
            deploy(wRecord);
            
        }

        private DateTime creationTime { get; set; }

        [Category("Report")]
        [DisplayName("Key")]
        [Description("--")]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 60)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public string key { get; set; }

        [Category("Report")]
        [DisplayName("Agg.")]
        [Description("Number of rows that were aggregated into this row")]
        [imb(imbAttributeName.measure_letter, "-")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int summedRows { get; set; } = 1;

        [Category("Report")]
        [DisplayName("Iteration")]
        [imb(imbAttributeName.measure_letter, "I")]
        [Description("Ordinal id of the iteration")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.firstEntry)]
        public int iteration { get; set; }

        [Category("Result")]
        [DisplayName("Relevant")]
        [imb(imbAttributeName.measure_letter, "|P_rel|")]
        [Description("Number of loaded relevant pages")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int relevantPageCount { get; set; }

        [Category("Result")]
        [DisplayName("Not relevant")]
        [imb(imbAttributeName.measure_letter, "|P_not|")]
        [Description("Number of loaded but irrelevant pages")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int irrelevantPageCount { get; set; }

        [Category("Workload")]
        [DisplayName("Pages loaded")]
        [imb(imbAttributeName.measure_letter, "|P_all|")]
        [Description("Number of loaded/crawled pages")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int loadedPageCount { get; set; }

        [Category("Workload")]
        [DisplayName("Duplicate pages")]
        [imb(imbAttributeName.measure_letter, "|P_dbl|")]
        [Description("Number of pages found to be duplicate of already a crawled page")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int duplicateCount { get; set; }

        [Category("Workload")]
        [DisplayName("Real loads")]
        [imb(imbAttributeName.measure_letter, "|LT_req|")]
        [Description("Includes all calls to the Loader component, including the HTTP requests that failed")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int realLoadsCount { get; set; }

        [Category("Result")]
        [DisplayName("Page Precision")]
        [imb(imbAttributeName.measure_letter, "E_pp")]
        [Description("Crawl precision measure, P_rel / P_all")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public double E_PP { get; set; }

        [Category("Result")]
        [DisplayName("Term Precision")]
        [imb(imbAttributeName.measure_letter, "E_tp")]
        [Description("Measure of term harvest")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public double E_TP { get; set; }

        [Category("Result")]
        [DisplayName("Term Efficiency")]
        [imb(imbAttributeName.measure_letter, "E_th")]
        [Description("Measure of term harvest efficiency - rel. terms per page")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double E_TH { get; set; }

        [Category("Iteration")]
        [DisplayName("Load URL")]
        [Description("Sent to the Loader component for loading (this/last iteration)")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public string targetUrl { get; set; }

        [Category("Iteration")]
        [DisplayName("Page Relevancy")]
        [Description("Result of multi-language detection")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public string targetLanguage { get; set; }

        [Category("Iteration")]
        [DisplayName("Page Relevancy - detection certency")]
        [Description("Result of multi-language detection")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public string targetEvalRatio { get; set; }

        [Category("Iteration")]
        [DisplayName("Duration")]
        [imb(imbAttributeName.measure_letter, "I_t")]
        [Description("Iteration duration in seconds")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F2")]
        public double time_duration_s { get; set; }
        //public Double time_duration_gross_s { get; set; }

        [Category("Iteration")]
        [imb(imbAttributeName.measure_letter, "I_ts")]
        [Description("Since the first iteration, period in seconds")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public double time_sincefirst_s { get; set; }

        [Category("Result")]
        [DisplayName("Blocks")]
        [imb(imbAttributeName.measure_letter, "E_b-all")]
        [Description("Number of unique content blocks harvested")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int blocks_all { get; set; }

        [Category("Result")]
        [DisplayName("Blocks Relevant")]
        [imb(imbAttributeName.measure_letter, "E_b-rel")]
        [Description("Number of unique relevant content blocks harvested")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public int blocks_relevant { get; set; }

        [Category("Result")]
        [DisplayName("Terms")]
        [imb(imbAttributeName.measure_letter, "E_t-all")]
        [Description("Number of unique terms/words harvested")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int terms_all { get; set; }

        [Category("Result")]
        [DisplayName("Terms Relevant")]
        [imb(imbAttributeName.measure_letter, "E_t-rel")]
        [Description("Number of unique terms/words found on a relevant page")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public int terms_relevant { get; set; }

        //public Int32 dataLoad { get; set; }
        //public Int32 cpuLoad { get; set; }

        /// <summary> Total time consumed by FRA part of the iteration cycle </summary>
        [Category("Time")]
        [DisplayName("FRA_Time")]
        [imb(imbAttributeName.measure_letter, "T_fra")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [Description("Total time consumed by FRA part of the iteration cycle")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]	
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [imb(imbAttributeName.reporting_hide)]
        public double FRA_SummaryRuntime { get; set; }


        #region -----------  FRA_TimePercent  -------  [Percentage of time that FRA participated in the complete Job execution time]
        private double _FRA_TimePercent = 0; // = new Double();
                                             /// <summary>
                                             /// Percentage of time that FRA participated in the complete Job execution time
                                             /// </summary>
        // [XmlIgnore]
        [Category("Time")]
        [DisplayName("FRA Time Ratio")]
        [imb(imbAttributeName.measure_letter, "T_fra")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Percentage of time that FRA participated in the complete Job execution time")]
        [imb(imbAttributeName.measure_important)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [imb(imbAttributeName.reporting_hide)]
        public double FRA_TimePercent
        {
            get
            {
                return _FRA_TimePercent;
            }
            set
            {
                _FRA_TimePercent = value;
                OnPropertyChanged("FRA_TimePercent");
            }
        }
        #endregion


        /// <summary> Collected InfoPrize (calculated by relevant terms) score by page loaded  </summary>
        [Category("Result")]
        [DisplayName("IP harvest")]
        [imb(imbAttributeName.measure_letter, "IPh")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i∙Lm_i}/|P_all|")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected InfoPrize (calculated by relevant terms) score by page loaded ")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IP_collected { get; set; } = 0;


        [Category("Result")]
        [DisplayName("IP")]
        [imb(imbAttributeName.measure_letter, "IP")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i∙Lm_i}")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected InfoPrize (calculated by relevant terms) score")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IP { get; set; } = 0;


        /// <summary> Collected Lemmas's (counted by relevant terms) score by page loaded </summary>
        [Category("Result")]
        [DisplayName("Lm harvest")]
        [imb(imbAttributeName.measure_letter, "Lm")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/|P_all|")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [Description("Collected Lemmas's (counted by relevant terms) score by page loaded")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Lm_collected { get; set; } =0;



        /// <summary> Lemma recall rate - domain level, recall rateo </summary>
        [Category("Result")]
        [DisplayName("Lm recall")]
        [imb(imbAttributeName.measure_letter, "E_r")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Lemma recall rate - domain level, recall rateo")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Lm_recall { get; set; } =0;



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







        /// <summary> Use of CPU resource measured by the last sample acquired </summary>
        [Category("Ratio")]
        [DisplayName("CPU")]
        [imb(imbAttributeName.measure_letter, "CPU")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [Description("Use of CPU resource measured by the last sample acquired")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        public double CPU { get; set; } = default(double);





        /// <summary>
        /// Deploys information from wRecord, including the key
        /// </summary>
        /// <param name="wRecord">The w record.</param>
        public void deploy(modelSpiderSiteRecord wRecord)
        {

            double i_lm_harvest = 0;
            double i_lm_recall = 0;
            double i_pi_harvest = 0;
            double i_pi_nominal = 0;

            dataUnitSpiderIteration spi_first = wRecord.timeseries.GetData().FirstOrDefault() as dataUnitSpiderIteration;
            dataUnitSpiderIteration spi_last = wRecord.timeseries.lastEntry as dataUnitSpiderIteration;
            dataUnitSpiderIteration spi_current = wRecord.timeseries.currentEntry as dataUnitSpiderIteration;
            
            if (spi_current != null) time_duration_s = creationTime.Subtract(spi_current.rowCreated).TotalSeconds; else time_duration_s = 0;
           // if (spi_last != null) time_duration_gross_s = creationTime.Subtract(spi_last.rowCreated).TotalSeconds; else time_duration_gross_s = 0;
            if (spi_first != null) time_sincefirst_s = creationTime.Subtract(spi_first.rowCreated).TotalSeconds; else time_sincefirst_s = 0;


            indexDomain idomain = wRecord.GetIndexInfo();   // imbWEMManager.index.domainIndexTable.GetDomain(wRecord.domainInfo.domainName);

            iteration = wRecord.iteration;
            
            blocks_all = wRecord.context.targets.blocks.Count(false);
            blocks_relevant = wRecord.context.targets.blocks.Count(true);

            terms_all = wRecord.context.targets.termsAll.Count();
            terms_relevant = wRecord.context.targets.termSerbian.Count();

            var TFIDF = wRecord.MasterTFIDF; // imbWEMManager.index.experimentEntry.globalTFIDFCompiled;

            var mchs = TFIDF.GetMatches(wRecord.context.targets.termSerbian);

          

            


            //TFIDF.GetScoreAggregate()


            key = wRecord.domainInfo.domainName + iteration.ToString("D3");

            int relCount = 0;
            int irelCount = 0;
            int lCount = 0;
            int rCount = 0;
            int dCount = 0;

            double fraDuration = 0;
            int modulesContained = 0;


            int rec = 0;
            foreach (frontierRankingAlgorithmIterationRecord gen in wRecord.frontierDLC.generalRecords)
            {
                rec++; 
                fraDuration += gen.duration; 
            }

            FRA_SummaryRuntime = fraDuration.GetRatio((double)rec);

            FRA_TimePercent = FRA_SummaryRuntime.GetRatio(time_duration_s);

            var rtake = wRecord.tRecord.measureTaker.GetLastTake();
            if (rtake != null)
            {
                CPU = rtake.cpuRateOfProcess;
            }


            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                foreach (moduleDLCRecord mod in wRecord.frontierDLC)
                {
                    if (mod != null)
                    {
                        modulesContained += mod.GetLastEntry().accumulated;
                    }
                }
            }


            List<string> hashList = new List<string>();
            List<spiderTarget> nonDuplicate = new List<spiderTarget>();
            foreach (spiderTarget t in wRecord.context.targets.GetLoaded())
            {

                indexPage ipage = t.GetIndexPage(); //imbWEMManager.index.pageIndexTable.GetPageForUrl(t.url);
                //i_pi_harvest += ipage.InfoPrize;

                if (ipage != null)
                {
                    i_pi_nominal += ipage.InfoPrize;
                }

                bool isDuplicate = t.isDuplicate;

                if (isDuplicate)
                {
                    if (!hashList.Contains(t.pageHash))
                    {
                        hashList.Add(t.pageHash);
                        isDuplicate = false;
                    } 
                }

                if (!isDuplicate)
                {
                    if (t.IsRelevant)
                    {
                        relCount++;
                    }
                    else
                    {
                        irelCount++;
                    }
                    lCount++;
                    nonDuplicate.Add(t);

                } else
                {
                    dCount++;
                }
                
            }
            relevantPageCount = relCount;
            irrelevantPageCount = irelCount;
            loadedPageCount = lCount;
            duplicateCount = dCount;

            int mchs_c = 0;
            int id_lm_c = 0;

            if (idomain != null) id_lm_c = idomain.Lemmas;
            if (mchs != null) mchs_c = mchs.Count();

            i_lm_harvest = mchs_c.GetRatio(loadedPageCount);

            IP = TFIDF.GetScoreForMatch(wRecord.context.targets.termSerbian);

            i_lm_recall = mchs_c.GetRatio(id_lm_c);
            if (i_lm_recall > 1) i_lm_recall = 1;


            if (idomain != null) IP_recall = i_pi_nominal.GetRatio(idomain.InfoPrize).ClipToK();
            if (idomain != null) Term_recall = wRecord.context.targets.termSerbian.Count().GetRatio(idomain.Words).ClipToK();
            Page_recall = relevantPageCount.GetRatio(wRecord.pageRecallTarget).ClipToK();

            i_pi_nominal = i_pi_nominal.GetRatio(loadedPageCount);
            i_pi_harvest = IP.GetRatio((double)lCount);



            spiderTaskResult lastResult = null;
            foreach (spiderTaskResult r in wRecord.spiderTaskResults)
            {
                lastResult = r;
                rCount = rCount + r.Count;
            }
            
            realLoadsCount = rCount;

            if (lastResult != null)
            {
                targetUrl = "";
                targetLanguage = "";
                targetEvalRatio = "";

                foreach (spiderTaskResultItem item in lastResult.items.Values)
                {
                    targetUrl = targetUrl.add(item.target.url, ",");

                    var t = wRecord.context.targets.GetByTarget(item.target);
                    if (t != null)
                    {
                        if (t.evaluation != null)
                        {
                            targetLanguage = targetLanguage.add(t.evaluatedLanguage.ToString(), ";");
                            targetEvalRatio = targetEvalRatio.add(t.evaluation.result_ratio.ToString(), ";");
                        } else
                        {
                            if (t.isDuplicate)
                            {
                                targetLanguage = targetLanguage.add("duplicate", ";");
                                targetEvalRatio = targetEvalRatio.add("duplicate", ";");
                            } else
                            {
                                targetLanguage = targetLanguage.add("unknown", ";");
                                targetEvalRatio = targetEvalRatio.add("unknown", ";");
                            }
                            
                        }
                    }
                }
            }

            if ((relevantPageCount == 0)||(loadedPageCount == 0))
            {
                E_PP = 0;
            }
            else
            {
                E_PP = (double)relevantPageCount / (double)loadedPageCount;
            }
            if ((wRecord.context.targets.termSerbian.Count == 0) || (wRecord.context.targets.termsAll.Count==0) || (loadedPageCount==0))
            {
                E_TP = 0;
                E_TH = 0;
            } else
            {
                E_TP = (double)wRecord.context.targets.termSerbian.Count / (double)wRecord.context.targets.termsAll.Count;
                E_TH = (double)wRecord.context.targets.termSerbian.Count / (double)loadedPageCount;
            }

            IPnominal = i_pi_nominal;
            IP_collected = i_pi_harvest;
            
            Lm_collected = i_lm_harvest;
            Lm_recall = i_lm_recall;

        }
    }

}