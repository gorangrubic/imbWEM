// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticMacroBase.cs" company="imbVeles" >
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
    using imbSCI.Reporting.meta.delivery;
    using imbSCI.Reporting.meta.documentSet;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    //public Enum analyticMacroDataEnum
    //{
    //    macro_displayname,
    //    macro_displaydesc,
    //    macro_displaycategory,
    //}


    /// <summary>
    /// Base class for <see cref="analyticMacroBase"/> instructions
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public abstract class analyticMacroBase :imbBindable
    {

        /// <summary>
        /// Helps the method build spiders.
        /// </summary>
        /// <param name="aJob">a job.</param>
        /// <param name="sciProject">The science project.</param>
        /// <returns></returns>
        public abstract List<ISpiderEvaluatorBase> helpMethodBuildSpiders(analyticJob aJob, analyticProject sciProject);


        public abstract spiderStageControl helpMethodBuildStageControl(analyticJobRecord aRecord, modelSpiderTestRecord spiderRecord);



        /// <summary>
        /// Executes the other common analysis required for report build. Updates the results to <see cref="analyticJobRecord"/>
        /// </summary>
        /// <seealso cref="metaDocumentRootSet"/>
        /// <seealso cref="imbSCI.Reporting.script.docScriptCompiled"/>
        /// <param name="aRecord">a record.</param>
        public abstract void executeOtherCommons(analyticJobRecord aRecord);

        /// <summary>
        /// Builds the global report on job executed using stored records and builds reports on commons
        /// </summary>
        /// <param name="aRecord">a record.</param>
        /// <returns></returns>
        public abstract metaDocumentRootSet executeBuildReport(analyticJobRecord aRecord);


        /// <summary>
        /// Defines deliveryInstance unit and calls construction 
        /// </summary>
        /// <param name="aReport">a report.</param>
        /// <returns></returns>
        public abstract deliveryInstance executeRenderReport(metaDocumentRootSet aReport, analyticJobRecord aRecord);

        /// <summary>
        /// Runs the macro. To adjust macro execution override <see cref="innerRun(analyticJob, analyticJobRunFlags, analyticProject, builderForLog)"/> method. 
        /// <c>innerRun</c> is called witnih this method, after common initalization procedure
        /// </summary>
        /// <remarks>
        /// <para>Report creation is done after <see cref="innerRun(analyticJob, analyticJobRunFlags, analyticProject, builderForLog)"/> call.</para>
        /// </remarks>
        /// <param name="aJob">a job.</param>
        /// <param name="aFlags">a flags.</param>
        /// <param name="aProject">a project.</param>
        /// <param name="aTerminal">a terminal.</param>
        /// <returns></returns>
        public deliveryInstance run(analyticJob aJob, analyticJobRunFlags aFlags, analyticProject aProject = null, builderForLog aTerminal = null)
        {
            string runstamp = aJob.runstamp;

            //aFlags = aFlags.SetFlag<analyticJobRunFlags>(analyticJobRunFlags.report_FolderPurge, imbWEMManager.settings.postReportEngine.reportPurgeFolder);

            //bool projectCreated = false;
            //if ((aProject == null) || aFlags.HasFlag(analyticJobRunFlags.setup_sciProjectFromPreset))
            //{
            //    //aProject = new analyticProject();
            //    //aProject.afterLoadDeploy();
            //    //projectCreated = true;
            //}

            //analyticJobRecord aRecord = new analyticJobRecord(aJob, aProject, aFlags);
            //aceLog.consoleControl.setAsOutput(aRecord, "aRecord");

            //// <---------- counting the sample
            //imbWEMManager.webProfileGroups.setGroupCounts(aProject.mainWebProfiler.webSiteProfiles);

            //if (projectCreated) aRecord.logBuilder.log("SciProject new instance (byFlag) ::" + aProject.GetType().Name + " with defaults"); // Analytic macro script [" + this.GetType().Name + "] execution started");

            //if (aTerminal != null)
            //{
            //    //aceLog.logBuilderRegistry.Add(logOutputSpecial.systemMainLog, aTerminal);
            //    logSystem.externalLoger = aTerminal;
            //}

            //// ----------------------------------------------------------------  INITIATION SECTION

            ////-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //if (!aFlags.HasFlag(analyticJobRunFlags.execution_skipTest))
            //{
            //    aRecord.logBuilder.open("tag", "Execution: " + GetType().Name, "the system initial self-configuration");

            //    ////// ---------------------------------------------------- INNER RUN CALL
            //    innerRun(aJob, aFlags, aProject, aRecord);
            //    ////// ---------------------------------------------------- INNER RUN CALL


            //    aRecord.logBuilder.close();
            //    ///// inner run called
            //} else
            //{
            //    aRecord.logBuilder.log("The macro script never executed :: " + analyticJobRunFlags.execution_skipTest + " default instance created");
            //}




            //// <---------- Record is finished
            //aRecord.recordFinish();

            //// ----------------------------------------------------------------  REPORTING SECTION


            //aRecord.logBuilder.log("Report construction initiated");
            //// -- create deliveryInstance

            //executeOtherCommons(aRecord);

            //metaDocumentRootSet aReport = executeBuildReport(aRecord);

            // deliveryInstance reportDeliveryInstance = executeRenderReport(aReport, aRecord);

            return null; // reportDeliveryInstance;
            
        }



        /// <summary>
        /// Override this method with the <see cref="analyticMacroBase"/> execution code. 
        /// </summary>
        /// <remarks>
        /// <para>It is called after initiation part from <see cref="run(analyticJob, analyticJobRunFlags, analyticProject, builderForLog)"/></para>
        /// <para>This method would never be called it <see cref="analyticJobRunFlags.execution_skipTest"/> is inside <see cref="aFlags"/></para>
        /// </remarks>
        /// <param name="aJob">a job.</param>
        /// <param name="aFlags">a flags.</param>
        /// <param name="aProject">a project.</param>
        /// <param name="aTerminal">a terminal.</param>
        /// <returns></returns>
        protected abstract bool innerRun(analyticJob aJob, analyticJobRunFlags aFlags, analyticProject aProject, analyticJobRecord aRecord);




    }

}