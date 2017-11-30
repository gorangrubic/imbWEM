// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticConsoleState.cs" company="imbVeles" >
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
namespace imbWEM.Core.console
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
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
    using imbSCI.DataComplex.tests;
    using imbSCI.Reporting.meta.delivery;
    using imbSCI.Reporting.meta.documentSet;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.project;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    public class analyticConsoleState:aceAdvancedConsoleStateBase
    {

        [XmlIgnore]
        public pluginStackCollection pluginStack { get; set; } = new pluginStackCollection();


        private testLabelingSettings _runstampSetup = new testLabelingSettings(); // = new testLabelingSettings();
                                                      /// <summary>
                                                      /// Settings of the runstamp generation
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("Runstamp Setup")]
        [Description("Settings of the runstamp generation")]
        public testLabelingSettings runstampSetup
        {
            get
            {
                return _runstampSetup;
            }
            set
            {
                _runstampSetup = value;
                OnPropertyChanged("runstampSetup");
            }
        }


        public string setupHash_global { get; set; } = "";
        public string setupHash_crawler { get; set; } = "";

        private spiderStageControl _stageControl ; // = new spiderStageControl();
                                                      /// <summary>
                                                      /// Stage control to be assigned to each crawler instance
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("stageControl")]
        [Description("Stage control to be assigned to each crawler instance")]
        [XmlIgnore]
        public spiderStageControl stageControl
        {
            get
            {
                return _stageControl;
            }
            set
            {
                _stageControl = value;
                OnPropertyChanged("stageControl");
            }
        }



        private analyticJob _job = default(analyticJob); // = new analyticJob();
                                                      /// <summary>
                                                      ///Current AnalyticJob
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("Job")]
        [Description("Current AnalyticJob")]
        [XmlIgnore]
        public analyticJob job
        {
            get
            {
                return _job;
            }
            set
            {
                _job = value;
                OnPropertyChanged("job");
            }
        }


        private analyticProject _sciProject ;
        /// <summary> </summary>
        [Category("analyticConsoleState")]
        [DisplayName("Sci Project")]
        [Description("instance of Job Record class")]
        public analyticProject sciProject
        {
            get
            {
                if (_sciProject == null)
                {
                    _sciProject = new analyticProject();
                    
                }
                
                return _sciProject;
            }
            set
            {
                _sciProject = value;
                OnPropertyChanged("sciProject");
            }
        }

        private analyticJobRecord _aRecord = default(analyticJobRecord); // = new analyticJobRecord();
                                                      /// <summary>
                                                      /// instance of Job Record class
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("Job Record")]
        [Description("instance of Job Record class")]
        [XmlIgnore]
        public analyticJobRecord aRecord
        {
            get
            {
                return _aRecord;
            }
            set
            {
                _aRecord = value;
                OnPropertyChanged("aRecord");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public DirectoryInfo lastJobDirectory { get; set; }


        private deliveryUnit _reportDeliveryUnit = default(deliveryUnit); // = new deliveryUnit();
                                                      /// <summary>
                                                      /// Unit used to deliver the report rendering
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("reportDeliveryUnit")]
        [Description("Unit used to deliver the report rendering")]
        [XmlIgnore]
        public deliveryUnit reportDeliveryUnit
        {
            get
            {
                return _reportDeliveryUnit;
            }
            set
            {
                _reportDeliveryUnit = value;
                OnPropertyChanged("reportDeliveryUnit");
            }
        }




        private metaDocumentRootSet _reportDocument = default(metaDocumentRootSet); // = new metaDocumentRootSet();
                                                      /// <summary>
                                                      /// Report document to be constructed after analytic job finished
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("reportDocument")]
        [Description("Report document to be constructed after analytic job finished")]
        [XmlIgnore]
        public metaDocumentRootSet reportDocument
        {
            get
            {
                return _reportDocument;
            }
            set
            {
                _reportDocument = value;
                OnPropertyChanged("reportDocument");
            }
        }


        [Category("analyticConsoleState")]
        [DisplayName("Sample")]
        [Description("Currently loaded list of domains to load")]
        [XmlIgnore]
        public webSiteSimpleSample sampleList { get; set; } = new webSiteSimpleSample();


        private List<string> _failedDomains  = new List<string>();
                                                      /// <summary>
                                                      /// list of domains that were failed with seed url
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("failedDomains")]
        [Description("list of domains that were failed with seed url")]
        public List<string> failedDomains
        {
            get
            {
                return _failedDomains;
            }
            set
            {
                _failedDomains = value;
                OnPropertyChanged("failedDomains");
            }
        }

        /// <summary>
        /// Makes the run stamp.
        /// </summary>
        /// <returns></returns>
        public string makeTheRunStamp()
        {
            string runstamp = job.testInfo.getRunStamp(runstampSetup);
            
            runstamp = runstamp.add(sampleList.Count().ToString("D3"), "_");
            job.runstamp = runstamp;
            lastRunstamp = runstamp;
            return runstamp;
        }


        /// <summary>
        /// 
        /// </summary>
        public string lastRunstamp { get; set; }


        public crawlerDomainTaskMachineSettings crawlerJobEngineSettings { get; set; } 


        private ISpiderEvaluatorBase _crawler ; // = new ISpiderEvaluatorBase();
                                                      /// <summary>
                                                      /// Currently selected crawler
                                                      /// </summary>
        [Category("analyticConsoleState")]
        [DisplayName("Current Crawler")]
        [Description("Currently selected crawler")]
        [XmlIgnore]
        public ISpiderEvaluatorBase crawler
        {
            get
            {
                return _crawler;
            }
            set
            {
                _crawler = value;
                OnPropertyChanged("crawler");
            }
        }

        public string sampleTags { get; set; }
        public string sampleFile { get;  set; }
        public indexPerformanceEntry indexSession { get; set; }
    }

}