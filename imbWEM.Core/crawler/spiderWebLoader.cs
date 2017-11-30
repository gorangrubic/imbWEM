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


            if (imbWEMManager.settings.crawlerJobEngine.crawlerDoParallelTaskLoads)
            {
                Parallel.ForEach(sTask, ln =>
                {
                    modelSpiderPageRecord pRecord = wRecord.getChildRecord(ln, ln.url); //.startChildRecord(ln, ln.url);

                    spiderTaskResultItem rItem = runSpiderTaskItem(ln, sTask.doTokenization, pRecord);

                    if (rItem.status != pageStatus.failed)
                    {
                        wRecord.context.targets.AttachPage(rItem, pRecord.logBuilder, blockCount);
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

            

            page = doWebRequest(ln.url, pRecord);

            rItem.finish(page, pRecord.iteration);

            if (page.status == pageStatus.failed)
            {
              //  wRecord.log("Page: " + ln.url + " failed --- *metrics* extraction impossibile");
                return rItem;
            }


            pRecord.pGeneralRecord = pRecord.wRecord.wGeneralRecord.getChildRecord(page, page.url);
            pRecord.acceptPage(page);

            pRecord.pGeneralRecord.AddSideRecord(pRecord);
            

           
            pRecord.init(rItem.sPage);
           // pRecord.pGeneralRecord.loadedBy.Set(pRecord.wRecord.tRecord.instance, true, checkListLogic.OR, true);

            //var pGeneralRecord = pRecord.pGeneralRecord;
            
            //if (pGeneralRecord.loadedBy.)
            //{
            //    pGeneralRecord.tokenizedContent = page.tokenizedContent as htmlContentPage;

               
            //}


            // pRecord.wRecord.siteProfile.automaticReconnect(pRecord.wRecord.profiler, pRecord.wRecord.aRecord.testRunStamp);

            //

            /*
            if (pGeneralRecord.pProfile == null)
            {
                webPageProfile wpp = new webPageProfile();
                //pRecord.wRecord.siteProfile.pageProfiles.sel
                wpp.url = page.url;
                wpp.textContent = page.result.document.processedSource;
                wpp.title = page.caption;
                pGeneralRecord.pProfile = wpp;//.pageProfiles_relation.getOrCreate(wRecord.testRunStamp) as webPageProfile;
                pRecord.wRecord.siteProfile.pageProfiles.Add(wpp);
               
                
                pGeneralRecord.pMetrics = pGeneralRecord.pProfile.metrics;

                pGeneralRecord.rpHtml = metricsEngine.getHtmlMetrics(page, _settings, crawlerContext.report);
                pGeneralRecord.rpToken = metricsEngine.getTokenMetrics(page, _settings, imbLanguageFramework.imbLanguageFrameworkManager.serbian.basic, crawlerContext.report);
                // <-------- ugaseno zbog buga
                // pGeneralRecord.rpLang = metricsEngine.getLanguageReport(page, imbLanguageFramework.imbLanguageFrameworkManager.serbian.basic, crawlerContext.AgentSettings.sampleTaker_languageTests, _settings, crawlerContext.report);

                pGeneralRecord.rpHtml.sendToObject(pGeneralRecord.pProfile, true, false, true);
                pGeneralRecord.rpToken.sendToObject(pGeneralRecord.pProfile, false, false, true);
                // pGeneralRecord.rpLang.sendToObject(pGeneralRecord.pProfile, false, false, true);
                
               
                
               // pGeneralRecord.pProfile.setModified();
             //   pGeneralRecord.pProfile.saveItem();

            }
            */
            // wRecord.log("Page general record have no page profile set yet ---> building general record for: " + page.url);




            // --- render diagrama u String prebaciti u output context
            //   String mainDiagramOutput = diagramOutput.getOutput(mainDiagram, null);



            //aceLog.log("Metrics for " + page.url + " saved html[" + pGeneralRecord.rpHtml.Count + "]" +
            //           " tokenstats[" + pGeneralRecord.rpToken.Count + "]" +
            //           " token_unique[" + pGeneralRecord.tokenFrequencyMatrixByCategory[contentTokenCountCategory.all].Count + "]"); // langtest[" + pGeneralRecord.rpLang.Count + "]");


            //   pGeneralRecord.pProfile.metrics.setModified();
            // pGeneralRecord.pProfile.metrics.saveItem();



            return rItem;
        }


        internal crawledPage makeCrawledPage(webResult result, modelSpiderPageRecord pRecord)
        {
            crawledPage page = new crawledPage(result.response.responseUrl, 0);

            page.result = result.request.result as IWebResult;

            XPathNavigator nav = result.document.getDocumentNavigator();

            page.report.connect(nav, true);

            page.report = metricsEngine.getMetaReport(page, page.report);

            var nodes_entry = page.report.report("links", htmlDefinitions.HTMLTags_linkTags);


            if (nodes_entry.nodes != null)
            {
                foreach (IXPathNavigable nd in nodes_entry.nodes)
                {
                    try
                    {
                        XPathNavigator ndv;
                        // logSystem.log(c + " : Starting new link:" + nd.toStringSafe(), logType.Debug);
                        if (nd == null)
                        {
                            continue;
                        }
                        ndv = nd.CreateNavigator();
                        if (ndv == null)
                        {
                            continue;
                        }

                        // logSystem.log(c + " : Processing link: " + ndv.OuterXml.Trim(), logType.Debug);
                        //String xml = ndv.OuterXml.toStringSafe();

                        link l = new link(ndv);

                        if (!l.isDefaultHomePage)
                        {
                            page.links.Add(l);
                        }
                        else
                        {
                        }

                        // c++;
                    }
                    catch (Exception ex)
                    {
                        throw new aceGeneralException(ex.Message, ex, page, "Error in link processing");
                    }
                    //page.acceptLink(linkTools.nodeToLink(nd.CreateNavigator(), result.response.responseDomain, page.url), _crawlerAgentContext.AgentSettings.storeAllLinks);
                }

                page.links.deployCollection(page);

            }

            page.isCrawled = true;
            page.status = pageStatus.loaded;
            return page;
        }
      

        /// <summary>
        /// Does the web request
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="crawlerContext">The crawler context.</param>
        /// <returns></returns>
        internal crawledPage doWebRequest(string url, modelSpiderPageRecord pRecord)
        {
            //var pReport = wRecord.childRecord;

            if (!url.Contains(imbUrlOps.urlShemaSufix))
            {
                

              //  url = url.ensureStartsWith(crawlerContext.domain.ensureEndsWith("/")).validateUrlShema(crawlerContext.shema);

                //imbUrlOps.getStandardizedUrl(url.ensureStartsWith(crawlerContext.domain), crawlerContext.shema);
            }


            url = controler.GetDuplicateUrl(url);



            webRequestClient request = new webRequestClient(url, webRequestActionType.HTMLasXML);
            
            request.doContentCheck = false;
            request.doCoolOff = false;
            request.doRetryExecution = false;
            request.doSubdomainVariations = false;
            request.doTimeoutLimiter = true;

            request.doLogCacheLoaded = imbWEMManager.settings.executionLog.doPageLoadedFromCache;
            request.doLogNewLoad = imbWEMManager.settings.executionLog.doPageLoadedLog;
            request.doLogRequestError = imbWEMManager.settings.executionLog.doPageErrorOrDuplicateLog;
            
            request.htmlSettings.doTransliterateToLat = false;
            request.htmlSettings.doRemoveHtmlEntities = true;
            request.htmlSettings.doUpperCase = true;
            request.htmlSettings.doAutocloseOnEnd = true;


          
            //if (webclientSettings.doUseProxy)
            //{
            //    imbProxy prox = imbProxyManagerEngine.globalProxyList.getDefaultVelesProxy();
            //    request.proxyToUse = prox.getWebProxy();
            //}


            webResult result = null;

            if (controler.CheckFail(request.url))
            {
                result = new webResult(request);
                request.status = webRequestEventType.error;
                request.lastLogMessage = "Url [" + request.url + "] is known fail address";
                
                
            } else
            {
                result = request.executeRequest(webclientSettings);

                if (result.request.isErrorStatus)
                {
                    controler.SetFailUrl(request.url);
                }

                if (result.request.status == webRequestEventType.error)
                {

                }
            }
            
            

            if (dataLoad != null)
            {
                dataLoad.AddBytes(result.byteSize);
            }


            crawledPage page = makeCrawledPage(result, pRecord); //crawlerContext.deployPage(result, crawlerFlags);



            return page;

        }
    }
}
