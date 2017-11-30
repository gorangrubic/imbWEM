// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticJob.cs" company="imbVeles" >
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
namespace imbWEM.Core.project
{
    using System;
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
    using imbNLP.Data.evaluate;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon;
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
    using imbSCI.Core.reporting.colors;
    using imbSCI.Core.reporting.style.core;
    using imbSCI.Core.reporting.style.enums;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tests;
    using imbSCI.Reporting.meta.delivery;
    using imbSCI.Reporting.meta.delivery.units;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Object model describing web analytic test execution session
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class analyticJob:imbBindable, IAppendDataFieldsExtended, IObjectWithNameAndDescription
	{


		private textEvaluator _langTextEvaluator;
		/// <summary>
		/// 
		/// </summary>
		public textEvaluator langTextEvaluator
		{
			get {
				if (_langTextEvaluator == null) _langTextEvaluator = new textEvaluator(semanticLexiconManager.manager);
				return _langTextEvaluator;
			}
			protected set { _langTextEvaluator = value; }
		}


		/// <summary>
		/// Name for this instance
		/// </summary>
		public string name { get; set; } = "analyticJob";

		/// <summary>
		/// Human-readable description of object instance
		/// </summary>
		public string description { get; set; } = "";


		/// <summary>
		/// Initializes a new instance of the <see cref="analyticJob"/> class.
		/// </summary>
		public analyticJob()
		{
			testInfo = new testDefinition("Spider comparative experiment", "The test performes Key Pages detection running four different Spider algorithms for each item in selected sample scope.", 1);
			DeliveryUnit = new deliveryUnitBootmarkReport();
			DeliveryUnit.setup();

		 //   sampleTags = imbWEMManager.settings.sampleGroup.SplitSmart(",", "primary");
			//sampleGroup = imbAnalyticsEngine.imbWEMManager.webProfileGroups.primarySample;
			

		// report = new sampleProcessReport();
		theme = new styleTheme(colorSet:aceBaseColorSetEnum.imbScience, h1Size:16, pSize:14, margin:4, padding:4, pageFontName:aceFont.TrebuchetMS,headingFontName:aceFont.TrebuchetMS);

			spider = new spiderUnit();
			
		}


		private int _sampleSkip = 0;
		/// <summary> </summary>
		public int sampleSkip
		{
			get
			{
				return _sampleSkip;
			}
			set
			{
				_sampleSkip = value;
				OnPropertyChanged("sampleSkip");
			}
		}


		//public analyticConsole console { get; set; }


		#region


		/// <summary>
		/// Appends its data points into new or existing property collection
		/// </summary>
		/// <param name="data">Property collection to add data into</param>
		/// <returns>Updated or newly created property collection</returns>
		public PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
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

		/// <summary>
		/// Appends its data points into new or existing property collection
		/// </summary>
		/// <param name="data">Property collection to add data into</param>
		/// <returns>
		/// Updated or newly created property collection
		/// </returns>
		/// <exception cref="NotImplementedException"></exception>
		PropertyCollection IAppendDataFields.AppendDataFields(PropertyCollection data)
		{
			return AppendDataFields(data as PropertyCollectionExtended);
		}


		/// <summary>
		/// 
		/// </summary>
		public string runstamp { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public spiderUnit spider { get; set; }


		//private spiderEvaluatorCollection _evaluatorCollection; <---- spiders su sada kod MACRO-a
		///// <summary>
		///// 
		///// </summary>
		//public spiderEvaluatorCollection evaluatorCollection
		//{
		//    get { return _evaluatorCollection; }
		//    set { _evaluatorCollection = value; }
		//}


		/// <summary>
		/// 
		/// </summary>
		public testDefinition testInfo { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public deliveryUnit DeliveryUnit { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public List<string> sampleTags { get; set; } = new List<string>();


		//      private List<sampleGroupItem> _sampleGroups;
		///// <summary>
		///// Gets the primary sample.
		///// </summary>
		///// <value>
		///// The primary sample.
		///// </value>
		//public List<sampleGroupItem> sampleGroups
		//{
		//	get { return _sampleGroups; }
		//	private set { _sampleGroups = value; }
		//}

		/// <summary>
		/// static and autoinitiated object
		/// </summary>
		public styleTheme theme { get; }

		/// <summary>
		/// Referrence to the console that created this job (if it is created by the console instance)
		/// </summary>
		/// <value>
		/// The console.
		/// </value>
		public analyticConsole console { get; internal set; }



		#endregion

	}
}
