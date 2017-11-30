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

    #endregion

    /// <summary>
    /// Glavna klasa za izvršavanje logike detektovanja templejta
    /// </summary>
    public static class templateDetection
    {
        /// <summary>
        /// Izvršava imbBasic metod detekcije templejta
        /// </summary>
        /// <param name="source">Lista učitanih stranica</param>
        /// <param name="settings">Podešavanja</param>
        /// <returns>Sređena definicija templejta</returns>
        public static templateDefinition detectTemplate_imbBasic(crawledPage[] source, imbWebTemplateSettings settings)
        {
            templateDefinition output = new templateDefinition();

            List<string> xPathList;

            // COMMON TREE DETECTION
            switch (settings.commonTreeDetection)
            {
                default:
                case commonTreeMethod.imbEndNodePathFrequency:
                    output.xPathStruktura = templateOperations.commonTree_imbENPF(source, settings);
                    break;
            }

            crawledPage c = source.First();
            //imbNamespaceSetup nsSetup = new imbNamespaceSetup(c.xmlDocument);


            // COMMON CONTENT CHECK
            output.xPathStruktura = templateOperations.commonContentCheck(source, output.xPathStruktura, settings);
            templateExtensions.prepareContent(output, settings);

            // page track
            if (settings.doSavePageUrls)
            {
                foreach (crawledPage p in source)
                {
                    output.relatedPages.Add(p.url);
                }
            }


            output.score = source.Length;

            if (output.xPathStruktura.Count == 0)
            {
                logSystem.log("Template detection failed!", logType.Warning);
            }

            return output;
        }
    }
}