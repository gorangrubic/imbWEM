// --------------------------------------------------------------------------------------------------------------------
// <copyright file="imbWebTemplateSettings.cs" company="imbVeles" >
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
using imbWEM.Core.crawler.evaluators;
using imbWEM.Core.crawler.model;
using imbWEM.Core.crawler.modules.performance;
using imbWEM.Core.crawler.rules.active;
using imbWEM.Core.crawler.targets;
using imbWEM.Core.directReport;
using imbWEM.Core.stage;

namespace imbWEM.Mining.pageTemplate
{
    #region imbVELES USING

    using System.Collections.Generic;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq;
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

//    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    #endregion

    /// <summary>
    /// Klasa za zajednička podešavanja koja razmenjuju imbModul i low-level Context
    /// </summary>
    public class imbWebTemplateSettings : aceCommonTypes.primitives.imbBindable
    {
        #region -----------  uniKeyMode  -------  [način na koji se formira uniKey oznaka]

        private uniKeyCreation _uniKeyMode = uniKeyCreation.idToString; // = new uniKeyCreation();

        /// <summary>
        /// način na koji se formira uniKey oznaka
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("uniKeyMode")]
        [Description("način na koji se formira uniKey oznaka")]
        public uniKeyCreation uniKeyMode
        {
            get { return _uniKeyMode; }
            set
            {
                _uniKeyMode = value;
                OnPropertyChanged("uniKeyMode");
            }
        }

        #endregion

        #region -----------  macroAlgorithm  -------  [Koji algoritam koristi za detekciju]

        private templateDetectionAlgorithms _macroAlgorithm = templateDetectionAlgorithms.imbBasic;
                                            // = new templateDetectionAlgorithms();

