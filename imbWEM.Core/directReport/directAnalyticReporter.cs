// --------------------------------------------------------------------------------------------------------------------
// <copyright file="directAnalyticReporter.cs" company="imbVeles" >
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
namespace imbWEM.Core.directReport
{
    using System;
    using System.Collections.Concurrent;
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
    using imbSCI.Core.data;
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
    using imbSCI.DataComplex.extensions.data.operations;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.directReport.data;
    using imbWEM.Core.directReport.enums;
    using imbWEM.Core.stage;
    using imbSCI.DataComplex.tables;
    using imbSCI.Core.files;
    using imbSCI.Core.math.aggregation;
    using imbWEM.Core.crawler.core;
    using imbSCI.Core.reporting.render.builders;

    public class directAnalyticReporter : directReporterBase
    {
        public directAnalyticReporter(string reportName, folderNode reportRootDir,  aceAuthorNotation __notation) : base(reportName, reportRootDir, __notation)
        {
           if (imbWEMManager.settings.directReportEngine.doDomainReport) siteRecords = new folderNodeForInstances<modelSpiderSiteRecord>(folder[DRFolderEnum.sites], getSiteName);
            if (imbWEMManager.settings.directReportEngine.doIterationReport) iterationRecords = new folderNodeForInstances<int>(folder[DRFolderEnum.it], getIterationName);

            REPORT_DOMAIN_PAGES = imbWEMManager.settings.directReportEngine.DR_ReportDomainPages;
            REPORT_DOMAIN_TERMS = imbWEMManager.settings.directReportEngine.DR_ReportDomainTerms;
            REPORT_ITERATION_TERMS = imbWEMManager.settings.directReportEngine.DR_ReportIterationTerms;
            REPORT_ITERATION_URLS = imbWEMManager.settings.directReportEngine.DR_ReportIterationUrls;
            REPORT_WRECORD_LOG = imbWEMManager.settings.directReportEngine.DR_ReportWRecordLog;
            REPORT_TIMELINE = imbWEMManager.settings.directReportEngine.DR_ReportTimeline;

            REPORT_MODULES = imbWEMManager.settings.directReportEngine.DR_ReportModules;


            aceLog.consoleControl.setLogFileWriter(folder[DRFolderEnum.logs].pathFor("log.txt"));
            /*
            TextWriter logOut = File.CreateText();
            aceLog.consoleControl.logWritter = logOut;
            aceLog.consoleControl.logFileWriteOn = true;
            */
        }


        public string getSiteName(modelSpiderSiteRecord instance)
        {
            return instance.domainInfo.domainRootName;
        }

        public string getIterationName(int iteration)
        {
            return iteration.ToString("D3");
        }


        /// <summary> </summary>
        public folderNodeForInstances<modelSpiderSiteRecord> siteRecords { get; protected set; }


        /// <summary> </summary>
        public folderNodeForInstances<int> iterationRecords { get; protected set; }


        /// <summary> </summary>
        public iterationMatrix urlsLoaded { get; protected set; } = new iterationMatrix();


        /// <summary> </summary>
        public iterationMatrix urlsDetected { get; protected set; } = new iterationMatrix();


        public iterationMatrix SerbianPages { get; } = new iterationMatrix();


        /// <summary> </summary>
        public iterationMatrix termsExtracted { get; protected set; } = new iterationMatrix();


        /// <summary> </summary>
        public iterationMatrix serbianTermsExtracted { get; protected set; } = new iterationMatrix();


        /// <summary> </summary>
        public iterationMatrix serbianBlocksExtracted { get; protected set; } = new iterationMatrix();


        /// <summary> </summary>
        public iterationMatrix sentencesExtracted { get; protected set; } = new iterationMatrix();


