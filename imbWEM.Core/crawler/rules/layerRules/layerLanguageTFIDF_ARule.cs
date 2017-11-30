// --------------------------------------------------------------------------------------------------------------------
// <copyright file="layerLanguageTFIDF_ARule.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.layerRules
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
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class layerLanguageTFIDF_ARule : layerDistributionActiveRuleBase
    {
        public layerLanguageTFIDF_ARule(basicLanguage __language, int __layerID, ISpiderEvaluatorBase __parent, int __layerID2=-1) 
            : base("Language TF-IDF Test ({0})", "Tests Target tokens against the specified language [{0}], sets layerID [{1}] and calculates layer weight score as sum of matched Target token TF-IDFs minus sum of unmatched."
                  + "If resulting weight score is more than 0 the layerID [{1}] is assigned, if it's less than 0 then the layer2ID [{2}] is assigned", __layerID, __parent, __layerID2)
        {

            language = __language;
            name = string.Format(name, language.languageEnglishName);
            description = string.Format(description, language.languageEnglishName, layerID, layer2ID);
        }


        /// <summary>
        /// 
        /// </summary>
        public basicLanguage language { get; protected set; }


        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderTarget target = wRecord.context.targets.GetOrCreateTarget(link, false, false);

            spiderEvalRuleResult output = new spiderEvalRuleResult(this);

            double weight = 0;
            foreach (IWeightTableTerm term in target.tokens)
            {
                if (language.isKnownWord(term.nominalForm))
                {
                    weight += target.tokens.GetTF_IDF(term);
                } else
                {
                    weight -= target.tokens.GetTF_IDF(term);
                }
            }

            if (weight > 0)
            {
                output.layer = layerID;
            } else
            {
                output.layer = layer2ID;
                
            }
            output.weightScore = weight;

            return output;

        }

        public override void prepare()
        {
            iteration = 0;
            wRecord = null;
        }

        public override void learn(ISpiderElement element)
        {
            //
        }

        public override void onStartIteration()
        {
            //
        }

       
    }

}