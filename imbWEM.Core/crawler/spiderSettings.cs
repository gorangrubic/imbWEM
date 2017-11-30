// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderSettings.cs" company="imbVeles" >
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.enums;
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class spiderSettings : imbBindable
    {

       // public crawlerSettings legacySettings { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public spiderEvaluatorExecutionFlags flags { get; set; } = spiderEvaluatorExecutionFlags.doTrimPrimaryOutput;


        #region ----------- Boolean [ FRONTIER_doLinkHarvest ] -------  [true - enables link harvest from the loaded page, recommanded value is true]
        private bool _FRONTIER_doLinkHarvest = true;
        /// <summary>
        /// true - enables link harvest from the loaded page, recommanded value is true
        /// </summary>
        [Category("Switches")]
        [DisplayName("FRONTIER_doLinkHarvest")]
        [Description("true - enables link harvest from the loaded page, recommanded value is true")]
        public bool FRONTIER_doLinkHarvest
        {
            get { return _FRONTIER_doLinkHarvest; }
            set { _FRONTIER_doLinkHarvest = value; OnPropertyChanged("FRONTIER_doLinkHarvest"); }
        }
        #endregion


        #region ----------- Boolean [ FRONTIER_doLinkResolver ] -------  [true - enables link resolving after the harvest -- resolver will unify links that point to the same target]
        private bool _FRONTIER_doLinkResolver = true;
        /// <summary>
        /// true - enables link resolving after the harvest -- resolver will unify links that point to the same target
        /// </summary>
        [Category("Switches")]
        [DisplayName("FRONTIER_doLinkResolver")]
        [Description("true - enables link resolving after the harvest -- resolver will unify links that point to the same target")]
        public bool FRONTIER_doLinkResolver
        {
            get { return _FRONTIER_doLinkResolver; }
            set { _FRONTIER_doLinkResolver = value; OnPropertyChanged("FRONTIER_doLinkResolver"); }
        }
        #endregion



        private linkNature _FRONTIER_harvestNature = linkNature.navigation; // = new linkNature();
                                                                            /// <summary>
                                                                            /// Flags enabling harvest links of specified <see cref="linkNature"/>
                                                                            /// </summary>
        [Category("Switches")]
        [DisplayName("FRONTIER_harvestNature")]
        [Description("Flags enabling harvest links of specified linkNature")]
        public linkNature FRONTIER_harvestNature
        {
            get
            {
                return _FRONTIER_harvestNature;
            }
            set
            {
                _FRONTIER_harvestNature = value;
                OnPropertyChanged("FRONTIER_harvestNature");
            }
        }


        private linkScope _FRONTIER_harvestScope = linkScope.inner;// default(linkScope); // = new linkScope();
                                                                   /// <summary>
                                                                   /// Flags enabling link harvest for specified linkScope
                                                                   /// </summary>
        [Category("Switches")]
        [DisplayName("FRONTIER_harvestScope")]
        [Description("Flags enabling link harvest for specified linkScope")]
        public linkScope FRONTIER_harvestScope
        {
            get
            {
                return _FRONTIER_harvestScope;
            }
            set
            {
                _FRONTIER_harvestScope = value;
                OnPropertyChanged("FRONTIER_harvestScope");
            }
        }



        private spiderPullDecayModes _FRONTIER_PullDecayModes = default(spiderPullDecayModes); // = new spiderPullDecayModes();
                                                      /// <summary>
                                                      /// Flags that turn on one of Target auto removal modes
                                                      /// </summary>
        [Category("spiderSettings")]
        [DisplayName("FRONTIER_PullDecayModes")]
        [Description("Flags that turn on one of Target auto removal modes")]
        public spiderPullDecayModes FRONTIER_PullDecayModes
        {
            get
            {
                return _FRONTIER_PullDecayModes;
            }
            set
            {
                _FRONTIER_PullDecayModes = value;
                OnPropertyChanged("FRONTIER_PullDecayModes");
            }
        }


        private spiderPullLoadTakeMode _FRONTIER_PullTakeMode = spiderPullLoadTakeMode.onlyLTSpecified; // default(spiderPullLoadTakeMode); // = new spiderPullLoadTakeMode();
                                                      /// <summary>
                                                      /// How it makes the final selection of the targets
                                                      /// </summary>
        [Category("spiderSettings")]
        [DisplayName("FRONTIER_PullTakeMode")]
        [Description("How it makes the final selection of the targets")]
        public spiderPullLoadTakeMode FRONTIER_PullTakeMode
        {
            get
            {
                return _FRONTIER_PullTakeMode;
            }
            set
            {
                _FRONTIER_PullTakeMode = value;
                OnPropertyChanged("FRONTIER_PullTakeMode");
            }
        }



        /// <summary> SEMANTIC_DOCUMENT_CONTRACTION </summary>
        [Category("Semantics")]
        [DisplayName("Document contraction")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("SEMANTIC_DOCUMENT_CONTRACTION")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int TermExpansionForContent { get; set; } = 0;



        /// <summary> autoconfigurable :: switching semantic record tracking (TF-IDF) if required by a module </summary>
        /// 
        [Category("AutoSwitch")]
        [DisplayName("doEnableDLC_TFIDF")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("autoconfigurable :: switching semantic record tracking (TF-IDF) if required by a module")]
        public bool doEnableDLC_TFIDF { get; set; } = false;



        /// <summary> autoconfigurable :: switching page block decomposing on DLC level </summary>
        [Category("AutoSwitch")]
        [DisplayName("doEnableDLC_BlockTree")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("autoconfigurable :: switching page block decomposing on DLC level")]
        public bool doEnableDLC_BlockTree { get; set; } = false;



        /// <summary> shoulw crawler perform crosslink detection on each iteration </summary>
        [Category("Flag")]
        [DisplayName("doEnableCrossLinkDetection")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("shoulw crawler perform crosslink detection on each iteration")]
        public bool doEnableCrossLinkDetection { get; set; } = false;



        /// <summary> Customization comments </summary>
        [Category("Label")]
        [DisplayName("Comment")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Customization comments")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string Comment { get; set; } = "";


        /// <summary> Signature sufix used for experiment crawler ID </summary>
        [Category("Label")]
        [DisplayName("SignatureSufix")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("Signature sufix used for experiment crawler ID")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SignatureSufix { get; set; } = "";






        public spiderSettings()
        {



        }





        private int _limitIterations = 10; //= default(Int32); // = new Int32();
        /// <summary>
        /// Maximum allowed spider iterations per web site profile
        /// </summary>
        [Category("spiderSettings")]
        [DisplayName("I_max")]
        [Description("Maximum allowed spider iterations")]
        public int limitIterations
        {
            get
            {
                return _limitIterations;
            }
            set
            {
                _limitIterations = value;
                OnPropertyChanged("limitIterations");
            }
        }


        private int _limitTotalLinks = 1000;//= default(Int32); // = new Int32();
                                                      /// <summary>
                                                      /// Total maximum of links to be loaded
                                                      /// </summary>
        [Category("spiderSettings")]
        [DisplayName("TT_max")]
        [Description("Total maximum of detected")]
        public int limitTotalLinks
        {
            get
            {
                return _limitTotalLinks;
            }
            set
            {
                _limitTotalLinks = value;
                OnPropertyChanged("limitTotalLinks");
            }
        }


        private int _limitIterationNewLinks = 5; //= default(Int32); // = new Int32();
                                                      /// <summary>
                                                      /// LoadTake - LT
                                                      /// </summary>
        [Category("spiderSettings")]
        [DisplayName("LT")]
        [Description("Load Take - how much should the crawler load in next iteration")]
        public int limitIterationNewLinks
        {
            get
            {
                return _limitIterationNewLinks;
            }
            set
            {
                _limitIterationNewLinks = value;
                OnPropertyChanged("limitIterationNewLinks");
            }
        }



        private int _limitTotalPageLoad = 30; // = new Int32();
                                                      /// <summary>
                                                      /// Limits number of pages allowed to be loaded by spider
                                                      /// </summary>
        [Category("spiderSettings")]
        [DisplayName("PL_max")]
        [Description("Limits number of pages allowed to be loaded by spider")]
        public int limitTotalPageLoad
        {
            get
            {
                return _limitTotalPageLoad;
            }
            set
            {
                _limitTotalPageLoad = value;
                OnPropertyChanged("limitTotalPageLoad");
            }
        }



        private int _primaryPageSetSize = 10;
            
        /// <summary>
        /// Target size of primary page set 
        /// </summary>
        [Category("spiderSettings")]
        [DisplayName("RP_c")]
        [Description("Target size of primary page set")]
        public int primaryPageSetSize
        {
            get
            {
                return _primaryPageSetSize;
            }
            set
            {
                _primaryPageSetSize = value;
                OnPropertyChanged("primaryPageSetSize");
            }
        }


    }

}