// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexPageTable.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.tools;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webPage;
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class indexPageTable:objectTable<indexPage>
    {
        public indexPageTable(string __keyProperty, string __tableName) : base(__keyProperty, __tableName)
        {
        }

        public indexPageTable(string __filePath, bool autoLoad=true) : base(__filePath, autoLoad)
        {
        }

        private void makeStat(bool callAnswered)
        {
            if (imbWEMManager.index.indexSessionEntry != null)
            {
                if (callAnswered)
                {
                    imbWEMManager.index.indexSessionEntry.PositiveCalls++;
                }
                else
                {
                    imbWEMManager.index.indexSessionEntry.NegativeCalls++;
                }
            }
        }

        protected override string GetPrimaryKeySelect(string keyValue)
        {
          //  keyValue = md5.GetMd5Hash(keyValue);

            return base.GetPrimaryKeySelect(keyValue);
        }


        public aceConcurrentBag<string> urlsNotInIndex { get; set; } = new aceConcurrentBag<string>();


        /// <summary>
        /// Returns flags describing index's knowledge about this page
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public indexPageEvaluationEntryState GetPageAssertion(string url)
        {
            string key = md5.GetMd5Hash(url);
            if (!ContainsKey(key))
            {
                urlsNotInIndex.AddUnique(url);
                return indexPageEvaluationEntryState.notInTheIndex;
            }
            
            indexPageEvaluationEntryState output = indexPageEvaluationEntryState.inTheIndex;

            var page = GetOrCreate(key);

            switch (page.relevancyText)
            {
                case "isRelevant":
                    makeStat(true);
                    output |= indexPageEvaluationEntryState.haveEvaluationEntry | indexPageEvaluationEntryState.isRelevant;
                    break;
                case "notRelevant":
                    makeStat(true);
                    output |= indexPageEvaluationEntryState.haveEvaluationEntry | indexPageEvaluationEntryState.notRelevant;
                    break;
                default:
                    makeStat(false);
                    output |= indexPageEvaluationEntryState.haveNoEvaluationEntry;
                    break;
            }

            return output;
        }

        public indexPage GetPageForUrl(string url)
        {
            return GetOrCreate(md5.GetMd5Hash(url));
        }

        public List<indexPage> GetPagesForUrls(IEnumerable<string> urls)
        {
            List<indexPage> output = new List<indexPage>();
            foreach (string url in urls)
            {
                output.Add(GetOrCreate(md5.GetMd5Hash(url)));
            }
            return output;
        }


        /// <summary>
        /// Gets the URL assertion and returns metrics to the provided variables
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <param name="relevant">The relevant.</param>
        /// <param name="notRelevant">The not relevant.</param>
        /// <param name="notKnown">The not known.</param>
        /// <param name="notInIndex">Index of the not in.</param>
        public indexURLAssertionResult GetUrlAssertion(IEnumerable<string> urls, indexURLAssertionResult output=null) // , out Int32 relevant, out Int32 notRelevant, out Int32 notKnown, out Int32 notInIndex
        {

            if (output == null) output = new indexURLAssertionResult();
            

            foreach (string url in urls)
            {
                indexPageEvaluationEntryState state = GetPageAssertion(url);
                output.Add(state, url);
            }

            return output;

        }


        /// <summary>
        /// Gets the URL assertion from collection of spider links
        /// </summary>
        /// <param name="urls">The urls.</param>
        /// <returns></returns>
        public indexURLAssertionResult GetUrlAssertion(IEnumerable<spiderLink> urls) // , out Int32 relevant, out Int32 notRelevant, out Int32 notKnown, out Int32 notInIndex
        {
            indexURLAssertionResult output = new indexURLAssertionResult();


            foreach (spiderLink url in urls)
            {
                indexPageEvaluationEntryState state = GetPageAssertion(url.url);
                output.Add(state, url.url);
            }

            return output;
        }



        /// <summary>
        /// Gets the pages with acceptable state checked with <see cref="GetPageAssertion(string)"/>, populates the list od domains.
        /// </summary>
        /// <param name="aceptableState">State of the aceptable.</param>
        /// <returns>Returns index pages and populates string list of domains</returns>
        public List<indexPage> GetPagesAndDomains(indexPageEvaluationEntryState acceptableState, out List<indexDomain> domains)
        {
            List<indexPage> output = new List<indexPage>();
            domains = new List<indexDomain>();

            foreach (indexPage page in this)
            {
                indexPageEvaluationEntryState state = GetPageAssertion(page.url);

                if (state.HasFlag(acceptableState))
                {
                    var dom = imbWEMManager.index.domainIndexTable[page.domain];

                    if (!Enumerable.Any(domains, x=>x.domain==page.domain))
                    {
                        
                        domains.Add(dom);
                    }
                    dom.addToPageSet(page);

                    output.Add(page);
                }
            }
            
            return output;
        }


        //public String SetPageTFIDF(webPageTF wpTF, indexDomain idomain)
        //{
        //    String hash = idomain.HashCode + "-" + wpTF.ipage.HashCode;
        //    var dt = wpTF.GetDataTable(wpTF.ipage.url);
        //    String path = imbWEMManager.index.folder.pathFor(hash + ".xml");

        //    objectSerialization.saveObjectToXML<webPageTF>(path, wpTF);
        //    dt.Save(imbWEMManager.index.experimentEntry.crawlRecordFolder, imbWEMManager.authorNotation, hash);

        //    wpTF.ipage.TFIDFcompiled = true;

        //    wpTF.ipage.Lemmas = wpTF.Count();


        //    AddOrUpdate(wpTF.ipage);

        //    dt.GetReportAndSave(imbWEMManager.index.experimentEntry.crawlRecordFolder, imbWEMManager.authorNotation, hash, true);
        //    return path;
        //}


        /// <summary>
        /// Returns all pages for the domain specified
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public List<indexPage> GetPagesForDomain(string domainName)
        {
            domainAnalysis da = new domainAnalysis(domainName);
            

            var rows = tableSelect("domain = '" + da.domainName + "'");

            var pages = GetObjectFromRows(rows);

            List<indexPage> output = new List<indexPage>();
            List<string> urls = new List<string>();
            foreach (indexPage page in pages)
            {
                if (urls.Contains(page.url))
                {

                } else
                {
                    output.Add(page);
                    urls.Add(page.url);
                }
                
            }
            return output;
        }
        
    }

}