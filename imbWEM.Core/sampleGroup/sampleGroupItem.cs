// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sampleGroupItem.cs" company="imbVeles" >
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
namespace imbWEM.Core.sampleGroup
{
    using System;
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
    using imbSCI.Data.enums.fields;
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
    /// Group definition - part of a <see cref="sampleGroupSet"/>
    /// </summary>
    public class sampleGroupItem : imbBindable, IAppendDataFields
    {

        
        /// <summary> </summary>
        public int TotalCount
        {
            get
            {
                return parent.TotalCount;
            }
        }


        private sampleGroupSet _parent ;
        /// <summary> </summary>
        public sampleGroupSet parent
        {
            get
            {
                return _parent;
            }
            internal set
            {
                _parent = value;
                OnPropertyChanged("parent");
            }
        }


        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        public PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();

            data.Add(templateFieldBasic.sample_group, groupTag, "Database tag", "Tag string to mark sample item row in the database table");
            data.Add("groupweight", weight, "Weight factor", "Relative weight number used for automatic population-to-group assigment");
            data.Add(templateFieldBasic.sample_limit, groupSizeLimit, "Size limit", "Optional limit for total count of population within this group");
            data.Add(templateFieldBasic.sample_totalcount, count, "Sample count", "Sample item entries count attached to this group");
            data.Add("isclosed", isClosed, "Group closed", "TRUE if the group will not accept any new item");

            return data;
        }

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>
        /// Updated or newly created property collection
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        PropertyCollection IAppendDataFields.AppendDataFields(PropertyCollection data)
        {
            return AppendDataFields(data as PropertyCollectionExtended);
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="sampleGroupItem"/> class with default configuration
        /// </summary>
        public sampleGroupItem()
        {

        }

        /// <summary>
        /// Defines a <see cref="sampleGroupItem"/> 
        /// </summary>
        /// <param name="title">Descriptive title of the group</param>
        /// <param name="tag">Short code_safe string to be used in database for this group</param>
        /// <param name="weightFactor">Relative weight number used for automatic population-to-group assigment</param>
        /// <param name="limit">Optional limit for total count of population within this group</param>
        public sampleGroupItem(string title, string tag, int weightFactor=1, int limit = -1)
        {
            groupTag = tag;
            groupTitle = title;
            weight = weightFactor;
            groupSizeLimit = limit;
        }

        /// <summary>
        /// Defines a <see cref="sampleGroupItem"/> 
        /// </summary>
        /// <param name="title">Descriptive title of the group</param>
        /// <param name="tag">Enum member to be used in database to point this group</param>
        /// <param name="weightFactor">Relative weight number used for automatic population-to-group assigment</param>
        /// <param name="limit">Optional limit for total count of population within this group</param>
        public sampleGroupItem(string title, Enum tag, int weightFactor = 1, int limit = -1)
        {
            groupTag = tag.ToString();
            groupTitle = title;
            weight = weightFactor;
            groupSizeLimit = limit;
        }


        /// <summary>
        /// Number of items detected in this group
        /// </summary>
        public int count { get; set; }


        /// <summary>
        /// TRUE if the group will not accept any new item
        /// </summary>
        [Category("Switches")]
        [DisplayName("isClosed")]
        [Description("TRUE if the group will not accept any new item")]
        public bool isClosed
        {
            get
            {
                if (groupSizeLimit == -1) return false;

                return (count >= groupSizeLimit);
            }
        }
        

        private int _groupSizeLimit = -1; // = new Int32();
        /// <summary>
        /// Limit of group size. If -1 limit is disabled
        /// </summary>
        /// <value>
        /// The group size limit.
        /// </value>
        [Category("sampleGroupItem")]
        [DisplayName("groupSizeLimit")]
        [Description("Limit of group size")]
        public int groupSizeLimit
        {
            get
            {
                return _groupSizeLimit;
            }
            set
            {
                _groupSizeLimit = value;
                OnPropertyChanged("groupSizeLimit");
            }
        }


        private string _groupDescription = default(string); // = new String();
                                                      /// <summary>
                                                      /// Human-readable description of this group
                                                      /// </summary>
        [Category("sampleGroupItem")]
        [DisplayName("groupDescription")]
        [Description("Human-readable description of this group")]
        public string groupDescription
        {
            get
            {
                return _groupDescription;
            }
            set
            {
                _groupDescription = value;
                OnPropertyChanged("groupDescription");
            }
        }


        #region -----------  groupTitle  -------  [Naziv grupe - moze sadrzati i komentar, namenjeno korisniku]
        private string _groupTitle = "Group1"; // = new String();
                                    /// <summary>
                                    /// Naziv grupe - moze sadrzati i komentar, namenjeno korisniku
                                    /// </summary>
        // [XmlIgnore]
        [Category("sampleGroupItem")]
        [DisplayName("Title")]
        [Description("Naziv grupe - moze sadrzati i komentar, namenjeno korisniku")]
        public string groupTitle
        {
            get
            {
                return _groupTitle;
            }
            set
            {
                // Boolean chg = (_groupTitle != value);
                _groupTitle = value;
                OnPropertyChanged("groupTitle");
                // if (chg) {}
            }
        }
        #endregion


        #region -----------  groupTag  -------  [Tag grupe - koristi se za oznacavanje uzorka u bazi podataka]
        private string _groupTag = "g1"; // = new String();
                                    /// <summary>
                                    /// Tag grupe - koristi se za oznacavanje uzorka u bazi podataka
                                    /// </summary>
        // [XmlIgnore]
        [Category("sampleGroupItem")]
        [DisplayName("Tag")]
        [Description("Tag grupe - koristi se za oznacavanje uzorka u bazi podataka")]
        public string groupTag
        {
            get
            {
                return _groupTag;
            }
            set
            {
                // Boolean chg = (_groupTag != value);
                _groupTag = value;
                OnPropertyChanged("groupTag");
                // if (chg) {}
            }
        }
        #endregion


        #region -----------  weight  -------  [Tezina grupe - oznacava relativan odnos velicine ove grupe u odnosu na ostale u setu uzorka ]
        private int _weight = 1; // = new Int32();
                                    /// <summary>
                                    /// Tezina grupe - oznacava relativan odnos velicine ove grupe u odnosu na ostale u setu uzorka 
                                    /// </summary>
        // [XmlIgnore]
        [Category("sampleGroupItem")]
        [DisplayName("Weight")]
        [Description("Tezina grupe - oznacava relativan odnos velicine ove grupe u odnosu na ostale u setu uzorka ")]
        public int weight
        {
            get
            {
                return _weight;
            }
            set
            {
                // Boolean chg = (_weight != value);
                _weight = value;
                OnPropertyChanged("weight");
                // if (chg) {}
            }
        }
        #endregion

        /*
        internal void setGroupBorder(Int32 __groupBorder)
        {
            _groupBorder = __groupBorder;
        }
        */
        internal int _groupBorder;// = "";
        /// <summary>
        /// Privremen broj koji se koristi kod rasporedjivanja itema u grupe
        /// </summary>
        public int groupBorder
        {
            get { return _groupBorder; }
        }



    }
}
