// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelWebPageGeneralRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.pageAnalytics.core;
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
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.project.records;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class modelWebPageGeneralRecord:modelRecordSummaryBase<crawledPage, spiderLink, modelSpiderPageRecord> //modelRecordStandaloneBase<crawledPage>
    {
        
        public modelWebPageGeneralRecord(string __testTimeStamp, crawledPage __instance):base(__testTimeStamp, __instance)
        {
            page = __instance; 
           // tokenizedContent = __instance.tokenizedContent as htmlContentPage;

            

        }



        private spiderLinkByFlagCollection _linksByFlags = new spiderLinkByFlagCollection();
        /// <summary> linkovi sa oznakama koliko su poznati a koliko ne </summary>
        public spiderLinkByFlagCollection linksByFlags
        {
            get
            {
                return _linksByFlags;
            }
            protected set
            {
                _linksByFlags = value;
                OnPropertyChanged("linksByFlags");
            }
        }

        #region METRICS REPORTS 

        //private metricsReport _rpHtml;
        ///// <summary> </summary>
        //public metricsReport rpHtml
        //{
        //    get
        //    {
        //        return _rpHtml;
        //    }
        //    set
        //    {
        //        _rpHtml = value;
        //        OnPropertyChanged("rpHtml");
        //    }
        //}


        //private metricsReport _rpToken;
        ///// <summary> </summary>
        //public metricsReport rpToken
        //{
        //    get
        //    {
        //        return _rpToken;
        //    }
        //    set
        //    {
        //        _rpToken = value;
        //        OnPropertyChanged("rpToken");
        //    }
        //}


        //private metricsReport _rpLang;
        ///// <summary> </summary>
        //public metricsReport rpLang
        //{
        //    get
        //    {
        //        return _rpLang;
        //    }
        //    set
        //    {
        //        _rpLang = value;
        //        OnPropertyChanged("rpLang");
        //    }
        //}

        //private webPageProfile _pProfile;
        ///// <summary> </summary>
        //public webPageProfile pProfile
        //{
        //    get
        //    {
        //        return _pProfile;
        //    }
        //    set
        //    {
        //        _pProfile = value;
        //        OnPropertyChanged("pProfile");
        //    }
        //}


        //private webPageMetrics _pMetrics;
        ///// <summary> </summary>
        //public webPageMetrics pMetrics
        //{
        //    get
        //    {
        //        return _pMetrics;
        //    }
        //    set
        //    {
        //        _pMetrics = value;
        //        OnPropertyChanged("pMetrics");
        //    }
        //}



        private string _sourceContent = "";
        /// <summary> </summary>
        public string sourceContent
        {
            get
            {
                return _sourceContent;
            }
            set
            {
                _sourceContent = value;
                OnPropertyChanged("sourceContent");
            }
        }

        #endregion


        /// <summary>
                                   /// Bindable property
                                   /// </summary>
        public crawledPage page { get; }


        //private aceEnumDictionary<contentTokenCountCategory, tokenStatistics> _tokenFrequencyMatrixByCategory = new aceEnumDictionary<contentTokenCountCategory, tokenStatistics>();
        ///// <summary> </summary>
        //public aceEnumDictionary<contentTokenCountCategory, tokenStatistics> tokenFrequencyMatrixByCategory
        //{
        //    get
        //    {
        //        return _tokenFrequencyMatrixByCategory;
        //    }
        //    set
        //    {
        //        _tokenFrequencyMatrixByCategory = value;
        //        OnPropertyChanged("tokenFrequencyMatrix");
        //    }
        //}


        private object AddSideRecordLock = new object();

        public override void AddSideRecord(modelSpiderPageRecord __sideRecord)
        {
            lock (AddSideRecordLock)
            {
                __sideRecord.pGeneralRecord = this;
                base.AddSideRecord(__sideRecord);
            }
        }
        

        //public Boolean ReceiveTokenizedContent(htmlContentPage __tokenizedContent)
        //{
        //    if (tokenizedContent != null)
        //    {
        //        return false;
        //    }
        //    tokenizedContent = __tokenizedContent;

        //    tokenFrequencyMatrixByCategory[contentTokenCountCategory.all].Add(tokenizedContent?.tokens);
        //    tokenFrequencyMatrixByCategory[contentTokenCountCategory.allKnownWordTokens].Add(tokenizedContent.tokens[contentTokenFlag.languageKnownWord, contentTokenFlag.languageWord]);
        //    tokenFrequencyMatrixByCategory[contentTokenCountCategory.headingContentTokens].Add(tokenizedContent.tokens[contentTokenFlag.title, contentTokenFlag.subsentence_title, contentTokenFlag.titleOneWord]);
        //    tokenFrequencyMatrixByCategory[contentTokenCountCategory.linkTitleTokens].Add(tokenizedContent.tokens[contentTokenFlag.titleNavigation]);
        //    tokenFrequencyMatrixByCategory[contentTokenCountCategory.normalSentences].Add(tokenizedContent.sentences[contentSentenceFlag.regular, contentSentenceFlag.question, contentSentenceFlag.normal]);
            

        //    return true;
        //}

        //private htmlContentPage _tokenizedContent;
        ///// <summary>
        ///// 
        ///// </summary>
        //public htmlContentPage tokenizedContent
        //{
        //    get { return _tokenizedContent; }
        //    protected set { _tokenizedContent = value; }
        //}


        //private diagramModel _pageContentDiagram;
        ///// <summary> </summary>
        //public diagramModel pageContentDiagram
        //{
        //    get
        //    {
        //        return _pageContentDiagram;
        //    }
        //    protected set
        //    {
        //        _pageContentDiagram = value;
        //        OnPropertyChanged("pageContentDiagram");
        //    }
        //}

        public analyticJobRecord aRecord
        {
            get
            {
                return (analyticJobRecord)parent.parent.parent;
            }
        }


        //private instanceCheckList<ISpiderEvaluatorBase> _loadedBy;
        ///// <summary> </summary>
        //public instanceCheckList<ISpiderEvaluatorBase> loadedBy
        //{
        //    get
        //    {
        //        //if (_loadedBy == null)
        //        //{
        //        //    _loadedBy = new instanceCheckList<ISpiderEvaluatorBase>(aRecord.spiderList);
        //        //}
        //        return _loadedBy;
        //    }
        //    protected set
        //    {
        //        _loadedBy = value;
        //        OnPropertyChanged("loadedBy");
        //    }
        //}


        /// <summary>
        /// modelWebPageGeneralRecord access to the <see cref="modelWebSiteGeneralRecord"/>
        /// </summary>
        /// <value>
        /// Reference to execution record: <see cref="modelWebSiteGeneralRecord"/>
        /// </value>
        /// <author>
        /// Goran Grubić
        /// </author>
        /// <remarks>
        /// Intance of  records. <seealso cref="modelWebPageGeneralRecord"/>
        /// </remarks>
        public modelWebSiteGeneralRecord wGeneralRecord
        {
            get
            {
                return (modelWebSiteGeneralRecord)parent;
            }
        }


        public override bool VAR_AllowAutoOutputToConsole
        {
            get
            {
                return false;
            }
        }

        public override string VAR_LogPrefix
        {
            get
            {
                return "pageGen";
            }
        }


        /// <summary>
        /// Boing to be executed before <see cref="M:imbReportingCore.data.modelRecords.modelRecordBase.datasetBuildOnFinish" /> and <see cref="M:imbReportingCore.data.modelRecords.modelRecordBase.datasetBuildOnFinishDefault" />
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void _summaryFinished()
        {
            //if (tokenizedContent == null)
            //{
            //    String msg = "Summary finish called [" + instance.url + "] -- no Page Content set [" + instanceID + "]";
            //    if (instance.clickDepth == 0)
            //    {
            //        msg = msg.addLine("-- the domain is unaccessible");
            //        //parent.log(msg);
                    
            //        return;
            //    }

            //    aceLog.log(msg);
            //    //throw new aceGeneralException(msg, null, instance, "Execution fail");
            //}
            

            //var data = pageContent.AppendDataFields();
            //return data.buildDataTableVerticalSummaryTable(globalMeasureUnitDictionary.globalTableEnum.scoreTable); // as Da

           // dataSet.Tables.Add(dt);

           // dataSet.Tables.Add(tokenCounters.buildDataTable(globalTableEnum.tokensByCategory.GetTableName()));


        }

        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();
            return data;
        }

        public override void datasetBuildOnFinish()
        {
            
        }

        public override void recordFinish(params object[] resources)
        {
            _recordFinish();
        }

        public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
        {
            _recordStart(__testRunStamp, __instanceID);
        }
        
        protected override void _recordStartHandle()
        {
            
        }

       
    }
}
