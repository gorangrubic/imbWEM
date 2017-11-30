// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexEngineConfiguration.cs" company="imbVeles" >
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

    public class IndexEngineConfiguration:imbBindable
    {
        public IndexEngineConfiguration()
        {

        }

        public void prepare()
        {

        }


        /// <summary> If <c>true</c> it will perform index backup and publishing on start-up </summary>
        [Category("Flag")]
        [DisplayName("doIndexPublishAndBackupOnOpenSession")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will perform index backup and publishing on start-up")]
        public bool doIndexPublishAndBackupOnOpenSession { get; set; } = true;



        /// <summary> Forces index to make a backup on each save </summary>
        [Category("Flag")]
        [DisplayName("doIndexBackupOnEachSave")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Forces index to make a backup on each save")]
        public bool doIndexBackupOnEachSave { get; set; } = false;




        /// <summary> Index is updated at end of each DLC thread </summary>
        [Category("Flag")]
        [DisplayName("doIndexUpdateOnDLC")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("Index is updated at end of each DLC thread")]
        public bool doIndexUpdateOnDLC { get; set; } = true;



        /// <summary> Number of DLCs sent for index update after which the index should do autosave of its datatables </summary>
        [Category("Count")]
        [DisplayName("doIndexAutoSaveOnDLCs")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of DLCs sent for index update after which the index should do autosave of its datatables")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int doIndexAutoSaveOnDLCs { get; set; } = 10;



        public bool doIndexLoadMainIndex { get; set; } = true;
        public bool doIndexFullTrustMode { get; set; } = true;


        /// <summary>
        /// Number of links to traverse in one iteration by BreadthNFirst, if not overriden by the crawler settings
        /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("crawlerBreadthNFirstTake")]
        [Description("Number of links to traverse in one iteration by BreadthNFirst, if not overriden by the crawler settings")]
        public int crawlerBreadthNFirstTake { get; set; } = 10;


        /// <summary>
        /// Iteration limit when doing full scan
        /// </summary>
        [Category("Analysis")]
        [DisplayName("stageIterationLimitFullScan")]
        [Description("Iteration limit when doing full scan")]
        public int stageIterationLimitFullScan { get; set; } = 1000;


        /// <summary> If <c>true</c> it will keep the index database in read only mode </summary>
        [Category("Flag")]
        [DisplayName("doRunIndexInReadOnlyMode")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will keep the index database in read only mode")]
        public bool doRunIndexInReadOnlyMode { get; set; } = true;



        /// <summary> If <c>true</c> it will save into index all url's that were queried for relevancy/stats but were not found in the index </summary>
        [Category("Flag")]
        [DisplayName("doSaveFailedURLQueries")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will save into index all url's that were queried for relevancy/stats but were not found in the index")]
        public bool doSaveFailedURLQueries { get; set; } = true;



        /// <summary> If <c>true</c>, on index update call it will save records on urls detected during the last crawl </summary>
        [Category("Flag")]
        [DisplayName("doSaveDetectedURLs")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c>, on index update call it will save records on urls detected during the last crawl")]
        public bool doSaveDetectedURLs { get; set; } = true;





        /// <summary> If <c>true</c> it will skip indexPage entries that have above 0 IP and LM  </summary>
        [Category("Flag")]
        [DisplayName("plugIn_indexDBUpdater_optimizedMode")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will skip indexPage entries that have above 0 IP and LM ")]
        public bool plugIn_indexDBUpdater_optimizedMode { get; set; } = true;


        /// <summary> If <c>true</c> it will create separate instance of Master TF-IDF for each DLC - it will consume more memory but increase performance </summary>
        [Category("Flag")]
        [DisplayName("plugIn_indexDBUpdater_TFIDF_per_DLC")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will create separate instance of Master TF-IDF for each DLC - it will consume more memory but increase performance")]
        public bool plugIn_indexDBUpdater_TFIDF_per_DLC { get; set; } = true;


        /// <summary> If <c>true</c> it will update domain aggregate information on IP, lemmas and etc. </summary>
        [Category("Flag")]
        [DisplayName("plugIn_indexDBUpdater_updateDomainEntry")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will update domain aggregate information on IP, lemmas and etc.")]
        public bool plugIn_indexDBUpdater_updateDomainEntry { get; set; } = false;




    }

}