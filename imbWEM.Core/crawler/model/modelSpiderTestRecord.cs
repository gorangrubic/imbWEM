// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelSpiderTestRecord.cs" company="imbVeles" >
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



//using aceCommonTypes.extensions.text;

namespace imbWEM.Core.crawler.model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbNLP.Data;
    using imbNLP.Data.evaluate;
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
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    // using webSiteComplexCrawler;


	/// <summary>
	/// Data model :: spider execution record for <see cref="testDefinition"/>
	/// </summary>
	/// <seealso cref="aceCommonTypes.primitives.imbBindable" />
	public class modelSpiderTestRecord : modelRecordParentBase<ISpiderEvaluatorBase, spiderWeb, modelSpiderSiteRecord>, IModelSpiderGlobals
	{

		public aceDictionarySet<moduleIterationRecordSummary, DataTable> frontierDLCDataTables = new aceDictionarySet<moduleIterationRecordSummary, DataTable>();


	  //  public List<DataTable> modulesOfDomainOverview = new List<DataTable>();
	  //  public List<DataTable> modulesOfDomainSummary = new List<DataTable>();

		public List<string> crashedDomains { get; protected set; } = new List<string>();

		public List<string> relevantPages { get; protected set; } = new List<string>();


		public objectTable<iterationPerformanceRecord> lastDomainIterationTable { get; protected set; } = new objectTable<iterationPerformanceRecord>("key", "domain_last_iteration");

		public override modelRecordMode VAR_RecordModeFlags
		{
			get
			{
				return modelRecordMode.singleStarter | modelRecordMode.obligationInitBeforeStart | modelRecordMode.particularScope;
			}
		}


		//private aceEnumDictionary<contentTokenCountCategory, tokenStatistics> _tokenFrequencyMatrixByCategory = new aceEnumDictionary<contentTokenCountCategory, tokenStatistics>();
		///// <summary> </summary>
		//public aceEnumDictionary<contentTokenCountCategory, tokenStatistics> tokenFrequencyMatrixByCategory
		//{
		//    get
		//    {
		//        return _tokenFrequencyMatrixByCategory;
		//    }
		//    set
		//    {
		//        _tokenFrequencyMatrixByCategory = value;
		//        OnPropertyChanged("tokenFrequencyMatrix");
		//    }
		//}



		private modelSpiderTestGeneralRecord _tGeneralRecord;
		/// <summary> </summary>
		public modelSpiderTestGeneralRecord tGeneralRecord
		{
			get
			{
				return _tGeneralRecord;
			}
			set
			{
				_tGeneralRecord = value;
				OnPropertyChanged("tGeneralRecord");
			}
		}


		/// <summary>
		/// Gets a value indicating whether [variable allow automatic output to console].
		/// </summary>
		/// <value>
		/// <c>true</c> if [variable allow automatic output to console]; otherwise, <c>false</c>.
		/// </value>
		public override bool VAR_AllowAutoOutputToConsole
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the variable log prefix.
		/// </summary>
		/// <value>
		/// The variable log prefix.
		/// </value>
		public override string VAR_LogPrefix
		{
			get
			{
				return "SpiderTest";
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public directAnalyticReporter reporter { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public performanceCpu cpuTaker { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public performanceDataLoad dataLoadTaker { get; set; }

		/// <summary>
		/// Resource sampler
		/// </summary>
		/// <value>
		/// The measure taker.
		/// </value>
		public performanceResources measureTaker { get; set; }

		private performanceRecord _performance;
		/// <summary>
		/// 
		/// </summary>
		public performanceRecord performance
		{
			get {
				if (_performance == null) _performance = new performanceRecord(instance.name);
				return _performance;
			}
			set { _performance = value; }
		}



		private multiLanguageEvaluator _evaluator;
		/// <summary>
		/// 
		/// </summary>
		public multiLanguageEvaluator evaluator
		{
			get {

				if (_evaluator == null)
				{
					_evaluator = new multiLanguageEvaluator(basicLanguageEnum.english, basicLanguageEnum.serbian, basicLanguageEnum.slovenian, basicLanguageEnum.russian, basicLanguageEnum.german, basicLanguageEnum.italian, basicLanguageEnum.serbianCyr);

				}

				return _evaluator;
			}
			set { _evaluator = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public List<string> allUrls { get; set; } = new List<string>();


		/// <summary>
		/// 
		/// </summary>
		public List<string> allBlocks { get; set; } = new List<string>();


		/// <summary>
		/// 
		/// </summary>
		public List<string> allDetectedUrls { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public List<string> allTerms { get; set; }


		/// <summary>
		/// Gets the spider test record.
		/// </summary>
		/// <value>
		/// The t record.
		/// </value>
		public modelSpiderTestRecord tRecord
		{
			get
			{
				return this;
			}
		}

		///// <summary>
		///// Gets the crawler module reference <see cref="webSiteComplexCrawler.complexCrawlerModule"/>
		///// </summary>
		///// <value>
		///// The crawler.
		///// </value>
		//public complexCrawlerModule crawler
		//{
		//	get
		//	{
		//		return aRecord.sciProject.mainWebCrawler;
		//	}
		//}

		///// <summary>
		///// Gets the profiler.
		///// </summary>
		///// <value>
		///// The profiler.
		///// </value>
		//public webSiteProfilerModule profiler
		//{
		//	get
		//	{
		//		return aRecord.sciProject.mainWebProfiler;
		//	}
		//}


		/// <summary>
		/// Reference to the spider instance running this
		/// </summary>
		/// <value>
		/// The spider.
		/// </value>
		public ISpiderEvaluatorBase spider
		{
			get { return instance; }
		}


		/// <summary>
		/// Gets the current stage.
		/// </summary>
		/// <value>
		/// The stage.
		/// </value>
		public spiderStageBase stage
		{
			get
			{
				return stageControl.stage;
			}
		}

		private spiderStageControl _stageControl ;
		/// <summary>
		/// Gets or sets the stage control.
		/// </summary>
		/// <value>
		/// The stage control.
		/// </value>
		public spiderStageControl stageControl
		{
			get
			{
				return _stageControl;
			}
			protected set
			{

				_stageControl = value;
				OnPropertyChanged("stageControl");
			}
		}


		/// <summary>
		/// modelSpiderTestRecord access to the <see cref="analyticJobRecord"/>
		/// </summary>
		/// <value>
		/// Reference to execution record: <see cref="analyticJobRecord"/>
		/// </value>
		/// <author>
		/// Goran Grubić
		/// </author>
		/// <remarks>
		/// Intance of  records. <seealso cref="modelSpiderTestRecord" />
		/// </remarks>
		public analyticJobRecord aRecord {
			get {
				return parent as analyticJobRecord;
			}
		}


		/// <summary>
		/// Gets a job.
		/// </summary>
		/// <value>
		/// a job.
		/// </value>
		public analyticJob aJob
		{
			get
			{
				return aRecord.job;

			}
			
		}




		/// <summary>
		/// Initializes a new instance of the <see cref="modelSpiderTestRecord"/> class.
		/// </summary>
		/// <param name="__instance">The instance.</param>
		public modelSpiderTestRecord(ISpiderEvaluatorBase __instance) :base("",__instance)
		{
		   // logBuilder.isEnabled = imbWEMManager.settings.executionLog.doKeepSpiderRec;
			instance = __instance;

		}


		public void initialize(spiderStageControl stageControl)
		{
			stageControl = stageControl.Clone(instance.name);
			state = modelRecordStateEnum.initiated;
		}

		/// <summary>
		/// Inititializes the specified a flags.
		/// </summary>
		/// <param name="aFlags">a flags.</param>
		/// <param name="macroScript">The macro script.</param>
		public void inititialize(analyticJobRunFlags aFlags=analyticJobRunFlags.none, analyticMacroBase macroScript=null)
		{
			if (macroScript != null)
			{
				stageControl = macroScript.helpMethodBuildStageControl(aRecord, this);
			}
		//	tokenCounters.connectParents(aRecord.tokenCounters);
			state = modelRecordStateEnum.initiated;
		   // report.deploySample(sample);
		}


		private numericSampleStatistics _stats_pageLoaded = new numericSampleStatistics(0);
		/// <summary> </summary>
		public numericSampleStatistics stats_pageLoaded
		{
			get
			{
				return _stats_pageLoaded;
			}
			protected set
			{
				_stats_pageLoaded = value;
				OnPropertyChanged("stats_pageLoaded");
			}
		}


		private numericSampleStatistics _stats_pageSelected = new numericSampleStatistics(0);
		/// <summary> </summary>
		public numericSampleStatistics stats_pageSelected
		{
			get
			{
				return _stats_pageSelected;
			}
			protected set
			{
				_stats_pageSelected = value;
				OnPropertyChanged("stats_pageSelected");
			}
		}


		private numericSampleStatistics _stats_linksDetected = new numericSampleStatistics(0);
		/// <summary> </summary>
		public numericSampleStatistics stats_linksDetected
		{
			get
			{
				return _stats_linksDetected;
			}
			protected set
			{
				_stats_linksDetected = value;
				OnPropertyChanged("stats_linksDetected");
			}
		}


		private numericSampleStatistics _stats_iterationMade = new numericSampleStatistics(0);
		/// <summary> </summary>
		public numericSampleStatistics stats_iterationMade
		{
			get
			{
				return _stats_iterationMade;
			}
			protected set
			{
				_stats_iterationMade = value;
				OnPropertyChanged("stats_iterationMade");
			}
		}



		public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
		{

			_recordStart(__testRunStamp, __instanceID);
			// here put your code

		}

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
			//var sites = GetChildRecords();

			//foreach (var sRecord in sites)
			//{
			//    stats_pageSelected.Add(sRecord.resultPageSet.Count());
			//    stats_linksDetected.Add(sRecord.web.webLinks.Count());
			//    stats_iterationMade.Add(sRecord.iteration);
			//    stats_pageLoaded.Add(sRecord.web.webPages.Count());
			//}

			//stats_pageSelected.reCalculate(instanceCountCollection<int>.preCalculateTasks.all);
			//stats_linksDetected.reCalculate(instanceCountCollection<int>.preCalculateTasks.all);
			//stats_iterationMade.reCalculate(instanceCountCollection<int>.preCalculateTasks.all);
			//stats_pageLoaded.reCalculate(instanceCountCollection<int>.preCalculateTasks.all);

			// throw new NotImplementedException();
		}

	   
	}
}
