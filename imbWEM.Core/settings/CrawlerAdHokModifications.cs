// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrawlerAdHokModifications.cs" company="imbVeles" >
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
namespace imbWEM.Core.settings
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
    using imbNLP.Data;
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

    public class CrawlerAdHokModifications:imbBindable
    {
        public CrawlerAdHokModifications() { }

        public bool FLAG_doAdjustDiversityScore { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [flag do add lexicon language rule].
        /// </summary>
        /// <value>
        /// <c>true</c> if [flag do add lexicon language rule]; otherwise, <c>false</c>.
        /// </value>
        public bool FLAG_doAddLexiconLanguageRule { get; set; } = false;



        /// <summary> DIVERSITY </summary>
        [Category("DIVERSITY MODULE")]
        [DisplayName("Diversity_TargetTermFactor")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Weight of Target Semantic Terms semantic similarity in semantic diversity calculations")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Diversity_TargetTermFactor { get; set; } = 0.5;



        /// <summary> Ratio </summary>
        [Category("DIVERSITY MODULE")]
        [DisplayName("Diversity_PageContentTermFactor")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Weight of Crawled page content semantic similarity in semantic diversity calculations")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double Diversity_PageContentTermFactor { get; set; } = 0.5;



        /// <summary> Steps in semantic expansion </summary>
        [Category("DIVERSITY MODULE")]
        [DisplayName("Diversity_DefaultExpansionSteps")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Steps in semantic expansion")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public int Diversity_DefaultExpansionSteps { get; set; } = 2;



        /// <summary>  </summary>
        [Category("LANGUAGE MODULE")]
        [DisplayName("Language_primary")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public basicLanguageEnum Language_primary { get; set; } = basicLanguageEnum.serbian;



        /// <summary>  </summary>
        [Category("LANGUAGE MODULE")]
        [DisplayName("Language_secondary")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("")] // [imb(imbAttributeName.reporting_escapeoff)]
        public basicLanguageEnum Language_secondary { get; set; } = basicLanguageEnum.english;





        public void prepare()
        {

        }
    }
}