// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerErrorLog.cs" company="imbVeles" >
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

// using imbAnalyticsEngine.webSiteComplexCrawler;

namespace imbWEM.Core.crawler.engine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.extensions.io;
    using imbACE.Core.operations;
    using imbACE.Network.extensions;
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
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums;
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

    public class crawlerErrorLog : aceExceptionInfoBase, IModelRecordSerializable, ICrawlerDomainTaskSerializable
    {
        /// <summary>
        /// Saves the XML into specified path or default diagnostic location
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Full filepath of the saved XML or empty string if failed</returns>
        public override string SaveXML(string path = null)
        {
            if (path.isNullOrEmpty())
            {
                path = "diagnostic";
            }

            path = path.add(GetFilename(), "\\");

            string xml = objectSerialization.ObjectToXML(this);

            FileInfo fi = path.getWritableFile();

            if (fi.FullName.saveToFile(xml))
            {
                return fi.FullName;
            }
            else
            {
                return "";
            }
        }

        public static crawlerErrorLog FirstError;

        /// <summary>
        /// IF TRUE it will throw exception
        /// </summary>
        public static bool throwException = false;

        public static crawlerErrorLog CreateAndSave(Exception __axe, IModelRecord __relevantRecord = null, crawlerDomainTask __task = null, crawlerErrorEnum __errorType = crawlerErrorEnum.none)
        {
            crawlerErrorLog output = new crawlerErrorLog(__axe, __relevantRecord, __task, __errorType,1);

            if (FirstError == null)
            {
                FirstError = output;

            }

            Console.WriteLine("Error [" + output.Title + "]");
           // Console.WriteLine("Message [" + output.Message + "]");
           // Console.WriteLine("Spec.msg. [" + output.specificMessage + "]");

            string path = output.SaveXML();

            Console.WriteLine("Crawler error log saved to: " + path);

            if (FirstError == output)
            {
                if (imbWEMManager.settings.executionLog.doOpenFirstException)
                {
                    externalToolExtensions.run(externalTool.notepadpp, path);
                }
            }

            if (imbWEMManager.settings.executionLog.doThrowDLCException)
            {
                throw __axe;
            }

            return output;
        }
        /// <summary>
        /// don-t use, this is just for serialization
        /// </summary>
        public crawlerErrorLog()
        {

        }

        public crawlerErrorLog(Exception __axe, IModelRecord __relevantRecord = null, crawlerDomainTask __task = null, crawlerErrorEnum __errorType = crawlerErrorEnum.none, int stacks=1)
        {
            deploy(__axe, __errorType, "", __task, __relevantRecord, __task, stacks+1);
        }



        /// <summary>
        /// Gets the filename 
        /// </summary>
        /// <returns></returns>
        public override string GetFilename()
        {

            string output = "secondary_error_";

            if (FirstError == this)
            {
                output = "primary_error_";
            }

            if (isThreadCancelError)
            {
                output = "thread_cancel_";
            }

            if (task != null)
            {
                //output = output.add(TaskJobName, "_");
                output = output.add(TaskCrawlerName, "_");
                output = output.add(TaskRunStamp, "_");
                output = output.add(TaskDomainName, "_");

            } else if (relevantRecord != null)
            {
                output = output.add(modelClassName, "_");
                output = output.add(modelInstanceID, "_");
                output = output.add(modelInstanceID, "_");
            }

            string timestamp = time.ToShortDateString() + time.ToShortTimeString();

            output = output + timestamp;

            output = output.getCleanFileName();

            output = output.add("xml", ".");

            return output;
        }


        #region ----------- Boolean [ isThreadCancelError ] -------  [IsThreadError]

        /// <summary>
        /// IsThreadError
        /// </summary>
        [Category("Switches")]
        [DisplayName("isThreadCancelError")]
        [Description("IsThreadError")]
        public bool isThreadCancelError { get; set; } = false;

        #endregion


        protected void deploy(Exception __axe, crawlerErrorEnum __errorType, string __specificMessage, object __relevantInstance, IModelRecord __relevantRecord, crawlerDomainTask __task = null, int stacks=1)
        {
            specificMessage = __specificMessage;
            relevantRecord = __relevantRecord;
            if (relevantRecord == null)
            {
                if (__relevantInstance is IModelRecord)
                {
                    relevantRecord = (IModelRecord)__relevantInstance;
                }
            }

            if (__errorType == crawlerErrorEnum.none) __errorType = crawlerErrorEnum.exceptionError;

            if (__axe != null)
            {

                if (__axe is aceGeneralException)
                {
                    axe = (aceGeneralException)__axe;
                    axe.SetLogSerializable(this);
                }
                else if (__axe is Exception)
                {
                    if (__axe.Message.Contains("thread"))
                    {
                        isThreadCancelError = true;
                    }
                    axe = new aceGeneralException(__axe.Message, __axe, __relevantInstance, "Crawler error: " + __errorType.ToString(), stacks + 2);
                    axe.SetLogSerializable(this);
                } else 
                {
                    axe = null;
                }

                
            }


            if (__relevantRecord != null)
            {
                relevantRecord = relevantRecord;
               // relevantRecord.SetLogSerializable(this);
            }

            if (__task != null)
            {
                task = __task;
                __task.SetLogSerializable(this);
                __errorType |= crawlerErrorEnum.domainTaskError;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string specificMessage { get; set; }


        /// <summary>
        /// Referenca prema crawlerDomainTask-u
        /// </summary>
        protected crawlerDomainTask task { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected IModelRecord relevantRecord { get; set; }


        /// <summary>
        /// Ref. ka ACE exceptionu
        /// </summary>
        protected aceGeneralException axe { get; set; }

        public string modelClassName { get; set; }

        public string modelUID { get; set; }

        public string modelInstanceID { get; set; }

        public string modelRunStamp { get; set; }

        public string modelLogContent { get; set; }

        public int modelIndexCurrent { get; set; }

        public DateTime modelTimeStart { get; set; }

        public DateTime modelTimeFinish { get; set; }

        public string modelNote { get; set; }

        public modelRecordStateEnum modelState { get; set; }

        public modelRecordRemarkFlags modelRemarkFlags { get; set; }

        public string modelDataFieldDamp { get; set; }

        public string modelStartingThread { get; set; }

        public string TaskExecutionThreadName { get; set; }

        public bool TaskIsStageAborted { get; set; }

        public DateTime TaskStartTime { get; set; }

        public int TaskTimeLimitForOneItemLoad { get; set; }

        public int TaskTargetLoaded { get; set; }

        public int TaskTargetDetected { get; set; }

        public crawlerDomainTaskStatusEnum TaskStatus { get; set; }

        public string TaskDomainName { get; set; }

        public string TaskCrawlerName { get; set; }

        public string TaskRunStamp { get; set; }

        public string TaskJobName { get; set; }

        public DateTime TaskLastIteration { get; set; }

        public bool TaskIsActive { get; set; }

        public crawlerDomainTaskIterationPhase TaskIterationStatus {get;set;}
    }

}