// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTaskResultItem.cs" company="imbVeles" >
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.pageAnalytics.core;
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

    /// <summary>
    /// Result of spider task item
    /// </summary>
    public class spiderTaskResultItem
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="spiderTaskResultItem"/> class.
        /// </summary>
        /// <param name="__task">The task.</param>
        public spiderTaskResultItem(spiderLink __task)
        {
            target = __task;
            task = __task.link;
            startTime = DateTime.Now;

        }

        /// <summary>
        /// Finishes the result item
        /// </summary>
        /// <param name="__page">The page.</param>
        public void finish(crawledPage __page, int __iteration)
        {
            page = __page;
            status = page.status;
            duration = DateTime.Now.Subtract(startTime);
            sPage = new spiderPage(page, target.iterationDiscovery, __iteration); // wRecord.iteration);
            sPage.spiderResult = this;
        }


        /// <summary>
        /// 
        /// </summary>
        public pageStatus status { get; protected set; } = pageStatus.unknown;


        /// <summary>
        /// Time of constructor call
        /// </summary>
        public DateTime startTime { get; protected set; }


        /// <summary>
        /// Duration of this task execution
        /// </summary>
        public TimeSpan duration { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public spiderLink target { get; set; }


        /// <summary>
        /// Link to open
        /// </summary>
        public link task { get; protected set; }


        /// <summary> </summary>
        public spiderPage sPage { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public crawledPage page { get; protected set; }

        public void dispose()
        {
            if (!imbWEMManager.settings.executionLog.doPreserveWebDocument)
            {
                page.result.releaseDocumentFromMemory();
                
                
            }
        }
    }

}