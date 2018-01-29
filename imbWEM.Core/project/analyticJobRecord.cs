// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticJobRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.project.records
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
    using imbSCI.Core.collection.checkLists;
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
    // using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbReportingCore.data.modelRecords.modelRecordParentBase{imbAnalyticsEngine.project.analyticJob, imbAnalyticsEngine.spider.core.spiderEvaluatorBase, imbAnalyticsEngine.spider.model.modelSpiderTestRecord}" />
    /// <seealso cref="imbSCI.DataComplex.data.modelRecords.IModelParentRecord" />
    public class analyticJobRecord : modelRecordParentBase<analyticJob, ISpiderEvaluatorBase, modelSpiderTestRecord>, IModelParentRecord
    {

        
        #region <--------------------------- GLOBAL REGISTERS --------------------------------

       



        //private contentPageRegistry _tokenizedContentGlobalRegistar = new contentPageRegistry();
        ///// <summary> </summary>
        //public contentPageRegistry tokenizedContentGlobalRegistar
        //{
        //    get
        //    {
        //        return _tokenizedContentGlobalRegistar;
        //    }
        //    protected set
        //    {
        //        _tokenizedContentGlobalRegistar = value;
        //        OnPropertyChanged("tokenizedContent");
        //    }
        //}

        //private contentTreeGlobalCollection _contentTreeGlobalRegistar = new contentTreeGlobalCollection();
        ///// <summary> </summary>
        //public contentTreeGlobalCollection contentTreeGlobalRegistar
        //{
        //    get
        //    {
        //        return _contentTreeGlobalRegistar;
        //    }
        //    protected set
        //    {
        //        _contentTreeGlobalRegistar = value;
        //        OnPropertyChanged("contentTreeGlobalRegistar");
        //    }
        //}

        



        public override bool VAR_AllowAutoOutputToConsole
        {
            get
            {
                return true;
            }
        }

        public override string VAR_LogPrefix
        {
            get
            {
                return "JobRecord";
            }
        }



        //private aceEnumDictionary<contentTokenCountCategory, tokenStatistics> _tokenFrequencyMatrixByCategory = new aceEnumDictionary<contentTokenCountCategory, tokenStatistics>();
        ///// <summary> </summary>
        //public aceEnumDictionary<contentTokenCountCategory, tokenStatistics> tokenFrequencyMatrixByCategory
        //{
        //    get
        //    {
        //        return _tokenFrequencyMatrixByCategory;
        //    }
        //    set
        //    {
        //        _tokenFrequencyMatrixByCategory = value;
        //        OnPropertyChanged("tokenFrequencyMatrix");
        //    }
        //}


        #endregion <--------------------------- GLOBAL REGISTERS --------------------------------

        /// <summary>
        /// Reference to the webSiteProfileSample instance
        /// </summary>
        
        //public webSiteProfileSample sample { get; protected set; } = new webSiteProfileSample();

        /// <summary>
        /// Reference to the sciProject instance
        /// </summary>
        public analyticProject sciProject { get; protected set; }


        private analyticJob _job = new analyticJob();
        /// <summary> </summary>
        public analyticJob job
        {
            get
            {
                return _job;
            }
            protected set
            {
                _job = value;
                OnPropertyChanged("job");
            }
        }


        private analyticJobRunFlags _runFlags = new analyticJobRunFlags();
        /// <summary> </summary>
        public analyticJobRunFlags runFlags
        {
            get
            {
                return _runFlags;
            }
            protected set
            {
                _runFlags = value;
                OnPropertyChanged("runFlags");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int sampleTakeLimit { get; set; }

        private int _sampleBlockOrdinalNumber;
        /// <summary> </summary>
        public int sampleBlockOrdinalNumber
        {
            get
            {
                return _sampleBlockOrdinalNumber;
            }
            set
            {
                _sampleBlockOrdinalNumber = value;
                OnPropertyChanged("sampleBlockOrdinalNumber");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<ISpiderEvaluatorBase> spiderList { get; set; } = new List<ISpiderEvaluatorBase>();

        public analyticJobRecord(analyticJob __job) : base(__job.name, __job)
        {

        }

            public analyticJobRecord(analyticJob __job, analyticProject __sciProject, analyticJobRunFlags aFlags) :base(__sciProject.name, __job)
        {
            
            job = __job;
            //sampleTakeLimit = aFlags.getSampleTakeLimit();
            //if (sampleTakeLimit == 0) sampleTakeLimit = imbWEMManager.settings.sampleTakeLimit;

            //sampleTakeLimit = sampleTakeLimit + job.sampleSkip;

            //sampleTakeLimit = Math.Min(imbWEMManager.settings.sampleTakeLimit, sampleTakeLimit);

            sampleBlockOrdinalNumber = 0; // imbWEMManager.settings.sampleTakeBlock;

            sciProject = __sciProject;

          //  contentTreeGlobalRegistar.allowExistingContentTrees = imbWEMManager.settings.loaderComponent.cacheInMemoryContentTree;
            
            //tokenizedContentGlobalRegistar.allowUseExisting = imbWEMManager.settings.loaderComponent.cacheInMemoryTokenizedContent;
            


        }




        private modelSpiderTestGeneralRecord _tGeneralRecord ;
        /// <summary> </summary>
        public modelSpiderTestGeneralRecord tGeneralRecord
        {
            get
            {
                return _tGeneralRecord;
            }
            protected set
            {
                _tGeneralRecord = value;
                OnPropertyChanged("tGeneralRecord");
            }
        }


        public void initialize()
        {
            log("Runstamp [" + testRunStamp + "]");

            tGeneralRecord = new modelSpiderTestGeneralRecord(testRunStamp, job);
            tGeneralRecord.parent = this;

            foreach (ISpiderEvaluatorBase evalBase in spiderList)
            {
                log("Assigning [" + evalBase.name + "] to [" + instanceID + "]");

                var tRecord = children.Add(evalBase);
                tRecord.parent = this;
                tRecord.instanceID = evalBase.name;
                tRecord.testRunStamp = testRunStamp;
                // evalBase.language = imbLanguageFramework.imbLanguageFrameworkManager.serbian.basic;

                tRecord.inititialize();

            }

           // log("Sample entries selected [" + sample.Count() + "] from [" + sample.usedGroups.Join(",") + "]");


            logBuilder.close();


            recordStart(testRunStamp, name);

            startChildRecord(spiderList.First(), spiderList.First().name);

        }


        public List<webSiteProfile> sample { get; set; } = new List<webSiteProfile>();

        /// <summary>
        /// Initializes the soft: sets sample collection (but still don't create wRecords), sets tRecord, and starts
        /// </summary>
        /// <param name="insample">The insample.</param>
        public void initializeSoft(List<webSiteProfile> insample)
        {
            log("Analytic job record [" + GetType().Name + "] initialization started");
            log("Runstamp [" + testRunStamp + "]");

            sample = insample;

         //   log("Sample entries selected [" + sample.Count() + "] from [" + sample.usedGroups.Join(",") + "]");

            modelSpiderTestRecord tRecord = children.First().Value;

            tGeneralRecord = new modelSpiderTestGeneralRecord(testRunStamp, job);
            tGeneralRecord.parent = this;
            

            recordStart(testRunStamp, name);

            startChildRecord(tRecord.instance, tRecord.instance.name);

        }

        /// <summary>
        /// Initializes the specified a flags.
        /// </summary>
        /// <param name="aFlags">a flags.</param>
        /// <param name="macroScript">The macro script.</param>
        public void initialize(analyticJobRunFlags aFlags, analyticMacroBase macroScript)
        {
            log("Analytic macro script [" + GetType().Name + "] initialization started");
            log("Runstamp [" + testRunStamp + "]");

            logBuilder.open("tag", "Job initialization", "the system initial self-configuration");

            //imbSemanticEngine.imbSemanticEngineManager.cacheManagerForContentTreeBuilder.cacheLoadDisabled = (!aFlags.HasFlag(analyticJobRunFlags.enable_WebStructureCache));
            //imbSemanticEngine.imbSemanticEngineManager.cacheManagerForContentPage.cacheLoadDisabled = (!aFlags.HasFlag(analyticJobRunFlags.enable_NLPCache));
            //imbSemanticEngine.imbSemanticEngineManager.cacheManagerForHtmlContentPage.cacheLoadDisabled = (!aFlags.HasFlag(analyticJobRunFlags.enable_WebCache));

            checkList aFlagCheckList = new checkList(aFlags);

            //if (!sample.Any())
            //{
            //    sample = sciProject.getSamples(job.sampleTags, sampleTakeLimit, testRunStamp, sampleBlockOrdinalNumber); //sciProject.mainWebProfiler.webSiteProfiles.Where<webSiteProfile>(x => x.groupTags.Contains(job.sampleGroup.groupTag)).Take(sampleTakeLimit);// wpGroups.selectGroup(wpGroups.primarySample, terminal, profiler.webSiteProfiles, runstamp, "", false);
            //    log(sample.logMessage);
            //}

            //sample = new webSiteProfileSample(wbp, sciProject.mainWebProfiler, sciProject.mainWebProfiler.webSiteProfiles);
            //sample.usedGroups.Add(job.sampleGroup.groupTag);
            //sample.usedStamp = testRunStamp;
            //sample.usedSettings = sciProject.mainWebProfiler.sampler;
            if (!spiderList.Any())
            {
                spiderList = macroScript.helpMethodBuildSpiders(job, sciProject);
            }

            tGeneralRecord = new modelSpiderTestGeneralRecord(testRunStamp, job);
            tGeneralRecord.parent = this;

            foreach (ISpiderEvaluatorBase evalBase in spiderList)
            {
                log("Assigning [" + evalBase.name + "] to [" + instanceID + "]");
                
                var tRecord = children.Add(evalBase);
                tRecord.parent = this;
                tRecord.instanceID = evalBase.name;
                tRecord.testRunStamp = testRunStamp;
               // evalBase.language = imbLanguageFramework.imbLanguageFrameworkManager.serbian.basic;

                tRecord.inititialize(aFlags, macroScript);
                
            }
            
         //   log("Sample entries selected [" + sample.Count() + "] from [" + sample.usedGroups.Join(",") + "]");


            logBuilder.close();


            recordStart(testRunStamp, sciProject.name);

            startChildRecord(spiderList.First(), spiderList.First().name);

        }


        /// <summary>
        /// Global data shipped to report builder
        /// </summary>
        public PropertyCollectionExtended data { get; set; }


        private DataTable _sampleTable;
        /// <summary> </summary>
        public DataTable sampleTable
        {
            get
            {
                return _sampleTable;
            }
            protected set
            {
                _sampleTable = value;
                OnPropertyChanged("sampleTable");
            }
        }

        public override modelRecordMode VAR_RecordModeFlags
        {
            get
            {
                return modelRecordMode.singleStarter | modelRecordMode.obligationStartBeforeInit | modelRecordMode.summaryScope;
            }
        }

        public override void datasetBuildOnFinish()
        {
            /*

            this.BuildExperimentTokenStatistics(contentTokenCountCategory.allKnownWordTokens, analyticRecordTableExporters.contentTokenAggregationMode.singleTakeForUniqueURL | analyticRecordTableExporters.contentTokenAggregationMode.equalizeForPageCountOnExperimentLevel);
            this.BuildExperimentTokenStatistics(contentTokenCountCategory.headingContentTokens, analyticRecordTableExporters.contentTokenAggregationMode.singleTakeForUniqueURL);
            this.BuildExperimentTokenStatistics(contentTokenCountCategory.linkTitleTokens, analyticRecordTableExporters.contentTokenAggregationMode.singleTakeForUniqueURL);
            this.BuildExperimentTokenStatistics(contentTokenCountCategory.normalSentences, analyticRecordTableExporters.contentTokenAggregationMode.singleTakeForUniqueURL);
            this.BuildExperimentTokenStatistics(contentTokenCountCategory.pageTitleAndDomainTokens, analyticRecordTableExporters.contentTokenAggregationMode.singleTakeForUniqueURL);


            // <<< --------------------------------------------------------------- calling particular builds
            int pRecordCount = 0;
            int sRecordCount = 0;
            int tRecordCount = 0;
            tGeneralRecord.pageUrlListGeneral = new instanceCountCollection<string>();
            foreach (modelSpiderTestRecord tRecord in GetChildRecords())
            {
                modelWebSiteGeneralRecord wGeneralRecord = null;
                foreach (modelSpiderSiteRecord sRecord in tRecord.GetChildRecords())
                {
                    modelWebPageGeneralRecord pGeneralRecord = null;
                    foreach (modelSpiderPageRecord pRecord in sRecord.GetChildRecords())
                    {
                        if ((pGeneralRecord == null) && (pRecord.pGeneralRecord != null))
                        {
                            pRecord.pGeneralRecord.loadedBy.Set(tRecord.instance, true);
                        }
                        
                        pRecord.datasetBuildOnFinishDefault();
                        pRecord.datasetBuildOnFinish();
                        pRecordCount++;
                        if (pRecord.pageInstance != null)
                        {
                            tGeneralRecord.pageUrlListGeneral.AddInstance(pRecord.pageInstance.url, "datasetBuildOnFinish()");
                        }

                    }
                    sRecord.datasetBuildOnFinishDefault();
                    sRecord.datasetBuildOnFinish();
                    sRecordCount++;
                }
                tRecord.datasetBuildOnFinishDefault();
                tRecord.datasetBuildOnFinish();
                tRecordCount++;
            }

            tGeneralRecord.pagesLoaded = pRecordCount;
            
            
            foreach (modelWebSiteGeneralRecord sGeneralRecord in tGeneralRecord.GetChildRecords())
            {
                foreach (modelWebPageGeneralRecord pGeneralRecord in sGeneralRecord.GetChildRecords())
                {
                    pGeneralRecord.datasetBuildOnFinishDefault();
                    pGeneralRecord.datasetBuildOnFinish();
                    pGeneralRecord.summaryFinished();
                }

                sGeneralRecord.datasetBuildOnFinishDefault();
                sGeneralRecord.datasetBuildOnFinish();
                sGeneralRecord.summaryFinished();

            }

            tGeneralRecord.datasetBuildOnFinishDefault();
            tGeneralRecord.datasetBuildOnFinish();
            tGeneralRecord.summaryFinished();
            // <-----------------------------------------------------------------------------------------------


            //sampleTable = sample.buildDataTable("Sample for " + testRunStamp, imbDataTableExtensions.buildDataTableOptions.doInsertAutocountColumn | imbDataTableExtensions.buildDataTableOptions.doInherited | imbDataTableExtensions.buildDataTableOptions.doCreate, null);
           // dataSet.Tables.Add(sampleTable);

           // var data = AppendDataFields(null);
            //DataTable dt = data.buildDataTableVerticalSummaryTable(aceCommonTypes.reporting.globalMeasureUnitDictionary.globalTableEnum.identification);

           // dataSet.Tables.Add(dt);
           */

            logBuilder.AppendTable(sampleTable);
        }



        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>
        /// Updated or newly created property collection
        /// </returns>
        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            data = base.AppendDataFields(data, modelRecordFieldToAppendFlags.identification | modelRecordFieldToAppendFlags.commonData);

            return data;
        }

        /// <summary>
        /// Records the start. Make sure to call <see cref="!:_recordStart" /> at beginning of the method
        /// </summary>
        /// <param name="__testRunStamp"></param>
        /// <param name="__instanceID"></param>
        /// <param name="resources"></param>
        public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
        {
            _recordStart(__testRunStamp, __instanceID);
        }

        /// <summary>
        /// Records the finish. Make sure to call <see cref="!:_recordFinish" /> at the end of the method
        /// </summary>
        /// <param name="resources"></param>
        public override void recordFinish(params object[] resources)
        {
            _recordFinish();
          
        }

        public DataTable GetSummary()
        {
            var data = base.AppendDataFields(null, modelRecordFieldToAppendFlags.identification | modelRecordFieldToAppendFlags.modelRecordCommonData);
            data.name = "Analytic Job";
            data.description = "Summary report on the main execution record.";
            return data; //.buildDataTableVerticalSummaryTable(aceCommonTypes.reporting.globalMeasureUnitDictionary.globalTableEnum.identification);
        }

        /*
        public DataTable GetSpiderTable()
        {
            List<modelSpiderTestRecord> spidersRecords = GetChildRecords();

            //spidersRecords.BuildDataTableHorizontal("Spiders", "Overview of spiders ran", PropertyEntryColumn.entry_name, new string[] { nameof(modelSpiderTestRecord.spider.name), nameof(modelSpiderTestRecord.childIndexCurrent) }
            
            //DataTable table = siteRecords.Get("instance").buildDataTable("Crawled site", "Overview of processed web sites", PropertyEntryColumn.entry_description,
            //    new String[] { "instance.domain", nameof(modelWebSiteGeneralRecord.state), nameof(crawlerAgentContext.status), nameof(crawlerAgentContext.pageCaption), nameof(crawlerAgentContext.isCrawled) });
            
          //     ;

        }*/
        /*
        public DataTable GetSampleTable()
        {
            return sample.BuildDataTableHorizontal("Processed sample set: " + job.sampleGroup.groupTitle, job.sampleGroup.groupDescription, PropertyEntryColumn.entry_description | PropertyEntryColumn.entry_name,
               new String[] { nameof(webSiteProfile.domain), nameof(webSiteProfile.ownerName), nameof(webSiteProfile.siteName), nameof(webSiteProfile.state), nameof(webSiteProfile.siteName) });
            
        }
        */
       

    }
}
