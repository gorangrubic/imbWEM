// --------------------------------------------------------------------------------------------------------------------
// <copyright file="controlPlugIn_TCAutomatic.cs" company="imbVeles" >
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.data.measurement;
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
    using imbSCI.DataComplex.trends;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.plugins.engine;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    public class controlPlugIn_TCAutomatic:enginePlugIn_base, IEnginePlugIn
    {
       
        Enum[] IPlugInCommonBase.INSTALL_POINTS
        {
            get
            {
                return new Enum[] { crawlJobEngineStageEnum.statusReport };
            }
        }

        public controlPlugIn_TCAutomatic()
        {
            name = "TC-Control";
        }




        /// <summary> LastAverage </summary>
        [Category("Ratio")]
        [DisplayName("CPUAverageLast")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("LastAverage")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CPUAverageLast { get; set; } = 0;

        public double CPUAverageDefendLine { get; set; } = 0;


        public int TCBeforeBan { get; set; } = 0;

        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { crawlJobEngineStageEnum.statusReport };
            }
        }


        public measureTrendTaker<performanceResourcesTake> trendMemory { get; set; }

        public measureTrendTaker<performanceResourcesTake> trendCPU { get; set; }

        public measureTrendTaker<performanceResourcesTake> trendDataLoad { get; set; }

        public measureTrendTaker<performanceDataLoadTake> trendContentPages { get; set; }
        public measureTrendTaker<performanceDataLoadTake> trendContentTerms { get; set; }
        public measureTrendTaker<performanceDataLoadTake> trendIterations { get; set; }


        public const string TREND_LINE = "{0,60} | {1,60} | {2,60}";

        public override void eventUniversal<TFirst, TSecond>(crawlJobEngineStageEnum stage, crawlerDomainTaskMachine __machine, TFirst __task, TSecond __resource)
        {
            switch (stage)
            {
                
                case crawlJobEngineStageEnum.statusReport:

                    //var tMemory = __machine.measureTaker.GetTrend(trendMemory);
                    //var tCPU = __machine.measureTaker.GetTrend(trendCPU);
                    //var tDataLoad = __machine.measureTaker.GetTrend(trendDataLoad);

                    //var tContentPages = __machine.dataLoadTaker.GetTrend(trendContentPages);
                    //var tContentTerms = __machine.dataLoadTaker.GetTrend(trendContentTerms);
                    //var tIterations = __machine.dataLoadTaker.GetTrend(trendIterations);

                    //loger.AppendLine(String.Format(TREND_LINE, tMemory.GetTrendInline(), tCPU.GetTrendInline(), tDataLoad.GetTrendInline()));
                    //loger.AppendLine(String.Format(TREND_LINE, tContentPages.GetTrendInline(), tContentTerms.GetTrendInline(), tIterations.GetTrendInline()));




                    int change = 0;
                    int newTC = __machine.maxThreads;
                    double maxLatencyToLimit = 0;
                    double maxLatency = 0;
                    bool doBoost = false;
                    string domainThatLates = "";
                    string threadId = "";
                    Thread criticalThread = null;
                    double average = CPUAverageLast;
                   double avgChange = average - CPUAverageLast;

                    double maxAge = 0;
                    crawlerDomainTask taskOldest = null;

                    var tasks = __machine.task_running.ToList();

                    foreach (Task task in tasks)
                    {
                        crawlerDomainTask taskInRun = task.AsyncState as crawlerDomainTask;
                        double since = taskInRun.sinceLastIterationStart;
                        double tage = DateTime.Now.Subtract(taskInRun.startTime).TotalMinutes;
                        maxLatency = Math.Max(maxLatency,since);
                        if (maxAge <= tage)
                        {
                            maxAge = tage;
                            taskOldest = taskInRun;
                        }
                        if (maxLatency <= since)
                        {
                            domainThatLates = taskInRun.wRecord.domain;
                            if (taskInRun?.executionThread != null) threadId = taskInRun.executionThread.ManagedThreadId.ToString() + " [" + taskInRun.executionThread.Priority.ToString() + "]";
                            criticalThread = taskInRun.executionThread;
                        }
                    }

                    maxLatencyToLimit = maxLatency.GetRatio(__machine.TimeLimitForTask);
                    double maxAgeLimit = maxAge.GetRatio(__machine._timeLimitForDLC);

                    double totalAgeLimit = DateTime.Now.Subtract(__machine.startTime).TotalMinutes.GetRatio(__machine.TimeLimitForCompleteJob);


                    loger.log("Max. latency:    [" + maxLatency.ToString("F2") + " min][" + maxLatencyToLimit.ToString("P2") + "] " + domainThatLates + " Thread: " + threadId);

                    if (taskOldest != null)
                    {
                        loger.log("Oldest DLC:      [" + maxAge.ToString("F2") + " min][" + maxAgeLimit.ToString("P2") + "] " + taskOldest.wRecord.domain + " Thread: " + taskOldest.executionThread.ManagedThreadId.ToString() + " [" + taskOldest.executionThread.Priority.ToString() + "]");

                    }

                    #region TIMEOUT PREVENTION -----------------------------------------
                    if (imbWEMManager.settings.crawlerJobEngine.doTaskTimeOutPrevention)
                    {
                        if (totalAgeLimit > 0.9)
                        {
                            bool newDisable = false;
                            foreach (Task task in tasks)
                            {
                                crawlerDomainTask t = task.AsyncState as crawlerDomainTask;
                                if (!t.isLoaderDisabled)
                                {
                                    t.isLoaderDisabled = true;
                                    newDisable = true;
                                    loger.log("Time Limit Critical: loader is disabled for: " + t.wRecord.domain + " due execution time limit for Thread: " + t.executionThread.ManagedThreadId.ToString());
                                }
                            }


                            if (newDisable) aceTerminalInput.doBeepViaConsole(1200, 250, 5);
                        }

                        if (maxAgeLimit > 0.9)
                        {
                            if (!taskOldest.isLoaderDisabled)
                            {
                                taskOldest.isLoaderDisabled = true;
                                loger.consoleAltColorToggle();
                                loger.log("DLC Time Limit Critical: loader is disabled for: " + taskOldest.wRecord.domain + " due execution time limit for Thread: " + taskOldest.executionThread.ManagedThreadId.ToString());
                                loger.consoleAltColorToggle();
                                aceTerminalInput.doBeepViaConsole();
                            }
                        }


                        doBoost = false;

                        if (maxLatencyToLimit > 0.5)
                        {
                            if (criticalThread != null)
                            {
                                criticalThread.Priority = ThreadPriority.AboveNormal;
                            }
                            change = -2;
                        }
                        else if (maxLatencyToLimit > 0.70)
                        {
                            if (criticalThread != null)
                            {
                                criticalThread.Priority = ThreadPriority.Highest;
                            }
                            change = -4;
                        }
                        else if (maxLatencyToLimit > 0.90)
                        {
                            loger.log("Max. latency critical :: REDUCING TO SINGLE THREAD : ");

                            foreach (Task task in tasks)
                            {
                                crawlerDomainTask taskInRun = task.AsyncState as crawlerDomainTask;
                                if (taskInRun?.executionThread != null) taskInRun.executionThread.Priority = ThreadPriority.BelowNormal;
                            }

                            if (criticalThread != null)
                            {
                                criticalThread.Priority = ThreadPriority.Highest;
                            }

                            newTC = 1;
                        }
                        else
                        {
                            foreach (Task task in tasks)
                            {
                                crawlerDomainTask taskInRun = task.AsyncState as crawlerDomainTask;
                                if (taskOldest == taskInRun)
                                {
                                    if (taskInRun?.executionThread != null) taskInRun.executionThread.Priority = ThreadPriority.AboveNormal;
                                }
                                else
                                {
                                    if (taskInRun?.executionThread != null) taskInRun.executionThread.Priority = ThreadPriority.Normal;
                                }

                            }

                            doBoost = true;
                        }
                    }

                    if (imbWEMManager.settings.crawlerJobEngine.doAutoAdjustTC)
                    {

                        #endregion --------------------------- ^ timeout prevention ^^

                        if (doBoost) // <------ TC adjust
                        {


                            var takes = __machine.cpuTaker.GetLastSamples(imbWEMManager.settings.crawlerJobEngine.CPUSampleForAutoAdjustMax);


                            if (takes.Count < imbWEMManager.settings.crawlerJobEngine.CPUSampleForAutoAdjust) return;

                            average = (takes.Average(x => x.reading) / 100);

                            avgChange = average - CPUAverageLast;

                            double CPUMargin = imbWEMManager.settings.crawlerJobEngine.CPUMargin;
                            int dlc = __machine.taskRunning;
                            CPUAverageDefendLine = Math.Max(average, CPUAverageLast);

                            if (dlc < (__machine.maxThreads - 1)) return;

                            if (average < imbWEMManager.settings.crawlerJobEngine.CPUTarget)
                            {
                                if (average < (CPUAverageDefendLine - CPUMargin))
                                {
                                    change = -1;
                                }
                                else
                                {
                                    change = 1;
                                }
                            }
                            else if (average > imbWEMManager.settings.crawlerJobEngine.CPULimit)
                            {
                                change = -1;
                            }

                            newTC = Math.Min(__machine.maxThreads + change, imbWEMManager.settings.crawlerJobEngine.TCAutoLimit);
                            if (newTC < 0) newTC = 1;

                            CPUAverageLast = average;
                        }
                        else
                        {
                            if (change != 0) newTC = Math.Min(__machine.maxThreads + change, imbWEMManager.settings.crawlerJobEngine.TCAutoLimit);
                            if (newTC < 0) newTC = 1;
                        }
                    }

                    int e_change = newTC - __machine.maxThreads;
                    __machine.maxThreads = newTC;

                    
                    loger.log("CPU average [" + average.ToString("P2") + "][" + avgChange.ToString("P2") + "] - (change: " + e_change + ") TC: " + __machine.maxThreads.ToString("D3") + " DLC:[" + __machine.taskRunning.ToString("D3") + "]");


                   
                    /*
                    if (average < imbWEMManager.settings.crawlerJobEngine.CPUTarget)
                    {
                        
                        ;
                        
                    }
                    else if (average > imbWEMManager.settings.crawlerJobEngine.CPULimit)
                    {
                        __machine.maxThreads = Math.Min(__machine.maxThreads - 1, imbWEMManager.settings.crawlerJobEngine.TCAutoLimit);
                        loger.log("CPU average [" + average.ToString("P2") + "]  >  reducing TC to: " + __machine.maxThreads.ToString("D3") + " DLC:[" + __machine.taskRunning.ToString("D3") + "]");
                    }
                    else
                    {
                        loger.log("CPU average [" + average.ToString("P2") + "]  ---------- TC_max: " + __machine.maxThreads.ToString("D3") + " DLC:[" + __machine.taskRunning.ToString("D3") + "]");
                    }
                    */
                    break;
            }
        }


        //public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as directReporterBase, __task, __wRecord);

        //public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord) => eventCrawlJobFinished(__machine, __tRecord);


        //public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as directReporterBase, __task, __wRecord);


        public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {
            
        }

        public void eventDLCFinished(crawlerDomainTaskMachine __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            
        }

       
        public void eventDLCInitiated(crawlerDomainTaskMachine __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            
        }

       
        public override void eventPluginInstalled()
        {
            IsEnabled = imbWEMManager.settings.crawlerJobEngine.doAutoAdjustTC;

            //trendMemory = new measureTrendTaker<performanceResourcesTake>(x => x.physicalMemory, "RAM", "MiB", 4, 2, 1, 0.02);
            //trendCPU = new measureTrendTaker<performanceResourcesTake>(x => x.cpuRateOfProcess, "CPU", "%", 4, 2, 1, 0.04);
            //trendDataLoad = new measureTrendTaker<performanceResourcesTake>(x => (x.pageLoadsRealSample / (x.secondsSinceLastTake.GetRatio(60))), "DataLoad", "MiB/min", 4, 2, 1, 0.04);
            //trendContentPages = new measureTrendTaker<performanceDataLoadTake>(x => (x.ContentPages.GetRatio(x.secondsSinceLastTake.GetRatio(60))), "ProcPage", "n/min", 4, 2, 1, 0.04);
            //trendContentTerms = new measureTrendTaker<performanceDataLoadTake>(x => (x.ContentTerms.GetRatio(x.secondsSinceLastTake.GetRatio(60))), "Terms", "n/min", 4, 2, 1, 0.04);
            //trendIterations = new measureTrendTaker<performanceDataLoadTake>(x => (x.CrawlerIterations.GetRatio(x.secondsSinceLastTake.GetRatio(60))), "FRA_it", "i/min", 4, 2, 1, 0.04);
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as crawlerDomainTaskMachine, __task, __wRecord);

       

        public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as crawlerDomainTaskMachine, __task, __wRecord);

        
    }
}