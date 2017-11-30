// --------------------------------------------------------------------------------------------------------------------
// <copyright file="experimentSessionEntry.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.experimentSession
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webPage;
    using imbNLP.Data;
    using imbNLP.Data.evaluate;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.morphology;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.Data.semanticLexicon.term;
    using imbNLP.Transliteration;
    using imbSCI.Core.attributes;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.settings;
    using imbWEM.Core.stage;

    [imb(imbAttributeName.reporting_categoryOrder, "session")]
    public class experimentSessionEntry:performanceRecord
    {



        /// <summary> Sample source - name of file or subset </summary>
        [Category("Label")]
        [DisplayName("SampleSource")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Sample source - name of file or subset")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SampleSource { get; set; } = default(string);


        /// <summary> If <c>true</c> it will randomize order of domains in the Crawl Job domain list </summary>
        [Category("Flag")]
        [DisplayName("SampleRandomOrder")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will randomize order of domains in the Crawl Job domain list")]
        public bool SampleRandomOrder { get; set; } = true;



        /// <summary>  </summary>
        [Category("Label")]
        [DisplayName("SampleListHash")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SampleListHash { get; set; } = "";




        public void StartSession(string __CrawlID, indexPerformanceEntry __indexID, string __SessionID, analyticConsoleState __state)
        {
            CrawlID = __CrawlID;
            SessionID = __SessionID;
            state = __state;

            var tmpFolder = new folderNode("reportOutput", "reporting module", "");

            sessionReportFolder = tmpFolder.createDirectory(SessionID, "", imbWEMManager.settings.directReportEngine.doAutoRenameSessionFolder); //  Directory.CreateDirectory(path);

            TestID = CrawlID + "-" + SessionID;
        
            ReportPath = sessionReportFolder.path;

            sessionCrawlerFolder = sessionReportFolder.createDirectory(__CrawlID, "Report folder for Crawl [" + __CrawlID + "] - part of session: " + SessionID);



            indexSubFolder = imbWEMManager.index.folder; //.createDirectory(__indexID.IndexRepository, "Index folder fo sub index", false);

            TFIDF_ConstructFolder = imbWEMManager.index.folder; //.createDirectory(SessionID, "TFIDF cache files for this session", false);

            FileInfo master_file = GetTFIDF_Master_File();

            if (globalTFIDFCompiled == null)
            {
                globalTFIDFCompiled = new weightTableCompiled(master_file.FullName, true, SessionID);
                globalTFIDFCompiled.ReadOnlyMode = true;
            }

            SampleRandomOrder = imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleOrder;
            SampleSource = state.sampleTags.add(state.sampleFile, ";");

            SampleListHash = randomizeSample();
            
            

        }

        public string randomizeSample()
        {
            string hash = "-- Sample Randomizer Disabled --";
            if (!SampleRandomOrder) return hash;
            Random rnd = new Random();

            if (state.sampleList != null)
            {
                if (state.sampleList.Any())
                {
                    state.sampleList.Randomize();
                    return md5.GetMd5Hash(state.sampleList.JoinInFormat());
                }
            }
            return "-- Sample Set not ready --";
        }

        public FileInfo GetWordList_File(indexDomain idomain, indexPage ipage)
        {
            return TFIDF_ConstructFolder.pathFor("p_words_" + idomain.HashCode + "-" + ipage.HashCode + ".txt").getWritableFile(getWritableFileMode.newOrExisting);
        }

        public FileInfo GetWordList_File(indexDomain idomain)
        {
            return TFIDF_ConstructFolder.pathFor("d_words_" + idomain.HashCode + ".txt").getWritableFile(getWritableFileMode.newOrExisting);
        }

        public FileInfo GetTFIDF_DLC_File(indexDomain idomain, getWritableFileMode mode=getWritableFileMode.newOrExisting)
        {
            return TFIDF_ConstructFolder.pathFor("dlc_" + idomain.HashCode + ".xml").getWritableFile(mode);
        }

        public weightTableCompiled GetTFIDF_DLC(indexDomain idomain, getWritableFileMode mode = getWritableFileMode.newOrExisting)
        {
            FileInfo fi = new FileInfo (TFIDF_ConstructFolder.pathFor("dlc_" + idomain.HashCode + ".xml")); //.getWritableFile(mode);
            return new weightTableCompiled(fi.FullName, true, idomain.domain);
        }


        public FileInfo GetTFIDF_Master_File(getWritableFileMode mode = getWritableFileMode.newOrExisting)
        {
            return TFIDF_ConstructFolder.pathFor("master_tf-idf.xml").getWritableFile(mode);
        }


        /// <summary>
        /// Closes the session --- saves TF-IDF
        /// </summary>
        /// <param name="tRecords">The t records.</param>
        public void CloseSession(IEnumerable<modelSpiderTestRecord> tRecords)
        {
            // <------------ ako bude sta trebalo

            if (imbWEMManager.settings.TFIDF.doSaveMasterTFIDFonEndOfCrawl)
            {


                if (globalTFIDFCompiled != null)
                {
                    globalTFIDFCompiled.GetDataTable("master_tf-idf_" + SessionID).GetReportAndSave(sessionReportFolder, imbWEMManager.authorNotation, "master_tf-idf_", true);
                    globalTFIDFCompiled.Save();
                    globalTFIDFCompiled.ReadOnlyMode = true;
                }
            }

            var tRecord = tRecords.FirstOrDefault();
            if (tRecord != null)
            {
                this.setObjectBySource(tRecord.performance);
                
            }

            if (imbWEMManager.settings.directReportEngine.doPublishIndexPerformanceTable)
            {
                imbWEMManager.index.indexSessionRecords.GetDataTable().GetReportAndSave(sessionReportFolder, imbWEMManager.authorNotation, "index_operation_records", true);
            }

            if (imbWEMManager.settings.directReportEngine.doPublishExperimentSessionTable)
            {
                imbWEMManager.index.experimentManager.GetDataTable().GetReportAndSave(sessionReportFolder, imbWEMManager.authorNotation, "experiment_session_records", true);
            }


        }
        public weightTableCompiled LoadNewTFIDF_Master(bool loadNew=true)
        {
            if (loadNew)
            {
                FileInfo master_file = GetTFIDF_Master_File();

                var output = new weightTableCompiled(master_file.FullName, true, SessionID);
                output.ReadOnlyMode = true;
                return output;
            } else
            {
                FileInfo master_file = GetTFIDF_Master_File();

                if (globalTFIDFCompiled == null)
                {
                    globalTFIDFCompiled = new weightTableCompiled(master_file.FullName, true, SessionID);
                    globalTFIDFCompiled.ReadOnlyMode = true;
                }
                return globalTFIDFCompiled;
            }
        }

        public List<weightTableCompiled> GetTFIDF_DLC_AllCached(builderForLog loger=null)
        {
            List<weightTableCompiled> allDLC_TFIDFs = new List<weightTableCompiled>();

            List<string> DLC_TFIDF_Files = TFIDF_ConstructFolder.findFiles("dlc_*.xml");

            if (loger != null) loger.log("[" + DLC_TFIDF_Files.Count + "] DLC TFIDF files detected in the cache folder [" + TFIDF_ConstructFolder.path + "]");

           

            int tc = DLC_TFIDF_Files.Count;
            double tr = 0;
            int c = 0;


            foreach (string fPath in DLC_TFIDF_Files)
            {
                c++;
                weightTableCompiled dlc = new weightTableCompiled(fPath, true, c.ToString("D5"));

                allDLC_TFIDFs.Add(dlc);

                tr = c.GetRatio(tc);
                if (loger != null) aceLog.consoleControl.writeToConsole(tr.ToString("P2") + " ", loger, false, 0);


            }
            return allDLC_TFIDFs;
        }


        /// <summary>
        /// Gets the tfidf master: loads from file or returns any existing instance
        /// </summary>
        /// <returns></returns>
        public weightTableCompiled GetTFIDF_Master(builderForLog loger, bool __useExisting=true, bool __saveToCache=true)
        {
            bool rebuild = !__useExisting;
            FileInfo master_file = GetTFIDF_Master_File();

            if (globalTFIDFCompiled == null)
            {
                globalTFIDFCompiled = new weightTableCompiled(master_file.FullName, __useExisting, SessionID);
                globalTFIDFCompiled.ReadOnlyMode = true;
            }

            if (globalTFIDFCompiled.Count == 0)
            {
                rebuild = true;
            } else
            {
                if (loger != null) loger.log("Master table loaded [" + globalTFIDFCompiled.Count + "]");
            }

            if (rebuild)
            {

                int input_c = 0;
                int output_c = 0;
                
                List<weightTableCompiled> allDLC_TFIDFs = GetTFIDF_DLC_AllCached(loger);

                if (loger != null) loger.log("Rebuilding Master Table ");

                termDocumentSet construct = new termDocumentSet(SessionID, "Temporary TF-IDF construct table for session: " + SessionID);

                int tc = allDLC_TFIDFs.Count;
                double tr = 0;
                int c = 0;

                foreach (weightTableCompiled dlc in allDLC_TFIDFs)
                {
                    c++;

                    termDocument td = construct.Add(dlc) as termDocument;
                    input_c += td.Count();
                     
                    tr = c.GetRatio(tc);
                    if (loger != null) aceLog.consoleControl.writeToConsole(tr.ToString("P2")+" ", loger, false, 0);
                    
                   // output_c = construct.AggregateDocument.Count();
                }

                globalTFIDFCompiled = construct.AggregateDocument.GetCompiledTable(loger);
                output_c = construct.AggregateDocument.Count();

                tr = input_c.GetRatio(output_c);
                if (loger != null) loger.log("Master Table - final semantic compression rate: [" + tr.ToString("P2") + "]");
            }

            if (__saveToCache)
            {
                if (loger != null) loger.log("Master Table saved to:[" + master_file.FullName + "]"); // Namesemantic compression rate: [" + tr.ToString("P2") + "]");
                globalTFIDFCompiled.SaveAs(master_file.FullName, getWritableFileMode.overwrite);
            }

            return globalTFIDFCompiled;
        }

        //public webSitePageTFSet GetTFIDF_MasterConstruct()
        //{
        //    if (globalTFIDFSet == null)
        //    {
        //        globalTFIDFSet = new webSitePageTFSet(SessionID, "Temporary TF-IDF table for master table construction");
                
        //    }
        //    return globalTFIDFSet;
        //}

        private List<string> GetDLCTerms_Heuristics(modelSpiderSiteRecord __wRecord, builderForLog loger, bool __useExisting, bool __saveToCache, multiLanguageEvaluator evaluator, indexDomain idomain)
        {
            List<string> allTerms = new List<string>();

            List<string> DLCTerms = new List<string>();

            FileInfo dlcWordList = GetWordList_File(idomain);
            
            if (dlcWordList.Exists && __useExisting)
            {
                DLCTerms = dlcWordList.FullName.openFileToList(true);
                return DLCTerms;
            }

            var tLoaded = __wRecord.context.targets.GetLoaded();
            int tc = tLoaded.Count;
            int ti = 0;
            int ts = 10;
            int c = 0;
            double tp = 0;

            foreach (spiderTarget target in tLoaded)
            {
                ti++;
                c++;
                tp = ti.GetRatio(tc);

                if (target.IsRelevant)
                {
                    string cont = target.pageText.transliterate();
                    cont = WebUtility.HtmlDecode(cont);
                   // cont = cont.imbHtmlDecode();
                   
                    allTerms.AddRange(cont.getTokens(true, true, true, true, 4)); //, loger);


                }

                if (c > 10)
                {
                    c = 0;
                    aceLog.consoleControl.writeToConsole("Pages processed [" + tp.ToString("P2") + "]", loger, false, 0);
                }
            }

            multiLanguageEvaluation evaluation = evaluator.evaluate(allTerms, null, null);

            DLCTerms.AddRange(evaluation.singleLanguageTokens);

            if (!imbWEMManager.settings.TFIDF.doUseOnlySingleMatch) DLCTerms.AddRange(evaluation.multiLanguageTokens);

            DLCTerms = semanticLexiconManager.lexiconCache.encodeTwins(DLCTerms);

            if (imbWEMManager.settings.TFIDF.doSaveDomainWordList)
            {
                if (__saveToCache)
                {
                    DLCTerms.saveContentOnFilePath(dlcWordList.FullName);
                }
            }

            return DLCTerms;
        }

        public weightTableCompiled GetOrCreateTFIDF_DLC_Heuristic(modelSpiderSiteRecord __wRecord, builderForLog loger, bool __useExisting, bool __saveToCache, multiLanguageEvaluator evaluator = null)
        {
            indexDomain idomain = imbWEMManager.index.domainIndexTable.GetOrCreate(__wRecord.domain);

            FileInfo TFIDF_DLC_File = GetTFIDF_DLC_File(idomain, getWritableFileMode.existing);
            weightTableCompiled TFIDF_DLC = null;

            if (TFIDF_DLC_File.Exists && __useExisting)
            {
                TFIDF_DLC = new weightTableCompiled(TFIDF_DLC_File.FullName, true, idomain.domain + "_DLC_TF_IDF");

                loger.log("DLC TF-IDF[" + TFIDF_DLC.Count + "] cache found for: " + idomain.domain);
                return TFIDF_DLC;
            }

            // <--------------- evaluator selection

            if (evaluator == null) evaluator = __wRecord.tRecord.evaluator;

            loger.log("DLC TF-IDF heuristic construction for: " + idomain.domain + " initiated.");

            termDocument domainTable = new termDocument();
            domainTable.expansion = 1;

            double tp = 0;

            var DLCTerms = GetDLCTerms_Heuristics(__wRecord, loger, __useExisting, __useExisting, evaluator, idomain);



            domainTable.AddTokens(DLCTerms, loger);

            tp = domainTable.Count().GetRatio(DLCTerms.Count); // allTerms.Count.GetRatio(tc);

            loger.log("[" + idomain.domain + "] preprocess finished. DLC TF-IDF terms [" + domainTable.Count() + "] - Semantic compression: " + tp.ToString("P2"));

            TFIDF_DLC = domainTable.GetCompiledTable(loger);
            TFIDF_DLC.name = "DLC-TFIDF " + idomain.domain;
            
            return TFIDF_DLC;

        }


        /// <summary>
        /// Gets the information prize for terms specified
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public double GetInfoPrizeForTerms(IEnumerable<string> terms, builderForLog loger = null)
        {
            double output = 0;
            if (globalTFIDFCompiled != null)
            {
                List<IWeightTableTerm> mchl = globalTFIDFCompiled.GetMatches(terms);
                foreach (weightTableTermCompiled cterm in mchl)
                {
                    output += cterm.tf_idf;
                }
            }
            return output;
        }

        public List<string> GetTermsForPage(spiderTarget target, indexDomain idomain=null, indexPage ipage=null, multiLanguageEvaluator evaluator = null, builderForLog loger = null)
        {

            if (idomain == null) idomain = imbWEMManager.index.domainIndexTable.GetOrCreate(target.parent.wRecord.domain);
            if (ipage == null) ipage = imbWEMManager.index.pageIndexTable.GetOrCreate(md5.GetMd5Hash(target.url));

            List<string> output = new List<string>();

            FileInfo file = GetWordList_File(idomain, ipage);

            if (imbWEMManager.settings.TFIDF.doUseSavedPageWordlists && file.Exists)
            {
                output = file.FullName.openFileToList(true);

                

                return output;
            }

            string cont = target.pageText.transliterate();
           // cont = cont.imbHtmlDecode();

            termDocument pageTF = null;

            if (evaluator == null) evaluator = target.parent.wRecord.tRecord.evaluator;

            multiLanguageEvaluation evaluation = evaluator.evaluate(cont);

            if (evaluation.result_language == basicLanguageEnum.serbian)
            {

                
                List<string> pt = new List<string>();

                pt.AddRange(evaluation.singleLanguageTokens);

                if (!imbWEMManager.settings.TFIDF.doUseOnlySingleMatch) pt.AddRange(evaluation.multiLanguageTokens);

                pt.RemoveAll(x => !x.isCleanWord());
                pt.RemoveAll(x => x.isSymbolicContentOnly());

                var tkns = semanticLexiconManager.lexiconCache.encodeTwins(pt);

                
                    output.AddRange(tkns);
                
            }

            if (imbWEMManager.settings.TFIDF.doSavePageWordlist)
            {
                output.saveContentOnFilePath(file.FullName);
            }

            return output;
        }

        /// <summary>
        /// Gets the or create tfidf DLC.
        /// </summary>
        /// <param name="__wRecord">The w record.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="__useExisting">if set to <c>true</c> [use existing].</param>
        /// <param name="__saveToCache">if set to <c>true</c> [save to cache].</param>
        /// <param name="evaluator">The evaluator.</param>
        /// <returns></returns>
        public weightTableCompiled GetOrCreateTFIDF_DLC(modelSpiderSiteRecord __wRecord, builderForLog loger, bool __useExisting, bool __saveToCache, multiLanguageEvaluator evaluator=null) {

            indexDomain idomain = imbWEMManager.index.domainIndexTable.GetOrCreate(__wRecord.domain);

            FileInfo TFIDF_DLC_File = GetTFIDF_DLC_File(idomain, getWritableFileMode.existing);
            weightTableCompiled TFIDF_DLC = null;

            if (TFIDF_DLC_File.Exists && __useExisting)
            {
                TFIDF_DLC = new weightTableCompiled(TFIDF_DLC_File.FullName, true, idomain.domain + "_DLC_TF_IDF");

                loger.log("DLC TF-IDF[" + TFIDF_DLC.Count + "] cache found for: " + idomain.domain);
                return TFIDF_DLC;    
            }

            if (evaluator == null) evaluator = __wRecord.tRecord.evaluator;

            // <--------------- evaluator selection

            if (imbWEMManager.settings.TFIDF.doUseHeuristicDLCTFIDFConstruction)
            {
                TFIDF_DLC = GetOrCreateTFIDF_DLC_Heuristic(__wRecord, loger, __useExisting, __saveToCache, evaluator);
            }
            else
            {
                
                loger.log("DLC TF-IDF construction for: " + idomain.domain + " initiated.");

                termDocumentSet domainSet = new termDocumentSet("DomainTFIDF_source");

                var tLoaded = __wRecord.context.targets.GetLoaded();
                int tc = tLoaded.Count;
                int ti = 0;
                int ts = 10;
                int c = 0;

                int input_c = 0;
                int output_c = 0;
                double io_r = 0;

                foreach (spiderTarget target in tLoaded)
                {
                    ti++;
                    c++;
                    double tp = ti.GetRatio(tc);

                    if (target.IsRelevant)
                    {

                        var wordlist = GetTermsForPage(target, idomain, null, evaluator, loger);
                        input_c += wordlist.Count;

                        termDocument pageTF = domainSet.AddTable(target.pageHash) as termDocument;
                        pageTF.expansion = 1;
                        pageTF.AddTokens(wordlist, loger);

                        output_c += pageTF.Count();
                    }

                    if (c > 10)
                    {
                        c = 0;
                        io_r = output_c.GetRatio(input_c);
                        aceLog.consoleControl.writeToConsole("Pages processed [" + tp.ToString("P2") + "] Semantic compression rate: " + io_r.ToString("P2"), loger, false, 0);
                    }
                }

                loger.log("[" + idomain.domain + "] preprocess finished. DLC TF-IDF terms [" + domainSet.CountAllDocuments() + "]");

                TFIDF_DLC = domainSet.AggregateDocument.GetCompiledTable(loger);
                TFIDF_DLC.name = "DLC-TFIDF " + idomain.domain;

            
            }

            idomain.Lemmas = TFIDF_DLC.Count;

            if (__saveToCache)
            {
                if (TFIDF_DLC.SaveAs(TFIDF_DLC_File.FullName, getWritableFileMode.overwrite))
                {
                    loger.log("[" + idomain.domain + "] DLC TF-IDF compiled table cache saved to: " + TFIDF_DLC_File.FullName);
                } else
                {
                    loger.log("[" + idomain.domain + "] DLC TF-IDF compiled table save failed");
                }
            }

            imbWEMManager.index.domainIndexTable.AddOrUpdate(idomain);

            return TFIDF_DLC;
        }

                   

        /// <summary>
        /// Folder for TF drops
        /// </summary>
        /// <value>
        /// The tf folder.
        /// </value>
        [XmlIgnore]
        public folderNode TFIDF_ConstructFolder { get; set; }
        
        [XmlIgnore]
        public folderNode sessionReportFolder { get; set; }

        /// <summary>
        /// Folder in which is the crawler report
        /// </summary>
        /// <value>
        /// The session crawler folder.
        /// </value>
        [XmlIgnore]
        public folderNode sessionCrawlerFolder { get; set; }

       // [XmlIgnore]
       // public folderNode crawlRecordFolder { get; set; }

        [XmlIgnore]
        public folderNode indexSubFolder { get; set; }


        [XmlIgnore]
        //public webSitePageTFSet globalTFIDFSet { get; set; }

        //[XmlIgnore]
        public weightTableCompiled globalTFIDFCompiled { get; set; }

        //[XmlIgnore]
        //public multiLanguageEvaluator termevaluator { get; set; }


        public const string PATH_CompiledFTIDF = "tf_idf.xml";




        private object updateIndexLock = new object();

        private object updateIndexLockB = new object();

        private object updateIndexLockC = new object();


        private object updateIndexLockD = new object();


        /// <summary>
        /// Performs full domain reevaluation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="loger">The loger.</param>
        /// <param name="__wRecord">The w record.</param>
        /// <param name="evaluator">The evaluator.</param>
        public void doDomainEvaluation(IndexEngineConfiguration settings, builderForLog loger, modelSpiderSiteRecord __wRecord, multiLanguageEvaluator evaluator, weightTableCompiled mTFIDF)
        {
            indexDomain idomain = null;

            //lock (updateIndexLockD)
            //{
                idomain = imbWEMManager.index.domainIndexTable.GetDomain(__wRecord.domainInfo.domainName);
           // }

            idomain.url = __wRecord.domain;

            //if (mTFIDF == null) mTFIDF = GetTFIDF_Master(loger, true, true);

            double dIP = 0;
            int p = 0;
            List<string> dTerms = new List<string>();

            List<string> dDistinctTerms = new List<string>();

            List<string> dLemmas = new List<string>();
            List<string> dWords = new List<string>();


            List<string> urls = new List<string>();

            bool doEvalD = true;



            foreach (spiderTarget target in __wRecord.context.targets.GetLoaded())
            {
                indexPage ipage = null;
                
               // lock (updateIndexLock)
               // {
                    ipage = imbWEMManager.index.deployTarget(target, __wRecord, idomain);
               // }
                bool doEval = true;
                int dLc = 0;

                if (settings.plugIn_indexDBUpdater_optimizedMode)
                {
                    if ((ipage.InfoPrize > 0) && (ipage.Lemmas > 0) && (ipage.relevancyText == nameof(indexPageRelevancyEnum.isRelevant)))
                    {
                        doEval = false;

                        if (ipage.AllWords.isNullOrEmpty()) doEval = true;
                        if (ipage.AllLemmas.isNullOrEmpty()) doEval = true;
                    }
                }

                if (doEval)
                {
                    List<string> terms = new List<string>();

                    if (ipage.AllWords.isNullOrEmpty())
                    {
                        terms = GetTermsForPage(target, idomain, ipage, evaluator, loger);
                    } else
                    {
                        terms = ipage.AllWords.SplitSmart(",", "", true);
                    }


                    ipage.AllWords = terms.toCsvInLine();


                    double IP = 0;

                    List<string> lemmas = new List<string>();

                    List<IWeightTableTerm> mchl = mTFIDF.GetMatches(terms);

                    if (ipage.AllLemmas.isNullOrEmpty())
                    {
                        

                        //  terms = GetTermsForPage(target, idomain, ipage, evaluator, loger);

                        lemmas.AddRange(mchl.Select(x => x.nominalForm));
                    }
                    else
                    {

                        lemmas = ipage.AllLemmas.SplitSmart(",", "", true);
                    }


                    



                    foreach (weightTableTermCompiled cterm in mchl)
                    {
                        IP += cterm.tf_idf;
                        //dTerms.AddUnique(cterm.nominalForm);
                        
                        if (cterm.df == 1)
                        {
                            dDistinctTerms.AddUnique(cterm.nominalForm);
                        }
                    }

                    ipage.InfoPrize = IP;

                    dIP += IP;

                    ipage.Lemmas = lemmas.Count;
                    
                    ipage.AllLemmas = lemmas.toCsvInLine();

                    dWords.AddRange(terms);
                    dLemmas.AddRange(lemmas);

                    ipage.Note = "indexUpdate" + SessionID;

                 //   lock (updateIndexLockB)
                //    {
                        imbWEMManager.index.pageIndexTable.AddOrUpdate(ipage);
                 //   }
                  // if (loger!=null) loger.AppendLine(String.Format("[{0,25}] [{1,70}] IP[{2,7}] LM[{3,6}]", idomain.domain, ipage.url.TrimToMaxLength(60), ipage.InfoPrize.ToString("F4"), ipage.Lemmas.ToString("D5")));
                }
                else
                {
                    dIP += ipage.InfoPrize;
                    doEvalD = false;
                    // if (loger != null) loger.AppendLine(String.Format("[{0,25}] [{1,70}] IP[{2,7}] LM[{3,6}]", "  ^---- using existing ", ipage.url.TrimToMaxLength(60), ipage.InfoPrize.ToString("F4"), ipage.Lemmas.ToString("D5")));
                }

                urls.Add(ipage.url);

                p++;
                loger.AppendLine(string.Format("[{0,25}] [{1,70}] IP[{2,7}] LM[{3,6}]", idomain.domain, ipage.url.toWidthMaximum(60), ipage.InfoPrize.ToString("F4"), ipage.Lemmas.ToString("D5")));
                target.Dispose();
            }


            if (imbWEMManager.settings.indexEngine.plugIn_indexDBUpdater_updateDomainEntry)
            {

                if (!doEvalD)
                {
                    var dlc_tf = imbWEMManager.index.experimentEntry.GetTFIDF_DLC(idomain);
                    int dlc_c = dlc_tf.Count;


                    idomain.TFIDFcompiled = (dlc_c > 0);
                    idomain.Lemmas = dlc_c;
                }
                else
                {
                    idomain.Lemmas = dLemmas.Count;
                    idomain.Words = dWords.Count;
                    idomain.TFIDFcompiled = (dLemmas.Count > 0);
                    idomain.DistinctLemmas = dDistinctTerms.toCsvInLine();
                    idomain.AllLemmas = dLemmas.toCsvInLine();
                    idomain.AllWords = dWords.toCsvInLine();
                }
                idomain.InfoPrize = dIP;
                //if (doEvalD) 
                

                var urlAssert = imbWEMManager.index.pageIndexTable.GetUrlAssertion(urls);

                idomain.relevantPages = urlAssert[indexPageEvaluationEntryState.isRelevant].Count;
                idomain.notRelevantPages = urlAssert[indexPageEvaluationEntryState.notRelevant].Count;
                idomain.detected = urlAssert[indexPageEvaluationEntryState.haveNoEvaluationEntry].Count;
                idomain.Crawled = urlAssert.certainty;
                idomain.RelevantContentRatio = urlAssert.relevant;
                string rpp = string.Format("[{0,25}] Pages [{1,10}] IP[{2,10}] LM[{3,10}]", idomain.domain, p, idomain.InfoPrize.ToString("F5"), idomain.Lemmas.ToString("D7"));
                if (loger != null) loger.AppendLine(rpp);
            }
            

            

            if (imbWEMManager.settings.indexEngine.plugIn_indexDBUpdater_updateDomainEntry)
            {
                imbWEMManager.index.domainIndexTable.AddOrUpdate(idomain);
            }
                imbWEMManager.index.wRecordsDeployed++;
           
            __wRecord.Dispose();

            
        }







        //[Category("Session")]
        //[DisplayName("TestID")]
        //[imb(imbAttributeName.measure_letter, "ID")]
        //[imb(imbAttributeName.measure_setUnit, "-")]
        //[Description("UID of the particular test")] // [imb(imbAttributeName.reporting_escapeoff)]
        //public String TestID { get; set; } = default(String);

        /// <summary>  </summary>
        [Category("Session")]
        [DisplayName("SessionID")]
        [imb(imbAttributeName.measure_letter, "SID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SessionID { get; set; } = default(string);

        /// <summary> Start of the session </summary>
        [Category("Session")]
        [DisplayName("Start")]
        [imb(imbAttributeName.measure_letter, "S")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [imb(imbAttributeName.reporting_valueformat, "dd.mm.yyyy")]
        [Description("Start of the session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public DateTime Start { get; set; } = default(DateTime);



        /// <summary>  </summary>
        [Category("Session")]
        [DisplayName("CrawlID")]
        [imb(imbAttributeName.measure_letter, "CID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the crawl session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string CrawlID { get; set; } = default(string);





        /// <summary>  </summary>
        [Category("Session")]
        [DisplayName("Crawl Report")]
        [imb(imbAttributeName.measure_letter, "Crawl report path")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("File path to the crawl report")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string ReportPath { get; set; } = default(string);

        [XmlIgnore]
        public analyticConsoleState state { get; internal set; }
    }
}
