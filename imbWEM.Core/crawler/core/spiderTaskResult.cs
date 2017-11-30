// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTaskResult.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.core
{
    using System;
    using System.Collections;
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
    using imbCommonModels.pageAnalytics.enums;
    using imbCommonModels.structure;
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

    public class spiderTaskResult:IEnumerable<spiderTaskResultItem>
    {


        //private PropertyCollectionExtended _resultStatistics = new PropertyCollectionExtended();
        ///// <summary>
        ///// 
        ///// </summary>
        //public PropertyCollectionExtended resultStatistics
        //{
        //    get { return _resultStatistics; }
        //    set { _resultStatistics = value; }
        //}

        public spiderTaskResult(spiderTask __task)
        {
            startTime = DateTime.Now;
            task = __task;
        }

        public spiderTaskResult()
        {
            startTime = DateTime.Now;
        }


        private object AddResultLock = new object();

        public void AddResult(spiderTaskResultItem __result)
        {
            lock (AddResultLock)
            {
                items.Add(__result.task, __result);
            }
        }

        public void finish()
        {
            duration = DateTime.Now.Subtract(startTime);
        }


        public double calculateSuccessRate()
        {
            int cR = this.Count();
            int cL = 0;
            foreach (spiderTaskResultItem r in this)
            {
                if (r.status == pageStatus.loaded)
                {
                    cL++;
                }
            }
            if (cL == 0) return 0;
            if (cR == 0) return 0;
            return (double)cL / (double)cR;
        }

        
      

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <returns></returns>
        public DataTable getDataTable()
        {
            DataTable table = new DataTable("Spider result [i:" + task.iteration + "]");

            
            DataColumn item = table.Columns.Add("Item");
            DataColumn url = table.Columns.Add("_Url");
            DataColumn caption = table.Columns.Add("Page caption");
            DataColumn status = table.Columns.Add("Status");
            DataColumn duration_column = table.Columns.Add("Duration");

            int c = 0;
            foreach (spiderTaskResultItem rItem in items.Values)
            {
                DataRow dr = table.NewRow();
                dr[item] = c.ToString("D3");
                dr[url] = rItem.task.url;
                dr[caption] = rItem.page.caption;
                dr[status] = rItem.status;
                dr[duration_column] = rItem.duration.TotalMilliseconds.getTimeSecString();
                table.Rows.Add(dr);
                c++;
            }

            return table;
        }


        /// <summary>
        /// Time of constructor call
        /// </summary>
        public DateTime startTime { get; protected set; }


        /// <summary>
        /// Time span between constructor and <see cref="finish"/> call 
        /// </summary>
        public TimeSpan duration { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<link, spiderTaskResultItem> items { get; protected set; } = new Dictionary<link, spiderTaskResultItem>();

        public int Count
        {
            get
            {
                return items.Count;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderTask task { get; set; }


        IEnumerator<spiderTaskResultItem> IEnumerable<spiderTaskResultItem>.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }
    }

}