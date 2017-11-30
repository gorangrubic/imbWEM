// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticConsoleWorkspace.cs" company="imbVeles" >
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
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
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
    using imbSCI.Core.data;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    public class analyticConsoleWorkspace : aceAdvancedConsoleWorkspace
    {
        public analyticConsoleWorkspace(aceAdvancedConsole<analyticConsoleState, analyticConsoleWorkspace> __console) : base(__console)
        {
            folderReportOutput = Directory.CreateDirectory("reportOutput");
        }

        public folderNode folderReportOutput { get; set; }

        public override void setAdditionalWorkspaceFolders()
        {
            folder.Add(ACFolders.constructor, "Semantic Lexicon constructor", "Subdirectory with debug logs and meta data created by the Semantic Lexicon Constructor instance");
            folder.Add(ACFolders.samples, "Sample lists", "Serialized webSiteProfileSample lists");
            folder.Add(ACFolders.reports, "Reports", "Content from direct reporter meta data reports");
        }

        /// <summary>
        /// Makes the folder in reportOutput with specified name, optionally adds numeric sufix to ensure it has unique name
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="avoidCollision">if set to <c>true</c> [avoid collision].</param>
        /// <returns></returns>
        public DirectoryInfo makeFolder(string folderName, bool avoidCollision=true)
        {
            DirectoryInfo reportFolder = new DirectoryInfo("reportOutput");
            string repStamp = folderName;
            int c = 0;
            if (avoidCollision)
            {
                while (Directory.Exists(reportFolder.FullName.add(repStamp, "\\")))
                {
                    c++;
                    repStamp = folderName.add(c.ToString("D2"), "_");
                }
            }
            DirectoryInfo repStampFolder = Directory.CreateDirectory(reportFolder.FullName.add(repStamp, "\\"));
            return repStampFolder;
        }

        public string makeFolderName(string folderName, bool avoidCollision = true, DirectoryInfo inDirectory = null)
        {
            if (inDirectory == null) inDirectory = new DirectoryInfo("reportOutput");
            string repStamp = folderName;
            int c = 0;
            if (avoidCollision)
            {
                while (Directory.Exists(inDirectory.FullName.add(repStamp, "\\")))
                {
                    c++;
                    repStamp = folderName.add(c.ToString("D2"), "_");
                }
            }
            return repStamp;
        }




       

        public directAnalyticReporter makeReporter(string crawlerName, aceAuthorNotation notation)
        {
            directAnalyticReporter output = new directAnalyticReporter(crawlerName, folder[ACFolders.reports], this, notation);

            return output;
        }

        //public directJobReporter makeJobReporter(DirectoryInfo repStampFolder, aceAuthorNotation notation)
        //{
        //    directJobReporter output = new directJobReporter("reports", repStampFolder.FullName, this, notation);

        //    return output;
        //}

        public bool sampleExist(string sampleName)
        {
            sampleName = sampleName.ensureEndsWith(".xml");
            string path = folder[ACFolders.samples].pathFor(sampleName);
            return File.Exists(path);
        }

        [Flags]
        public enum sampleAcceptAndPrepareStates
        {
            none =0,
            started = 1,
            existingSampleDetected = 2,
            fromPageIndexImportCalled = 3,
            filesourceFoundForImportFromFile = 4,
            fromDomainIndexImportCalled = 5,
            filesourceNOTFOUND_ForImportFromFile = 6,
            internalSampleFilesourceLoaded = 7,
            groupTagsSpecified_databaseImportCalled = 8,
            noSciProjectFound = 9,
            sampleListStillEmpty = 10,
            sampleListExported = 11,
            filepathArgumentSupplied = 12,
            sourcePathDiscovered = 13,
            sourcePathNOTFOUND = 14,
        }

        /// <summary>
        /// Samples the accept and prepare -- central sample set operation
        /// </summary>
        /// <param name="filepath">The filepath.</param>
        /// <param name="fileHasPriority">if set to <c>true</c> [file has priority].</param>
        /// <param name="group_tags">The group tags.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="fromDomainIndex">Index of from domain.</param>
        /// <param name="fromPageIndex">Index of from page.</param>
        /// <param name="samplefilename">The samplefilename.</param>
        public void sampleAcceptAndPrepare(string filepath = "", bool fileHasPriority = false, string group_tags = "", int limit = 0, 
            int skip = 0, indexDomainContentEnum fromDomainIndex = indexDomainContentEnum.none, indexPageEvaluationEntryState fromPageIndex = indexPageEvaluationEntryState.none,
            string samplefilename="")
        {
            List<string> domains = new List<string>();
            List<string> pages = new List<string>();
            sampleAcceptAndPrepareStates processState = sampleAcceptAndPrepareStates.started;

            analyticConsoleState state = console.state as analyticConsoleState;

            string sourcePath = filepath;

            if (!filepath.isNullOrEmpty())
            {
                processState |= sampleAcceptAndPrepareStates.filepathArgumentSupplied;
                sourcePath = folder.findFile(filepath, SearchOption.AllDirectories, false);
                if (sourcePath.isNullOrEmpty())
                {
                    processState |= sampleAcceptAndPrepareStates.sourcePathDiscovered;
                }
                else
                {
                    processState |= sampleAcceptAndPrepareStates.sourcePathNOTFOUND;
                }
            }

            if (state.aRecord == null)
            {
                console.log("You should define job before calling this command!", true);
                return;
            }

            int startSampleCount = 0;

            if (state.sampleList != null)
            {
                startSampleCount = state.sampleList.Count();
                processState |= sampleAcceptAndPrepareStates.existingSampleDetected;
            } else
            {
                state.sampleList = new webSiteSimpleSample();
            }

            
            state.sampleTags = group_tags;
            state.sampleFile = filepath;
            console.response.log("Sample with group_tags=" + group_tags + ", samplename=" + filepath + ", fileHasPriority=" + fileHasPriority + ".");

            // ==============================================================================================
            if (!state.sampleList.Any())
            {
                if (fromPageIndex != indexPageEvaluationEntryState.none) // -------------------------- LOADING FROM THE PAGE INDEX
                {
                    processState |= sampleAcceptAndPrepareStates.fromPageIndexImportCalled;
                    List<indexDomain> dSample = new List<indexDomain>();

                    var pageList = imbWEMManager.index.pageIndexTable.GetPagesAndDomains(fromPageIndex, out dSample);
                    if (imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleTake) dSample.Randomize();
                    state.sampleList.Add(dSample);
                }
            }

            // ==============================================================================================
            if (!state.sampleList.Any()) 
            {
                if (fromDomainIndex != indexDomainContentEnum.none) // -------------------------- LOADING FROM THE DOMAIN INDEX
                {
                    processState |= sampleAcceptAndPrepareStates.fromDomainIndexImportCalled;
                    var list = imbWEMManager.index.domainIndexTable.GetDomainUrls(fromDomainIndex);
                    if (imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleTake) list.Randomize();
                    foreach (string str in list) state.sampleList.Add(str);
                }
            }

            if (!state.sampleList.Any())  // -------------------------- LOADING THE EXTERNAL SAMPLE FILE
            {
                
                if (!sourcePath.isNullOrEmpty())
                {
                    processState |= sampleAcceptAndPrepareStates.filesourceFoundForImportFromFile;
                    var domainList = sourcePath.openFileToList(true);
                    if (imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleTake) domainList.Randomize();
                    state.sampleList.Add(domainList, skip, limit);
                    console.response.log("Sample external file list [" + samplefilename + "] found at [" + sourcePath + "] containing [" + domainList.Count() + "] domains.");
                } else
                {
                    processState |= sampleAcceptAndPrepareStates.filesourceNOTFOUND_ForImportFromFile;
                }
            }

            if (!state.sampleList.Any()) // -------------- LOADING THE INTERNAL SAMPLE FILE
            {

                if (!filepath.isNullOrEmptyString())
                {
                    if (fileHasPriority && sampleExist(filepath))
                    {
                        processState |= sampleAcceptAndPrepareStates.internalSampleFilesourceLoaded;
                        state.sampleList = loadSample(filepath, imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleTake);
                    }
                    else
                    {
                        if (state.sciProject != null) // -------------------------- LOADING FROM THE DATABASE 
                        {
                            if (!group_tags.isNullOrEmpty())
                            {
                                processState |= sampleAcceptAndPrepareStates.groupTagsSpecified_databaseImportCalled;
                              //  state.sampleList = state.sciProject.getSamples(group_tags.getTokens(), limit, "stamp", 0, imbWEMManager.settings.crawlerJobEngine.doRandomizeSampleTake);
                            }
                        } else
                        {
                            processState |= sampleAcceptAndPrepareStates.noSciProjectFound;
                        }
                    }
                }
                else
                {
                   // sample = state.sampleList;
                }
            }

            if (!state.sampleList.Any())
            {
                processState |= sampleAcceptAndPrepareStates.sampleListStillEmpty;
                var ace = new aceGeneralException("Sample creation failed: " + processState.ToString(), null, this, "Sample import failed :: ");
                throw ace;
                return;
            }

            int AddedSampleCount = state.sampleList.Count - startSampleCount;

            console.log("Added to the sample list [" + AddedSampleCount + "] at current job record [" + state.aRecord.job.name + "]", true);
           

            // ==============================================================================================

            if (!filepath.isNullOrEmpty()) // -------------------------- EXPORTING INTO LOCAL XML FILE
            {
                processState |= sampleAcceptAndPrepareStates.sampleListExported;
                var fi = saveSample(filepath, state.sampleList);
                console.output.log("Sample list exported to: " + fi.Name);
                state.sampleFile = fi.Name;
            }

            console.output.AppendLine("--- loged sample import procedure states: [" + processState.ToString() + "]");
        }


        /// <summary>
        /// Loads the sample.
        /// </summary>
        /// <param name="sampleName">Name of the sample.</param>
        /// <returns></returns>
        public webSiteSimpleSample loadSample(string sampleName, bool random=false)
        {
            sampleName = sampleName.ensureEndsWith(".xml");
            string path = folder[ACFolders.samples].pathFor(sampleName);
            webSiteSimpleSample output = objectSerialization.loadObjectFromXML(path, typeof(webSiteSimpleSample)) as webSiteSimpleSample;
            if (random) output.Randomize();
            return output;
        }

        /// <summary>
        /// Saves the sample.
        /// </summary>
        /// <param name="sampleName">Name of the sample.</param>
        /// <param name="sample">The sample.</param>
        /// <returns></returns>
        public FileInfo saveSample(string sampleName, webSiteSimpleSample sample)
        {
            sampleName = Path.GetFileNameWithoutExtension(sampleName);
            sampleName = sampleName.ensureEndsWith(".xml");
            string path = folder[ACFolders.samples].pathFor(sampleName);
            webSiteSimpleSample simpleSample = sample;
            objectSerialization.saveObjectToXML(simpleSample, path);
            return new FileInfo(path);
        }
    }

}