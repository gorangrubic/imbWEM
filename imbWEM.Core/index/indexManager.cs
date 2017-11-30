// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexManager.cs" company="imbVeles" >
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
namespace imbWEM.Core.index
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.tools;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.pageAnalytics.core;
    using imbCommonModels.structure;
    using imbCommonModels.webPage;
    using imbNLP.Data;
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
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.index.experimentSession;
    using imbWEM.Core.plugins.index;
    using imbWEM.Core.stage;

    /// <summary>
    /// INDEX MANAGEMENT
    /// </summary>
    public class indexManager
    {

        public indexPlugInCollection plugins { get; set; }

        public bool isFullTrustMode { get; set; } = false;
        public bool doAutoLoad { get; set; } = false;

        public indexPerformanceRecord indexSessionRecords { get; set; }

        public indexPerformanceEntry indexSessionEntry { get; set; }


        public experimentSessionRegistry experimentManager { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is preloaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is preloaded; otherwise, <c>false</c>.
        /// </value>
        public bool isPreloaded
        {
            get
            {
                return pageIndexTable.info != null;
            }
        }

        public indexManager(bool __doAutoLoad)
        {
            doAutoLoad = __doAutoLoad;
            folder = new folderNode("index", "Index data", "Folder for index data");
            if (doAutoLoad)
            {
                OpenIndex(indexPerformanceEntry.GLOBAL_IndexRepository, experimentSessionRegistry.GENERAL_SESSIONID);
            }



        }

        public List<string> GetIndexList()
        {
            var list = indexSessionRecords.GetList();
            List<string> indexes = new List<string>();
            foreach (indexPerformanceEntry item in list)
            {
                collectionExtensions.AddUnique(indexes, item.IndexRepository);
            }
            return indexes;
            //experimentManager.
        }


        public const string PATH_PageIndex = "pageIndex.xml";
        public const string PATH_DomainIndex = "domainIndex.xml";
        public const string PATH_ExperimentSessions = "ESIndex.xml";
        public const string PATH_IndexPerformance = "indexPerformanceRecords.xml";
        public const string PATH_MainFolder = "index";



        /// <summary>
        /// Gets the index filepath: FALSE for indexDomainTable, TRUE for indexPageTable
        /// </summary>
        /// <param name="forPageIndexTable">if set to <c>true</c> [for page index table].</param>
        /// <returns></returns>
        public string GetIndexFilepath(string IndexID, bool forPageIndexTable)
        {
             var __folder = new folderNode(PATH_MainFolder, "Global Index", "Folder for Global Index data");

            string pi = PATH_PageIndex;
            string di = PATH_DomainIndex;
            string es = PATH_ExperimentSessions;

            indexSessionRecords = new indexPerformanceRecord(folder.pathFor(PATH_IndexPerformance), doAutoLoad);

                folder = new folderNode(PATH_MainFolder.add(IndexID, "\\"), "Local Index", "Folder for Local Index (" + IndexID + ") data");

                pi = folder.pathFor(IndexID.getCleanFilePath() + "\\" + pi);
                di = folder.pathFor(IndexID.getCleanFilePath() + "\\" + di);
                es = folder.pathFor(IndexID.getCleanFilePath() + "\\" + es);

            if (forPageIndexTable)
            {
                return folder.pathFor(pi);
            } else
            {
                return folder.pathFor(di);
            }

            

        }

        /// <summary>
        /// Opens the index.
        /// </summary>
        /// <param name="IndexID">The index identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns></returns>
        public indexPerformanceEntry OpenIndex(string IndexID, string sessionId = "*")
        {
            string pi = PATH_PageIndex;
            string di = PATH_DomainIndex;
            string es = PATH_ExperimentSessions;

            folder = new folderNode(PATH_MainFolder, "Global Index", "Folder for Global Index data");

            indexSessionRecords = new indexPerformanceRecord(folder.pathFor(PATH_IndexPerformance), doAutoLoad);

            if (sessionId == "*") sessionId = experimentSessionRegistry.GENERAL_SESSIONID;

            if (IndexID == "*" || IndexID == indexPerformanceEntry.GLOBAL_IndexRepository)
            {

            } else
            {
                folder = new folderNode(PATH_MainFolder.add(IndexID, "\\"), "Local Index", "Folder for Local Index (" + IndexID + ") data");

                pi = folder.pathFor(IndexID.getCleanFilePath() + "\\" + pi);
                di = folder.pathFor(IndexID.getCleanFilePath() + "\\" + di);
                es = folder.pathFor(IndexID.getCleanFilePath() + "\\" + es);
            }

            pageIndexTable = new indexPageTable(folder.pathFor(pi), doAutoLoad);
            domainIndexTable = new indexDomainTable(folder.pathFor(di), doAutoLoad);

            if (imbWEMManager.settings.indexEngine.doRunIndexInReadOnlyMode)
            {
                pageIndexTable.ReadOnlyMode = true;
                domainIndexTable.ReadOnlyMode = true;
            }
            experimentManager = new experimentSessionRegistry(sessionId, folder.pathFor(es), doAutoLoad);
            current_indexID = IndexID;

            plugins = new indexPlugInCollection(experimentEntry);
            plugins.IsEnabled = false;

            lastIndexSave = DateTime.Now;
            return indexSessionEntry;
        }

        public string current_indexID { get; set; } = "";

        public experimentSessionEntry experimentEntry { get; set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="crawlId">The crawl identifier.</param>
        /// <returns></returns>
        public indexPerformanceEntry StartSession(string crawlId, analyticConsoleState state)
        {

            indexSessionEntry = indexSessionRecords.GetOrCreate(DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString());
            indexSessionEntry.SessionID = experimentManager.SessionID;
            indexSessionEntry.CrawlID = crawlId;
            indexSessionEntry.IndexRepository = current_indexID;
            indexSessionEntry.Start = DateTime.Now;


            experimentEntry = experimentManager.StartSession(crawlId, indexSessionEntry, state);
            plugins = new indexPlugInCollection(experimentEntry);
            plugins.IsEnabled = true;
            domainIndexTable.deploySession();
            //imbWEMManager.index.experimentManager.globalTFIDFSet.GetAggregateDataTable().saveObjectToXML(folder.pathFor("TFIFD_aggregate"));

            if (imbWEMManager.settings.indexEngine.doIndexPublishAndBackupOnOpenSession)
            {
                Publish(imbWEMManager.authorNotation, experimentEntry.indexSubFolder);
            }

            return indexSessionEntry;
        }

        /// <summary>
        /// Closes the session.
        /// </summary>
        public void CloseSession(IEnumerable<modelSpiderTestRecord> tRecords)
        {

            if (imbWEMManager.settings.indexEngine.doSaveFailedURLQueries)
            {
                pageIndexTable.ReadOnlyMode = false;

                int i = 0;
                int c = 0;
                int ic = pageIndexTable.urlsNotInIndex.Count;
                int ib = ic / 10;
                aceLog.log("Deploying queried URLs that were not in the index (" + ic.ToString() + ")");

                foreach (string url in pageIndexTable.urlsNotInIndex)
                {
                    i++;
                    c++;

                    indexPage page = pageIndexTable.GetPageForUrl(url);
                    page.url = url;

                    domainAnalysis da = new domainAnalysis(url);


                    page.domain = da.domainName;

                    pageIndexTable.AddOrUpdate(page);


                    if (i >= ib)
                    {

                        aceLog.log("URL processed: " + c.GetRatio(ic).ToString("P2") + " (" + c + ")");
                        i = 0;
                    }
                }
            }

            if (indexSessionEntry != null)
            {
                aceLog.log("Saving index engine performance : ... ");


                if (!SKIP_INDEXUPDATE)
                {
                    var das = imbWEMManager.index.domainIndexTable.GetDomainIndexAssertion(null, true);

                    aceLog.log("Saving index engine performance : DomainAssetion done ");

                    indexSessionEntry.Domains = domainIndexTable.Count;
                    indexSessionEntry.Pages = pageIndexTable.Count;
                    indexSessionEntry.PagesEvaluated = pageIndexTable.Where(x => !collectionExtensions.isNullOrEmpty(x.relevancyText)).Count();
                    indexSessionEntry.CrawlerHash = experimentManager.CurrentSession.state.setupHash_crawler;
                    indexSessionEntry.GlobalSetupHash = experimentManager.CurrentSession.state.setupHash_global;
                    indexSessionEntry.Duration = DateTime.Now.Subtract(indexSessionEntry.Start).TotalMinutes;

                    aceLog.log("Saving index engine performance : PagesEvaluated counted ");


                    indexSessionEntry.CertainityPP = das.certainty;
                    indexSessionEntry.MasterTFIDFCoverage = das.masterTFIDFApplied;
                    indexSessionEntry.DomainTFIDFs = das[indexDomainContentEnum.completeDomainTFIDF].Count;

                }

                aceLog.log("Saving index engine performance : Saving index ");

                if (imbWEMManager.settings.directReportEngine.doPublishIndexPerformanceTable)
                {
                    indexSessionRecords.AddOrUpdate(indexSessionEntry);
                }

                // experimentManager.globalTFIDFSet.GetAggregateDataTable().saveObjectToXML(folder.pathFor(experimentSessionRegistry.PATH_CompiledFTIDF));
            }


            //Publish(imbWEMManager.authorNotation, null);
            //Publish(imbWEMManager.authorNotation, experimentManager.CurrentSession.sessionReportFolder);

            experimentManager.CloseSession(tRecords);

        }

        public const bool SKIP_INDEXUPDATE = true;

        public void SetActiveTargets(modelSpiderSiteRecord wRecord, indexDomain domain)
        {
            List<indexPage> pages = domain.getPageSet();
           // wRecord.web.setSeedUrl(domain.url);
            //spiderPage sp = new spiderPage()

            crawledPage cpage = new crawledPage(domain.url, 0);

            spiderPage spage = new spiderPage(cpage, 0, 0);

            foreach (indexPage p in pages)
            {
                link l = new link(p.url);
                wRecord.context.processLink(l, spage, false);
            }
           
        }

        public void SetTarget(modelSpiderSiteRecord wRecord, indexPage page)
        {
            link ln = new link(page.url);
            
            //spiderLink sLink = new spiderLink()
            wRecord.context.processLink(ln, wRecord.web.webPages.items.FirstOrDefault().Value, false);
        }

        /// <summary>
        /// Rechecks the specified loger.
        /// </summary>
        /// <param name="loger">The loger.</param>
        public void Recheck(ILogBuilder loger)
        {
            domainIndexTable.recheck(pageIndexTable, loger);
        }

        /// <summary>
        /// Load index information from the external data table source
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="loger">The loger.</param>
        public void ExecuteIndexPageOperation(indexPageTableOperation operation, string sourceFile, List<string> domainsString=null, ILogBuilder loger=null)
        {
            List<indexPage> pages = new List<indexPage>();

            int i = 0;
            int c = 0;
            int imax = pages.Count / 20;

            List<indexDomain> domains = new List<indexDomain>();

            List<string> dList = new List<string>();
            if (domainsString != null)
            {
                foreach (string dm in domainsString)
                {
                    domainAnalysis da = new domainAnalysis(dm);
                    dList.Add(da.domainName);
                }

            }
            domainsString = dList;

            switch (operation)
            {
                default:
                    break;
                case indexPageTableOperation.flushNotInSample:

                    pages = pageIndexTable.GetList();

                    imax = domainsString.Count / 10;

                    // domains = domainIndexTable.GetDomains(indexDomainContentEnum.any);

                    List<indexPage> newPages = new List<indexPage>();
                    foreach (string dm in domainsString)
                    {
                        newPages.AddRange((IEnumerable<indexPage>) pageIndexTable.GetPagesForDomain(dm));

                        if (i > imax)
                        {
                            i = 0;
                            loger.log("Selecting pages [" + ((double)c / (double)domainsString.Count).ToString("P2") + "]");
                        }
                        i++;
                        c++;
                    }

                    pageIndexTable.Clear();
                    Save();





                    imax = newPages.Count / 20;


                    foreach (indexPage page in newPages)
                    {
                        pageIndexTable.Add(page);

                        if (i > imax)
                        {
                            i = 0;
                            loger.log("Adding pages [" + ((double)c / (double)newPages.Count).ToString("P2") + "]");
                        }
                        i++;
                        c++;
                    }

                    domainIndexTable.Clear();

                    Save();
                    Recheck(loger);

                    Publish(new aceAuthorNotation());

                    break;
                case indexPageTableOperation.flushNotLoaded:
                    pages = pageIndexTable.GetList();
                    //domains = domainIndexTable.GetDomains(indexDomainContentEnum.any);

                    foreach (indexPage page in pages)
                    {
                        if (page.byteSize ==0)
                        {
                            pageIndexTable.Remove(page);
                        }

                        if (i > imax)
                        {
                            i = 0;
                            loger.log("Removing pages from index [" + ((double)c / (double)pages.Count).ToString("P2") + "]");
                        }
                        i++;
                        c++;
                    }
                    break;
                case indexPageTableOperation.loadReviewedTable:
                    ApplyManualPageIndex(sourceFile, loger, true);
                    break;
            }

        }

        /// <summary>
        /// Applies the index of the manual page --- loads external, manually authored truth index
        /// </summary>
        /// <param name="manualPageIndex">Index of the manual page.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="callDomainIndexRecheck">if set to <c>true</c> [call domain index recheck].</param>
        public void ApplyManualPageIndex(string manualPageIndex = "pageIndex_revision.xlsx", ILogBuilder loger=null, bool callDomainIndexRecheck=true)
        {
            string path = folder.pathFor(manualPageIndex);

            

            if (File.Exists(path))
            {
                DataTable manualIndex = path.deserializeDataTable(dataTableExportEnum.excel, folder);

                int loaded = pageIndexTable.Load(manualIndex, loger, objectTableUpdatePolicy.overwrite);

                if (loger != null) loger.log("Page index entries updated: " + loaded);

                if (callDomainIndexRecheck)
                {
                    Recheck(loger);
                }

                isFullTrustMode = true;

            }
            else
            {
                loger.log("Excel file not found on: " + path);
            }
        }

        /// <summary>
        /// Publishes the specified notation.
        /// </summary>
        /// <param name="notation">The notation.</param>
        /// <param name="__folder">The folder.</param>
        public void Publish(aceAuthorNotation notation=null, folderNode __folder = null)
        {
            if (notation == null) notation = new aceAuthorNotation();

            if (__folder == null)
            {
                __folder = folder;
            }

            Save();

            pageIndexTable.GetDataTable().GetReportAndSave(__folder, notation, "pageIndex");
            domainIndexTable.GetDataTable().GetReportAndSave(__folder, notation, "domainIndex");
          
          //  indexSessionRecords.GetDataTable().GetReportAndSave(__folder, notation);

          //  domainIndexTable.GetDomainUrls(indexDomainContentEnum.onlyRelevant).saveContentOnFilePath(__folder.pathFor("indexDomains_onlyRelevant.txt"));
          //  domainIndexTable.GetDomainUrls(indexDomainContentEnum.onlyNonRelevant).saveContentOnFilePath(__folder.pathFor("indexDomains_onlyNonRelevant.txt"));
          //  domainIndexTable.GetDomainUrls(indexDomainContentEnum.bothRelevantAndNonRelevant).saveContentOnFilePath(__folder.pathFor("indexDomains_BothRelevantAndNon.txt"));

        }




        private object saveLock = new object();


        /// <summary>
        /// Saves the index to default location
        /// </summary>
        public void Save()
        {
            lock (saveLock)
            {
                lastIndexSave = DateTime.Now;

                getWritableFileMode mode = getWritableFileMode.newOrExisting;

                if (imbWEMManager.settings.indexEngine.doIndexBackupOnEachSave)
                {
                    mode = getWritableFileMode.autoRenameExistingToOld;
                }

                pageIndexTable.Save(mode);
                domainIndexTable.Save(mode);
                indexSessionRecords.Save(mode);
                experimentManager.Save(mode);

                
         
            }

     
            wRecordsDeployed = 0;
            
        }



        private object deployWRecordLock = new object();


        public int wRecordsDeployed = 0;
        private bool wRecordDeployedAutosaveDisable = false;


        public indexPage deployTarget(spiderTarget target, modelSpiderSiteRecord wRecord, indexDomain idomain)
        {
            indexPage page = pageIndexTable.GetPageForUrl(target.url); //.GetOrCreate(md5.GetMd5Hash(target.url));
            if (idomain == null) idomain = domainIndexTable.GetDomain(wRecord.domainInfo.domainName);
            page.url = target.url;

            page.tst = target.tokens.ToList().toCsvInLine(",");
            page.domain = wRecord.domainInfo.domainName;
            if (target.isLoaded)
            {
                if (target.evaluation != null)
                {
                    if (target.evaluation.result_language != basicLanguageEnum.unknown)
                    {
                        page.langTestRatio = target.evaluation.result_ratio;
                        page.singleMatchTokens = target.evaluation.singleLanguageTokens.toCsvInLine(",");
                        page.multiMatchTokens = target.evaluation.multiLanguageTokens.toCsvInLine(",");
                        page.wordCount = target.evaluation.allContentTokens.Count();
                        page.AllWords = target.evaluation.allContentTokens.toCsvInLine();
                        page.language = target.evaluation.result_language.ToString();


                    }
                }
                if (target.IsRelevant)
                {
                    page.relevancy = indexPageRelevancyEnum.isRelevant;

                }
                else if (target.evaluatedLanguage == basicLanguageEnum.unknown)
                {
                    page.relevancy = indexPageRelevancyEnum.unknown;

                }
                else
                {
                    page.relevancy = indexPageRelevancyEnum.notRelevant;

                }
                page.byteSize = target.page.spiderResult.page.result.byteSize;
            }



            pageIndexTable.AddOrUpdate(page);

            return page;
        }


        /// <summary>
        /// Deploys one DLC into current index
        /// </summary>
        /// <param name="wRecord">The w record.</param>
        public void deployWRecord(modelSpiderSiteRecord wRecord)
        {
            pageIndexTable.ReadOnlyMode = false;

            wRecord.indexDeployedMe = true;
                var idomain = domainIndexTable.GetDomain(wRecord.domainInfo.domainName);

                foreach (spiderTarget target in wRecord.context.targets)
                {

                    if (target.evaluation != null)
                    {
                        deployTarget(target, wRecord, idomain);

                        //indexPage page = pageIndexTable.GetOrCreate(md5.GetMd5Hash(target.url));
                        //page.url = target.url;

                        //page.tst = target.tokens.ToList().Join(',');
                        //page.domain = wRecord.domainInfo.domainName;

                        //page.langTestRatio = target.evaluation.result_ratio;
                        //page.singleMatchTokens = target.evaluation.singleLanguageTokens.Join(",");
                        //page.multiMatchTokens = target.evaluation.multiLanguageTokens.Join(",");
                        //page.wordCount = target.evaluation.allContentTokens.Count();

                        //page.language = target.evaluation.result_language.ToString();

                        //if (target.IsRelevant)
                        //{
                        //    page.relevancy = indexPageRelevancyEnum.isRelevant;

                        //}
                        //else if (target.evaluatedLanguage == imbLanguageFramework.basicLanguageEnum.unknown)
                        //{
                        //    page.relevancy = indexPageRelevancyEnum.unknown;

                        //}
                        //else
                        //{
                        //    page.relevancy = indexPageRelevancyEnum.notRelevant;

                        //}
                        //page.byteSize = target.page.spiderResult.page.result.byteSize;


                        //pageIndexTable.AddOrUpdate(page, objectTableUpdatePolicy.overwrite);

                        //page.byteSize = target.page.spiderResult.page.result.document.source.GetBytes().Count();
                    }
                    else
                    {
                        if (imbWEMManager.settings.indexEngine.doSaveDetectedURLs)
                        {
                            deployTarget(target, wRecord, idomain);
                        }
                    }




                }

                var domain = domainIndexTable.GetOrCreate(wRecord.domainInfo.domainName);
                domain.url = wRecord.domain;
                domainIndexTable.AddOrUpdate(domain);
            
           
            wRecordsDeployed++;
            
            if (!wRecordDeployedAutosaveDisable)
            {
                if (wRecordsDeployed >= imbWEMManager.settings.indexEngine.doIndexAutoSaveOnDLCs)
                {
                    aceLog.log("Index save called after: " + wRecordsDeployed + " deployed");
                    Save();
                }
            }
        }

        public DateTime lastIndexSave { get; set; } = DateTime.Now;

        /// <summary>
        /// Deploy spider record into index datatables
        /// </summary>
        /// <param name="tRecord">The t record.</param>
        public void deploy(modelSpiderTestRecord tRecord)
        {
            wRecordDeployedAutosaveDisable = true;

            pageIndexTable.ReadOnlyMode = false;
            domainIndexTable.ReadOnlyMode = false;

            int i = 0;
            int c = 0;
            int ic = tRecord.children.Count;
            int ib = tRecord.children.Count / 10;
            aceLog.log("Deploying crawl data into index (" + ic.ToString() + ")");

            foreach (KeyValuePair<spiderWeb,modelSpiderSiteRecord> pair in tRecord.children)
            {
                i++;
                c++;
                if (!pair.Value.indexDeployedMe)
                {
                    deployWRecord(pair.Value);
                }
                if (i >= ib)
                {
                    
                    aceLog.log("DLC processed: " + c.GetRatio(ic).ToString("P2"));
                    i = 0;
                }
            }
            wRecordDeployedAutosaveDisable = false;
            Save();

            pageIndexTable.ReadOnlyMode = imbWEMManager.settings.indexEngine.doRunIndexInReadOnlyMode;
            domainIndexTable.ReadOnlyMode = imbWEMManager.settings.indexEngine.doRunIndexInReadOnlyMode;
        }

        public indexPageTable pageIndexTable { get; protected set; }

        public indexDomainTable domainIndexTable { get; protected set; }


        public folderNode folder { get; protected set; }
    }
}
