// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ruleUrlTitleMatch.cs" company="imbVeles" >
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
    public class ruleUrlTitleMatch : spiderEvalRuleForLinkBase
    {
        public override spiderEvalRuleRoleEnum role
        {
            get
            {
                return spiderEvalRuleRoleEnum.rankScoring;
            }
        }


        public ruleUrlTitleMatch(spiderEvaluatorSimpleBase __parent) : base("Url vs Title", "If url and link title have a word in common; double reward if all title words are found in url", 2,-1, __parent)
        {

        }

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult output = new spiderEvalRuleResult(this);

            string cp = link.link.caption;

            if (cp.isNullOrEmpty())
            {
                output.comment = "caption is empty";
                return output;
            }

            //if (cp.IsEmptyOrWhiteSpace())
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



            List<string> words = link.link.caption.getStringTokens();
            string url = link.link.pathAndQuery;

            
            //url = url.Replace(link.link.domain, "");

            bool score = false;
            bool doubleScore = true;


            output.comment = "url[" + url + "]";

            foreach (string wrd in words)
            {
                if (url.Contains(wrd))
                {
                    output.comment += "[" + wrd + "]ok ";
                    score = true;
                } else
                {
                    output.comment += "[" + wrd + "]no ";
                    doubleScore = false;
                }
            }
            if (score)
            {
                output.score = scoreUnit;

                if (doubleScore)
                {
                    output.score += scoreUnit;
                }

            } else
            {
                output.score = penaltyUnit;
            }
            return output;
        }

        public override void prepare()
        {
            // -- nothing to prepare
        }
    }
}