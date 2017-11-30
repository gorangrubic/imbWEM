// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frontierEntryCommonBase.cs" company="imbVeles" >
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
    using imbSCI.Core.math.aggregation;
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
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public abstract class frontierEntryCommonBase : imbBindable
    //    , IObjectTableEntry
    {

        protected frontierEntryCommonBase()
        {
            start = DateTime.Now;
        }

        [DisplayName("Entry ID")]
        [Category("Report")]
        [Description("Unique record ID")]
        [imb(imbAttributeName.collectionPrimaryKey)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        public string name { get; set; }

        [DisplayName("Rows")]
        [Category("Report")]
        [Description("Number of rows aggregated here")]
        [imb(imbAttributeName.measure_setUnit, "n of records")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        public int rowsAggregated { get; set; } = 1;


        [DisplayName("Iteration")]
        [Category("Execution")]
        [imb(imbAttributeName.measure_letter, "M_i")]
        [imb(imbAttributeName.measure_setUnit, "n-th iteration")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.firstEntry)]
        [imb(imbAttributeName.viewPriority, 1)]
        public int iteration { get; set; } = 0;

        [DisplayName("Start")]
        [Category("Execution")]
        [Description("Moment when this iteration started")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "Time")]
        [imb(imbAttributeName.reporting_valueformat, "T")]
        [imb(imbAttributeName.viewPriority, 2)]
        public DateTime start { get; set; } = DateTime.Now;
        

    }

}