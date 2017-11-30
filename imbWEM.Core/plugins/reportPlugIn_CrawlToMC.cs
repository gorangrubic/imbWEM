// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_CrawlToMC.cs" company="imbVeles" >
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
    using imbCommonModels.webStructure;
    using imbMiningContext;
    using imbMiningContext.MCRepository;
    using imbMiningContext.MCWebPage;
    using imbMiningContext.MCWebSite;
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
    using imbSCI.Core.files.fileDataStructure;
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
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    // using reporting.dataUnits;

    public class reportPlugIn_CrawlToMC : reportPlugIn_base
    {
        public reportPlugIn_CrawlToMC() : base("CrawlToMC", "Plugin feeds the crawling data into active MCRepository, creates or updates MCWebSite entries as DLC process is finished")
        {
        }

        private void onTargetPageAttached(modelSpiderSiteRecord __wRecord, modelSpiderSiteRecordEventArgs __args)
        {
            imbMCRepository mcRepo = console.analyticConsole.mainAnalyticConsole.MCManager.activeRepository;
            imbMCWebSite wRepo = webSiteReposByDomain[__wRecord.domain];

            ISpiderTarget target = __args.Target;

            if (mcRepo.isTargetProper(target))
            {
                imbMCWebPage pRepo = mcRepo.BuildWebPage(target, wRepo, loger);
                pRepo.indexEntry = imbWEMManager.index.pageIndexTable.GetPageForUrl(target.url);
                pRepo.HtmlSourceCode = __args.sourceHtml;
                pRepo.XmlSourceCode = __args.sourceXml;

                pRepo.SaveDataStructure(wRepo.folder, loger);
            }


        }

        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { crawlReportingStageEnum.domain, crawlReportingStageEnum.init, crawlReportingStageEnum.DLCPreinitiation};
            }
        }

        protected override string __homePath
        {
            get
            {
                return imbMCManager.MCRepo_DefaultDirectoryName;
            }
        }
        

        public override void Dispose()
        {
            // base.Dispose();
        }

        public override void eventCrawlJobFinished(crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {

            // per module summary


            // all three modules summary

        }

        public override void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {

        }

        public override void eventAtEndOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            
        }


        public override void eventStatusReport(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {

        }


        // private Dictionary<modelSpiderSiteRecord, imbMCWebSite> webSiteRepos = new Dictionary<modelSpiderSiteRecord, imbMCWebSite>();
        private aceConcurrentDictionary<imbMCWebSite> webSiteReposByDomain = new aceConcurrentDictionary<imbMCWebSite>();

        public override void eventPluginInstalled()
        {
            IsEnabled = true;
        }

        public override void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {

        }

        public override void eventDLCInitiated(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {


        }

        

        public override void eventDLCFinished(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            imbMCRepository mcRepo = console.analyticConsole.mainAnalyticConsole.MCManager.activeRepository;
            imbMCWebSite wRepo = webSiteReposByDomain[__wRecord.domain];

            mcRepo.siteTable.AddOrUpdate(wRepo.entry);
            wRepo.SaveDataStructure(mcRepo.folder, loger);

            //  console.analyticConsole.mainAnalyticConsole.MCManager.activeRepository.BuildWebSite(__wRecord.context.targets, __wRecord.domainInfo, loger);

        }



        public override void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord)
        {
            switch (stage)
            {
                case crawlReportingStageEnum.DLCPreinitiation:

                    wRecord.context.OnTargetPageAttached += new modelSpiderSiteRecordEvent(onTargetPageAttached);
                    imbMCRepository mcRepo = console.analyticConsole.mainAnalyticConsole.MCManager.activeRepository;
                    imbMCWebSite wRepo = mcRepo.GetWebSite(wRecord.domainInfo, true, loger);
                    if (!webSiteReposByDomain.ContainsKey(wRecord.domain))
                    {
                        webSiteReposByDomain.Add(wRecord.domain, wRepo);
                    } else
                    {
                        loger.log("DLC sent to CrawlToMC plugin second time: " + wRecord.domain);
                    }

                    mcRepo.siteTable.AddOrUpdate(wRepo.entry);

                    wRepo.SaveDataStructure(mcRepo.folder, loger);
                    break;
            }
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