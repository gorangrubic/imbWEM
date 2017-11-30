// --------------------------------------------------------------------------------------------------------------------
// <copyright file="relationShipCollection.cs" company="imbVeles" >
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
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class relationShipCollection:IDictionary<string, spiderLink>
    {

        //public Int32 Count()
        //{
        //    return items.Count();
        //}

        public relationShipCollection(spiderLinkRelationType __relation)
        {
            relation = __relation;
        }

        public bool ContainsAsTarget(string hash)
        {
            return items.Keys.Any(x => x.EndsWith(hash));
        }

        public bool ContainsAsOrigin(string hash)
        {
            return items.Keys.Any(x => x.StartsWith(hash));
        }

        public List<spiderLink> GetTargetingTo(string hash)
        {
            List<spiderLink> output = new List<spiderLink>();
            foreach (var pair in items)
            {
                if (pair.Key.EndsWith(hash))
                {
                    output.Add(pair.Value);
                }
                
            }
            return output;
        }

        public List<spiderLink> GetOriginFrom(string hash)
        {
            List<spiderLink> output = new List<spiderLink>();
            foreach (var pair in items)
            {
                if (pair.Key.StartsWith(hash))
                {
                    output.Add(pair.Value);
                }

            }
            return output;
        }

        /// <summary> </summary>
        public Dictionary<string, spiderLink> items { get; protected set; } = new Dictionary<string, spiderLink>();


        /// <summary>
        /// 
        /// </summary>
        public spiderLinkRelationType relation { get; set; }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, spiderLink>)items).Keys;
            }
        }

        public ICollection<spiderLink> Values
        {
            get
            {
                return ((IDictionary<string, spiderLink>)items).Values;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, spiderLink>)items).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, spiderLink>)items).IsReadOnly;
            }
        }

        public spiderLink this[string key]
        {
            get
            {
                return ((IDictionary<string, spiderLink>)items)[key];
            }

            set
            {
                ((IDictionary<string, spiderLink>)items)[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, spiderLink>)items).ContainsKey(key);
        }

        //private Int32 ISSUE_COUNT_TARGETORIGINHASH = 0;


        /// <summary> </summary>
        public int ISSUE_COUNT_TARGETORIGINHASH { get; protected set; } = 0;


        public void Add(string key, spiderLink value)
        {
            if (value.targetHash == value.originHash)
            {
                ISSUE_COUNT_TARGETORIGINHASH++;
                imbWEMManager.log.log("[" + ISSUE_COUNT_TARGETORIGINHASH + "] target==origin hash->: " + value.url + " [" + value.targetHash +"]" );
            }
            else
            {
                if (items.ContainsKey(key))
                {
                    value.countOnThePage++;
                }
                else
                {
                    ((IDictionary<string, spiderLink>)items).Add(key, value);

                    imbWEMManager.log.log("[" + value.url + "] detected on [" + value.originPage.url + "] of [" + value.domain + "]");
                }
            }
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, spiderLink>)items).Remove(key);
        }

        public bool TryGetValue(string key, out spiderLink value)
        {
            return ((IDictionary<string, spiderLink>)items).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, spiderLink> item)
        {
            if (item.Value.targetHash == item.Value.originHash)
            {
                ISSUE_COUNT_TARGETORIGINHASH++;
                aceLog.loger.AppendLine("[" + ISSUE_COUNT_TARGETORIGINHASH + "] target==origin hash->: " + item.Value.url + " [" + item.Value.targetHash + "]");
            }
            else
            {
                ((IDictionary<string, spiderLink>)items).Add(item);
            }
        }

        public void Clear()
        {
            ((IDictionary<string, spiderLink>)items).Clear();
        }

        public bool Contains(KeyValuePair<string, spiderLink> item)
        {
            return ((IDictionary<string, spiderLink>)items).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, spiderLink>[] array, int arrayIndex)
        {
            ((IDictionary<string, spiderLink>)items).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, spiderLink> item)
        {
            return ((IDictionary<string, spiderLink>)items).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, spiderLink>> GetEnumerator()
        {
            return ((IDictionary<string, spiderLink>)items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, spiderLink>)items).GetEnumerator();
        }
    }

}