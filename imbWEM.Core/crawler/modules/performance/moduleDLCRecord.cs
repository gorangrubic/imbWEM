// --------------------------------------------------------------------------------------------------------------------
// <copyright file="moduleDLCRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.modules.performance
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.implementation;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbSCI.DataComplex.tables.objectTable{T}.spider.modules.performance.moduleIterationRecord}" />
    public class moduleDLCRecord:objectTable<moduleIterationRecord>
    {
        protected spiderModuleBase module { get; set; }

        protected modelSpiderSiteRecord wRecord { get; set; }

        protected spiderModularEvaluatorBase spider { get; set; }


        
        public string jobName { get; set; }
        public string crawlerName { get; set; }
        public string domainName { get; set; }

        public string moduleName { get; set; }
        public string moduleType { get; set; }

        public Type moduleClass { get; set; }
        public moduleIterationRecordSummary moduleSummaryEnum { get; set; }

        //public Int32 moduleSlot { get; set; }


        //#region -----------  processCount  -------  [number of target processings]
        //private Int32 _processCount = 0; // = new Int32();
        //                              /// <summary>
        //                              /// number of target processings
        //                              /// </summary>
        //// [XmlIgnore]
        //[Category("Counters")]
        //[DisplayName("processCount")]
        //[Description("number of target processings")]
        //public Int32 processCount
        //{
        //    get
        //    {
        //        return _processCount;
        //    }
        //    set
        //    {
        //        _processCount = value;
        //        OnPropertyChanged("processCount");
        //    }
        //}
        //#endregion



        //#region -----------  singleOutput  -------  [only one output target]
        //private Int32 _singleOutput = 0; // = new Int32();
        //                              /// <summary>
        //                              /// only one output target
        //                              /// </summary>
        //// [XmlIgnore]
        //[Category("Counters")]
        //[DisplayName("singleOutput")]
        //[Description("only one output target")]
        //public Int32 singleOutput
        //{
        //    get
        //    {
        //        return _singleOutput;
        //    }
        //    set
        //    {
        //        _singleOutput = value;
        //        OnPropertyChanged("singleOutput");
        //    }
        //}
        //#endregion


        //#region -----------  moduleGaveUp  -------  [modul je odustao tokom evaluacije]
        //private Int32 _moduleGaveUp = 0; // = new Int32();
        //                              /// <summary>
        //                              /// modul je odustao tokom evaluacije
        //                              /// </summary>
        //// [XmlIgnore]
        //[Category("Counters")]
        //[DisplayName("moduleGaveUp")]
        //[Description("modul je odustao tokom evaluacije")]
        //public Int32 moduleGaveUp
        //{
        //    get
        //    {
        //        return _moduleGaveUp;
        //    }
        //    set
        //    {
        //        _moduleGaveUp = value;
        //        OnPropertyChanged("moduleGaveUp");
        //    }
        //}
        //#endregion


       


        public moduleDLCRecord():base("name", "module")
        {

        }


        public string GetPrimaryKey(int iteration)
        {
            return iteration.ToString("D3") + moduleName;
        }

        public moduleIterationRecord StartNewRecord(int iteration)
        {
            return GetOrCreate(GetPrimaryKey(iteration));
        }


        public void start(spiderModuleBase __module, modelSpiderSiteRecord __wRecord)
        {
            module = __module;
            
            wRecord = __wRecord;
            //spider = __spider;

            jobName = wRecord.tRecord.aJob.name;
            crawlerName = wRecord.tRecord.instance.name;
            domainName = wRecord.domain;

            name = module.name; //+ "_" + crawlerName + "_" + wRecord.domainInfo.domainRootName;
            table.TableName = name + "_" + wRecord.domainInfo.domainName;

            jobName = wRecord.tRecord.aJob.name;
            crawlerName = wRecord.tRecord.instance.name;
            domainName = wRecord.domain;

            moduleName = module.name;
            moduleType = module.GetType().BaseType.Name;

            moduleClass = module.GetType();

            if (moduleClass == typeof(languageModule)) moduleSummaryEnum = moduleIterationRecordSummary.language;
            if (moduleClass == typeof(templateModule)) moduleSummaryEnum = moduleIterationRecordSummary.template;
            if (moduleClass == typeof(structureModule)) moduleSummaryEnum = moduleIterationRecordSummary.structure;
            if (moduleClass == typeof(diversityModule)) moduleSummaryEnum = moduleIterationRecordSummary.diversity;
            
            //moduleSlot = spider.modules.IndexOf(module);
        }

        public void dispose()
        {
            module = null;
            wRecord = null;
            spider = null;
        }

      



        ///// <summary> Cumulative module FRA algorithm process duration </summary>
        //[Category("Time")]
        //[DisplayName("ProcessTime")]
        //[imb(imbAttributeName.measure_letter, "TM_sum")]
        //[imb(imbAttributeName.measure_setUnit, "s")]
        //[Description("Cumulative module FRA algorithm process duration")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        //public Double ProcessTime { get; set; } = 0;


        ///// <summary> Contained by modules </summary>
        //[Category("Count")]
        //[DisplayName("Targets in layers")]
        //[imb(imbAttributeName.measure_letter, "")]
        //[imb(imbAttributeName.measure_setUnit, "n")]
        //[Description("Contained by modules")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        //public Int32 TargetsInLayers { get; set; } = default(Int32);






        public moduleDLCRecord(string __keyProperty, string __tableName) : base("name", __tableName)
        {
        }

       
    }

}
