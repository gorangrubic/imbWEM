// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerDomainTask.cs" company="imbVeles" >
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
    using System.Threading;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.interfaces;
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;
    using imbWEM.Core.stage.macro;

    /// <summary>
    /// Single domain task
    /// </summary>
    public class crawlerDomainTask:ILogSerializableProvider<ICrawlerDomainTaskSerializable>
    {




        public directAnalyticReporter reporter
        {
            get { return parent.parent.reporter; }
        }


        /// <summary> </summary>
        public Thread executionThread { get; protected set; }


        private bool _isStageAborted = false;

        /// <summary> </summary>
        public bool isStageAborted
        {
            get { return _isStageAborted || imbWEMManager.MASTERKILL_SWITCH; }
            set { _isStageAborted = value; }
        }


        /// <summary> loader is disabled for this task -- crawler will not get new spiderResults </summary>
        [Category("Flag")]
        [DisplayName("isLoaderDisabled")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("loader is disabled for this task -- crawler will not get new spiderResults")]
        public bool isLoaderDisabled { get; set; } = false;


        /// <summary>
        /// 
        /// </summary>
        private DateTime lastIterationStart { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Determines whether this instance is active. Returns <c>false</c> if more than <see cref="TimeLimitForOneItemLoad"/> minutes since last new iteration started <see cref="lastIterationStart"/>
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActive()
        {
            
            if (lastIterationStart == DateTime.MinValue)
            {
                return true;
            }

            if (DateTime.Now.Subtract(lastIterationStart).TotalMinutes > TimeLimitForOneItemLoad)
            {
                return false;
            }

            return true;
        }

        public double sinceLastIterationStart
        {
            get
            {
                if (lastIterationStart == DateTime.MinValue) return 0;
                return DateTime.Now.Subtract(lastIterationStart).TotalMinutes;
            }
        }


        /// <summary>
        /// Proper abort call
        /// </summary>
        public void CallAbort(CancellationTokenSource tokenSource)
        {
            isStageAborted = true;
            status = crawlerDomainTaskStatusEnum.aborted;


            tokenSource.Cancel();

            Thread.SpinWait(10);


            

            if (executionThread != null)
            {
                if (executionThread.IsAlive)
                {
                    try
                    {

                        executionThread.Join(10000);
                        executionThread.Abort();
                    }
                    catch (Exception ex)
                    {
                        aceLog.log(ex.Message, null, true);
                    }
                }
            }


            Closing();
        }


        /// <summary> </summary>
        public DateTime startTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        public int TimeLimitForOneItemLoad { get
            {
                if (wRecord != null)
                {
                    if (wRecord.iteration == 0)
                    {
                        return parent.parent.TimeLimitForTask * 2;
                    }
                }
                return parent.parent.TimeLimitForTask;
            }
            //set
            //{

            //}
        }


        /// <summary>
        /// 
        /// </summary>
        public crawlerDomainTaskIterationPhase iterationStatus { get; set; }


        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void start()
        {
            

            iterationStatus = crawlerDomainTaskIterationPhase.iterationProcessNotStarted;
            status = crawlerDomainTaskStatusEnum.working;

            executionThread = Thread.CurrentThread;


            if (status == crawlerDomainTaskStatusEnum.aborted)
            {
                aceLog.log("Aborted DomainTask --> start()");
                return;
            }



            lastIterationStart = DateTime.Now;
            startTime = DateTime.Now;
            

            aceLog.consoleControl.setAsOutput(wRecord, "" + wProfile.domain);



            parent.parent.reportPlugins.eventUniversal(crawlReportingStageEnum.DLCPreinitiation, reporter, this, wRecord);



            try
            {
                iterationStatus = crawlerDomainTaskIterationPhase.loadingSeedPage;
                // <--- STAGE 1

                spiderWebLoader loader = new spiderWebLoader(parent.parent.dataLoadTaker);
                loader.controler = parent.parent.webLoaderControler;
                stageControl.prepare();
                                

                spiderTask sTask = evaluator.getSpiderSingleTask(web.seedLink, wRecord, 1); // <-------- 

                spiderTaskResult sResult = loader.runSpiderTask(sTask, wRecord); // <--------------------------------------------------------[ izvršava 

                if (sResult.calculateSuccessRate() == 0)
                {
                    wRecord.log("Domain [" + wRecord.domain + "] is considered as failed since landing page load failed");
                    parent.parent.webLoaderControler.SetFailedDomain(wProfile, wRecord);
                }
                

                spiderObjectiveSolutionSet solSet = null;


                stageControl.stage.EnterStage(wRecord, evaluator);



                parent.parent.plugins.eventDLCInitiated(parent.parent, this, wRecord); //.eventDLCFinished(parent.parent, this, wRecord);
                evaluator.plugins.eventDLCInitiated(evaluator as spiderEvaluatorBase, this, wRecord);
                imbWEMManager.index.plugins.eventDLCInitiated(imbWEMManager.index.experimentEntry, this, wRecord);
                parent.parent.reportPlugins.eventDLCInitiated(reporter, this, wRecord);


                int lastTermCount = 0;

                // <--- STAGE 2
                do
                {


                    iterationStatus = crawlerDomainTaskIterationPhase.iterationStart;

                    lastIterationStart = DateTime.Now;

                    dataUnitSpiderIteration iDataUnit = wRecord.timeseries.CreateEntry(null, sTask.iteration);


                    iterationStatus = crawlerDomainTaskIterationPhase.receiveResult;

                    if (imbWEMManager.MASTERKILL_SWITCH)
                    {

                        aceLog.log("MASTERKILL SWITCH ON :: crawlerDomainTask->" + iterationStatus.ToString());
                        isStageAborted = true;
                        sResult.items.Clear();
                        sResult.task.Clear();
                        evaluator.settings.limitIterations = wRecord.iteration - 5;
                        evaluator.settings.limitTotalPageLoad= 0;


                        Closing();
                        return;
                    }


                    if (isStageAborted)
                    {
                        Closing();
                        return;
                    }



                    evaluator.plugins.processLoaderResult(sResult, wRecord, this);

                     // wRecord.context.targets.termsAll.Count();

                    var iter = wRecord.iterationTableRecord.GetLastEntryTouched();
                    if (iter != null)
                    {
                        lastTermCount = iter.terms_all;
                    }

                    evaluator.operation_receiveResult(sResult, wRecord);

                    // __tc = wRecord.context.targets.termsAll.Count() - __tc;

                    


                    if (isStageAborted)
                    {
                        Closing();
                        return;
                    }

                    iterationStatus = crawlerDomainTaskIterationPhase.applyLinkRules;
                    evaluator.plugins.processAfterResultReceived(wRecord, this);
                    solSet = evaluator.operation_applyLinkRules(wRecord);

                    if (isStageAborted)
                    {
                        Closing();
                        return;
                    }

                    iterationStatus = crawlerDomainTaskIterationPhase.getLoadTask;
                    sTask = evaluator.operation_GetLoadTask(wRecord);

                    if (isStageAborted)
                    {
                        Closing();
                        return;
                    }

                    iterationStatus = crawlerDomainTaskIterationPhase.loadingTask;
                    if (isLoaderDisabled)
                    {
                        wRecord.log("-- Loader component is disabled for this [" + wRecord.domain + "] task.");
                        sResult = new spiderTaskResult();
                    }
                    else
                    {
                        sResult = loader.runSpiderTask(sTask, wRecord);
                    }
                    if (isStageAborted)
                    {
                        Closing();
                        return;
                    }


                    parent.parent.dataLoadTaker.AddIteration();

                    iterationStatus = crawlerDomainTaskIterationPhase.updatingData;

                    if (evaluator.settings.doEnableCrossLinkDetection)
                    {
                        evaluator.operation_detectCrossLinks(wRecord);
                    }

                    iDataUnit.checkData();

                    targetLoaded = iDataUnit.tc_loaded_p;
                    targetDetected = iDataUnit.tc_detected_p;



                    if (reporter != null)
                    {
                        try {
                            int lTC = 0;
                            var iter2 = wRecord.iterationTableRecord.GetLastEntryTouched();
                            if (iter2 != null)
                            {
                                lTC = iter2.terms_all - lastTermCount;
                                
                            }

                            reporter.reportIteration(iDataUnit, wRecord, evaluator); // <------ ovde se kreira nova iteracija
                            imbWEMManager.index.plugins.eventIteration(imbWEMManager.index.experimentEntry, this, wRecord);


                            parent.parent.dataLoadTaker.AddContentPage(lTC, sResult.Count);
                        }
                        catch (Exception ex)
                        {
                            throw new aceGeneralException(ex.Message, ex, reporter, "Reporter.reportIteration() exception");
                        }
                    }

                    parent.parent.reportPlugins.eventIteration(evaluator, this, wRecord);


                    iterationStatus = crawlerDomainTaskIterationPhase.checkingRules;

                    if (targetLoaded >= evaluator.settings.limitTotalPageLoad)
                    {
                        isStageAborted = true;
                        wRecord.log("--- Loaded pages count meet limit [" + targetLoaded + "] on iteration [" + iDataUnit.iteration + "].");
                    }

                    if (iDataUnit.iteration >= evaluator.settings.limitIterations)
                    {
                        isStageAborted = true;
                        wRecord.log("--- Iteration limit reached [" + iDataUnit.iteration + "].");
                    }


                    if (DateTime.Now.Subtract(startTime).TotalMinutes >= parent.parent._timeLimitForDLC)
                    {
                        isStageAborted = true;
                        wRecord.log("--- Timeout : crawler domain task [" + wRecord.web.seedLink.url + "] aborted after [" + DateTime.Now.Subtract(startTime).TotalMinutes + "] minutes.");
                    }

                    if (isStageAborted) break;

                } while ((!stageControl.stage.CheckStage(wRecord, solSet, sTask)) && !isStageAborted);
                iterationStatus = crawlerDomainTaskIterationPhase.pageEvaluation;

                // <---- STAGE 3
                wRecord.resultPageSet = evaluator.operation_evaluatePages(wRecord);

                Closing();

            } catch (Exception ex)
            {

                
                crawlerErrorEnum errorType = crawlerErrorEnum.domainTaskError;

                switch (iterationStatus)
                {
                    case crawlerDomainTaskIterationPhase.applyLinkRules:
                        errorType = crawlerErrorEnum.spiderModuleError;
                        break;
                    case crawlerDomainTaskIterationPhase.getLoadTask:
                        errorType = crawlerErrorEnum.spiderGetTaskError;
                        break;
                    case crawlerDomainTaskIterationPhase.loadingTask:
                        errorType = crawlerErrorEnum.spiderLoadingError;
                        break;
                    case crawlerDomainTaskIterationPhase.pageEvaluation:
                        errorType = crawlerErrorEnum.spiderModuleError;
                        break;

                }

                string domainName = wRecord.domainInfo.domainName;

                if (!tRecord.crashedDomains.Contains(domainName))
                {
                    wRecord.log("Domain crashed first time: " + ex.Message);
                    aceLog.log("Domain [" + domainName + "] crashed first time: " + ex.Message);
                    aceLog.log("Domain [" + domainName + "] is restarting... ");
                    status = crawlerDomainTaskStatusEnum.waiting;
                    tRecord.crashedDomains.Add(wRecord.domainInfo.domainName);
                    reInitialization();
                    start();

                } else
                {
                    status = crawlerDomainTaskStatusEnum.aborted;
                   
                    wRecord.log("Aborted by execution exception: " + ex.Message);
                    
                }

                var clog = reporter.CreateAndSaveError(ex, wRecord, this, errorType);
                wRecord.log(clog.Message);
                //  crawlerErrorLog cel = new crawlerErrorLog(ex, wRecord, this, errorType);



            } finally
            {
                
            }

           aceLog.consoleControl.removeFromOutput(wRecord); //, "sp:" + tRecord.instance.name);

        }

        private bool closeCalled = false;

        /// <summary>
        /// Ending procedure
        /// </summary>
        /// <exception cref="aceGeneralException">Reporter.reportDomainFinished() exception</exception>
        public void Closing()
        {
            if (closeCalled) return;
            closeCalled = true;
           

            // wRecord.children.FinishAllStarted();
            wRecord.recordFinish();



            //if (imbWEMManager.settings.TFIDF.doExploitStandardCC && wRecord.tRecord.instance.settings.doEnableDLC_TFIDF)
            //{
            //    imbWEMManager.index.domainIndexTable.SetSiteTFCompiled(wRecord.context.targets.dlTargetPageTokens, wRecord.domain);
            //}

            if (imbWEMManager.settings.indexEngine.doIndexUpdateOnDLC)
            {
                imbWEMManager.index.deployWRecord(wRecord);
            }

            if (reporter != null)
            {
                try
                {
                    reporter.reportDomainFinished(wRecord);
                }
                catch (Exception ex)
                {
                    var  axe = new aceGeneralException(ex.Message, ex, reporter, "Reporter.reportDomainFinished() exception");
                    var clog = reporter.CreateAndSaveError(axe, wRecord, this, crawlerErrorEnum.DReportError);
                    wRecord.log(clog.Message);
                    throw axe;
                }
            }

            parent.parent.plugins.eventDLCFinished(parent.parent, this, wRecord);
            evaluator.plugins.eventDLCFinished(evaluator as spiderEvaluatorBase, this, wRecord);
            imbWEMManager.index.plugins.eventDLCFinished(imbWEMManager.index.experimentEntry, this, wRecord);
            parent.parent.reportPlugins.eventDLCFinished(evaluator, this, wRecord);

            if (isStageAborted)
            {
                status = crawlerDomainTaskStatusEnum.aborted;
            }
            else
            {
                status = crawlerDomainTaskStatusEnum.done;
            }


            if (imbWEMManager.settings.executionLog.doRemoveWRecordOnFinished)
            {
               // var tRecord = wRecord.tRecord;
               // var wGeneralRecord = wRecord.wGeneralRecord;
                tRecord.children.Remove(wRecord);
                tRecord.tGeneralRecord.children.Remove(wGeneralRecord);

            }

            if (wRecord.iteration < 2)
            {
                wRecord.log("Domain [" + wRecord.domain + "] considered as failed since less then two iterations were made on it"); 
                parent.parent.webLoaderControler.SetFailedDomain(wRecord.wProfile,wRecord);
            }

            iterationStatus = crawlerDomainTaskIterationPhase.iterationProcessFinished;
        }

        public void SetLogSerializable(ICrawlerDomainTaskSerializable output)
        {
            if (executionThread != null)
            {
                output.TaskExecutionThreadName = executionThread.Name;
            }
            output.TaskIsStageAborted = isStageAborted;
            output.TaskStartTime = startTime;
            output.TaskStatus = status;
            output.TaskTargetDetected = targetDetected;
            output.TaskTargetLoaded = targetLoaded;
            output.TaskTimeLimitForOneItemLoad = TimeLimitForOneItemLoad;

            if (wRecord != null)
            {
                if (wRecord.domainInfo != null)
                {
                    output.TaskDomainName = wRecord.domainInfo.domainRootName;
                } else
                {
                    output.TaskDomainName = wRecord.domain;
                    
                }
            }

            if (evaluator != null)
            {
                output.TaskCrawlerName = evaluator.name;
            }
            else
            {
                if (tRecord != null)
                {
                    output.TaskCrawlerName = tRecord.instance.name;
                }
            }
            if(aRecord != null)
            {
                output.TaskJobName = aRecord.name;

            }
            output.TaskIterationStatus = iterationStatus;
            output.TaskIsActive = IsActive();
            output.TaskLastIteration = lastIterationStart;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="crawlerDomainTask"/> class.
        /// </summary>
        /// <param name="__wProfile">The w profile.</param>
        /// <param name="__parent">The parent.</param>
        public crawlerDomainTask(webSiteProfile __wProfile, crawlerDomainTaskCollection __parent)
        {
            initialization(__wProfile, __parent);
        }

        protected void reInitialization()
        {
            wRecord = null;

            initialization(wProfile, parent);
        }

        /// <summary>
        /// Cration of wRecords
        /// </summary>
        /// <param name="__wProfile">The w profile.</param>
        /// <param name="__parent">The parent.</param>
        protected void initialization(webSiteProfile __wProfile, crawlerDomainTaskCollection __parent)
        {
            try
            {
                parent = __parent;
                wProfile = __wProfile;

                aRecord = parent.tRecord.aRecord;
               // crawler = aRecord.sciProject.mainWebCrawler;
              

                tRecord = parent.tRecord;

               // crawlerContext = aRecord.crawledContextGlobalRegister.GetContext(wProfile.domain, tRecord.instance.settings.legacySettings, wProfile, aRecord.testRunStamp);

             //   tGeneralRecord = aRecord.tGeneralRecord;

               

              //  aRecord.tGeneralRecord.startChildRecord(wProfile, wProfile.url);
              //  wGeneralRecord = aRecord.tGeneralRecord.childRecord; // aRecord.tGeneralRecord.children.GetRecord(crawlerContext, true);


                evaluator = tRecord.instance.Clone(tRecord.logBuilder);

                stageControl = tRecord.stageControl;

                if (stageControl == null)
                {
                    stageControl = new macroStageControlDeclareKeyPages("Default", "CrawlerDomainTask");
                    stageControl.stage.stageIterationLimit = evaluator.settings.limitIterations;

                }
                //stageControl = parent.aMacro.helpMethodBuildStageControl(aRecord, tRecord);

                web = new spiderWeb();

                evaluator.setStartPage(wProfile.url, web);

                tRecord.startChildRecord(web, wProfile.url);
                wRecord = tRecord.childRecord;
                wRecord.wGeneralRecord = wGeneralRecord;
                wRecord.frontierDLC = new frontierRankingAlgorithmDLCRecord(wRecord);
                wRecord.spiderReal = evaluator;
                wRecord.spiderDLCTask = this;
                //wRecord.siteProfile = __wProfile;

                evaluator.plugins.eventDLCInitiated(evaluator as spiderEvaluatorBase, this, wRecord);

                //  TimeLimitForOneItemLoad = parent.TimeLimitForOneItemLoad;
            }
            catch (Exception ex)
            {
                status = crawlerDomainTaskStatusEnum.aborted;
                var clog = crawlerErrorLog.CreateAndSave(ex, wRecord, this, crawlerErrorEnum.jobTaskInitiationError);

            }
        }


        /// <summary> </summary>
        public int targetLoaded { get; protected set; } = 0;


        /// <summary> </summary>
        public int targetDetected { get; protected set; } = 1;


        private double _finishedRatio;
        /// <summary> </summary>
        public double finishedRatio
        {
            get
            {
                if (targetLoaded == 0) return 0;
                if (targetDetected == 0) return 0;

                int b = targetDetected;
                if (evaluator != null)
                {
                    b=Math.Min(targetDetected, evaluator.settings.limitTotalPageLoad);
                }

                _finishedRatio = (((double) targetLoaded) / ((double) b));

                return _finishedRatio;
            }
            
        }




        /// <summary> </summary>
        public spiderWeb web { get; set; }
        /// <summary> </summary>
        internal ISpiderEvaluatorBase evaluator { get; set; }
        /// <summary> </summary>
        protected modelSpiderTestGeneralRecord tGeneralRecord { get; set; }


        /// <summary> </summary>
        public spiderStageControl stageControl { get; set; }



        /// <summary> </summary>
        public modelSpiderSiteRecord wRecord { get; protected set; }


        /// <summary> </summary>
        public modelSpiderTestRecord tRecord { get; set; }


        /// <summary> </summary>
        public analyticJobRecord aRecord { get; set; }



        /// <summary> </summary>
        public modelWebSiteGeneralRecord wGeneralRecord { get; set; }


        ///// <summary> </summary>
        //public crawlerAgentContext crawlerContext { get; set; }

        ///// <summary> </summary>
        //public webSiteProfilerModule profiler { get; set; }


        /// <summary> </summary>
     //   public complexCrawlerModule crawler { get; set; }



        private crawlerDomainTaskStatusEnum _status = crawlerDomainTaskStatusEnum.waiting;
        /// <summary> </summary>
        public crawlerDomainTaskStatusEnum status
        {
            get
            {
                
                if (_status != crawlerDomainTaskStatusEnum.done)
                {
                    if (executionThread == null)
                    {
                        return crawlerDomainTaskStatusEnum.aborted;
                    }
                    if (!executionThread.IsAlive)
                    {
                        return crawlerDomainTaskStatusEnum.done;
                    }
                    switch (executionThread.ThreadState)
                    {
                        case ThreadState.Stopped:
                        case ThreadState.Suspended:
                            return crawlerDomainTaskStatusEnum.done;
                            break;
                    }
                }
                return _status;
            }
            set
            {
                _status = value;
            }
        }


        /// <summary>Reference to parent collection</summary>
        public crawlerDomainTaskCollection parent { get; set; }


        /// <summary> </summary>
        public webSiteProfile wProfile { get; protected set; }

    }

}
