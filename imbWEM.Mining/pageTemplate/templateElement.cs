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

//      using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Jedan element templejta - sadrži putanju, text sadržaj i XmlNode (koji se ne serijalizuje)
    /// </summary>
    public class templateElement : imbBindable
    {
        /// <summary>
        /// Pravi element od prosledjenog node-a i podesavanja
        /// </summary>
        public static templateElement makeElement(string __xpath, XmlNode __source)
        {
            templateElement output = new templateElement();

            output.xPath = __xpath;
            output.source = __source;
            output.content = __source.InnerText;

            return output;
        }

        #region -----------  xPath  -------  [Putanja ka XmlNode-u koji je deo strukture]

        private string _xPath; // = new String();

        /// <summary>
        /// Putanja ka XmlNode-u koji je deo strukture
        /// </summary>
        // [XmlIgnore]
        [Category("templateElement")]
        [DisplayName("xPath")]
        [Description("Putanja ka XmlNode-u koji je deo strukture")]
        public string xPath
        {
            get { return _xPath; }
            set
            {
                _xPath = value;
                OnPropertyChanged("xPath");
            }
        }

        #endregion

        #region -----------  content  -------  [Sadržaj koji se nalazi unutar XmlNode-a]

        private string _content; // = new String();

        /// <summary>
        /// Sadržaj koji se nalazi unutar XmlNode-a
        /// </summary>
        // [XmlIgnore]
        [Category("templateElement")]
        [DisplayName("content")]
        [Description("Sadržaj koji se nalazi unutar XmlNode-a")]
        public string content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("content");
            }
        }

        #endregion

        #region -----------  source  -------  [Izvorni XmlNode]

        private XmlNode _source; // = new XmlNode();

        /// <summary>
        /// Izvorni XmlNode
        /// </summary>
        [XmlIgnore]
  //      [JsonIgnore]
        [Category("templateElement")]
        [DisplayName("source")]
        [Description("Izvorni XmlNode")]
        public XmlNode source
        {
            get { return _source; }
            set
            {
                _source = value;
                OnPropertyChanged("source");
            }
        }

        #endregion
    }
}