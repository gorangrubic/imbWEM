// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticJobNote.cs" company="imbVeles" >
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
namespace imbWEM.Core.project
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
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
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Builder for text note about performed experiment
    /// </summary>
    /// <seealso cref="aceCommonTypes.reporting.render.builders.builderForLog" />
    public class analyticJobNote:builderForLog
    {
        public folderNode folder { get; set; }

        public analyticJobNote(folderNode __folder)
        {
            folder = __folder;
        }

        public void WriteAboutJob(analyticConsoleState state, analyticConsoleWorkspace workspace, analyticConsole console)
        {
            log(":: " + state.job.name + " ::");
            
            AppendLine("Test ID: " + imbWEMManager.index.experimentEntry.TestID);
            AppendLine("Test signature: " + imbWEMManager.index.experimentEntry.TestSignature);

            AppendLine("Test description: " + state.job.description);
            AppendLine("Test report folder: " + folder.path);

            

            AppendHorizontalLine();

            AppendLine("Sample size: " + state.sampleList.Count());
            
            AppendLine("Sample subsets: " + state.sampleTags);
            AppendLine("Sample block: " + state.aRecord.sampleBlockOrdinalNumber);
            AppendLine("Sample limit: " + state.aRecord.sampleTakeLimit);
            AppendLine("Sample file name: " + state.sampleFile);
            AppendLine("Sample order hash: " + imbWEMManager.index.experimentEntry.SampleListHash);


            AppendLine("Index auto-preloaded: " + imbWEMManager.index.doAutoLoad);
            AppendLine("Lexicon preloaded: " +semanticLexiconManager.lexiconCache.isLexiconPreloaded);

            AppendHorizontalLine();
            AppendLine("Run stamp: " + state.aRecord.testRunStamp);
            string hash = md5.GetMd5Hash(objectSerialization.ObjectToXML(imbWEMManager.settings));
            state.setupHash_global = hash;

            AppendLine("Settings [imbAnalyticEngine] hash: " + hash);
            AppendLine(" ^-- to make sure multiple tests were running under the same settings compare the hash");

            

            if (console.scriptRunning != null)
            {
                AppendHorizontalLine();
                AppendLine("Job started by ACE script: " + console.scriptRunning.info.Name);
                AppendLine("=== Content of the script == start ======");
                AppendLine(console.scriptRunning.getContent());
                AppendLine("=== Content of the script == end   ======");
            }

            AppendHorizontalLine();

            if (imbWEMManager.commandArgs.Any())
            {
                
                AppendLine("Application was started with commandline arguments: ");
                int a = 1;
                foreach (string arg in imbWEMManager.commandArgs)
                {
                    AppendLine(a.ToString("D3") + " [" + arg + "]");
                }
                AppendHorizontalLine();
            }
            else
            {
                AppendLine("Application was started without commandline arguments ");
                AppendHorizontalLine();
            }

            var process = Process.GetCurrentProcess();
            process.Refresh();

            AppendLine("Process name: " + process.ProcessName);
            AppendLine("Process ID: " + process.Id);
            

            AppendHorizontalLine();

        }

        public void WriteAboutCrawlerRun(modelSpiderTestRecord tRecord, crawlerDomainTaskMachine cDTM, analyticConsoleState state)
        {
            AppendHorizontalLine();
            AppendLine("Crawler name:           " + cDTM.tRecord.instance.name);
            AppendLine("Crawler description:    " + cDTM.tRecord.instance.description);

            AppendLine("Session report folder:      " + cDTM.folder.path);
            AppendLine("Crawler report folder:      " + imbWEMManager.index.experimentEntry.sessionCrawlerFolder.path);
            AppendLine("--------------------------------------------------------------------------- ");

            string settings = objectSerialization.ObjectToXML(tRecord.instance.settings);
            string hash = md5.GetMd5Hash(settings);

            var fileinfo = settings.saveStringToFile(
                imbWEMManager.index.experimentEntry.sessionCrawlerFolder.pathFor(imbWEMManager.index.experimentEntry.SessionID.getFilename() + "_settings.xml").getWritableFile().FullName);

            state.setupHash_crawler = hash;

            AppendLine("Crawler settings hash:  " + hash);
            AppendLine("Crawler complete hash:  " + tRecord.instance.crawlerHash);
            //  AppendLine("Crawler settings file:  " + fileinfo.Name);

            AppendLine("--------------- Crawler configuration overview ---------------------------- ");



            AppendLine("PL_max    	   | PL         - Page Load max per domain    	        | : " + tRecord.instance.settings.limitTotalPageLoad);
            AppendLine("LT_t    	   | LT         - Load Take per iteration    	        | : " + tRecord.instance.settings.limitIterationNewLinks);
            AppendLine("I_max    	   | I_max      - Iteration limit per DLC	            | : " + tRecord.instance.settings.limitIterations);

            AppendLine("PS_c *         |            - Page Select count (not used)          | : " + tRecord.instance.settings.primaryPageSetSize);
            AppendLine("--------------------------------------------------------------------------- ");
            AppendLine("-- * parameteers not used in by this version ------------------------------ ");
            AppendLine();

            AppendLine("--------------------------------------------------------------------------- ");


           



            var duration = DateTime.Now.Subtract(cDTM.startTime);
            AppendLine("Start time:         " + cDTM.startTime.ToShortTimeString());
            AppendLine("Finish time:        " + DateTime.Now.ToShortTimeString());
            AppendLine("Duration (minutes): " + duration.TotalMinutes);
            AppendLine("^-- includes post-crawl reporting and index database update");

            AppendLine("Failed domains:     " + cDTM.webLoaderControler.GetFailedDomains().Count());
            AppendLine("^-- includes domains that were accesable but no links discovered    ");
            AppendLine("Failed URLs:        " + cDTM.webLoaderControler.GetFailedURLsCount());
            
            AppendHorizontalLine();

           // cDTM.tRecord.instance.Describe(this);
        }


        /// <summary>
        /// Saves the note into assigned folder. Default name: note.txt
        /// </summary>
        /// <param name="name">The name.</param>
        public void SaveNote(string name="", bool toPDF=false)
        {
            if (name.isNullOrEmptyString())
            {
                name = "note";
            }

            if (toPDF)
            {
                name = name.ensureEndsWith(".pdf");
            } else
            {
                name = name.ensureEndsWith(".txt");
                
            }
            
            string path = folder.pathFor(name).getWritableFile().FullName;
            // aceCommonTypes.reporting.reportOutputQuickTools.saveMarkdownToPDF

            if (toPDF)
            {
                ContentToString().saveMarkdownToPDF(path, false);
            } else
            {
                ContentToString().saveStringToFile(path);
            }
        }
    }
}
