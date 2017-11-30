// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageModule.cs" company="imbVeles" >
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
    using imbNLP.Data.basic;
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
    using imbWEM.Core.crawler.rules.layerRules;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class languageModule : spiderLayerModuleBase
    {
        public languageModule(ISpiderEvaluatorBase __parent, basicLanguage __langA, basicLanguage __langB) 
            : base("Language Module", "The Targets are distributed into layers by the Passive rules and Active rules testing the tokens of the Target.", __parent)
        {
            languageA = __langA;
            languageB = __langB;
          //  setup();
        }

        /// <summary>
        /// 
        /// </summary>
        public basicLanguage languageA { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public basicLanguage languageB { get; set; }


        public override void setup()
        {
            layers.AddLayer("Primary", "Links evaluated to match languageA");
            layers.AddLayer("Secondary", "Links evaluated to match languageB");
            layers.AddLayer("Reserve", "Links not matching languageA nor languageB");

            AddRule(new layerNeedlePRule("sr,srp,rs,srb,srpski,serbian,yu", 0, parent, -1));
            AddRule(new layerNeedlePRule("en,eng,gb,gbr,uk,engleski,english", 1, parent, -1));

            AddRule(new layerNeedlePRule("de,ger,nem,deutsch,german,deu,nemacki,nemački", 2, parent, -1));
            AddRule(new layerNeedlePRule("ru,rus,russkiy,ruski,russian", 2, parent, -1));
            AddRule(new layerNeedlePRule("it,ita,italian,italiano,italijanski", 2, parent, -1));
            AddRule(new layerNeedlePRule("fr,fra,french,français,francuski", 2, parent, -1));
            AddRule(new layerNeedlePRule("hu,hun,hungarian,magyar,mađarski,madjarski", 2, parent, -1));
            AddRule(new layerNeedlePRule("es,esp,spanish,espanol,španski,spanski", 2, parent, -1));
            AddRule(new layerNeedlePRule("si,sl,slovenian,slo,slovenački,slovenacki,slovenski", 2, parent, -1));

            AddRule(new layerLanguageTFIDF_ARule(languageA, 0, parent, -1));
            AddRule(new layerLanguageTFIDF_ARule(languageB, 1, parent, -1));

            if (imbWEMManager.settings.crawlAdHok.FLAG_doAddLexiconLanguageRule)
            {
                AddRule(new layerLexiconARule(1, parent, 2));
            }

            restPolicy = spiderLayerModuleEvaluationRestPolicy.assignToTheDeepestLayer;
            pullLimit = -1;
            doTakeFromLower = false;
        }

        public override void reportIteration(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            base.reportIteration(reporter, wRecord);
        }

        public override void reportDomainFinished(directAnalyticReporter reporter, modelSpiderSiteRecord wRecord)
        {
            
        }

        public override void reportCrawlFinished(directAnalyticReporter reporter, modelSpiderTestRecord tRecord)
        {
            
        }
    }
}
