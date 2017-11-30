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

    public static class templateExtensions
    {
        /// <summary>
        /// Deo imbBasic metodologije
        /// </summary>
        /// <param name="template">Definicija templejta ciji sadržaj treba podesiti</param>
        /// <param name="settings">Podešavanja</param>
        public static void prepareContent(templateDefinition template, imbWebTemplateSettings settings)
        {
            /*
            StringBuilder textMaker = new StringBuilder();
            template.templateXML = new XmlDocument(nsSetup.namespaceManager.NameTable);
            
            String tmp = template.templateXML.OuterXml;

            template.templateXML.Prefix = nsSetup.nsPrefix;



            String basicXML = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" + Environment.NewLine;
            basicXML += "<span>" + Environment.NewLine;
            basicXML += "<html xmlns=\"" + nsSetup.nsSourceUrl + "\" >" + Environment.NewLine;
            basicXML += "</html>" + Environment.NewLine;
            basicXML += "</span>" + Environment.NewLine;
            
            template.templateXML.LoadXml(basicXML);
            



            foreach (KeyValuePair<String, templateElement> el in template.xPathStruktura)
            {
                if (settings.doMakeTEXT)
                {

                    textMaker.AppendLine(el.Value.content);
                }
                if (settings.doMakeXML)
                {
                   // imbXmlExtendedTools.makeNodeByxPath(template.templateXML, template.templateXML.DocumentElement, el.Key, el.Value.source, nsSetup);
                }
            }

            if (settings.doMakeHTML)
            {
                template.templateHTML = template.templateXML.OuterXml;
            }
             * */
        }
    }
}