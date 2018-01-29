// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTools.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler
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
    using imbSCI.Core.reporting.render;
    using imbSCI.Data;
    using imbSCI.Data.collection.layers;
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
    using imbWEM.Core.crawler.modules;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public static class spiderTools
    {
        public static void Describe(this ISpiderEvaluatorBase evaluator, ITextRender output)
        {
            output.AppendHeading("Crawler [" + evaluator.name + "]");

            output.AppendPair("Class name", evaluator.GetType().Name, true, ": ");

            output.AppendPair("Description", evaluator.description, true, ": ");



            if (evaluator is spiderModularEvaluatorBase)
            {
                spiderModularEvaluatorBase evaluator_spiderModularEvaluatorBase = (spiderModularEvaluatorBase)evaluator;

                foreach (var md in evaluator_spiderModularEvaluatorBase.modules)
                {
                    md.DescribeModule(output);
                }

            }



            if (evaluator is spiderEvaluatorSimpleBase)
            {
                spiderEvaluatorSimpleBase evaluator_spiderEvaluatorSimpleBase = (spiderEvaluatorSimpleBase)evaluator;

                evaluator_spiderEvaluatorSimpleBase.linkActiveRules.ToList().ForEach(x => x.DescribeRule(output));

            }


            output.AppendHorizontalLine();

            output.open("div", "General configuration", "Crawler configuration properties declared in common settings class");

            evaluator.settings.GetUserManual(output, "", true, true);

            output.close();



        }

        public static void DescribeModule(this ISpiderModuleBase module, ITextRender output)
        {
            output.open("p", "Module [" + module.name + "]", module.description);
            
            output.AppendPair("Class name", module.GetType().Name, true, ": ");

            output.AppendPair("Description", module.description, true, ": ");


            if (module is spiderLayerModuleBase)
            {
                spiderLayerModuleBase module_spiderLayerModuleBase = (spiderLayerModuleBase)module;
                foreach (var l in module_spiderLayerModuleBase.layers)
                {
                    l.DescribeLayer(output);
                }

            }


            output.AppendPair("Active rules", module.CountRules(spiderEvalRuleResultEnum.active), true, ": ");

            output.AppendPair("Passive rules", module.CountRules(spiderEvalRuleResultEnum.passive), true, ": ");

            foreach (IRuleBase md in module.rules)
            {
                md.DescribeRule(output);
            }

            module.GetUserManual(output, "", false, true);

            output.close();
        }

        public static void DescribeRule(this IRuleBase rule, ITextRender output)
        {
            output.open("p", "Rule: " + rule.name, rule.description);
            output.AppendPair("Class", rule.GetType().Name, true, ": ");
            output.AppendPair("Role", rule.role, true, ": ");
            output.AppendPair("Subject", rule.subject, true, ": ");
            output.AppendPair("Priority", rule.priority, true, ": ");
            output.AppendPair("Mode", rule.mode, true, ": ");
            output.AppendPair("Tag", rule.tagName, true, ": ");
            
            output.close();
        }

        public static void DescribeLayer(this layerCollection layer, ITextRender output)
        {
            output.open("li", "Layer: " + layer.name, layer.description);
            output.AppendPair("Slot", layer.id.ToString("D3"), true, ": ");
            output.close();
        }
    }

}