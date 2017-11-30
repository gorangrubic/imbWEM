// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTarget.cs" company="imbVeles" >
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
// Project: imbWEM.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------

namespace imbWEM.Core.crawler.targets
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using HtmlAgilityPack;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.contentBlock;
    using imbCommonModels.webPage;
    using imbCommonModels.webStructure;
    using imbNLP.Core.contentExtensions;
    using imbNLP.Core.textRetrive;
    using imbNLP.Data;
    using imbNLP.Data.evaluate;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.morphology;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.Data.semanticLexicon.term;
    using imbSCI.Core.attributes;
    using imbSCI.Core.collection;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    /// <summary>
    /// Target is designated by the pointed absolute location
    /// </summary>
    public class spiderTarget : ISpiderTarget
    {
        /// <summary>
        /// 
        /// </summary>
        public string targetHash { get; set; }


        /// <summary> </summary>
        public int iterationLoaded { get; protected set; } = 0;


        /// <summary>
        /// Discovery iteration
        /// </summary>
        public int iterationDiscovery { get; protected set; } = 0;


        public spiderTarget(string __url, spiderTargetCollection __parent)
        {
            url = __url;
            parent = __parent;
            deploy();

        }

        public spiderTarget(spiderLink __vector, spiderTargetCollection __parent)
        {   
            parent = __parent;
            deploy(__vector);
        }


        /// <summary>
        /// Adds the new vector to the target. <see cref="spiderLink.originPage"/> has to be specified otherwise exception will be thrown. Returns <c>true</c> if it is new vector for this target
        /// </summary>
        /// <param name="__vector">The vector.</param>
        /// <returns></returns>
        /// <exception cref="aceGeneralException">Supplied spiderLink vector has no origin specified - null - No origin page for link: " + __vector.url</exception>
        public bool AddVector(spiderLink __vector)
        {
            if (__vector.originPage != null)
            {
                string hk = getKey(__vector); 
                
                if (linkVectors.ContainsKey(hk))
                {
                    return false;
                }
                else
                {
                    deploy(__vector, hk);
                    return true;
                }
            } else
            {
                throw new aceGeneralException("Supplied spiderLink vector has no origin specified", null, __vector, "No origin page for link: " + __vector.url);

            }

            return false;
        }

        protected string getKey(spiderLink __vector)
        {
            return parent.GetKeyForVector(__vector);
        }


        protected void deploy(spiderLink __vector=null, string __vkey=null)
        {
            List<string> tkns = new List<string>();
            if (url.isNullOrEmpty())
            {
                url = __vector.url;
                iterationDiscovery = __vector.iterationDiscovery;
                targetHash = __vector.targetHash;
            }
            

            if (__vector != null)
            {
                if (__vkey == null) __vkey = getKey(__vector);

                linkVectors.Add(__vkey, __vector);

                

                string rUrl = parent.wRecord.domainInfo.GetURLWithoutDomainName(__vector.url);

                if (rUrl.Length > 0)
                {
                    if (!__vector.domain.isNullOrEmpty()) rUrl = rUrl.Replace(__vector.domain, "");


                    var r = rUrl.getTokens(true, false, true, true,1);

                    r = semanticLexiconManager.lexiconCache.decodeTwins(r);

                    var c = new List<string>();

                    foreach (string caption in __vector.captions)
                    {
                        if (!caption.isNullOrEmpty())
                        {
                            c.AddRange(caption.getTokens(true, false, false, true));
                        }
                    }

                    //__vector.captions.getTokens(true, false, true,true,2);



                    tkns.AddRange(r);

                    tkns.AddRange(c);

                }
            } else
            {
                tkns.AddRange(url.getTokens(true, true, true));
            }
            
            if (tokens == null)
            {
                tokens = parent.dlTargetLinkTokens.AddTable("tkns_" + key) as termDocument;
                tokens.expansion = 0;
                tokens.AddTokens(tkns);
                
            } else
            {
                tokens.AddTokens(tkns);
            }
        }


        /// <summary> </summary>
        public spiderTargetCollection parent { get; protected set; }

        public termQueryDocument getQuery(int expansionSteps, ILogBuilder loger)
        {
            if (query == null)
            {
                query = new termQueryDocument();
                List<string> tkns = new List<string>();
                foreach (IWeightTableTerm term in tokens)
                {
                    tkns.Add(term.nominalForm);

                }
                query.AddQueryTerms(tkns, expansionSteps, loger);
            }
            return query;
        }


        /// <summary>
        /// Token table describing this target - tokens that are extracted from URL
        /// </summary>
        public termDocument tokens { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        protected termQueryDocument query { get; set; }


        private string _key = "";
        /// <summary> </summary>
        public string key
        {
            get
            {
                if (_key.isNullOrEmpty())
                {
                    key = parent.GetHash(url);
                }
                return _key;
            }
            protected set
            {
                _key = value;
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string url { get; protected set; } = "";


        /// <summary>
        /// Gets the all vectors, including those that are coming from the same page
        /// </summary>
        /// <returns></returns>
        public List<spiderLink> GetVectors()
        {
            return linkVectors.Values.ToList();
        }


        /// <summary>
                                                      /// Language that was found during evaluation
                                                      /// </summary>
        [Category("spiderTarget")]
        [DisplayName("evaluatedLanguage")]
        [Description("Language that was found during evaluation")]
        public basicLanguageEnum evaluatedLanguage { get; set; } = basicLanguageEnum.unknown;

        /// <summary>
        /// Gets a value indicating whether this target is relevant. (shortcut for testing evaluation result language)
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is relevant; otherwise, <c>false</c>.
        /// </value>
        public bool IsRelevant
        {
            get
            {
                return (evaluatedLanguage == basicLanguageEnum.serbian);
            }
        }

      

        private object RelevantPageLock = new object();


        protected indexPage indexPageInfo { get; set; }

        /// <summary>
        /// Gets <see cref="indexPage"/> entry from current Index database instance
        /// </summary>
        /// <returns></returns>
        public indexPage GetIndexPage()
        {
            if (indexPageInfo == null)
            {
                indexPageInfo = imbWEMManager.index.pageIndexTable.GetPageForUrl(url);
            }
            return indexPageInfo;
        }



        /// <summary>
        /// Gets the HTML document from loaded page (<see cref="HtmlDocument"/>)
        /// </summary>
        /// <returns></returns>
        public HtmlDocument GetHtmlDocument()
        {
            HtmlDocument htmlDoc = (HtmlDocument)page.webpage.result.HtmlDocument;
            
            return htmlDoc;
        }


        /// <summary>
        /// Attaches the page - if the page was already attached returns <c>false</c>
        /// </summary>
        /// <param name="__page">The page.</param>
        /// <returns></returns>
        public bool AttachPage(spiderPage __page, ILogBuilder response, int targetBlockCount=3)
        {
            if (page != __page)
            {
                page = __page;

                HtmlDocument htmlDoc = GetHtmlDocument();
                

                iterationLoaded = parent.wRecord.iteration;
                
                if (htmlDoc != null)
                {
                    XPathNavigator xnav = htmlDoc.DocumentNode.CreateNavigator();
                    
                    pageText = xnav.retriveText(imbWEMManager.settings.contentProcessor.textRetrieve);

                    pageText = WebUtility.HtmlDecode(pageText);
                    pageHash = md5.GetMd5Hash(pageText);

                    if (parent.wRecord.tRecord.instance.settings.doEnableDLC_BlockTree)
                    {
                        contentTree = htmlDoc.buildTree(page.webpage.domain);  // contentTree = new nodeTree(page.webpage.domain, htmlDoc);
                        contentBlocks = contentTree.getBlocks(targetBlockCount);
                        contentBlocks.CalculateScores();
                    }


                    var ignoreTokens = parent.wRecord.domainInfo.domainWords;

                    var preprocessedTokens = parent.wRecord.tRecord.evaluator.GetAllProperTokensSortedByFrequency(pageText);


                    if (parent.wRecord.tRecord.instance.settings.doEnableDLC_TFIDF)
                    {
                        content = parent.dlTargetPageTokens.AddTable(key) as termDocument;
                        content.expansion = parent.wRecord.tRecord.instance.settings.TermExpansionForContent;
                        content.AddTokens(preprocessedTokens.ToList(), response);
                    }

                    
                    

                    bool evaluationOk = false;

                    indexPageEvaluationEntryState pageState = indexPageEvaluationEntryState.haveNoEvaluationEntry;


                    if (imbWEMManager.settings.indexEngine.doIndexFullTrustMode)
                    {
                        pageState = imbWEMManager.index.pageIndexTable.GetPageAssertion(url);
                    } else
                    {
                        pageState = indexPageEvaluationEntryState.notInTheIndex;
                    }

                    if (pageState.HasFlag(indexPageEvaluationEntryState.haveEvaluationEntry))
                    {
                        evaluation = new multiLanguageEvaluation();
                        evaluation.result_language = evaluatedLanguage;
                        evaluationOk = pageState.HasFlag(indexPageEvaluationEntryState.isRelevant);
                        evaluatedLanguage = basicLanguageEnum.serbian;
                    }
                    else
                    {
                        evaluation = parent.wRecord.tRecord.evaluator.evaluate(pageText, ignoreTokens, preprocessedTokens.ToList());
                        evaluatedLanguage = evaluation.result_language;
                    }



                   



                    lock (RelevantPageLock)
                    {
                        if (IsRelevant)
                        {

                            parent.wRecord.context.targets.termSerbian.AddRange(preprocessedTokens);

                            parent.wRecord.relevantPages.AddUnique(__page.url);

                            parent.wRecord.tRecord.relevantPages.AddUnique(__page.url);
                        }

                        else
                        {
                            parent.wRecord.context.targets.termOther.AddRange(preprocessedTokens);
                        }

                        parent.wRecord.context.targets.termsAll.AddRange(preprocessedTokens);
                    }



                    // <----- calling event
                    var targs = new modelSpiderSiteRecordEventArgs(this);
                    //targs.htmlDoc = htmlDoc;
                    if (parent.wRecord.context.OnTargetPageAttached != null) parent.wRecord.context.OnTargetPageAttached(parent.wRecord, targs);

                    

                }

                return true;
            }
            return false;
        }

        //public textEvaluation evaluation { get; protected set; }

        public multiLanguageEvaluation evaluation { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string pageText { get; set; }


        public string pageHash { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public nodeTree contentTree { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public nodeBlockCollection contentBlocks { get; set; } = new nodeBlockCollection();


        /// <summary>
        /// 
        /// </summary>
        public termDocument content { get; set; }

        #region ----------- Boolean [ isLoaded ] -------  [Da li je target ucitan?]
        
        /// <summary>
        /// Da li je target ucitan?
        /// </summary>
        
        public bool isLoaded
        {
            get {
                if (page == null) return false;
                if (pageText.isNullOrEmpty()) return false;
                return true;
            }
            
        }
        #endregion


        /// <summary>
        /// Attached page
        /// </summary>
        public spiderPage page { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, spiderLink> linkVectors { get; protected set; } = new Dictionary<string, spiderLink>();


        /// <summary> </summary>
        public spiderEvalRuleResultSet marks { get; protected set; } = new spiderEvalRuleResultSet();

        /// <summary>
        /// True if this target is content duplicate (confirmed by HTML source code hash) of another, already crawled target. Target that was loaded first has False, any other duplicate has True.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is duplicate; otherwise, <c>false</c>.
        /// </value>
        public bool isDuplicate { get; set; }
        /// <summary>
        /// Reference to the first crawled target, having the same HTML source code hash fingerprint
        /// </summary>
        /// <value>
        /// The duplicate of.
        /// </value>
        public spiderTarget duplicateOf { get; set; }

        ISpiderPage ISpiderTarget.page
        {
            get
            {
                return page;
            }

            set
            {
                page = (spiderPage) value;
            }
        }

        ISpiderTarget ISpiderTarget.duplicateOf
        {
            get
            {
                return ((ISpiderTarget)duplicateOf);
            }

           
        }

        IWeightTable ISpiderTarget.tokens
        {
            get
            {
                return tokens;
            }
        }

        IWeightTable ISpiderTarget.content
        {
            get
            {
                return content;
            }
            
        }

        public void Dispose()
        {
            evaluation = null;
            page = null;
            marks = null;
        }
    }

}