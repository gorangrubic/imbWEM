﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticConsole.cs" company="imbVeles" >
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
namespace imbWEM.Core.console
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbMiningContext;
    using imbNLP.Data;
using imbNLP.Data.extended.domain;
using imbNLP.Data.extended.unitex;
using imbNLP.Data.semanticLexicon;
using imbNLP.Data.semanticLexicon.core;
using imbNLP.Data.semanticLexicon.explore;
using imbNLP.Data.semanticLexicon.morphology;
using imbNLP.Data.semanticLexicon.procedures;
using imbNLP.Data.semanticLexicon.source;
using imbNLP.Data.semanticLexicon.term;
    using imbSCI.Core.attributes;
    using imbSCI.Core.collection;
    using imbSCI.Core.data;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.fields;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tests;
    using imbSCI.Reporting.meta.delivery;
    using imbSCI.Reporting.meta.delivery.units;
    using imbSCI.Reporting.meta.documentSet;
    using imbWEM.Core.console.plugins;
    using imbWEM.Core.crawler;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.evaluators.modular;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.index.experimentSession;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;
    using imbWEM.Core.stage.macro;
    using imbWEM.Core.plugins;
    //using imbNLP.PartOfSpeech.pipeline.plugin;
    using imbNLP.Data.semanticLexicon;
    using imbWEM.Core.consolePlugin;

    /// <summary>
    /// Analytic Console 
    /// </summary>
    /// <seealso cref="aceCommonServices.terminal.console.aceAdvancedConsole{imbWEMManager.console.analyticConsoleState, imbWEMManager.console.analyticConsoleWorkspace}" />
    public class analyticConsole : aceAdvancedConsole<analyticConsoleState, analyticConsoleWorkspace>
    {

        //private pipelineModelConsolePlugin _mcPipelinePlugin = null;
        ///// <summary>
        ///// Part-of-speech resolver plugin
        ///// </summary>
        ///// <value>
        ///// Instance of POS resolver
        ///// </value>
        //public pipelineModelConsolePlugin mcPipelinePlugin
        //{
        //    get
        //    {
        //        if (_mcPipelinePlugin == null) _mcPipelinePlugin = new pipelineModelConsolePlugin(this);
        //        return _mcPipelinePlugin;
        //    }
        //}



        public static analyticConsole mainAnalyticConsole { get; set; } = new analyticConsole();


        private crawlJobPlugin _wem = null;
        /// <summary>
        /// Gets the crawlJob plugin
        /// </summary>
        /// <value>
        /// The wem.
        /// </value>
        public crawlJobPlugin wem
        {
            get
            {
                if (_wem == null) _wem = new crawlJobPlugin(this);
                return _wem;
            }
        }


        /// <summary>
        /// </summary>
        public override string consoleTitle
        {
            get
            {
                return "Analytic Console";
            }

            //protected set
            //{
            //    base.consoleTitle = value;
            //}
        }

        public analyticConsole():base()
        {
            
        }
        /// <summary>
        /// Gets the encode.
        /// </summary>
        /// <value>
        /// The encode.
        /// </value>
        public override aceCommandConsoleIOEncode encode
        {
            get
            {
                return aceCommandConsoleIOEncode.dos;
            }
        }

        /// <summary>
        /// Bindable property
        /// </summary>
        public override analyticConsoleWorkspace workspace
        {
            get
            {
                if (_workspace == null)
                {
                    _workspace = new analyticConsoleWorkspace(this);
                }
                return _workspace;
            }
        }



        private plugIn_dataLoader _analytics;
        /// <summary>
        /// Plugin for post-processing of exported DataTable files
        /// </summary>
        public plugIn_dataLoader analytics
        {
            get {

                if (_analytics == null) _analytics = new plugIn_dataLoader(this, nameof(analytics));
                return _analytics;
            }
            set { _analytics = value; }
        }



        private imbMCManager _MCManager;
        /// <summary> </summary>
        public imbMCManager MCManager
        {
            get
            {
                if (_MCManager == null) _MCManager = new imbMCManager(this);
                return _MCManager;
            }
            set
            {
                _MCManager = value;
                OnPropertyChanged("MCManager");
            }
        }



        


        [aceMenuItem(aceMenuItemAttributeRole.Key, "preloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "preloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "PreloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Performs the Lexicon preload to increase performances of the semantic components")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Loads all Lemma and Instances from triplestore into memory and indexed dictionaries")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        /// <summary>
        /// Method of menu option PreloadLexicon (key:preloadLexicon). <args> expects param: word:String;steps:Int32;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  word:String;steps:Int32;debug:Boolean;</param>
        /// <remarks>
        /// <para>Loads all Lemma and Instances from triplestore into memory and indexed dictionaries</para>
        /// <para>Performs the Lexicon preload to increase performances of the semantic components</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runPreloadLexicon(aceOperationArgs args)
        {
            DateTime start = DateTime.Now; output.getLastLine();

            semanticLexiconManager.lexiconCache.preloadLexicon(response, semanticLexiconManager.manager.lexiconContext);


            output.AppendLine("PreloadLexicon done in: " + DateTime.Now.Subtract(start).TotalMinutes.ToString("#0.0##") + " min");
            output.getLastLine().saveStringToFile("preloadLexicon_record.txt");
        }




        //[aceMenuItem(aceMenuItemAttributeRole.Key, "OpenSession")]
        //[aceMenuItem(aceMenuItemAttributeRole.aliasNames, "OpenSession")]
        //[aceMenuItem(aceMenuItemAttributeRole.DisplayName, "OpenSession")]
        //[aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        //[aceMenuItem(aceMenuItemAttributeRole.Description, "Selects and preloads local index and Experiment session information. useJobSettings option will ignore other params and use Job definition")]
        //[aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set report output information and create or load local index")]
        ////[aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        //// [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        //// [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runOpenSession.md")]
        ////  [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "experimentSession=\"General\":String;IndexID=\"*\":String;useJobSettings=true:Boolean;")]

        ///// <summary>
        ///// Aces the operation run open session.
        ///// </summary>
        ///// <param name="experimentSession">The experiment session.</param>
        ///// <param name="IndexID">The index identifier.</param>
        ///// <param name="useJobSettings">if set to <c>true</c> [use job settings].</param>
        ///// <param name="crawlFolderNameTemplate">The crawl folder name template.</param>
        ///// <exception cref="aceGeneralException">
        ///// You must define Job before calling OpenSession! Check Job command in console help. - null - null - Just have Job defined!
        ///// or
        ///// You must call CJEngineSetup before OpenSession call - null - CrawlJobEngine setup required
        ///// </exception>
        //public void aceOperation_runOpenSession(string experimentSession, string IndexID, bool useJobSettings,
        //    string crawlFolderNameTemplate = "*"
        //    )
        //{

        //    if (state.job == null) throw new aceGeneralException("You must define Job before calling OpenSession! Check Job command in console help.", null, null, "Just have Job defined!");
        //    if (state.crawlerJobEngineSettings == null) throw new aceGeneralException("You must call CJEngineSetup before OpenSession call", null, this, "CrawlJobEngine setup required");

        //    response.log("OpenSession with experimentSession=" + experimentSession + ", IndexID =" + IndexID + ", useJobSettings=" + useJobSettings + ".");

        //    if (useJobSettings)
        //    {
        //        string sessionName = state.makeTheRunStamp();
        //        if (imbWEMManager.settings.directReportEngine.REPORT_DirectoryNameByJobName)
        //        {
        //            sessionName = state.job.name.getCleanPropertyName();
        //        }
                
        //        experimentSession = workspace.makeFolderName(sessionName, true);
                

        //        response.log("Job runstamp generated: " + state.job.runstamp);

        //    }
        //   imbWEMManager.index.OpenIndex(IndexID, experimentSession);

        //    var spiderEvals = state.aRecord.GetChildRecords();
        //    var frst = spiderEvals.FirstOrDefault();

        //    string crawlReportFolderName = "";

        //    if (crawlFolderNameTemplate == "*")
        //    {
        //        crawlReportFolderName = frst.instance.name.getCleanFilepath().Replace("-", "");
        //    } else
        //    {
        //        crawlReportFolderName = nameComposer.GetCrawlFolderName(state.crawler, state, crawlFolderNameTemplate);
        //    }


            

        //    state.indexSession = imbWEMManager.index.StartSession(crawlReportFolderName, state);
            
        //}





        //[Display(GroupName = "define", Name = "CJEngineSetup", ShortName = "CJES", Description = @"Crawl Job Engine controls the parallel execution of the Crawl Job. 
        //    Tdl_max defines max. minutes per one domain level crawl, Tll_max per single link load and TC_max defines number of parallel domain loads.")]
        //[aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "This command sets the most important parameters of the Crawl Job execution. For Tdl_max and Tll_max value -1 means limit is off, for TC_max value -1 means auto management.")]
        ///// <summary>Crawl Job Engine controls the parallel execution of the Crawl Job.</summary> 
        ///// <remarks><para>This command sets the most important parameters of the Crawl Job execution</para></remarks>
        ///// <param name="TC_max">Maximum number of parallel DLC executing in the same moment</param>
        ///// <param name="Tdl_max">Maximum minutes allowed for single DLC to run</param>
        ///// <param name="Tll_max">Maximum minutes of single iteration allowed for a DLC before its termination</param>
        ///// <param name="Tcjl_max">Maximum minutes for the complete Crawl Job execution</param>
        ///// <seealso cref="aceOperationSetExecutorBase"/>
        //public void aceOperation_defineCrawlJobEngineSettings(
        //    [Description("Maximum number of parallel DLC executing in the same moment")] int TC_max = 8,
        //    [Description("Maximum minutes allowed for single DLC to run")] int Tdl_max = 50,
        //    [Description("Maximum minutes of single iteration allowed for a DLC before its termination")] int Tll_max = 20,
        //    [Description("Maximum minutes for the complete Crawl Job execution")] int Tcjl_max = 100)
        //{
        //    state.crawlerJobEngineSettings = new crawlerDomainTaskMachineSettings();
        //    state.crawlerJobEngineSettings.TC_max = TC_max;
        //    state.crawlerJobEngineSettings.Tdl_max = Tdl_max;
        //    state.crawlerJobEngineSettings.Tll_max = Tll_max;
        //    state.crawlerJobEngineSettings.Tcjl_max = Tcjl_max;
        //}





        //[aceMenuItem(aceMenuItemAttributeRole.Key, "R")]
        //[aceMenuItem(aceMenuItemAttributeRole.aliasNames, "run")]
        //[aceMenuItem(aceMenuItemAttributeRole.DisplayName, "RunJob")]
        //[aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        //[aceMenuItem(aceMenuItemAttributeRole.Description, "Runs the current analytic job using current settings")]
        //[aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will call initialization of analytic job record and perform parralel domain load.")]
        ///// <summary>
        ///// Method of menu option RunJob (key:R). <args> expects param: Tdl_max:Int32;Tll_max:Int32;TC_max:Int32;
        ///// </summary>        
        ///// <remarks>
        ///// <para>It will call initialization of analytic job record and perform parralel domain load. For Tdl_max and Tll_max value -1 means limit is off, for TC_max value -1 means auto management</para>
        ///// <para>Runs the current analytic job where Tdl_max defines max. minutes per one domain level crawl, Tll_max per single link load and TC_max defines number of parallel domain loads.</para>
        ///// <para>Message if item disabled: (disabled)</para>
        ///// </remarks>
        ///// <seealso cref="aceOperationSetExecutorBase"/>
        //public void aceOperation_runRunJob()
        //{
        //    DateTime start = DateTime.Now;
        //    if (state.aRecord == null)
        //    {
        //        output.log("Error: define Job before calling this command.");
        //        return;
        //    }

        //    int Tdl_max = state.crawlerJobEngineSettings.Tdl_max;
        //    int Tll_max = state.crawlerJobEngineSettings.Tll_max;
        //    int TC_max = state.crawlerJobEngineSettings.TC_max;


        //    // imbWEMManager.settings.crawlAdHok.Diversity_DefaultExpansionSteps = TExp;

        //    response.log("RunJob with Tdl_max=" + Tdl_max + ", Tll_max =" + Tll_max + ", TC_max=" + TC_max + ".");

        //    var spiderEvals = state.aRecord.GetChildRecords();
        

        //    response.log("Job Record soft initialization call");
        //    state.aRecord.initializeSoft(state.sampleList);

         

        //    int c = 0;

            

        //    DirectoryInfo di = imbWEMManager.index.experimentManager.CurrentSession.sessionReportFolder;
            

        //    var notation = new aceAuthorNotation();

        //    // ------------------ note creation -------------------
        //    analyticJobNote note = new analyticJobNote(imbWEMManager.index.experimentEntry.sessionCrawlerFolder);

        //    note.WriteAboutJob(state, workspace, this);

        //    note.AppendLine("--------------- Crawl Job configuration overview -------------------------- ");
        //    note.AppendLine("   Script var | Article    - Description                             ");
        //    note.AppendLine("--------------------------------------------------------------------------- ");
        //    note.AppendLine("   Tdl_max    | Tdl        - Time limit per domain - in minutes     | : " + Tdl_max);
        //    note.AppendLine("   Tll_max    | Tac        - Time limit for inactivity - in minutes | : " + Tll_max);
        //    note.AppendLine("   TC_max     | TC         - Maximum number of JLC threads allowed  | : " + TC_max);
        //    note.AppendLine("--------------------------------------------------------------------------- ");
        //    note.AppendHorizontalLine();
        //    note.AppendLine("-- if the test was finished without problem at the last line it will be message [RunJob completed] ---");
        //    note.AppendLine("-- if not: something went wrong - check the logs ---");
        //    note.AppendHorizontalLine();
        //    note.SaveNote();

        //    foreach (modelSpiderTestRecord tRecord in spiderEvals)
        //    {
        //        c++;
        //        spiderWebLoaderControler controler = null;

        //        directAnalyticReporter reporter = new directAnalyticReporter(imbWEMManager.index.experimentEntry.CrawlID, imbWEMManager.index.experimentEntry.sessionCrawlerFolder, workspace, notation);



        //        state.pluginStack.InstallTo(imbWEMManager.index.plugins, plugInGroupEnum.index, true);

        //        tRecord.performance = imbWEMManager.index.experimentEntry;

        //        response.log(tRecord.instance.name + " crawl start");


        //        crawlerDomainTaskMachine cDTM = new crawlerDomainTaskMachine(tRecord, state.aRecord.sample, reporter, di)
        //        {


        //            maxThreads = TC_max,
        //            _timeLimitForDLC = Tdl_max,
        //            TimeLimitForTask = Tll_max
        //        };

        //        //state.pluginStack
        //        state.pluginStack.InstallTo(cDTM.plugins, plugInGroupEnum.engine, false);
        //        state.pluginStack.InstallTo(tRecord.instance.plugins, plugInGroupEnum.crawler, false);
        //        state.pluginStack.InstallTo(cDTM.reportPlugins, plugInGroupEnum.report, false);

        //        cDTM.startAutoParallel(true); // ----- execution

        //        response.log(tRecord.instance.name + " crawl finished");

        //        cDTM.webLoaderControler.Save();

        //        controler = cDTM.webLoaderControler;
        //        state.failedDomains.AddRange(controler.GetFailedDomains());

        //        reporter.reportCrawler(tRecord);


        //        note.WriteAboutCrawlerRun(tRecord, cDTM, state);

        //        scriptRunning.getContent().saveStringToFile(imbWEMManager.index.experimentEntry.sessionCrawlerFolder.pathFor("script.ace"));


        //        if (imbWEMManager.settings.directReportEngine.doPublishExperimentSessionTable)
        //        {
        //            imbWEMManager.index.experimentManager.AddOrUpdate(tRecord.performance as experimentSessionEntry);
        //        }
        //    }




        //    imbWEMManager.index.CloseSession(spiderEvals);



        //    output.AppendLine("RunJob done in: " + DateTime.Now.Subtract(start).TotalMinutes.ToString("#0.0##") + " min");
        //    workspace.saveLog(response, "run_job");

        //    note.AppendLine("[RunJob completed]");
        //    note.SaveNote();


            
        //    imbWEMManager.settings.Save(imbWEMManager.index.experimentEntry.sessionCrawlerFolder.pathFor("imbAnalyticEngineSettings.xml"));
        //    var sl = state.sampleList.ToList();
        //    sl.saveContentOnFilePath(note.folder.pathFor("sample.txt"));
            
            
           
        //}








       


        [aceMenuItem(aceMenuItemAttributeRole.Key, "crawlerComment")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "crawlerComment")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "CrawlerComment")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "config")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Use this command to add text about specific customizations made to the crawler instance")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will add comment line into crawler settings and set signature sufix that is used for experiment session crawler signature")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_configCrawlerComment.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "comment=\"word\":String;signatureSufix=5:String;appendSufix=false:Boolean;")]
        /// <summary>
        /// Method of menu option CrawlerComment (key:crawlerComment). <args> expects param: comment:String;signatureSufix:String;appendSufix:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  comment:String;signatureSufix:String;appendSufix:Boolean;</param>
        /// <remarks>
        /// <para>It will add comment line into crawler settings and set signature sufix that is used for experiment session crawler signature</para>
        /// <para>Use this command to add text about specific customizations made to the crawler instance</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_configCrawlerComment(aceOperationArgs args)
        {
            
            string comment = args.Get<string>("comment");
            string signatureSufix = args.Get<string>("signatureSufix");
            bool appendSufix = args.Get<bool>("appendSufix");

            response.log("CrawlerComment with comment=" + comment + ", signatureSufix =" + signatureSufix + ", appendSufix=" + appendSufix + ".");

            state.crawler.settings.Comment = state.crawler.settings.Comment.addLine(comment);
            if (appendSufix) {
                state.crawler.settings.SignatureSufix = state.crawler.settings.SignatureSufix.add(signatureSufix, "|");
            } else
            {
                state.crawler.settings.SignatureSufix = signatureSufix;
            }
        }



        [aceMenuItem(aceMenuItemAttributeRole.Key, "crawlerSet")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "crawlerSet")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "CrawlerSet")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "config")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Use this command to change configuration of the current crawler")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will pharse the parameters and change crawler configuration from default settings to specified ones")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_configCrawlerSet.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "par=\"nameOfTheParameter\":String;newValue=1.0:Double;appendComment=true:Boolean;")]

        /// <summary>
        /// Method of menu option CrawlerSet (key:crawlerSet). <args> expects param: par:String;newValue:Double;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  par:String;newValue:Double;debug:Boolean;</param>
        /// <remarks>
        /// <para>It will pharse the parameters and change crawler configuration from default settings to specified ones</para>
        /// <para>Use this command to change configuration of the current crawler</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_configCrawlerSet(aceOperationArgs args)
        {
            
            string par = args.Get<string>("par");
            double newValue = args.Get<double>("newValue");
            bool appendComment = args.Get<bool>("appendComment");

            bool changed = false;

            if (state.crawler == null) response.log("No current crawler specified");

            var pi = state.crawler.getProperty(par);

            if (pi == null)
            {
                response.log("ERROR: Parameter [" + par + "] not found at crawler [" + state.crawler.name + "]");
            } else
            {
                double oldValue = state.crawler.imbGetPropertySafe(par, 0).imbConvertValueSafeTyped<double>();
                state.crawler.imbSetPropertyConvertSafe(pi, newValue);

                double curValue = state.crawler.imbGetPropertySafe<double>(pi);

                changed = (newValue != curValue);

                if (changed)
                {
                    response.log("Parameter [" + par + $"] not changed [old:{oldValue}] [in:{newValue}] [final:{curValue}] {state.crawler.name}");
                } else
                {
                    response.log($"Parameter [{par}] changed from {oldValue} to {newValue} at crawler [{state.crawler.name}]");
                    state.crawler.settings.Comment = state.crawler.settings.Comment.addLine($"[{par}] set to: {newValue}  (def: {oldValue})");
                    state.crawler.settings.SignatureSufix = state.crawler.settings.SignatureSufix.add($"{par}{newValue}", "|");
                }

            }

            
            
        }




        [aceMenuItem(aceMenuItemAttributeRole.Key, "SIP")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "sip")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "sampleByIndexPage")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "define")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Selects the sample set from the index database accorting to specified indexPageEvaluationEntryState")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will select all pages that reflect the specified criterion and put their domains into current sample list")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "evaluationState=\"indexPageEvaluationEntryState.haveNoEvaluationEntry\":indexPageEvaluationEntryState;limit=100:Int32;skip=0:Int32;")]
        /// <summary>
        /// Method of menu option sampleByIndexPage (key:SIP). <args> expects param: evaluationState:indexPageEvaluationEntryState;limit:Int32;skip:Int32;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  evaluationState:indexPageEvaluationEntryState;limit:Int32;skip:Int32;</param>
        /// <remarks>
        /// <para>It will select all pages that reflect the specified criterion and put their domains into current sample list</para>
        /// <para>Selects the sample set from the index database accorting to specified indexPageEvaluationEntryState</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_definesampleByIndexPage(aceOperationArgs args)
        {
            var assert = imbWEMManager.index.domainIndexTable.GetDomainIndexAssertion(null, true);
            List<string> l = assert[indexDomainContentEnum.indexed];

            
            state.sampleList = new webSiteSimpleSample();
            state.sampleList.Add(imbWEMManager.index.domainIndexTable.GetDomains(l));

            

            //workspace.sampleAcceptAndPrepare(null, true, null, args.Get<Int32>("limit"), args.Get<Int32>("skip"), indexDomainContentEnum.none, args.Get<indexPageEvaluationEntryState>("evaluationState"));
        }



        [aceMenuItem(aceMenuItemAttributeRole.Key, "SI")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "indexSample")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "IndexSample")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "define")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Sets sample from index database according to domain content type")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will query domain index table for domains with specified contentType and add it to the current sample list")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "type=\"indexDomainContentEnum.bothRelevantAndNonRelevant\":indexDomainContentEnum;samplename=\"sample_domains\":String;fileHasPriority=true:Boolean;limit=300:Int32;skip=0:Int32;")]
        
        /// <summary>
        /// Method of menu option IndexSample (key:SI). <args> expects param: type:indexDomainContentEnum;samplename:String;limit:Int32;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  type:indexDomainContentEnum;samplename:String;limit:Int32;</param>
        /// <remarks>
        /// <para>It will query domain index table for domains with specified contentType and add it to the current sample list</para>
        /// <para>Sets sample from index database according to domain content type</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_defineIndexSample(aceOperationArgs args)
        {
            workspace.sampleAcceptAndPrepare(args.Get<string>("samplename"), args.Get<bool>("fileHasPriority"), args.Get<string>("group_tags"), args.Get<int>("limit"), args.Get<int>("skip"), args.Get<indexDomainContentEnum>("type"), indexPageEvaluationEntryState.none);
        }





        [aceMenuItem(aceMenuItemAttributeRole.Key, "S")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "sample")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Sample")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "define")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Declares working sample set - domain urls. If fileHasPriority is true it will load local file list.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Load, saves, queries - list of domains")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "group_tags=\"primary\":String;samplename=\"sample_domains\":String;fileHasPriority=true:Boolean;limit=300:Int32;skip=0:Int32;")]
        /// <summary>
        /// Method of menu option Sample (key:S). <args> expects param: group_tags:String;filepath:String;fileHasPriority:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  group_tags:String;filepath:String;fileHasPriority:Boolean;</param>
        /// <remarks>
        /// <para>Load, saves, queries - list of domains</para>
        /// <para>Declares working sample set - domain urls. If fileHasPriority is true it will load local file list.</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_defineSample(aceOperationArgs args)
        {
            workspace.sampleAcceptAndPrepare(args.Get<string>("samplename"), args.Get<bool>("fileHasPriority"), args.Get<string>("group_tags"), args.Get<int>("limit"), args.Get<int>("skip"), indexDomainContentEnum.none, indexPageEvaluationEntryState.none);
        }


        [aceMenuItem(aceMenuItemAttributeRole.Key, "importSample")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "importSample")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "ImportSample")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "define")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Importing domain sample list from txt file")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It loads specified txt file found in the console project and loads domains specified there into state sample list")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "filename=\"domains_focus.txt\":String;samplename=\"focus_domains\":String;fileHasPriority=true:Boolean;limit=300:Int32;skip=0:Int32;")]
        /// <summary>
        /// Method of menu option ImportSample (key:importSample). <args> expects param: word:String;steps:Int32;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  word:String;steps:Int32;debug:Boolean;</param>
        /// <remarks>
        /// <para>It loads specified txt file found in the console project and loads domains specified there into state sample list</para>
        /// <para>Importing domain sample list from txt file</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_defineImportSample(aceOperationArgs args)
        {
            workspace.sampleAcceptAndPrepare(args.Get<string>("filename"), args.Get<bool>("fileHasPriority"), args.Get<string>("group_tags"), args.Get<int>("limit"), args.Get<int>("skip"), indexDomainContentEnum.none, indexPageEvaluationEntryState.none, args.Get<string>("samplename"));
        }



        [aceMenuItem(aceMenuItemAttributeRole.Key, "pageIndex")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "pageIndex")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "pageIndex")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Performs management operations over page index data table -- loads external index data etc")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will propt for secondary command if not specified by parameters")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "operation=\"none\":indexPageTableOperation;sourceFile=dt_pageindex_prev.xlsx:String;")]
        /// <summary>
        /// Method of menu option pageIndex (key:pageIndex). <args> expects param: operation:indexPageTableOperation;sourceFile:String;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  operation:indexPageTableOperation;sourceFile:String;debug:Boolean;</param>
        /// <remarks>
        /// <para>It will propt for secondary command if not specified by parameters</para>
        /// <para>Performs management operations over page index data table</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runpageIndex(aceOperationArgs args)

        {
            indexPageTableOperation operation = args.Get<indexPageTableOperation>("operation");
            
            string sourceFile = args.Get<string>("sourceFile");
            bool ok = false;
            string sufix = "";
            if (state.sampleList.Any())
            {
                sufix = " (Active sample: " + state.sampleList.Count + ")";
            }
            if (operation == indexPageTableOperation.none)
            {
                operation = aceTerminalInput.askForEnum<indexPageTableOperation>("Select indexPage operation to perform", indexPageTableOperation.none);
            }
            if (operation == indexPageTableOperation.loadReviewedTable)
            {
                if (!File.Exists(imbWEMManager.index.folder.pathFor(sourceFile)))
                {
                    var files = imbWEMManager.index.folder.findFiles("*.xlsx|*.csv", SearchOption.TopDirectoryOnly, true);
                    sourceFile = aceTerminalInput.askForOption("Select data table file for operation: " + operation.ToString() + "", files.First(), files, null).toStringSafe();
                }

                ok = File.Exists(imbWEMManager.index.folder.pathFor(sourceFile));
            } else
            {
                 ok = aceTerminalInput.askYesNo("Are you sure to: " + operation.ToString() + "? " + sufix , false);

                
            }

            if (ok)
            {
                imbWEMManager.index.ExecuteIndexPageOperation(operation, sourceFile, state.sampleList, output);
            }
        }




        //[aceMenuItem(aceMenuItemAttributeRole.Key, "J")]
        //[aceMenuItem(aceMenuItemAttributeRole.aliasNames, "job")]
        //[aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Job")]
        //[aceMenuItem(aceMenuItemAttributeRole.Category, "define")]
        //[aceMenuItem(aceMenuItemAttributeRole.Description, "AnaliticJob declares one experimental run, this is the first command to call in scripts with experiment definitions")]
        //[aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Creates new instance of ActivityJog and assigns it to the current state.")]
        //[aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        //// [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        //// [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_definedefineJob.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "jobName=\"test01\":String;jobDesc=\"5\":String;defaultStage=true:Boolean;stampPrefix=\"\":String;testCount=5:Int32;")]

        ///// <summary>
        ///// Method of menu option defineJob (key:J). <args> expects param: jobName:String;jobDesc:String;defaultStage:Boolean;
        ///// </summary>
        ///// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  jobName:String;jobDesc:String;defaultStage:Boolean;</param>
        ///// <remarks>
        ///// <para>Creates new instance of ActivityJog and assigns it to the current state.</para>
        ///// <para>AnaliticJob declares one experimental run, this is the first command to call in scripts with experiment definitions</para>
        ///// <para>Message if item disabled: (disabled)</para>
        ///// </remarks>
        ///// <seealso cref="aceOperationSetExecutorBase"/>
        //public void aceOperation_defineJob(aceOperationArgs args)
        //{
            
        //    string jobName = args.Get<string>("jobName");
        //    string jobDesc = args.Get<string>("jobDesc");
        //    bool defaultStage = args.Get<bool>("defaultStage");
        //    string stampPrefix = args.Get<string>("stampPrefix");
        //    int testCount = args.Get<int>("testCount");

        //    output.log("Job with jobName=" + jobName + ", jobDesc =" + jobDesc + ", defaultStage=" + defaultStage + " was defined");

        //    state.job = new analyticJob();
        //    state.job.console = this;
        //    state.runstampSetup = new testLabelingSettings();
        //    state.job.name = jobName;

        //    state.job.testInfo.caption = jobName;
        //    state.job.testInfo.description = jobDesc;
        //    state.job.testInfo.versionCount = testCount;

        //    analyticProject aProject = state.sciProject;
        //    bool projectCreated = false;
        //    if (aProject == null)
        //    {
        //        aProject = new analyticProject();
                
        //        projectCreated = true;
        //    }

        //    state.sciProject = aProject;
            
        //    state.aRecord = new project.records.analyticJobRecord(state.job, state.sciProject, analyticJobRunFlags.none);
        //    aceLog.consoleControl.setAsOutput(state.aRecord, "aRecord");

        //    if (projectCreated) state.aRecord.logBuilder.log("SciProject new instance (byFlag) ::" + aProject.GetType().Name + " with defaults"); // Analytic macro script [" + this.GetType().Name + "] execution started");


        //    if (defaultStage)
        //    {
        //        state.stageControl = new macroStageControlFullScan(jobName, "Common stage control");
        //    }

        //    imbWEMManager.GCCall("Cleaning memory if any earlier job data left...");
        //}




        [aceMenuItem(aceMenuItemAttributeRole.Key, "setGlobal")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "setGlobal")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "SetGlobal")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "config")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Allows script-side modification of imbAnalyticFramework.settings global variables ")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It changes the property value according to given property path, relative to imbAnalyticFramework.settings instance")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        /// <summary>
        /// Aces the operation configuration set global.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        public void aceOperation_configSetGlobal(string path, string value)
        {
            DateTime start = DateTime.Now; output.getLastLine();

            PropertyExpression pe = imbWEMManager.settings.GetExpressionResolved(path);
            if (pe.state == PropertyExpressionStateEnum.resolvedAll)
            {
                pe.setValue(value);
                //log("Path [" + path + "] [" + pe.state.ToString() + "] [" + value + "] set");
            } else
            {
                log("Path [" + path + "] [" + pe.state.ToString() + "] failed to be resolved in part: [" + pe.undesolvedPart + "]");
            }
            
           
        }










        private semanticLexiconManager _manager;
        /// <summary>
        /// 
        /// </summary>
        public semanticLexiconManager manager
        {
            get
            {
                if (_manager == null) _manager = semanticLexiconManager.manager;
                return _manager;
            }
            set { _manager = value; }
        }


        public override void onStartUp()
        {

            imbLanguageFrameworkManager.Prepare();

            manager.workspaceFolderPath = workspace.folder.path;

            log("Preparing Semantic Lexicon manager", true);

            manager.prepare();
            log("Preparing Semantic Lexicon cache", true);
            manager.prepareCache(output,workspace.folder);
            manager.constructor.startConstruction(workspace.folder[ACFolders.constructor].path.removeEndsWith("\\"));

            imbWEMManager.index.isFullTrustMode = imbWEMManager.settings.indexEngine.doIndexFullTrustMode;

            if (imbWEMManager.commandArgs.Any())
            {
                string line = imbWEMManager.commandArgs.toCsvInLine(" ");
                executeCommand(line);
            } else
            {
                var script = workspace.loadScript("autoexec.ace");
                executeScript(script);
            }


            
            
        }

        protected override void doCustomSpecialCall(aceCommandActiveInput input)
        {
            // <---
        }
    }
}
