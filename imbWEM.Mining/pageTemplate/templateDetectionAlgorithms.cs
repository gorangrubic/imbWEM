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

    public enum templateDetectionAlgorithms
    {
        /// <summary>
        /// Za zadate stranice ide redom i pronalazi zajedničku strukturu 
        /// na kraju uzme sadržaj iz prve stranice u nizu -
        /// </summary>
        imbBasic,

        /// <summary>
        /// Restricted top-down mapping for Template Detection ("A fast and robust method for web page template detection and removal", ACM conference 2006)
        /// Prvo se slučajnim uzorkom izaberu dve stranice, onda krene detekcija
        /// </summary>
        RTDM_TD,

        /// <summary>
        /// RBM_TD: Restricted Bottom-Up Mapping for Template Detection ("On Finding Templates on Web Collections", WWW 2009)
        /// Oslanja se na "xPath tree" mapiranje
        /// </summary>
        RBM_TD,

        /// <summary>
        /// Multiple Template Detection   ("On Finding Templates on Web Collections", WWW 2009)
        /// Vrši klasterizaciju stranica prema njihovom template-u i simultano izdvaja templates
        /// </summary>
        MTD,

        /// <summary>
        /// Koristi .NET alatke za prepoznavanje razlike u XML fajlu
        /// </summary>
        imbGeneric,
    }
}