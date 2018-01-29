// --------------------------------------------------------------------------------------------------------------------
// <copyright file="domainSemanticSpider.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.evaluators
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
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.controlLink;
    using imbWEM.Core.crawler.rules.controlObjective;
    using imbWEM.Core.crawler.rules.pagerules;
    using imbWEM.Core.crawler.rules.passive;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class domainSemanticSpider: spiderEvaluatorSimpleBase
    {
        

        public domainSemanticSpider() : base("DomainSemanticSpider", "Domain-knowledge and Lexicon guided web analysis.", "reportInclude\\testdocs\\domainSemanticSpider.md", true)
        {

            

            settings.limitTotalPageLoad = 2;
            settings.limitIterations = 2;
            settings.limitTotalLinks = 20;
            settings.limitIterationNewLinks = 5;
        }

        public override void setupAll()
        {
            linkPassiveRules.Add(new ruleUrlTitleMatch(this));
            // linkPassiveRules.Add(new ruleKnownWordInCaption(this));

            //  linkPassiveRules.Add(new ruleUrlHasKnownWords(this));
            //  linkPassiveRules.Add(new ruleHasLanguageName(this));

            linkActiveRules.Add(new ruleActiveInflows(this));
            linkActiveRules.Add(new ruleActiveCrossLink(this));
            linkActiveRules.Add(new ruleActiveLinkDepth(this));
            linkActiveRules.Add(new ruleActiveLinkStructure(this));

            pageScoreRules.Add(new pageruleLinkScoreBonus(this));
            pageScoreRules.Add(new pageruleInboundScore(this));
            pageScoreRules.Add(new pageruleCrosslinkScore(this));
            pageScoreRules.Add(new pageruleTitleUnique(this));
            //   pageScoreRules.Add(new pageruleTitleKnownUniqueWords(this));

            controlLinkRules.Add(new controlTrimScoreTail(this));
            controlRules.Add(new controlPageLoad(this, 5));
            controlRules.Add(new controlLsaSpike(this, 2)); // <------------- Link score mean spike detection
            controlRules.Add(new controlDisabler(this)); // <------------- temporary disabler
        }
    }

}