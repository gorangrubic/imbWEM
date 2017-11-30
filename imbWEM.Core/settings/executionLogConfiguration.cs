// --------------------------------------------------------------------------------------------------------------------
// <copyright file="executionLogConfiguration.cs" company="imbVeles" >
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

    public class executionLogConfiguration:imbBindable
    {

        /// <summary> If <c>true</c> it opens default external txt editor to show the first exception recorded in the session </summary>
        [Category("Exception")]
        [DisplayName("doOpenFirstException")]
        [Description("If <c>true</c> it opens default external txt editor to show the first exception recorded in the session")]
        public bool doOpenFirstException { get; set; } = false;



        /// <summary> If <c>true</c> it will throw exception at crawlerErrorLog creation </summary>
        [Category("Flag")]
        [DisplayName("doThrowDLCException")]
        [Description("If <c>true</c> it will throw exception at crawlerErrorLog creation")]
        public bool doThrowDLCException { get; set; } = true;





        public executionLogConfiguration() { }
        public void prepare()
        {

        }

        private int _crawlerDomainObligatoryReportAfterSeconds = 15; // = new Int32();
        /// <summary>
        /// After what number of seconds it will be forced to show report on all active tasks
        /// </summary>
        [Category("Analysis")]
        [DisplayName("crawlerDomainObligatoryReportAfterSeconds")]
        [Description("After what number of seconds it will be forced to show report on all active tasks")]
        public int crawlerDomainObligatoryReportAfterSeconds
        {
            get
            {
                return _crawlerDomainObligatoryReportAfterSeconds;
            }
            set
            {
                _crawlerDomainObligatoryReportAfterSeconds = value;
                OnPropertyChanged("crawlerDomainObligatoryReportAfterSeconds");
            }
        }



        #region ----------- Boolean [ doPreserveWebDocument ] -------  [Should crawled page object preserve HTML document object and source]
        private bool _doPreserveWebDocument = false;
        /// <summary>
        /// Should crawled page object preserve HTML document object and source
        /// </summary>
        [Category("Switches")]
        [DisplayName("doPreserveWebDocument")]
        [Description("Should crawled page object preserve HTML document object and source")]
        public bool doPreserveWebDocument
        {
            get { return _doPreserveWebDocument; }
            set { _doPreserveWebDocument = value; OnPropertyChanged("doPreserveWebDocument"); }
        }
        #endregion





        private bool _doKeepPageRecLog = false; // = new Boolean();
                                                   /// <summary>
                                                   /// True allows to pRecord to keep log content
                                                   /// </summary>
        [Category("Log")]
        [DisplayName("doKeepPageRecLog")]
        [Description("True allows to pRecord to keep log content")]
        public bool doKeepPageRecLog
        {
            get
            {
                return _doKeepPageRecLog;
            }
            set
            {
                _doKeepPageRecLog = value;
                OnPropertyChanged("doKeepPageRecLog");
            }
        }


        private bool _doKeepSiteRec = false; // = new Boolean();
                                                /// <summary>
                                                /// Allows to wRecord to keep log content
                                                /// </summary>
        [Category("Log")]
        [DisplayName("doKeepSiteRec")]
        [Description("Allows to wRecord to keep log content")]
        public bool doKeepSiteRec
        {
            get
            {
                return _doKeepSiteRec;
            }
            set
            {
                _doKeepSiteRec = value;
                OnPropertyChanged("doKeepSiteRec");
            }
        }


        private bool _doKeepSpiderRec = false; // = new Boolean();
                                                  /// <summary>
                                                  /// Allows to tRecord to keep log content
                                                  /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("doKeepSpiderRec")]
        [Description("Allows to tRecord to keep log content")]
        public bool doKeepSpiderRec
        {
            get
            {
                return _doKeepSpiderRec;
            }
            set
            {
                _doKeepSpiderRec = value;
                OnPropertyChanged("doKeepSpiderRec");
            }
        }




        private bool _doKeepTreeBuilderLog = false; // = new Boolean();
                                                       /// <summary>
                                                       /// Description of $property$
                                                       /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("doKeepTreeBuilderLog")]
        [Description("Description of $property$")]
        public bool doKeepTreeBuilderLog
        {
            get
            {
                return _doKeepTreeBuilderLog;
            }
            set
            {
                _doKeepTreeBuilderLog = value;
                OnPropertyChanged("doKeepTreeBuilderLog");
            }
        }



        #region ----------- Boolean [ doRemoveWRecordOnFinished ] -------  [Da li da skloni wRecord iz memorije ukoliko je zavrsen]
        private bool _doRemoveWRecordOnFinished = false;
        /// <summary>
        /// Da li da skloni wRecord iz memorije ukoliko je zavrsen
        /// </summary>
        [Category("Switches")]
        [DisplayName("doRemoveWRecordOnFinished")]
        [Description("Da li da skloni wRecord iz memorije ukoliko je zavrsen")]
        public bool doRemoveWRecordOnFinished
        {
            get { return _doRemoveWRecordOnFinished; }
            set { _doRemoveWRecordOnFinished = value; OnPropertyChanged("doRemoveWRecordOnFinished"); }
        }
        #endregion





        private bool _doAutoflushLogs = true; // = new Boolean();
                                                 /// <summary>
                                                 /// It will autoflush all log builder instances after reaching flush trashold
                                                 /// </summary>
        [Category("Log")]
        [DisplayName("doAutoflushLogs")]
        [Description("It will autoflush all log builder instances after reaching flush trashold")]
        public bool doAutoflushLogs
        {
            get
            {
                return _doAutoflushLogs;
            }
            set
            {
                _doAutoflushLogs = value;
                OnPropertyChanged("doAutoflushLogs");
            }
        }


        private int _logAutoflushLength = 50000; // = new Int32();
                                                   /// <summary>
                                                   /// Length trashold for log to be auto flushed
                                                   /// </summary>
        [Category("Log")]
        [DisplayName("logAutoflushLength")]
        [Description("Length trashold for log to be auto flushed")]
        public int logAutoflushLength
        {
            get
            {
                return _logAutoflushLength;
            }
            set
            {
                _logAutoflushLength = value;
                OnPropertyChanged("logAutoflushLength");
            }
        }

        #region ----------- Boolean [ doPageTokenizationLog ] -------  [Do log when a page is tokenized]
        private bool _doShowSemanticEngineLog
            = false;
        /// <summary>
        /// Do log when a page is tokenized
        /// </summary>
        [Category("Switches")]
        [DisplayName("doShowSemanticEngineLog")]
        [Description("Do log when a page is tokenized")]
        public bool doShowSemanticEngineLog
        {
            get { return _doShowSemanticEngineLog; }
            set { _doShowSemanticEngineLog = value; OnPropertyChanged("doShowSemanticEngineLog"); }
        }
        #endregion


        #region ----------- Boolean [ doPageLoadedLog ] -------  [Show message when page was newly loaded from the web]
        private bool _doPageLoadedLog = true;
        /// <summary>
        /// Show message when page was newly loaded from the web
        /// </summary>
        [Category("Switches")]
        [DisplayName("doPageLoadedLog")]
        [Description("Show message when page was newly loaded from the web")]
        public bool doPageLoadedLog
        {
            get { return _doPageLoadedLog; }
            set { _doPageLoadedLog = value; OnPropertyChanged("doPageLoadedLog"); }
        }
        #endregion



        #region ----------- Boolean [ doPageErrorOrDuplicateLog ] -------  [Show message when page load failed or a duplicated content was found]
        private bool _doPageErrorOrDuplicateLog = false;
        /// <summary>
        /// Show message when page load failed or a duplicated content was found
        /// </summary>
        [Category("Switches")]
        [DisplayName("doPageErrorOrDuplicateLog")]
        [Description("Show message when page load failed or a duplicated content was found")]
        public bool doPageErrorOrDuplicateLog
        {
            get { return _doPageErrorOrDuplicateLog; }
            set { _doPageErrorOrDuplicateLog = value; OnPropertyChanged("doPageErrorOrDuplicateLog"); }
        }
        #endregion


        #region ----------- Boolean [ doPageLoadedFromCache ] -------  [show message when a page is loaded from cache]
        private bool _doPageLoadedFromCache = false;
        /// <summary>
        /// show message when a page is loaded from cache
        /// </summary>
        [Category("Switches")]
        [DisplayName("doPageLoadedFromCache")]
        [Description("show message when a page is loaded from cache")]
        public bool doPageLoadedFromCache
        {
            get { return _doPageLoadedFromCache; }
            set { _doPageLoadedFromCache = value; OnPropertyChanged("doPageLoadedFromCache"); }
        }
        #endregion

    }

}