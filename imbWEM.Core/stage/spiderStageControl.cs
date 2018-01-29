// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderStageControl.cs" company="imbVeles" >
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
namespace imbWEM.Core.stage
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
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;

    /// <summary>
    /// Control class for stage design
    /// </summary>
    [DisplayName("Basic stage control")]
	[Description("Basic stage host with a  simple general objective solutions: iteration limit")]
	public class spiderStageControl:IAppendDataFieldsExtended   
	{


		private static PropertyCollectionCategoryList<spiderStageControl> _DataCategoryList;
		/// <summary>
		/// DataField Shemas, divided by Category specifications
		/// </summary>
		/// <remarks>
		/// <para>Auto-scanning of this level of inherence, properties splitted to categories.</para>
		/// <para>Supported <see cref="Attribute"/>: <see cref="DisplayNameAttribute"/>, <see cref="CategoryAttribute"/>, <see cref="DescriptionAttribute"/></para>
		/// <para>Supported <see cref="imbSCI.Core.attributes.imbAttribute"/>: <see cref="imbSCI.Core.attributes.imbAttributeName.measure_displayGroup"/>, <see cref="imbSCI.Core.attributes.imbAttributeName.measure_displayGroupDescripton"/>...
		/// </remarks>
		/// <example>
		/// <code>
		/// [DisplayName("Basic stage control")]
		/// [Description("Basic stage host with a  simple general objective solutions: iteration limit")]
		/// public class spiderStageControl : IAppendDataFieldsExtended
		/// </code>
		/// </example>
		/// <example>
		/// <code>
		/// [DisplayName("Sample size")]
		/// [Description("Sampled population count")]
		/// [imb(imbAttributeName.measure_displayGroup, "Sample")]
		/// [imb(imbAttributeName.measure_displayGroupDescripton, "Sample statictics")]
		/// [imb(imbAttributeName.measure_metaModelName, "SampleSize")]
		/// [imb(imbAttributeName.measure_metaModelPrefix, "SM01")]
		/// public Int32 sampleSize 
		/// </code>
		/// <para>Recommanded attributes for meta description</para>
		/// </example>
		/// <value>
		/// List of PropertyCollectionExtended representing group/categories of properties
		/// </value>
		public static PropertyCollectionCategoryList<spiderStageControl> DataCategoryList
		{
			get
			{
				if (_DataCategoryList == null)
				{
					_DataCategoryList = new PropertyCollectionCategoryList<spiderStageControl>();
				}
				return _DataCategoryList;
			}
		}




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

		public void AddObjective(string name, string description, spiderObjectiveEnum t1)
		{
			stages.Last().AddObjective(name, description, t1);
		}

		/// <summary>
		/// 
		/// </summary>
		[Category("Stage control")]
		[DisplayName("Name")]
		[Description("Name of StageControl class.")]
		public string name { get; set; }


		/// <summary>
		/// 
		/// </summary>
		[Category("Stage control")]
		[DisplayName("Description")]
		[Description("Description of the control implementation")]
		public string description { get; set; }


		/// <summary>
		/// 
		/// </summary>
		[Category("Stage control")]
		[DisplayName("Builder name")]
		[Description("Name of the analyticMacro class that built this control")]
		public string builderName { get; set; }


		/// <summary>
		/// 
		/// </summary>
		[Category("Stage control")]
		[DisplayName("Built for")]
		[Description("Spider algorithm issued stage control from the builder")]
		public string builtFor { get; set; }

		/// <summary>
		/// Clone the <see cref="spiderStageControl"/> with modified <c>builtFor</c> signature
		/// </summary>
		/// <param name="__builtFor">The built for.</param>
		/// <returns></returns>
		public spiderStageControl Clone(string __builtFor="")
		{
			if (__builtFor.isNullOrEmpty()) __builtFor = builtFor;
			spiderStageControl output = new spiderStageControl(builderName, __builtFor);

			foreach (spiderStageBase stage in stages)
			{
				
				output.stages.Add(stage.Clone());
			}

			return output;

		}


		public spiderStageControl(string __builderName, string __builtFor)
		{
			builderName = __builderName;
			builtFor = __builtFor;


            name = GetType().Name; //iTI.displayName;
			//description = iTI.displayDescription;
		}

		public void prepare()
		{
			foreach (spiderStageBase ssb in stages)
			{
				ssb.stageControl = this;
				ssb.prepare();
			}
			index = 0;
		}

		public T AddStage<T>(string name, string description, string codename) where T : spiderStageBase, new()
		{
			T output = new T();
			if (!name.isNullOrEmpty()) output.name = name;
			if (!description.isNullOrEmpty()) output.description = description;
			if (!codename.isNullOrEmpty()) output.codename = codename;

			//index = stages.Count();
			stages.Add(output);

			return output;
		}

		


		public spiderStageBase stage
		{
			get
			{
				return stages[index];
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public int index { get; set; } = 0;


		/// <summary> </summary>
		protected List<spiderStageBase> stages { get; set; } = new List<spiderStageBase>();
	}

}