        public override void deployCustomFolders()
        {
            if (imbWEMManager.settings.directReportEngine.DR_ReportIterationUrls || imbWEMManager.settings.directReportEngine.DR_ReportTimeline)  folder.Add(DRFolderEnum.it, "Iterations", @"*  contains *dt_iteration_[hostname].xlsx* for each DLC
* the spreadsheet contains iterationPerformanceRecord data for all iterations of[hostname] domain
* domain level crawl iteration timeline spreadsheet files");
            if (imbWEMManager.settings.directReportEngine.DR_ReportDomainTerms || imbWEMManager.settings.directReportEngine.DR_ReportIterationTerms) folder.Add(DRFolderEnum.terms, "Terms", @"TF-IDF tables and other records on extracted terms and tokens. *  additional reports on extracted relevant and irrelevant terms and blocks
* *[hostname]_blocks.txt *
> list of unique content block Md5 hash codes
* *[hostname]_terms.txt *
> list of distinct term tokens extracted from crawled pages");
            if (imbWEMManager.settings.directReportEngine.doDomainReport || imbWEMManager.settings.directReportEngine.DR_ReportDomainPages 
                || imbWEMManager.settings.directReportEngine.DR_ReportDomainTerms || imbWEMManager.settings.directReportEngine.DR_ReportModules_XMLIteration
                || imbWEMManager.settings.directReportEngine.DR_ReportModules_DomainReports || imbWEMManager.settings.directReportEngine.DR_ReportIterationUrls 
                || imbWEMManager.settings.directReportEngine.DR_ReportWRecordLog || imbWEMManager.settings.directReportEngine.DR_ReportIterationUrls) folder.Add(DRFolderEnum.sites, "Sites", "Records on crawled domains");

           // folder.Add(DRFolderEnum.pages, "Crawled pages", "Records on crawled pages");
            folder.Add(DRFolderEnum.crawler, "Crawler", @"Information on crawler that was ran. 
Aggregate files describing performance of the crawler in general.

### /crawler 
> resource utilization time series

#### /crawler/dt_cpu_[Report name].csv		[M]
* all sample takes on total CPU usage on the system

#### /crawler/dt_data_[Report name].csv		[M]
* all sample takes on processed HTML source bytes

#### /crawler/dt_resource_[Report name].xlsx	[O]

* all sample takes on multiple computing resources data points

> CPU % for the process only(max: 800 % because of 8 CPU cores), memory allocation, disk write bytes...




### /crawler
* Crawl performance series

### /crawler/dt_timeline_performance_[Report name].xlsx
* spreadsheet with aggregate(crawl) performances aligned by iterations

### /crawler/dt_timeline_[Report name].xlsx
* additional information on crawl dynamics, many data points are obsolete(not used) in the current design



### /crawler/performance.xml 					
> XML serialized instance of crawlerPerformance object

* contains the same data as shown in the summary spreadsheet and some additional information
* generated to support automatic post - processing of the report

### /crawler/urls_relevant_loaded.txt			[R]
* all urls of crawled relevant pages
");
        }

      

        private string _iterationDescription;
        /// <summary>
        /// 
        /// </summary>
        public string iterationDescription
        {
            get {
                if (_iterationDescription == null)
                {
                    _iterationDescription = "Meaning of file sufixes: _dt = detected targets, _ld = loaded targets, _blc = block content md5() hash list.";
                }
                return _iterationDescription;
            }
            set { _iterationDescription = value; }
        }


        private string _pageDescription;
        /// <summary>
        /// 
        /// </summary>
        public string pageDescription
        {
            get {
                if (_pageDescription == null)
                {
                    _pageDescription = "The contained files: content.txt - extracted textual content of the page, links.txt - extracted links on the page.";
                }
                return _pageDescription;
            }
            set { _pageDescription = value; }
        }


        public folderNode getIterationFolder(int iteration, modelSpiderSiteRecord wRecord)
        {
            if (siteRecords != null)
            {
                return siteRecords[wRecord].Add("I" + iteration.ToString("D3"), wRecord.domainInfo.domainRootName + iteration.ToString("D3"), "Iteration " + iteration + " on domain: " + wRecord.domainInfo.domainName + ". " + iterationDescription);
            } else
            {
                return null;
            }
        }

        public bool REPORT_ITERATION_TERMS { get; set; } = false;
        public bool REPORT_ITERATION_URLS { get; set; } = false;

        public bool REPORT_DOMAIN_TERMS { get; set; } = true;
        public bool REPORT_DOMAIN_PAGES { get; set; } = true;

        public bool REPORT_TIMELINE { get; set; } = true;
        public bool REPORT_WRECORD_LOG { get; set; } = true;
        public bool REPORT_MODULES { get; set; } = true;

        public void reportIteration(dataUnitSpiderIteration dataUnit, modelSpiderSiteRecord wRecord, ISpiderEvaluatorBase evaluator)
        {
            iterationPerformanceRecord ip_record = new iterationPerformanceRecord(wRecord);

            wRecord.iterationTableRecord.Add(ip_record);


            folderNode fn; //siteRecords[wRecord].Add(dataUnit.iteration.ToString("D3"), wRecord.domainInfo.domainRootName + dataUnit.iteration.ToString("D3"), "Iteration " + dataUnit.iteration + " on domain: " + wRecord.domainInfo.domainName);

           
            if (imbWEMManager.settings.directReportEngine.doIterationReport)
            {
                

                if (imbWEMManager.settings.directReportEngine.doDomainReport)
                {
                    fn = getIterationFolder(dataUnit.iteration, wRecord);
                    if (REPORT_WRECORD_LOG)
                    {
                        
                            
                            wRecord.logBuilder.getLastLine().saveStringToFile(fn.pathFor("wrecord.txt"));
                        
                    }

                    


                    string fileprefix = wRecord.domainInfo.domainRootName.getCleanFilepath();


                    textByIteration url_loaded = urlsLoaded[wRecord]; //.GetOrAdd(wRecord, new textByIteration());
                    textByIteration url_detected = urlsDetected[wRecord]; //, new textByIteration());
                                                                          //textByIteration terms_ext = termsExtracted[wRecord];
                                                                          //textByIteration sentence_ext = sentencesExtracted[wRecord];

                   

                    if (REPORT_MODULES)
                    {
                        if (imbWEMManager.settings.directReportEngine.DR_ReportModules_XMLIteration)
                        {
                            if (wRecord.tRecord.instance is spiderModularEvaluatorBase) wRecord.frontierDLC.reportIterationOut(wRecord, fn);
                        }
                    }

                    string its = dataUnit.iteration.ToString("D3");


                    //DataTable dt = wRecord.context.targets.GetDataTable();
                    //dt.SetTitle(fileprefix + "_targets");
                    //dt.serializeDataTable(aceCommonTypes.enums.dataTableExportEnum.csv, "", fn, notation);

                    //sentence_ext[dataUnit.iteration].AddRangeUnique(wRecord.context.targets.blocks.GetHashList());

                    //if (REPORT_ITERATION_TERMS)
                    //{
                    //    fileunit blocks = new fileunit(fn.pathFor(its + "_blc.txt"), false);


                    //    blocks.setContentLines(sentence_ext[dataUnit.iteration]);

                    //    blocks.Save();
                    //}

                    if (REPORT_TIMELINE)
                    {

                       

                        objectSerialization.saveObjectToXML(ip_record, fn.pathFor("performance.xml"));

                    }





                    if (REPORT_ITERATION_URLS)
                    {


                        if (wRecord.iteration > 0)
                        {
                            builderForMarkdown now_loaded = new builderForMarkdown();

                            //fileunit now_loaded = new fileunit(fn.pathFor(its + "_loadedNow.txt"), false);
                            List<spiderTarget> targets_loaded = wRecord.context.targets.GetLoadedInIteration(wRecord.iteration - 1);
                            
                            int tc = 0;
                            foreach (spiderTarget t in targets_loaded)
                            {
                                reportTarget(t, fn, tc);
                                now_loaded.AppendLine(t.url);
                                now_loaded.AppendHorizontalLine();
                                now_loaded.Append(t.marks.GetActiveResults());
                                now_loaded.AppendHorizontalLine();
                                now_loaded.Append(t.marks.GetPassiveResults());
                                now_loaded.AppendHorizontalLine();

                                var dt = t.marks.getHistory(t.url, wRecord.tRecord.instance.name);
                                dt.Save(fn, imbWEMManager.authorNotation, its + "_loadedNow");

                                now_loaded.AppendTable(dt, false);

                                tc++;
                            }

                            now_loaded.ToString().saveStringToFile(fn.pathFor(its + "_loadedNow.txt"));


                            spiderTaskResult loadResults = wRecord.spiderTaskResults[wRecord.iteration - 1];
                            loadResults.getDataTable().GetReportAndSave(fn, notation, "loadResults", true); // .serializeDataTable(aceCommonTypes.enums.dataTableExportEnum.excel, "loadResults", fn, notation);
                        }



                        fileunit detected = new fileunit(fn.pathFor(its + "_dt.txt"), false);
                        fileunit loaded = new fileunit(fn.pathFor(its + "_ld.txt"), false);

                        fileunit relp = new fileunit(fn.pathFor(its + "_srb_ld.txt"), false);
                        relp.Append(wRecord.relevantPages, true);

                        foreach (spiderTarget t in wRecord.context.targets)
                        {

                            if (t.page != null)
                            {
                                //t.contentBlocks.ForEach(x => sentence_ext[dataUnit.iteration].AddUnique(x.textHash));

                                loaded.Append(t.url);
                                url_loaded[dataUnit.iteration].Add(t.url);
                            }
                            else
                            {
                                detected.Append(t.url);
                                url_detected[dataUnit.iteration].Add(t.url);
                            }
                        }


                        string lineFormat = "{0,5} {1,30} [s:{1,6}]" + Environment.NewLine;

                        fileunit active = new fileunit(fn.pathFor(its + "_act.txt"), false);
                        int c = 1;

                        foreach (var lnk in wRecord.web.webActiveLinks)
                        {
                            active.Append(string.Format(lineFormat, c, lnk.url, lnk.marks.score));
                            active.Append(lnk.marks.GetLayerAssociation());
                            c++;
                        }


                        detected.Save();
                        loaded.Save();
                        active.Save();
                    }


                   
                }
            }

           
          

            wRecord.tRecord.instance.reportIteration(this, wRecord);
        }

        internal crawlerErrorLog CreateAndSaveError(Exception ex, modelSpiderSiteRecord wRecord, crawlerDomainTask crawlerDomainTask, crawlerErrorEnum errorType)
        {
            crawlerErrorLog clog = crawlerErrorLog.CreateAndSave(ex, wRecord, crawlerDomainTask, errorType);
            clog.SaveXML(folder[DRFolderEnum.logs].pathFor("DLC_crash_" + wRecord.domainInfo.domainRootName.getFilename()));
            return clog;
        }

        private void reportTarget(spiderTarget t, folderNode fn, int c)
        {
            string pageFolder = "P" + c.ToString("D3") + "_" + t.IsRelevant.ToString();
            folderNode pfn = fn.Add(pageFolder, "Page " + c.ToString(), "Report on page " + t.url + " crawled by " + name + ". Target.IsRelevant: " + t.IsRelevant +".".addLine(pageDescription));

            fileunit content = new fileunit(pfn.pathFor("content.txt"), false);
            fileunit links = new fileunit(pfn.pathFor("links.txt"), false);

            if (t.evaluation != null)
            {
                t.evaluation.saveObjectToXML(pfn.pathFor("relevance.xml"));
            }

            content.setContent(t.pageText);
            //t.page.relationship.outflowLinks
            if (t.page != null)
            {

                foreach (spiderLink ln in t.page.relationship.outflowLinks.items.Values)
                {
                    string rl = ln.url;

                    links.Append(ln.url);
                }

                //t.page.webpage.links.ForEach(x => links.Append(x.nature + " | " + x.name + " | " + x.url));
            }
            content.Save();
            links.Save();
            //  marks.Save();

        }

        /// <summary>
        /// Runs when a DLC is finished
        /// </summary>
        /// <param name="wRecord">The w record.</param>
        public void reportDomainFinished(modelSpiderSiteRecord wRecord)
        {
            folderNode fn = null;

            string fileprefix = wRecord.domainInfo.domainRootName.getCleanFilepath();

           

            if (imbWEMManager.settings.directReportEngine.doDomainReport)
            {

                fn = folder[DRFolderEnum.sites].Add(wRecord.domainInfo.domainRootName.getCleanFilepath(), "Report on " + wRecord.domainInfo.domainName, "Records on domain " + wRecord.domainInfo.domainName + " crawled by " + name);

                if (REPORT_DOMAIN_TERMS)
                {
                    if (wRecord.tRecord.instance.settings.doEnableDLC_TFIDF)
                    {
                        if (wRecord.context.targets.dlTargetPageTokens != null) wRecord.context.targets.dlTargetPageTokens.GetDataSet(true).serializeDataSet("token_ptkn", fn, dataTableExportEnum.excel, notation);
                    }
                    if (wRecord.context.targets.dlTargetLinkTokens != null) wRecord.context.targets.dlTargetLinkTokens.GetDataSet(true).serializeDataSet("token_ltkn", fn, dataTableExportEnum.excel, notation);
                }

                if (REPORT_DOMAIN_PAGES)
                {
                    int c = 1;
                    foreach (spiderTarget t in wRecord.context.targets.GetLoadedInOrderOfLoad())
                    {
                        reportTarget(t, fn, c);
                        c++;
                    }
                }

            


                fileunit wLog = new fileunit(folder[DRFolderEnum.logs].pathFor(fileprefix + ".txt"), false);
                wLog.setContent(wRecord.logBuilder.ContentToString(true));

                wLog.Save();


                if (REPORT_ITERATION_URLS)
                {

                    textByIteration url_loaded = urlsLoaded[wRecord]; //.GetOrAdd(wRecord, new textByIteration());
                    textByIteration url_detected = urlsDetected[wRecord]; //, new textByIteration());


                    fileunit url_ld_out = new fileunit(folder[DRFolderEnum.sites].pathFor(fileprefix + "_url_ld.txt"), false);
                    fileunit url_dt_out = new fileunit(folder[DRFolderEnum.sites].pathFor(fileprefix + "_url_dt.txt"), false);
                    fileunit url_srb_out = new fileunit(folder[DRFolderEnum.sites].pathFor(fileprefix + "_url_srb_ld.txt"), false);

                    url_ld_out.setContentLines(url_loaded.GetAllUnique());
                    url_dt_out.setContentLines(url_detected.GetAllUnique());
                    url_srb_out.setContentLines(wRecord.relevantPages);

                    url_ld_out.Save();
                    url_dt_out.Save();
                    url_srb_out.Save();

                }

                //terms_out.Save();
                //sentence_out.Save();


                
            }

            if (REPORT_MODULES)
            {
                if (wRecord.tRecord.instance is spiderModularEvaluatorBase) wRecord.frontierDLC.reportDomainOut(wRecord, fn, fileprefix);
            }

            if (REPORT_TIMELINE)
            {
                wRecord.iterationTableRecord.GetDataTable(null, "iteration_performace_" + fileprefix).GetReportAndSave(folder[DRFolderEnum.it], notation, "iteration_performace_" + fileprefix); //, notation);   
            }

            //if (REPORT_TIMELINE)
            //{
            //    DataTable dt = wRecord.GetTimeSeriesPerformance();
            //    timeSeries.Add(dt);
            //    dt.GetReportAndSave(folder[DRFolderEnum.it], notation, "iteration_frontier_stats_" + fileprefix);
            //}


            wRecord.tRecord.lastDomainIterationTable.Add(wRecord.iterationTableRecord.GetLastEntryTouched());

            wRecord.tRecord.instance.reportDomainFinished(this, wRecord);

            wRecord.Dispose();


            
        }


        private ConcurrentBag<DataTable> _timeSeries = new ConcurrentBag<DataTable>();
        /// <summary> </summary>
        public ConcurrentBag<DataTable> timeSeries
        {
            get
            {
                return _timeSeries;
            }
            protected set
            {
                _timeSeries = value;
                OnPropertyChanged("timeSeries");
            }
        }



        public void reportCrawler(modelSpiderTestRecord tRecord)
        {
            folderNode fn = folder[DRFolderEnum.crawler];

            string fileprefix = tRecord.instance.name.getCleanFilePath(); //tRecord.name.getCleanFilepath();

            if (REPORT_TIMELINE)
            {
                DataTable timeline = timeSeries.GetAggregatedTable("frontier_stats", dataPointAggregationAspect.overlapMultiTable); //.GetSumTable("timeline_" + fileprefix.Replace(" ", ""));
                timeline.GetReportAndSave(folder[DRFolderEnum.crawler], notation, "frontier_stats" + fileprefix);
            }


            if (REPORT_ITERATION_URLS)
            {
                tRecord.allUrls = urlsLoaded.GetAllUnique();
                tRecord.allDetectedUrls = urlsDetected.GetAllUnique();

                saveOutput(tRecord.allDetectedUrls, folder[DRFolderEnum.crawler].pathFor("urls_detected.txt"));
                saveOutput(tRecord.allUrls, folder[DRFolderEnum.crawler].pathFor("urls_loaded.txt"));
                saveOutput(tRecord.relevantPages, folder[DRFolderEnum.crawler].pathFor("urls_relevant_loaded.txt"));
            }
            //    Int32 iterations = tRecord.instance.settings.limitIterations;


            DataTable cpuTable = tRecord.cpuTaker.GetDataTableBase("cpuMetrics").GetReportAndSave(folder[DRFolderEnum.crawler], notation, "cpu_" + fileprefix);
            DataTable dataTable = tRecord.dataLoadTaker.GetDataTableBase("dataLoadMetrics").GetReportAndSave(folder[DRFolderEnum.crawler], notation, "dataload_" + fileprefix);

            DataTable resourcesTable = tRecord.measureTaker.GetDataTableBase("resourceMetrics").GetReportAndSave(folder[DRFolderEnum.crawler], notation, "resource_"+fileprefix);

            

           

            if (imbWEMManager.settings.directReportEngine.doPublishPerformance)
            {
                tRecord.performance.folderName = folder.name;
                tRecord.performance.deploy(tRecord);

                tRecord.performance.saveObjectToXML(folder[DRFolderEnum.crawler].pathFor("performance.xml"));

                DataTable pTable = tRecord.performance.GetDataTable(true).GetReportAndSave(folder, notation, "crawler_performance" + fileprefix);

            }

            tRecord.lastDomainIterationTable.GetDataTable(null, imbWEMManager.index.experimentEntry.CrawlID).GetReportAndSave(folder, notation, "DLCs_performance_" + fileprefix);

            tRecord.reporter = this;
            

            signature.deployReport(tRecord);
            //signature.notation = notation;
            signature.saveObjectToXML(folder.pathFor("signature.xml"));

            

            folder.generateReadmeFiles(notation);

            fileunit tLog = new fileunit(folder[DRFolderEnum.logs].pathFor(fileprefix + ".txt"), false);
            tLog.setContent(tRecord.logBuilder.ContentToString(true));
            tLog.Save();

            tRecord.instance.reportCrawlFinished(this, tRecord);


            aceLog.consoleControl.setLogFileWriter();
            
        }

        
        

        /// <summary>
        /// 
        /// </summary>
        public crawlerSignature signature { get; set; }


        public void saveOutput(List<string> content, string path)
        {
            fileunit tLog = new fileunit(path, false);
            tLog.setContentLines(content);
            tLog.Save();
        }

    }

}