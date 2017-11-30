// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelSpiderSiteRecord.cs" company="imbVeles" >
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Network.tools;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webStructure;
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
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.enums.tableReporting;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.linknode;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.modules;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.project;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    // using webSiteComplexCrawler;


    /// <summary>
    /// Data model :: spider execution record for <see cref="testDefinition"/>
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class modelSpiderSiteRecord : modelRecordParentBase<spiderWeb, spiderLink, modelSpiderPageRecord>, IModelSiteGlobals
    {





        public bool indexDeployedMe { get; set; } = false;

        private weightTableCompiled _MasterTFIDF;
        public weightTableCompiled MasterTFIDF
        {
            get {

                if (_MasterTFIDF == null) {
                    _MasterTFIDF = imbWEMManager.index.experimentEntry.LoadNewTFIDF_Master(imbWEMManager.settings.TFIDF.doUseSeparateTFIDFperDLC_iterationEvaluation);
                    _MasterTFIDF.ReadOnlyMode = true;
                }
                return _MasterTFIDF;
            }

        }


        /// <summary>
        /// Informacija iz domen indeksa
        /// </summary>
        /// <value>
        /// The index domain information.
        /// </value>
        protected indexDomain indexDomainInfo { get; set; }

        /// <summary>
        /// Gets the index information.
        /// </summary>
        /// <returns></returns>
        public indexDomain GetIndexInfo()
        {
            if (indexDomainInfo == null)
            {
                indexDomainInfo = imbWEMManager.index.domainIndexTable.GetDomain(domainInfo.domainName);
            }
            return indexDomainInfo;
        }


        private int _pageRecallTarget = -1;

        /// <summary>
        /// Target for recall measurement
        /// </summary>
        public int pageRecallTarget
        {
            get {
                if (_pageRecallTarget == -1)
                {
                    if (tRecord?.instance?.settings != null)
                    {
                        var iDomain = indexDomainInfo;
                        if (iDomain == null)
                        {
                            _pageRecallTarget = wRecord.tRecord.instance.settings.limitTotalPageLoad;
                        }
                        else
                        {
                            _pageRecallTarget = Math.Min(wRecord.tRecord.instance.settings.limitTotalPageLoad, GetIndexInfo().relevantPages);
                        }
                    }
                }
                return _pageRecallTarget; }
            protected set { _pageRecallTarget = value; }
        }



        /// <summary>
        /// Record on module performances
        /// </summary>
        /// <value>
        /// The frontier DLC.
        /// </value>
        public frontierRankingAlgorithmDLCRecord frontierDLC { get; set; } // = new frontierRankingAlgorithmDLCRecord();

        //public moduleDLCRecord moduleRecords { get; set; };

        public List<string> relevantPages { get; protected set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public spiderModuleData<spiderLink> lastInput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public spiderModuleData<spiderLink> lastOutput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public spiderModuleData<spiderLink> currentModuleData { get; set; }


        private modelWebSiteGeneralRecord _wGeneralRecord;
        /// <summary> </summary>
        public modelWebSiteGeneralRecord wGeneralRecord
        {
            get
            {
                return _wGeneralRecord;
            }
            set
            {
                _wGeneralRecord = value;
                OnPropertyChanged("wGeneralRecord");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public objectTable<iterationPerformanceRecord> iterationTableRecord { get; set; }


        public override modelRecordMode VAR_RecordModeFlags
        {
            get
            {
                return modelRecordMode.singleStarter | modelRecordMode.obligationStartBeforeInit | modelRecordMode.particularScope;
            }
        }

        //private aceEnumDictionary<contentTokenCountCategory, tokenStatistics> _tokenFrequencyMatrixByCategory = new aceEnumDictionary<contentTokenCountCategory, tokenStatistics>();
        ///// <summary> </summary>
        //public aceEnumDictionary<contentTokenCountCategory, tokenStatistics> tokenFrequencyMatrixByCategory
        //{
        //	get
        //	{
        //		return _tokenFrequencyMatrixByCategory;
        //	}
        //	set
        //	{
        //		_tokenFrequencyMatrixByCategory = value;
        //		OnPropertyChanged("tokenFrequencyMatrix");
        //	}
        //}


        private spiderDLContext _context;
        /// <summary>
        /// 
        /// </summary>
        public spiderDLContext context
        {
            get {

                if (_context == null)
                {
                    _context = new spiderDLContext(this);
                }
                return _context;
            }
            set { _context = value; }
        }



        private string _domain;
        /// <summary>
        /// 
        /// </summary>
        public string domain
        {
            get {
                if (_domain.isNullOrEmpty()) _domain = web.domain;
                return _domain; }
            set { _domain = value; }
        }


        private spiderWeb _web;
        /// <summary> </summary>
        public spiderWeb web
        {
            get
            {
                return (spiderWeb)instance;
            }
            set
            {
                _web = value;
                OnPropertyChanged("web");
            }
        }

        public modelSpiderSiteRecord wRecord
        {
            get
            {
                return this;
            }
        }

        //private crawlerAgentContext _crawlerContext;
        ///// <summary>
        ///// OVO samostalno instancira
        ///// </summary>
        //public crawlerAgentContext crawlerContext
        //{
        //	get
        //	{
        //		return _crawlerContext;
        //	}
        //}


        public modelSpiderTestRecord tRecord
        {
            get
            {
                return (modelSpiderTestRecord)parent;
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

        public analyticJobRecord aRecord
        {
            get
            {
                return parent.parent as analyticJobRecord;
            }
        }


        //public contentTreeGlobalCollection contentTreeGlobalRegistar
        //{
        //	get
        //	{
        //		return aRecord.contentTreeGlobalRegistar;
        //	}
        //}


        //private contentTreeDomainCollection _contentTreeCollection = new contentTreeDomainCollection();
        ///// <summary> </summary>
        //public contentTreeDomainCollection contentTreeCollection
        //{
        //	get
        //	{
        //		return _contentTreeCollection;
        //	}
        //	protected set
        //	{
        //		_contentTreeCollection = value;
        //		OnPropertyChanged("contentTreeCollection");
        //	}
        //}


        public override bool VAR_AllowAutoOutputToConsole
        {
            get
            {
                return true;
            }
        }

        public override string VAR_LogPrefix
        {
            get
            {
                return "SiteRec";
            }
        }



        public spiderStageBase stage
        {
            get
            {
                return tRecord.stageControl.stage;
            }
        }

        /// <summary> </summary>
        public spiderStageControl stageControl
        {
            get
            {
                return tRecord.stageControl;
            }
        }



        ///// <summary> </summary>
        //public webSiteProfilerModule profiler
        //{
        //	get
        //	{
        //		return aRecord.sciProject.mainWebProfiler;
        //	}
        //}


        /// <summary>
        /// 
        /// </summary>
        public domainAnalysis domainInfo { get; set; }


        /// <summary>
        /// Real instance of spider/crawler used for this DLC thread
        /// </summary>
        [XmlIgnore]
        public ISpiderEvaluatorBase spiderReal { get; set; }


        /// <summary>
        /// Reference to crawler domain task that runs this DLC thread
        /// </summary>
        public crawlerDomainTask spiderDLCTask { get; set; }


        /// <summary>
        /// Reference to the spider blueprint instance of crawler (the one created by analytic console)
        /// </summary>
        /// <value>
        /// The spider.
        /// </value>
        public ISpiderEvaluatorBase spider
        {
            get { return tRecord.instance; }
        }


        public modelSpiderSiteRecord(string __testRunStamp, spiderWeb __instance) : base(__testRunStamp, __instance)
        {
            //logBuilder.isEnabled = imbWEMManager.settings.executionLog.doKeepSiteRec;


            domainInfo = new domainAnalysis(instance.seedLink.url);
            iterationTableRecord = new objectTable<iterationPerformanceRecord>("key", "iteration_" + domainInfo.domainName.Replace(".", "_"));



            //moduleRecords.start()

            // domainInfo = new domainAnalysis(__instance.seedLink.url);


            // stats.Add(modelSpiderSideFields.mss_totalcrosslinks, 0, "Total crosslinks", "Total number of crosslinks detected among pages");

            //stats.Add(modelSpiderSiteTimelineEnum.tl_iteration, 0, "Iteration", "Final iteration count");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_pagesloaded, 0, "Pages loaded", "Total count of pages loaded");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_pagesdetected, 0, "Pages detected", "Total count of pages loaded");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_pagesaccepted, 0, "Pages accepted", "Total count of pages loaded");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_totallinks, 0, "Links detected", "Total links processed");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_linksaccepted, 0, "Links accepted", "Total links processed");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_activelinks, 0, "Ative links", "Active links left at the end of procedure");
            //stats.Add(modelSpiderSiteTimelineEnum.tl_stability, 0, "Pages loaded", "Total links processed");

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void initialize()
        {
            state = modelRecordStateEnum.initiated;

            // domainInfo = new domainAnalysis(instance.seedLink.url);
        }


        /// <summary>
        /// Starts the child record.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="__instanceID">The instance identifier.</param>
        public override void startChildRecord(spiderLink instance, string __instanceID)
        {
            base.startChildRecord(instance, __instanceID);
        }

        /// <summary>
        /// Finishes the child record.
        /// </summary>
        public override void finishChildRecord()
        {
            base.finishChildRecord();
        }


        // <------------------------------------------------------- INCLUDED DATA POINTS ----------------------------------------------------------------------------->


        private dataUnitSpiderIterationHistory _timeseries = new dataUnitSpiderIterationHistory();
        /// <summary>Time series with statistics of each iteration</summary>
        public dataUnitSpiderIterationHistory timeseries
        {
            get
            {
                if (_timeseries == null) _timeseries = new dataUnitSpiderIterationHistory(stage.stageIterationLimit);
                return _timeseries;
            }
            protected set
            {
                _timeseries = value;
                OnPropertyChanged("timeseries");
            }
        }



        private numericSampleStatisticsList _crossLinkStats = new numericSampleStatisticsList("Crosslink statistics", "Descriptive statistics on crosslinks for each iteration within the *1st stage* of crawling");
        /// <summary> </summary>
        public numericSampleStatisticsList crossLinkStats
        {
            get
            {
                return _crossLinkStats;
            }
            protected set
            {
                _crossLinkStats = value;
                OnPropertyChanged("crossLinkStats");
            }
        }





        private linknodeBuilder _linkHierarchy = new linknodeBuilder();
        /// <summary> </summary>
        public linknodeBuilder linkHierarchy
        {
            get
            {
                return _linkHierarchy;
            }
            protected set
            {
                _linkHierarchy = value;
                OnPropertyChanged("linkHierarchy");
            }
        }


        /// <summary>
        /// Collection of results
        /// </summary>
        public List<spiderTaskResult> spiderTaskResults { get; set; } = new List<spiderTaskResult>();


        /// <summary>
        /// 
        /// </summary>
        public List<spiderPage> resultPageSet { get; set; } = new List<spiderPage>();


        private int _blockMax = int.MinValue;
        /// <summary>
        /// 
        /// </summary>
        public int blockMax
        {
            get {
                if (_blockMax == int.MinValue) checkBlockMinMax();
                return _blockMax; }
            set { _blockMax = value; }
        }


        private int _blockMin = int.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        public int blockMin
        {
            get {
                if (_blockMin == int.MaxValue) checkBlockMinMax();
                return _blockMin; }
            set { _blockMin = value; }
        }


        private int _pageTokenizationFailed;
        /// <summary> </summary>
        public int pageTokenizationFailed
        {
            get
            {
                return _pageTokenizationFailed;
            }
            set
            {
                _pageTokenizationFailed = value;
                OnPropertyChanged("pageTokenizationFailed");
            }
        }


        protected void checkBlockMinMax()
        {
            pageTokenizationFailed = 0;
            //foreach (var sp in web.webPages.items.Values)
            //{
            //	if (sp.webpage.tokenizedContent != null)
            //	{
            //		Int32 blc = sp.webpage.tokenizedContent.blocks.Count();
            //		_blockMin = Math.Min(_blockMin, blc);
            //		_blockMax = Math.Max(_blockMax, blc);
            //	} else
            //	{
            //		pageTokenizationFailed++;
            //	}
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public int iteration { get; set; } = 0;

        //public webSiteProfile siteProfile { get; internal set; }

        public int duplicateCount { get; set; } = 0;

        private contentHashAndAddressEntryList _listOfDuplicatedPages = new contentHashAndAddressEntryList("Duplicate page content detected", "Content duplicate detected using MD5 content hash. The frequency assigned to the each row is showing the number of duplicated detected that far for the same hash value.", contentHashTypeEnum.pageContent);
        /// <summary> </summary>
        public contentHashAndAddressEntryList listOfDuplicatedPages
        {
            get
            {
                return _listOfDuplicatedPages;
            }
            protected set
            {
                _listOfDuplicatedPages = value;
                OnPropertyChanged("listOfDuplicatedPages");
            }
        }


        /// <summary>
        /// Records the start. Make sure to call <see cref="!:_recordStart" /> at beginning of the method
        /// </summary>
        /// <param name="__testRunStamp"></param>
        /// <param name="__instanceID"></param>
        /// <param name="resources"></param>
        public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
        {
            _recordStart(__testRunStamp, __instanceID);
            // here put your code

        }



        /// <summary>
        /// Records the finish. Make sure to call <see cref="!:_recordFinish" /> at the end of the method
        /// </summary>
        /// <param name="resources"></param>
        public override void recordFinish(params object[] resources)
        {
            _recordFinish();
            // here put your code
        }

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();
            /*
			data.Add("grouptag", groupTag, "Database tag", "Tag string to mark sample item row in the database table");
			data.Add("groupweight", weight, "Weight factor", "Relative weight number used for automatic population-to-group assigment");
			data.Add("grouplimit", groupSizeLimit, "Size limit", "Optional limit for total count of population within this group");
			data.Add("groupcount", count, "Sample count", "Sample item entries count attached to this group");
			*/
            return data;
        }

        public override void datasetBuildOnFinish()
        {


            web.webPageContentHashList.RemoveUnderFreg(2);
            web.webPageContentHashList.reCalculate();

            // <--- ovde treba da kreiram agregatni data point

            //.Tables.Add(crossLinkStats.getDataTableHorizontal());
        }

        // <--------------------------- data getters

        public void Dispose()
        {
            timeseries = null;
            lastInput = null;
            lastOutput = null;
            currentModuleData = null;
            web = null;
            wProfile = null;
            listOfDuplicatedPages = null;
            resultPageSet = null;
            spiderTaskResults = null;
            crossLinkStats = null;
            //_crawlerContext = null;
            linkHierarchy = null;
            _MasterTFIDF = null;

        }

        public webSiteProfile wProfile { get; set; }

            

		/// <summary>
		/// Gets the crosslinks statistics. 
		/// </summary>
		/// <remarks>
		/// Supported <c>format</c>s are: <see cref="dataTableDeliveryEnum.verticalCalculatedSummary"/>, <see cref="dataTableDeliveryEnum.dataTableVertical"/>, <see cref="dataTableDeliveryEnum.dataTableHorizontal"/>, <see cref="dataTableDeliveryEnum.comparative"/>
		/// </remarks>
		/// <param name="format">The format.</param>
		/// <returns></returns>
		public DataTable GetCrossLinkStats(dataTableDeliveryEnum format)
		{
			if (format.HasFlag(dataTableDeliveryEnum.verticalCalculatedSummary))
			{

				return crossLinkStats.GetSummary(dataTableSummaryRowEnum.sum).getDataTableVertical().SetClassName("Crosslink statistics summary");
			}

			if (format.HasFlag(dataTableDeliveryEnum.dataTableVertical))
			{
				return crossLinkStats.Current().getDataTableVertical().SetClassName("Crosslink statistics single iteration");
			}

			if (format.HasFlag(dataTableDeliveryEnum.dataTableHorizontal))
			{
				return crossLinkStats.getDataTableHorizontal().SetClassName("Crosslink statistics");
			}

			if (format.HasFlag(dataTableDeliveryEnum.comparative))
			{
				return crossLinkStats.getDataTableComparative().SetClassName("Crosslink statistics");
			}


			if (format.HasFlag(dataTableDeliveryEnum.dataTableHorizontal))
			{
				return crossLinkStats.getDataTableHorizontal().SetClassName("Crosslink statistics");
			}
			
			throw this.aceGenEx("The supplied format flags [" + format.ToString() + "] not supported");
			
		}
	}
}
