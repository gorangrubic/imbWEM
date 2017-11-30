// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webTemplateContext.cs" company="imbVeles" >
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

//      using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Low-level context objekat - serijalizuje se u bazu podataka ali ne i u projekat
    /// </summary>
    /// <remarks>
    /// Ovaj low-level Context objekat treba pre svega da sadrži rezultat rada nekog modula. 
    /// Ako sadrži neka privremena podešavanja i vrednosti treba ih označiti sa: [XmlIgnore] [JsonIgnore]
    /// Ukoliko treba da razmenjuje veliki broj podešavanja sa imbResourceContextualModul-om onda treba napraviti specijalizovanu klasu sa podešavanjima: imbWebTemplateSettings
    /// </remarks>
    public class webTemplateContext : imbBindable
    {
        #region -----------  mainTemplateID  -------  [ID koji upućuje na glavni template]

        private int _mainTemplateID = 0; // = new Int32();

        /// <summary>
        /// ID koji upućuje na glavni template
        /// </summary>
        // [XmlIgnore]
        [Category("imbWebTemplateContext")]
        [DisplayName("mainTemplateID")]
        [Description("ID koji upućuje na glavni template")]
        public int mainTemplateID
        {
            get { return _mainTemplateID; }
            set
            {
                _mainTemplateID = value;
                OnPropertyChanged("detectedTemplate");
                OnPropertyChanged("mainTemplateID");
            }
        }

        #endregion

        #region -----------  detectedTemplate  -------  [Templejt koji je detektovan. Ako ih je vise onda prvi u nizu. Ako nisu detektovani onda se vraca null]

        /// <summary>
        /// Templejt koji je detektovan. Ako ih je vise onda prvi u nizu. Ako nisu detektovani onda se vraca null
        /// </summary>
        [XmlIgnore]
  //      [JsonIgnore]
        [Category("imbWebTemplateContext")]
        [DisplayName("detectedTemplate")]
        [Description(
            "Templejt koji je detektovan. Ako ih je vise onda prvi u nizu. Ako nisu detektovani onda se vraca null")]
        public templateDefinition detectedTemplate
        {
            get
            {
                return collectionExtendTools.imbGetFirstValue(detectedTemplates, null, false, mainTemplateID);
                    //detectedTemplates[0];
            }
            set
            {
                detectedTemplates[mainTemplateID] = value;
                OnPropertyChanged("detectedTemplate");
            }
        }

        #endregion

        #region -----------  detectedTemplates  -------  [Lista svih detektovanih templejtova. Ako se pretpostavlja da ce biti samo jedan pronadjen onda bolje koristiti detectedTemplate varijablu koja ce uzeti prvi iz ove liste.]

        private List<templateDefinition> _detectedTemplates = new List<templateDefinition>();

        /// <summary>
        /// Lista svih detektovanih templejtova. Ako se pretpostavlja da ce biti samo jedan pronadjen onda bolje koristiti detectedTemplate varijablu koja ce uzeti prvi iz ove liste.
        /// </summary>
        // [XmlIgnore]
        [Category("imbWebTemplateContext")]
        [DisplayName("detectedTemplates")]
        [Description(
            "Lista svih detektovanih templejtova. Ako se pretpostavlja da ce biti samo jedan pronadjen onda bolje koristiti detectedTemplate varijablu koja ce uzeti prvi iz ove liste."
            )]
        public List<templateDefinition> detectedTemplates
        {
            get { return _detectedTemplates; }
            set
            {
                _detectedTemplates = value;
                OnPropertyChanged("detectedTemplates");
            }
        }

        #endregion

        #region -----------  sourcePages  -------  [Spisak stranica koje treba da se analiziraju - privremeno]

        private SynchronizedCollection<crawledPage> _sourcePages = new SynchronizedCollection<crawledPage>();

        /// <summary>
        /// Spisak stranica koje treba da se analiziraju - privremeno
        /// </summary>
        [XmlIgnore]
  //      [JsonIgnore]
        [Category("imbWebTemplateContext")]
        [DisplayName("sourcePages")]
        [Description("Spisak stranica koje treba da se analiziraju - privremeno")]
        public SynchronizedCollection<crawledPage> sourcePages
        {
            get { return _sourcePages; }
            set
            {
                _sourcePages = value;
                OnPropertyChanged("sourcePages");
            }
        }

        #endregion

        #region KONSTRUKTORI ---

        private imbWebTemplateSettings moduleContextSettings = new imbWebTemplateSettings();

        /// <summary>
        /// Konstruktor koji prihvata deljena podešavanja
        /// </summary>
        /// <param name="_sharedSetting">Objekat sa podešavanjima koja su zajednička sa Resource Modulom</param>
        public webTemplateContext(imbWebTemplateSettings _moduleContextSettings = null)
        {
            moduleContextSettings = _moduleContextSettings;
        }

        /// <summary>
        /// Konstruktor za potrebe serijalizacije
        /// </summary>
        public webTemplateContext()
        {
        }

        #endregion

        public void Add(templateDefinition td)
        {
            detectedTemplates.Add(td);
            td.id = detectedTemplates.Count();
            OnPropertyChanged("detectedTemplate");
        }

        #region STANDARDNE METODE

        /// <summary>
        /// Standardna komanda za pravljenje osnovnog String izveštaja
        /// </summary>
        /// <returns></returns>
        public string report()
        {
            string output = "";
            return output;
        }

        /// <summary>
        /// Prazni sve resurse koji su oznaceni u podesavanjima - priprema za serijalizaciju
        /// </summary>
        public void optimize()
        {
        }

        /// <summary>
        /// Pravi finalnu obradu dobijenih podataka
        /// </summary>
        public void deploy()
        {
        }

        #endregion
    }
}