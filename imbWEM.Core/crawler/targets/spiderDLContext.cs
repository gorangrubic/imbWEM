// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderDLContext.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.targets
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.extensions;
    using imbACE.Network.tools;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.enums;
    using imbCommonModels.pageAnalytics.enums;
    using imbCommonModels.structure;
    using imbCommonModels.webStructure;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Spider Domain Level Context
    /// </summary>
    public class spiderDLContext
    {

        /// <summary>
        /// Called at end of <see cref="spiderTarget.AttachPage(spiderPage, imbSCI.Core.interfaces.ILogBuilder, int)"/> method
        /// </summary>
        public modelSpiderSiteRecordEvent OnTargetPageAttached;

        /// <summary>
        /// Called when all loaded targets from <see cref="spiderTaskResult"/> are processed
        /// </summary>
        public modelSpiderSiteRecordEvent OnLoaderTaskProcessed;

        public spiderDLContext(modelSpiderSiteRecord __wRecord)
        {
            wRecord = __wRecord;
            web = __wRecord.web;
            spider = __wRecord.spider;
        }

        /// <summary> </summary>
        public ISpiderEvaluatorBase spider { get; protected set; }


        private spiderTargetCollection _targets;

        /// <summary>
        /// Target collection
        /// </summary>
        public spiderTargetCollection targets
        {
            get {
                if (_targets == null)
                {
                    _targets = new spiderTargetCollection(wRecord);
                }
                return _targets;
            }
            set { _targets = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public modelSpiderSiteRecord wRecord { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public spiderWeb web { get; set; }


        /// <summary>
        /// Accepts the loaded pages.
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <returns></returns>
        public int acceptLoadedPages(spiderTaskResult stResult)
        {
           
            int linkFlush = 0;
            foreach (spiderTaskResultItem cresult in stResult)
            {
                if (wRecord.web.webActiveLinks.items.Remove(cresult.target))
                {
                    linkFlush++;
                }
                
            }

            wRecord.logBuilder.log("[" + linkFlush.ToString() + "] active links were removed from the set as spiderTaskResult processed them.");
            return linkFlush;
        }


        /// <summary>
        /// Calculates the number of pages to be loaded in next iteration, in order to respect the PL_max limit in scenario where Load Take (LT) is higher then 1.
        /// </summary>
        /// <param name="activeLinks">The active links.</param>
        /// <returns></returns>
        public int GetNextIterationLTSize(IEnumerable<spiderLink> activeLinks)
        {
            var settings = wRecord.tRecord.instance.settings; 
            int toLimit = GetPageLoadsToLimit(settings.limitTotalPageLoad);
            int n = 1;

            switch (settings.FRONTIER_PullTakeMode)
            {
                case spiderPullLoadTakeMode.onlyLTSpecified:
                    n = Math.Min(wRecord.web.webActiveLinks.Count, settings.limitIterationNewLinks);
                    break;
                case spiderPullLoadTakeMode.onlyBestScored:
                    var al = wRecord.web.webActiveLinks.FirstOrDefault();
                    n = 0;
                    foreach (var ali in wRecord.web.webActiveLinks)
                    {
                        if (ali.marks.score == al.marks.score)
                        {
                            n++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case spiderPullLoadTakeMode.allAfterFRA:
                    
                    n = activeLinks.Count();
                    break;
                case spiderPullLoadTakeMode.none:
                    n = 0;
                    break;

            }



            n = Math.Min(n, toLimit);
            return n;
        }

        /// <summary>
        /// Returns distance from the Page Load limit
        /// </summary>
        /// <param name="limitTotalPageLoad">The limit total page load.</param>
        /// <returns></returns>
        public int GetPageLoadsToLimit(int limitTotalPageLoad)
        {
            int pageLoads = 0;

            var iterationRec = wRecord.iterationTableRecord.GetLastEntry();

            if (iterationRec != null) {
                pageLoads = iterationRec.loadedPageCount;
            } else {
                pageLoads = wRecord.context.targets.GetLoaded().Count - wRecord.duplicateCount;
            }


            int toLimit = limitTotalPageLoad - pageLoads;// (wRecord.context.targets.GetLoaded().Count - wRecord.duplicateCount);

            return toLimit;
        }


        /// <summary>
        /// Processes the link into Targets
        /// </summary>
        /// <param name="ln">The ln.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="doLinkResolver">if set to <c>true</c> [do link resolver].</param>
        /// <returns>If new target is created</returns>
        public bool processLink(link ln, spiderPage parentNode, bool doLinkResolver=true)
        {
            bool isNewLink = false;

            #region LINK NORMALIZATION =================================
            if (doLinkResolver)
            {
                ln.url = ln.getAbsoluteUrl(parentNode.webpage);
                ln.url = ln.url.httpsToHttpShema();
                ln.url = ln.url.equalizeUrlWithIndexFilenames();
                ln.url = wRecord.domainInfo.GetResolvedUrl(ln.url, imbWEMManager.settings.linkResolver.LNK_RemoveAnchors);

                domainAnalysis da = new domainAnalysis(ln.url);

                if (ln.url.IndexOf(da.domainName) > -1)
                {
                    int l = ln.url.Length - (ln.url.IndexOf(da.domainName) + da.domainName.Length);
                    if (l == 1)
                    {
                        ln.url = da.urlProper;
                    }
                }
            }
            #endregion ========================================================


            spiderLink sln = new spiderLink(parentNode, ln, wRecord.iteration); // <------------------------------------------------------------ upisuje referencu porekla: stranica, link i iteracija

            if (!spider.approveUrl(sln.link))
            {
                sln.flags |= spiderLinkFlags.urlNotSupported; // <---------------------------------------------------------------------- ako link nije poželjan / dozvoljen
            }
            else
            {
                spiderTarget target = targets.GetByTarget(sln);


                if (wRecord.web.webLinks.Add(sln))
                {
                    sln.flags |= spiderLinkFlags.newlinkVector;
                }
                else
                {
                    sln.flags |= spiderLinkFlags.oldlinkVector;
                }


                if (wRecord.web.webTargets.Add(sln))
                {
                    sln.flags |= spiderLinkFlags.newlinkTarget;
                }
                else
                {
                    sln.flags |= spiderLinkFlags.oldlinkTarget;
                }

                if (sln.flags.HasFlag(spiderLinkFlags.newlinkTarget) || (target == null))
                {
                    if (target == null)
                    {
                        isNewLink = true;
                        target = targets.GetOrCreateTarget(sln, true, true);
                        wRecord.web.webActiveLinks.Add(sln);
                    } else
                    {
                        isNewLink = false;
                    }
                    // <----------------------------------------------------------------------- upisuje u spisak aktivnih linkova
                }
            }
            
            return isNewLink;
        }



        /// <summary>
        /// Processes loader result
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="doLinkResolver">Performs LinkResolver component tasks over each harvested link</param>
        /// <param name="doLinkHarvest">Extract designated linkNature and linkScope from the content</param>
        /// <param name="nature">The nature of links to harvest - flags</param>
        /// <param name="scope">The scope of links to harvest - flags</param>
        /// <returns>Number of newly added links</returns>
        public int processLoaderResult(spiderTaskResult stResult, bool doLinkResolver=true, bool doLinkHarvest=true, linkNature nature = linkNature.navigation, linkScope scope = linkScope.inner)
        {
            int nw_failed_l = 0;
            foreach (spiderTaskResultItem cresult in stResult) // <--------------------------------------------------------------------------------- prolazi kroz sve učitane stranice
            {

                //cresult.page;
                spiderPage pg = cresult.sPage; //new spiderPage(cresult.page, wRecord.iteration); // <--------------------------------------------------------------- instancira spiderPage
                modelWebPageGeneralRecord pGeneralRecord = null;

                if (cresult.status != pageStatus.failed)
                {
                    web.webPageContentHashList.AddInstance(pg.contentHash, 1);

                    if (web.webPageContentHashList[pg.contentHash] > 1)
                    {
                        
                        if (imbWEMManager.settings.executionLog.doPageErrorOrDuplicateLog) aceLog.log("Page [" + pg.url + "] - is content duplicate ");
                        wRecord.listOfDuplicatedPages.Add(new contentHashAndAddressEntry(pg.url, pg.contentHash, web.webPageContentHashList[pg.contentHash]));
                        wRecord.duplicateCount++;
                        
                        var t = targets.GetByTarget(cresult.target);
                        if (t != null)
                        {
                            t.isDuplicate = true;
                        }
                        
                        continue;
                    }
                    

                    // <-------------------------------------------------- instancira pGeneralRecord
                    pGeneralRecord = wRecord.wGeneralRecord.children.GetRecord(pg.webpage, true);
                }
                
                cresult.target.targetedPage = pg; // <-------------------------------------------------------------------------------------------- upisuje u link referencu stranice

                if (!wRecord.web.webPages.Add(pg)) // <--------------------------------------------------------------------------------------------- registruje stranicu u webPages skup
                {
                    wRecord.logBuilder.log("Web page [" + pg.url + "] was loaded before - check the algorithm");
                }
                
                if (cresult.status != pageStatus.failed)
                {
                    if (doLinkHarvest)
                    {
                        List<link> links = cresult.page.links.Where<link>(x => (x.nature.HasFlag(nature) && x.scope.HasFlag(scope))).ToList(); // <---------------- izdvaja linkove sa stranice
                        int length = links.Count;

                        for (int i = 0; i < length; i++)// <------------------------------------------------------------------------------------------------- iteracija kroz linkove
                        {
                            processLink(links[i] as link, cresult.sPage);
                        }
                    }
                }
                else
                {
                    nw_failed_l++;
                }

                

                cresult.dispose();

            }

            if (OnLoaderTaskProcessed != null) OnLoaderTaskProcessed(wRecord, new modelSpiderSiteRecordEventArgs(stResult));

            return nw_failed_l;
        }

    }

}