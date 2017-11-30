// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticJobRunFlags.cs" company="imbVeles" >
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

    //public enum analyticSampleCountFlags
    //{
    //    sampleSkip5
    //}


    //public static class analyticJobExtensions
    //{
    //    public static 
    //}
    [Flags]
    public enum analyticJobRunFlags
    {
        none = 0,
        /// <summary>
        /// Limits sample size to development take of 5 
        /// </summary>
        sample_devTake5 = 1,
        /// <summary>
        /// Limits sample size to development take of 10
        /// </summary>
        sample_devTake10 = 2,

        /// <summary>
        /// The sample dev take15
        /// </summary>
        sample_devTake15 = sample_devTake5 | sample_devTake10,

        /// <summary>
        /// Limits sample size to development take of 25
        /// </summary>
        sample_devTake25 = 4,

        /// <summary>
        /// The sample dev take40
        /// </summary>
        sample_devTake40 = sample_devTake15 | sample_devTake25,

        /// <summary>
        /// The sample random take - it will randomly pick sample entries
        /// </summary>
        sample_randomTake = 8,



        /// <summary>
        /// The system will use web cache for http requests
        /// </summary>
        enable_WebCache = 16,

        /// <summary>
        /// The system will use web structure models cache
        /// </summary>
        enable_WebStructureCache = 32,

        /// <summary>
        /// The system will use cache for NLP analysis results
        /// </summary>
        enable_NLPCache = 64,

        /// <summary>
        /// The system will create new temp instance of <see cref="analyticProject"/> with default settings
        /// </summary>
        setup_sciProjectFromPreset = 128,

        /// <summary>
        /// Purges all files from report output directory before report creation starts
        /// </summary>
        report_FolderPurge = 256,
        /// <summary>
        /// Skip test execution --- used for reporting module debug
        /// </summary>
        execution_skipTest = 512,

        build_deliveryMeta = 1024,

        sample_devTake2 = 2048,
        sample_devTake100 = 4096,
       
    }

}