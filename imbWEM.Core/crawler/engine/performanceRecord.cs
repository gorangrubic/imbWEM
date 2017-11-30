// --------------------------------------------------------------------------------------------------------------------
// <copyright file="performanceRecord.cs" company="imbVeles" >
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
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.contentBlock;
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
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.fields;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins;
    using imbWEM.Core.stage;

    /// <summary>
    /// Keeping performance record of the crawler
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class performanceRecord :imbBindable, IReportBenchmark
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




        public performanceRecord()
        {

        }

        public performanceRecord(string __crawlerName)
        {
            crawlerName = __crawlerName;
        }



        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="insertValues">if set to <c>true</c> [insert values].</param>
        /// <returns></returns>
        public DataTable GetDataTable(bool insertValues, string nameOverride="")
        {
            
            DataTable performanceTable = new DataTable("perf_" + folderName);
            if (!nameOverride.isNullOrEmpty())
            {
                performanceTable.SetTitle(nameOverride);
            }

            // performanceTable.Add(nameof(performanceRecord.crawlerName));
            performanceTable.AddColumns(typeof(performanceRecord), nameof(crawlerName),
            nameof(domainsLoaded), nameof(jobTimeInMinutes), nameof(loadTotal),
            nameof(loadMbPerMinute), nameof(domainCrashed), nameof(pageLoads), nameof(pageLoadsReal),
            nameof(timePerDomain), nameof(pageLoadsPerDomain), nameof(termsPerPageLoads),
            nameof(blocksRecovered), nameof(blocksPerPageLoads), nameof(cpuAverage),
            nameof(termsRecoveredSerbian), nameof(blocksRecoveredSerbian),
             nameof(relevantPageLoads), nameof(relevantPagePerDomain), nameof(relevantVsLoadedAverage),
             nameof(termsRecoveredAll), nameof(termsRecoveredOther), nameof(domainCrashedList),
             nameof(pageLoadByIterationRecord), nameof(pageLoadDuplicate), nameof(FRA_TimePercent), nameof(ContentProcessor_TimePercent), nameof(IterationTimeAvg), nameof(Iterations));
            
            if (insertValues)
            {
                performanceTable.AddObject(this);
            }

            return performanceTable;
        }




        public DataRow SetDataRow(DataTable table, DataRow row = null)
        {
            bool insertNew = false;
            if (row == null)
            {
                insertNew = true;
            }

            if (insertNew) row = table.NewRow();


            row.SetData(this);

            if (insertNew) table.Rows.Add(row);
            return row;
        }


        public void deployRelatedValues(TimeSpan dataLoadSpan, int pageLoadOverride = -1)
        {
           
        }

        //public void deploy(performanceDataLoad dataLoadTaker, performanceCpu cpuTaker, Int32 pageLoadOverride = -1)
        //{
            



        //}

        public void deploy(modelSpiderTestRecord tRecord=null)
        {

            List<string> __termsSerbian = new List<string>();
            List<string> __termsOther = new List<string>();
            List<string> __termsAll = new List<string>();

            List<string> __blocksAll = new List<string>();
           // List<String> __blocksSerbianAll = new List<string>();

            domainCrashedList = tRecord.crashedDomains.ToList().toCsvInLine(",");
            domainCrashed = tRecord.crashedDomains.Count();
            int __pageLoadByIterationRecord = 0;
            int __pageLoadByTargets = 0;
            int __pageLoadDuplicate = 0;

            double __FRATimeAvgSum = 0;
            double __ITETimeAvgSum = 0;
            int __FRATimeTakes = 0;
            int __iterations = 0;

            foreach (var wRecord in tRecord.GetChildRecords())
            {

                double __FRATimeSum = 0;
                double __ITETimeSum = 0;

                __termsSerbian.AddRange(wRecord.context.targets.termSerbian);
                __termsOther.AddRange(wRecord.context.targets.termOther);
                __termsAll.AddRange(wRecord.context.targets.termsAll);

                __pageLoadByTargets += wRecord.context.targets.GetLoaded().Count();

                var lastIter = wRecord.iterationTableRecord.GetLastEntry();
                if (lastIter != null) __pageLoadByIterationRecord += lastIter.loadedPageCount;

                __pageLoadDuplicate += wRecord.duplicateCount;

                foreach (nodeBlock nb in wRecord.context.targets.blocks.GetBlockList())
                {
                    __blocksAll.AddUnique(nb.textHash);
                    
                    //if (nb.isSerbianContent)
                    //{
                    //    __blocksSerbianAll.AddUnique(nb.textHash);
                    //}
                }

                int __itCount = 0;
                foreach (iterationPerformanceRecord iteration in wRecord.iterationTableRecord)
                {
                    __FRATimeSum += iteration.FRA_SummaryRuntime;
                    __ITETimeSum += iteration.time_duration_s;
                    __itCount++;
                    __iterations++;
                }


                __FRATimeSum = __FRATimeSum / (double) __itCount;
                __ITETimeSum = __ITETimeSum / (double)__itCount;

                __FRATimeAvgSum += __FRATimeSum;
                __ITETimeAvgSum += __ITETimeSum;
                __FRATimeTakes++;
            }

            Iterations = __iterations;

            __FRATimeAvgSum = __FRATimeAvgSum / (double)__FRATimeTakes;
            __ITETimeAvgSum = __ITETimeAvgSum / (double)__FRATimeTakes;

            FRA_TimePercent = __FRATimeAvgSum / __ITETimeAvgSum;

            double __noFRATimeAvgSum = __ITETimeAvgSum - __FRATimeAvgSum;

            ContentProcessor_TimePercent = __noFRATimeAvgSum / __ITETimeAvgSum;

            IterationTimeAvg = __ITETimeAvgSum;

            termsRecoveredAll = __termsAll.Count();
            termsRecoveredOther = __termsOther.Count();
            termsRecoveredSerbian = __termsSerbian.Count();

            blocksRecovered = __blocksAll.Count();
          //  blocksRecoveredSerbian = __blocksSerbianAll.Count();
            //tRecord.allTerms = termsRecoveredAll;



            TimeSpan timeSpan = tRecord.cpuTaker.GetTimeSpan();
            jobTimeInMinutes = timeSpan.TotalMinutes;


            relevantPageLoads = tRecord.relevantPages.Count;

            //pageLoads = tRecord.allUrls.Count;

            pageLoads = __pageLoadByTargets;
            pageLoadDuplicate = __pageLoadDuplicate;
            pageLoadByIterationRecord = __pageLoadByIterationRecord;

            pageLoadsReal = tRecord.dataLoadTaker.pageLoads;

            domainsLoaded = tRecord.aRecord.sample.Count();
            
            cpuAverage = tRecord.cpuTaker.GetAverage();


            loadTotal = tRecord.dataLoadTaker.GetLastTake().reading;  //Convert.ToUInt64(dataLoadTaker.lastTake.reading);
            loadAverage = loadTotal / (double)timeSpan.TotalMinutes;




            double loadTotalKb = loadTotal / (double)1048576;

            if (domainsLoaded == 0)
            {
                //new aceGeneralException(nameof(domainsLoaded) + " is zero", null, this, nameof(performanceRecord) + " error in " + nameof(deploy) + " method.");

                return;
            }

            loadMbPerMinute = (double)loadTotalKb / (double)timeSpan.TotalMinutes;

            dataLoadPerDomain = loadTotal / (double)domainsLoaded;

            pageLoadsPerDomain = (double)pageLoads /(double) domainsLoaded;



            if (relevantPageLoads == 0)
            {
                relevantPagePerDomain = 0;
                relevantVsLoadedAverage = 0;
            }
            else
            {
                relevantPagePerDomain = (double)relevantPageLoads / (double)domainsLoaded;

            }

            if (pageLoads > 0)
            {
                relevantVsLoadedAverage = ((double)relevantPageLoads / (double)pageLoads);
            }


            timePerDomain = jobTimeInMinutes / ((double)domainsLoaded);

            termsPerPageLoads = ((double)termsRecoveredAll) / ((double)pageLoads);
            blocksPerPageLoads = ((double)blocksRecovered) / ((double)pageLoads);

        }

        public int domainCrashed { get; set; } = 0;
        public string domainCrashedList { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public double pageLoadsPerDomain { get; set; }


        /// <summary>
        /// Relevant page loads
        /// </summary>
        public int relevantPageLoads { get; set; }


        /// <summary>
        /// Number of relevant pages per domain
        /// </summary>
        public double relevantPagePerDomain { get; set; }


        /// <summary>
        /// Average of all relevant/loaded average ratios
        /// </summary>
        public double relevantVsLoadedAverage { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string folderName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string crawlerName { get; set; } = "";


        /// <summary>
        /// PL_all
        /// </summary>
        public int pageLoads { get; set; }


        /// <summary>
        /// C_bl_dc
        /// </summary>
        public int blocksRecovered { get; set; }


        /// <summary>
        /// W_dc
        /// </summary>
        public int termsRecovered { get; set; }


        public int termsRecoveredOther { get; set; }

        public int termsRecoveredAll { get; set; }


        private int _termsRecoveredSerbian;
        /// <summary> </summary>
        public int termsRecoveredSerbian
        {
            get
            {
                return _termsRecoveredSerbian;
            }
            set
            {
                _termsRecoveredSerbian = value;
                OnPropertyChanged("termsRecoveredSerbian");
            }
        }


        private int _blocksRecoveredSerbian;
        /// <summary> </summary>
        public int blocksRecoveredSerbian
        {
            get
            {
                return _blocksRecoveredSerbian;
            }
            set
            {
                _blocksRecoveredSerbian = value;
                OnPropertyChanged("blocksRecoveredSerbian");
            }
        }


        /// <summary>
        /// S_dc
        /// </summary>
        public int domainsLoaded { get; set; }


        /// <summary>
        /// T_job
        /// </summary>
        public double jobTimeInMinutes { get; set; }


        /// <summary>
        /// R_c-ptd
        /// </summary>
        public double timePerDomain { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double dataLoadPerDomain { get; set; }


        /// <summary>
        /// E_TP
        /// </summary>
        public double termsPerPageLoads { get; set; }


        /// <summary>
        /// E_BLP
        /// </summary>
        public double blocksPerPageLoads { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double loadTotal { get; set; }


        /// <summary>
        /// In kilobytes
        /// </summary>
        public double loadMbPerMinute { get; set; }


        /// <summary>
        /// R_e_cpu
        /// </summary>
        public double cpuAverage { get; set; }


        /// <summary>
        /// R_e_link
        /// </summary>
        public double loadAverage { get; set; }

        public int pageLoadsReal { get; set; }

        public int pageLoadByIterationRecord { get; set; }

        public int pageLoadDuplicate { get; set; }


        ///// <summary> Total time consumed by FRA part of the iteration cycle </summary>
        //[Category("Time")]
        //[DisplayName("FRA_Time")]
        //[imb(imbAttributeName.measure_letter, "T_fra")]
        //[imb(imbAttributeName.measure_setUnit, "s")]
        //[Description("Total time consumed by FRA part of the iteration cycle")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]	

        //public Double FRA_SummaryRuntime { get; set; }


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
        [Description("Percentage of time that FRA participated in the complete execution time (including all parallel threads, time per single thread)")]
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



        /// <summary> Ratio </summary>
        [Category("Time")]
        [DisplayName("ContentProcessor Time Ratio")]
        [imb(imbAttributeName.measure_letter, "T_cp")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Percentage of iteration time that was consumed by Content Processor component and Link Resolver")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double ContentProcessor_TimePercent { get; set; } = default(double);



        /// <summary> Ratio </summary>
        [Category("Time")]
        [DisplayName("Iteration Time Summary")]
        [imb(imbAttributeName.measure_letter, "It_avg")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        [Description("Unfolded average duration of iterations (single thread)")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double IterationTimeAvg { get; set; } = default(double);




        /// <summary> Complete count of all iterations </summary>
        [Category("Count")]
        [DisplayName("Iterations")]
        [imb(imbAttributeName.measure_letter, "I_n")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Complete count of all iterations")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Iterations { get; set; } = default(int);


    }

}