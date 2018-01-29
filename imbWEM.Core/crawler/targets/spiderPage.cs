// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderPage.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.targets
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
    using imbCommonModels.pageAnalytics.core;
    using imbCommonModels.webStructure;
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
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.interfaces;
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
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbWEM.Core.crawler.targets.spiderWebElementBase" />
    /// <seealso cref="imbSCI.Data.interfaces.IObjectWithNameAndDescription" />
    /// <seealso cref="imbCommonModels.webStructure.ISpiderPage" />
    public class spiderPage:spiderWebElementBase, IObjectWithNameAndDescription, ISpiderPage
    {
        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description { get; set; } = "";


        public override string originHash
        {
            get
            {
                    if (_originHash.isNullOrEmpty())
                    {
                            if (spiderResult != null)
                            {
                                _originHash = getHash(spiderResult.task.url);
                            } else
                            {
                                _originHash = getHash(url);
                            }
                    }
                    return _originHash;
            }
        }



        private string _contentHash; // = "";
                                    /// <summary>
                                    /// Page content hash
                                    /// </summary>
        public string contentHash
        {
            get {
                if (_contentHash.isNullOrEmpty())
                {
                    if (webpage != null)
                    {
                        _contentHash = md5.GetMd5Hash(webpage.result.sourceCode, false);
                    }
                }
                return _contentHash;
            }
        }


        public spiderPage(crawledPage __webpage, int __iteration, int __iterationLoad)
        {
            webpage = __webpage;
            url = webpage.url.toStringSafe("");
            iterationDiscovery = __iteration;
            name = __webpage.name;
            captions.Add(__webpage.pageCaption);
            description = __webpage.description;
        }


        /// <summary>
        /// 
        /// </summary>
        public spiderWebPageRelationShips relationship { get; set; } = new spiderWebPageRelationShips();


        /// <summary>
        /// Spider result that discovered this page
        /// </summary>
        public spiderTaskResultItem spiderResult { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public crawledPage webpage { get; set; }
    }

}