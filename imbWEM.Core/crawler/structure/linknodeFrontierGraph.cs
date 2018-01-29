// --------------------------------------------------------------------------------------------------------------------
// <copyright file="linknodeFrontierGraph.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.structure
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
    using imbSCI.DataComplex.linknode;
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
    /// Frontier graph implementation for <see cref="layerTargetUrlGraph"/>
    /// </summary>
    public class linknodeFrontierGraph
    {
        public void learn(ISpiderElement element)
        {
            spiderLink link = element as spiderLink;
            if (link != null) Gt.Add(link.url, link);

        }


        public linknodeElement GetLinkNode(string url)
        {
            if (Gd == null) return null;
            if (Gd.sourceNodes.ContainsKey(url))
            {
                return Gd.sourceNodes[url];
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public int bestNodeSearchLimit { get; set; } = 50;

        public void onStartIteration(modelSpiderSiteRecord wRecord)
        {
            Gt = new linknodeBuilder();
            Gc = new linknodeBuilder();

            foreach (spiderTarget target in wRecord.context.targets)
            {
                if (target.isLoaded)
                {
                    Gc.Add(target.url, target, 2);
                } else
                {
                    Gt.Add(target.url, target);
                }
                
            }

            Gd = null;
        }

        public  void prepare()
        {
            Gc = new linknodeBuilder();
            Gb = new linknodeBuilder();
            Gt = new linknodeBuilder();
            Gd = null;
        }

        protected void rebuildGd()
        {
            Gd = new linknodeBuilder();

            foreach (var pair in Gt.newpathNodes)
            {

                string path = pair.Key;
                int score = pair.Value.score;

                if (Gc.newpathNodes.Count() > 0)
                {
                    if (Gc.newpathNodes.ContainsKey(path))
                    {
                        score = score / Gc.newpathNodes[path].score;
                    }
                }
                if (Gb.newpathNodes.ContainsKey(path))
                {
                    score = score + Gb.newpathNodes[path].score;
                }
                string opath = Gt.newpathNodes[path].originalPath;
                Gd.Add(opath, Gt.newpathNodes[path].meta, score);

            }

            int sc = int.MinValue;
            linknodeElement scBest = null;
            foreach (var nd in Gd.sourceNodeList)
            {
                if (nd.score > sc)
                {
                    sc = nd.score;
                    scBest = nd;
                }
                
            }

            bestNode = scBest;
        }

        public void buildGd()
        {
            int i = 0;

            rebuildGd();

            if (bestNode == null) return;

            while (!bestNode.haveMeta())
            {
                i++;

                int bsc = bestNode.score;

                Gb.Add(bestNode.originalPath, bestNode.meta, -bsc);

                int bscp = bsc / bestNode.items.Count;

                foreach (var chld in bestNode.items)
                {
                    Gb.Add(chld.Value.originalPath, chld.Value.meta, bscp);
                }

                rebuildGd();

                if (i > bestNodeSearchLimit) break;
            }
        }

        /// <summary> </summary>
        public linknodeBuilder Gt { get; protected set; } = new linknodeBuilder();


        /// <summary> </summary>
        public linknodeBuilder Gd { get; protected set; } = new linknodeBuilder();


        /// <summary> </summary>
        public linknodeBuilder Gb { get; protected set; } = new linknodeBuilder();


        /// <summary> </summary>
        public linknodeBuilder Gc { get; protected set; } = new linknodeBuilder();


        /// <summary>
        /// 
        /// </summary>
        public linknodeElement bestNode { get; set; }
    }
}
