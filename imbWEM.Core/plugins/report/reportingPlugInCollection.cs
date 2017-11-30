// --------------------------------------------------------------------------------------------------------------------
// <copyright file="reportingPlugInCollection.cs" company="imbVeles" >
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
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
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.directReport.core;
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbAnalyticsEngine.spider.plugins.collections.APlugInCollectionBase{imbAnalyticsEngine.spider.engine.crawlReportingStageEnum, imbAnalyticsEngine.directReport.core.directReporterBase}" />
    public class reportingPlugInCollection : APlugInCollectionBaseCore, IAPlugInCollectionBase {



        public void eventDLCInitiated(object __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            foreach (IPlugInCommonBase<crawlReportingStageEnum, directReporterBase> plug in allPlugins)
            {
                plug.eventDLCInitiated(__parent as directReporterBase, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventDLCFinished(object __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            foreach (IPlugInCommonBase<crawlReportingStageEnum, directReporterBase> plug in allPlugins)
            {
                plug.eventDLCFinished(__parent as directReporterBase, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {
            foreach (IPlugInCommonBase<crawlReportingStageEnum, directReporterBase> plug in allPlugins)
            {
                plug.eventCrawlJobFinished(aJob, __machine, __tRecord);
            }
        }



        public directReporterBase __reporter { get; set; }

        public crawlerDomainTaskMachine __engine { get; set; }

        public List<IReportPlugin> reportPlugins { get; set; } = new List<IReportPlugin>();

        /// <summary>
        /// Installs the plug in.
        /// </summary>
        /// <param name="plug">The plug.</param>
        public void installPlugIn(object plug)
        {
            allPlugins.Add(plug as IPlugInCommonBase);

            if (plug == null) throw new aceGeneralException("Supplied plugin is null", null, this, "Plug Install error");

            foreach (Enum point in ((IPlugInCommonBase)plug).INSTALL_POINTS)
            {
                plugins.Add(point, ((IPlugInCommonBase)plug));


            }



            ((IPlugInCommonBase)plug).eventPluginInstalled();

        }

        public reportingPlugInCollection(directReporterBase __parent, crawlerDomainTaskMachine __machine) : base(__parent)
        {
            __reporter = __parent;
            __engine = __machine;
            parent = __parent;

        }
        
        public override void Dispose()
        {
            
        }

        //  public void reportCrawler(modelSpiderTestRecord tRecord) => eventUniversal<crawlerDomainTask, modelSpiderTestRecord>(crawlReportingStageEnum.crawler, __reporter, null, tRecord);


        //  public void reportDomainFinished(modelSpiderSiteRecord wRecord) => eventUniversal<crawlerDomainTask, modelSpiderSiteRecord>(crawlReportingStageEnum.domain, __reporter, null, wRecord);


        //   public void reportIteration(dataUnitSpiderIteration dataUnit, modelSpiderSiteRecord wRecord, ISpiderEvaluatorBase evaluator) => eventUniversal<dataUnitSpiderIteration, modelSpiderSiteRecord>(crawlReportingStageEnum.iteration, __reporter, dataUnit, wRecord);


        public void eventDLCFinished(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            foreach (IPlugInCommonBase<crawlReportingStageEnum, directReporterBase> plug in allPlugins)
            {
                plug.eventDLCFinished(null, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventDLCInitiated(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord) {
            foreach (IPlugInCommonBase<crawlReportingStageEnum, directReporterBase> plug in allPlugins)
            {
                plug.eventDLCInitiated(null, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventIteration(ISpiderEvaluatorBase __spider, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            if (!IsEnabled) return;

            foreach (reportPlugIn_base plug in allPlugins)
            {
                try
                {
                    plug.eventIteration(__spider, __task, __wRecord);
                    
                    
                }
                catch (Exception ex)
                {
                    aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at status report execution crashed: " + ex.Message);
                    crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                    cel.SaveXML();
                }
            }
        }

        public void eventCrawlJobFinished(crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord) // <---- ovaj se nikad ne poziva
        {
            foreach (reportPlugIn_base plug in allPlugins)
            {
                try
                {
                    plug.eventCrawlJobFinished(__machine, __tRecord);

                    
                }
                catch (Exception ex)
                {
                    aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at status report execution crashed: " + ex.Message);
                    crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                    cel.SaveXML();
                }
            }
        }


        public void eventStatusReport(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            if (!IsEnabled) return;

            foreach (reportPlugIn_base plug in allPlugins)
            {
                try
                {
                    plug.eventStatusReport(crawlerDomainTaskMachine, tRecord);
                    //if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                    
                }
                catch (Exception ex)
                {
                    aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at status report execution crashed: " + ex.Message);
                    crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                    cel.SaveXML();
                }
            }
        }

        public void eventUniversal(crawlReportingStageEnum stage, directReporterBase __parent, crawlerDomainTask __task, modelSpiderSiteRecord wRecord)
        {
            //if (!IsEnabled) return;

            if (plugins[stage].Any(x => x.IsEnabled))
            {
                foreach (reportPlugIn_base plug in allPlugins)
                {
                    try
                    {
                        switch (stage)
                        {
                            case crawlReportingStageEnum.domain:
                                plug.eventDLCFinished(__parent, __task, wRecord);
                                break;
                            case crawlReportingStageEnum.init:
                                plug.eventDLCInitiated(__parent, __task, wRecord);
                                break;
                            default:
                                plug.eventUniversal(stage, __parent, __task, wRecord);
                                break;
                        }
                        
                        //if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                        
                    }
                    catch (Exception ex)
                    {
                        aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at " + stage.ToString() + " execution crashed: " + ex.Message);
                        crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                        cel.SaveXML();
                    }
                }
            } else
            {

            }

        }

        public override bool IsEnabled
        {
            get
            {
                return true;
            }

            set
            {
                
            }
        }




        public void eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            if (!IsEnabled) return;

            foreach (reportPlugIn_base plug in allPlugins)
            {
                try
                {
                    plug.eventAtInitiationOfCrawlJob(crawlerDomainTaskMachine, tRecord);
                    //if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                    
                }
                catch (Exception ex)
                {
                    aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at status report execution crashed: " + ex.Message);
                    crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                    cel.SaveXML();
                }
            }
        }

        public void eventAtEndOfCrawlJob(crawlerDomainTaskMachine crawlerDomainTaskMachine, modelSpiderTestRecord tRecord)
        {
            if (!IsEnabled) return;

            foreach (reportPlugIn_base plug in allPlugins)
            {
                try
                {
                    plug.eventAtEndOfCrawlJob(crawlerDomainTaskMachine, tRecord);
                    //if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                 
                }
                catch (Exception ex)
                {
                    aceLog.log("Reporting Plugin [" + plug.name + "]:" + plug.GetType().Name + " at status report execution crashed: " + ex.Message);
                    crawlerErrorLog cel = new crawlerErrorLog(ex, null, null, crawlerErrorEnum.indexPlugin);
                    cel.SaveXML();
                }
            }
        }

        public bool Equals(reportingPlugInCollection other)
        {
            throw new NotImplementedException();
        }
    }

}