// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webSiteSimpleSample.cs" company="imbVeles" >
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
namespace imbWEM.Core.console
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Network.tools;
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
    using imbSCI.Core.extensions.data;
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
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;
    using imbCommonModels.webStructure;
    using imbSCI.Data.interfaces;
    using System;

    /// <summary>
    /// Simple web site profile sample collection
    /// </summary>
    public class webSiteSimpleSample:IEnumerable<string>, IObjectWithNameAndDescription
    {
        public webSiteSimpleSample()
        {

        }


        private String _name = "";
        /// <summary>
        /// Name for this instance
        /// </summary>
        public String name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _description = "";
        /// <summary>
        /// Human-readable description of object instance
        /// </summary>
        public String description
        {
            get { return _description; }
            set { _description = value; }
        }



        /// <summary> </summary>
        protected List<indexDomain> indexDomains { get; set; } = new List<indexDomain>();

        public bool hasIndexDomains
        {
            get
            {
                return indexDomains.Any();
            }
        }

        public List<indexDomain> getIndexDomains()
        {
            return indexDomains;
        }

        public static implicit operator List<string>(webSiteSimpleSample sample)
        {
            List<string> output = new List<string>();

            foreach (string pr in sample)
            {
                output.Add(pr);
            }

            return output;
        }

        public static implicit operator List<webSiteProfile>(webSiteSimpleSample sample)
        {
            List<webSiteProfile> output = new List<webSiteProfile>();
            foreach (string d in sample.domains) output.Add(new webSiteProfile(d));
            
            return output;
        }

        public static implicit operator webSiteSimpleSample(List<webSiteProfile> sample)
        {
            webSiteSimpleSample output = new webSiteSimpleSample();

            foreach (webSiteProfile pr in sample)
            {
                output.Add(pr.url);
            }

            return output;
        }

        [XmlIgnore]
        public int Count
        {
            get
            {
                return domains.Count;
            }
        }

        public int Add(List<indexDomain> __indexDomains, int start = 0, int take = -1)
        {
            int c = 0;
            int i = 0;
            if (take == -1)
            {
                take = __indexDomains.Count - start;
            }
            
            foreach (indexDomain domain in __indexDomains)
            {
                if ((i >= start) && (i < (start + take)))
                {
                    domainAnalysis da = new domainAnalysis(domain.url);
                    if (Add(da.urlProper))
                    {
                        indexDomains.Add(domain);
                        c++;
                    }
                }
                i++;
            }
            return c;
        }

        public int Add(List<string> source, int start = 0, int take = -1)
        {
            int c = 0;
            int i = 0;
            if (take == -1)
            {
                take = source.Count - start;
            }

            foreach (string domain in source)
            {
                if ((i >= start) && (i < (start + take)))
                {
                    domainAnalysis da = new domainAnalysis(domain);
                    if (Add(da.urlProper))
                    {
                        c++;
                    }
                }
                i++;
            }
            return c;
        }

        public void Randomize()
        {
            if (domains.Any()) domains.Randomize();
            if (indexDomains.Any()) indexDomains.Randomize();
        }

        /// <summary>
        /// Adds any new domains from the source
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public int Add(webSiteSimpleSample source, int start=0, int take=-1)
        {
            int c = 0;
            int i = 0;
            if (take == -1)
            {
                take = source.Count - start;
            }

            foreach (string domain in source.domains)
            {
                if ((i >= start) && (i < (start + take))) {
                    domainAnalysis da = new domainAnalysis(domain);
                    if (Add(da.urlProper))
                    {
                        c++;
                    }
                }
                i++;
            }
            return c;
        }

        public void Add(object item)
        {
            Add(item.toStringSafe());
        }

        public bool Add(string url)
        {
            if (!url.isNullOrEmpty())
            {
                if (!domains.Contains(url))
                {
                    domains.Add(url);
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)domains).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<string>)domains).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> domains { get; set; } = new List<string>();
    }

}