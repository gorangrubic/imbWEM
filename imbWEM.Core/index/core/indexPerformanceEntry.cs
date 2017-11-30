// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexPerformanceEntry.cs" company="imbVeles" >
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
    using System;
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

    [imb(imbAttributeName.reporting_categoryOrder, "Session, Evaluated, Count")]
    public class indexPerformanceEntry:imbBindable
    {





        /// <summary>  </summary>
        
        [Category("Session")]
        [DisplayName("RecordID")]
        [imb(imbAttributeName.measure_letter, "ID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("UID of the record")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string RecordID { get; set; } = default(string);

        /// <summary> Start of the session </summary>
        [Category("Session")]
        [DisplayName("Start")]
        [imb(imbAttributeName.measure_letter, "S")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [imb(imbAttributeName.reporting_valueformat, "DD.MM.YYYY HH.MM.SS")]
        [Description("Start of the session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public DateTime Start { get; set; } = default(DateTime);


        /// <summary>
        /// What index version this session used: Global Index or separated (Local) experimental index
        /// </summary>
        [Category("Session")]
        [DisplayName("Index Repository")]
        [Description("What index version this session used: GlobalIndex or separated (Local) experimental index")]
        [imb(imbAttributeName.measure_optimizeUnit, true)]
        public string IndexRepository { get; set; } = GLOBAL_IndexRepository;

        public static string GLOBAL_IndexRepository = "GlobalIndex";


        /// <summary> Number of </summary>
        [Category("Count")]
        [DisplayName("Domains")]
        [imb(imbAttributeName.measure_letter, "|D|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Domains { get; set; } = default(int);


        /// <summary> Number of </summary>
        [Category("Count")]
        [DisplayName("Pages")]
        [imb(imbAttributeName.measure_letter, "|P_all|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Pages { get; set; } = default(int);



        /// <summary> Number of page records with evaluation set </summary>
        [Category("Evaluated")]
        [DisplayName("Pages Evaluated")]
        [imb(imbAttributeName.measure_letter, "|P_eval|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of page records with evaluation set")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int PagesEvaluated { get; set; } = default(int);


        [Category("Evaluated")]
        [DisplayName("Domains with TF-IDF")]
        [imb(imbAttributeName.measure_letter, "|W_tfidf|")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of page records with evaluation set")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int DomainTFIDFs { get; set; } = 0;


        /// <summary>  </summary>
        [Category("Session")]
        [DisplayName("CrawlID")]
        [imb(imbAttributeName.measure_letter, "CID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the crawl session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string CrawlID { get; set; } = default(string);


        /// <summary>  </summary>
        [Category("Session")]
        [DisplayName("SessionID")]
        [imb(imbAttributeName.measure_letter, "SID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SessionID { get; set; } = default(string);

        [Category("Settings")]
        [DisplayName("Global Hash")]
        [imb(imbAttributeName.measure_letter, "GS_md5")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("MD5 hash of the global application configuration - used as verification when comparing crawl reports")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string GlobalSetupHash { get; set; } = default(string);


   
        /// <summary> Crawler Hash </summary>
        /// 
        [Category("Settings")]
        [DisplayName("Crawler Hash")]
        [imb(imbAttributeName.measure_letter, "CS_md5")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("MD5 hash of the crawler configuration - used for verification of the same experiment setup")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string CrawlerHash { get; set; } = default(string);



        /// <summary> Number of page index assertations that found no data in the index database </summary>
        [Category("Count")]
        [DisplayName("NegativeCalls")]
        [imb(imbAttributeName.measure_letter, "Q_n")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of page index assertations that found no data in the index database")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int NegativeCalls { get; set; } = default(int);


        /// <summary> Number of responded page index queries for URLs </summary>
        [Category("Count")]
        [DisplayName("PositiveCalls")]
        [imb(imbAttributeName.measure_letter, "Q_p")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of responded page index queries for URLs")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int PositiveCalls { get; set; } = default(int);



        /// <summary> Session duration in minutes </summary>
        [Category("Session")]
        [DisplayName("Duration")]
        [imb(imbAttributeName.measure_letter, "T_s")]
        [imb(imbAttributeName.measure_setUnit, "min")]
        [imb(imbAttributeName.reporting_valueformat, "#.##")]
        [Description("Session duration in minutes")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Duration { get; set; } = default(double);



        /// <summary> Certainity of potential precission assessment - i.e. proportion of evaluated page content compared to unevaluated </summary>
        [Category("Quality")]
        [DisplayName("Certainity of PP")]
        [imb(imbAttributeName.measure_letter, "Pot.Prec.")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Certainity of potential precission assessment - i.e. proportion of evaluated page content compared to unevaluated")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double CertainityPP { get; set; } = 0;



        /// <summary> Proportion between domains with constructed TF-IDF cache in all domains </summary>
        [Category("Quality")]
        [DisplayName("MasterTFIDF Coverage")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Proportion between domains with constructed TF-IDF cache in all domains")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double MasterTFIDFCoverage { get; set; } = 0;



       








    }

}