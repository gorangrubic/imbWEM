// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_workload.cs" company="imbVeles" >
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
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    // using reporting.dataUnits;


    public class reportPlugIn_workload : reportPlugIn_withRecord<reportPlugIn_workloadEntry>, IPlugInCommonBase
    {


        public reportPlugIn_workload() : base("Workload", "Efficiency monitoring and trend analyser")
        {
           
        }

        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { crawlReportingStageEnum.crawler, crawlReportingStageEnum.finish, crawlReportingStageEnum.statusReport, crawlReportingStageEnum.start,
                    crawlReportingStageEnum.domain, crawlReportingStageEnum.iteration, crawlReportingStageEnum.init};
            }
        }

        protected override string __homePath
        {
            get
            {
                return "index\\workload";
            }
        }

        protected override string __recordKeyProperty
        {
            get
            {
                return "EntryID";
            }
        }

        protected string __recordPath { get; set; } = "index\\workload\\results.xml";

        public string recordPath
        {
            get
            {
                return __recordPath;
            }
        }

        public reportPlugIn_workload_state plugin_state { get; set; } = new reportPlugIn_workload_state();
        public reportPlugIn_workload_settings plugin_settings { get; set; } = new reportPlugIn_workload_settings();



        public override void Dispose()
        {
            // base.Dispose();
        }

        public override void eventCrawlJobFinished(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord)
        {

            records.Save();
            records.GetDataTable(null, plugin_state.TestID).GetReportAndSave(imbWEMManager.index.experimentEntry.sessionCrawlerFolder, imbWEMManager.authorNotation, "workload_", true);


        }

        public override void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord)
        {
            plugin_settings = imbWEMManager.settings.supportEngine.plugIn_workload_settings;
            plugin_state.statePrepare(plugin_settings);

            string ad = plugin_settings.stepUp_start.ToString() + plugin_settings.stepUp_step.ToString();

            __recordPath = homeFolder.pathFor("results_" + tRecord.instance.name + ad + ".xml");

            records = new objectTable<reportPlugIn_workloadEntry>(recordPath, false, __recordKeyProperty, plugin_state.TestID);

            if (plugin_settings.stepUp_enabled)
            {
                plugin_state.pluginState = workloadPluginState.preparing;
                _machine.maxThreads = plugin_settings.stepUp_start;

            } else
            {
                plugin_state.pluginState = workloadPluginState.disabled;
            }
            
        }

        public override void eventAtEndOfCrawlJob(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord)
        {

        }

  


        public override void eventStatusReport(crawlerDomainTaskMachine _machine, modelSpiderTestRecord tRecord)
        {
            plugin_state.stateUpdate(_machine, tRecord, this, imbWEMManager.index.experimentEntry);
        }




        public override void eventPluginInstalled()
        {

        }

        public override void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {

        }

        public override void eventDLCInitiated(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            plugin_state.doCheckCriteria(__task.parent.parent, __wRecord.tRecord, this, imbWEMManager.index.experimentEntry);

        }

        public override void eventDLCFinished(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            plugin_state.doCheckCriteria(__task.parent.parent, __wRecord.tRecord, this, imbWEMManager.index.experimentEntry);
        }



        public override void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord)
        {

        }


        /// <summary>
        /// It is called when the software starts to shutdown
        /// </summary>
        public override void onExit()
        {

        }

        public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord) => eventCrawlJobFinished(__machine, __tRecord);

        public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as directReporterBase, __task, __wRecord);


        public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as directReporterBase, __task, __wRecord);



 


       
    }

}