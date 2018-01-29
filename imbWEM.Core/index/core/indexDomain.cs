// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexDomain.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.enumworks;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    [XmlInclude(typeof(indexDomainContentEnum))]
    [imb(imbAttributeName.reporting_categoryOrder, "Link,IndexData,Information Volume")]
    [imb(imbAttributeName.help, "Information Prize is sum of application domain level TF-IDF weights for lemmas found on a page")]
    public class indexDomain
    {
        /// <summary>
        /// GCompiled tf_idf
        /// </summary>
        /// <value>
        /// The tf idf compiled.
        /// </value>
       // [XmlIgnore]
       // public webSiteLemmaTFSetObjectTable tf_idf_compiled { get; set; }

        public indexDomain()
        {

        }

        protected List<indexPage> pageSet { get; set; } = new List<indexPage>();
        
        public void addToPageSet(indexPage page)
        {
            pageSet.Add(page);
        }

        public List<indexPage> getPageSet()
        {
            return pageSet;
        }



        /// <summary>
        /// URL to the landing page
        /// </summary>
        [Category("Link")]
        [DisplayName("Full URL")]
        [Description("URL to the landing page")]
        public string url { get; set; }

        /// <summary>
        /// Rechecks the specified pages.
        /// </summary>
        /// <param name="pages">The pages.</param>
        public void recheck(List<indexPage> pages)
        {
            int relCount = 0;
            int nonRelCount = 0;
            int detCount = 0;

            foreach (indexPage page in pages)
            {
                switch (page.relevancy)
                {
                    case indexPageRelevancyEnum.isRelevant:
                        relCount++;
                        break;
                   
                    case indexPageRelevancyEnum.notRelevant:
                        nonRelCount++;
                        break;
                    case indexPageRelevancyEnum.none:
                    case indexPageRelevancyEnum.unknown:
                        detCount++;
                        break;
                    default:
                        break;
                }
            }

            _relevantPages = relCount;
            _notRelevantPages = nonRelCount;
            detCount = detCount + relCount + nonRelCount;

            _detected = detCount;

            if ((relCount > 0) && (nonRelCount > 0))
            {
                contentType = indexDomainContentEnum.bothRelevantAndNonRelevant;
            } else if (relCount > 0)
            {
                contentType = indexDomainContentEnum.onlyRelevant;
            } else if (nonRelCount > 0)
            {
                contentType = indexDomainContentEnum.onlyNonRelevant;
            } else
            {
                contentType = indexDomainContentEnum.unknown;
            }

            HashCode = md5.GetMd5Hash(domain);

        }


        [Category("IndexData")]
        [DisplayName("Content Type")]
        [Description("Remark on relevant vs irrelevant pages evaluated for the domain")]
        [imb(imbAttributeName.measure_letter, "-")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        public string contentTypeText { get; set; }


        private indexDomainContentEnum _contentType = indexDomainContentEnum.none;
        [XmlIgnore]

        [Category("IndexData")]
        [DisplayName("Content Type")]
        [Description("Remark on relevant vs irrelevant pages evaluated for the domain")]
        [imb(imbAttributeName.measure_letter, "-")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        public indexDomainContentEnum contentType
        {
            get {
                if (_contentType == indexDomainContentEnum.none)
                {
                    _contentType = (indexDomainContentEnum)typeof(indexDomainContentEnum).getEnumByName(contentTypeText, indexDomainContentEnum.none);
                }
                return _contentType;
            }
            set {
                contentTypeText = value.ToString();
                _contentType = value;
            }
        }


        [imb(imbAttributeName.collectionPrimaryKey)]
        [Category("Link")]
        [DisplayName("Domain")]
        [Description("Clean domain name")]
        public string domain { get; set; }

        private int _relevantPages = 0;
        [Category("IndexData")]
        [DisplayName("Relevant")]
        [Description("Number of indexed and crawled pages that found to be relevant")]
        [imb(imbAttributeName.measure_letter, "|R_det|")]
        [imb(imbAttributeName.measure_setUnit, "n of pages")]
        public int relevantPages
        {
            get { return _relevantPages; }
            set {
                if (_relevantPages < value)
                {
                    _relevantPages = value;
                }
                 }
        }

        private int _notRelevantPages = 0;

        [Category("IndexData")]
        [DisplayName("Not relevant")]
        [Description("Number of indexed pages that are not relevant")]
        [imb(imbAttributeName.measure_letter,"|R_not|")]
        [imb(imbAttributeName.measure_setUnit, "n of pages")]
        public int notRelevantPages
        {
            get { return _notRelevantPages; }
            set {
                if (_notRelevantPages < value)
                {
                    _notRelevantPages = value;
                }
                
            }
        }

        private int _detected = 0;
        [Category("IndexData")]
        [DisplayName("Detected")]
        [Description("Number of detected but never crawled pages")]
        [imb(imbAttributeName.measure_letter, "|R_det|")]
        [imb(imbAttributeName.measure_setUnit, "n of pages")]
        public int detected
        {
            get { return _detected; }
            set {
                if (_detected < value)
                {
                    _detected = value;
                }
            }
        }



        /// <summary> Ratio between all known pages and number of crawled ones </summary>
        [Category("Ratio")]
        [DisplayName("Crawled")]
        [imb(imbAttributeName.measure_letter, "C_w")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Ratio between all known pages and number of crawled ones")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Crawled { get; set; } = 0;


/// <summary> Ratio of relevant versus irrelevant pages, currently in the index datatable </summary>
[Category("Ratio")]
[DisplayName("Relevant Content")]
[imb(imbAttributeName.measure_letter, "p/n")]
[imb(imbAttributeName.measure_setUnit, "%")]
[Description("Ratio of relevant versus irrelevant pages, currently in the index datatable")]
[imb(imbAttributeName.reporting_valueformat, "F2")]
// [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_escapeoff)]
public double RelevantContentRatio { get; set; }=0;









        /// <summary> Number of distinct lemmas on relevant language, discovered on the domain </summary>
        [Category("Information Volume")]
        [DisplayName("Lemmas")]
        [imb(imbAttributeName.measure_letter, "|Lem_w|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of distinct lemmas on relevant language, discovered on the domain")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Lemmas { get; set; } = default(int);


        /// <summary> Number of distinct lemmas on relevant language, discovered on the domain </summary>
        [Category("Information Volume")]
        [DisplayName("Words")]
        [imb(imbAttributeName.measure_letter, "|W|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of distinct words on relevant language, discovered on the domain")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Words { get; set; } = default(int);

        /// <summary> Total Information Prize available on this web site </summary>
        [Category("Information Volume")]
        [DisplayName("Information Prize")]
        [imb(imbAttributeName.measure_letter, "∑|IP_w|")]
        [imb(imbAttributeName.measure_setUnit, "IP")]
        [Description("Total Information Prize available on this web site")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double InfoPrize { get; set; } = default(double);


        /// <summary>  </summary>
        [Category("Information Volume")]
        [DisplayName("DistinctLemmas")]
        [imb(imbAttributeName.measure_letter, "Lem[]")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("List of lemmas found to be unique for this web site in respect to the scope of currently selected index")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string DistinctLemmas { get; set; } = "";

        /// <summary> allwords found in the content </summary>
        [Category("Information Volume")]
        [DisplayName("AllWords")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string AllWords { get; set; } = ""; // allwords found in the content

        /// <summary> allwords found in the content </summary>
        [Category("Information Volume")]
        [DisplayName("AllLemmas")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string AllLemmas { get; set; } = ""; // allwords found in the content


        /// <summary> True - if the Index Engine has precompiled TF-IDF for this web site </summary>
        [Category("IndexData")]
        [DisplayName("TF-IDF compiled")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("True - if the Index Engine has precompiled TF-IDF for this web site")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public bool TFIDFcompiled { get; set; } = false;




        private string _HashCode = "";

        /// <summary> Hash code that pairs this domain to its TF-IDF table stored in the index  </summary>
        [Category("Information Volume")]
        [DisplayName("Hash code")]
        [imb(imbAttributeName.measure_letter, "TF-IDF#")]
        [imb(imbAttributeName.measure_setUnit, "#")]
        [Description("Hash code that pairs this domain to its TF-IDF table stored in the index ")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string HashCode {
            get
            {

                if (_HashCode.isNullOrEmpty()) _HashCode = md5.GetMd5Hash(domain);
                return _HashCode;
            }
            set
            {
                _HashCode = value;
            }
        }



        /// <summary> Language detected at landing page of the domain </summary>
        [Category("SideIndex")]
        [DisplayName("LandingLanguage")]
        [imb(imbAttributeName.measure_letter, "lP_lang")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Language detected at landing page of the domain. This property is filled by reportPlugIn_sideIndexer.")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string LandingLanguage { get; set; } = default(string);


        /// <summary> TRUE if the landing page found to be relevant </summary>
        [Category("SideIndex")]
        [DisplayName("LandingRelevant")]
        [imb(imbAttributeName.measure_letter, "lP_rel")]
        [Description("TRUE if the landing page found to be relevant. This property is filled by reportPlugIn_sideIndexer.")]
        public bool LandingRelevant { get; set; } = false;







    }

}