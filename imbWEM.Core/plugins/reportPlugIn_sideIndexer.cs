// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_sideIndexer.cs" company="imbVeles" >
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
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webPage;
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
    using imbSCI.DataComplex.extensions.data.operations;
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
    using imbWEM.Core.index.core;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    public class reportPlugIn_sideIndexer : reportPlugIn_withRecord<indexDomain>
    {

        public builderForLog output { get; set; } = new builderForLog();

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
                return "plugins\\sideindexer";
            }
        }

        protected override string __recordKeyProperty
        {
            get
            {
                return "domain";
            }
        }


        public string recordFileName { get; set; } = "sideindex_domains.xml";

        protected string recordPath
        {
            get
            {
                return "index\\benchmark\\results.xml";
            }
        }


        public reportPlugIn_sideIndexer() : base("SideIndexer", "Regurarly stores basic DLC data in session report folder, supports after-crash continuation")
        {
        }

        public folderNode crawlerReportFolder { get; set; }



        public override void Dispose()
        {
            // base.Dispose();
        }

        public override void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {

            imbWEMManager.index.domainIndexTable.recheck(imbWEMManager.index.pageIndexTable, output);

            //reportFolder = imbWEMManager.index.experimentEntry.sessionReportFolder;


            //String recordName = imbWEMManager.index.experimentEntry.SessionID.getFilename();

            string path = imbWEMManager.index.experimentEntry.sessionReportFolder.pathFor(recordFileName);

            records = new objectTable<indexDomain>(path, true, __recordKeyProperty, name);
            records.description = "Side index";


            var domains = records.GetList();
            List<string> __url = new List<string>(); // http://www.
            Dictionary<string, indexDomain> dict = new Dictionary<string, indexDomain>();

            domains.ForEach(x => __url.Add(x.url));
            domains.ForEach(x => dict.Add(x.url, x));


            int dc_ik = 0;
            List<crawlerDomainTask> tasks = new List<crawlerDomainTask>();
            foreach (var task in __machine.items.items)
            {
                if (Enumerable.Any(__url, x => x == task.wRecord.instanceID)) // wRecord.instanceID = http://www.
                {
                    task.status = crawlerDomainTaskStatusEnum.aborted;
                    tasks.Add(task);
                }
                else
                {
                    if (imbWEMManager.settings.supportEngine.reportPlugIn_sideIndexer_UseIfPagesKnown)
                    {
                        indexDomain iDomainFromIndex = imbWEMManager.index.domainIndexTable.GetOrCreate(task.wRecord.instanceID);

                        records.AddOrUpdate(iDomainFromIndex,objectTableUpdatePolicy.updateIfHigher);

                        if (dict.ContainsKey(task.wRecord.instanceID))
                        {
                            indexDomain iDomain = dict[task.wRecord.instanceID];
                            if ((iDomain.relevantPages + iDomain.notRelevantPages) >= tRecord.instance.settings.limitTotalPageLoad)
                            {
                                dc_ik++;
                                tasks.Add(task);
                            }
                        }
                    }
                }
            }

            foreach (var task in __machine.items.items)
            {

            }

                int dc = 0;

            foreach (var task in tasks)
            {
                crawlerDomainTask t_out = null;
                if (__machine.items.items.TryDequeue(out t_out)) dc++;
            }

            aceLog.consoleControl.setAsOutput(output, "SideIndex");
            if (dc > 0) output.log("DLCs processed in an earlier session: " + dc);
            if (dc_ik > 0) output.log("DLCs removed from schedule because the index has already enough pages loaded: " + dc_ik);



        }


        private void SaveAll()
        {
            records.Save();

            records.GetDataTable(null, "side_index").GetReportAndSave(imbWEMManager.index.experimentEntry.sessionReportFolder, imbWEMManager.authorNotation, "side_index", true);

        }

        private void exportIndexTablesClean(string filename)
        {
            DataTable dt = imbWEMManager.index.domainIndexTable.GetDataTable();
            dt.ClearData(nameof(indexDomain.AllWords), nameof(indexDomain.AllLemmas));
            dt.GetReportAndSave(imbWEMManager.index.experimentEntry.sessionReportFolder, imbWEMManager.authorNotation, "domain_" + filename, true);

            DataTable dtp = imbWEMManager.index.pageIndexTable.GetDataTable();
            dt.ClearData(nameof(indexPage.AllWords), nameof(indexPage.AllLemmas));
            dt.GetReportAndSave(imbWEMManager.index.experimentEntry.sessionReportFolder, imbWEMManager.authorNotation, "page_" + filename, true);
        }

        public override void eventCrawlJobFinished(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {
            SaveAll();

            exportIndexTablesClean("final");
        }


        public override void eventStatusReport(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            //loger.log(tRecord.)

        }

        public override void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {

        }

        public override void eventDLCInitiated(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            



        }

        public int DLCCount { get; set; } = 0;

        public override void eventDLCFinished(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            if (__task.status == crawlerDomainTaskStatusEnum.aborted)
            {
                return;
            }

            if (__wRecord.iterationTableRecord == null) return;
            if (__wRecord.iterationTableRecord.Count == 0) return;

            indexDomain iDomain = records.GetOrCreate(__wRecord.instanceID);
            iDomain.url = __wRecord.domainInfo.urlProper;
            iDomain.domain = __wRecord.domain;

            var lastRec = __wRecord.iterationTableRecord.LastOrDefault();

            var firstRec = __wRecord.iterationTableRecord.FirstOrDefault();

            iDomain.relevantPages = lastRec.relevantPageCount;
            iDomain.notRelevantPages = lastRec.irrelevantPageCount;
            iDomain.detected = __wRecord.web.webActiveLinks.Count();
            iDomain.Words = __wRecord.context.targets.termsAll.Count();
            iDomain.LandingLanguage = firstRec.targetLanguage;
            iDomain.LandingRelevant = firstRec.relevantPageCount > 0;

            records.AddOrUpdate(iDomain, objectTableUpdatePolicy.updateIfHigher);
            DLCCount++;

            if (DLCCount >= imbWEMManager.settings.supportEngine.reportPlugIn_sideIndexer_DLCToSave)
            {
                DLCCount = 0;
                SaveAll();
                output.log("Side Index save and publish triggered on [" + __task.parent.parent.taskDone + "] DLC completed");
            }

        }



        public override void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord)
        {

        }


        public objectTable<performanceRecord> record_performances { get; protected set; }

        public objectTable<moduleFinalOverview> record_moduleImpact { get; protected set; }


      
        /// <summary>
        /// Just before 
        /// </summary>
        /// <param name="crawlerDomainTaskMachine">The crawler domain task machine.</param>
        /// <param name="tRecord">The t record.</param>
        public override void eventAtEndOfCrawlJob(crawlerDomainTaskMachine __machine, modelSpiderTestRecord tRecord)
        {
            SaveAll();
            exportIndexTablesClean("finish");
        }


        public override void eventPluginInstalled()
        {
            // records = new objectTable<reportPlugIn_benchmarkResults>(recordPath, true, __recordKeyProperty, name);
            //  records.description = "Summary report on the most relevant evaluation metrics.";

            exportIndexTablesClean("initial");

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