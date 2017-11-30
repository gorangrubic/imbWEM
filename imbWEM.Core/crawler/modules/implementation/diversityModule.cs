// --------------------------------------------------------------------------------------------------------------------
// <copyright file="diversityModule.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.modules.implementation
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class diversityModule : spiderRankingModuleBase
    {
        /// <summary>
        /// 
        /// </summary>
        public double tt_diversityFactor { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double pt_diversityFactor { get; set; }


        /// <summary> Steps for semantic expansion </summary>
        [Category("Count")]
        [DisplayName("termExpansionSteps")]
        [imb(imbAttributeName.measure_letter, "TExp")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Steps for semantic expansion")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int termExpansionSteps { get; set; } = 3;



        public diversityModule(double ttd, double ptd, ISpiderEvaluatorBase __parent, int expansionSteps) : 
            base("Diversity Module", "Use inversed semantic similarity between a target and Target link url/title corpus and Target page content corpus", __parent)
        {
            tt_diversityFactor = ttd;
            pt_diversityFactor = ptd;

            termExpansionSteps = expansionSteps;

         //   setup();
        }



        /// <summary>
        /// Evaluation procedure -- implementation for modules without layers
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="wRecord">The w record.</param>
        /// <returns></returns>
        public override ISpiderModuleData evaluate(ISpiderModuleData input, modelSpiderSiteRecord wRecord)
        {
            List<spiderLink> output = new List<spiderLink>();
            spiderModuleData<spiderLink> outdata = new spiderModuleData<spiderLink>();

            moduleDLCRecord moduleLevelReportTable = ((spiderModuleData<spiderLink>)input).moduleDLC;
            moduleIterationRecord moduleDLCRecordTableEntry = ((spiderModuleData<spiderLink>)input).moduleDLCRecordTableEntry;

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules)
            {
                //dataInput.moduleDLCRecordTableEntry = dataInput.moduleDLC.GetOrCreate(wRecord.iteration.ToString("D3") + module.name);
                moduleDLCRecordTableEntry.reportEvaluateStart(input as spiderModuleData<spiderLink>, wRecord, this); // <--- module level report --- start
            }

            input.active.ForEach(x => output.Add(x as spiderLink)); // ----- this is part where the layer modules are emulated

            if (imbWEMManager.settings.directReportEngine.DR_ReportModules) moduleDLCRecordTableEntry.reportEvaluateEnd(output, wRecord, this); // <--- module level report --- start
            outdata.active.AddRange(rankLinks(output, wRecord.iteration));


            if (imbWEMManager.settings.directReportEngine.DR_ReportModules) moduleDLCRecordTableEntry.reportEvaluateAlterRanking(outdata.active, wRecord,this); // <--- module level report --- start

            return outdata;
        }
        
        public override void setup()
        {
            AddRule(new rankDiversityALink(parent, tt_diversityFactor, pt_diversityFactor, termExpansionSteps));
        }

        public override void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            
        }

        public override void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {

        }

        public override void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord)
        {

        }
    }

}