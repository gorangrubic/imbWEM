// --------------------------------------------------------------------------------------------------------------------
// <copyright file="imbWEMManager.cs" company="imbVeles" >
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
namespace imbWEM.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.cache;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.extensions;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbNLP.Data;
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
    using imbSCI.Core.data;
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
    using imbWEM.Core.index;
    using imbWEM.Core.project;
    using imbWEM.Core.sampleGroup;
    using imbWEM.Core.stage;

    #region imbVELES USING

    #endregion


    /// <summary>
    /// Analiticki manager
    /// </summary>
    public static class imbWEMManager
    {
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static indexManager index { get; private set; }


        private static List<string> _indexAllFilenames;
        /// <summary>
        /// List of all known index filenames, not only HTML
        /// </summary>
        public static List<string> indexAllFilenames
        {
            get
            {
                if (_indexAllFilenames == null)
                {
                    _indexAllFilenames = new List<string>();
                    _indexAllFilenames.AddRange(new string[] {"index.pl",
                                                     "index.html",
                                                     "index.htm",
                                                     "index.shtml",
                                                     "index.php",
                                                     "index.php5",
                                                     "index.php4",
                                                     "index.php3",
                                                     "index.cgi",
                                                     "default.html",
                                                     "default.htm",
                                                     "home.html",
                                                     "home.htm",
                                                     "Index.html",
                                                     "Index.htm",
                                                     "Index.shtml",
                                                     "Index.php",
                                                     "Index.cgi",
                                                     "Default.html",
                                                     "Default.htm",
                                                     "Home.html",
                                                     "Home.htm",
                                                     "placeholder.html" });
                }
                return _indexAllFilenames;
            }
        }


        private static List<string> _indexHtmlFilenames;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static List<string> indexHtmlFilenames
        {
            get
            {
                if (_indexHtmlFilenames == null)
                {
                    _indexHtmlFilenames = new List<string>();
                    _indexHtmlFilenames.AddRange(new string[] { "index.html", "index.htm", "default.html", "default.htm" });
                }
                return _indexHtmlFilenames;
            }
        }


        /// <summary>
        /// Removes known default filenames
        /// </summary>
        /// <param name="originalUrl">The original URL.</param>
        /// <returns></returns>
        public static string equalizeUrlWithIndexFilenames(this string originalUrl)
        {
            string output = originalUrl;
            foreach (string fn_end in indexAllFilenames)
            {
                if (originalUrl.EndsWith(fn_end,StringComparison.InvariantCultureIgnoreCase))
                {
                    output = output.substring(output.Length - fn_end.Length);
                    output = output.ensureEndsWith("/");
                    return output;
                }
            }
            return output;
        }


        /// <summary>
        /// Globalni terminator Crawl joba
        /// </summary>
        /// <value>
        ///   <c>true</c> if [masterkill switch]; otherwise, <c>false</c>.
        /// </value>
        public static bool MASTERKILL_SWITCH { get; set; } = false;

        private static imbWEMManagerSettings _settings;
        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static imbWEMManagerSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new imbWEMManagerSettings();
                    _settings.Load();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }







        private static aceAuthorNotation _authorNotation;
        /// <summary>
        /// 
        /// </summary>
        public static aceAuthorNotation authorNotation
        {
            get
            {
                if (_authorNotation == null)
                {
                    _authorNotation = new aceAuthorNotation();
                    //_notation.name = base.notation.software;
                    //_notation.organization = base.notation.organization; // "KOPLAS PRO ltd Serbia | Faculty of Organizational Sciences, Belgrade University, Serbia";
                    //_notation.author = base.notation.author; // "Goran Grubić";
                    //_notation.copyright = notation.copyright; // "All Rights reserved © 2013-2017.";
                    //_notation.content.Add(notation.comment);
                }

                return _authorNotation;
            }
            set { _authorNotation = value; }
        }



        #region -----------  webProfileGroups  -------  [Set grupa za web profile]

        /// <summary>
                                                                                   /// Set grupa za web profile
                                                                                   /// </summary>
        [XmlIgnore]
        [Category("analyticProject")]
        [DisplayName("webProfileGroups")]
        [Description("Set grupa za web profile")]
        public static sampleGroupSetInPhd webProfileGroups { get; } = new sampleGroupSetInPhd();

        #endregion


        /// <summary>
        ///
        /// </summary>
        public static analyticJob mainAnalyticJob { get; set; } = new analyticJob();


        /// <summary>
        ///
        /// </summary>
        public static builderForLog log { get; set; } = new builderForLog(logOutputSpecial.analyticEngine);

        public static void GCCall(string contextMessage = "")
        {
            long memBefore = GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForFullGCComplete();
            long memAfter = GC.GetTotalMemory(true);


            long released = memBefore - memAfter;
            aceLog.log("Memory released [" + contextMessage + "] " + released.getMByteCountFormated() + "");
            
        /// <summary>
        }


        /// <summary>
        /// 
        /// </summary>
        public static List<string> commandArgs { get; set; } = new List<string>();


        /// sluzi samo da bi AnalyticEngine bio referenciran
        /// </summary>
        public static void prepare(analyticProject sciProject)
        {
            aceLog.consoleControl.setAsOutput(log, "analitic");
            // <----------------------- ova nebuloza ispod je zbog multithreadinga ---
            string tmp = "\\=".getCleanFilepath();
            tmp = tmp.getCleanFileName();
            tmp = tmp.getFilename("tmp");
            // <----------------------------------------------------------------------

            if (sciProject != null)
            {
                
              //  sciProject.afterFinalDeploy();

            }

            if (settings.supportEngine.doLanguagePrepareCall)  imbLanguageFrameworkManager.Prepare();

            

            settings.prepare();



            //imbSemanticEngine.imbSemanticEngineManager.prepare();

            imbSCI.Reporting.script.docScriptAppendExtensions.defaultRowLimit = settings.postReportEngine.tableExcerptRowLimitDefault;

            if (settings.loaderComponent.cacheDoUseExternalPath)
            {
                log.log("Using external path for cache root");
                cacheSystem.defaultCacheDirectoryPath = settings.loaderComponent.cacheExternalPathForCache;
                
            }

            log.log("Cache path: " + cacheSystem.defaultCacheDirectoryPath);

            log.log("imbAnalyticEngineManager->prepare() call");

            builderForLog.autoFlushDisabled = !settings.executionLog.doAutoflushLogs;
            builderForLog.autoFlushLength = settings.executionLog.logAutoflushLength;

            md5.isCacheEnabled = settings.supportEngine.doChacheMD5;

           // imbSemanticEngine.contentTree.contentTreeBuilder.doKeepLog = imbWEMManager.settings.executionLog.doKeepTreeBuilderLog;

            imbSCI.Reporting.script.docScriptAppendExtensions.doAllowExcelAttachments = settings.postReportEngine.reportBuildDoExcelExport;
            imbSCI.Reporting.script.docScriptAppendExtensions.doAllowJSONAttachments = settings.postReportEngine.reportBuildDoJSONExport;




            //webProfileGroups.setGroupCounts(sciProject.mainWebProfiler.webSiteProfiles);

            //webProfileGroups.describe(log);

            log.log("Index/home page filenames count: " + indexHtmlFilenames.Count);
            log.log("All index page filenames count: " + indexAllFilenames.Count);


            if (index == null)
            {
                index = new indexManager(settings.indexEngine.doIndexLoadMainIndex);
            }


            aceLog.consoleControl.removeFromOutput(log);
        }
    }
}