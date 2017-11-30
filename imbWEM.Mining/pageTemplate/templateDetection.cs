// --------------------------------------------------------------------------------------------------------------------
// <copyright file="templateDetection.cs" company="imbVeles" >
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