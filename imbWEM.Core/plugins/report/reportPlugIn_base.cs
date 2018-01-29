// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportPlugIn_base.cs" company="imbVeles" >
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
namespace imbWEM.Core.plugins.report
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
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.plugins.content;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.stage;

    public abstract class reportPlugIn_base: plugIn_base, IObjectWithNameAndDescription, IReportPlugin, IPlugInCommonBase<crawlReportingStageEnum, directReporterBase>
    {
        /// <summary>
        /// Full path to place where the plugin should keep it's table
        /// </summary>
        /// <value>
        /// The record path.
        /// </value>
        protected string __recordPath { get; set; }

        protected abstract string __homePath { get; }



        public reportPlugIn_base(string __name, string __description)
        {
            name = __name;
            description = __description;
            loger = new builderForLog();
            aceLog.consoleControl.setAsOutput(loger, name);
            homeFolder = new folderNode(__homePath, "Home folder of plugin: " + __name, "Internal data for pluting " + __name + ". " + __description);

        }

       

        

        public abstract void eventStatusReport(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord);

        public abstract void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord);

      //  public abstract void eventPluginInstalled(directReporterBase __spider);


        public abstract void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord);

        public abstract void eventDLCInitiated(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord);

        public abstract void eventDLCFinished(directReporterBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord);

        public abstract void eventCrawlJobFinished(crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord);

        public abstract void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord);
        



        /// <summary>
        /// Just before 
        /// </summary>
        /// <param name="crawlerDomainTaskMachine">The crawler domain task machine.</param>
        /// <param name="tRecord">The t record.</param>
        public abstract void eventAtEndOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord);

        public void eventUniversal<TFirst, TSecond>(crawlReportingStageEnum stage, directReporterBase __parent, TFirst __task, TSecond __resource) => eventUniversal(stage, __parent, __task as crawlerDomainTask, __resource as modelSpiderSiteRecord);


        /// <summary>
        /// Folder where the reports are stored
        /// </summary>
        /// <value>
        /// The report folder.
        /// </value>
        public folderNode reportFolder { get; protected set; }


        /// <summary>
        /// Home folder> shere it saves internal records
        /// </summary>
        /// <value>
        /// The home folder.
        /// </value>
        public folderNode homeFolder { get; protected set; }




        public builderForLog loger { get; set; }

      

        public plugInGroupEnum plugin_group
        {
            get
            {
                return plugInGroupEnum.report;
            }

        }
    }
}