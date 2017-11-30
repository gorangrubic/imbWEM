// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentHashAndAddressEntry.cs" company="imbVeles" >
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

    /// <summary>
    /// Single entry of hashed content
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class contentHashAndAddressEntry:imbBindable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="contentHashAndAddressEntry"/> class.
        /// </summary>
        /// <param name="__contentAddress">The content address.</param>
        /// <param name="__contentHash">The content hash.</param>
        /// <param name="__freq">The freq.</param>
        public contentHashAndAddressEntry(string __contentAddress, string __contentHash, int __freq)
        {
            contentAddress = __contentAddress;
            contentHash = __contentHash;
            
            frequency = __freq;
        }

        private string _contentAddress = ""; // = new String();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        [Category("contentHashAndAddressEntry")]
        [DisplayName("contentAddress")]
        [Description("Address of content")]
        public string contentAddress
        {
            get
            {
                return _contentAddress;
            }
            set
            {
                _contentAddress = value;
                OnPropertyChanged("contentAddress");
            }
        }


        private string _contentHash = ""; // = new String();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        [Category("contentHashAndAddressEntry")]
        [DisplayName("contentHash")]
        [Description("Hashed value")]
        public string contentHash
        {
            get
            {
                return _contentHash;
            }
            set
            {
                _contentHash = value;
                OnPropertyChanged("contentHash");
            }
        }


       


        private int _frequency = 0; // = new Int32();
                                                      /// <summary>
                                                      /// Number of occourances
                                                      /// </summary>
        [Category("contentHashAndAddressEntry")]
        [DisplayName("frequency")]
        [Description("Number of occurrances")]
        public int frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
                OnPropertyChanged("frequency");
            }
        }



    }
}
