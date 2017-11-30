// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelSpiderPageRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.model
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
    using imbCommonModels.pageAnalytics.core;
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
    using imbSCI.Core.extensions.data;
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
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    // using webSiteComplexCrawler;
    //..htmlNodeProcessing


    /// <summary>
    /// Data model :: spider execution record for <see cref="testDefinition"/>
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class modelSpiderPageRecord : modelRecordStandaloneBase<spiderLink>, IModelSiteGlobals
    {







        private modelWebPageGeneralRecord _pGeneralRecord ;
        /// <summary> </summary>
        public modelWebPageGeneralRecord pGeneralRecord
        {
            get
            {
                return _pGeneralRecord;
            }
             set
            {
                _pGeneralRecord = value;
                OnPropertyChanged("pGeneralRecord");
            }
        }


        public override modelRecordMode VAR_RecordModeFlags
        {
            get
            {
                return modelRecordMode.singleStarter | modelRecordMode.obligationStartBeforeInit | modelRecordMode.particularScope;
            }
        }



        private spiderWeb _web;
        /// <summary> </summary>
        public spiderWeb web
        {
            get
            {
                return wRecord.web;
            }
           
        }

        public modelSpiderPageRecord pRecord
        {
            get
            {
                return this;
            }
        }


        public modelSpiderSiteRecord wRecord
        {
            get
            {
                return (modelSpiderSiteRecord) parent;
            }
        }

        
        //private crawlerAgentContext _crawlerContext;
        ///// <summary>
        ///// OVO samostalno instancira
        ///// </summary>
        //public crawlerAgentContext crawlerContext
        //{
        //    get
        //    {
        //        return wRecord.crawlerContext;
        //    }
        //}


        /// <summary>
        /// modelSpiderPageRecord access to the <see cref="modelSpiderTestRecord"/>
        /// </summary>
        /// <value>
        /// Reference to execution record: <see cref="modelSpiderTestRecord"/>
        /// </value>
        /// <author>
        /// Goran Grubić
        /// </author>
        /// <remarks>
        /// Intance of  records. <seealso cref="modelSpiderPageRecord"/>
        /// </remarks>
        public modelSpiderTestRecord tRecord
        {
            get
            {
                return (modelSpiderTestRecord)parent.parent;
            }
        }
        
        /// <summary> </summary>
        public analyticJob aJob
        {
            get
            {
                return aRecord.job;

            }

        }

        /// <summary>
        /// modelSpiderPageRecord access to the <see cref="analyticJobRecord"/>
        /// </summary>
        /// <value>
        /// Reference to execution record: <see cref="analyticJobRecord"/>
        /// </value>
        /// <author>
        /// Goran Grubić
        /// </author>
        /// <remarks>
        /// Intance of  records. <seealso cref="modelSpiderPageRecord"/>
        /// </remarks>
        public analyticJobRecord aRecord
        {
            get
            {
                return parent.parent.parent as analyticJobRecord;
            }
        }


        /// <summary>
        /// Gets the stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        public spiderStageBase stage
        {
            get
            {
                return tRecord.stageControl.stage;
            }
        }

        /// <summary>
        /// Gets the stage control.
        /// </summary>
        /// <value>
        /// The stage control.
        /// </value>
        public spiderStageControl stageControl
        {
            get
            {
                return tRecord.stageControl;
            }
        }

      

        ///// <summary>
        ///// Gets the profiler.
        ///// </summary>
        ///// <value>
        ///// The profiler.
        ///// </value>
        //public webSiteProfilerModule profiler
        //{
        //    get
        //    {
        //        return aRecord.sciProject.mainWebProfiler;
        //    }
        //}


        /// <summary>
        /// Reference to the spider instance running this
        /// </summary>
        /// <value>
        /// The spider.
        /// </value>
        public ISpiderEvaluatorBase spider
        {
            get { return tRecord.instance; }
        }
        

        #region access to global registars 

        /// <summary>
                                    /// Bindable property
                                    /// </summary>
        public crawledPage page { get; private set; }


        public void init(spiderPage __pageInstance)
        {
            state = modelRecordStateEnum.initiated;
            pageInstance = __pageInstance;
            if (pGeneralRecord != null)
            {
                pGeneralRecord.sideRecordSets.AddRecord(instance, this);
            } else
            {
                //pGeneralRecord.instance
            }

        }

        public void acceptPage(crawledPage __page)
        {
            if (__page != null) page = __page;

            log("Page [" + page.caption + "] loaded [" + page.url + "]");

           
        }

        #endregion




        public override bool VAR_AllowAutoOutputToConsole
        {
            get
            {
                return false;
            }
        }

        public override string VAR_LogPrefix
        {
            get
            {
                return "page";
            }
        }





        public modelSpiderPageRecord(string __testRunStamp, spiderLink __instance) : base(__testRunStamp, __instance)
        {
            iteration = __instance.iterationDiscovery;
            //logBuilder.isEnabled = imbWEMManager.settings.executionLog.doKeepPageRecLog;
           // pageProfile = __instance;
        }




        private spiderPage _pageInstance ;
        /// <summary> </summary>
        public spiderPage pageInstance
        {
            get
            {
                return _pageInstance;
            }
            protected set
            {
                _pageInstance = value;
                OnPropertyChanged("pageInstance");
            }
        }

        #region REPORTING DATA ----------------------------------------------------------------



        private int _linksToPage = 0; // = new Int32();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>

        public int linksToPage
        {
            get
            {
                return _linksToPage;
            }
            set
            {
                _linksToPage = value;
                OnPropertyChanged("linksToPage");
            }
        }



        private int _linksFromPage = 0; // = new Int32();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        
        public int linksFromPage
        {
            get
            {
                return _linksFromPage;
            }
            set
            {
                _linksFromPage = value;
                OnPropertyChanged("linksFromPage");
            }
        }


        private int _linkScore = 0; // = new Int32();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        
        public int linkScore
        {
            get
            {
                return _linkScore;
            }
            set
            {
                _linkScore = value;
                OnPropertyChanged("linkScore");
            }
        }


        private int _pageScore = 0; // = new Int32();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        
        public int pageScore
        {
            get
            {
                return _pageScore;
            }
            set
            {
                _pageScore = value;
                OnPropertyChanged("pageScore");
            }
        }


        private relationShipCounter _relationCounter;
        /// <summary>
        /// 
        /// </summary>
        public relationShipCounter relationCounter
        {
            get {
                if (_relationCounter == null) _relationCounter = new relationShipCounter(instance);
                return _relationCounter;
            }
            set { _relationCounter = value; }
        }

         
        

        private int _iteration = default(int); // = new Int32();                                    
        [Category("modelSpiderPageRecord")]
        [DisplayName("Iteration")]
        [Description("Loaded iteration, the iteration this page was loaded")]
        public int iteration
        {
            get
            {
                return _iteration;
            }
            set
            {
                _iteration = value;
                OnPropertyChanged("iteration");
            }
        }

        private bool _crosslinkWithAllSelected = new bool();
        /// <summary> </summary>
        public bool crosslinkWithAllSelected
        {
            get
            {
                return _crosslinkWithAllSelected;
            }
            protected set
            {
                _crosslinkWithAllSelected = value;
                OnPropertyChanged("crosslinkWithAllSelected");
            }
        }


        #endregion






        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            return base.AppendDataFields(data, modelRecordFieldToAppendFlags.modelRecordCommonData| modelRecordFieldToAppendFlags.modelRecordInstanceData);
        }

        public override void datasetBuildOnFinish()
        {
            
            iteration = instance.iterationDiscovery;
            if (pageInstance == null)
            {

                pageInstance = wRecord.web.webPages.GetPageByLink(instance);


            }

            if (pageInstance == null) return;
            //wRecord.web.webPages.items.Values.fi


            linksToPage = pageInstance.relationship.inflowLinks.Count();
            linksFromPage = pageInstance.relationship.outflowLinks.Count();
            linkScore = pageInstance.spiderResult.target.marks.score;
            pageScore = pageInstance.marks.score;

        }

        public override void datasetBuildOnFinishDefault()
        {
            if (dataCollectionExtendedList == null) dataCollectionExtendedList = new PropertyCollectionExtendedList();

            var data = AppendDataFields(null, modelRecordFieldToAppendFlags.modelRecordInstanceData | modelRecordFieldToAppendFlags.modelRecordCommonData);
            //dataSet.Tables.Add(data.buildDataTableVertical(globalTableEnum.identification.GetTableName(), globalTableEnum.identification.GetTableDescription()));

            dataCollectionExtendedList.Add(data, "Execution record");
            
        }

        public override void recordFinish(params object[] resources)
        {

            if (pageInstance == null)
            {
                aceLog.log("Warning :: page instance is not set for modelSpiderPageRecord["+instance.url+"] at [" + tRecord.instance.name + "]");
                return;
            }

            var outputTwo = resources.getFirstOfType<List<spiderPage>>();
            relationCounter.SetRelationShip(outputTwo);

            crosslinkWithAllSelected = (relationCounter[spiderLinkRelationType.crosslinked] == outputTwo.Count);

           // dataSet.Tables.Add();

            _recordFinish();
        }

        public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
        {
            _recordStart(__testRunStamp, __instanceID);
        }

        protected override void _recordStartHandle()
        {
            
        }
    }

}