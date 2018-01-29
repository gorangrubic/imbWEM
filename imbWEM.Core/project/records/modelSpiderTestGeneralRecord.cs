// --------------------------------------------------------------------------------------------------------------------
// <copyright file="modelSpiderTestGeneralRecord.cs" company="imbVeles" >
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
namespace imbWEM.Core.project.records
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
    using imbCommonModels.webStructure;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class modelSpiderTestGeneralRecord : modelRecordSummaryParentBase<analyticJob, webSiteProfile, modelWebSiteGeneralRecord, ISpiderEvaluatorBase, modelSpiderTestRecord>
    {
        public modelSpiderTestGeneralRecord(string __testRunStamp, analyticJob __instance) : base(__testRunStamp, __instance)
        {
        }

        public override void AddSideRecord(modelSpiderTestRecord __sideRecord)
        {
            __sideRecord.tGeneralRecord = this;
            base.AddSideRecord(__sideRecord);
        }

        public override bool VAR_AllowAutoOutputToConsole
        {
            get
            {
                return true;
            }
        }

        public override string VAR_LogPrefix
        {
            get
            {
                return "siteGeneral";
            }
        }





        private int _pagesLoaded = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Total count of unique pages loaded by spiders
                                                      /// </summary>
        [Category("modelSpiderTestGeneralRecord")]
        [DisplayName("pagesLoaded")]
        [Description("Total count of unique pages loaded by spiders")]
        public int pagesLoaded
        {
            get
            {
                return _pagesLoaded;
            }
            set
            {
                _pagesLoaded = value;
                OnPropertyChanged("pagesLoaded");
            }
        }


        private int _spidersTested = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Count of spider algorithms tested
                                                      /// </summary>
        [Category("modelSpiderTestGeneralRecord")]
        [DisplayName("spidersTested")]
        [Description("Count of spider algorithms tested")]
        public int spidersTested
        {
            get
            {
                return _spidersTested;
            }
            set
            {
                _spidersTested = value;
                OnPropertyChanged("spidersTested");
            }
        }



        private instanceCountCollection<string> _pageUrlListGeneral = default(instanceCountCollection<string>); // = new instanceCountCollection<String>();
        /// <summary>
        /// The complete list of loaded urls
        /// </summary>
        [Category("modelSpiderTestGeneralRecord")]
        [DisplayName("pageUrlListGeneral")]
        [Description("The complete list of loaded urls")]
        public instanceCountCollection<string> pageUrlListGeneral
        {
            get
            {
                return _pageUrlListGeneral;
            }
            set
            {
                _pageUrlListGeneral = value;
                OnPropertyChanged("pageUrlListGeneral");
            }
        }






        public override PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null)
        {
            if (data == null) data = new PropertyCollectionExtended();
            // ubaciti svoje podatke
            return data;
        }

        protected override void _summaryFinished()
        {
            // prikupiti summary child recorda
        }

        public override void datasetBuildOnFinish()
        {
            // izgraditi data setove
        }

        public override void recordFinish(params object[] resources)
        {
            _recordFinish();
        }

        public override void recordStart(string __testRunStamp, string __instanceID, params object[] resources)
        {
            _recordStart(__testRunStamp, __instanceID);
        }


    }
}
