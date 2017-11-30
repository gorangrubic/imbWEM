// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderUnit.cs" company="imbVeles" >
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    /// <summary>
    /// Execution host used with <see cref="ISpiderEvaluatorBase"/> classes
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class spiderUnit:imbBindable
    {
        
        /// <summary>
        /// Executes the key page search. /// PREVAZIDJENO
        /// </summary>
        /// <param name="__evaluator">The evaluator.</param>
        /// <param name="rootPageUrl">The root page URL.</param>
        /// <param name="crawlerContext">The crawler context.</param>
        /// <param name="webSiteReport">The web site report.</param>
        /// <param name="siteProfile">The site profile.</param>
        /// <returns></returns>
        public void executeKeyPageSearch(ISpiderEvaluatorBase __evaluator, string rootPageUrl, modelWebSiteGeneralRecord wGeneralRecord, webSiteProfile siteProfile, modelSpiderTestRecord tRecord, modelSpiderTestGeneralRecord tGeneralRecord)
        {
            //evaluator = __evaluator;
            //// <--------------------------------------------------------------------------------------
            //spiderWeb web = new spiderWeb();
            //tRecord.startChildRecord(web, rootPageUrl); // .siteRecords.Add(siteProfile);

            //wGeneralRecord.recordStart(tGeneralRecord.testRunStamp, rootPageUrl);
            //tRecord.childRecord.wGeneralRecord = wGeneralRecord;
            //modelSpiderSiteRecord wRecord = tRecord.childRecord;
            //wRecord.siteProfile = siteProfile;

            //aceLog.consoleControl.setAsOutput(wRecord, "" + siteProfile.domain);
            

            //tRecord.stageControl.prepare();

            //crawlerAgentContext crawlerContext = wGeneralRecord.instance;
            //evaluator.setStartPage(rootPageUrl,web , crawlerContext);



            //spiderTask sTask = evaluator.getSpiderSingleTask(web.seedLink, tRecord.childRecord, 1);
            //spiderTaskResult sResult = loader.runSpiderTask(sTask, crawlerContext, tRecord.childRecord);

            
            //spiderStageControl sControl = tRecord.stageControl;
            //spiderObjectiveSolutionSet solSet = null;


            //sControl.stage.EnterStage(tRecord.childRecord, evaluator);


            //do
            //{
            //    //aceLog.consoleControl.setAsOutput(tRecord.childRecord);

            //    dataUnitSpiderIteration iDataUnit = tRecord.childRecord.timeseries.CreateEntry(null, sTask.iteration);

            //    evaluator.operation_receiveResult(sResult, wRecord);

            //    solSet = evaluator.operation_applyLinkRules(wRecord);

            //    sTask = evaluator.operation_GetLoadTask(wRecord);

            //    sResult = loader.runSpiderTask(sTask, crawlerContext, wRecord);

            //    evaluator.operation_detectCrossLinks(wRecord);

            //    tRecord.log("Spider [" + __evaluator.name + "] for [" + rootPageUrl + "] collected [" + wRecord.children.Count() + "] pages");

            //    iDataUnit.checkData();

            //    // <--------------------- Crawler objective control override
                
                
            //} while (!sControl.stage.CheckStage(wRecord, solSet, sTask));

            //wRecord.children.FinishAllStarted();

            //wGeneralRecord.children.FinishAllStarted();
    
            //tRecord.log("Spider [" + __evaluator.name + "] for [" + rootPageUrl + "] collected [" + wRecord.children.Count() + "] pages");

            //List<spiderPage> pages = evaluator.operation_evaluatePages(tRecord.childRecord);
            //wRecord.resultPageSet = pages;

            //wGeneralRecord.recordFinish();
            //tRecord.finishChildRecord();

            //long memBefore = GC.GetTotalMemory(false);
            //GC.Collect();
            //long memAfter = GC.GetTotalMemory(true);


            //long released = memBefore - memAfter;
            //aceLog.log("Memory released: " + released.getMByteCountFormated() + "");
            //aceLog.consoleControl.removeFromOutput(wRecord); //, "sp:" + tRecord.instance.name);
            // <--------------------------------------------------------------------------------------
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderWebLoader loader { get; protected set; } = new spiderWebLoader();


        /// <summary>
        /// 
        /// </summary>
        public ISpiderEvaluatorBase evaluator { get; set; }
    }
}
