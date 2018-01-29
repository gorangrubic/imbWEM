// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_workload_settings.cs" company="imbVeles" >
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
namespace imbWEM.Core.plugins
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

    public class reportPlugIn_workload_settings
    {
        public reportPlugIn_workload_settings()
        {

        }
        /// <summary> Macro sampling size - also defines the warming up period </summary>
        [Category("Count")]
        [DisplayName("MacroSampleSize")]
        [imb(imbAttributeName.measure_letter, "Sm_s")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Macro sampling size - also defines the warming up period")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int macroSampleSize { get; set; } = 4;

        [Category("Count")]
        [DisplayName("Warming up")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of ticks it waits before start recording the measured means")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int warmingUpTicks { get; set; } = 1;


        /// <summary> Ratio </summary>
        [Category("Trends")]
        [DisplayName("ZeroMargin")]
        [imb(imbAttributeName.measure_letter, "Sm_m")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Trend spear margin value around zero that is equalized to stable metrics")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double ZeroMargin { get; set; } = 0.05;




        /// <summary> Number of warninigs before calling JLC termination </summary>
        [Category("Count")]
        [DisplayName("TerminationWarningCount")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of warninigs before calling JLC termination")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int term_warningCount { get; set; } = 1;

        [Category("Termination")]
        [Description("Percentage of available memory at which it triggers JLC termination. Negative value disables the criterion")]
        [DisplayName("RAM Limit")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.measure_letter, "TER_mem")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        public double term_availableMemory { get; set; } = -0.1;


        /// <summary> If <c>true</c> it enables JLC termination once number of DLCs left in the cue is smaller than TC_max  </summary>
        [Category("Termination")]
        [DisplayName("JLC Tail")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> it enables JLC termination once number of DLCs left in the cue is smaller than TC_max ")]
        public bool term_JLCinTail { get; set; } = false;



        /// <summary> Termination criterion based on number of completed DLCs. When -1 the criterion is off </summary>
        [Category("Termination")]
        [DisplayName("DLC Target")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Termination criterion based on number of completed DLCs. When -1 the criterion is off")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int term_DLCFinished { get; set; } = -1;





        /// <summary> If <c>true</c> enables the parallelism step up procedure </summary>
        [Category("Multithreading")]
        [DisplayName("stepUp_enabled")]
        [imb(imbAttributeName.measure_letter, "")]
        [Description("If <c>true</c> enables the parallelism step up procedure")]
        public bool stepUp_enabled { get; set; } = false;



        /// <summary> Number of DLCs to be completed for switching to the next measurement group </summary>
        [Category("Multithreading")]
        [DisplayName("stepUp_DLCCount")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Number of DLCs to be completed for switching to the next measurement group")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int stepUp_DLCCount { get; set; } = 20;


        /// <summary> Initial value of the TC_max </summary>
        [Category("Multithreading")]
        [DisplayName("stepUp_start")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Initial value of the TC_max")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int stepUp_start { get; set; } = 2;


        /// <summary> TC_max change step for next measurement group </summary>
        [Category("Multithreading")]
        [DisplayName("stepUp_step")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("TC_max change step for next measurement group")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int stepUp_step { get; set; } = 2;


        




    }

}