// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ruleKnownWordInCaption.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.data;
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
    /// 
    /// </summary>
    /// <seealso cref="spiderEvalRuleForLinkBase" />
    public class ruleKnownWordInCaption : spiderEvalRuleForLinkBase
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
        /// Initializes a new instance of the <see cref="ruleKnownWordInCaption"/> class.
        /// </summary>
        /// <param name="__parent">The parent.</param>
        public ruleKnownWordInCaption(spiderEvaluatorSimpleBase __parent, basicLanguage __language) : base("Caption known words", "If all words in link caption were recognized by Hunspell dictionary", 4, -1, __parent)
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

            string cp = link.link.caption;

            if (cp.isNullOrEmpty())
            {
                output.comment = "caption is empty";
                return output;
            }

            //if (cp.isEmptyOrWhiteSpace())
            //{
            //    output.comment = "caption is whitespace";
            //    return output;
            //}

            if (cp.isNumber())
            {
                output.comment = "caption is numeric";
                return output;
            }

            if (cp.isSymbolicContentOnly())
            {
                output.comment = "caption is symbolic content";
                return output;
            }


            List<string> words = cp.getStringTokens();

            bool score = false;
            bool penalty = false;

            output.comment = "lang_ok:";

            foreach (string wrd in words)
            {
                if (wrd.isCleanWord())
                {
                    if (language.isKnownWord(wrd))
                    {
                        output.comment += wrd + ";";
                        score = true;
                    } else
                    {
                        output.comment = wrd + " is not lang_ok";
                        score = false;
                        penalty = true;
                        break;
                    }
                }
            }

            if (score)
            {
                output.score = scoreUnit;
            } 

            if (penalty)
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
