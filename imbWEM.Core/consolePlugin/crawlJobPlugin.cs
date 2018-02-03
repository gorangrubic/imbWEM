// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlJobPlugin.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbWEM.Core.consolePlugin
{

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using imbACE.Core;
    using imbACE.Core.application;
    using imbACE.Core.plugins;
    using imbACE.Core.operations;
    using imbACE.Services.application;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.files.search;

    using imbWEM.Core.crawler.evaluators.modular;
    using imbWEM.Core.crawler.evaluators;
    using imbSCI.Core.extensions.typeworks;
    using imbWEM.Core.project;
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.stage;
    using imbWEM.Core.project.records;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler;
    using System.ComponentModel;
    using imbMiningContext;
    using imbACE.Core.core.exceptions;
    using imbSCI.Core.extensions.io;
    using imbWEM.Core.console;
    using imbWEM.Core.index.core;
    using imbACE.Core.core;
    using imbWEM.Core.stage.macro;
    using imbWEM.Core.wemTools;
    using imbACE.Services.terminal.dialogs;
    using imbACE.Network.tools;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.experimentSession;
    using imbNLP.Data;
    using imbSCI.Core.reporting;


    /// <summary>
    /// Plugin for imbACE console - crawlJobPlugin
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.aceConsolePluginBase" />
    public class crawlJobPlugin : aceConsolePluginBase
    {


        public crawlJobContext context { get; set; } = new crawlJobContext();



        protected String CleanName(String classname)
        {
            classname = classname.Replace("_", "");
            classname = classname.Replace("-", "");
            classname = classname.Replace(" ", "");
            classname = classname.ToUpper();
            return classname;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="crawlJobPlugin"/> class.
        /// </summary>
        /// <param name="__parent">The parent.</param>
        public crawlJobPlugin(IAceAdvancedConsole __parent) : base(__parent, "crawlJobPlugin", "This is imbACE advanced console plugin for crawlJobPlugin")
        {
            
            //output = newOutput;

            imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(output, "WEM Plugin");

        }




        [Display(GroupName = "define", Name = "Job", ShortName = "", Description = "AnaliticJob declares one experimental run, this is the first command to call in scripts with experiment definitions")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Creates new instance of ActivityJog and assigns it to the current state.")]
        /// <summary>
        /// AnaliticJob declares one experimental run, this is the first command to call in scripts with experiment definitions
        /// </summary>
        /// <param name="jobName">Name of the Job to define</param>
        /// <param name="jobDesc">Description for the job</param>
        /// <param name="defaultStage">If true it will prepare default crawler stage to execute crawl in</param>
        /// <param name="stampPrefix">Prefix at timestamp</param>
        /// <param name="stampCount">Stamp version count</param>
        /// <remarks>
        /// Creates new instance of ActivityJog and assigns it to the current state.
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_defineJob(
            [Description("Name of the Job to define")] String jobName = "job",
            [Description("Description for the job")] String jobDesc = "",
            [Description("If true it will prepare default crawler stage to execute crawl in")] Boolean defaultStage = true,
            [Description("Prefix at timestamp")] String stampPrefix = "",
            [Description("Stamp version count")] Int32 stampCount = 1)

        {
            context = new crawlJobContext();
            var job = new analyticJob();
            job.name = jobName;
            job.description = jobDesc;
            context.job = job;
            context.aRecord = new analyticJobRecord(job);
                       
            aceLog.consoleControl.setAsOutput(context.aRecord, "aRecord");
            
            if (defaultStage)
            {
                context.stageControl = new macroStageControlFullScan(jobName, "Common stage control");
            }
            
        }



        [Display(GroupName = "add", Name = "Crawler", ShortName = "", Description = "Defines new instance of the specified crawler. LT_t defines link take per iteration, I_max is iteration limit, PL_max defines max. page loads, PS_c is count of selected pages at end.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "New crawler is attached to the AnalyticJobRecord and set as current on the state level")]
        /// <summary>Defines new instance of the specified crawler. LT_t defines link take per iteration, I_max is iteration limit, PL_max defines max. page loads, PS_c is count of selected pages at end.</summary> 
        /// <remarks><para>New crawler is attached to the AnalyticJobRecord and set as current on the state level</para></remarks>
        /// <param name="classname">Name of the crawler class</param>
        /// <param name="LT_t">Load take - number of parallel loads</param>
        /// <param name="I_max">Iteration number limit</param>
        /// <param name="PL_max">Page Loads limit</param>
        /// <param name="instanceNameSufix">Crawler name sufix</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_addCrawler(
            [Description("Name of the crawler class")] String classname = "SM_LTS",
            [Description("Load take - number of parallel loads")] Int32 LT_t = 1,
            [Description("Iteration number limit")] Int32 I_max = 100,
            [Description("Page Loads limit")] Int32 PL_max = 50,
            [Description("Crawler name sufix")] String instanceNameSufix = "",
            [Description("Primary language")] basicLanguageEnum primLanguage = basicLanguageEnum.serbian,
            [Description("Secondary language")] basicLanguageEnum secLanguage = basicLanguageEnum.english
            )
        {
            if (context.aRecord == null)
            {
                output.log("Error: define Job before calling this command.");
                return;
            }
                      

            var evaluator = wemTypesManager.wemTypes.crawlerTypes.GetInstance(classname, output);
            
            if (evaluator != null)
            {
                evaluator.name = (context.aRecord.children.Count + 1).ToString("D2") + evaluator.name;

                evaluator.settings.limitIterations = I_max;
                evaluator.settings.limitTotalPageLoad = PL_max;
                evaluator.settings.limitIterationNewLinks = LT_t;
                //evaluator.settings.primaryPageSetSize = PS_c;


                if (evaluator is ISpiderWithLanguageModule)
                {
                    ISpiderWithLanguageModule evaluator_ISpiderWithLanguageModule = (ISpiderWithLanguageModule)evaluator;
                    evaluator_ISpiderWithLanguageModule.primaryLanguage = primLanguage;
                    evaluator_ISpiderWithLanguageModule.secondaryLanguage = secLanguage;

                }


                evaluator.name = evaluator.name + "_" + instanceNameSufix;
                output.log("Crawler [" + evaluator.name + "] iteration limit set [" + evaluator.settings.limitIterations + "], total page load limit set [" + evaluator.settings.limitTotalPageLoad + "], links take limit set [" + evaluator.settings.limitIterationNewLinks + "]");


                context.crawler = evaluator;
                context.aRecord.spiderList.Add(evaluator);

                var tRecord = context.aRecord.children.Add(evaluator);
                tRecord.parent = context.aRecord;
                tRecord.instanceID = evaluator.name;
                tRecord.testRunStamp = context.aRecord.testRunStamp;
                tRecord.initialize(context.stageControl);

                output.log("Crawler [" + evaluator.name + "] assigned to job [" + context.aRecord.job.name + "] on slot [" + (context.aRecord.spiderList.Count - 1) + "]");
                output.log("Crawler [" + evaluator.name + "] set as the current crawler in the console state");
            } else
            {
                output.log("Crawler class [" + classname + "] not recognized!!!");
            }
        }




        [Display(GroupName = "add", Name = "SampleFile", ShortName = "", Description = "Imports sample from text file")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Loads the file and adds domain urls from it into context's sample list")]
        /// <summary>
        /// Imports sample from text file
        /// </summary>
        /// <param name="path">path to file with samples, if * it will open dialog to select the file</param>
        /// <param name="inWorkspace">if true, the file path is interpreted as relative to console workspace</param>
        /// <param name="sampleName">Name of the sample list, if empty it will not change current sample list name</param>
        /// <param name="replace">if set to true it will replace any existing samples in the list</param>
        /// <param name="debug">if true it will report on link preprocessing</param>
        /// <remarks>
        /// Loads the file and adds domain urls from it into context's sample list
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase" />
        public void aceOperation_addSampleFile(
            [Description("path to file with samples, if * it will open dialog to select the file")] String path = "*",
            [Description("if true, the file path is interpreted as relative to console workspace")] Boolean inWorkspace = true,
            [Description("Name of the sample list, if empty it will not change current sample list name")] String sampleName = "",
            [Description("if set to true it will replace any existing samples in the list")] Boolean replace = false,
            [Description("Number of entries to skip, from the imported file")] Int32 skip = 0,
            [Description("If set above 0, it limits the total number of domains imported")] Int32 limit = -1,
            [Description("if true it will report on link preprocessing")] Boolean debug = true)
        {
            IAceAdvancedConsole console = parent as IAceAdvancedConsole;
            
            if (path == "*")
            {
                String defPath = appManager.Application.folder_projects.path;
                if (inWorkspace)
                {
                    if (console != null) defPath = console.workspace.folder.path;
                }
                path = dialogs.openSelectFile(imbACE.Services.textBlocks.smart.dialogSelectFileMode.selectFileToOpen, "*.txt", defPath, "Select file to import web domains sample from");
                inWorkspace = false;
            }

            if (Path.IsPathRooted(path)) inWorkspace = false;

            if (inWorkspace) {
                if (console != null) path = console.workspace.folder.pathFor(path);
            }

            if (limit == -1) limit = 10000;
            if (skip < 0) skip = 0;

            if (File.Exists(path))
            {
                if (replace)
                {
                    context.sampleList = new webSiteSimpleSample();
                }

                if (!sampleName.isNullOrEmpty()) context.sampleList.name = sampleName;

                var list = path.openFileToList(true);

                Int32 c = 0;
                
                foreach (String l in list)
                {
                    domainAnalysis da = new domainAnalysis(l);
                    if (c < skip)
                    {
                        if (debug)
                        {
                            output.Append(String.Format("Skipping {0,-20} => {1,-20}", l, da.urlProper));
                        }
                    }
                    else
                    {
                        if (c >= limit) break;

                        if (debug)
                        {
                            output.Append(String.Format("Adding   {0,-20} => {1,-20}", l, da.urlProper));
                        }
                        context.sampleList.Add(da.urlProper);
                    }
                }
                
            } else
            {
                output.log("Sample list file not found at [" + path + "]");
            }
        }





        [Display(GroupName = "add", Name = "Plugin", ShortName = "", Description = "Allows additional execution customization by crawling plugin")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will create instance of specified plug in and set it into proper collection")]
        /// <summary>Allows additional execution customization by crawling plugin</summary> 
        /// <remarks><para>It will create instance of specified plug in and set it into proper collection</para></remarks>
        /// <param name="plugin_classname">Proper name of the crawling plugin class</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_addPlugin(
            [Description("Proper name of the crawling plugin class")] String plugin_classname = "*")
        {
            Int32 c = context.pluginStack.Count;
            var plugin = wemTypesManager.wemTypes.crawlPluginTypes.GetInstance(plugin_classname, output);
            plugin.consoleHost = parent as IAceAdvancedConsole;
            plugin.consolePlugin = this;

            if (plugin != null) context.pluginStack.Add(plugin);
        }





        [Display(GroupName = "define", Name = "CJEngineSetup", ShortName = "CJES", Description = @"Crawl Job Engine controls the parallel execution of the Crawl Job. 
            Tdl_max defines max. minutes per one domain level crawl, Tll_max per single link load and TC_max defines number of parallel domain loads.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "This command sets the most important parameters of the Crawl Job execution. For Tdl_max and Tll_max value -1 means limit is off, for TC_max value -1 means auto management.")]
        /// <summary>Crawl Job Engine controls the parallel execution of the Crawl Job.</summary> 
        /// <remarks><para>This command sets the most important parameters of the Crawl Job execution</para></remarks>
        /// <param name="TC_max">Maximum number of parallel DLC executing in the same moment</param>
        /// <param name="Tdl_max">Maximum minutes allowed for single DLC to run</param>
        /// <param name="Tll_max">Maximum minutes of single iteration allowed for a DLC before its termination</param>
        /// <param name="Tcjl_max">Maximum minutes for the complete Crawl Job execution</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_defineCrawlJobEngineSettings(
        [Description("Maximum number of parallel DLC executing in the same moment")] int TC_max = 8,
        [Description("Maximum minutes allowed for single DLC to run")] int Tdl_max = 50,
        [Description("Maximum minutes of single iteration allowed for a DLC before its termination")] int Tll_max = 20,
        [Description("Maximum minutes for the complete Crawl Job execution")] int Tcjl_max = 100)
        {
            var crawlerJobEngineSettings = new crawlerDomainTaskMachineSettings();
            crawlerJobEngineSettings.TC_max = TC_max;
            crawlerJobEngineSettings.Tdl_max = Tdl_max;
            crawlerJobEngineSettings.Tll_max = Tll_max;
            crawlerJobEngineSettings.Tcjl_max = Tcjl_max;

            context.crawlerJobEngineSettings = crawlerJobEngineSettings;
        }


        [aceMenuItem(aceMenuItemAttributeRole.Key, "OpenSession")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "OpenSession")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "OpenSession")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Selects and preloads local index and Experiment session information. useJobSettings option will ignore other params and use Job definition")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will set report output information and create or load local index")]
         /// <summary>
        /// Aces the operation run open session.
        /// </summary>
        /// <param name="experimentSession">The experiment session.</param>
        /// <param name="IndexID">The index identifier.</param>
        /// <param name="useJobSettings">if set to <c>true</c> [use job settings].</param>
        /// <param name="crawlFolderNameTemplate">The crawl folder name template.</param>
        /// <exception cref="aceGeneralException">
        /// You must define Job before calling OpenSession! Check Job command in console help. - null - null - Just have Job defined!
        /// or
        /// You must call CJEngineSetup before OpenSession call - null - CrawlJobEngine setup required
        /// </exception>
        public void aceOperation_runOpenSession(string experimentSession, string IndexID, bool useJobSettings,
          string crawlFolderNameTemplate = "*"
          )
        {

            if (context.job == null) throw new aceGeneralException("You must define Job before calling OpenSession! Check Job command in console help.", null, null, "Just have Job defined!");
            if (context.crawlerJobEngineSettings == null) throw new aceGeneralException("You must call CJEngineSetup before OpenSession call", null, this, "CrawlJobEngine setup required");

            output.log("OpenSession with experimentSession=" + experimentSession + ", IndexID =" + IndexID + ", useJobSettings=" + useJobSettings + ".");

            if (useJobSettings)
            {
                string sessionName = "";
                if (imbWEMManager.settings.directReportEngine.REPORT_DirectoryNameByJobName)
                {
                    sessionName = context.job.name.getCleanPropertyName();
                }

                experimentSession =  sessionName;
                
                

            }
            
            imbWEMManager.index.OpenIndex(IndexID, experimentSession);

            var spiderEvals = context.aRecord.GetChildRecords();
            var frst = spiderEvals.FirstOrDefault();

            string crawlReportFolderName = "";

            if (crawlFolderNameTemplate == "*")
            {
                crawlReportFolderName = frst.instance.name.getCleanFilepath().Replace("-", "");
            }
            else
            {
                crawlReportFolderName = nameComposer.GetCrawlFolderName(context.crawler, context.crawlerJobEngineSettings, crawlFolderNameTemplate);
            }




           context.indexSession = imbWEMManager.index.StartSession(crawlReportFolderName, context);

        }



        [Display(GroupName = "run", Name = "Run", ShortName = "R", Description = "Runs the current crawl job")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Starts crawl execution")]
        /// <summary>Runs the current crawl job</summary> 
        /// <remarks><para>Starts crawl execution</para></remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runRun()
        {

            IAceAdvancedConsole console = parent as IAceAdvancedConsole;

            // your code
            DateTime start = DateTime.Now;
            if (context.aRecord == null)
            {
                output.log("Error: define Job before calling this command.");
                return;
            }

            int Tdl_max = context.crawlerJobEngineSettings.Tdl_max;
            int Tll_max = context.crawlerJobEngineSettings.Tll_max;
            int TC_max = context.crawlerJobEngineSettings.TC_max;

            var spiderEvals = context.aRecord.GetChildRecords();

            
            context.aRecord.initializeSoft(context.sampleList);

            

            int c = 0;



            DirectoryInfo di = imbWEMManager.index.experimentManager.CurrentSession.sessionReportFolder;


            var notation = appManager.AppInfo;

            // ------------------ note creation -------------------
            analyticJobNote note = new analyticJobNote(imbWEMManager.index.experimentEntry.sessionCrawlerFolder);
            note.WriteAboutJob(context, console.workspace, console);

            note.AppendLine("--------------- Crawl Job configuration overview -------------------------- ");
            note.AppendLine("   Script var | Article    - Description                             ");
            note.AppendLine("--------------------------------------------------------------------------- ");
            note.AppendLine("   Tdl_max    | Tdl        - Time limit per domain - in minutes     | : " + Tdl_max);
            note.AppendLine("   Tll_max    | Tac        - Time limit for inactivity - in minutes | : " + Tll_max);
            note.AppendLine("   TC_max     | TC         - Maximum number of JLC threads allowed  | : " + TC_max);
            note.AppendLine("--------------------------------------------------------------------------- ");
            note.AppendHorizontalLine();
            note.AppendLine("-- if the test was finished without problem at the last line it will be message [RunJob completed] ---");
            note.AppendLine("-- if not: something went wrong - check the logs ---");
            note.AppendHorizontalLine();
            note.SaveNote();

            foreach (modelSpiderTestRecord tRecord in spiderEvals)
            {
                c++;
                spiderWebLoaderControler controler = null;

                directAnalyticReporter reporter = new directAnalyticReporter(imbWEMManager.index.experimentEntry.CrawlID, imbWEMManager.index.experimentEntry.sessionCrawlerFolder, notation);



                context.pluginStack.InstallTo(imbWEMManager.index.plugins, plugInGroupEnum.index, true);

                tRecord.performance = imbWEMManager.index.experimentEntry;

                output.log(tRecord.instance.name + " crawl start");


                crawlerDomainTaskMachine cDTM = new crawlerDomainTaskMachine(tRecord, context.aRecord.sample, reporter, di)
                {
                    maxThreads = TC_max,
                    _timeLimitForDLC = Tdl_max,
                    TimeLimitForTask = Tll_max
                };

                //state.pluginStack
                context.pluginStack.InstallTo(cDTM.plugins, plugInGroupEnum.engine, false);
                context.pluginStack.InstallTo(tRecord.instance.plugins, plugInGroupEnum.crawler, false);
                context.pluginStack.InstallTo(cDTM.reportPlugins, plugInGroupEnum.report, false);

                cDTM.startAutoParallel(true); // ----- execution

                output.log(tRecord.instance.name + " crawl finished");

                cDTM.webLoaderControler.Save();

                controler = cDTM.webLoaderControler;
                

                reporter.reportCrawler(tRecord);


                note.WriteAboutCrawlerRun(tRecord, cDTM);

                if (console != null)
                {
                    console.scriptRunning.getContent().saveStringToFile(imbWEMManager.index.experimentEntry.sessionCrawlerFolder.pathFor("script.ace"));
                }




                if (imbWEMManager.settings.directReportEngine.doPublishExperimentSessionTable)
                {
                    imbWEMManager.index.experimentManager.AddOrUpdate(tRecord.performance as experimentSessionEntry);
                }
            }

            
           imbWEMManager.index.CloseSession(spiderEvals);



            output.AppendLine("RunJob done in: " + DateTime.Now.Subtract(start).TotalMinutes.ToString("#0.0##") + " min");
            
            note.AppendLine("[RunJob completed]");
            note.SaveNote();



           // imbWEMManager.settings.Save(imbWEMManager.index.experimentEntry.sessionCrawlerFolder.pathFor("imbAnalyticEngineSettings.xml"));

            var sl = context.sampleList.ToList();
            sl.saveContentOnFilePath(note.folder.pathFor("sample.txt"));
        }



    }


}

