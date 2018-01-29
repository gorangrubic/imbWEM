// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderEvalRuleCollection.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.core
{
    using System.Collections;
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
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    //public class spiderControlRuleCollection:spiderEvalRuleCollection
    //{

    //}

    /// <summary>
    /// Collection of rules
    /// </summary>
    /// <seealso cref="System.Collections.IEnumerable" />
    public class spiderEvalRuleCollection:IEnumerable, IEnumerable<IRuleBase>
    {

        /// <summary>
        /// Gets the rules according to the specified filters. <see cref="spiderEvalRuleResultEnum.none"/> and <see cref="spiderEvalRuleRoleEnum.none"/> are wilcards
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public List<IRuleBase> GetRules(spiderEvalRuleResultEnum mode, spiderEvalRuleRoleEnum role, spiderEvalRuleSubjectEnum subject)
        {
            List<IRuleBase> output = new List<IRuleBase>();

            foreach (IRuleBase rule in this)
            {
                bool accept = true;
                if (mode != spiderEvalRuleResultEnum.none)
                {
                    if (rule.mode != mode)
                    {
                        accept = false;
                        continue;
                    }
                }
                if (role != spiderEvalRuleRoleEnum.none)
                {
                    if (accept)
                    {
                        if (rule.role != role)
                        {
                            accept = false;
                            continue;
                        }
                    }
                }
                if (subject != spiderEvalRuleSubjectEnum.none)
                {
                    if (accept)
                    {
                        if (rule.subject != subject)
                        {
                            accept = false;
                            continue;
                        }
                    }
                }

                if (accept)
                {
                    output.Add(rule);
                }
            }

            return output;
        }

        public int Count()
        {
            return items.Count();
        }

        public void Add(IRuleBase rule)
        {
            items.Add(rule);
        }

        /// <summary>
        /// Prepares the collection - clears all temporary data
        /// </summary>
        public void prepare()
        {
            foreach (IRuleBase item in items)
            {
                item.prepare();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<IRuleBase> items { get; set; } = new List<IRuleBase>();

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        IEnumerator<IRuleBase> IEnumerable<IRuleBase>.GetEnumerator()
        {
            return ((IEnumerable<IRuleBase>)items).GetEnumerator();
        }
    }

}