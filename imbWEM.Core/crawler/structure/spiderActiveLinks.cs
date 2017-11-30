// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderActiveLinks.cs" company="imbVeles" >
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
    using System;
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

    /// <summary>
    /// Self-managed collection of active links
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IList{imbAnalyticsEngine.spider.core.spiderLink}" />
    /// <seealso cref="System.Collections.IList" />
    public class spiderActiveLinks:IList<spiderLink>, IList
    {
        public spiderLink this[int index]
        {
            get
            {
                return ((IList<spiderLink>)items)[index];
            }

            set
            {
                ((IList<spiderLink>)items)[index] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)items)[index];
            }

            set
            {
                ((IList)items)[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IList<spiderLink>)items).Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IList)items).IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<spiderLink>)items).IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((IList)items).IsSynchronized;
            }
        }

        /// <summary> </summary>
        public List<spiderLink> items { get; protected set; } = new List<spiderLink>();

        public object SyncRoot
        {
            get
            {
                return ((IList)items).SyncRoot;
            }
        }

        public int Add(object value)
        {
            return ((IList)items).Add(value);
        }

        public void Add(spiderLink item)
        {
            ((IList<spiderLink>)items).Add(item);
        }

        public void Clear()
        {
            ((IList<spiderLink>)items).Clear();
        }

        public bool Contains(object value)
        {
            return ((IList)items).Contains(value);
        }

        public bool Contains(spiderLink item)
        {
            return ((IList<spiderLink>)items).Contains(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)items).CopyTo(array, index);
        }

        public void CopyTo(spiderLink[] array, int arrayIndex)
        {
            ((IList<spiderLink>)items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<spiderLink> GetEnumerator()
        {
            return ((IList<spiderLink>)items).GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return ((IList)items).IndexOf(value);
        }

        public int IndexOf(spiderLink item)
        {
            return ((IList<spiderLink>)items).IndexOf(item);
        }

        public void Insert(int index, object value)
        {
            ((IList)items).Insert(index, value);
        }

        public void Insert(int index, spiderLink item)
        {
            ((IList<spiderLink>)items).Insert(index, item);
        }

        public void Remove(object value)
        {
            ((IList)items).Remove(value);
        }

        public bool Remove(spiderLink item)
        {
            return ((IList<spiderLink>)items).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<spiderLink>)items).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<spiderLink>)items).GetEnumerator();
        }
    }

}