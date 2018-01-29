// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_workload_state.cs" company="imbVeles" >
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
    using System.Text.RegularExpressions;
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
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.fields;
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
    using imbWEM.Core.index.experimentSession;
    using imbWEM.Core.stage;

    public class reportPlugIn_workload_state: IReportBenchmark
    {

        public measureTrendTaker<performanceResourcesTake> trendMemory { get; set; }

        public measureTrendTaker<performanceResourcesTake> trendCPU { get; set; }
        public measureTrendTaker<performanceTake> trendCPUm { get; set; }

        public measureTrendTaker<performanceResourcesTake> trendDataLoad { get; set; }

        public measureTrendTaker<performanceDataLoadTake> trendContentPages { get; set; }
        public measureTrendTaker<performanceDataLoadTake> trendContentTerms { get; set; }
        public measureTrendTaker<performanceDataLoadTake> trendIterations { get; set; }

        measureTrend tContentTerms;
        measureTrend tIterations;
        measureTrend tContentPages;
        measureTrend tCPUm;
        measureTrend tDataLoad;
        measureTrend tCPU;
        measureTrend tMemory;

        double mMemory = 1;


        public void doReadData(crawlerDomainTaskMachine _machine)
        {
            var lastTake = _machine.measureTaker.GetLastTake();

            tMemory = _machine.measureTaker.GetTrend(trendMemory);
            tCPU = _machine.measureTaker.GetTrend(trendCPU);
            tCPUm = _machine.cpuTaker.GetTrend(trendCPUm);

            tDataLoad = _machine.measureTaker.GetTrend(trendDataLoad);

            tContentPages = _machine.dataLoadTaker.GetTrend(trendContentPages);

            tContentTerms = _machine.dataLoadTaker.GetTrend(trendContentTerms);

            tIterations = _machine.dataLoadTaker.GetTrend(trendIterations);


            mMemory = lastTake.availableMemory.GetRatio(lastTake.totalMemory);
        }

        public string doCreateEntry(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry)
        {
            string recID = "";
            if (_machine != null)
            {
                thisSampleID = _machine.dataLoadTaker.CountTakes();
            }
            if (lastSampleID == -1) lastSampleID = 0;

            if (thisSampleID != lastSampleID)
            {
                RecordID++;

                // <------------------ RECORD CREATION
                recID = GetEntryID(RecordID, measureGroup);
                lastEntry = plugin.records.GetOrCreate(recID);
                lastEntry.RecordID = RecordID;
                lastEntry.pluginState = pluginState.ToString();

                switch (pluginState)
                {
                    case workloadPluginState.active:
                        lastEntry.measureGroup = measureGroup;
                        break;
                    default:
                        lastEntry.measureGroup = -1;
                        break;
                }

                lastEntry.SetTestIDAndSignature(tRecord.instance, entry.state, tRecord);

                lastEntry.terminationWarning = terminationWarning;
                lastEntry.availableMemory = mMemory;

                lastEntry.ContentPages = tContentPages.MicroMean;
                lastEntry.cpuRateOfMachine = tCPUm.MicroMean;
                lastEntry.cpuRateOfProcess = tCPU.MicroMean;
                lastEntry.physicalMemory = tMemory.MicroMean;


                lastEntry.CrawlerIterations = tIterations.MicroMean;
                lastEntry.DataLoad = tDataLoad.MicroMean;

                lastEntry.dlcDone = _machine.taskDone;
                lastEntry.dlcRunning = _machine.taskRunning;
                lastEntry.dlcWaiting = _machine.taskWaiting;
                lastEntry.dlcMaximum = _machine.maxThreads;

                plugin.records.AddOrUpdate(lastEntry);
            }
            lastSampleID = thisSampleID;
            return recID;
        }


        public void stateUpdate(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry)
        {
            comment = "";

            doReadData(_machine);

            // <------------------ DATA COLLECTION


           

           
            
            // <------------------ STATE DECISION
            doCheckFacts(_machine, tRecord, plugin, entry);
            
            doPerform(_machine, tRecord, plugin, entry);

            doCreateEntry(_machine, tRecord, plugin, entry);

            

            doCheckCriteria(_machine, tRecord, plugin, entry);

            
            

            

            // <------------------ PRINTING OUT ----------------------------

            plugin.loger.AppendHorizontalLine();
            
            if (pluginState != workloadPluginState.disabled)
            {
                string st_in = pluginState.ToString();
                if (pluginState == workloadPluginState.active) st_in = "_" + st_in + "_";
                plugin.loger.AppendLine(string.Format(STATUSLINE_ONE, st_in, lastEntry.RecordID.ToString("D3"), lastEntry.measureGroup, lastEntry.dlcMaximum, lastEntry.dlcRunning, lastEntry.dlcWaiting).toWidthExact(Console.BufferWidth-11, "="));
                
            }

            plugin.loger.AppendLine(tMemory.GetTrendInline() + " | " + tCPU.GetTrendInline() + " | " + tCPUm.GetTrendInline());
            plugin.loger.AppendLine(tDataLoad.GetTrendInline() + " | " + tContentPages.GetTrendInline() + " | " + tIterations.GetTrendInline());

            //plugin.loger.AppendLine("--- Info: " );
            if (pluginState != workloadPluginState.disabled)
            {
                plugin.loger.AppendLine(string.Format(STATUSLINE_TWO, mMemory.ToString("P2"), 
                    lastEntry.terminationWarning.ToString("D3"), lastEntry.dlcDone, DLCDoneForNext, thisSampleID, lastSampleID).toWidthExact(Console.BufferWidth - 11, "="));
                //  plugin.loger.AppendLine(String.Format(STATUSLINE_TWO, mMemory.ToString("P2"), g).toWidthExact(Console.BufferWidth-11, "="));
            }


           
            

        }

        public const string STATUSLINE_ONE = "=== {0,-15} === Rec:{1,3} Group:{2,3} == TC[{3,5}] RUN[{4,5}] CUE[{5,5}] ===";
        public const string STATUSLINE_TWO = "=== RAM available: {0,10} ============== W[{1,5}] ===== DLC _{2,5}_ / _{3,5}_ === Sample [{4,5}:{5,5}] =";

        private int lastSampleID = -1;
        private int thisSampleID = -1;

        private void doPerform(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry)
        {
            
            switch (pluginState)
            {
                case workloadPluginState.active:
                  

               
                break;
                case workloadPluginState.cooldown:
                    if (cooldownIndex > 0)
                    {

                        comment = comment.add($"Coolingdown [{cooldownIndex}]");
                        cooldownIndex = cooldownIndex - (thisSampleID - lastSampleID);
                        
                    } else
                    {
                        comment = comment.add($"Cooldown finished");
                        pluginState = workloadPluginState.active;
                    }
                    break;
                case workloadPluginState.disabled:
                    break;
                case workloadPluginState.none:
                    break;
                case workloadPluginState.preparing:
                    if (tCPU.SampleState.HasFlag(measureTrendSampleState.macroMean))
                    {
                        pluginState = workloadPluginState.active;
                        plugin.loger.log("Workload plugin ready");
                    }
                    break;
                case workloadPluginState.sampleTail:
                    break;
                case workloadPluginState.terminating:

                    terminate(_machine);
                    
                    break;
                case workloadPluginState.wormingUp:
                    if (wormingUpIndex > 0)
                    {
                        comment = comment.add($"WormingUp [{wormingUpIndex}]");
                        wormingUpIndex = wormingUpIndex - (thisSampleID - lastSampleID);
                    }
                    else
                    {
                        comment = comment.add($"WormingUp finished");
                        pluginState = workloadPluginState.active;
                    }
                    break;
            }
        
            
        }

        public void terminate(crawlerDomainTaskMachine _machine)
        {
            _machine.items.items.ToList().ForEach(x => x.isStageAborted = true);

            imbWEMManager.MASTERKILL_SWITCH = true;

            _machine.allTaskDone = true;
            _machine.TimeLimitForCompleteJob = 1;
            _machine._timeLimitForDLC = 1;
        }

        private void doCheckFacts(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry)
        {
            if (mMemory < plugin.plugin_settings.term_availableMemory)
            {
                warningUpDate("Available RAM [" + mMemory.ToString("P2") + "] is below the termination limit [" + plugin.plugin_settings.term_availableMemory.ToString("P2") + "]", true, plugin);
                if (terminationWarning >= plugin.plugin_settings.term_warningCount)
                {
                    isMemoryLimit = true;
                }
                else
                {

                }
            }
            else if (_machine.taskWaiting == 0)
            {
                warningUpDate("There is no DLCs waiting [" + _machine.taskWaiting + "] - no way to run DLCs up to TC_max [" + _machine.maxThreads + "]", true, plugin);
                if (terminationWarning >= plugin.plugin_settings.term_warningCount)
                {
                    isSampleTail = true;
                }
            }
            else
            {
                if (terminationWarning > 0)
                {
                    warningUpDate("All termination criteria clean", false, plugin);
                }
            }

            if (_machine.taskRunning > _machine.maxThreads)
            {
                plugin.loger.log($" Running {_machine.taskRunning} more then TC_max {_machine.maxThreads} - switching to cooldown");
                cooldownIndex = plugin.plugin_settings.warmingUpTicks;
                pluginState = workloadPluginState.cooldown;
            }
        }

        private void doStartNextGroup(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry) {

            doReadData(_machine);

            DLCDoneForNext = _machine.taskDone + plugin.plugin_settings.stepUp_DLCCount;
            pluginState = workloadPluginState.wormingUp;
            string msg = $"Measure group {measureGroup} completed -- DLCs done: {_machine.taskDone}";

            plugin.loger.log(msg);

            comment = comment.add($"Group {measureGroup} done");
            wormingUpIndex = plugin.plugin_settings.warmingUpTicks;
            if (wormingUpIndex == 0) pluginState = workloadPluginState.active;
            measureGroup = measureGroup + 1;
            _machine.maxThreads += plugin.plugin_settings.stepUp_step;

            

        }

        public void doCheckCriteria(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord, reportPlugIn_workload plugin, experimentSessionEntry entry)
        {
            if (!tCPU.SampleState.HasFlag(measureTrendSampleState.macroMean))
            {
                pluginState = workloadPluginState.preparing;
            }

            if (plugin.plugin_settings.term_DLCFinished > 0) // <----- da li je aktiviran ovaj uslov
            {
                if (_machine.taskDone >= plugin.plugin_settings.term_DLCFinished)
                {
                    terminate(_machine);

                }
            }



            if (pluginState == workloadPluginState.active)
            {
                if (_machine.taskDone >= DLCDoneForNext)
                {
                    doStartNextGroup(_machine, tRecord, plugin, entry);

                    stateUpdate(_machine, tRecord, plugin, entry);
                    
                    plugin.records.Save(getWritableFileMode.overwrite);
                }

                if (isSampleTail)
                {
                    if (plugin.plugin_settings.term_JLCinTail)
                    {
                        terminate(_machine);
                    }
                    else
                    {
                        pluginState = workloadPluginState.sampleTail;
                    }
                }

                if (isMemoryLimit)
                {
                    if (plugin.plugin_settings.term_availableMemory > 0)
                    {
                        terminate(_machine);
                    }
                    else
                    {
                        pluginState = workloadPluginState.cooldown;

                        cooldownIndex = plugin.plugin_settings.warmingUpTicks;
                    }
                }
            }
        }

        private bool isSampleTail = false;
        private bool isMemoryLimit = false;

        private int cooldownIndex = 0;
        private int wormingUpIndex = 0;

        private string comment = "";


        private string warningUpDate(string message, bool isActive, reportPlugIn_workload plugin)
        {
            if (isActive)
            {
                terminationWarning++;
                if (terminationWarning >= plugin.plugin_settings.term_warningCount)
                {
                    plugin.loger.AppendLine("Termination initiated [" + message + "] :: warning (" + terminationWarning + " / " + plugin.plugin_settings.term_warningCount + ")");
                    comment = comment.add($"Termination:{ message}");
                } else
                {
                    plugin.loger.consoleAltColorToggle();
                    plugin.loger.AppendLine("Termination criterion met [" + message + "] :: warning (" + terminationWarning + " / " + plugin.plugin_settings.term_warningCount + ") issued.");

                    comment = comment.add($"Warninig {terminationWarning}:{message}"); 
                    plugin.loger.consoleAltColorToggle();
                }

                return message;
            } else
            {
                plugin.loger.AppendLine("Termination warnings counter set to 0 .".add(message, " "));
                comment = comment.add($"Warninig reset:{message}");
                terminationWarning = 0;
            }
            return "";
        }


        private reportPlugIn_workloadEntry lastEntry { get; set; } = null;

        public static Regex REGEX_extractEntryID = new Regex(@"^([\w\-_]*)-[\d]{4}-[\d]{4}");

        public static string ExtractEntryID(string EntryID)
        {
            string output = REGEX_extractEntryID.Match(EntryID).Groups[1].Value;
            return output.Replace("-", "").Replace("_", "");
        }

        private string GetEntryID(int __recordID, int __measureGroup)
        {
            return TestID +"-" + __recordID.ToString("D4") + "-" + __measureGroup.ToString("D4");
        }

        public void statePrepare(reportPlugIn_workload_settings plugin_settings)
        {
            TestID = imbWEMManager.index.experimentEntry.TestID;
            Crawler = imbWEMManager.index.experimentEntry.CrawlID;

            RecordID = 0;
            //settings = plugin_settings;
            DLCDoneForNext = plugin_settings.stepUp_DLCCount;

            trendMemory = new measureTrendTaker<performanceResourcesTake>(x => x.physicalMemory, "RAM", "MiB", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendMemory.format = "#,###.##";

            trendCPU = new measureTrendTaker<performanceResourcesTake>(x => x.cpuRateOfProcess, "CPUp", "%", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendCPU.format = "P2";

            trendCPUm = new measureTrendTaker<performanceTake>(x => (x.reading/100), "CPUm", "%", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendCPUm.format = "P2";
            
            trendDataLoad = new measureTrendTaker<performanceResourcesTake>(x => x.bytesLoadedSample * x.PerMinuteFactor, "Data", "MiB/min", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendDataLoad.format = "#,###.##";

            trendContentPages = new measureTrendTaker<performanceDataLoadTake>(x => x.ContentPages * x.PerMinuteFactor, "Pages", "n/min", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendContentPages.format = "#,###.##";

            trendContentTerms = new measureTrendTaker<performanceDataLoadTake>(x => x.ContentTerms * x.PerMinuteFactor, "Terms", "n/min", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendContentTerms.format = "#,###.##";

            trendIterations = new measureTrendTaker<performanceDataLoadTake>(x => x.CrawlerIterations * x.PerMinuteFactor, "Cycles", "i/min", plugin_settings.macroSampleSize, -1, -1, plugin_settings.ZeroMargin);
            trendIterations.format = "#,###.##";
        }


        public reportPlugIn_workload_state()
        {

        }

        [Category("Count")]
        [DisplayName("DLC Target")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Count of finished DLCs that should trigger next measure group")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int DLCDoneForNext { get; set; } = 0;

        /// <summary> current RecordID  </summary>
        [Category("Count")]
        [DisplayName("RecordID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Ordinal ID number of the entry")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int RecordID { get; set; } = 0;




        [Category("Label")]
        [DisplayName("TestID")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the test run: MD5(global settings) - MD5(crawler settings) - [extra 4 characters]")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 70)]
        public string TestID { get; set; } = "";

        [Category("State")]
        [DisplayName("Status")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("State of the reporting plugin")] // [imb(imbAttributeName.reporting_escapeoff)]
        public workloadPluginState pluginState { get; set; } = workloadPluginState.disabled;

        [Category("State")]
        [Description("Ordinal number of the current measure group, -1 means the plugin is not in the active state")]
        [DisplayName("Group")]
        [imb(imbAttributeName.measure_setUnit, "#")]
        [imb(imbAttributeName.measure_letter, "ID")]
        public int measureGroup { get; set; } = 0;

        [Category("State")]
        [Description("Number of termination warnings issued")]
        [DisplayName("Warning Count")]
        [imb(imbAttributeName.measure_setUnit, "")]
        [imb(imbAttributeName.measure_letter, "-")]
        public int terminationWarning { get; set; } = 0;

        /// <summary> Test signature with the most important configuration values </summary>
        [Category("Label")]
        [DisplayName("TestSignature")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Test signature with the most important configuration values")] // [imb(imbAttributeName.reporting_escapeoff)]
        [imb(imbAttributeName.reporting_function, templateFieldDataTable.columnWidth, 60)]
        public string TestSignature { get; set; } = "";




        /// <summary> Name of the crawler design that was tested </summary>
        [Category("Label")]
        [DisplayName("Crawler")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Name of the crawler design that was tested")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string Crawler { get; set; } = default(string);
    }

}