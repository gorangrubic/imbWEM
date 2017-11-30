// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderWebLoaderControler.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webStructure;
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
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Helps WebLoader optimization by keeping record on unaccessable urls and page duplicates
    /// </summary>
    public class spiderWebLoaderControler
    {
        protected folderNode folder { get; set; }
        protected fileunit failList { get; set; }
        protected fileunit domainFailList { get; set; }
        protected fileunit duplicateList { get; set; }

        protected ConcurrentDictionary<string, string> duplicates { get; set; } = new ConcurrentDictionary<string, string>();


        protected int saveIndex { get; set; } = 0;

        protected const int saveIndexStart = 1000;


        private object CheckLock = new object();

        /// <summary>
        /// Checks the fail: TRUE if url is known to fail
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public bool CheckFail(string url)
        {
            lock (CheckLock) {
                if (failList.contentLines.Contains(url)) return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the fail URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void SetFailUrl(string url)
        {
            failList.AppendUnique(new string[] { url }, false);

            saveIndex--;

            if (saveIndex < 1)
            {
                Save();
                saveIndex = saveIndexStart;
            }

        }

        /// <summary>
        /// Sets the duplicate URL.
        /// </summary>
        /// <param name="urlDuplicate">The URL duplicate.</param>
        /// <param name="urlOriginal">The URL original.</param>
        public void SetDuplicateUrl(string urlDuplicate, string urlOriginal)
        {
            if (!duplicates.ContainsKey(urlDuplicate))
            {
                duplicates.TryAdd(urlDuplicate, urlOriginal);
                duplicateList.Append(urlDuplicate + "|||" + urlOriginal);
            }
        }

        /// <summary>
        /// Gets the duplicate URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public string GetDuplicateUrl(string url)
        {
            if (duplicates.ContainsKey(url))
            {
                return duplicates[url];
            } else
            {
                return url;
            }
        }

        public const string FILE_FAILLIST = "webLoader_failList.txt";
        public const string FILE_DOMAINFAILLIST = "webLoader_domainFailList.txt";
        public const string FILE_DUPLICATE = "webLoader_duplicateList.txt";

        /// <summary>
        /// Prepares the specified loger.
        /// </summary>
        /// <param name="loger">The loger.</param>
        public void prepare(ILogBuilder loger, folderNode __folder)
        {
            folder = __folder;

            failList = new fileunit(folder.pathFor(FILE_FAILLIST),true);
            domainFailList = new fileunit(folder.pathFor(FILE_DOMAINFAILLIST), true);
            duplicateList = new fileunit(folder.pathFor(FILE_DUPLICATE),true);

            foreach (string ln in duplicateList.contentLines)
            {
                var sp = ln.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                duplicates.TryAdd(sp[0], sp[1]);
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            failList.Save();

            duplicateList.Save();
            domainFailList.Save();
        }

        protected Dictionary<string, webSiteProfile> failedSample = new Dictionary<string, webSiteProfile>();

        /// <summary>
        /// Returns list of failed domains
        /// </summary>
        /// <returns></returns>
        public List<webSiteProfile> GetFailedProfiles()
        {
            return failedSample.Values.ToList();
        }

        public List<string> GetFailedDomains()
        {
            return failedSample.Keys.ToList();
        }

        public int GetFailedURLsCount()
        {
            return failList.contentLines.Count();
        }

        public bool isFailedDomain(string domain)
        {
            if (domainFailList.contentLines.Contains(domain))
            {
                return true;
            }

            return failedSample.ContainsKey(domain);
        }

        public bool isFailedDomain(webSiteProfile wProfile)
        {
            if (domainFailList.contentLines.Contains(wProfile.domain))
            {
                return true;
            }

            return failedSample.ContainsValue(wProfile);
        }

        /// <summary>
        /// Sets the failed domain.
        /// </summary>
        /// <param name="wProfile">The w profile.</param>
        public void SetFailedDomain(webSiteProfile wProfile, modelSpiderSiteRecord wRecord=null)
        {
            if ((wRecord == null) && (wProfile == null))
            {
                var axe = new aceGeneralException("Supplied wProfile and wRecord are null - this task should never run on the first place", null, this, "SetFailedDomain(webSiteProfile null)");
                throw axe;
            }
            else
            {
                string domain = "[unknown]";
                if (wProfile != null) domain = wProfile.domain;
                if (wRecord != null) domain = wRecord.domain;

                if (!failedSample.ContainsKey(domain))
                {
                    failedSample.Add(domain, wProfile);
                    domainFailList.Append(domain, true);
                }
            }
        }
    }

}