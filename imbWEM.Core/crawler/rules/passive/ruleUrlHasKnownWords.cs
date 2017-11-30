// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ruleUrlHasKnownWords.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.rules.passive
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// If url has words (longer than 3 chars) recognized by Hunspell dictionary
    /// </summary>
    /// <seealso cref="spiderEvalRuleForLinkBase" />
    public class ruleUrlHasKnownWords : spiderEvalRuleForLinkBase
    {

        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public basicLanguage language { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ruleUrlHasKnownWords"/> class.
        /// </summary>
        /// <param name="__parent">The parent.</param>
        public ruleUrlHasKnownWords(spiderEvaluatorSimpleBase __parent, basicLanguage __language) : base("Url language words", "If url has words (3+ chars) recognized by Hunspell dictionary", 3, -1, __parent)
        {
            language = __language;
        }

        /// <summary>
        /// Evaluates the specified link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns></returns>
        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this);

            if (link.link.pathAndQuery.isNullOrEmpty())
            {
                output.comment = "no pathAndQuery";
                return output;
            }

            List<string> words = link.link.pathAndQuery.getStringTokensMinLength();

            bool score = false;

            if (!words.Any())
            {
                output.comment = "no words in url";
                return output;
            }

            foreach (string wrd in words)
            {
                if (language.isKnownWord(wrd))
                {
                    score = true;
                    break;
                }
            }

            if (score)
            {
                output.score = scoreUnit;
            } else
            {
                output.score = penaltyUnit;
            }

            return output;
        }

        /// <summary>
        /// Prepares this instance - clears temporary data
        /// </summary>
        public override void prepare()
        {
            // -- nothing to prepare
        }
    }

}
