// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpiderEvaluatorBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.evaluators
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
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins.crawler;
    using imbWEM.Core.stage;

    public interface ISpiderEvaluatorBase : IAppendDataFields, IObjectWithName, IObjectWithNameAndDescription
    {

        void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord);
        void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord);


        string crawlerHash { get; }
        string FullDescription { get; }


        crawlerPlugInCollection plugins { get; }

            spiderUnit parent { get; }
        /// <summary>
        /// Spider settings
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        spiderSettings settings { get; }

        /// <summary>
        /// Prepares all rules for new case
        /// </summary>
        void prepareAll();

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        PropertyCollectionExtended AppendDataFields(PropertyCollection data = null);

        /// <summary>
        /// E1: Operations the receive result.
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        dataUnitSpiderIteration operation_receiveResult(spiderTaskResult stResult, modelSpiderSiteRecord wRecord);

        /// <summary>
        /// E2: Applies passive link rules to new Active links
        /// </summary>
        /// <param name="wRecord">The s record.</param>
        spiderObjectiveSolutionSet operation_applyLinkRules(modelSpiderSiteRecord wRecord);

        /// <summary>
        /// E3: Performes ranking, selects the next task and drops links below 
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        spiderTask operation_GetLoadTask(modelSpiderSiteRecord wRecord);

        /// <summary>
        /// E4: Performes ranking, selects the next task and drops links below
        /// </summary>
        /// <param name="sRecord">The s record.</param>
        /// <returns></returns>
        void operation_detectCrossLinks(modelSpiderSiteRecord sRecord);

        /// <summary>
        /// E3: Performes ranking, selects the next task and drops links below 
        /// </summary>
        /// <param name="stResult">The st result.</param>
        /// <param name="wRecord">The s record.</param>
        List<spiderPage> operation_evaluatePages(modelSpiderSiteRecord wRecord);

        /// <summary>
        /// Approves the URL for futher consideration based on url string content
        /// </summary>
        /// <param name="ln">The link to check</param>
        /// <returns>TRUE if link seems to be ok, FALSE if link url is not accepted by <see cref="spiderSettings.urlBanNeedles"/> list</returns>
        bool approveUrl(link ln);

        /// <summary>
        /// Creates single web loading task
        /// </summary>
        /// <param name="lnk">The LNK.</param>
        /// <param name="sReport">The s report.</param>
        /// <param name="iteration">The iteration.</param>
        /// <returns></returns>
        spiderTask getSpiderSingleTask(spiderLink lnk, modelSpiderSiteRecord sReport, int iteration);

        /// <summary>
        /// Sets the start page and inicializes all rule sets
        /// </summary>
        /// <param name="rootUrl">The root URL.</param>
        /// <param name="web">The web.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        spiderWeb setStartPage(string rootUrl, spiderWeb web);

        /// <summary>
        /// Spider evaluator algorithm name
        /// </summary>
        string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string description { get; set; }

        /// <summary>
        /// filepath to external file with <c>about spider</c> text
        /// </summary>
        string aboutFilepath { get; }


        
        ISpiderEvaluatorBase Clone(ILogBuilder logBuilder);
    }

}