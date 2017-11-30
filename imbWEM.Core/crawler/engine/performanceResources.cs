// --------------------------------------------------------------------------------------------------------------------
// <copyright file="performanceResources.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.engine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
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
    using imbWEM.Core.stage;

    /// <summary>
    /// Multiple resources monitoring
    /// </summary>
    /// <seealso cref="performanceBase{T}.spider.engine.performanceResourcesTake}" />
    public class performanceResources : performanceBase<performanceResourcesTake>
    {
        public override int secondsBetweenTakesDefault
        {
            get
            {
                return imbWEMManager.settings.supportEngine.monitoringSampling;
            }
        }

        public performanceResources(string __name, crawlerDomainTaskMachine __cDTM):base(__name)
        {
            cDTM = __cDTM;
        }

        private PerformanceCounter cpuPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter memoryPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter freeMemoryPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter allMemoryPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter diskReadsPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter diskWritesPerformanceCounter = new PerformanceCounter();
        private PerformanceCounter diskTransfersPerformanceCounter = new PerformanceCounter();

        protected PerformanceCounter pcProcess { get; set; }

        public crawlerDomainTaskMachine cDTM { get; set; }

        public TimeSpan start { get; set; }
        public TimeSpan oldCPUTime { get; set; } = new TimeSpan(0);
        public DateTime lastMonitorTime { get; set; } = DateTime.UtcNow;
        public DateTime StartTime = DateTime.UtcNow;

        public static double CPUUsageTotal
        {
            get;
            private set;
        }

        public static double CPUUsageLastMinute
        {
            get;
            private set;
        }

        public const double MEM_UNIT = 1048576;

        public override void measure(performanceResourcesTake t)
        {
            process.Refresh();

            TimeSpan newCPUTime = process.TotalProcessorTime - start;
            CPUUsageLastMinute = (newCPUTime - oldCPUTime).TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(lastMonitorTime).TotalSeconds);
            lastMonitorTime = DateTime.UtcNow;
            CPUUsageTotal = newCPUTime.TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(StartTime).TotalSeconds);
            oldCPUTime = newCPUTime;

            t.cpuRateOfProcess = CPUUsageLastMinute;



            t.pagedMemory = process.PagedMemorySize64 / MEM_UNIT;
            t.physicalMemory = process.WorkingSet64 / MEM_UNIT;
            t.virtualMemory = process.VirtualMemorySize64 / MEM_UNIT;

            t.availableMemory = freeMemoryPerformanceCounter.NextValue();
            t.totalMemory = t.physicalMemory + t.availableMemory;

            t.diskRead = diskReadsPerformanceCounter.NextValue() / MEM_UNIT;
            t.diskWrite = diskWritesPerformanceCounter.NextValue() / MEM_UNIT;
            
            t.dlcRunning = cDTM.task_running.Count();
            t.dlcWaiting = cDTM.task_waiting.Count();
            t.dlcCanceled = cDTM.task_canceled.Count();
            t.pageLoadsRealTotal = cDTM.dataLoadTaker.pageLoads;
            t.bytesLoadedTotal = cDTM.dataLoadTaker.totalBytes / MEM_UNIT;

            if (lastTake != null)
            {
                t.pageLoadsRealSample = t.pageLoadsRealTotal - lastTake.pageLoadsRealTotal;
                t.bytesLoadedSample = t.bytesLoadedTotal - lastTake.bytesLoadedTotal;
            }
            
        }

        protected Process process { get; set; }

       

        public override void prepare()
        {
            
            process = Process.GetCurrentProcess();
            start = process.TotalProcessorTime;

            pcProcess = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
            pcProcess.NextValue();

            cpuPerformanceCounter.CategoryName = "Processor";
            cpuPerformanceCounter.CounterName = "% Processor Time";
            cpuPerformanceCounter.InstanceName = "_Total";

            cpuPerformanceCounter.NextValue();

            

            diskReadsPerformanceCounter.CategoryName = "PhysicalDisk";
            diskReadsPerformanceCounter.CounterName = "Disk Read Bytes/sec";
            diskReadsPerformanceCounter.InstanceName = "_Total";
            diskReadsPerformanceCounter.NextValue();

            diskWritesPerformanceCounter.CategoryName = "PhysicalDisk";
            diskWritesPerformanceCounter.CounterName = "Disk Write Bytes/sec";
            diskWritesPerformanceCounter.InstanceName = "_Total";

            freeMemoryPerformanceCounter = new PerformanceCounter("Memory", "Available MBytes");

            freeMemoryPerformanceCounter.NextValue();

            diskWritesPerformanceCounter.NextValue();
            


        }
    }
}
