// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexAssertionBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.core
{
    using System;
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
    using imbSCI.Core.math;
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

    public abstract class indexAssertionBase<TEnum, T>:aceEnumListSet<TEnum, T> where TEnum : IComparable
    {
        public indexAssertionBase()
        {

        }
 
        protected List<T> items { get; set; } = new List<T>();
        protected Dictionary<T, TEnum> flagsByItem { get; set; } = new Dictionary<T, TEnum>();

        public abstract TEnum FlagEvaluated { get; }
        public abstract TEnum FlagRelevant { get; }
        public abstract TEnum FlagIndexed { get; }


        protected abstract void recalculateCustom();

        protected void recalculate()
        {
            int c = items.Count();
            _certainty = this[FlagEvaluated].Count().GetRatio(c);
            _relevant = this[FlagRelevant].Count().GetRatio(this[FlagEvaluated].Count());
            _indexCoverage = this[FlagIndexed].Count().GetRatio(c);

            recalculateCustom();

            AcceptChanges();
        }


        public int CountUnique()
        {
            return items.Count();
        }

        private double _certainty = 0;
        /// <summary>How Certain is the answer regarding the relevance <see cref="relevant"/></summary>
        public double certainty
        {
            get
            {
                if (haveChange) recalculate();
                return _certainty;
            }
            protected set
            {
                _certainty = value;

            }
        }

        /// <summary>
        /// Gets the not relevant ratio
        /// </summary>
        /// <value>
        /// The not relevant.
        /// </value>
        public double notRelevant
        {
            get
            {
                return (1 - relevant);
            }
        }

        private double _relevant;
        /// <summary> relevant - precission </summary>
        public double relevant
        {
            get
            {
                if (haveChange) recalculate();
                return _relevant;
            }
            protected set
            {
                _relevant = value;
            }
        }

        private double _indexCoverage;
        /// <summary> Rate at witch evaluated items have their entry in the index </summary>
        public double indexCoverage
        {
            get
            {
                if (haveChange) recalculate();
                return _indexCoverage;
            }
            protected set
            {
                _indexCoverage = value;
            }
        }


        /// <summary>
        /// Appends the specified flags to the link in case it exists. Unline <see cref="Add(TEnum, T)"/> it doesn't remove existing record on the same link
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="link">The link.</param>
        public void Append(TEnum flags, T link)
        {

            base.Add(flags, link);
        }


        /// <summary>
        /// Support Flags and Enums -- automatically maintains if link was passed with new flags
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="link">The link.</param>
        public override void Add(TEnum flags, T link)
        {
            if (items.Contains(link))
            {
                Remove(link);
            }
            else
            {
                items.Add(link);
                flagsByItem.Add(link, flags);
            }
            base.Add(flags, link);
        }
    }

}