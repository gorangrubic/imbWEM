// --------------------------------------------------------------------------------------------------------------------
// <copyright file="plugIn_dataLoader.cs" company="imbVeles" >
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
namespace imbWEM.Core.console.plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using imbACE.Services.terminal;
    using imbCommonModels.webPage;
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
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math.aggregation;
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
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index;
    using imbWEM.Core.index.core;
    using imbWEM.Core.plugins;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="aceCommonServices.terminal.console.aceConsolePluginForFiles" />
    public class plugIn_dataLoader: aceConsolePluginForFiles
    {
        public const string HELP = "";

        /// <summary>
        /// Identification name 
        /// </summary>
        /// <param name="__name">The name.</param>
        public plugIn_dataLoader(string __name) : base(__name, HELP, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="plugIn_dataLoader"/> class.
        /// </summary>
        /// <param name="__parent">The parent.</param>
        /// <param name="__name">Property name at parent executor</param>
        public plugIn_dataLoader(IAceOperationSetExecutor __parent, string __name) : base(__parent, __name, HELP)
        {
        }





        [Display(GroupName = "repair", Name = "RecoverTime", ShortName = "", Description = "Recalculating time by importing dt_dataLoad exported table and updating performance exports for each crawl")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will load the results record for opened session to find all crawls, and import all dt_dataLoad Excel tables to sum sampling periods")]
        /// <summary>Recalculating time by importing dt_dataLoad exported table and updating performance exports for each crawl</summary> 
        /// <remarks><para>It will load the results record for opened session to find all crawls, and import all dt_dataLoad Excel tables to sum sampling periods</para></remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_repairRecoverTime()
        {
            var __recordKeyProperty = "TestID";
            var homeFolder = new folderNode("index\\benchmark", "Home folder of plugin: benchmark ", "Internal data for pluting benchmark");
            var recordName = imbWEMManager.index.experimentEntry.SessionID;

            var records = new objectTable<reportPlugIn_benchmarkResults>(homeFolder.pathFor(recordName.add("results", "_")), true, __recordKeyProperty, name);
            records.description = "Summary report on the most relevant evaluation metrics.";

            var record_performances = new objectTable<performanceRecord>(homeFolder.pathFor(recordName.add("performances", "_")), true, "TestID", name);

            var record_moduleImpact = new objectTable<moduleFinalOverview>(homeFolder.pathFor(recordName.add("modules", "_")), true, "ModuleName", name);

            // <---- making crawler list
            List<string> crawlerList = new List<string>();

            List<reportPlugIn_benchmarkResults> allRecords = records.GetList();
            var reportFolder = imbWEMManager.index.experimentEntry.sessionReportFolder;

            Dictionary<string, string> pathsForResultExcel = new Dictionary<string, string>();

            Dictionary<string, folderNode> foldersForResultExcel = new Dictionary<string, folderNode>();


            foreach (reportPlugIn_benchmarkResults result in allRecords)
            {
                crawlerList.Add(result.Crawler);
                output.AppendLine("Crawl found: " + result.Crawler);

                string pathCrawlerId = result.Crawler.Replace("-", "");

                folderNode resultNode = reportFolder[pathCrawlerId.ToUpper() + "\\crawler\\data"];

                string pathForData = resultNode.pathFor("dc_dataload_" + result.Crawler.ToLower() + ".csv");
                //String pathForResult = reportFolder.pathFor(pathCrawlerId);

                foldersForResultExcel.Add(result.Crawler, resultNode);

                //foldersForResultExcel.Add(result.Crawler, pathForResult);


               // String path = reportFolder.pathFor(pathForData, getWritableFileMode.existing);
                output.AppendLine("Loading datatable: " + pathForData);
                
                DataTable dataTable = null;
                dataTable = pathForData.deserializeDataTable(dataTableExportEnum.csv);
                output.AppendLine("DataTable loaded - rows[" + dataTable.Rows.Count + "]");
                DataColumn periodColumn = dataTable.Columns["Period"];
                double periodSum = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    string read = dr[periodColumn].ToString().Replace(",", ".");
                    double readD = double.Parse(read);
                    periodSum += readD;
                }

                output.AppendLine("Total execution time in seconds: " + periodSum.ToString("F5"));

                result.CrawlTime = periodSum / ((double)60);
                records.AddOrUpdate(result);
            }

            foreach (reportPlugIn_benchmarkResults result in allRecords)
            {
                folderNode resultFolder = foldersForResultExcel[result.Crawler];

                records.GetDataTable().GetReportAndSave(resultFolder, imbWEMManager.authorNotation, "results_timefix", true);

                output.AppendLine("Repaired result table saved to: " + resultFolder.path);
                // <---- fixing crawler results
            }
        }




        [Display(GroupName = "run", Name = "IndexData", ShortName = "", Description = "Exporting domain list according to criteria specified")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will select domains using existing data. If index name not specified it will scan index repository and ask user to pick one")]
        /// <summary>Exporting domain list according to criteria specified</summary> 
        /// <remarks><para>It will select domains using existing data. If index name not specified it will scan index repository and ask user to pick one</para></remarks>
        /// <param name="indexName">name of the index to harvest sample from - IndexID</param>
        /// <param name="minPages">required min. number of crawled/indexed pages in the doman--</param>
        /// <param name="minRelevant">required min. number of relevant pages in the index for the domain</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runIndexData(
            [Description("name of the index to harvest sample from - IndexID")] string indexName = "MainIndex",
            [Description("required min. number of crawled/indexed pages in the doman--")] int minPages = 30,
            [Description("required min. number of relevant pages in the index for the domain")] int minRelevant = 10)
        {

            if ((indexName == "*") || indexName.isNullOrEmpty()) 
            {
                List<string> indexList = imbWEMManager.index.GetIndexList();
                indexList.Add("*");
                aceTerminalInput.askForOption("Choose index to work with - or confirm * to load all indexes:", "*", indexList);
            }

            indexDatabaseStandalone indexDb = new indexDatabaseStandalone(indexName);
            
            
            
            imbWEMManager.index.OpenIndex(indexName, "plugin_dataLoader");

            imbWEMManager.index.pageIndexTable.ReadOnlyMode = true;
            imbWEMManager.index.domainIndexTable.ReadOnlyMode = true;
            List<indexDomain> d_list = new List<indexDomain>();

            List<indexPage> pages = imbWEMManager.index.pageIndexTable.GetPagesAndDomains(indexPageEvaluationEntryState.inTheIndex, out d_list);

            aceDictionarySet<indexDomain, indexPage> dict = new aceDictionarySet<indexDomain, indexPage>();
            List<string> list = new List<string>();
            List<indexDomain> domains = new List<indexDomain>();

            foreach (indexDomain domain in d_list)
            {
                
                List<indexPage> pl = Enumerable.Where(pages, x => x.url.Contains(domain.domain)).ToList();
                dict.Add(domain, pl);
                int prc = 0;
                if (pl.Count() > minPages)
                {
                    foreach (indexPage ip in pl)
                    {
                        if (ip.relevancyText == "isRelevant") prc++;
                        if (prc > minRelevant)
                        {
                            output.AppendLine($" {domain.domain} P[_{pl.Count()}_] Pr[_{prc}_] --> accepted, stop counting");
                          //  domains.Add(domain);
                            list.Add(domain.domain);
                            break;
                        }
                    }
                }
                
            }

          //  var domains = imbWEMManager.index.domainIndexTable.GetWhere(nameof(indexDomain.relevantPages) + " > " + minRelevant);
           // domains = domains.Where(x => ((x.relevantPages + x.notRelevantPages) > minPages)).ToList();
            string sampleName = indexName.add("Pa" + minPages + "Pr" + minRelevant, "_").add("txt", ".");
           
            domains.ForEach(x => list.Add(x.url));
            

            objectTable<indexDomain> dTable = new objectTable<indexDomain>("url", sampleName);
            domains.ForEach(x => dTable.AddOrUpdate(x));

            dTable.GetDataTable(null, sampleName).GetReportAndSave(folder, imbWEMManager.authorNotation, sampleName, true);

            folder = imbWEMManager.index.folder;

            string p = folder.pathFor(sampleName);
            list.saveContentOnFilePath(p);

            output.log("Exported sample saved to: " + p);
            

        }





        [Display(GroupName = "run", Name = "WorkloadData", ShortName = "", Description = "Performs post-processing of data collected by the workload plugin")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Loads all saved DataTables, groups rows in averages for each measure group and creates summary table with all experiments")]
        /// <summary>Performs post-processing of data collected by the workload plugin</summary> 
        /// <remarks><para>Loads all saved DataTables, groups rows in averages for each measure group and creates summary table with all experiments</para></remarks>
        /// <param name="searchPattern">pattern used to select input files</param>
        /// <param name="groupColumn">column name used for row grouping</param>
        /// <param name="overviewColumns">columns to include in overview table</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runWorkloadData(
            [Description("pattern used to select input files")] string searchPattern = "results*.xml",
            [Description("column name used for row grouping")] string groupColumn = "measureGroup")
           // [Description("columns to include in overview table")] String overviewColumns = "DataLoad,CrawlerIterations,ContentPages,dlcMaximum")
        {
            
            aceOperation_selectFiles(searchPattern, "index\\workload", true);

            folder = folder["index\\workload"];

            List<DataTable> tables = new List<DataTable>();

            dataPointAggregationType aggType = dataPointAggregationType.avg;

            int ci = 1;
            int c = selectedFiles.Count();

            output.log("[" + c + "] DataTable in the cue.");


            List<DataTable> allTables = new List<DataTable>();
            DataSet dSet = new DataSet();


            aceDictionarySet<string, DataTable> byCrawler = new aceDictionarySet<string, DataTable>();
            aceDictionarySet<string, DataTableForStatistics> byCrawlerRT = new aceDictionarySet<string, DataTableForStatistics>();
            
            DataTableForStatistics rt = null;

                    foreach (FileInfo fi in selectedFiles)
            {
                try
                {
                    objectTable<reportPlugIn_workloadEntry> workloadEntry = new objectTable<reportPlugIn_workloadEntry>(fi.FullName, true, "EntryID", "");
                    
                    objectTable<reportPlugIn_workloadEntry> workloadGrouped = new objectTable<reportPlugIn_workloadEntry>("EntryID", "aggregated");

                    aceDictionarySet<int, reportPlugIn_workloadEntry> workloadGroups = workloadEntry.GetGroups<int>(groupColumn, "terminationWarning = 0");

                    collectionAggregationResultSet<reportPlugIn_workloadEntry> aggregateSet = new collectionAggregationResultSet<reportPlugIn_workloadEntry>();

                    

                    foreach (var set in workloadGroups)
                    {
                        collectionAggregationResult<reportPlugIn_workloadEntry> aggregates = null;
                        aggregates = set.Value.GetAggregates(aggType);

                        var aggregate = aggregates[aggType];
                        aggregate.measureGroup = set.Key;
                        aggregate.EntryID = set.Key.ToString("D5") + "_" + aggType.ToString();
                        workloadGrouped.AddOrUpdate(aggregate);
                        aggregateSet.Add(aggregate.EntryID + "_" + fi.Name, aggregates);

                    }

                    string filename = (fi.Name + "_" + groupColumn + "_" + aggType.ToString()).getFilename();

                    string n = reportPlugIn_workload_state.ExtractEntryID(aggregateSet.lastItem.EntryID) + dSet.Tables.Count.ToString("D2");

                    DataTable dt = workloadGrouped.GetDataTable(dSet, n);
                    dt.SetDescription("Collection of [" + aggregateSet.recordType.Name + "] records grouped by [" + groupColumn + "]");
                    dt.SetAggregationAspect(dataPointAggregationAspect.subSetOfRows);
                    dt.SetAggregationOriginCount(aggregateSet.Count);
                    dt.SetAdditionalInfoEntry("Aggregation Type:", aggType);
                    dt.SetAdditionalInfoEntry("Data source file:", fi.Name);

                    dt.SetAdditionalInfoEntries("Last", aggregateSet.lastItem, typeof(string));
                    
                    dt.SetTitle(n);

                    byCrawler.Add(aggregateSet.firstItem.Crawler, dt);

                   // dt.TableName = n;
                 //   dSet.AddTable(dt);

                    
                    rt = dt.GetReportAndSave(folder, imbWEMManager.authorNotation, n.getFilename(), true);
                    byCrawlerRT.Add(aggregateSet.firstItem.Crawler, rt);
                    response.AppendLine("[" + ci + " / " + c + "] DataTable [" + fi.Name + "] had [" + workloadGroups.Keys.Count() + "] groups. Result saved as: " + filename);
                    ci++;

                } catch (Exception ex)
                {
                    output.log("[" + ci + " / " + c + "] DataTable [" + fi.FullName + "] failed.");
                    output.log(ex.Message);
                }
            }



            output.log("[" + c + "] DataTable processed.");

            dSet.serializeDataSet("workload_all", folder, dataTableExportEnum.excel, imbWEMManager.authorNotation);

            foreach (string key in byCrawler.Keys)
            {
                string filename = key.getFilename();
                DataSet sd = new DataSet(key);
                foreach (DataTable dti in byCrawler[key])
                {
                    sd.AddTable(dti.Copy());
                }
               
                sd.AddTable(byCrawlerRT[key].First().RenderLegend());
                sd.serializeDataSet(filename, folder, dataTableExportEnum.excel, imbWEMManager.authorNotation);
            }

        }




    }
}
