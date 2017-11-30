// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexDBUpdater.cs" company="imbVeles" >
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
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.structure;
    using imbCommonModels.webPage;
    using imbNLP.Data;
    using imbNLP.Data.evaluate;
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
    using imbSCI.DataComplex;
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
    using imbWEM.Core.index.core;
    using imbWEM.Core.index.experimentSession;
    using imbWEM.Core.plugins.index;
    using imbWEM.Core.project;
    using imbWEM.Core.settings;
    using imbWEM.Core.stage;

    public class indexDBUpdater : indexPlugIn_base
    {
        public indexDBUpdater() : base("indexUpdater", "I'm performing page reavaluation, info prize calculation and other indexDomain and indexPage stuff")
        {
        }

        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { indexMaintenanceStageEnum.indexWReportTFIDFStart, indexMaintenanceStageEnum.indexStructureCheck, indexMaintenanceStageEnum.DLCIteration,
                    indexMaintenanceStageEnum.indexMasterTFIDFCheck, indexMaintenanceStageEnum.indexMasterTFIDFApply };
            }
        }

        public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {

          //  imbWEMManager.index.Recheck(loger);

        }


        public override void eventIteration(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            
        }

        public override void eventDLCFinished(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            if (settings.plugIn_indexDBUpdater_TFIDF_per_DLC)
            {
                FileInfo master_file = session.GetTFIDF_Master_File();
                
                session.doDomainEvaluation(settings, loger, __wRecord, evaluator, new weightTableCompiled(master_file.FullName, true, session.SessionID));
                
            }
            else { 

                session.doDomainEvaluation(settings, loger, __wRecord, evaluator, session.GetTFIDF_Master(loger, true, false));
            }

            loger.AppendLine("Last index save: " + imbWEMManager.index.lastIndexSave.ToShortTimeString() 
                + " [" + imbWEMManager.index.wRecordsDeployed + " / " + settings.doIndexAutoSaveOnDLCs + " ] ");

            if (imbWEMManager.index.wRecordsDeployed >= settings.doIndexAutoSaveOnDLCs)
            {
                imbWEMManager.index.Save();
            }

        }

        public override void Dispose()
        {
            //base.Dispose();
        }

        public override void eventDLCInitiated(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
       
           

            if (!__wRecord.tRecord.instance.settings.FRONTIER_doLinkHarvest)
            {

                indexDomain idomain = imbWEMManager.index.domainIndexTable.GetDomain(__wRecord.domainInfo.domainName);
                List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesForDomain(__wRecord.domainInfo.domainName);


                var seedTarget = __wRecord.context.targets.GetLoaded().FirstOrDefault();

                var spage = seedTarget?.page;

                if (spage != null) loger.AppendLine(__wRecord.domain + " seed page selected -> " + spage.url);


                //FileInfo dlcFile = __session.GetTFIDF_DLC_File(idomain);

                foreach (indexPage p in pages)
                {
                    link l = new link(p.url);
                   // if (__wRecord.web.webActiveLinks.Contains())
                    __wRecord.context.processLink(l, spage, false);
                }
            }
        }

        public experimentSessionEntry session { get; set; }
        public IndexEngineConfiguration settings { get; set; }

        public override void eventPluginInstalled()
        {
            session = imbWEMManager.index.experimentEntry;
            aceLog.consoleControl.setAsOutput(loger, "IndexDB:" + session.SessionID);

            settings = imbWEMManager.settings.indexEngine;

            settings.doIndexUpdateOnDLC = false;
            settings.doIndexFullTrustMode = false;


            imbWEMManager.settings.directReportEngine.doPublishPerformance = false;

            

            //globalTFIDFConstruct = __session.GetTFIDF_MasterConstruct(); //new webSitePageTFSet(__spider.SessionID);

            //   globalTFIDFCompiled = __session.GetTFIDF_Master(); // new webSiteLemmaTFSetObjectTable(__session.indexSubFolder.pathFor(experimentSessionEntry.PATH_CompiledFTIDF), true, __session.SessionID);


            // domainTF_IDF = new aceConcurrentDictionary<weightTableCompiled>();



            //if (globalTFIDFCompiled.Count > 0)
            //{
            //    loger.log("TF-IDF compiled version found on: " + globalTFIDFCompiled.info.FullName);
            //}
            // domainAssertion =  imbWEMManager.index.domainIndexTable.GetDomainIndexAssertion(null, true);
            
            evaluator = new multiLanguageEvaluator(basicLanguageEnum.english, basicLanguageEnum.serbian, basicLanguageEnum.serbianCyr);
            evaluator.testTokenLimit = 5000;
            evaluator.tokenLengthMin = 3;
            evaluator.validTokenTarget = 2500;
            

        }

        public multiLanguageEvaluator evaluator { get; set; }

        //public override Enum[] INSTALL_POINTS
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public override void eventUniversal<indexDomain, indexPage>(indexMaintenanceStageEnum stage, experimentSessionEntry __parent, indexDomain __domain, indexPage __page)
        {

        }

        public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as experimentSessionEntry, __task, __wRecord);
       
        //public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        //{
        //    throw new NotImplementedException();
        //}

        public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as experimentSessionEntry, __task, __wRecord);

       
    }

}