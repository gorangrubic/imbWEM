// --------------------------------------------------------------------------------------------------------------------
// <copyright file="crawlerDomainTaskCollection.cs" company="imbVeles" >
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

// using imbAnalyticsEngine.webSiteComplexCrawler;

namespace imbWEM.Core.crawler.engine
{
    using System.Collections.Concurrent;
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
    using imbWEM.Core.project;
    using imbWEM.Core.stage;

    /// <summary>
    /// Crawler-vs-domain task collection
    /// </summary>
    public class crawlerDomainTaskCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public crawlerDomainTaskMachine parent { get; set; }


        /// <summary>
        /// Here the webSiteProfile is used to create crawlDomainTask and wRecords
        /// </summary>
        /// <param name="__tRecord">The t record.</param>
        /// <param name="__sample">The sample.</param>
        /// <param name="__parent">The parent.</param>
        public crawlerDomainTaskCollection(modelSpiderTestRecord __tRecord, List<webSiteProfile> __sample, crawlerDomainTaskMachine __parent)
        {
            sampleSize = __sample.Count();
            tRecord = __tRecord;
            parent = __parent;

            foreach (webSiteProfile profile in __sample)
            {
                //var crawlerContext = tRecord.aRecord.crawledContextGlobalRegister.GetContext(profile.domain, tRecord.aRecord.sciProject.mainWebCrawler.mainSettings, profile, tRecord.aRecord.testRunStamp);
                var task = new crawlerDomainTask(profile, this);
                items.Enqueue(task);
            }
        }


        public crawlerDomainTaskCollection(modelSpiderTestRecord __tRecord, List<webSiteProfile> __sample, analyticMacroBase __aMacro)
        {
            sampleSize = __sample.Count();
            tRecord = __tRecord;
            aMacro = __aMacro;

            foreach (webSiteProfile profile in __sample)
            {
                //var crawlerContext = tRecord.aRecord.crawledContextGlobalRegister.GetContext(profile.domain, tRecord.aRecord.sciProject.mainWebCrawler.mainSettings, profile, tRecord.aRecord.testRunStamp);
                var task = new crawlerDomainTask(profile, this);
                items.Enqueue(task);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TimeLimitForOneItemLoad { get; set; }


        /// <summary> </summary>
        internal analyticMacroBase aMacro { get; set; }


        /// <summary> </summary>
        public int sampleSize { get; protected set; }


        /// <summary>
        /// Ratio of domains waiting
        /// </summary>
        public double waitingRatio
        {
            get {
                if (items.Count() == 0) return 0;
                if (sampleSize == 0) return 0;
                return ((double)(items.Count() - (done.Count() + running.Count())) / ((double)sampleSize));
            }
        }

        /// <summary>
        /// Ratio of domains waiting
        /// </summary>
        public double doneRatio
        {
            get
            {
                if (done.Count() == 0) return 0;
                if (sampleSize == 0) return 0;
                return (((double)done.Count()) / ((double)sampleSize));
            }
        }


        /// <summary> </summary>
        public modelSpiderTestRecord tRecord { get; protected set; }

        public int waitingAndRunningCount
        {
            get
            {
                return (items.Count() - done.Count()) + running.Count();
            }
        }


        /// <summary>
        /// Scheduled items
        /// </summary>
        public ConcurrentQueue<crawlerDomainTask> items { get; protected set; } = new ConcurrentQueue<crawlerDomainTask>();


        /// <summary> </summary>
        public aceConcurrentBag<crawlerDomainTask> running { get; protected set; } = new aceConcurrentBag<crawlerDomainTask>();


        /// <summary> </summary>
        public aceConcurrentBag<crawlerDomainTask> done { get; protected set; } = new aceConcurrentBag<crawlerDomainTask>();


        /*
        private ConcurrentQueue<crawlerDomainTask> _running = new ConcurrentQueue<crawlerDomainTask>();
        /// <summary> </summary>
        public ConcurrentQueue<crawlerDomainTask> running
        {
            get
            {
                return _running;
            }
            protected set
            {
                _running = value;
                //OnPropertyChanged("running");
            }
        }


        private ConcurrentQueue<crawlerDomainTask> _done = new ConcurrentQueue<crawlerDomainTask>();
        /// <summary> </summary>
        public ConcurrentQueue<crawlerDomainTask> done
        {
            get
            {
                return _done;
            }
            protected set
            {
                _done = value;
                //OnPropertyChanged("done");
            }
        }
        */


    }

}