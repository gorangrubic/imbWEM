// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTargetCollection.cs" company="imbVeles" >
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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.contentBlock;
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
    using imbSCI.Data.collection.math;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.interfaces;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Web context targets collection
    /// </summary>
    /// <seealso cref="IObjectWithName" />
    /// <seealso cref="IObjectWithDescription" />
    /// <seealso cref="IObjectWithNameAndDescription" />
    public class spiderTargetCollection:IObjectWithName, IObjectWithDescription, IObjectWithNameAndDescription, IEnumerable<spiderTarget>, ISpiderTargetCollection, IEnumerable<ISpiderTarget>
    {


        /// <summary>
        /// 
        /// </summary>
        public List<string> termSerbian { get; set; } = new List<string>();


        /// <summary>
        /// 
        /// </summary>
        public List<string> termOther { get; set; } = new List<string>();


        /// <summary>
        /// 
        /// </summary>
        public List<string> termsAll { get; set; } = new List<string>();


        /// <summary>
        /// 
        /// </summary>
        public blockRegistry blocks { get; set; } = new blockRegistry();


        /// <summary>
        /// Gets the data table of all targets
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            DataTable output = new DataTable(wRecord.domain.getFilename());

            output.Add(nameof(spiderTarget.url), "Resolved url");
            output.Add("VectorIn", "Number of incoming links", "Vin_c", typeof(int));
            output.Add("VectorOut", "Number of outcoming links", "Vout_c", typeof(int));
            output.Add(nameof(spiderPage.iterationDiscovery), "Iteration discovery", "I_d", typeof(int));
            output.Add(nameof(spiderPage.iterationLoaded), "Iteration loaded", "I_l", typeof(int));
            output.Add(nameof(spiderPage.contentHash), "Content hash", "H");

            foreach (spiderTarget target in items.Values)
            {
                if (target.page != null)
                {
                    output.AddRow(target.url, target.linkVectors.Count(), target.page.relationship.outflowLinks.Count(), target.iterationDiscovery, target.iterationLoaded, target.page.contentHash);
                } else
                {
                    output.AddRow(target.url, target.linkVectors.Count(), 0, target.iterationDiscovery, target.iterationLoaded, "");
                }
            }

            return output;
        }


        /// <summary>
        /// 
        /// </summary>
        public aceRelationMatrix<ISpiderTarget, ISpiderTarget, int> linkMatrix { get; set; }

        public int getLinkCount(ISpiderTarget itemX, ISpiderTarget itemY)
        {
            int output = 0;

            spiderPage page = itemX.page as spiderPage;

            List<spiderLink> links = page.relationship.outflowLinks.GetTargetingTo(itemY.targetHash);

            return output = links.Count;
            //wRecord.web
        }

        public int getLinkCountRotated(ISpiderTarget itemY, ISpiderTarget itemX)
        {
            int output = 0;
            spiderPage page = itemX.page as spiderPage;

            List<spiderLink> links = page.relationship.outflowLinks.GetTargetingTo(itemY.targetHash);

            return output = links.Count;
            //wRecord.web
        }


        /// <summary>
        /// Gets the ace matrix rotated: X axis are unvisited targets, Y axis are visited
        /// </summary>
        /// <returns></returns>
        public imbTargetLinkMatrix GetAceMatrixRotated()
        {
            var loaded = GetLoaded();

            if (loaded.Any())
            {
                return  new imbTargetLinkMatrix(items.Values, loaded, getLinkCountRotated);
            }
            else
            {

                return null;
            }
        }

        public ArrayList GetLinkMatrixRotated()
        {
            var loaded = GetLoaded();

            if (loaded.Any())
            {
                linkMatrix = new aceRelationMatrix<ISpiderTarget, ISpiderTarget, int>(items.Values, loaded, getLinkCountRotated);
                return linkMatrix.GetMatrix();
            } else
            {

                return null;
            }
        }


        public ArrayList GetLinkMatrix()
        {
            var loaded = GetLoaded();

            if (loaded.Any())
            {
                linkMatrix = new aceRelationMatrix<ISpiderTarget, ISpiderTarget, int>(loaded, items.Values, getLinkCount);
                return linkMatrix.GetMatrix();
            } else
            {

                return null;
            }
            
        }

        public spiderTarget GetByURL(string url)
        {
            string k = GetHash(url);
            return items[k];
        }



        /// <summary>
        /// Gets targets loaded in order of load
        /// </summary>
        /// <returns></returns>
        public List<spiderTarget> GetLoadedInOrderOfLoad()
        {
            List<spiderTarget> output = GetLoaded();

            output.Sort((x, y) => x.iterationLoaded.CompareTo(y.iterationLoaded));

            return output;
        }


        /// <summary>
        /// Gets targets loaded in the specified iteration
        /// </summary>
        /// <param name="iteration">The iteration.</param>
        /// <returns></returns>
        public List<spiderTarget> GetLoadedInIteration(int iteration)
        {
            List<spiderTarget> output = new List<spiderTarget>();
            foreach (var pair in items)
            {
                if (pair.Value.isLoaded)
                {
                    if (pair.Value.iterationLoaded == iteration)
                    {
                        output.Add(pair.Value);
                    }
                }
            }
            return output;
        }


        /// <summary>
        /// Returns all loaded targets, in order of target detection
        /// </summary>
        /// <returns></returns>
        public List<spiderTarget> GetLoaded()
        {
            List<spiderTarget> output = new List<spiderTarget>();
            foreach (var pair in items)
            {
                if (pair.Value.isLoaded)
                {
                    output.Add(pair.Value);
                }
            }
            return output;
        }

        public spiderTargetCollection(modelSpiderSiteRecord __wRecord)
        {
            wRecord = __wRecord;
            string __domain = wRecord.domain;
            string __spiderName = wRecord.spider.name;

            name = __spiderName + " targets on " + __domain;
            description = "Registry of unique absolute URLs discovered on the web site: " + __domain + " by the " + __spiderName + " crawler";

            dlTargetLinkTokens = new termDocumentSet(GetHash("links_" + __domain + " " + __spiderName), "URL and anchor text tokens from links discovered on the web site: " + __domain + " by the " + __spiderName + " crawler");
            if (wRecord.tRecord.instance.settings.doEnableDLC_TFIDF)
            {
                dlTargetPageTokens = new termDocumentSet(GetHash("pages_" + __domain + " " + __spiderName), "Content text tokens from loaded pages the web site: " + __domain + " by the " + __spiderName + " crawler");
            }
        }


        /// <summary> </summary>
        public modelSpiderSiteRecord wRecord { get; protected set; }


        /// <summary>
        /// Name for this instance
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public string description { get; set; } = "";


        /// <summary> </summary>
        public termDocumentSet dlTargetLinkTokens { get; protected set; }


        /// <summary> </summary>
        public termDocumentSet dlTargetPageTokens { get; set; }


        /// <summary>
        /// Attaches the page and performes content decomposition
        /// </summary>
        /// <param name="pg">The pg.</param>
        /// <param name="response">The response.</param>
        /// <param name="targetBlockCount">The target block count.</param>
        /// <returns></returns>
        public spiderTarget AttachPage(spiderTaskResultItem pg, ILogBuilder response, int targetBlockCount=3)
        {
            string key = GetHash(pg.target.url);
            spiderTarget target = null;

            target = GetOrCreateTarget(pg.target, true, false);
            target.AttachPage(pg.sPage,response, targetBlockCount);

            if (target.contentBlocks.Any())
            {
                foreach (var bl in target.contentBlocks)
                {
                    blocks.Add(bl);
                    //blockContentHashList.AddUnique(bl_hash);
                }
            }

            return target;
        }

        /// <summary>
        /// Gets the or create target.
        /// </summary>
        /// <param name="linkVector">The link vector.</param>
        /// <returns></returns>
        public spiderTarget GetOrCreateTarget(spiderLink linkVector, bool autoAdd=true, bool processVector=true)
        {
            string key = GetHash(linkVector.url);
            spiderTarget target = null;

            if (items.ContainsKey(key))
            {
                target = items[key];
                if (processVector) target.AddVector(linkVector);
            } else
            {
                target = new spiderTarget(linkVector, this);
                if (autoAdd)
                {
                    items.Add(key, target);
                }
            }
            
            return target;
        }

        /// <summary>
        /// Gets the link by origin
        /// </summary>
        /// <param name="linkVector">The link vector.</param>
        /// <returns></returns>
        public spiderTarget GetByOrigin(spiderLink linkVector)
        {
            string key = GetHash(linkVector.originPage.url);
            if (items.ContainsKey(key)) { 
                return items[key];
            } else
            {
                return null;
            }
        }

        public spiderTarget GetByTarget(spiderLink linkVector)
        {
            string key = GetHash(linkVector.url);
            spiderTarget target = null;

            if (items.ContainsKey(key))
            {
                target = items[key];
            }
            else
            {
                
            }
            return target;
        }

        public spiderLinkFlags GetFlags(spiderLink linkVector)
        {
            spiderLinkFlags output = spiderLinkFlags.none;
            string key = GetHash(linkVector.url);
            if (!items.ContainsKey(key))
            {
                output = spiderLinkFlags.newlinkTarget;

                output |= spiderLinkFlags.newlinkVector;

            } else
            {
                output = spiderLinkFlags.oldlinkTarget;

                string vkey = GetKeyForVector(linkVector);
                if (items[key].linkVectors.ContainsKey(vkey))
                {
                    output |= spiderLinkFlags.oldlinkVector;
                }
                else
                {
                    output |= spiderLinkFlags.newlinkVector;
                }
            }

            return output;
        }


        public string GetHash(string url)
        {
            url = wRecord.domainInfo.GetURLWithoutDomainName(url);
            url = url.Replace("www.", "");
            return url.Replace(".", "-").getCleanFilePath();
        }

        public string GetKeyForVector(spiderLink linkVector)
        {
            return md5.GetMd5Hash(linkVector.originPage.url + linkVector.captions.toCsvInLine() + linkVector.urls);
        }

        public IEnumerator<spiderTarget> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        int ISpiderTargetCollection.getLinkCount(ISpiderTarget itemX, ISpiderTarget itemY)
        {
            return getLinkCount((spiderTarget)itemX, (spiderTarget)itemY);
        }

        int ISpiderTargetCollection.getLinkCountRotated(ISpiderTarget itemY, ISpiderTarget itemX)
        {
            return getLinkCountRotated((spiderTarget)itemY, (spiderTarget)itemX);
        }

        ISpiderTarget ISpiderTargetCollection.GetByURL(string url)
        {
            return (ISpiderTarget)GetByURL(url);
        }

        List<ISpiderTarget> ISpiderTargetCollection.GetLoadedInOrderOfLoad()
        {
            return (List<ISpiderTarget>)GetLoadedInOrderOfLoad().ConvertIList<spiderTarget, ISpiderTarget>();
        }

        List<ISpiderTarget> ISpiderTargetCollection.GetLoadedInIteration(int iteration)
        {
            return (List<ISpiderTarget>)GetLoadedInIteration(iteration).ConvertIList<spiderTarget, ISpiderTarget>();
        }

        List<ISpiderTarget> ISpiderTargetCollection.GetLoaded()
        {
            return (List<ISpiderTarget>)GetLoaded().ConvertIList<spiderTarget, ISpiderTarget>();
        }

        ISpiderTarget ISpiderTargetCollection.GetByOrigin(ISpiderLink linkVector)
        {
            return GetByOrigin((spiderLink)linkVector);
        }

        ISpiderTarget ISpiderTargetCollection.GetByTarget(ISpiderLink linkVector)
        {
            return GetByTarget((spiderLink)linkVector);
        }

        spiderLinkFlags ISpiderTargetCollection.GetFlags(ISpiderLink linkVector)
        {
            return GetFlags((spiderLink)linkVector);
        }

        string ISpiderTargetCollection.GetKeyForVector(ISpiderLink linkVector)
        {
            return GetKeyForVector((spiderLink)linkVector);
        }

        IEnumerator<ISpiderTarget> IEnumerable<ISpiderTarget>.GetEnumerator()
        {
            List<ISpiderTarget> output = new List<ISpiderTarget>();
            foreach (var tpair in items)
            {
                output.Add(tpair.Value);
            }
            return output.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, spiderTarget> items { get; set; } = new Dictionary<string, spiderTarget>();

        IWeightTableSet ISpiderTargetCollection.dlTargetLinkTokens
        {
            get
            {
                return dlTargetLinkTokens;
            }
        }

        IWeightTableSet ISpiderTargetCollection.dlTargetPageTokens
        {
            get
            {
                return dlTargetPageTokens;
            }

           
        }
    }

}