// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderEvaluatorBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.evaluators
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
    using imbCommonModels.pageAnalytics.enums;
    using imbCommonModels.structure;
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
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
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
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.control;
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins.content;
    using imbWEM.Core.plugins.crawler;
    using imbWEM.Core.stage;

    public abstract class spiderEvaluatorBase : ISpiderEvaluatorBase
    {
        public abstract void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        public abstract void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        public abstract void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord);


        public crawlerPlugInCollection plugins { get; protected set; }


        private void makeHash()
        {
            if (!setupCalled) setupAll();
            _crawlerHash = md5.GetMd5Hash(FullDescription);

        }


        /// <summary>
        /// 
        /// </summary>
        public string FullDescription { get; set; } = "";


        private string _crawlerHash = "";
        /// <summary>
        /// 
        /// </summary>
        public string crawlerHash
        {
            get
            {

                if (_crawlerHash.isNullOrEmpty())
                {
                    makeHash();
                }

                return _crawlerHash;
            }
            set { _crawlerHash = value; }
        }

        public spiderEvaluatorBase()
        {
            plugins = new crawlerPlugInCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="spiderEvaluatorSimpleBase"/> class.
        /// </summary>
        /// <param name="__name">The name.</param>
        /// <param name="__description">The description.</param>
        /// <param name="__aboutfile">The aboutfile.</param>
        /// <param name="__parent">The parent.</param>
        /// <param name="__doTokenization">if set to <c>true</c> [do tokenization].</param>
        public spiderEvaluatorBase(string __name, string __description, string __aboutfile, spiderUnit __parent)
        {
            name = __name;
            description = __description;
            aboutFilepath = __aboutfile;
            parent = __parent;
            plugins = new crawlerPlugInCollection(this);
        }




        public ISpiderEvaluatorBase Clone(ILogBuilder loger=null)
        {
            spiderEvaluatorBase output = GetType().getInstance(new object[] { parent }) as spiderEvaluatorBase;
            var changed = output.setObjectValueTypesBySource(this, loger);
            
            output.parent = parent;
            output.settings = settings;

            output.name = name;
            

            foreach (var plug in plugins.GetPlugIns())
            {
                plugIn_base plugin = plug.GetType().getInstance() as plugIn_base;
                    output.plugins.installPlugIn(plugin);   
            }

            prepareAll();

            return output;
        }
       

        protected spiderSettings _settings = new spiderSettings();
        /// <summary>
        /// 
        /// </summary>
        public spiderSettings settings
        {
            get { return _settings; }
            set { _settings = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderUnit parent { get; set; }

        /// <summary>
        /// Spider evaluator algorithm name
        /// </summary>
        public string name { get; set; } = "BlindSpider";

        /// <summary>
        /// Flow control objective rules
        /// </summary>
        public spiderEvalRuleCollection controlRules { get; protected set; } = new spiderEvalRuleCollection();

        /// <summary>
        /// Page drop-out rules
        /// </summary>
        public spiderEvalRuleCollection controlPageRules { get; protected set; } = new spiderEvalRuleCollection();

        /// <summary>
        /// Drop out rules
        /// </summary>
        public spiderEvalRuleCollection controlLinkRules { get; protected set; } = new spiderEvalRuleCollection();


        /// <summary>
        /// 
        /// </summary>
        public spiderEvalRuleCollection pageScoreRules { get; protected set; } = new spiderEvalRuleCollection();


        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; } = "Plain link extraction and ranking mechanism";

        /// <summary>
        /// filepath to external file with <c>about spider</c> text
        /// </summary>
        public string aboutFilepath { get; protected set; } = "reportInclude\\testdocs\\blindspider_about.md";


        /// <summary>
        /// Prepares all rules for new case
        /// </summary>
        public virtual void prepareAll()
        {
            if (!setupCalled) callSetupAll();


            controlRules.prepare();
            controlLinkRules.prepare();
            controlPageRules.prepare();

           
        }


        protected bool setupCalled { get; set; }

        /// <summary>
        /// Setups all.
        /// </summary>
        public abstract void setupAll();



        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public abstract PropertyCollectionExtended AppendDataFields(PropertyCollection data = null);

        /// <summary>
        /// Approves the URL for futher consideration based on url string content
        /// </summary>
        /// <param name="ln">The link to check</param>
        /// <returns>TRUE if link seems to be ok, FALSE if link url is not accepted by <see cref="spiderSettings.urlBanNeedles"/> list</returns>
        public virtual bool approveUrl(link ln)
        {
            bool output = true;
            string url = ln.url.ToLower();

            if (url == "#")
            {
                return false;
            }
            if (url == "")
            {
                return false;
            }
            if (url.isNullOrEmpty())
            {
                return false;
            }
            
            foreach (string ext in imbWEMManager.settings.linkResolver.urlBanNeedles)
            {
                if (url.Contains(ext))
                {
                    return false;
                }
            }

            foreach (string ext in imbWEMManager.settings.linkResolver.urlBanDomains)
            {
                if (url.Contains(ext))
                {
                    return false;
                }
            }
            return output;
        }

        /// <summary>
        /// Creates single web loading task
        /// </summary>
        /// <param name="lnk">The LNK.</param>
        /// <param name="sReport">The s report.</param>
        /// <param name="iteration">The iteration.</param>
        /// <returns></returns>
        public virtual spiderTask getSpiderSingleTask(spiderLink lnk, modelSpiderSiteRecord sReport, int iteration)
        {
            spiderTask output = new spiderTask(iteration, sReport.web);
           // output.doTokenization = flags.HasFlag(spiderEvaluatorExecutionFlags.doTokenization);

            output.Add(lnk);

            return output;
        }


        private void callSetupAll()
        {
            setupAll();
            setupCalled = true;
        }

        /// <summary>
        /// Sets the start page and inicializes all rule sets
        /// </summary>
        /// <param name="rootUrl">The root URL.</param>
        /// <param name="web">The web.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual spiderWeb setStartPage(string rootUrl, spiderWeb web)
        {
            
           // language = context.language;

            prepareAll();
            

            spiderLink splink = web.setSeedUrl(rootUrl);
            //context.domain = web.seedLink.domain;
            //context.url = rootUrl;
            //context.siteDomain = context.domain;
            //context.clickDepth = 0;


            return web;

        }

        PropertyCollection IAppendDataFields.AppendDataFields(PropertyCollection data)
        {
            return (PropertyCollection)AppendDataFields(data);
        }

        

        public abstract spiderObjectiveSolutionSet operation_applyLinkRules(modelSpiderSiteRecord wRecord);

        //public abstract spiderTask operation_GetLoadTask(modelSpiderSiteRecord wRecord);

        /// <summary>
        /// E3: Performes ranking, selects the next task and drops links below 
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        public virtual List<spiderPage> operation_evaluatePages(modelSpiderSiteRecord wRecord)
        {

            

            pageScoreRules.prepare();
            List<spiderPage> output = new List<spiderPage>();

            foreach (spiderPage pg in wRecord.web.webPages.items.Values)
            {
                if (pg.webpage.status == pageStatus.loaded)
                {
                    foreach (spiderEvalRuleForPageBase ruleForPage in pageScoreRules)
                    {
                        ruleForPage.learn(pg);
                    }
                    output.Add(pg);
                }
            }

            //foreach (spiderEvalRuleForPageBase ruleForPage in pageScoreRules)
            //{
            //    ruleForPage.AppendDataFields(wRecord.stats);
            //}


            List<spiderPage> outputTwo = new List<spiderPage>();

            foreach (spiderPage pg in output)
            {
                foreach (spiderEvalRuleForPageBase ruleForPage in pageScoreRules)
                {
                    spiderEvalRuleResult ruleResult = ruleForPage.evaluate(pg);
                    pg.marks.deploy(ruleResult);
                }
                int score = pg.marks.calculate(wRecord.iteration);

                if (score > -1)
                {
                    outputTwo.Add(pg);
                }
            }


            // <---------------------------------------------------------------------------------------- Application of page control rules
            spiderObjectiveSolutionSet obSet = new spiderObjectiveSolutionSet();
            foreach (controlPageRuleBase aRule in controlPageRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
                foreach (spiderPage pg in output)
                {
                    obSet.listen(aRule.evaluate(pg, wRecord));
                }
            }


            foreach (spiderPage page in obSet.links)
            {
                if (outputTwo.Count() > settings.primaryPageSetSize)
                {
                    outputTwo.Remove(page);
                }
                else
                {
                    break;
                }
            }
            // <-------------------------------------------------------------------------------------------------------------------------

            outputTwo.Sort((x, y) => x.marks.score.CompareTo(y.marks.score)); // <----------------------- sorts the pages after cut


            if (settings.flags.HasFlag(spiderEvaluatorExecutionFlags.doTrimPrimaryOutput)) // <------------------- does the final trim if it is turned on
            {
                int tkc = Math.Min(settings.primaryPageSetSize, outputTwo.Count());
                outputTwo = outputTwo.Take(tkc).ToList();
            }

            wRecord.resultPageSet = outputTwo; // <------------------------------------------------------ transfers the final set to the record

            foreach (spiderPage pg in outputTwo)
            {
                var pRecord = wRecord.children.GetRecord(pg.spiderResult.target);

                pRecord.recordFinish(wRecord.resultPageSet); // <---------------------------------------- calls record finish for page records
            }

            return outputTwo;
        }

        /// <summary>
        /// E1: Operations the receive result.
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        public dataUnitSpiderIteration operation_receiveResult(spiderTaskResult stResult, modelSpiderSiteRecord wRecord)
        {
            dataUnitSpiderIteration iDataUnit = wRecord.timeseries[stResult.task.iteration];

            wRecord.logBuilder.log("Received: " + stResult.Count() + " (it:" + stResult.task.iteration + ")");

            wRecord.iteration = stResult.task.iteration;

            if (stResult.Any())
            {
                wRecord.spiderTaskResults.Add(stResult);
            }

            int targetCount = wRecord.web.webTargets.items.Count();

            int linkFlush = wRecord.context.acceptLoadedPages(stResult);
            int nw_failed_l = wRecord.context.processLoaderResult(stResult, settings.FRONTIER_doLinkResolver, settings.FRONTIER_doLinkHarvest, settings.FRONTIER_harvestNature, settings.FRONTIER_harvestScope);
            

            int newLinks = wRecord.web.webTargets.items.Count() - targetCount;

            iDataUnit.nw_detected_l = newLinks;

            iDataUnit.nw_failed_l = nw_failed_l;
            iDataUnit.tc_detected_l = wRecord.web.webLinks.items.Count();
            iDataUnit.tc_loaded_p = wRecord.web.webPages.items.Count();
            iDataUnit.tc_detected_p = wRecord.web.webTargets.items.Count();
            iDataUnit.tc_ingame_l = wRecord.web.webActiveLinks.items.Count();

            iDataUnit.nw_processed_l = linkFlush;


            //sRecord.timeline.timeSeries[sRecord.iteration] = new PropertyCollectionExtended();
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_iteration, sRecord.iteration);
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_pagesloaded, );
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_totallinks, sRecord.web.webLinks.items.Count());
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_activelinks, );
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_tasksize, stResult.task.Count());
            //sRecord.timeline.timeSeries[sRecord.iteration].add(modelSpiderSiteTimelineEnum.tl_newlinks, newLinks);
            wRecord.logBuilder.log("Active links [" + wRecord.web.webActiveLinks.items.Count() + "] change [" + newLinks + "]");

            return iDataUnit;
        }


        protected virtual void operation_doControlAndStats(modelSpiderSiteRecord wRecord)
        {
            var stats = wRecord.web.webActiveLinks.calculateTotalAndAvgScore();

            wRecord.timeseries[wRecord.iteration].avg_score_l = stats.Item2;
            //   wRecord.timeseries[wRecord.iteration].tc_detected_l = stats.Item1;


            // <---------------- Control rules
            spiderObjectiveSolutionSet output = new spiderObjectiveSolutionSet();
            /// cleaning rule memory
            foreach (controlLinkRuleBase aRule in controlLinkRules)
            {
                aRule.startIteration(wRecord.iteration, wRecord);
            }

            /// perceiving current situation
            foreach (spiderLink sLink in wRecord.web.webActiveLinks)
            {
                foreach (controlLinkRuleBase aRule in controlLinkRules)
                {
                    aRule.learn(sLink, wRecord);
                }
            }


            /// --------- TRIM BELOW ZERO ------------ ///
            if (settings.FRONTIER_PullDecayModes.HasFlag(spiderPullDecayModes.belowZeroScoreRemoval))
            {
                foreach (spiderLink sLink in wRecord.web.webActiveLinks.ToList())
                {
                    if (sLink.marks.score < 0)
                    {
                        wRecord.web.webActiveLinks.Remove(sLink);
                        wRecord.log("Link [" + sLink.url + "] had score below zero");
                    }
                }
            }

            /// apply update on results
            foreach (spiderLink sLink in wRecord.web.webActiveLinks)
            {
                foreach (controlLinkRuleBase aRule in controlLinkRules)
                {
                    output.listen(aRule.evaluate(sLink, wRecord));
                }
            }

            int removed = 0;
            foreach (var link in output.links)
            { // <--------------------------------------------------- removes any links found at control solution set

                wRecord.web.webActiveLinks.Remove(link);
                removed++;
            }


            if (removed > 0)
            {
                wRecord.log("Control rules removed: " + removed.ToString() + " links from active links collection");
            }


            //wRecord.logBuilder.log("Link drop-out:" + output.links.Count + ". Now have " + wRecord.web.webActiveLinks.Count() + " links waiting.");

            stats = wRecord.web.webActiveLinks.items.calculateTotalAndAvgScore();
            wRecord.timeseries[wRecord.iteration].avg_scoreADO_l = stats.Item2;
            wRecord.timeseries[wRecord.iteration].tc_scoreADO_l = stats.Item1;
            wRecord.timeseries[wRecord.iteration].nw_ruledout_l = output.links.Count;


        }


        protected spiderTask __operation_GetLoadTaskCommon(modelSpiderSiteRecord wRecord, IEnumerable<spiderLink> activeLinks)
        {

            operation_doControlAndStats(wRecord);
            // <------------------------------------------------------------------------------------------


            int n = wRecord.context.GetNextIterationLTSize(activeLinks);

          

           // wRecord.logBuilder.log("Creating new spiderTask for iteration " + wRecord.iteration + " with " + n + " links to load. To Limit: " + toLimit);

            spiderTask outputTask = new spiderTask(wRecord.iteration + 1, wRecord.web);
            outputTask.AddRange(activeLinks.Take(n));

            foreach (var ali in activeLinks)
            {
                if (!outputTask.Contains(ali))
                {
                    ali.marks.cycleRegistration(wRecord.iteration);
                }
            }
            

            return outputTask;
        }

        /// <summary>
        /// E3: Performes ranking, selects the next task and drops links below 
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        public virtual spiderTask operation_GetLoadTask(modelSpiderSiteRecord wRecord)
        {


            return __operation_GetLoadTaskCommon(wRecord, wRecord.web.webActiveLinks);
        }



        /// <summary>
        /// Populate relationship information
        /// </summary>
        /// <param name="sRecord">The s record.</param>
        /// <returns></returns>
        public void operation_detectCrossLinks(modelSpiderSiteRecord sRecord)
        {
            // sRecord.logBuilder.log("Detection of cross links started for: " + sRecord.web.webPages.items.Count());

            // Connect all
            foreach (KeyValuePair<string, spiderLink> ln_pair in sRecord.web.webLinks.items)
            {

                foreach (KeyValuePair<string, spiderPage> pg_pair in sRecord.web.webPages.items)
                {
                    int pos = ln_pair.Key.IndexOf(pg_pair.Key);
                    if (pos == -1)
                    {
                        //sRecord.logBuilder.log("No inner page was associated with hash key [" + pg_pair.Key + "] : this must be root");
                    }
                    else if (pos < 5)
                    {
                        pg_pair.Value.relationship.outflowLinks.Add(ln_pair.Key, ln_pair.Value);

                    }
                    else
                    {
                        pg_pair.Value.relationship.inflowLinks.Add(ln_pair.Key, ln_pair.Value);
                    }

                }
            }


            int totalCrossLinks = 0;
            sRecord.crossLinkStats.StartNew();

            foreach (KeyValuePair<string, spiderPage> pg_pair in sRecord.web.webPages.items)
            {
                totalCrossLinks = 0;
                foreach (KeyValuePair<string, spiderLink> ln in pg_pair.Value.relationship.inflowLinks)
                {
                    string inverse = ln.Value.getLinkSignature(false, true);
                    if (sRecord.web.webLinks.items.ContainsKey(inverse))
                    {
                        pg_pair.Value.relationship.crossLinks.Add(inverse, sRecord.web.webLinks.items[inverse]);
                        totalCrossLinks++;
                    }
                }

                sRecord.crossLinkStats.Current().Add(totalCrossLinks);
            }




            //sRecord.stats.add(modelSpiderSideFields.mss_totalcrosslinks, totalCrossLinks);

            //sRecord.stats.add(modelSpiderSiteTimelineEnum.tl_iteration, sRecord.iteration);
            //sRecord.stats.add(modelSpiderSiteTimelineEnum.tl_pagesloaded, sRecord.web.webPages.items.Count());
            //sRecord.stats.add(modelSpiderSiteTimelineEnum.tl_totallinks, sRecord.web.webLinks.items.Count());
            //sRecord.stats.add(modelSpiderSiteTimelineEnum.tl_activelinks, sRecord.web.webActiveLinks.items.Count());

            // imbWEMManager.log.log("Detection of cross links finished: " + totalCrossLinks);


        }

        
    }

}