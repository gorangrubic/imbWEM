// --------------------------------------------------------------------------------------------------------------------
// <copyright file="diagramBuilderSpiderWeb.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.reporting
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
    using imbSCI.DataComplex.diagram;
    using imbSCI.DataComplex.diagram.core;
    using imbSCI.DataComplex.diagram.enums;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.linknode;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public static class diagramBuilderSpiderWeb
    {
        /// <summary>
        /// Builds the link path hierarchy model
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selectedPages">The selected pages.</param>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        public static diagramModel buildModel(this linknodeBuilder source, List<spiderPage> selectedPages, diagramModel output = null)
        {
            if (output == null) output = new diagramModel("URL path structure", "Representation of url path structure based on detected links.", diagramDirectionEnum.LR);

            if (!imbWEMManager.settings.postReportEngine.reportBuildDoGraphs) return output;

            Dictionary<diagramNode, List<linknodeElement>> links = new Dictionary<diagramNode, List<linknodeElement>>();
            Dictionary<diagramNode, List<linknodeElement>> new_links = new Dictionary<diagramNode, List<linknodeElement>>();

            var rootNode = output.AddNode("Root" + " (" + source.root.score + ")", diagramNodeShapeEnum.circle);
            links.Add(rootNode, source.root.items.Values.ToList());
            int c = 0;
            do
            {
                new_links = new Dictionary<diagramNode, List<linknodeElement>>();

                foreach (var pair in links)
                {
                    foreach (linknodeElement el in pair.Value)
                    {
                        var parentNode = output.AddNode(el.name + " (" + el.score + ")", diagramNodeShapeEnum.rounded);
                        if (el.items.Count > 0)
                        {
                            new_links.Add(parentNode, el.items.Values.ToList());
                            output.AddLink(pair.Key, parentNode, diagramLinkTypeEnum.normal);
                        } else
                        {
                            output.AddLink(pair.Key, parentNode, diagramLinkTypeEnum.dotted);
                        }
                        c++;
                        if (c > it_limit) return output;
                    }
                }

                links = new_links;
            } while (links.Count > 0);

            return output;
        }

        public const int it_limit = 100;
        public static diagramModel buildModel(this spiderWeb source, List<spiderPage> selectedPages, diagramModel output=null)
        {
            if (output == null) output = new diagramModel("Web structure", "Representation of crawled section of the web site structure.", diagramDirectionEnum.TB);

            if (!imbWEMManager.settings.postReportEngine.reportBuildDoGraphs) return output;

            foreach (spiderPage sp in selectedPages)
            {
                string desc = sp.url + " sc(" + sp.marks.score + ")";
                output.AddNode(desc, diagramNodeShapeEnum.circle, "", sp.originHash);
            }

            foreach (spiderLink sl in source.webLinks.items.Values)
            {
                if (selectedPages.Contains(sl.targetedPage) && selectedPages.Contains(sl.originPage))
                {
                    
                    var from = output.GetNodeByHash(sl.originHash);
                    var to = output.GetNodeByHash(sl.targetedPage.originHash);

                    string desc = " i(" + sl.iterationDiscovery +") sc(" + sl.marks.score + ")";

                    output.AddLink(from, to, diagramLinkTypeEnum.normal, desc, sl.originHash);
                }
                
            }

            return output;
        }

      
    }


}
