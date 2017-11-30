using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using BrightstarDB.EntityFramework;
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
using imbSCI.DataComplex.extensions.data.formats;
using imbSCI.DataComplex.extensions.text;
using imbSCI.DataComplex.special;

namespace imbWEM.Index.index.core.pageTemplate
{
    #region imbVELES USING

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
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