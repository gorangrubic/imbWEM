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
    using System.Xml;
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