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

//    using imbAPI.imbComplexDataTypes;

    #endregion

    /// <summary>
    /// 2013a: Alatke za primenu templatea (izdvajanje sadržaja itd)
    /// </summary>
    public static class templateOperations
    {
        /// <summary>
        /// COMMON TREE DETECTION: End Node xPath Frequency
        /// Koristi imbKeywordScoreList da bi utvrdio koje se putanje pojavljuju najviše
        /// one koje imaju poena koliko i kolekcija stranica se smatraju zajedničkom strukturom.
        /// Koristi FindXPath extenziju za uzimanje xPath putanje
        /// </summary>
        /// <param name="source">Stranice koje se testiraju - u XML strukturi</param>
        /// <param name="tolerance">Omogucava da se template-om smatraju i node xPaths koji imaju za N manje pojavljivanja</param>
        /// <returns>Listu xPath putanja za strukturu koja je zajednicka</returns>
        public static Dictionary<string, templateElement> commonTree_imbENPF(crawledPage[] source,
                                                                             imbWebTemplateSettings settings)
        {
            Dictionary<string, templateElement> output = new Dictionary<string, templateElement>();

            if (source.Length == 0)
            {
                logSystem.log("No pages supplied for common tree detection!", logType.Warning);
                return output;
            }

          //  imbKeywordScoreList allPaths = new imbKeywordScoreList();
            Dictionary<string, XmlNode> allXmlNodes = new Dictionary<string, XmlNode>();

            int limit = source.Count() - settings.treeTolerance;

            imbNamespaceSetup nsSetup;
            foreach (crawledPage cpage in source)
            {
                /*
                nsSetup = new imbNamespaceSetup(cpage.xmlDocument);
                List<XmlNode> _endNodes = cpage.xmlDocument.FirstChild.collectChildren(collectRelatives.endNodes, 0);
                foreach (XmlNode node in _endNodes)
                {
                    String xp = node.FindXPath(nsSetup);

                    allPaths.addKeyword(xp);
                    if (!allXmlNodes.ContainsKey(xp))
                    {
                        allXmlNodes.Add(xp, node);
                    }
                }
                */
            }

            //  imbKeywordScore[] limited = .Where<imbKeywordScore>(x => x.score >= limit) as imbKeywordScore[];

            //foreach (imbKeywordScore ki in allPaths)
            //{
            //    if (ki.score >= limit)
            //    {
            //        output.Add(ki.keyword, templateElement.makeElement(ki.keyword, allXmlNodes[ki.keyword]));
            //    }
            //}

            logSystem.log("Pronađeno ukupno [" + output.Count() + "] elemenata", logType.Execution);

            return output;
        }


        /// <summary>
        /// Description of $property$
        /// </summary>
        public static string makeUniKey(crawledPage[] input, imbWebTemplateSettings settings,
                                        templateDefinition template)
        {
            string output = ""; // = new String();
            //imbKeywordScoreList nameList = new imbKeywordScoreList();

            //switch (settings.uniKeyMode)
            //{
            //    case uniKeyCreation.idToString:
            //        output = template.id.ToString("D3");
            //        break;
            //    case uniKeyCreation.tokenizePageTitles:
            //        foreach (crawledPage p in input)
            //        {
            //            nameList.addText(p.pageCaption, false, imbNLPengine.imbBasic);
            //        }
            //        break;
            //    case uniKeyCreation.tokenizeTemplateContent:
            //        foreach (crawledPage p in input)
            //        {
            //            nameList.addText(template.templateHTML, false, imbNLPengine.imbBasic);
            //        }
            //        break;
            //}
            //if (output == "")
            //{
            //   nameList.sort();
            //   output = imbCollectionHelpers.imbGetFirstValue<String>(nameList.getStringList(), "", false, 0);
            //   output = output.TrimToMaxLength(10, "");
            //}

            //logSystem.log("UniKey created (" + settings.uniKeyMode + ") = " + output, logType.Execution);

            return output;
        }


        /// <summary>
        /// imbBasic metodologija :: 
        /// Primenjuje pravila vezana za sadržaj i ako treba formira commonContent za svaki od templateElement-a
        /// </summary>
        /// <param name="source">Lista sa stranicama koje treba da se analiziraju</param>
        /// <param name="xPathList">Postojeca xPathLista</param>
        /// <param name="settings">Podesavanja</param>
        /// <returns>Umanjenu listu (ako je tako podeseno) sa definisanim zajedničkim sadržajem</returns>
        public static Dictionary<string, templateElement> commonContentCheck(crawledPage[] source,
                                                                             Dictionary<string, templateElement>
                                                                                 xPathList,
                                                                             imbWebTemplateSettings settings)
        {
            Dictionary<string, templateElement> output = new Dictionary<string, templateElement>();

            //if (source.Length == 0)
            //{
            //    logSystem.log("No pages supplied for content check!", logType.Warning);
            //    return output;
            //}

            //switch (settings.contentPolicy)
            //{
            //    case commonContentPolicy.ignoreContent:

            //        return xPathList;
            //        break;
            //}


            //foreach (KeyValuePair<String, templateElement> el in xPathList)
            //{
            //    String tmpContent = null;

            //    String[] cmnContent = null; //= el.Value.content;
            //    List<String[]> contents = new List<string[]>();

            //    Boolean add = true;

            //   foreach (crawledPage p in source)
            //    {
            //        String textCon = "";
            //        XmlNode nd = imbAdvancedXPath.xPathExecution(el.Key, p.xmlDocument.DocumentElement, null, imbCore.xml.queryEngine.imbXPathQuery, true, 0);
            //        textCon = textRetriveEngine.retriveText(nd, settings.textRetriveSetup);


            //        switch (settings.contentPolicy)
            //        {
            //            default:
            //            case commonContentPolicy.onlyExactContent:
            //                if (tmpContent == null)
            //                {
            //                    tmpContent = textCon;
            //                }
            //                if (tmpContent != textCon)
            //                {
            //                    add = false;
            //                    break;
            //                }
            //                break;
            //            case commonContentPolicy.extractCommonContent:

            //                //String[] lines = imbNLPTools.defaultSplit(textCon, imbNLPengine.imbBasic, settings.contentTokenizationSettings);
            //                contents.Add(lines);
            //                break;
            //        }
            //    }

            //    if (settings.contentPolicy == commonContentPolicy.extractCommonContent)
            //    //{
            //    //    List<String> commonStrings = imbNLPTools.getCommonMembers(contents, settings.contentExtractionTolerance);
            //    //    String commonContent = imbNLPTools.defaultJoin(commonStrings.ToArray(), imbNLPengine.imbBasic, settings.contentTokenizationSettings);

            //        el.Value.content = commonContent;

            //        if (String.IsNullOrEmpty(commonContent))
            //        {
            //            add = false;
            //        }
            //    }

            //    if (add)
            //    {
            //        output.Add(el.Key, el.Value);
            //    }
            //}

            //logSystem.log(settings.contentPolicy + " :: Ulaz [" + xPathList.Count() + "] - na izlazu> [" + output.Count() + "]", logType.Execution);

            return output;
        }
    }
}