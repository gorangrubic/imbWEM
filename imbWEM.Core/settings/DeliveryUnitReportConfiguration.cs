// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeliveryUnitReportConfiguration.cs" company="imbVeles" >
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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class DeliveryUnitReportConfiguration:imbBindable
    {
        public DeliveryUnitReportConfiguration() {  }

        private int _tableExcerptRowLimitDefault = 10; // = new Int32();
        /// <summary>
        /// Default number of rows to include into visible report content
        /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("tableExcerptRowLimitDefault")]
        [Description("Default number of rows to include into visible report content")]
        public int tableExcerptRowLimitDefault
        {
            get
            {
                return _tableExcerptRowLimitDefault;
            }
            set
            {
                _tableExcerptRowLimitDefault = value;
                OnPropertyChanged("tableExcerptRowLimitDefault");
            }
        }


        private bool _reportBuildWebPageReport = false; // = new Boolean();
        /// <summary>
        /// TRUE enables generation of web page reports
        /// </summary>
        [Category("Report")]
        [DisplayName("Build Page Report")]
        [Description("Allows generation of web page reports")]
        public bool reportBuildWebPageReport
        {
            get
            {
                return _reportBuildWebPageReport;
            }
            set
            {
                _reportBuildWebPageReport = value;
                OnPropertyChanged("reportBuildWebPageReport");
            }
        }

        internal void prepare()
        {
            
        }


        #region ----------- Boolean [ reportBuildWebDomainReport ] -------  [Should web domain report be included]
        private bool _reportBuildWebDomainReport = false;
        /// <summary>
        /// Should web domain report be included
        /// </summary>
        [Category("Switches")]
        [DisplayName("reportBuildWebDomainReport")]
        [Description("Should web domain report be included")]
        public bool reportBuildWebDomainReport
        {
            get { return _reportBuildWebDomainReport; }
            set { _reportBuildWebDomainReport = value; OnPropertyChanged("reportBuildWebDomainReport"); }
        }
        #endregion



        #region ----------- Boolean [ reportBuildDoGraphs ] -------  [Should graphs be created in the reports]
        private bool _reportBuildDoGraphs = false;
        /// <summary>
        /// Should graphs be created in the reports
        /// </summary>
        [Category("Switches")]
        [DisplayName("reportBuildDoGraphs")]
        [Description("Should graphs be created in the reports")]
        public bool reportBuildDoGraphs
        {
            get { return _reportBuildDoGraphs; }
            set { _reportBuildDoGraphs = value; OnPropertyChanged("reportBuildDoGraphs"); }
        }
        #endregion



        #region ----------- Boolean [ reportBuildDoExcelExport ] -------  [Should Excel files be created as attachment]
        private bool _reportBuildDoExcelExport = false;
        /// <summary>
        /// Should Excel files be created as attachment
        /// </summary>
        [Category("Switches")]
        [DisplayName("reportBuildDoExcelExport")]
        [Description("Should Excel files be created as attachment")]
        public bool reportBuildDoExcelExport
        {
            get { return _reportBuildDoExcelExport; }
            set { _reportBuildDoExcelExport = value; OnPropertyChanged("reportBuildDoExcelExport"); }
        }
        #endregion


        #region ----------- Boolean [ reportBuildDoJSONExport ] -------  [Should JSON reports be created for attachment]
        private bool _reportBuildDoJSONExport = false;
        /// <summary>
        /// Should JSON reports be created for attachment
        /// </summary>
        [Category("Switches")]
        [DisplayName("reportBuildDoJSONExport")]
        [Description("Should JSON reports be created for attachment")]
        public bool reportBuildDoJSONExport
        {
            get { return _reportBuildDoJSONExport; }
            set { _reportBuildDoJSONExport = value; OnPropertyChanged("reportBuildDoJSONExport"); }
        }
        #endregion




        private bool _reportPageIndex = true; // = new Boolean();
        /// <summary>
        /// Includes page index table into report
        /// </summary>
        [Category("Report")]
        [DisplayName("Global Page Index")]
        [Description("Includes page index table into report")]
        public bool reportPageIndex
        {
            get
            {
                return _reportPageIndex;
            }
            set
            {
                _reportPageIndex = value;
                OnPropertyChanged("reportPageIndex");
            }
        }



        private bool _reportPurgeFolder = true; // = new Boolean();
                                                   /// <summary>
                                                   /// Description of $property$
                                                   /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("reportPurgeFolder")]
        [Description("Description of $property$")]
        public bool reportPurgeFolder
        {
            get
            {
                return _reportPurgeFolder;
            }
            set
            {
                _reportPurgeFolder = value;
                OnPropertyChanged("reportPurgeFolder");
            }
        }





        private bool _reportLinkIndex = true; // = new Boolean();
        /// <summary>
        /// Includes link index table into report
        /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("Link Index")]
        [Description("Includes link index table into report")]
        public bool reportLinkIndex
        {
            get
            {
                return _reportLinkIndex;
            }
            set
            {
                _reportLinkIndex = value;
                OnPropertyChanged("reportLinkIndex");
            }
        }
    }

}