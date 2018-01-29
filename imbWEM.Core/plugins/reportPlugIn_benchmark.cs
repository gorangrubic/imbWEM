// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_benchmark.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math.aggregation;
    using imbSCI.Core.reporting;
    using imbSCI.Core.reporting.render.builders;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.operations;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    public class reportPlugIn_benchmark : reportPlugIn_withRecord<reportPlugIn_benchmarkResults>
    {
        public reportPlugIn_benchmark() : base("BenchmarkReporter", "Plugin extracts the most relevant metrics and provides comparison between different tests")
        {
        }

        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { crawlReportingStageEnum.crawler, crawlReportingStageEnum.finish, crawlReportingStageEnum.statusReport, crawlReportingStageEnum.start,
                    crawlReportingStageEnum.domain, crawlReportingStageEnum.iteration, crawlReportingStageEnum.init};
            }
        }

        protected override string __homePath
        {
            get
            {
                return "index\\benchmark";
            }
        }

        protected override string __recordKeyProperty
        {
            get
            {
                return "TestID";
            }
        }


        public string recordFileName { get; set; } = "default.xml";

        protected string recordPath { get
            {
                return "index\\benchmark\\results.xml";
            }
        }

        public folderNode crawlerReportFolder { get; set; }



        public override void Dispose()
        {
           // base.Dispose();
        }

        public override void eventCrawlJobFinished(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {
            crawlerReportFolder = __machine.reporter.folder;

            var wRecords = tRecord.GetChildRecords();
            string fileprefix = tRecord.instance.name.getFilename();

            List<DataTable> iterationTimelines = new List<DataTable>();

            foreach (var wRecord in wRecords)
            {
                iterationTimelines.Add(wRecord.iterationTableRecord.GetDataTable());
            }
            int dlc_c = 0;

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {

                tRecord.frontierDLCDataTables[moduleIterationRecordSummary.fra_overview].GetAggregatedTable("fra_overview").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_overview".add(fileprefix, "_"), true);

                tRecord.frontierDLCDataTables[moduleIterationRecordSummary.all].GetAggregatedTable("fra_modules_all").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_modules_all".add(fileprefix, "_"), true);


                if (tRecord.frontierDLCDataTables[moduleIterationRecordSummary.language].Any())
                {

                    tRecord.frontierDLCDataTables[moduleIterationRecordSummary.language].GetAggregatedTable("fra_module_language").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_module_language_".add(fileprefix, "_"), true);
                }
                if (tRecord.frontierDLCDataTables[moduleIterationRecordSummary.structure].Any())
                {

                    tRecord.frontierDLCDataTables[moduleIterationRecordSummary.structure].GetAggregatedTable("fra_modules_structure").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_module_structure_".add(fileprefix, "_"), true);
                }
                if (tRecord.frontierDLCDataTables[moduleIterationRecordSummary.template].Any())
                {

                    tRecord.frontierDLCDataTables[moduleIterationRecordSummary.template].GetAggregatedTable("fra_modules_template").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_module_template".add(fileprefix, "_"), true);
                }
                if (tRecord.frontierDLCDataTables[moduleIterationRecordSummary.diversity].Any())
                {
                    tRecord.frontierDLCDataTables[moduleIterationRecordSummary.diversity].GetAggregatedTable("fra_module_diversity").GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_module_diversity_".add(fileprefix, "_"), true);
                }


                string finalOverviewPath = crawlerReportFolder.pathFor("fra_modules_impact".add(fileprefix, "_"), getWritableFileMode.newOrExisting);
                objectTable<moduleFinalOverview> finalOverview = new objectTable<moduleFinalOverview>(finalOverviewPath, false, "ModuleName", "module_impact");
                finalOverview.description = "Aggregate (DLC and iterations) metrics on modules' impact to the result.";

                aceDictionarySet<moduleIterationRecordSummary, moduleIterationRecord> moduleIterationsByModule = new aceDictionarySet<moduleIterationRecordSummary, moduleIterationRecord>();
                List<moduleIterationRecordSummary> moduleActive = new List<moduleIterationRecordSummary>();
                
                foreach (var wRecord in wRecords)
                {
                    dlc_c++;
                    foreach (var pair in wRecord.frontierDLC.modRecords)
                    {
                        moduleIterationsByModule.Add(pair.Value.moduleSummaryEnum, pair.Value.GetList());
                        if (!moduleActive.Contains(pair.Value.moduleSummaryEnum)) moduleActive.Add(pair.Value.moduleSummaryEnum);
                    }
                }

                int modC = 0;
                List<moduleFinalOverview> modList = new List<moduleFinalOverview>();
                foreach (var modType in moduleActive)
                {
                    moduleFinalOverview mfo = new moduleFinalOverview();
                    mfo.deploy(tRecord.instance.name, modType, moduleIterationsByModule[modType], dlc_c);
                    modC += moduleIterationsByModule[modType].Count;
                    finalOverview.AddOrUpdate(mfo);
                    modList.Add(mfo);
                }

                moduleFinalOverview mfoSum = new moduleFinalOverview();

                
                mfoSum.deploySum(tRecord.instance.name, modList);
                finalOverview.AddOrUpdate(mfoSum);

                foreach (var mfo in modList)
                {
                    mfo.SetTestIDAndSignature(tRecord.instance, imbWEMManager.index.experimentEntry.state, tRecord);
                    finalOverview.AddOrUpdate(mfo);

                    record_moduleImpact.AddOrUpdate(mfo);
                }


                mfoSum.SetTestIDAndSignature(tRecord.instance, imbWEMManager.index.experimentEntry.state, tRecord);
                record_moduleImpact.AddOrUpdate(mfoSum);

            //    finalOverview.SaveAs(finalOverviewPath.add(".xml"));
                DataTable fover = finalOverview.GetDataTable(null, mfoSum.Crawler);

                fover.SetAggregationOriginCount(modC);
                fover.SetAggregationAspect(dataPointAggregationAspect.onTableMultiRow);
                fover.GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "fra_modules_impact_overview", true);

                
                record_moduleImpact.Save();
                var midt = record_moduleImpact.GetDataTable(null, "Module impacts");
                midt.AddExtra("The last benchmark metrics entry [" + imbWEMManager.index.experimentEntry.CrawlID + "] inserted on " + DateTime.Now.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString());
                midt.GetReportAndSave(imbWEMManager.index.experimentEntry.sessionReportFolder, imbWEMManager.authorNotation, "fra_modules_impact_".add(fileprefix, "_"));
                

            } else
            {
                dlc_c = tRecord.children.Count();
            }

            if (iterationTimelines.Any())
            {
                DataTable crawlTimeline = iterationTimelines.GetAggregatedTable("Crawler_Timeline", dataPointAggregationAspect.overlapMultiTable);
                crawlTimeline.SetDescription("Iteration-synced aggregated performance timeline using DLC records [" + wRecords.Count + "] domains.");
                crawlTimeline.GetReportAndSave(imbWEMManager.index.experimentEntry.sessionCrawlerFolder, imbWEMManager.authorNotation, "timeline_performance_".add(imbWEMManager.index.experimentEntry.Crawler));
            }
            //String atl = "timeline_performance".add(tRecord.instance.name, "_").add("xml", ".");

            var domainPerfList = tRecord.lastDomainIterationTable.GetList();

            var benchmark = new reportPlugIn_benchmarkResults(); //records.GetOrCreate(imbWEMManager.index.experimentEntry.TestID);

            tRecord.performance.SetTestIDAndSignature(tRecord.instance, imbWEMManager.index.experimentEntry.state, tRecord);

            tRecord.performance.jobTimeInMinutes = tRecord.cpuTaker.GetTimeSpanInMinutes();

            record_performances.AddOrUpdate(tRecord.performance);

            benchmark.SetTestIDAndSignature(tRecord.instance, imbWEMManager.index.experimentEntry.state, tRecord);


            benchmark.CrawlTime = tRecord.cpuTaker.GetTimeSpanInMinutes(); //tRecord.cpuTaker.GetTimeSpan().TotalMinutes; //.timeFinish.Subtract(tRecord.timeStart).TotalMinutes;


            benchmark.IP = domainPerfList.Average(x => x.IP);
            benchmark.IPnominal = domainPerfList.Average(x => x.IPnominal);
            benchmark.IP_collected = domainPerfList.Average(x => x.IP_collected);
            benchmark.Lm_collected = domainPerfList.Average(x => x.Lm_collected);
            benchmark.Lm_recall = domainPerfList.Average(x => x.Lm_recall);
            benchmark.E_PP = domainPerfList.Average(x => x.E_PP);
            benchmark.E_TP = domainPerfList.Average(x => x.E_TP);
            benchmark.IP_recall = domainPerfList.Average(x => x.IP_recall);
            benchmark.Page_recall = domainPerfList.Average(x => x.Page_recall);
            benchmark.Term_recall = domainPerfList.Average(x => x.Term_recall);

            var resourcesamples = tRecord.measureTaker.GetLastSamples(1000);
            var lastsample = tRecord.measureTaker.GetLastTake();

            benchmark.DataLoad = lastsample.bytesLoadedTotal / benchmark.CrawlTime;
            benchmark.CPU = resourcesamples.Average(x => x.cpuRateOfProcess);
            benchmark.RAM = resourcesamples.Average(x => x.physicalMemory);

            records.AddOrUpdate(benchmark);
            records.Save();

            var dt = records.GetDataTable(null,imbWEMManager.index.experimentEntry.CrawlID);

            dt.AddExtra("The last benchmark metrics entry [" + benchmark.Crawler + "] inserted on " + DateTime.Now.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString());

            dt.SetAdditionalInfoEntry("DLC Threads - TC", __machine.maxThreads);
            dt.SetAdditionalInfoEntry("LoadTake - LT", tRecord.instance.settings.limitIterationNewLinks);
            dt.SetAdditionalInfoEntry("PageLoads - PL", tRecord.instance.settings.limitTotalPageLoad);
            dt.SetAdditionalInfoEntry("Sample size - DC", dlc_c);
            dt.SetAdditionalInfoEntry("Session ID", imbWEMManager.index.experimentEntry.SessionID);
            


            dt.GetReportAndSave(crawlerReportFolder, imbWEMManager.authorNotation, "result", true);

            benchmark.GetUserManualSaved(crawlerReportFolder.pathFor("crawler\\result.txt"));

            //  crawlTimeline.saveObjectToXML(homeFolder.pathFor(atl));
            //  crawlTimeline.saveObjectToXML(reportFolder.pathFor(atl));

            // all three modules summary

            imbWEMManager.settings.directReportEngine.GetUserManualSaved(crawlerReportFolder["crawler"].pathFor("settings_reportEngine.txt"));
            imbWEMManager.settings.crawlerJobEngine.GetUserManualSaved(crawlerReportFolder["crawler"].pathFor("settings_crawlJobEngine.txt"));
            imbWEMManager.settings.executionLog.GetUserManualSaved(crawlerReportFolder["crawler"].pathFor("settings_executionLogs.txt"));

            tRecord.instance.settings.GetUserManualSaved(crawlerReportFolder["crawler"].pathFor("settings_crawler.txt"));
            record_performances.Save();
            var perfDT = record_performances.GetDataTable(null, imbWEMManager.index.experimentEntry.CrawlID);
            perfDT.AddExtra("The last benchmark metrics entry [" + benchmark.Crawler + "] inserted on " + DateTime.Now.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString());

            perfDT.GetReportAndSave(imbWEMManager.index.experimentEntry.sessionReportFolder, imbWEMManager.authorNotation, "crawl_performances", true);

            

        }


        public override void eventStatusReport(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            //loger.log(tRecord.)

        }
      
        public override void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {

        }

        public override void eventDLCInitiated(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            if (imbWEMManager.settings.directReportEngine.doDomainReport)
            {
                string dlc_config = imbWEMManager.index.experimentEntry.sessionCrawlerFolder["sites"].pathFor("dlc_config_" + __wRecord.domainInfo.domainRootName.getFilename(".txt"));
                builderForMarkdown builder = new builderForMarkdown();
                spiderTools.Describe(__task.evaluator, builder);
                builder.ToString().saveStringToFile(dlc_config);
            }

            
            

        }

        public override void eventDLCFinished(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            
        }



        public override void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord)
        {
           
        }


        public objectTable<performanceRecord> record_performances { get; protected set; }

        public objectTable<moduleFinalOverview> record_moduleImpact { get; protected set; }


        public override void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {

            crawlerReportFolder = __machine.reporter.folder;

        }

        /// <summary>
        /// Just before 
        /// </summary>
        /// <param name="crawlerDomainTaskMachine">The crawler domain task machine.</param>
        /// <param name="tRecord">The t record.</param>
        public override void eventAtEndOfCrawlJob(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {

        }


        public override void eventPluginInstalled()
        {
           // records = new objectTable<reportPlugIn_benchmarkResults>(recordPath, true, __recordKeyProperty, name);
          //  records.description = "Summary report on the most relevant evaluation metrics.";
           reportFolder = imbWEMManager.index.experimentEntry.sessionReportFolder;

           
            string recordName = imbWEMManager.index.experimentEntry.SessionID.getFilename();

            records = new objectTable<reportPlugIn_benchmarkResults>(homeFolder.pathFor(recordName.add("results", "_")), true, __recordKeyProperty, name);
            records.description = "Summary report on the most relevant evaluation metrics.";


            record_performances = new objectTable<performanceRecord>(homeFolder.pathFor(recordName.add("performances", "_")), true, "TestID", name);

            record_moduleImpact = new objectTable<moduleFinalOverview>(homeFolder.pathFor(recordName.add("modules", "_")), true, "ModuleName", name);

        }


        /// <summary>
        /// It is called when the software starts to shutdown
        /// </summary>
        public override void onExit()
        {

        }

        public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord) => eventCrawlJobFinished(__machine, __tRecord);

        public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as directReporterBase, __task, __wRecord);
        

        public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as directReporterBase, __task, __wRecord);
       
    }

}