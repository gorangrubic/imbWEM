// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkResolverConfiguration.cs" company="imbVeles" >
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
namespace imbWEM.Core.settings
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

    public class LinkResolverConfiguration:imbBindable
    {
        public LinkResolverConfiguration()
        {
            
        }

        public void prepare()
        {
            defaultSetup();
        }

        public void defaultSetup()
        {
            if (urlBanNeedles.Count > 0) return;
            urlBanNeedles.Add("mailto:");
            urlBanNeedles.Add("cgi-bin:");
            urlBanNeedles.Add("cdn-bin:");
            urlBanNeedles.Add("cdn-cgi:");
            urlBanNeedles.Add("skype:");
            urlBanNeedles.Add(".ftpquota");

            // special needles
            urlBanNeedles.Add("javascript:void");
            urlBanNeedles.Add("javascript:");
            urlBanNeedles.Add("tel:");


            // documents
            urlBanNeedles.Add(".pdf");
            urlBanNeedles.Add(".xml");
            urlBanNeedles.Add(".odt");
            urlBanNeedles.Add(".doc");
            urlBanNeedles.Add(".docx");
            urlBanNeedles.Add(".ods");
            urlBanNeedles.Add(".xls");
            urlBanNeedles.Add(".xlsx");
            urlBanNeedles.Add(".ppt");
            urlBanNeedles.Add(".pptx");
            urlBanNeedles.Add(".ppsx");
            urlBanNeedles.Add(".pptm");
            urlBanNeedles.Add(".ppsm");
            urlBanNeedles.Add(".odp");
            urlBanNeedles.Add(".rtf");


            urlBanNeedles.Add(".plc");

            // images
            urlBanNeedles.Add(".psd");
            urlBanNeedles.Add(".jpg");
            urlBanNeedles.Add(".jpeg");
            urlBanNeedles.Add(".gif");
            urlBanNeedles.Add(".png");
            urlBanNeedles.Add(".svg");
            urlBanNeedles.Add(".tif");
            urlBanNeedles.Add(".tiff");
            urlBanNeedles.Add(".bmp");

            // archives
            urlBanNeedles.Add(".zip");
            urlBanNeedles.Add(".tar");
            urlBanNeedles.Add(".rar");
            urlBanNeedles.Add(".arj");
            urlBanNeedles.Add(".7z");

            // multi media
            urlBanNeedles.Add(".swf");
            urlBanNeedles.Add(".jar");

            // video
            urlBanNeedles.Add(".avi");
            urlBanNeedles.Add(".mp4");
            urlBanNeedles.Add(".ogv");
            urlBanNeedles.Add(".webm");
            urlBanNeedles.Add(".3gp");
            urlBanNeedles.Add(".flv");

            // audio
            urlBanNeedles.Add(".wav");
            urlBanNeedles.Add(".mp3");
            urlBanNeedles.Add(".ogg");
            urlBanNeedles.Add(".amr");
            urlBanNeedles.Add(".3ga");
            urlBanNeedles.Add(".mov");
            urlBanNeedles.Add(".wmv");






            // ---------- DOMAIN BANs

            urlBanDomains.Add("facebook");
            urlBanDomains.Add("tweeter");
            urlBanDomains.Add("youtube");
            urlBanDomains.Add("google");

        }

        /// <summary>
        /// List of string needles used for approveUrl call
        /// </summary>
        [Category("spiderSettings")]
        [DisplayName("urlBanNeedles")]
        [Description("List of string needles used for approveUrl call")]
        [XmlIgnore]
        public List<string> urlBanNeedles { get; set; } = new List<string>();


        /// <summary>
        /// Domains that are banned from frontier links
        /// </summary>
        [Category("spiderSettings")]
        [DisplayName("urlBanDomains")]
        [Description("Domains that are banned from frontier links")]
        [XmlIgnore]
        public List<string> urlBanDomains { get; set; } = new List<string>();


        public bool LNK_RemoveAnchors { get; set; } = true;
    }
}