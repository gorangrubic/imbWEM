// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoaderConfiguration.cs" company="imbVeles" >
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
namespace imbWEM.Core.settings
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.web.core;
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

    public class LoaderConfiguration:imbBindable
    {
        public LoaderConfiguration()
        {
            
        }

        public void prepare()
        {
            webclientSettings.timeout = httpRequestTimeoutMs;
            webclientSettings.doUseCache = true;
            
        }


        public webLoaderSettings webclientSettings { get; set; } = new webLoaderSettings();


        /// <summary>
        /// checks if page already existed before any links accepted
        /// </summary>
        [Category("Analysis")]
        [DisplayName("doPageContentHashCheck")]
        [Description("checks if page already existed before any links accepted")]
        public bool doPageContentHashCheck { get; set; } = true;


        /// <summary>
        /// Miliseconds allowed for one http request
        /// </summary>
        [Category("AHttp Loader")]
        [DisplayName("httpRequestTimeoutMs")]
        [Description("Miliseconds allowed for one http request")]
        public int httpRequestTimeoutMs { get; set; } = 5000;


        /// <summary>
                                                           /// Description of $property$
                                                           /// </summary>
        [Category("Cache")]
        [DisplayName("cacheInMemoryContentTree")]
        [Description("Cache content tree")]
        public bool cacheInMemoryContentTree { get; set; } = false;


        private bool _cacheInMemoryCrawledContext = false; // = new Boolean();
                                                              /// <summary>
                                                              /// Cache crawler meta info
                                                              /// </summary>
        [Category("Cache")]
        [DisplayName("cacheInMemoryCrawledContext")]
        [Description("Cache crawler information, site structure, page content")]
        public bool cacheInMemoryCrawledContext
        {
            get
            {
                return _cacheInMemoryCrawledContext;
            }
            set
            {
                _cacheInMemoryCrawledContext = value;
                OnPropertyChanged("cacheInMemoryCrawledContext");
            }
        }

        private bool _cacheInMemoryTokenizedContent = false; // = new Boolean();
                                                                /// <summary>
                                                                /// NLP and TreeCompression results
                                                                /// </summary>
        [Category("Cache")]
        [DisplayName("cacheInMemoryTokenizedContent")]
        [Description("NLP and TreeCompression results")]
        public bool cacheInMemoryTokenizedContent
        {
            get
            {
                return _cacheInMemoryTokenizedContent;
            }
            set
            {
                _cacheInMemoryTokenizedContent = value;
                OnPropertyChanged("cacheInMemoryTokenizedContent");
            }
        }






        private bool _cacheDoUseExternalPath = true; // = new Boolean();
                                                        /// <summary>
                                                        /// When true it uses external Cache path for data cache
                                                        /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("cacheDoUseExternalPath")]
        [Description("When true it uses external Cache path for data cache")]
        public bool cacheDoUseExternalPath
        {
            get
            {
                return _cacheDoUseExternalPath;
            }
            set
            {
                _cacheDoUseExternalPath = value;
                OnPropertyChanged("cacheDoUseExternalPath");
            }
        }


        private string _cacheExternalPathForCache = @"g:\imbVelesTestground\semanticTerminal\datacache\"; // = new String();
        /// <summary>
        /// Path to use for external cache
        /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("cacheExternalPathForCache")]
        [Description("Path to use for external cache")]
        public string cacheExternalPathForCache
        {
            get
            {
                return _cacheExternalPathForCache;
            }
            set
            {
                _cacheExternalPathForCache = value;
                OnPropertyChanged("cacheExternalPathForCache");
            }
        }



    }

}