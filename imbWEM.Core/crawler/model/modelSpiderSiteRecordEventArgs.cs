// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelSpiderSiteRecordEventArgs.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using HtmlAgilityPack;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webStructure;
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Objekat koji opisuje dogadjaj koji se desio objektu: modelSpiderSiteRecord
    /// </summary>
    public class modelSpiderSiteRecordEventArgs
    {
        public modelSpiderSiteRecordEventArgs()
        {
        }

        public modelSpiderSiteRecordEventArgs(ISpiderTarget __target, modelSpiderSiteRecordEventType __type = modelSpiderSiteRecordEventType.DLCTargetPageAttached)
        {
            Target = __target;
            if (__target is spiderTarget)
            {
                spiderTarget target = (spiderTarget) __target;
                sourceHtml = target.page.webpage.result.sourceCode;
                htmlDoc = target.page.webpage.result.HtmlDocument; //(HtmlDocument)target.page.webpage.result.document.getDocument<HtmlDocument>();
                sourceXml = htmlDoc.DocumentNode.OuterHtml;

                //sourceXml = target.page.spiderResult.page.result.document.getDocument<HtmlDocument>()
            }
            type = __type;
        }

        public modelSpiderSiteRecordEventArgs(spiderTaskResult __result, modelSpiderSiteRecordEventType __type = modelSpiderSiteRecordEventType.DLCTaskProcessed)
        {
            type = __type;
            LoadResult = __result;
        }

        public modelSpiderSiteRecordEventArgs(modelSpiderSiteRecordEventType __type, string __message = "")
        {
            message = __message;
            type = __type;
        }

        public string message { get; set; }


        /// <summary>
        /// Reference to HTML Document
        /// </summary>
        /// <value>
        /// The HTML document.
        /// </value>
        public HtmlDocument htmlDoc { get; set; }

        public string sourceHtml { get; set; }

        public string sourceXml { get; set; }

        public ISpiderTarget Target { get; set; }

        public spiderTaskResult LoadResult { get; set; }

        public modelSpiderSiteRecordEventType _type;
        /// <summary>
        /// Interfejs prema tipu dogadjaja
        /// </summary>
        public modelSpiderSiteRecordEventType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

    }
}