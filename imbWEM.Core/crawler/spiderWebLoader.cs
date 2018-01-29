// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderWebLoader.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using HtmlAgilityPack;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Network.extensions;
    using imbACE.Network.web.core;
    using imbACE.Network.web.enums;
    using imbACE.Network.web.events;
    using imbACE.Network.web.request;
    using imbACE.Network.web.result;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.contentMetrics;
    using imbCommonModels.pageAnalytics.core;
    using imbCommonModels.pageAnalytics.enums;
    using imbCommonModels.structure;
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Core.textRetrive;
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.loader;
    using imbWEM.Core.stage;

    /// <summary>
    /// Spider web loader and tokenizator
    /// </summary>
    public class spiderWebLoader
    {



        public spiderWebLoader(performanceDataLoad __dataLoad=null)
        {
            webclientSettings = imbWEMManager.settings.loaderComponent.webclientSettings;
            
            crawlerFlags = crawlerAgentFlag.detectAndProcessLinkNodes | crawlerAgentFlag.detectAndProcessMetaNodes | crawlerAgentFlag.runSaveContentBlock;

            tokenSettings = new nlpTokenizatorSettings();

           // tokenizatorEngine = new htmlSmartTokenizator(tokenSettings);

            trSetup = imbWEMManager.settings.contentProcessor.textRetrieve;

            dataLoad = __dataLoad;
        }


        /// <summary> </summary>
        public performanceDataLoad dataLoad { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public webLoaderSettings webclientSettings { get; set; } = new webLoaderSettings();


        /// <summary>
        /// 
        /// </summary>
        public nlpTokenizatorSettings tokenSettings { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public textRetriveSetup trSetup { get; set; }


        ///// <summary>
        ///// 
        ///// </summary>
        //public htmlSmartTokenizator tokenizatorEngine { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public crawlerAgentFlag crawlerFlags { get; set; }


        /// <summary>
        ///
        /// </summary>
        public int loadIndex { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public int blockCount { get; set; } = 3;

        public spiderWebLoaderControler controler { get; internal set; }


        /// <summary>
        /// Runs the spider task.
        /// </summary>
        /// <param name="sTask">The s task.</param>
        /// <param name="crawlerContext">The crawler context.</param>
        /// <returns></returns>
        public spiderTaskResult runSpiderTask(spiderTask sTask, modelSpiderSiteRecord wRecord)
        {
            
            spiderTaskResult sResult = sTask.createResult();

            try
            {

                if (imbWEMManager.settings.crawlerJobEngine.crawlerDoParallelTaskLoads)
                {
                    Parallel.ForEach(sTask, ln =>
                    {
                        modelSpiderPageRecord pRecord = wRecord.getChildRecord(ln, ln.url); //.startChildRecord(ln, ln.url);

                    spiderTaskResultItem rItem = runSpiderTaskItem(ln, sTask.doTokenization, pRecord);

                        if (rItem.status != pageStatus.failed)
                        {
                            wRecord.context.targets.AttachPage(rItem, pRecord.logBuilder, blockCount); // <-------------------------------- [ STIZE
                    }

                        sResult.AddResult(rItem);

                    });
                }
                else
                {
                    foreach (spiderLink ln in sTask)
                    {

                        modelSpiderPageRecord pRecord = wRecord.getChildRecord(ln, ln.url); //.startChildRecord(ln, ln.url);

                        spiderTaskResultItem rItem = runSpiderTaskItem(ln, sTask.doTokenization, pRecord);

                        if (rItem.status != pageStatus.failed)
                        {
                            wRecord.context.targets.AttachPage(rItem, pRecord.logBuilder, blockCount);
                        }

                        sResult.AddResult(rItem);
                    }
                }

            } catch (Exception ex)
            {
                imbWEMManager.log.log("runSpiderTask exception: " + ex.Message);
            }

            loadIndex = loadIndex + sResult.Count();


            if (loadIndex > imbWEMManager.settings.crawlerJobEngine.loadCountForGC)
            {
                long mem = GC.GetTotalMemory(false);
                GC.Collect();
                GC.WaitForFullGCComplete();
                long dmem = GC.GetTotalMemory(false);

                aceLog.log("Memory allocation reduction [after " + loadIndex + " tasks]: " + (mem - dmem).getMByteCountFormated());
                loadIndex = 0;
            }





            sResult.finish();

            return sResult;
        }

        //  public spiderTaskResultItem runSpiderTaskItem(spiderLink ln, crawlerAgentContext crawlerContext, Boolean __doTokenization, modelSpiderSiteRecord wRecord)

        /// <summary>
        /// Runs the spider task item.
        /// </summary>
        /// <param name="ln">The ln.</param>
        /// <param name="crawlerContext">The crawler context.</param>
        /// <param name="__doTokenization">if set to <c>true</c> [do tokenization].</param>
        /// <param name="wRecord">The w record.</param>
        /// <returns></returns>
        public spiderTaskResultItem runSpiderTaskItem(spiderLink ln, bool __doTokenization, modelSpiderPageRecord pRecord)
        {
            spiderTaskResultItem rItem = new spiderTaskResultItem(ln);

            crawledPage page = null;
            
            page = doWebRequest(ln.url, pRecord); // < ----------------------- ovde puca

            rItem.finish(page, pRecord.iteration);

            if (page.status == pageStatus.failed)
            {
              
                return rItem;
            }
            
          
            pRecord.acceptPage(page);

           
            pRecord.init(rItem.sPage);

            return rItem; // <---------------------------------------------- [ prolazi


          


        }


        /// <summary>
        /// Makes the crawled page.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="pRecord">The p record.</param>
        /// <returns></returns>
        /// <exception cref="aceGeneralException">Error in link processing</exception>
        internal crawledPage makeCrawledPage(IWebResult result, modelSpiderPageRecord pRecord)
        {
            crawledPage page = new crawledPage(result.responseUrl, 0);
            page.result = result;
            page.domain = pRecord.wRecord.domainInfo.domainName;
            
            var links = result.HtmlDocument.DocumentNode.Descendants("a");

            if (links.Any())
            {

            } else
            {

            }


            foreach (HtmlNode hn in links)
            {
                try
                {
                    var ndv = hn.CreateNavigator();
                    link l = new link(ndv);
                    if (!l.isDefaultHomePage)
                    {
                        page.links.Add(l);
                    }
                }
                catch (Exception ex)
                {
                    throw new aceGeneralException(ex.Message, ex, page, "Error in link processing");
                }
            }

            if (page.links.Count == 0)
            {

            }

            var meta = result.HtmlDocument.DocumentNode.Descendants("meta");
            foreach (HtmlNode hn in meta)
            {
                String name = hn.GetAttributeValue("name", "none");
                String content = hn.GetAttributeValue("content", "");

                switch (name)
                {
                    case "keywords":
                        page.pageKeywords = content.SplitSmart(",", "", true, true);
                        break;
                    case "description":
                        page.pageDescription = content;
                        break;
                }
            }

            var title = result.HtmlDocument.DocumentNode.Descendants("title").FirstOrDefault();
            if (title != null) {
                page.pageCaption = title.InnerText;
            }


            page.links.deployCollection(page);

            page.isCrawled = true;
            page.status = pageStatus.loaded;

            if (!page.links.byScope[imbCommonModels.enums.linkScope.inner].Any())
            {

            }

            return page;
        }

        //request.doContentCheck = false;
        //request.doCoolOff = false;
        //request.doRetryExecution = false;
        //request.doSubdomainVariations = false;
        //request.doTimeoutLimiter = true;

        //request.doLogCacheLoaded = imbWEMManager.settings.executionLog.doPageLoadedFromCache;
        //request.doLogNewLoad = imbWEMManager.settings.executionLog.doPageLoadedLog;
        //request.doLogRequestError = imbWEMManager.settings.executionLog.doPageErrorOrDuplicateLog;

        //request.htmlSettings.doTransliterateToLat = false;
        //request.htmlSettings.doRemoveHtmlEntities = true;
        //request.htmlSettings.doUpperCase = true;
        //request.htmlSettings.doAutocloseOnEnd = true;

        /// <summary>
        /// Does the web request
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="crawlerContext">The crawler context.</param>
        /// <returns></returns>
        internal crawledPage doWebRequest(string url, modelSpiderPageRecord pRecord)
        {
           
            url = controler.GetDuplicateUrl(url);
            
            if (url.isNullOrEmpty())
            {
                imbWEMManager.log.log("EMPTY URL PASSED TO THE WEB LOADER");
                imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(2200);
            }

            loaderRequest wemRequest = new loaderRequest(url);
            
            if (controler.CheckFail(wemRequest.url))
            {
                wemRequest.executed = true;
                wemRequest.statusCode = System.Net.HttpStatusCode.ExpectationFailed;
            } else
            {
                wemRequest = loaderSubsystem.ExecuteRequest(wemRequest);   // <-----------------------------
                if (wemRequest.statusCode != System.Net.HttpStatusCode.OK)
                {
                    controler.SetFailUrl(wemRequest.url);
                }   
            }
                       

            if (dataLoad != null)
            {
                dataLoad.AddBytes(wemRequest.byteSize);
            }


            crawledPage page = makeCrawledPage(wemRequest, pRecord); // <-----------------------------[ STIZE DO OVDE



            return page; // <---- prolazi

        }
    }
}
