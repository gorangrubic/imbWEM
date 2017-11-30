// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderLink.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.targets
{
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
    using imbCommonModels.structure;
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
    using imbSCI.Core.extensions.data;
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
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Link vector - pointing from one page to another
    /// </summary>
    /// <seealso cref="spiderWebElementBase" />
    /// <seealso cref="IObjectWithNameAndDescription" />
    public class spiderLink:spiderWebElementBase, ISpiderTargetElement, ISpiderLink
    {
        /// <summary>
        /// Number of detections on single page
        /// </summary>
        public int countOnTheDomain { get; set; } = 1;


        /// <summary>
        /// Number of detections on single page
        /// </summary>
        public int countOnThePage { get; set; } = 1;


        /// <summary> </summary>
        public int linkAge { get; set; }


        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        private string _description = "";
        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description
        {
            get {
                if (_description.isNullOrEmpty())
                {
                    _description = "Link found on [" + originPage.url + "] pointing to [" + link.url + "]";

                }
                return _description;
            }
            set { _description = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public string domain { get; set; }


        //public spiderLink(String url)
        //{
        //    link = new link(url);
        //    Uri tmp = new Uri(url);
        //    domain = tmp.Host;
        //    url = link.url.ToString();
        //    name = link.caption;
        //    description = "";
        //  //  originPage = __home;
        //   // iteration = __iteracija;

        //}



        public spiderLink(spiderPage __home, link __link, int __iteracija)
        {

            if (__home == null) throw new aceGeneralException("Page of origin for this link never provided", null, this, "Bad arguments at constructor");
            link = __link;
            url = link.url.ToString();
            originPage = __home;
            iterationDiscovery = __iteracija;
            name = link.caption;
            domain = link.domain;
            captions.Add(link.caption);
            urls.AddInstance(url, "Link urls @ spiderLink");
            

            //description = "";
            //Uri tmp = new Uri(url);
            //domain = tmp.Host;
            //if (url.isNullOrEmpty())
            //{
            //    throw new aceGeneralException("url is null");
            //}
        }


        //protected List<String> _captionTokens = new List<string>();

        //public List<String> captionTokens
        //{
        //    get {

        //        return captions.getTokens(true, true, true);
        //    }
           
        //}


       
        ///// <summary>
        ///// Elementi URL-a (odvojeno parametar i vrednost)
        ///// </summary>
        //public List<String> urlTokens
        //{
        //    get {
        //        return urls.getTokens(true, true, true);
        //    }
            
        //}



        public override string originHash
        {
            get
            {   if (originPage == null) return "--- root link ---";
                return originPage.originHash;
            }
        }
    

        private string _targetHash = "";
        /// <summary> </summary>
        public string targetHash
        {
            get
            {
                if (targetedPage == null) return getHash(link.url);
                return targetedPage.originHash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public link link { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public spiderPage originPage { get; set; }


        /// <summary>
        /// Object is set after <see cref="link"/> was loaded
        /// </summary>
        public spiderPage targetedPage { get; set; }


        //private Boolean _isFailed; // = "";
        //                            /// <summary>
        //                            /// Bindable property
        //                            /// </summary>
        //public Boolean isFailed
        //{
        //    get { return _isFailed; }
        //    set { _isFailed = value; }
        //}


        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        //public Boolean isLoaded
        //{
        //    get { return (pageLinksTo != null); }
        //}
        //private Int32 _iteration = -1;
        ///// <summary>
        ///// 
        ///// </summary>
        //public Int32 iteration
        //{
        //    get { return _iteration; }
        //    protected set { _iteration = value; }
        //}


        ///// <summary>
        ///// Sets the iteration.
        ///// </summary>
        ///// <param name="__iteration">The iteration.</param>
        //public void setIteration(Int32 __iteration)
        //{
        //    if (iteration == -1)
        //    {
        //        iteration = __iteration;
        //    }
        //}

        //private spiderPage _pageLinksTo;
        ///// <summary>
        ///// 
        ///// </summary>
        //public spiderPage pageLinksTo
        //{
        //    get { return _pageLinksTo; }
        //    set { _pageLinksTo = value; }
        //}


        //private spiderLinkFlags _flag = spiderLinkFlags.scheduled;
        ///// <summary>
        ///// Flags describing link
        ///// </summary>
        //public spiderLinkFlags flag
        //{
        //    get {

        //        _flag = spiderLinkFlags.scheduled;

        //        if (isLoaded) return spiderLinkFlags.loaded;

        //        if (isFailed) return spiderLinkFlags.failed;

        //        if (isDisabledForLoad) return spiderLinkFlags.skipped;

        //        return _flag;

        //    }
            
        //}


    }
}
