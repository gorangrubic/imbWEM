// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentHashAndAddressEntryList.cs" company="imbVeles" >
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
    using imbWEM.Core.stage;
    using imbSCI.Core.extensions.table;
    using imbACE.Network.extensions;

    public class contentHashAndAddressEntryList:List<contentHashAndAddressEntry>
    {
        /// <summary> </summary>
        public string listName { get; set; }


        /// <summary> </summary>
        public string listComment { get; set; }

        /// <summary>
        /// Description of $property$
        /// </summary>
        [Category("contentHashAndAddressEntry")]
        [DisplayName("contentType")]
        [Description("Type of hashed content")]
        public contentHashTypeEnum contentType { get; set; }


        public contentHashAndAddressEntryList(string __listName, string __comment, contentHashTypeEnum __type)
        {
            listName = __listName;
            listComment = __comment;
            contentType = __type;
        }

        /// <summary>
        /// Builds the data table with columns: address, hash and frequency
        /// </summary>
        /// <returns></returns>
        public DataTable BuildDataTable()
        {
            DataTable output = new DataTable((listName + contentType).getCleanFileName());
            output.SetTitle(listName);
            output.SetDescription(listComment.addLine("Content type: " + contentType.toString()));

            var col_address = output.Columns.Add("Address").SetDesc("Address of hashed content").SetHasLinks(true);
            var col_hash = output.Columns.Add("Hash").SetDesc("MD5 hash of the content");
            var col_freq = output.Columns.Add("Frequency").SetDesc("Number of occurrances");

            foreach (contentHashAndAddressEntry entry in this)
            {
                var nr = output.NewRow();
                nr[col_address] = entry.contentAddress;
                nr[col_hash] = entry.contentHash;
                nr[col_freq] = entry.frequency;
                output.Rows.Add(nr);
            }

            return output;
        }

    }
}