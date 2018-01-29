// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TFIDFConfiguration.cs" company="imbVeles" >
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

    public class TFIDFConfiguration
    {
        public void prepare()
        {

        }

        public TFIDFConfiguration() { }

        /// <summary> If true it will use <see cref="spider.targets.spiderDLContext.targets"/> TF-IDF as source</summary>
        [Category("Flag")]
        [DisplayName("doExploitStandardCC")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will")]
        public bool doExploitStandardCC { get; set; } = false;


        /// <summary> Auto builds the master TF_IDF from exploited standard CC </summary>
        [Category("Flag")]
        [DisplayName("doAutoConstructMasterTFfromStandardCC")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Auto builds the master TF_IDF from exploited standard CC")]
        public bool doAutoConstructMasterTFfromStandardCC { get; set; } = false;



        /// <summary> If <c>true</c> it will perform lexic contraction (packing all TermInstance into TermLemmas) for master TF build </summary>
        [Category("Flag")]
        [DisplayName("doPerformLexicContraction")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will perform lexic contraction (packing all TermInstance into TermLemmas) for master TF build")]
        public bool doPerformLexicContraction { get; set; } = true;



        /// <summary> If <c>true</c> it will use only pages that are evaluated as relevant </summary>
        [Category("Flag")]
        [DisplayName("doUseOnlyRelevantPages")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use only pages that are evaluated as relevant")]
        public bool doUseOnlyRelevantPages { get; set; } = true;


        /// <summary> If <c>true</c> it will use only single-dictionary matched words for MasterTF construction </summary>
        [Category("Flag")]
        [DisplayName("doUseOnlySingleMatch")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use only single-dictionary matched words for MasterTF construction")]
        public bool doUseOnlySingleMatch { get; set; } = false;



        /// <summary> If <c>true</c> it will use existing TF-IDF cached table </summary>
        [Category("Flag")]
        [DisplayName("doUseCachedDLCTables")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use existing TF-IDF cached table")]
        public bool doUseCachedDLCTables { get; set; } = false;


        /// <summary> If <c>true</c> it will save newly created TF-IDF tables to the cache </summary>
        [Category("Flag")]
        [DisplayName("doSaveCacheOfDLCTables")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will save newly created TF-IDF tables to the cache")]
        public bool doSaveCacheOfDLCTables { get; set; } = true;



        /// <summary> If <c>true</c> it will schedule also pages of domains that already have the DLC TF-IDF table in the cache. (for indexCrawler loading) </summary>
        [Category("Flag")]
        [DisplayName("doSchedulePagesWithDLCTable")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will schedule also pages of domains that already have the DLC TF-IDF table in the cache. (for indexCrawler loading)")]
        public bool doSchedulePagesWithDLCTable { get; set; } = false;


        /// <summary> If <c>true</c> it will save active master TF-IDF </summary>
        [Category("Flag")]
        [DisplayName("doSaveMasterTFIDFonEndOfCrawl")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will save active master TF-IDF")]
        public bool doSaveMasterTFIDFonEndOfCrawl { get; set; } = false;



        /// <summary> If <c>true</c> it will use alternative DLC TF-IDF construction that performes semantic contraction over total set of words and not on page TF-IDF way </summary>
        [Category("Flag")]
        [DisplayName("doUseHeuristicDLCTFIDFConstruction")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use alternative DLC TF-IDF construction that performes semantic contraction over total set of words and not on page TF-IDF way")]
        public bool doUseHeuristicDLCTFIDFConstruction { get; set; } = false;


        /// <summary> If <c>true</c> it will save wordlist for each processed page into index cache </summary>
        [Category("Flag")]
        [DisplayName("doSavePageWordlist")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will save wordlist for each processed page into index cache")]
        public bool doSavePageWordlist { get; set; } = false;


        /// <summary> If <c>true</c> it will save wordlist for each domain </summary>
        [Category("Flag")]
        [DisplayName("doSaveDomainWordList")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will save wordlist for each domain")]
        public bool doSaveDomainWordList { get; set; } = false;


        /// <summary> If <c>true</c> it will use saved wordlists from each page if its found, and prevent indexCrawler load of that page </summary>
        [Category("Flag")]
        [DisplayName("doUseSavedPageWordlists")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will use saved wordlists from each page if its found, and prevent indexCrawler load of that page")]
        public bool doUseSavedPageWordlists { get; set; } = false;



        /// <summary> If <c>true</c> it will create local copy of the master TF-IDF table for each DLC record </summary>
        [Category("Flag")]
        [DisplayName("doUseSeparateTFIDFperDLC_iterationEvaluation")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will create local copy of the master TF-IDF table for each DLC record")]
        public bool doUseSeparateTFIDFperDLC_iterationEvaluation { get; set; } = false;




    }



}
