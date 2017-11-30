// --------------------------------------------------------------------------------------------------------------------
// <copyright file="instanceByIteration.cs" company="imbVeles" >
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
namespace imbWEM.Core.directReport.data
{
    using System.Collections;
    using System.Collections.Concurrent;
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
    using imbWEM.Core.stage;

    /// <summary>
    /// Serializable instance by iteration
    /// </summary>
    /// <typeparam name="TInstance">The type of the instance.</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.Int32, System.Collections.Generic.List{TInstance}}}" />
    public abstract class instanceByIteration<TInstance>:IEnumerable<instanceList<TInstance, int>>
    {
        public int Count
        {
            get
            {
                int c = 0;
                foreach (instanceList<TInstance, int> p in this)
                {
                    c = c + p.Count;
                }
                return c;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        public instanceByIteration(string __name)
        {
            name = __name;
        }

        public instanceByIteration()
        {

        }

        public List<TInstance> GetAllNonUnique()
        {
            List<TInstance> output = new List<TInstance>();
            foreach (instanceList<TInstance, int> p in this)
            {
                output.AddRange(p);
            }
            return output;
        }

        public List<TInstance> GetAllUnique()
        {
            List<TInstance> output = new List<TInstance>();
            foreach (instanceList<TInstance, int> p in this)
            {
                output.AddRange(p);
            }
            return output;
        }


       

        public instanceList<TInstance, int> this[int iteration]
        {
            get
            {
                return items.GetOrAdd(iteration, new instanceList<TInstance, int>(iteration));
            }
        }

        public void Add(object pair)
        {
            if (pair is instanceList<TInstance, int>)
            {
                instanceList<TInstance, int> p = (instanceList<TInstance, int>)pair;
                items.GetOrAdd(p.key, p);
            }
        }

        /// <summary> </summary>
        protected ConcurrentDictionary<int, instanceList<TInstance, int>> items { get; set; } = new ConcurrentDictionary<int, instanceList<TInstance, int>>();


        IEnumerator<instanceList<TInstance, int>> IEnumerable<instanceList<TInstance, int>>.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }
    }

}