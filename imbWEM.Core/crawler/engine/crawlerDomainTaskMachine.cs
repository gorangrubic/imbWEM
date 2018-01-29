// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerDomainTaskMachine.cs" company="imbVeles" >
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
    using System.Threading.Tasks;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins.engine;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    public class crawlerDomainTaskMachine
    {

        public crawlerDomainTaskMachineSettings setup { get; set; } = new crawlerDomainTaskMachineSettings();


        public enginePlugInCollection plugins { get; set; }
        
        public reportingPlugInCollection reportPlugins { get; set; }

        /// <summary>
        /// Optimizuje ucitavanje
        /// </summary>
        public spiderWebLoaderControler webLoaderControler { get; set; } = new spiderWebLoaderControler();


        /// <summary>
        /// 
        /// </summary>
        public performanceCpu cpuTaker { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public performanceDataLoad dataLoadTaker { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public directAnalyticReporter reporter { get; set; }


        /// <summary> </summary>
        public int maxThreads { get; set; } 


        /// <summary>
        /// 
        /// </summary>
        public int _timeLimitForDLC { get; set; } = 0;

        
        /// <summary>
        /// It will call for brute force cancelation
        /// </summary>
        public int TimeLimitForDomainCrawlCancelation
        {
            get {
                return _timeLimitForDLC;
            }
        }



        private int _TimeLimitForTask = 0;
        /// <summary>
        /// 
        /// </summary>
        public int TimeLimitForTask
        {
            get { return _TimeLimitForTask; }
            set {
                if (items != null)
                {
                    items.TimeLimitForOneItemLoad = value;
                }
                _TimeLimitForTask = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double TimeForObligatoryReport { get; set; } = 0;


        /// <summary>
        /// Time limit in minutes
        /// </summary>
        public int TimeLimitForCompleteJob { get; set; } = 120;


        /// <summary>
        /// 
        /// </summary>
        public int LoadForMemoryFlush { get; set; } = 0;

        public folderNode folder { get; set; }

        public modelSpiderTestRecord tRecord { get; set; }

        public performanceResources measureTaker;


        public builderForLog logger { get; set; }

        public crawlerDomainTaskMachine(modelSpiderTestRecord __tRecord, List<webSiteProfile> sample, directAnalyticReporter __reporter, folderNode __folder)
        {
            reporter = __reporter;
            folder = __folder;
            tRecord = __tRecord;


            logger = new builderForLog();
            aceLog.consoleControl.setAsOutput(logger, tRecord.name);

            SetWebLoaderControler(__folder);

            
            items = new crawlerDomainTaskCollection(tRecord, sample, this);

            cpuTaker = new performanceCpu(tRecord.name);
            dataLoadTaker = new performanceDataLoad(tRecord.name);
            measureTaker = new performanceResources(tRecord.name, this);

            cpuTaker.take();
            dataLoadTaker.take();
            measureTaker.take();

            tRecord.cpuTaker = cpuTaker;
            tRecord.dataLoadTaker = dataLoadTaker;
            tRecord.measureTaker = measureTaker;

            plugins = new enginePlugInCollection(this);
            reportPlugins = new reportingPlugInCollection(reporter, this);

        }


        /// <summary>
        /// Sets the web loader controler.
        /// </summary>
        /// <param name="__folder">The folder.</param>
        /// <returns></returns>
        public spiderWebLoaderControler SetWebLoaderControler(folderNode __folder)
        {
            webLoaderControler = new spiderWebLoaderControler();
            if (tRecord != null)
            {
                webLoaderControler.prepare(tRecord.logBuilder, folder);
            } else
            {
                webLoaderControler.prepare(null, folder);
            }
            
            return webLoaderControler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="crawlerDomainTaskMachine"/> class.
        /// </summary>
        /// <param name="tRecord">The t record.</param>
        /// <param name="sample">The sample.</param>
        /// <param name="macro">The macro.</param>
        public crawlerDomainTaskMachine(modelSpiderTestRecord tRecord, List<webSiteProfile> sample, analyticMacroBase macro)
        {
            items = new crawlerDomainTaskCollection(tRecord, sample, macro);
           // maxThreads = imbWEMManager.settings.crawlerDomainThreadCountAllowed;

        }


        /// <summary> </summary>
        public DateTime lastStatusReport { get; protected set; }


        /// <summary> </summary>
        public DateTime startTime { get; set; }


        /// <summary> </summary>
        public int lastLoad { get; set; } = 0;


        /// <summary> </summary>
        public List<Thread> activatedThreads { get; protected set; } = new List<Thread>();


        private int _taskRunning;
        /// <summary> </summary>
        public int taskRunning
        {
            get
            {
                if (task_running != null) _taskRunning = task_running.Count;
                return _taskRunning;
            }
            set
            {
                _taskRunning = value;
            }
        }


        private int _taskDone = new int();
        /// <summary> </summary>
        public int taskDone
        {
            get
            {
                if (task_finished != null) _taskDone = task_finished.Count;
                return _taskDone;
            }
            protected set
            {
                _taskDone = value;
            }
        }


        private int _taskWaiting = 0;
        /// <summary> </summary>
        public int taskWaiting
        {
            get
            {
                if (task_waiting != null) _taskWaiting = task_waiting.Count;
                return _taskWaiting;
            }
            protected set
            {
                _taskWaiting = value;
                
            }
        }




        private object changeTaskRunningLock = new object();

        public void changeTaskRunning(bool increase=true)
        {
            lock (changeTaskRunningLock) { 
                if (increase)
                {
                    taskRunning++;
                } else
                {
                    taskRunning--;
                }
            }
        }


        private object changeTaskDoneLock = new object();

        public void changeTaskDone(bool increase=true)
        {
            lock (changeTaskDoneLock)
            {
                if (increase)
                {
                    taskDone++;
                } else
                {
                    taskDone--;
                }
            }
        }


        /// <summary> </summary>
        public bool allTaskDone { get; set; }


        public void checkSettings()
        {
            if (maxThreads == -1) { }

            if (_timeLimitForDLC == 0)
            {
                _timeLimitForDLC = 120; //imbWEMManager.settings.crawlerTimeLimitForDomainTaskMinutes;
            }

            if (LoadForMemoryFlush == 0)
            {
                LoadForMemoryFlush = imbWEMManager.settings.crawlerJobEngine.domainCountDoneForGC;
            }

             TimeForObligatoryReport = imbWEMManager.settings.executionLog.crawlerDomainObligatoryReportAfterSeconds.GetRatio(60);

            dataLoadTaker.secondsBetweenTakes = imbWEMManager.settings.supportEngine.monitoringSampling;
            measureTaker.secondsBetweenTakes = imbWEMManager.settings.supportEngine.monitoringSampling;
            cpuTaker.secondsBetweenTakes = imbWEMManager.settings.supportEngine.monitoringSampling;
        }

        /// <summary>
        /// Auto parallel execution --- the framework handles the thread pool and other stuff.
        /// </summary>
        private void _autoParallel()
        {

            Parallel.ForEach<crawlerDomainTask>(items.items, taskToRun =>
            {
                string tskName = taskToRun.wProfile.domain;
                try
                {
                    Thread.Sleep(50);
                    //changeTaskRunning();

                    aceLog.log("[" + taskToRun.wProfile.domain + "] started on [id:" + Thread.CurrentThread.ManagedThreadId + "]");

                    items.running.Add(taskToRun);
                    taskToRun.start();

                    Thread.Sleep(50);
                    crawlerDomainTask taskToRemove = null;
                    //items.running.TryTake(out taskToRemove);
                    logger.log("[" + taskToRun.wProfile.domain + "] finished [tl:" + taskToRun.targetLoaded + "][td:" + taskToRun.targetDetected + "]");
                    items.done.AddUnique(taskToRun);
                    //changeTaskDone();
                    //changeTaskRunning(false);
                } catch (Exception ex)
                {
                    taskToRun.isStageAborted = true;
                    aceLog.log("[" + tskName + "] Thread crashed [" + ex.Message + ":" + ex.GetType().Name);
                }

            });

            allTaskDone = true;
        }

        private void _managedOneTask(object domainTask)
        {
            crawlerDomainTask taskToRun = (crawlerDomainTask)domainTask;
            try
            {

                items.running.Add(taskToRun);

                taskToRun.start();
                Thread.Sleep(50);

                crawlerDomainTask taskToRemove = taskToRun;
               
                

                logger.log("[" + taskToRun.wProfile.domain + "] finished [tl:" + taskToRun.targetLoaded + "][td:" + taskToRun.targetDetected + "]");
                items.done.AddUnique(taskToRun);
            } catch (Exception ex)
            {
                
                crawlerDomainTask taskToRemove = taskToRun;
               

                items.done.AddUnique(taskToRun);

                crawlerErrorLog clog = crawlerErrorLog.CreateAndSave(ex, taskToRun.wRecord, taskToRun, crawlerErrorEnum.domainOneTaskError);

                aceLog.log("[" + taskToRun.wProfile.domain + "] Thread crashed [" + ex.Message + ":" + ex.GetType().Name);

                taskToRun.CallAbort(cancelTokens[taskToRun]);
            }
        }


        private Dictionary<crawlerDomainTask, CancellationTokenSource> cancelTokens = new Dictionary<crawlerDomainTask, CancellationTokenSource>();


        internal List<Task> tasks = new List<Task>();
        internal List<Task> task_waiting = new List<Task>();
        internal List<Task> task_started = new List<Task>();
        internal List<Task> task_finished = new List<Task>();
        internal List<Task> task_canceled = new List<Task>();

        internal List<Task> task_running = new List<Task>();


        private void _managedParallel()
        {


            cancelTokens = new Dictionary<crawlerDomainTask, CancellationTokenSource>();

            try
            {

                foreach (var item in items.items)
                {
                    cancelTokens.Add(item, new CancellationTokenSource());

                    Task task = new Task(_managedOneTask, item, cancelTokens[item].Token);

                    tasks.Add(task);
                    task_waiting.Add(task);
                }

                maxThreads = Math.Min(maxThreads, tasks.Count);

                int rIndex = 0;
                while (task_finished.Count < tasks.Count)
                {

                    if (task_running.Count < maxThreads)
                    {
                        int toRun = maxThreads - task_running.Count;

                        for (int i = 0; i < toRun; i++)
                        {

                            Task task = task_waiting.FirstOrDefault();

                            if (task != null)
                            {
                                task_waiting.Remove(task);
                                task_running.Add(task);
                                task_started.Add(task);
                                task.Start();
                            }
                            else
                            {

                            }
                        }

                        rIndex = task_started.Count;
                    }

                    foreach (Task task in task_running.ToList())
                    {
                        switch (task.Status)
                        {
                            case TaskStatus.Faulted:
                                task_canceled.Add(task);
                                task_finished.Add(task);
                                task_running.Remove(task);
                                break;
                            case TaskStatus.RanToCompletion:
                                task_finished.Add(task);
                                task_running.Remove(task);
                                break;
                            case TaskStatus.Canceled:
                                task_canceled.Add(task);
                                task_finished.Add(task);
                                task_running.Remove(task);
                                break;
                            default:
                                break;
                        }
                    }

                    Thread.Sleep(imbWEMManager.settings.crawlerJobEngine.crawlerDomainCheckTickMs);

                    foreach (Task task in task_running.ToList())
                    {
                        bool abortTask = false;


                      
                        crawlerDomainTask taskToRun = (crawlerDomainTask)task.AsyncState;

                        
                        if (taskToRun.startTime != DateTime.MinValue)
                        {
                            if (DateTime.Now.Subtract(taskToRun.startTime).TotalMinutes > TimeLimitForDomainCrawlCancelation)
                            {

                                abortTask = true;


                                aceLog.log("Forced cancelation of [" + taskToRun.wProfile.domain + "] due double execution timeout [" + DateTime.Now.Subtract(taskToRun.startTime).TotalMinutes.ToString("#0.00") + "]", null, true);
                            }
                            else if (DateTime.Now.Subtract(taskToRun.startTime).TotalMinutes > _timeLimitForDLC)
                            {
                                abortTask = true;

                                taskToRun.isStageAborted = true;

                            }
                        }

                        if (!isEnabled)
                        {
                            aceLog.log("General Crawl Engine Abort Call --> " + taskToRun.wRecord.domain);

                            abortTask = true;
                        }


                        if (abortTask)
                        {
                            taskToRun.CallAbort(cancelTokens[taskToRun]);
                            
                            

                            // taskToRun.reporter.reportDomainFinished(taskToRun.wRecord);

                            task_finished.Add(task);
                            task_canceled.Add(task);
                            task_running.Remove(task);
                        }




                        if (!taskToRun.IsActive())
                        {
                            taskToRun.CallAbort(cancelTokens[taskToRun]);
                            aceLog.log("Task [" + taskToRun.wProfile.domain + "] became inactive on state [" + taskToRun.iterationStatus.ToString() + "] -- calling for abortion", null, true);
                            task_finished.Add(task);
                            task_running.Remove(task);
                            task_canceled.Add(task);
                        }
                    }



                    if (DateTime.Now.Subtract(startTime).TotalMinutes > TimeLimitForCompleteJob)
                    {
                        aceLog.log("Canceling any threads creation due time limit reached");
                        break;
                    }

                }
            } catch (Exception ex)
            {
                
              //  plugins.eventUniversal(crawlJobEngineStageEnum.error, this);
                
                var clog = crawlerErrorLog.CreateAndSave(ex, items.tRecord, null, crawlerErrorEnum.TaskMachineError);

            }

            if (task_finished.Count == tasks.Count)
            {
                allTaskDone = true;
            } else if (items.done.Count == items.items.Count)
            {
                allTaskDone = true;

            } else if (items.running.Count == 0) {
                allTaskDone = true;
            }



        }

        /// <summary>
        /// Optimizovan preko .NET Taskova --- tek treba da se implementira
        /// </summary>
        public void startAutoParallel(bool runManaged=false)
        {
            checkSettings();

            reporter.signature = new crawlerSignature();
            reporter.signature.deployTaskMachine(this);

          //  aceLog.consoleControl.setAsOutput(logger.logBuilder, items.tRecord.instance.name);
            startTime = DateTime.Now;

            items.tRecord.aRecord.tGeneralRecord.recordStart(items.tRecord.aRecord.testRunStamp, "spiderGeneralRecord::" + items.tRecord.instance.name);

            items.tRecord.aRecord.tGeneralRecord.AddSideRecord(items.tRecord.aRecord.childRecord);

            taskWaiting = items.items.Count;
            allTaskDone = false;
            Thread runThread = null;


            reportPlugins.eventAtInitiationOfCrawlJob(this, tRecord);


            try
            {

                if (!runManaged)
                {
                    runThread = new Thread(_autoParallel);

                }
                else
                {
                    runThread = new Thread(_managedParallel);
                }
            } catch (Exception ex)
            {
               var clog = crawlerErrorLog.CreateAndSave(ex, items.tRecord, null, crawlerErrorEnum.TaskMachineRunThreadError);
            }

            runThread.Start();

            try
            {
                do
                {
                    cpuTaker.checkTake();
                    dataLoadTaker.checkTake();
                    measureTaker.checkTake();

                    if (DateTime.Now.Subtract(lastStatusReport).TotalMinutes > TimeForObligatoryReport)
                    {
                        plugins.eventUniversal<crawlerDomainTask, spiderEvalRuleBase>(crawlJobEngineStageEnum.performanceTakeCycle, this, null, null);
                    }

                    Thread.Sleep(imbWEMManager.settings.crawlerJobEngine.crawlerDomainCheckTickMs);

                    foreach (crawlerDomainTask taskInRun in items.running.ToList())
                    {
                        switch (taskInRun.status)
                        {
                            
                            case crawlerDomainTaskStatusEnum.aborted:
                            case crawlerDomainTaskStatusEnum.done:
                                crawlerDomainTask rem = taskInRun;
                                items.running.TryTake(out rem);
                                items.done.AddUnique(taskInRun);
                                break;
                            default:

                                //if (DateTime.Now.Subtract(taskInRun.startTime).TotalMinutes > TimeLimitForDomainCrawl)
                                //{
                                //    taskInRun.isStageAborted = true;
                                //    taskInRun.status = crawlerDomainTaskStatusEnum.aborted;

                                //}
                                break;

                        }
                    }

                    if (imbWEMManager.MASTERKILL_SWITCH)
                    {
                        cpuTaker.take();
                        dataLoadTaker.take();
                        measureTaker.take();

                        Cancel();

                    }

                    if (DateTime.Now.Subtract(lastStatusReport).TotalMinutes > TimeForObligatoryReport)
                    {
                        statusReport();
                    }


                    if (DateTime.Now.Subtract(startTime).TotalMinutes > TimeLimitForCompleteJob)
                    {
                        aceLog.log("Job time limit triggered - cancelling all running tasks");
                        Cancel();
                    }

                    if ((items.done.Count() - lastLoad) > LoadForMemoryFlush)
                    {
                        lastLoad = items.done.Count();
                        imbWEMManager.GCCall("Regular memory cleanup after " + lastLoad + " domains crawled");
                    }

                    //if ((items.done.Count > 1) && (!items.running.Any()))
                    //{
                    //    allTaskDone = true;
                    //}

                } while (!allTaskDone);
            } catch (Exception ex)
            {
                var clog = crawlerErrorLog.CreateAndSave(ex, items.tRecord, null, crawlerErrorEnum.TaskMachineMonitoringError);
            }



            aceLog.log("Terminating parent run thread");
            runThread.Join();
            aceLog.log("Parent thread terminated");

            if (!imbWEMManager.MASTERKILL_SWITCH)
            {
                cpuTaker.take();
                dataLoadTaker.take();
                measureTaker.take();
            }


            items.tRecord.aRecord.tGeneralRecord.recordFinish();

            try
            {

                items.tRecord.instance.plugins.eventCrawlJobFinished(items.tRecord.aJob, this, items.tRecord);
                imbWEMManager.index.plugins.eventCrawlJobFinished(items.tRecord.aJob, this, items.tRecord);
                plugins.eventCrawlJobFinished(items.tRecord.aJob, this, items.tRecord);

                reportPlugins.eventCrawlJobFinished(items.tRecord.aJob, this, tRecord);

            } catch (Exception ex)
            {

            }
            aceTerminalInput.doBeepViaConsole(1200, 150, 2);

            // aceLog.log("[" + i + " / " + slimit + "] Spider[" + si + "][" + items.tRecord.instance.name + "]  [" + percent.ToString("P") + "]");

           // aceLog.consoleControl.removeFromOutput(logger.logBuilder);

        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void start()
        {
            
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            isEnabled = false;
            logger.log("Crawling terminated after [" + items.done.Count() + "] domains done (" + items.doneRatio.ToString("D2") + ")");
            logger.log("Waiting for active threads to finish [" + activatedThreads.Count() + "] domains done (" + items.doneRatio.ToString("D2") + ")");


            items.TimeLimitForOneItemLoad = 0;
            TimeLimitForCompleteJob = 0;

            foreach (Thread th in activatedThreads)
            {
                if (th.IsAlive)
                {
                    th.Join();
                    logger.log("Thread [" + th.Name + "] finished");
                }
            }
            statusReport();
        }
        

        public void statusReport()
        {
            lastStatusReport = DateTime.Now;

            


            // aceLog.consoleControl.setAsOutput(items.tRecord);
            logger.log("----------");
          //  logger.logBuilder.consoleAltColorToggle();

            

            double maxLatency = 0;

            foreach (Task task in task_running.ToList())
            {
                crawlerDomainTask taskInRun = task.AsyncState as crawlerDomainTask;
                double minRun = DateTime.Now.Subtract(taskInRun.startTime).TotalMinutes;
          
                
                string fR = "[d: _" + taskInRun.finishedRatio.ToString("P2") + "_ ]";
                string LbyD = "[ _" + taskInRun.targetLoaded + "/" + taskInRun.targetDetected + "_ ]";
                string TbyL = "[t: _" + minRun.ToString("#0.00") + "/" + _timeLimitForDLC.ToString() + "_ ]";

                string dom = "(initiating)";

                if (taskInRun.wRecord != null)
                {
                    if (taskInRun.wRecord.state == modelRecordStateEnum.initiated)
                    {
                        dom = taskInRun.wRecord.domainInfo.domainName;
                    }
                    else
                    {
                        if (taskInRun.wProfile != null)
                        {
                            dom = taskInRun.wProfile.domain;
                        }
                        else
                        {
                            dom = "(initiating)";
                        }
                    }
                }

                maxLatency = Math.Max(maxLatency, taskInRun.sinceLastIterationStart);

                string form = "{0,40} {1,12} {2,12} {3,12} {4,10}";
                logger.log(string.Format(form, dom, fR, LbyD, TbyL, "[a:" + taskInRun.sinceLastIterationStart.ToString("#0.00") + "]"));
            }
            
            double DRatio = (double)task_finished.Count() / (double)tasks.Count();
            double RRatio = (double)task_running.Count() / (double)tasks.Count();
            double WRatio = (1 - ((double)task_started.Count() / (double)tasks.Count()));
            logger.log("--- " + items.tRecord.instance.name + " [w: _" + WRatio.ToString("P2") + "_ ] [d: _" + DRatio.ToString("P2") + "_ ]" + "] [r: _" + RRatio.ToString("P2") + "_ ]" + " [t: _" + (DateTime.Now.Subtract(startTime).TotalMinutes.ToString("#0.00")) + "_ ]");



            plugins.eventUniversal<crawlerDomainTask, spiderEvaluatorBase>(crawlJobEngineStageEnum.statusReport, this, null, null);
            reportPlugins.eventStatusReport(this, tRecord);



           // logger.logBuilder.consoleAltColorToggle();



            aceTerminalInput.doBeepViaConsole(4400, 200, 1);


         
        }


        /// <summary>
        /// Indicating if the machine should continue running
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do continue]; otherwise, <c>false</c>.
        /// </value>
        public bool doContinue
        {
            get
            {
                if (!isEnabled) return false;
                if (items.waitingAndRunningCount == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isEnabled { get; protected set; } = true;


        /// <summary> </summary>
        internal crawlerDomainTaskCollection items { get; set; }
    }

}