// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderWeb.cs" company="imbVeles" >
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.pageAnalytics.core;
    using imbCommonModels.structure;
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
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    /// <seealso cref="IObjectWithNameAndDescription" />
    public class spiderWeb:imbBindable, IObjectWithNameAndDescription
    {
        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description { get; set; } = "";


        /// <summary>
        /// Gets the data table summary.
        /// </summary>
        /// <returns></returns>
        public DataTable getDataTableSummary()
        {
            PropertyCollectionExtended dataExtended = AppendDataFields(null) as PropertyCollectionExtended;

            DataTable output = dataExtended.getDataTable(PropertyEntryColumn.entry_name, PropertyEntryColumn.entry_value, PropertyEntryColumn.entry_description);

            return output;
        }



        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public PropertyCollection AppendDataFields(PropertyCollection data = null)
        {
            PropertyCollectionExtended dataExtended = new PropertyCollectionExtended();

            dataExtended.Add("seed", seedLink.link.url, "Start url", "URL the spider started from");
            dataExtended.Add("links", webActiveLinks.Count(), "Links", "Number of links discovered during the spider operation");
            dataExtended.Add("pages", webPages.items.Count(), "Pages", "Number of pages discovered during the spider operation");
            dataExtended.Add("pages_result", webPages.items.Count(), "Page set", "Pages accepted for further analysis");
            dataExtended.Add("flags", flags, "Flags", "Flags about the spider operation");

            
            return dataExtended;
        }



        private instanceCountCollection<string> _webPageContentHashList = new instanceCountCollection<string>();
        /// <summary>Frequencies of webPage content hashes </summary>
        public instanceCountCollection<string> webPageContentHashList
        {
            get
            {
                return _webPageContentHashList;
            }
            protected set
            {
                _webPageContentHashList = value;
                OnPropertyChanged("webPageContentHashList");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderActiveLinks webActiveLinks { get; set; } = new spiderActiveLinks();


        private spiderWebPageRegistry _webPages = new spiderWebPageRegistry();
        /// <summary>Registry of loaded pages </summary>
        public spiderWebPageRegistry webPages
        {
            get
            {
                return _webPages;
            }
            protected set
            {
                _webPages = value;
                OnPropertyChanged("webPages");
            }
        }


        private spiderWebLinkTargetRegistry _webTargets = new spiderWebLinkTargetRegistry();
        /// <summary> </summary>
        public spiderWebLinkTargetRegistry webTargets
        {
            get
            {
                return _webTargets;
            }
            protected set
            {
                _webTargets = value;
                OnPropertyChanged("webTargets");
            }
        }



        private spiderWebLinkRegistry _webLinks = new spiderWebLinkRegistry();
        /// <summary> </summary>
        public spiderWebLinkRegistry webLinks
        {
            get
            {
                return _webLinks;
            }
            protected set
            {
                _webLinks = value;
                OnPropertyChanged("webLinks");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderLink seedLink { get; set; }


        private string _domain; //= new String();
        /// <summary> </summary>
        public string domain
        {
            get
            {
                return _domain;
            }
            protected set
            {
                _domain = value;
                OnPropertyChanged("domain");
            }
        }


        public spiderWeb()
        {
            //seed = __seedPage;
            
            //allPages.AddSpiderPageByRef(seed);
        }

        public const string ORIGIN_OF_ROOTURL = "imb.veles.rs";

        public spiderLink setSeedUrl(string rootUrl)
        {
            link lnk = new link(rootUrl, linkProcessFlags.standard);
            Uri __rootUrl = new Uri(rootUrl);

            crawledPage cpage = new crawledPage(ORIGIN_OF_ROOTURL, 0);
            
            spiderPage spage = new spiderPage(cpage, 0, 0);
           // webPages.Add(spage);
            spiderLink splink = new spiderLink(spage, lnk, 1);
            //splink.li = lnk;//allLinks.AddSpiderLink(lnk);
            seedLink = splink;
            name = rootUrl;
            splink.domain = __rootUrl.Host;
            domain = __rootUrl.Host;
            splink.link.domain = domain;
            //webLinks.Add(splink);
            //webTargets.Add(splink);
            return splink;
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderWebFlags flags { get; set; } = spiderWebFlags.none;


        public PropertyCollectionExtended getLinkStatistics()
        {
            return AppendDataFields() as PropertyCollectionExtended;
        }

      
      

    }
}
