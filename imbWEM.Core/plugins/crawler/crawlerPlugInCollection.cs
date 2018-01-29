// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerPlugInCollection.cs" company="imbVeles" >
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
namespace imbWEM.Core.plugins.crawler
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.reporting.dataUnits;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.plugins.core;
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    public class crawlerPlugInCollection:APlugInCollectionBase<crawlerDomainTaskIterationPhase, spiderEvaluatorBase>, IAPlugInCollectionBase
    {
        public void eventDLCInitiated(object __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            foreach (IPlugInCommonBase<crawlerDomainTaskIterationPhase, spiderEvaluatorBase> plug in allPlugins)
            {
               plug.eventDLCInitiated(__parent as spiderEvaluatorBase, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventDLCFinished(object __parent, crawlerDomainTask __task, modelSpiderSiteRecord __wRecord)
        {
            foreach (IPlugInCommonBase<crawlerDomainTaskIterationPhase, spiderEvaluatorBase> plug in allPlugins)
            {
                plug.eventDLCFinished(__parent as spiderEvaluatorBase, __task, __wRecord); /// aJob, __machine, __tRecord);
            }
        }

        public void eventCrawlJobFinished(analyticJob aJob, crawlerDomainTaskMachine __machine, modelSpiderTestRecord __tRecord)
        {
            foreach (IPlugInCommonBase<crawlerDomainTaskIterationPhase, spiderEvaluatorBase> plug in allPlugins)
            {
                plug.eventCrawlJobFinished(aJob, __machine, __tRecord);
            }
        }

        public crawlerPlugInCollection(spiderEvaluatorBase __crawler):base(__crawler)
        {

        }

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

        public override void eventUniversal<TFirst, TSecond>(crawlerDomainTaskIterationPhase stage, spiderEvaluatorBase __parent, TFirst __resource, TSecond __resourceSecond)
        {
            
        }

        /// <summary>
        /// Processes the universal call.
        /// </summary>
        /// <param name="phase">The phase.</param>
        /// <param name="wRecord">The w record.</param>
        /// <param name="wTask">The w task.</param>
        /// <param name="sResult">The s result.</param>
        /// <param name="tRecord">The t record.</param>
        /// <param name="dataUnit">The data unit.</param>
        protected void processUniversalCall(crawlerDomainTaskIterationPhase phase, modelSpiderSiteRecord wRecord, crawlerDomainTask wTask, spiderTaskResult sResult = null,
            modelSpiderTestRecord tRecord = null, dataUnitSpiderIteration dataUnit = null)
        {
            if (!IsEnabled) return;

            if (plugins[phase].Any(x => x.IsEnabled))
            {
                foreach (ISpiderPlugIn plug in plugins[phase])
                {
                    try
                    {
                        switch (phase)
                        {
                            case crawlerDomainTaskIterationPhase.applyLinkRules:
                                if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                                break;
                            case crawlerDomainTaskIterationPhase.checkingRules:

                                break;
                            case crawlerDomainTaskIterationPhase.getLoadTask:
                                if (plug is ISpiderPlugInForContentPostprocess) ((ISpiderPlugInForContentPostprocess)plug).processEndOfIteration(wRecord, wTask);
                                break;
                            case crawlerDomainTaskIterationPhase.iterationProcessFinished:
                                break;
                            case crawlerDomainTaskIterationPhase.iterationProcessInit:
                                break;
                            case crawlerDomainTaskIterationPhase.iterationProcessNotStarted:
                                break;
                            case crawlerDomainTaskIterationPhase.iterationStart:
                                break;
                            case crawlerDomainTaskIterationPhase.loadingSeedPage:
                                break;
                            case crawlerDomainTaskIterationPhase.loadingTask:
                                break;
                            case crawlerDomainTaskIterationPhase.none:
                                break;
                            case crawlerDomainTaskIterationPhase.pageEvaluation:
                                if (plug is ISpiderPlugInForContentPostprocess) ((ISpiderPlugInForContentPostprocess)plug).processAtDLCFinished(wRecord, wTask);
                                break;
                            case crawlerDomainTaskIterationPhase.receiveResult:
                                if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processLoaderResult(sResult, wRecord, wTask);
                                break;
                            case crawlerDomainTaskIterationPhase.updatingData:
                                break;
                        }
                        //if (plug is ISpiderPlugInForContent) ((ISpiderPlugInForContent)plug).processAfterResultReceived(wRecord, wTask);
                        
                    }
                    catch (Exception ex)
                    {
                        aceLog.log("Index Plugin [" + plug.name + "]:" + plug.GetType().Name + " at " + phase.ToString() + " execution crashed: " + ex.Message);
                        crawlerErrorLog cel = new crawlerErrorLog(ex, wRecord, wTask, crawlerErrorEnum.crawlerPlugin);
                        cel.SaveXML();
                    }

                   



                }
            }
        }

        public void processAfterResultReceived(modelSpiderSiteRecord wRecord, crawlerDomainTask wTask) => processUniversalCall(crawlerDomainTaskIterationPhase.applyLinkRules, wRecord, wTask);

        public void processAtDLCFinished(modelSpiderSiteRecord wRecord, crawlerDomainTask wTask) => processUniversalCall(crawlerDomainTaskIterationPhase.pageEvaluation, wRecord, wTask);

        public void processEndOfIteration(modelSpiderSiteRecord wRecord, crawlerDomainTask wTask) => processUniversalCall(crawlerDomainTaskIterationPhase.getLoadTask, wRecord, wTask);
        
        public void processLoaderResult(spiderTaskResult sResult, modelSpiderSiteRecord wRecord, crawlerDomainTask wTask) => processUniversalCall(crawlerDomainTaskIterationPhase.receiveResult, wRecord, wTask, sResult);

       
    }

}