        /// <summary>
        /// Koji algoritam koristi za detekciju
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("Macro Algorithm")]
        [Description("Koji algoritam koristi za detekciju")]
        public templateDetectionAlgorithms macroAlgorithm
        {
            get { return _macroAlgorithm; }
            set
            {
                _macroAlgorithm = value;
                OnPropertyChanged("macroAlgorithm");
            }
        }

        #endregion

        #region -----------  doMakeHTML  -------  [Formirace HTML (string) sadržaj templejta ]

        private bool _doMakeHTML; // = new Boolean();

        /// <summary>
        /// Formirace HTML (string) sadržaj templejta 
        /// </summary>
        // [XmlIgnore]
        [Category("Output")]
        [DisplayName("doMakeHTML")]
        [Description("Formirace HTML (string) sadržaj templejta ")]
        public bool doMakeHTML
        {
            get { return _doMakeHTML; }
            set
            {
                _doMakeHTML = value;
                OnPropertyChanged("doMakeHTML");
            }
        }

        #endregion

        #region -----------  doMakeTEXT  -------  [Formirace Text sadržaj templejta]

        private bool _doMakeTEXT; // = new Boolean();

        /// <summary>
        /// Formirace Text sadržaj templejta
        /// </summary>
        // [XmlIgnore]
        [Category("Output")]
        [DisplayName("doMakeTEXT")]
        [Description("Formirace Text sadržaj templejta")]
        public bool doMakeTEXT
        {
            get { return _doMakeTEXT; }
            set
            {
                _doMakeTEXT = value;
                OnPropertyChanged("doMakeTEXT");
            }
        }

        #endregion

        #region -----------  doMakeXML  -------  [Rekonstruisaće XML (HTML) strukturu templejta]

        private bool _doMakeXML; // = new Boolean();

        /// <summary>
        /// Rekonstruisaće XML (HTML) strukturu templejta
        /// </summary>
        // [XmlIgnore]
        [Category("Output")]
        [DisplayName("doMakeXML")]
        [Description("Rekonstruisaće XML (HTML) strukturu templejta")]
        public bool doMakeXML
        {
            get { return _doMakeXML; }
            set
            {
                _doMakeXML = value;
                OnPropertyChanged("doMakeXML");
            }
        }

        #endregion

        #region -----------  commonTreeDetection  -------  [Način na koji se detektuje zajednička struktura stranica]

        private commonTreeMethod _commonTreeDetection = commonTreeMethod.imbEndNodePathFrequency;
                                 // = new commonTreeMethod();

        /// <summary>
        /// Način na koji se detektuje zajednička struktura stranica
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("Structure tree Mode")]
        [Description("Način na koji se detektuje zajednička struktura stranica")]
        public commonTreeMethod commonTreeDetection
        {
            get { return _commonTreeDetection; }
            set
            {
                _commonTreeDetection = value;
                OnPropertyChanged("commonTreeDetection");
            }
        }

        #endregion

        #region -----------  treeTolerance  -------  [Tolerancija prilikom detektovanja zajedničke strukture stranica - različito se primenjuje u zavisnosti od metode]

        private int _treeTolerance; // = new Int32();

        /// <summary>
        /// Tolerancija prilikom detektovanja zajedničke strukture stranica - različito se primenjuje u zavisnosti od metode
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("treeTolerance")]
        [Description(
            "Tolerancija prilikom detektovanja zajedničke strukture stranica - različito se primenjuje u zavisnosti od metode"
            )]
        public int treeTolerance
        {
            get { return _treeTolerance; }
            set
            {
                _treeTolerance = value;
                OnPropertyChanged("treeTolerance");
            }
        }

        #endregion

        #region -----------  contentPolicy  -------  [Na koji način tretira sadržaj unutar strukture koja se smatra zajedničkom]

        private commonContentPolicy _contentPolicy = commonContentPolicy.extractCommonContent;
                                    // = new commonContentPolicy();

        /// <summary>
        /// Na koji način tretira sadržaj unutar strukture koja se smatra zajedničkom
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("Node content Policy")]
        [Description("Na koji način tretira sadržaj unutar strukture koja se smatra zajedničkom")]
        public commonContentPolicy contentPolicy
        {
            get { return _contentPolicy; }
            set
            {
                _contentPolicy = value;
                OnPropertyChanged("contentPolicy");
            }
        }

        #endregion

        #region -----------  contentExtractionTolerance  -------  [Stepen tolerancije prilikom izvlačenja zajedničkog sadržaja. Ako je više od 0 biće i sadržaja koji nisu zajednički baš svim node-ima]

        private int _contentExtractionTolerance = 0; // = new Int32();

        /// <summary>
        /// Stepen tolerancije prilikom izvlačenja zajedničkog sadržaja. Ako je više od 0 biće i sadržaja koji nisu zajednički baš svim node-ima
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("Content Extraction Tolerance")]
        [Description(
            "Stepen tolerancije prilikom izvlačenja zajedničkog sadržaja. Ako je više od 0 biće i sadržaja koji nisu zajednički baš svim node-ima"
            )]
        public int contentExtractionTolerance
        {
            get { return _contentExtractionTolerance; }
            set
            {
                _contentExtractionTolerance = value;
                OnPropertyChanged("contentExtractionTolerance");
            }
        }

        #endregion

        #region -----------  contentTokenizationSettings  -------  [Podešavanja tokenizacije sadržaja - za commonContentExtraction]

        private imbNLPsettings _contentTokenizationSettings = new imbNLPsettings();

        /// <summary>
        /// Podešavanja tokenizacije sadržaja - za commonContentExtraction
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("contentTokenizationSettings")]
        [Description("Podešavanja tokenizacije sadržaja - za commonContentExtraction")]
        [ExpandableObject]
        public imbNLPsettings contentTokenizationSettings
        {
            get { return _contentTokenizationSettings; }
            set
            {
                _contentTokenizationSettings = value;
                OnPropertyChanged("contentTokenizationSettings");
            }
        }

        #endregion

        #region -----------  textRetriveSetup  -------  [Podešavanja vezana za prevođenje node-a u tekst]

        private textRetriveSetup _textRetriveSetup = new textRetriveSetup();

        /// <summary>
        /// Podešavanja vezana za prevođenje node-a u tekst
        /// </summary>
        // [XmlIgnore]
        [Category("Output")]
        [DisplayName("textRetriveSetup")]
        [Description("Podešavanja vezana za prevođenje node-a u tekst")]
        [ExpandableObject]
        public textRetriveSetup textRetriveSetup
        {
            get { return _textRetriveSetup; }
            set
            {
                _textRetriveSetup = value;
                OnPropertyChanged("textRetriveSetup");
            }
        }

        #endregion

        #region -----------  doSavePageUrls  -------  [Da li da pamti od kojih je stranica nastao template]

        private bool _doSavePageUrls = true; // = new Boolean();

        /// <summary>
        /// Da li da pamti od kojih je stranica nastao template
        /// </summary>
        // [XmlIgnore]
        [Category("Detection")]
        [DisplayName("Save Page Urls")]
        [Description("Da li da pamti od kojih je stranica nastao template")]
        public bool doSavePageUrls
        {
            get { return _doSavePageUrls; }
            set
            {
                _doSavePageUrls = value;
                OnPropertyChanged("doSavePageUrls");
            }
        }

        #endregion

        #region -----------  sampleSetup  -------  [Podešavanja za odabir stranica za uzorkovanje pri template detekciji]

        private sampleSettings _sampleSetup = new sampleSettings();

        /// <summary>
        /// Podešavanja za odabir stranica za uzorkovanje pri template detekciji
        /// </summary>
        // [XmlIgnore]
        [Category("Page Sample")]
        [DisplayName("sampleSetup")]
        [Description("Podešavanja za odabir stranica za uzorkovanje pri template detekciji")]
        [ExpandableObject]
        public sampleSettings sampleSetup
        {
            get { return _sampleSetup; }
            set
            {
                _sampleSetup = value;
                OnPropertyChanged("sampleSetup");
            }
        }

        #endregion
    }
}