// --------------------------------------------------------------------------------------------------------------------
// <copyright file="directReportConfiguration.cs" company="imbVeles" >
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class directReportConfiguration:imbBindable
    {
        public directReportConfiguration() { }

        public void prepare()
        {
            DataTableForStatisticsExtension.tableReportCreation_useShortNames = tableReporting_UseShortNames;


            //if (StyleHeadingCategory == null)
            //{
            //    StyleHeadingCategory = new ExcelParagraph()
            //}
        }


        //public ExcelNamedStyleXml StyleHeadingCategory { get; set; }
        //public ExcelNamedStyleXml StyleHeadingLabel { get; set; }
        //public ExcelNamedStyleXml StyleHeadingLetter { get; set; }
        //public ExcelNamedStyleXml StyleDataRowEven { get; set; }
        //public ExcelNamedStyleXml StyleDataRowOdd { get; set; }

        public bool DR_ReportIterationTerms { get; set; } = false;
        public bool DR_ReportIterationUrls { get; set; } = false;
        public bool DR_ReportDomainTerms { get; set; } = false;
        public bool DR_ReportDomainPages { get; set; } = false;
        public bool DR_ReportWRecordLog { get; set; } = true;
        public bool DR_ReportTimeline { get; set; } = true;

        public bool DR_ReportModules { get; set; } = true;



        /// <summary> General switch for special term and TF-IDF reporting </summary>
        [Category("Flag")]
        [DisplayName("doTF_IDF_crawlReports")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("General switch for special term and TF-IDF reporting")]
        public bool doTF_IDF_crawlReports { get; set; } = false;




        /// <summary> General switch for all iteration based reporting (except module reporting) </summary>
        [Category("Flag")]
        [DisplayName("doIterationReport")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("General switch for all iteration based reporting (except module reporting)")]
        public bool doIterationReport { get; set; } = false;


        /// <summary> General switch for all domain-level based reporting (except as aggregate source for module reporting) </summary>
        [Category("Flag")]
        [DisplayName("doDomainReport")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("General switch for all domain-level based reporting (except as aggregate source for module reporting)")]
        public bool doDomainReport { get; set; } = false;



        /// <summary> Export module iteration record XML objects (if module reporting is enabled)  </summary>
        [Category("Modules")]
        [DisplayName("DR_ReportModules_XMLIteration")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Export module iteration record XML objects (if module reporting is enabled) ")]
        public bool DR_ReportModules_XMLIteration { get; set; } = false;


        /// <summary> Export module reports for each domain - (if module reporting is enabled) </summary>
        [Category("Modules")]
        [DisplayName("DR_ReportModules_DomainReports")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Export module reports for each domain - (if module reporting is enabled)")]
        public bool DR_ReportModules_DomainReports { get; set; } = false;




        /// <summary> Autorename crawler folder if already exists </summary>
        [Category("ReportOutput")]
        [DisplayName("doAutoRenameCrawlerFolder")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Autorename crawler folder if already exists")]
        public bool doAutoRenameCrawlerFolder { get; set; } = true;


        /// <summary> autorename session folder if already exists </summary>
        [Category("Flag")]
        [DisplayName("doAutoRenameSessionFolder")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("autorename session folder if already exists")]
        public bool doAutoRenameSessionFolder { get; set; } = true;



        /// <summary> If it should create performanceRecord at the end of crawl job </summary>
        [Category("Flag")]
        [DisplayName("doPublishPerformance")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If it should create performanceRecord at the end of crawl job")]
        public bool doPublishPerformance { get; set; } = true;



        /// <summary> If <c>true</c> it will publich experimentSessionTable into report directory </summary>
        [Category("Flag")]
        [DisplayName("doPublishExperimentSessionTable")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will publish experimentSessionTable into report directory")]
        public bool doPublishExperimentSessionTable { get; set; } = true;


        /// <summary> If <c>true</c> it will publish index performance table into report directory </summary>
        [Category("Flag")]
        [DisplayName("Publish index performance table")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will publish index performance table into report directory")]
        public bool doPublishIndexPerformanceTable { get; set; } = true;







        /// <summary> If <c>true</c> it will use short version of table name for filename </summary>
        [Category("Flag")]
        [DisplayName("tableReporting_UseShortNames")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use short version of table name for filename")]
        public bool tableReporting_UseShortNames { get; set; } = true;




        public bool REPORT_DirectoryNameByJobName { get; set; } = true;
    }

}