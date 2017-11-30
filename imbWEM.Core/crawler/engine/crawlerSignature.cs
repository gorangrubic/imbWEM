// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerSignature.cs" company="imbVeles" >
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

// using imbAnalyticsEngine.webSiteComplexCrawler;

namespace imbWEM.Core.crawler.engine
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
    using imbSCI.Core.data;
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
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class crawlerSignature:imbBindable
    {
        public crawlerSignature()
        {

        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="insertValues">if set to <c>true</c> [insert values].</param>
        /// <returns></returns>
        public DataTable GetDataTable(bool insertValues, string nameOverride = "")
        {

            DataTable performanceTable = new DataTable("sgn_" + nameOverride);
            if (!nameOverride.isNullOrEmpty())
            {
                performanceTable.SetTitle(nameOverride);
            }

            // performanceTable.Add(nameof(performanceRecord.crawlerName));
            performanceTable.AddColumns(GetType(), nameof(name), nameof(description), nameof(reportFolder), nameof(slot), nameof(className),
                nameof(spiderSettings.limitIterationNewLinks), nameof(spiderSettings.limitIterations), nameof(spiderSettings.limitTotalLinks), nameof(spiderSettings.limitTotalPageLoad));

            
            if (insertValues)
            {
                performanceTable.AddObject(this);

            }

            return performanceTable;
        }


        /*
        public Int32 limitIterationNewLinks => settings.limitIterationNewLinks;
        public Int32 limitIterations => settings.limitIterations;
        public Int32 limitTotalLinks => settings.limitTotalLinks;
        public Int32 limitTotalPageLoad => settings.limitTotalPageLoad;
        */


        #region -----------  limitTotalPageLoad  -------  [count / ratio]
        private int _limitTotalPageLoad = 0; // = new Int32();
                                      /// <summary>
                                      /// count / ratio
                                      /// </summary>
        // [XmlIgnore]
        [Category("Counters")]
        [DisplayName("limitTotalPageLoad")]
        [Description("count / ratio")]
        public int limitTotalPageLoad
        {
            get
            {
                return settings.limitTotalPageLoad;
            }
            set
            {
                settings.limitTotalPageLoad = value;
                OnPropertyChanged("limitTotalPageLoad");
            }
        }
        #endregion


        #region -----------  limitIterationNewLinks  -------  [count / ratio]
        private int _limitIterationNewLinks = 0; // = new Int32();
                                      /// <summary>
                                      /// count / ratio
                                      /// </summary>
        // [XmlIgnore]
        [Category("Counters")]
        [DisplayName("limitIterationNewLinks")]
        [Description("count / ratio")]
        public int limitIterationNewLinks
        {
            get
            {
                return settings.limitIterationNewLinks;
            }
            set
            {
                settings.limitIterationNewLinks = value;
                OnPropertyChanged("limitIterationNewLinks");
            }
        }
        #endregion

        #region -----------  limitIterations  -------  [count / ratio]
        private int _limitIterations = 0; // = new Int32();
                                      /// <summary>
                                      /// count / ratio
                                      /// </summary>
        // [XmlIgnore]
        [Category("Counters")]
        [DisplayName("limitIterations")]
        [Description("count / ratio")]
        public int limitIterations
        {
            get
            {
                return settings.limitIterations;
            }
            set
            {
                settings.limitIterations = value;
                
            }
        }
        #endregion



        #region -----------  limitTotalLinks  -------  [count / ratio]
        private int _limitTotalLinks = 0; // = new Int32();
                                      /// <summary>
                                      /// count / ratio
                                      /// </summary>
        // [XmlIgnore]
        [Category("Counters")]
        [DisplayName("limitTotalLinks")]
        [Description("count / ratio")]
        public int limitTotalLinks
        {
            get
            {
                return settings.limitTotalLinks;
            }
            set
            {
                settings.limitTotalLinks = value;
            }
        }
        #endregion




        public DataRow SetDataRow(DataTable table, DataRow row = null)
        {
            bool insertNew = false;
            if (row == null)
            {
                insertNew = true;
            }

            if (insertNew) row = table.NewRow();


            row.SetData(this);

            if (insertNew) table.Rows.Add(row);
            return row;
        }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string reportFolder { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int slot { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string className { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public spiderSettings settings { get; set; } = new spiderSettings();


        /// <summary>
        /// 
        /// </summary>
        public aceAuthorNotation notation { get; set; } = new aceAuthorNotation();


        public void deployReport(modelSpiderTestRecord tRecord)
        {
            className = tRecord.instance.GetType().Name;
            name = tRecord.instance.name;
            description = tRecord.instance.description;
            reportFolder = tRecord.reporter.folder.name;
            settings = tRecord.instance.settings;
            slot = tRecord.aRecord.GetChildRecords().IndexOf(tRecord);
        }


        public void deployTaskMachine(crawlerDomainTaskMachine cDTM)
        {
            TimeForObligatoryReport = cDTM.TimeForObligatoryReport;
            TimeLimitForCompleteJob = cDTM.TimeLimitForCompleteJob;
            TimeLimitForTask = cDTM.TimeLimitForTask;
            TimeLimitForDomainCrawl = cDTM._timeLimitForDLC;
            LoadForMemoryFlush = cDTM.LoadForMemoryFlush;

            
        }


        /// <summary>
        /// 
        /// </summary>
        public int TimeLimitForDomainCrawl { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public int TimeLimitForTask { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public double TimeForObligatoryReport { get; set; } = 0;


        /// <summary>
        /// Time limit in minutes
        /// </summary>
        public int TimeLimitForCompleteJob { get; set; } = 120;


        /// <summary>
        /// 
        /// </summary>
        public int LoadForMemoryFlush { get; set; } = 0;
    }

}