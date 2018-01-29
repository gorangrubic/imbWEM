// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexMasterTFIDFConstructor.cs" company="imbVeles" >
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
    using imbSCI.DataComplex.tables;
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
    using imbWEM.Core.stage;

    public class indexMasterTFIDFConstructor : indexPlugIn_base
    {
      //  [XmlIgnore]
       // public webSitePageTFSet globalTFIDFConstruct { get; set; }

        [XmlIgnore]
        public weightTableCompiled globalTFIDFCompiled { get; set; }



        public indexMasterTFIDFConstructor() : base("mTFIDFCreator", "I'm constructing the master TF IDF table")
        {
        }



        public override Enum[] INSTALL_POINTS
        {
            get
            {
                return new Enum[] { indexMaintenanceStageEnum.indexWReportTFIDFStart, indexMaintenanceStageEnum.indexStructureCheck,
                    indexMaintenanceStageEnum.indexMasterTFIDFCheck, indexMaintenanceStageEnum.indexMasterTFIDFApply };
            }
        }

        public override void Dispose()
        {
            //base.Dispose();
        }

        public override void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {
            string indexID = imbWEMManager.index.current_indexID;

            var MasterTFIDF = imbWEMManager.index.experimentManager.CurrentSession.GetTFIDF_Master(loger, false, true);

            loger.log("Master TF-IDF [" + MasterTFIDF.name + "] has [" + MasterTFIDF.Count + "] terms defined.");

            MasterTFIDF.GetDataTable().GetReportAndSave(imbWEMManager.index.experimentManager.CurrentSession.sessionReportFolder, imbWEMManager.authorNotation, "master_tf_idf", true);



            //loger.log("--- saved to: " + path); // + " + globalTFIDFConstruct.Count() + "] DLC -> aggregated into MasterTF_IDF table by crawler [" + __tRecord.name + "]");




            //String path = imbWEMManager.index.experimentEntry.crawlRecordFolder.pathFor(indexID.add("xml", "."), aceCommonTypes.enums.getWritableFileMode.overwrite);




            //  imbWEMManager.index.experimentEntry.globalTFIDFCompiled = globalTFIDFConstruct.AggregateDocument.GetCompiledTable(loger);

            //loger.log("[" + imbWEMManager.index.experimentEntry.globalTFIDFCompiled.Count + "] terms were aggregated from [" + __tRecord.name + "] crawl data");

            /*
            tfd.SetTitle("MasterTFIDF");

            tfd.saveObjectToXML(path);
            tfd.GetReportAndSave(imbWEMManager.index.experimentEntry.recordFolder, imbWEMManager.authorNotation, "lemma", true);


            path = imbWEMManager.index.folder.pathFor(experimentSessionRegistry.PATH_AggregateFTIDF, aceCommonTypes.enums.getWritableFileMode.overwrite);

            tfd.saveObjectToXML(path);
            tfd.GetReportAndSave(imbWEMManager.index.folder, imbWEMManager.authorNotation, "lemma", true);

            loger.log("[" + globalTFIDFConstruct.Count() + "] DLC -> aggregated into MasterTF_IDF table by crawler [" + __tRecord.name + "]");
            loger.log("--- saved to: " + path); // + " + globalTFIDFConstruct.Count() + "] DLC -> aggregated into MasterTF_IDF table by crawler [" + __tRecord.name + "]");



            var allTerms = globalTFIDFConstruct.AggregateDocument.GetAllTerms();
            
            Double IPd = 0;

            foreach (termSpark t in allTerms)
            {
                var tfidf_entry =  globalTFIDFCompiled.GetOrCreate(t.nominalForm);
                tfidf_entry.termInstanceList = t.GetAllTermString();
                tfidf_entry.termInstances = tfidf_entry.termInstanceList.toCsvInLine();
                tfidf_entry.freqNorm = globalTFIDFConstruct.AggregateDocument.GetNFreq(t);
                tfidf_entry.freqAbs = globalTFIDFConstruct.AggregateDocument.GetAFreq(t);
                tfidf_entry.df = globalTFIDFConstruct.AggregateDocument.GetBDFreq(t);
                tfidf_entry.idf = globalTFIDFConstruct.AggregateDocument.GetIDF(t);
                tfidf_entry.tf_idf = globalTFIDFConstruct.AggregateDocument.GetTF_IDF(t);
                globalTFIDFCompiled.AddOrUpdate(tfidf_entry);
                IPd += tfidf_entry.tf_idf;
            }


            loger.log("[" + globalTFIDFConstruct.Count() + "] IP sum ->  [" + IPd.ToString("F3") + "]");


            path = imbWEMManager.index.folder.pathFor(experimentSessionRegistry.PATH_CompiledFTIDF, aceCommonTypes.enums.getWritableFileMode.overwrite);
            globalTFIDFCompiled.SaveAs(path);
            DataTable gdt = globalTFIDFCompiled.GetDataTable();
            
            // <--------- ubaciti dopunske informacije
            //gdt.SetAdditionalInfoEntry("Crawled domains", )
            gdt.GetReportAndSave(imbWEMManager.index.folder, imbWEMManager.authorNotation, "tf_idf_compiled");

            */

            /*
            var domains = __tRecord. //__spider.state.sampleList.getIndexDomains();

            distinct = new List<string>();
            allterms = new List<string>();
            foreach (IWeightTableTerm t in wTFIDF.GetAllTerms())
            {
                if (wTFIDF.GetBDFreq(t) == 1)
                {
                    distinct.Add(t.nominalForm);
                }
                allterms.Add(t.nominalForm);
            }

            idomain.DistinctLemmas = distinct.toCsvInLine();

            allterms.saveContentOnFilePath(__spider.indexSubFolder.pathFor(idomain.domain.getFilename(".txt"), aceCommonTypes.enums.getWritableFileMode.overwrite));
            */



            // -----------------------------------------------

            //var domains = imbWEMManager.index.experimentEntry.state.sampleList.getIndexDomains();
            //// <---------- nije zavrseno

            //foreach (indexDomain idomain in domains)
            //{
            //    List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesForDomain(idomain.domain);



            //    imbWEMManager.index.domainIndexTable.AddOrUpdate(idomain);
            //}



        }

       

        public override void eventDLCFinished(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            weightTableCompiled DLC_TDF = __session.GetOrCreateTFIDF_DLC(__wRecord, loger, imbWEMManager.settings.TFIDF.doUseCachedDLCTables, imbWEMManager.settings.TFIDF.doSaveCacheOfDLCTables, evaluator);

         //   domainTF_IDF.Add(__wRecord.domain, DLC_TDF);

            /*


            indexDomain idomain = imbWEMManager.index.domainIndexTable.GetOrCreate(__wRecord.domain);

            List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesForDomain(__wRecord.domain);


            loger.log("[" + idomain.domain + "] application of DLC TF-IDF");

            
            allterms = new List<string>();
            List<String> DLCTerms = new List<string>();
            ti = 0;
            tc = pages.Count;
            foreach (indexPage ipage in pages)
            {
                //if (ipage.relevancy == indexPageRelevancyEnum.isRelevant)
                //{

                    spiderTarget tPage = __wRecord.context.targets.GetByURL(ipage.url); // tLoaded.FirstOrDefault(x => (x.key == __wRecord.context.targets.GetHash(ipage.url)));
                    
                    if (!selected.Contains(tPage))
                    {
                        continue;
                    }

                    if (tPage == null)
                    {
                        loger.log("-- page: " + ipage.url + " [not found in the crawler context of: " + idomain.url);
                        continue;
                    }

                    // __wRecord.context.targets.GetByURL(ipage.url);
                    termDocument dPage = (termDocument)domainSet[tPage.pageHash];
                   

                    if (dPage == null)
                    {
                        continue;
                    }

                    dPage.expansion = 0;
                    distinct = new List<string>();


                    var wt = dPage.GetAllTerms();
                    foreach (IWeightTableTerm t in wt)
                    {
                        if (dPage.GetBDFreq(t) == 1)
                        {
                            distinct.Add(t.nominalForm);
                        }
                        allterms.Add(t.nominalForm);
                    }

                    ipage.DistinctLemmas = distinct.toCsvInLine();
                    ipage.RelevantTerms = allterms.toCsvInLine();
                    ipage.TFIDFcompiled = true;

                    DLCTerms.AddRangeUnique(allterms);

                    dPage.GetDataTableClean(ipage.HashCode).saveObjectToXML(__session.indexSubFolder.pathFor(GetCompbinedHash(idomain, ipage) + ".xml"));

                    ti++;
                    Double tp = ti.GetRatio(tc);
                    aceLog.consoleControl.writeToConsole(tp.ToString("P2"), loger, false, 0);

                    imbWEMManager.index.pageIndexTable.AddOrUpdate(ipage);
                //}
            }

            loger.log("[" + idomain.domain + "] application of DLC TF-IDF (done)");


            loger.log("[" + idomain.domain + "] constructing DLC TF-IDF for Master TF-IDF (semantic compression)");

            // -------------
            //var sparks = DLCTerms.getSparks(1, loger, false);

            webPageTF wTFIDF = globalTFIDFConstruct.AddTable(idomain.HashCode) as webPageTF;
            
            wTFIDF.AddPageTerms(allterms, 0, loger);
            
            //wTFIDF.AddTokens(DLCTerms, loger);

            String path = __session.indexSubFolder.pathFor(idomain.HashCode + ".xml").getWritableFile().FullName;
            wTFIDF.GetDataTable("Lemma" + idomain.domain, null, false).saveObjectToXML(path);

            

            idomain.Lemmas = wTFIDF.Count();

            imbWEMManager.index.domainIndexTable.AddOrUpdate(idomain);

            loger.log("[" + idomain.domain + "] TF-IDF operations done");
            */
        }

        public override void eventDLCInitiated(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            // imbWEMManager.index.domainIndexTable
            var state = __session.state;


            indexDomain idomain = imbWEMManager.index.domainIndexTable.GetDomain(__wRecord.domainInfo.domainName);
            List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesForDomain(__wRecord.domainInfo.domainName);

            /*
            __session.state.crawler.settings.FRONTIER_doLinkHarvest = false;
            __session.state.crawler.settings.FRONTIER_doLinkResolver = false;
            */

            var seedTarget = __wRecord.context.targets.GetLoaded().FirstOrDefault();
            //.webPages.items.Values.First();
            var spage = seedTarget?.page;

            if (spage != null) loger.AppendLine(__wRecord.domain + " seed page selected -> " + spage.url);


            FileInfo dlcFile = __session.GetTFIDF_DLC_File(idomain);

            if ((!dlcFile.Exists) || imbWEMManager.settings.TFIDF.doSchedulePagesWithDLCTable)
            {

                foreach (indexPage p in pages)
                {
                    link l = new link(p.url);

                    if (!p.url.Contains(__wRecord.domainInfo.domainRootName))
                    {
                        loger.AppendLine(__wRecord.domain + " -X-> " + p.url + " Wrong link association?");
                       aceTerminalInput.doBeepViaConsole(1600, 200, 3);
                    }

                    __wRecord.context.processLink(l, spage, false);
                }

                loger.AppendLine(__wRecord.domain + " -> " + __wRecord.web.webActiveLinks.Count + " targets set for load");
            } else
            {
                loger.AppendLine(__wRecord.domain + " -> DLC cache found: " + dlcFile.FullName);
            }
            
        }

        public multiLanguageEvaluator evaluator { get; set; }

        public indexDomainAssertionResult domainAssertion { get; set; }

    //    public aceConcurrentDictionary<weightTableCompiled> domainTF_IDF { get; set; } = new aceConcurrentDictionary<weightTableCompiled>();

        //protected 



        public override void eventPluginInstalled()
        {
            experimentSessionEntry session = imbWEMManager.index.experimentEntry;
            aceLog.consoleControl.setAsOutput(loger, "TFIDF:" + session.SessionID);

         //   globalTFIDFConstruct = session.GetTFIDF_MasterConstruct(); //new webSitePageTFSet(__spider.SessionID);
            
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
        

        public override void eventUniversal<indexDomain, indexPage>(indexMaintenanceStageEnum stage, experimentSessionEntry __parent, indexDomain __domain, indexPage __page)
        {
            
        }

        public override void eventDLCFinished<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCFinished(__parent as experimentSessionEntry, __task, __wRecord);


        public override void eventDLCInitiated<TParent>(TParent __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) => eventDLCInitiated(__parent as experimentSessionEntry, __task, __wRecord);

        public override void eventIteration(experimentSessionEntry __session, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
          
        }
    }

}