// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderStageBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.stage
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
    using imbSCI.Core.extensions.typeworks;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;

    /// <summary>
    /// Describes one loop or part of workflow
    /// </summary>
    /// <seealso cref="aceCommonTypes.interfaces.IObjectWithNameAndDescription" />
    public abstract class spiderStageBase:IObjectWithNameAndDescription
    {

        

       

        internal void prepare()
        {
            //if (stageIteration > 1)
            //{
            //    Exception ex = new aceGeneralException("spiderStage[" + name + "]:[" + GetType().Name + "] received unwanted " + nameof(prepare) + " call since its iteration count is [" + stageIteration + "]");
            //    throw ex;
            //} 
            stageIteration = 1;
            foreach (spiderObjective obj in objectives)
            {
                obj.prepare();
            }
        }


        /// <summary>
        /// The global stage iteration limit
        /// </summary>
        public const int GLOBAL_stageIterationLimit = 1000;


        /// <summary>
        /// 
        /// </summary>
        public int stageIterationLimit { get; set; } = 500;


        /// <summary>
        /// 
        /// </summary>
        public int stageIteration { get; set; } = -1;

        /// <summary>
        /// Clones this instance of stage 
        /// </summary>
        /// <returns></returns>
        public spiderStageBase Clone()
        {
            spiderStageBase output = GetType().getInstance() as spiderStageBase;
            foreach (spiderObjective objective in objectives)
            {
                output.AddObjective(objective.name, objective.description, objective.tag);
            }
            return output;
        }

        protected spiderStageBase()
        {

        }
        public spiderStageBase(string __name, string __description)
        {
            name = __name;
            description = __description;
        }

        public spiderObjective AddObjective(string __name, string __description, spiderObjectiveEnum __tag)
        {
            spiderObjective obj = new spiderObjective();
            obj.name = __name;
            obj.description = __description;
            obj.codename = __tag.ToString();
            obj.tag = __tag;
            obj.supervisor = spiderObjectiveModeEnum.controlRule;
            objectives.Add(obj);
            return obj;
        }


        public void EnterStage(modelSpiderSiteRecord wRecord, ISpiderEvaluatorBase sEvaluator)
        {
            wRecord.logBuilder.log("-- entering stage [" + name + "] with " + objectives.Count() + " objectives.");
            wRecord.logBuilder.log("> " + description + " (codename:" + codename + ")");
            wRecord.logBuilder.log("> stage iteration limit: " + stageIterationLimit + " (global limit:" + GLOBAL_stageIterationLimit + ")");

            foreach (spiderObjective objective in objectives)
            {
                objective.prepare();
                wRecord.logBuilder.log("> Objective [" + objective.name + "] t:" + objective.supervisor + " ");
            }
        }

        public bool CheckStage(modelSpiderSiteRecord wRecord, spiderObjectiveSolutionSet oSet, spiderTask task)
        {
            bool okToLeave = false;

            if (task.Count() == 0)
            {
                wRecord.logBuilder.log("> Spider task [i:" + task.iteration + "] have no tasks defined. Aborting the stage loop.");
                okToLeave = true;
                return okToLeave;
            }

            // <----------------------------- OBJECTIVE SOLUTION SET
            okToLeave = operation_executeObjectiveSolutionSet(oSet, wRecord);
            if (okToLeave) return okToLeave;

            // <----------------------------- SPIDER LIMITS OVERRIDERS ---------------|
            if (stageIteration > wRecord.tRecord.instance.settings.limitIterations)
            {
                wRecord.log("> Spider settings (limit iterations) trigered abort at [" + stageIteration + "] Aborting the stage loop.");
                okToLeave = true;
                return okToLeave;
            }
            // <----------------------------------------------------------------------|

            // <----------------------------- SPIDER LIMITS OVERRIDERS ---------------|
            if (wRecord.web.webPages.Count() > wRecord.tRecord.instance.settings.limitTotalPageLoad)
            {
                wRecord.log("> Spider settings (limit pages load) trigered abort at [" + wRecord.web.webPages.Count() + "] Aborting the stage loop.");
                okToLeave = true;
                return okToLeave;
            }
            // <----------------------------------------------------------------------|




            if (stageIteration > stageIterationLimit)
            {
                wRecord.logBuilder.log("> Stage [" + name + "] iteration limit reached [ " + stageIterationLimit + " ] -- aborting [" + objectives.Count + "] objectives and move on");
                okToLeave = true;
                return okToLeave;
            }
            
            if (stageIteration > GLOBAL_stageIterationLimit)
            {
                Exception ex = new aceGeneralException("spiderStage [" + name + "] reached the " + nameof(GLOBAL_stageIterationLimit) + "(" + GLOBAL_stageIterationLimit.ToString() + ")");
                throw ex;
            }

            stageIteration++;

           

            return okToLeave;
        }

        protected void resolveObjective(spiderObjective obj, spiderObjectiveSolution sol, modelSpiderSiteRecord wRecord)
        {
            switch (sol.status)
            {
                case spiderObjectiveStatus.notSolved:
                    break;
                case spiderObjectiveStatus.avoided:
                case spiderObjectiveStatus.aborted:
                    obj.status = sol.status;
                    wRecord.logBuilder.log("Objective [" + obj.name + "] updated status: " + obj.status);
                    aceTerminalInput.doBeepViaConsole(1200, 100, 2);
                    break;
                case spiderObjectiveStatus.solved:
                    obj.status = sol.status;
                    wRecord.logBuilder.log("Objective [" + obj.name + "] updated status: " + obj.status);
                   aceTerminalInput.doBeepViaConsole(1600, 100, 2);
                    break;
            }
        }


        public bool operation_executeObjectiveSolutionSet(spiderObjectiveSolutionSet solutionSet, modelSpiderSiteRecord wRecord)
        {
            bool leave = false;
            foreach (spiderObjectiveSolution sol in solutionSet)
            {
                if (sol.status != spiderObjectiveStatus.unknown)
                {
                    switch (sol.type)
                    {
                        case spiderObjectiveType.dropOutControl:

                            break;
                        case spiderObjectiveType.flowControl:

                            List<spiderObjectiveEnum> targets = sol.targetedObjective.getEnumListFromFlags<spiderObjectiveEnum>();
                            foreach (spiderObjectiveEnum target in targets)
                            {
                                spiderObjective selObjective = objectives.FirstOrDefault<spiderObjective>(x => x.tag == target);
                                if (selObjective != null)
                                {
                                    resolveObjective(selObjective, sol, wRecord);
                                }
                            }
                            break;
                    }
                }
            }

            leave = true;

            foreach (spiderObjective obj in objectives)
            {
                if (obj.status == spiderObjectiveStatus.notSolved) leave = false;
            }
            if (leave)
            {
                wRecord.logBuilder.log("All objectives solved/avoided/aborted. Leaving the stage [" + name + "]");
            }

            return leave;
        }


        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description { get; set; } = "";


        /// <summary>
        /// 
        /// </summary>
        public string codename { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected List<spiderObjective> objectives { get; set; } = new List<spiderObjective>();

        public spiderStageControl stageControl { get; internal set; }
    }
}
