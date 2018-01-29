// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportSystemConfiguration.cs" company="imbVeles" >
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
    using imbWEM.Core.plugins;
    using imbWEM.Core.stage;

    public class SupportSystemConfiguration:imbBindable
    {
        public SupportSystemConfiguration() { }



        /// <summary> plugin registry </summary>
        
        [XmlIgnore]
        public Dictionary<string, Type> plugins { get; set; } = new Dictionary<string, Type>();



        public void prepare()
        {

            List<Type> types = new List<Type>(); // imbCore.resources.typology.imbTypologyDiscovery.getTypesForNamespaceAndChildspace("spider.plugins");

            types.Add(typeof(indexMasterTFIDFConstructor));
            types.Add(typeof(indexDBUpdater));
            types.Add(typeof(controlPlugIn_TCAutomatic));
            types.Add(typeof(reportPlugIn_benchmark));
            types.Add(typeof(reportPlugIn_workload));
            types.Add(typeof(reportPlugIn_sideIndexer));
            types.Add(typeof(reportPlugIn_CrawlToMC));

            foreach (Type t in types)
            {
                if (t.IsClass)
                {
                    if (!t.IsAbstract)
                    {
                        plugins.Add(t.Name.ToLower(), t);
                    }
                }
            }
            aceLog.log(":: plugins detected: " + plugins.Count(), null, true);
        }





        private bool _doLanguagePrepareCall = false; // = new Boolean();
                                                        /// <summary>
                                                        /// If true it will load all language definitions on startup
                                                        /// </summary>
        [Category("Boot")]
        [DisplayName("doLanguagePrepareCall")]
        [Description("If true it will load all language definitions on startup")]
        public bool doLanguagePrepareCall
        {
            get
            {
                return _doLanguagePrepareCall;
            }
            set
            {
                _doLanguagePrepareCall = value;
                OnPropertyChanged("doLanguagePrepareCall");
            }
        }




        private bool _doAskForSetupEdit = false; // = new Boolean();
                                                    /// <summary>
                                                    /// If true it will ask for settings editing before auto-execution
                                                    /// </summary>
        [Category("Boot")]
        [DisplayName("doAskForSetupEdit")]
        [Description("If true it will ask for settings editing before auto-execution")]
        public bool doAskForSetupEdit
        {
            get
            {
                return _doAskForSetupEdit;
            }
            set
            {
                _doAskForSetupEdit = value;
                OnPropertyChanged("doAskForSetupEdit");
            }
        }



        private bool _doChacheMD5 = false; // = new Boolean();
                                              /// <summary>
                                              /// Description of $property$
                                              /// </summary>
        [Category("imbAnalyticEngineSettings")]
        [DisplayName("doChacheMD5")]
        [Description("Description of $property$")]
        public bool doChacheMD5
        {
            get
            {
                return _doChacheMD5;
            }
            set
            {
                _doChacheMD5 = value;
                OnPropertyChanged("doChacheMD5");
            }
        }



        /// <summary> Period between two sample takes - system resources utilization monitoring </summary>
        [Category("Monitoring")]
        [DisplayName("monitoringSampling")]
        [imb(imbAttributeName.measure_letter, "RM_t")]
        [imb(imbAttributeName.measure_setUnit, "second")]
        [Description("Period between two sample takes - system resources utilization monitoring")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int monitoringSampling { get; set; } = 30;



        /// <summary> Number of DLCs to finish to trigger index publication </summary>
        [Category("Count")]
        [DisplayName("reportPlugIn_sideIndexer_DLCToSave")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of DLCs to finish to trigger index publication")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int reportPlugIn_sideIndexer_DLCToSave { get; set; } = 30;


        /// <summary> If <c>true</c> it will remove from the schedule domains that already have recorded pages in total more than Page Load limit of the current crawl </summary>
        [Category("Flag")]
        [DisplayName("reportPlugIn_sideIndexer_UseIfPagesKnown")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it will remove from the schedule domains that already have recorded pages in total more than Page Load limit of the current crawl")]
        public bool reportPlugIn_sideIndexer_UseIfPagesKnown { get; set; } = true;





        public reportPlugIn_workload_settings plugIn_workload_settings { get; set; } = new reportPlugIn_workload_settings();
    }

}