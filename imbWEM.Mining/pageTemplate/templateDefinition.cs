// --------------------------------------------------------------------------------------------------------------------
// <copyright file="templateDefinition.cs" company="imbVeles" >
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
// Project: imbWEM.Mining
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
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
using imbSCI.Data.collection.nested;
using imbSCI.Data.data;
using imbSCI.Data.enums.reporting;
using imbSCI.DataComplex.data.modelRecords;
using imbSCI.DataComplex.extensions.data.formats;
using imbSCI.DataComplex.extensions.text;
using imbSCI.DataComplex.special;
// using imbWEM.Core.crawler.evaluators;
// using imbWEM.Core.crawler.model;
// using imbWEM.Core.crawler.modules.performance;
// using imbWEM.Core.crawler.rules.active;
// using imbWEM.Core.crawler.targets;
// using imbWEM.Core.directReport;
// using imbWEM.Core.stage;

namespace imbWEM.Mining.pageTemplate
{
    #region imbVELES USING

    using System.Collections.Generic;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbACE.Services.terminal;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.morphology;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.Data.semanticLexicon.term;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.special;

    #endregion

    /// <summary>
    /// 2013a: Klasa koja u sebi sadrži definiciju detektovanog web template-a
    /// </summary>
    public class templateDefinition : imbBindable
    {
        #region -----------  xmlNamespaceUrl  -------  [Namespace URL koji je pronadjen na stranici]

        private string _xmlNamespaceUrl = "http://www.w3.org/1999/xhtml"; // = new String();

        /// <summary>
        /// Namespace URL koji je pronadjen na stranici
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("xmlNamespaceUrl")]
        [Description("Namespace URL koji je pronadjen na stranici")]
        public string xmlNamespaceUrl
        {
            get { return _xmlNamespaceUrl; }
            set
            {
                _xmlNamespaceUrl = value;
                OnPropertyChanged("xmlNamespaceUrl");
            }
        }

        #endregion

        #region -----------  id  -------  [pozicija u trenutnoj listi templejtova]

        private int _id; // = new Int32();

        /// <summary>
        /// pozicija u trenutnoj listi templejtova
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("id")]
        [Description("pozicija u trenutnoj listi templejtova")]
        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }

        #endregion

        #region -----------  score  -------  [Broj stranica koje imaju ovaj template]

        private int _score; // = new Int32();

        /// <summary>
        /// Broj stranica koje imaju ovaj template
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("score")]
        [Description("Broj stranica koje imaju ovaj template")]
        public int score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPropertyChanged("score");
            }
        }

        #endregion

        #region -----------  structCount  -------  [Broj elemenata u strukturi]

        /// <summary>
        /// Broj elemenata u strukturi
        /// </summary>
        [XmlIgnore]
  //      [JsonIgnore]
        [Category("templateDefinition")]
        [DisplayName("structCount")]
        [Description("Broj elemenata u strukturi")]
        public int structCount
        {
            get { return xPathStruktura.Count(); }
        }

        #endregion

        public templateDefinition(string __uniKey = "")
        {
            uniKey = __uniKey;
        }

        #region -----------  uniKey  -------  [Univerzalni string kljuc kojim treba template da se prepoznaje / razlikuje u odnosu na druge]

        private string _uniKey = "FAIL"; // = new String();

        /// <summary>
        /// Univerzalni string kljuc kojim treba template da se prepoznaje / razlikuje u odnosu na druge
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("uniKey")]
        [Description("Univerzalni string kljuc kojim treba template da se prepoznaje / razlikuje u odnosu na druge")]
        public string uniKey
        {
            get { return _uniKey; }
            set
            {
                _uniKey = value;
                OnPropertyChanged("uniKey");
            }
        }

        #endregion

        #region -----------  relatedPages  -------  [spisak stranica od kojih je napravljen template - url]

        private List<string> _relatedPages = new List<string>();

        /// <summary>
        /// spisak stranica od kojih je napravljen template - url
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("relatedPages")]
        [Description("spisak stranica od kojih je napravljen template - url")]
        public List<string> relatedPages
        {
            get { return _relatedPages; }
            set
            {
                _relatedPages = value;
                OnPropertyChanged("relatedPages");
            }
        }

        #endregion

        #region -----------  xPathStruktura  -------  [Lista putanja prema zajednickoj strukturi koja cini template]

        private Dictionary<string, templateElement> _xPathStruktura = new Dictionary<string, templateElement>();

        /// <summary>
        /// Lista putanja prema zajednickoj strukturi koja cini template
        /// </summary>
        [XmlIgnore]
  //      [JsonIgnore]
        [Category("templateDefinition")]
        [DisplayName("xPathStruktura")]
        [Description("Lista putanja prema zajednickoj strukturi koja cini template")]
        public Dictionary<string, templateElement> xPathStruktura
        {
            get { return _xPathStruktura; }
            set
            {
                _xPathStruktura = value;
                OnPropertyChanged("structCount");
                OnPropertyChanged("xPathStruktura");
            }
        }

        #endregion

        #region -----------  templateHTML  -------  [HTML templejta koji je detektovan. U zavisnosti od algoritma to je> garantovan ili ne zajednicki sadrzaj analiziranih stranica]

        private string _templateHTML = ""; // = new String();

        /// <summary>
        /// HTML templejta koji je detektovan. U zavisnosti od algoritma to je> garantovan ili ne zajednicki sadrzaj analiziranih stranica
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("templateHTML")]
        [Description(
            "HTML templejta koji je detektovan. U zavisnosti od algoritma to je> garantovan ili ne zajednicki sadrzaj analiziranih stranica"
            )]
        public string templateHTML
        {
            get { return _templateHTML; }
            set
            {
                _templateHTML = value;
                OnPropertyChanged("templateHTML");
            }
        }

        #endregion

        #region -----------  templateTEXT  -------  [TEXT izlaz templejta koji je detektovan.]

        private string _templateTEXT; // = new String();

        /// <summary>
        /// TEXT izlaz templejta koji je detektovan.
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("templateTEXT")]
        [Description("TEXT izlaz templejta koji je detektovan.")]
        public string templateTEXT
        {
            get { return _templateTEXT; }
            set
            {
                _templateTEXT = value;
                OnPropertyChanged("templateTEXT");
            }
        }

        #endregion

        #region -----------  templateXML  -------  [XML dokument - pseudo HTML sa sadržajem koji je deo template-a]

        private XmlDocument _templateXML; // = new String();

        /// <summary>
        /// XML dokument - pseudo HTML sa sadržajem koji je deo template-a
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("templateXML")]
        [Description("XML dokument - pseudo HTML sa sadržajem koji je deo template-a")]
        public XmlDocument templateXML
        {
            get { return _templateXML; }
            set
            {
                _templateXML = value;
                OnPropertyChanged("templateXML");
            }
        }

        #endregion

        /*
        #region -----------  elements  -------  [Lista elemenata koja se snima prilikom serijalizacije]
        private List<templateElement> _elements; // = new List<templateElement>();
        /// <summary>
        /// Lista elemenata koja se snima prilikom serijalizacije
        /// </summary>
        // [XmlIgnore]
        [Category("templateDefinition")]
        [DisplayName("elements")]
        [Description("Lista elemenata koja se snima prilikom serijalizacije")]
        public List<templateElement> elements
        {
            get
            {
                return _elements;
            }
            set
            {
                _elements = value;
                OnPropertyChanged("elements");
            }
        }
        #endregion
        */
    }
}