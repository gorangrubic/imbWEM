// --------------------------------------------------------------------------------------------------------------------
// <copyright file="layerTargetUrlGraph.cs" company="imbVeles" >
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
    using imbWEM.Core.crawler.structure;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;
    using imbSCI.DataComplex.linknode;

    public class layerTargetUrlGraph : layerDistributionActiveRuleBase
    {
        /// <summary>
        /// 
        /// </summary>
        public linknodeFrontierGraph tree { get; set; } = new linknodeFrontierGraph();


        /// <summary>
        /// 
        /// </summary>
        public int layer3ID { get; set; }


        public layerTargetUrlGraph(int __layerID, int __layer2ID, int __layer3ID, ISpiderEvaluatorBase __parent)
            : base("Target URL Graph", "In the Learning phase creates link-path graph out of all links sent for evaluation, according to internal score algorithm it will assign [{0}], [{1}] or [{2}].", __layerID, __parent, __layer2ID)
        {
            description = string.Format(description, __layerID, __layer2ID, __layer3ID);
            layer3ID = __layer3ID;
        }

        

        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            spiderEvalRuleResult result = new spiderEvalRuleResult(this);

            if (tree.Gd == null) tree.buildGd();

            if (tree.Gd == null)
            {
                result.layer= layer3ID;
                return result;
            }

            linknodeElement linkNode = tree.GetLinkNode(link.url); //.Gd.sourceNodes[link.url];

            if (linkNode == null)
            {
                result.layer = layer3ID;
                return result;
            }
            if (tree.bestNode == linkNode)
            {
                result.layer = layerID;
            } else if (tree.bestNode.items.Values.Contains(linkNode))
            {
                result.layer = layerID;
            } else if (tree.bestNode.items.Values.Any(x=>x.items.Values.Contains(linkNode)))
            {
                result.layer = layer2ID;
            } else
            {
                result.layer = layer3ID;
            }

            return result;

        }

        public override void learn(ISpiderElement element)
        {
            tree.learn(element);

        }

        public override void onStartIteration()
        {
            tree.onStartIteration(wRecord);
        }

        public override void prepare()
        {
            tree = new linknodeFrontierGraph();
            tree.prepare();
        }
    }

}