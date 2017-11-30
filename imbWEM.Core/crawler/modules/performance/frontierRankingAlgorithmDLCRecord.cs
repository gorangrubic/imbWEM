// --------------------------------------------------------------------------------------------------------------------
// <copyright file="frontierRankingAlgorithmDLCRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.modules.performance
{
    using System;
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
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.operations;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.implementation;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="aceCommonTypes.data.tables.objectDataSet{imbAnalyticsEngine.spider.modules.performance.moduleDLCRecord, imbAnalyticsEngine.spider.modules.performance.moduleIterationRecord}" />
    public class frontierRankingAlgorithmDLCRecord : objectDataSet<moduleDLCRecord, moduleIterationRecord>
    {
        public string jobName { get; set; }
        public string crawlerName { get; set; }
        public string domainName { get; set; }


        public objectTable<frontierRankingAlgorithmIterationRecord> generalRecords { get; set; }

        /// <summary>
        /// Reports the start iteration: posle ekstrakcije, pre rangiranja
        /// </summary>
        /// <param name="currentIteration">The current iteration.</param>
        /// <param name="__wRecord">The w record.</param>
        /// <returns></returns>
        public frontierRankingAlgorithmIterationRecord reportStartOfFRA(int currentIteration, modelSpiderSiteRecord __wRecord, spiderModuleData<spiderLink> input)
        {

            var entry = generalRecords.GetOrCreate(crawlerName + currentIteration.ToString("D3"));
            entry.iteration = currentIteration;

            

            Dictionary<string, spiderLink> urls = new Dictionary<string, spiderLink>();
            int newUrls = 0;
            int oldUrls = 0;

            foreach (var pair in input.active)
            {
                urls.Add(pair.url, pair);
                if (pair.iterationDiscovery == currentIteration)
                {
                    newUrls++;
                } else
                {
                    oldUrls++;
                }
            }

            var assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(urls.Keys);

            entry.PLleft =  __wRecord.context.GetPageLoadsToLimit(__wRecord.tRecord.instance.settings.limitTotalPageLoad);

            entry.evaluationKnown = assertion[indexPageEvaluationEntryState.haveEvaluationEntry].Count();
            entry.evaluationUnknown = assertion[indexPageEvaluationEntryState.haveNoEvaluationEntry].Count() + assertion[indexPageEvaluationEntryState.notInTheIndex].Count();
            entry.evaluationCertainty = assertion.certainty;

            entry.inputTargets = urls.Count;
            entry.newTargets = newUrls;
            entry.oldTargets = oldUrls;
            entry.inputPotentialPrecission = assertion.relevant;


            assertion.performInfoGainEstimation(entry.PLleft);
            entry.PotInputIP = assertion.IPnominal;
            
            
            return entry;
        }





        /// <summary>
        /// Called after the all modules at end of FRA
        /// </summary>
        /// <param name="__wRecord">The w record.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        public frontierRankingAlgorithmIterationRecord reportEndOfFRA(modelSpiderSiteRecord __wRecord, frontierRankingAlgorithmIterationRecord entry, spiderModuleData<spiderLink> output)
        {
            entry.output = output.active.Count;

            if (entry.inputTargets > entry.output)
            {
                entry.accumulation = entry.inputTargets - entry.output;
            } else
            {
                entry.drain = entry.output - entry.inputTargets;
            }

            Dictionary<string, spiderLink> urls = new Dictionary<string, spiderLink>();


            foreach (var pair in output.active)
            {
                urls.Add(pair.url, pair);
            }

            var assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(urls.Keys);

            entry.outputPotentialPrecission = assertion.relevant; ///[indexPageEvaluationEntryState.isRelevant].Count.GetRatio(assertion[indexPageEvaluationEntryState.haveEvaluationEntry].Count);

            assertion.performInfoGainEstimation(entry.PLleft);
            entry.PotOutputIP = assertion.IPnominal;

            entry.PotChangeIP = entry.PotOutputIP - entry.PotInputIP;

            entry.potentialPrecissionChange = entry.outputPotentialPrecission - entry.inputPotentialPrecission;




            entry.moduleUse = 0;

            foreach (var modPair in modRecords)
            {
                moduleIterationRecord moduleReport = modPair.Value.GetFirstWhere(nameof(moduleIterationRecord.iteration) + " = " + entry.iteration);
                if (moduleReport != null)
                {
                    entry.moduleUse++;

                    if (modPair.Key == typeof(languageModule).Name)
                    {
                        entry.accumulatedLanguage = moduleReport.accumulated;
                    }
                    else if (modPair.Key == typeof(structureModule).Name)
                    {
                        entry.accumulatedTemplate = moduleReport.accumulated;

                    }
                    else if (modPair.Key == typeof(templateModule).Name)
                    {
                        entry.accumulatedStructure = moduleReport.accumulated;
                    }
                  
                }
            }





            entry.duration = DateTime.Now.Subtract(entry.start).TotalSeconds;


            generalRecords.AddOrUpdate(entry);

            return entry;
        }


        [Category("Time")]
        [DisplayName("ProcessTime")]
        [imb(imbAttributeName.measure_letter, "FRA_sum")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        [Description("Cumulative FRA algorithm process duration")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double ProcessTime { get; set; } = 0;

        /// <summary>
        /// Generates ouiteration out.
        /// </summary>
        /// <param name="wRecord">The w record.</param>
        /// <param name="fn">The function.</param>
        public void reportIterationOut(modelSpiderSiteRecord wRecord, folderNode fn)
        {
            
            generalRecords.GetLastEntry().saveObjectToXML(fn.pathFor("modules_performance.xml"));
            foreach (moduleDLCRecord mod in wRecord.frontierDLC)
            {
                var lastModEntry = mod.GetLastEntry();
                if (lastModEntry != null)
                {
                    lastModEntry.saveObjectToXML(fn.pathFor("module_" + mod.moduleName + ".xml"));
                }
            }
        }

        public DataTable generalDataTable {get;set;}
        public DataTable moduleSummaryDataTable { get; set; }

        public void reportDomainOut(modelSpiderSiteRecord wRecord, folderNode fn, string fileprefix)
        {
          
            generalDataTable = generalRecords.GetDataTable(); //.GetReportTableVersion(true);
            generalDataTable.TableName = "Overview-" + wRecord.domainInfo.domainRootName;
            generalDataTable.SetDescription("General FRA metrics timeline for crawler [" + wRecord.tRecord.instance.name + "] DLC [" + wRecord.domain + "]");
            
            wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.fra_overview, generalDataTable);

            
            List<DataTable> dtsum = new List<DataTable>();
            string modLetter = "";

            
            foreach (var modPair in modRecords)
            {
                PropertyCollectionExtended mpce = new PropertyCollectionExtended();
                mpce = modPair.Value.buildPCE(false, null, "moduleName", "moduleType", "jobName", "crawlerName");

                DataTable tmp = null;
                tmp = modPair.Value.GetDataTable();
                tmp.SetTitle(modPair.Key);
                tmp.SetDescription("Timeline of module " + modPair.Key + " during crawl of [" + wRecord.domain + "] using [" + wRecord.tRecord.instance.name + "] crawler");
                tmp.SetAdditionalInfo(mpce);


                if (modPair.Key == typeof(languageModule).Name)
                {
                    wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.language, tmp);
                    modLetter = modLetter + "L";
                    dtsum.Add(tmp);
                } else if (modPair.Key == typeof(structureModule).Name)
                {
                    wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.structure, tmp);
                    dtsum.Add(tmp);
                    modLetter = modLetter + "S";

                } else if (modPair.Key == typeof(templateModule).Name)
                {
                    wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.template, tmp);
                    dtsum.Add(tmp);
                    modLetter = modLetter + "T";
                } else if (modPair.Key == typeof(diversityModule).Name)
                {
                    wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.diversity, tmp);
                }
                
                
            }

            DataTable fraSum = dtsum.GetSumTable("FRA_" + modLetter).SetDescription("Summary Timeline metrics for Frontier Layer Modules (" + modLetter + ") on domain: [" + wRecord.domain + "]");
            fraSum.SetAggregationOriginCount(dtsum.Count).SetTitle("Layered Modules");

            

            //                public String jobName { get; set; }
            //   public String crawlerName { get; set; }
            //  public String domainName { get; set; }

            PropertyCollectionExtended pce = this.buildPCE(false, null, "jobName", "crawlerName");


            fraSum.SetAdditionalInfoEntry("JobName", jobName);
            fraSum.SetAdditionalInfoEntry("CrawlerName", crawlerName);

            wRecord.tRecord.frontierDLCDataTables.Add(moduleIterationRecordSummary.all, fraSum);



            if (imbWEMManager.settings.directReportEngine.DR_ReportModules_DomainReports)
            {
                generalDataTable.GetReportAndSave(fn, imbWEMManager.authorNotation, "fra_general_" + fileprefix);

                fraSum.AddExtra("The table is aggregated timeline of layer modules [" + modLetter + "] in this crawler.");

                fraSum.GetReportAndSave(fn, imbWEMManager.authorNotation, "fra_modules_sum_" + fileprefix);
            }

        }




        public frontierRankingAlgorithmDLCRecord()
        {
            generalRecords = new objectTable<frontierRankingAlgorithmIterationRecord>("name", name);

        }

        public frontierRankingAlgorithmDLCRecord(modelSpiderSiteRecord wRecord) //  string __dataSetName, string __dataSetDescription, folderNode __fileFolder = null) : base(__dataSetName, __dataSetDescription, __fileFolder)
        {
            generalRecords = new objectTable<frontierRankingAlgorithmIterationRecord>("name", name);
            deploy(wRecord);
        }

        public Dictionary<string, moduleDLCRecord> modRecords { get; set; } = new Dictionary<string, moduleDLCRecord>();

        protected void deploy(modelSpiderSiteRecord wRecord)
        {

            
            name = wRecord.tRecord.instanceID + "_" + wRecord.domainInfo.domainRootName;
            crawlerName = wRecord.tRecord.instance.name;
            domainName = wRecord.domain;


            if (wRecord.tRecord.instance is spiderModularEvaluatorBase)
            {
                spiderModularEvaluatorBase modSpider = wRecord.tRecord.instance as spiderModularEvaluatorBase;
                List<moduleDLCRecord> dlc = new List<moduleDLCRecord>();
                foreach (spiderModuleBase module in modSpider.modules)
                {
                    moduleDLCRecord modRecord = GetNew(module.name);
                    modRecord.start(module, wRecord);

                    modRecords.Add(module.GetType().Name, modRecord);

                    //dlc.Add(modRecord);
                }

                //foreach (var md in dlc) 
            }
        }
    }

}