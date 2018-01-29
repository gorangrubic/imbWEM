// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexPerformanceRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.core
{
    using System;
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class indexPerformanceRecord:objectTable<indexPerformanceEntry>
    {

        public void evaluateIndexPerformance(indexPerformanceEntry indexSessionEntry, builderForLog loger, string crawlId)
        {
            indexSessionEntry.SessionID = imbWEMManager.index.experimentManager.SessionID;
            indexSessionEntry.CrawlID = "[Index Evaluation]";
            indexSessionEntry.IndexRepository = imbWEMManager.index.current_indexID;
            indexSessionEntry.Start = DateTime.Now;
            indexSessionEntry.CrawlerHash = analyticConsole.mainAnalyticConsole.state.setupHash_crawler;
            indexSessionEntry.GlobalSetupHash = analyticConsole.mainAnalyticConsole.state.setupHash_global;

             imbWEMManager.index.domainIndexTable.GetDomains(indexDomainContentEnum.any);

            //indexSessionEntry.Domains =  domainIndexTable.Count;
            //indexSessionEntry.Pages = pageIndexTable.Count;
            //indexSessionEntry.PagesEvaluated = pageIndexTable.Where(x => !x.relevancyText.isNullOrEmpty()).Count();

        }


        public int CleanRecords(int days=1, int hours=0)
        {
            int limit = (days * 24) + hours;
            var offLimit = GetList().Where(x => DateTime.Now.Subtract(x.Start).TotalHours > limit).ToList();
            return Remove(offLimit);
        }

        public indexPerformanceRecord() : base("RecordID", "IndexPerformanceRecords")
        {
        }



        public indexPerformanceRecord(string __filePath, bool autoLoad) : base(__filePath, autoLoad, "RecordID", "IndexPerformanceRecords")
        {
            
        }
    }

}