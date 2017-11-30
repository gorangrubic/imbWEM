// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderWebPageRegistry.cs" company="imbVeles" >
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
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class spiderWebPageRegistry:IWebRegistries
    {
        public int Count()
        {
            return items.Count();
        }


        /// <summary> </summary>
        public List<string> hashList { get; protected set; } = new List<string>();


        public spiderPage this[string hash]
        {
            get
            {
                return items[hash];
            }
        }

        /// <summary>
        /// Gets the page by link targeting the same url
        /// </summary>
        /// <param name="sLink">The s link.</param>
        /// <returns></returns>
        public spiderPage GetPageByLink(spiderLink sLink)
        {
            if (items.ContainsKey(sLink.targetHash))
            {
                return items[sLink.targetHash];
            }

            if (sLink.targetedPage != null)
            {
                return sLink.targetedPage;
            }

            return null;
        }

        public const bool useCription = true;
        /// <summary>
        /// Adds the specified link if not already known
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns></returns>
        public bool Add(spiderPage page)
        {
            string key = page.getPageSignature(useCription);
            if (items.ContainsKey(key))
            {

                if (items[key].webpage.isCrawled == false)
                {
                    if (page.webpage.isCrawled)
                    {
                        items[key] = page;
                       imbWEMManager.log.log("Temp. spiderPage [" + items[key].name + "] replaced by new instance for domain [" + page.webpage.domain + "]");
                    } else
                    {
                        aceLog.log("---- This page is not crawled yet and it cannot substitute registrated one ----");
                       // throw new aceGeneralException("This page is not crawled yet and it cannot substitute registrated one", null, this, "New page under same key is not crawled");
                    }
                } else
                {
                    aceLog.log("---- shouldn't replace existing page if it is crawled yet ----");
                    //throw new aceGeneralException("You shouldn't replace existing page if it is crawled yet", null, this, "Page under same key is crawled");
                }

                
                return false;
            }
            else
            {
                items.Add(key, page);
                hashList.Add(key);
                return true;
            }

        }

        /// <summary>Page targets</summary>
        public Dictionary<string, spiderPage> items { get; protected set; } = new Dictionary<string, spiderPage>();
    }

}