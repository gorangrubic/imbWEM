// --------------------------------------------------------------------------------------------------------------------
// <copyright file="performanceDataLoad.cs" company="imbVeles" >
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.data.measurement;
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

    public class performanceDataLoad : performanceBase<performanceDataLoadTake>
    {
        public performanceDataLoad():base()
        {

        }

        public performanceDataLoad(string __name) : base(__name)
        {

        }

        private object AddBytesLock = new object();


        private object addIterationLock = new object();


        public void AddIteration(int iteration=1)
        {
            lock (addIterationLock)
            {
                iterationCount += iteration;
            }
        }



        private object addContentPageLock = new object();


        public void AddContentPage(int terms, int pages=1)
        {
            lock (addContentPageLock)
            {
                termCount = termCount + terms; 
                pageCount = pageCount + pages;
            }
        }


        /// <summary>
        /// Add loaded bytes
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public void AddBytes(int bytes)
        {
            lock (AddBytesLock)
            {
                totalBytes = totalBytes + Convert.ToUInt64(bytes);
                pageLoads++;
            }
        }

        public int termCount { get; set; } = 0;

        public int pageCount { get; set; } = 0;

        public int iterationCount { get; set; } = 0;

        /// <summary> </summary>
        public ulong totalBytes { get; protected set; } = 0;


        /// <summary>
        /// Number of AddBytes call
        /// </summary>
        public int pageLoads { get; private set; } = 0;

        public override int secondsBetweenTakesDefault
        {
            get
            {
                return imbWEMManager.settings.supportEngine.monitoringSampling; 
            }
        }



        /*
        private ulong _lastRead = 0;
        /// <summary>
        /// 
        /// </summary>
        protected ulong lastRead
        {
            get { return _lastRead; }
            set { _lastRead = value; }
        }
        */


        public override void measure(performanceDataLoadTake t)
        {
            ulong output = totalBytes;

            t.reading = Convert.ToDouble(output);
            lock (addContentPageLock)
            {
                t.ContentPages = pageCount;
                pageCount = 0;

                t.ContentTerms = termCount;
                termCount = 0;
            }

            lock (addIterationLock)
            {
                t.CrawlerIterations = iterationCount;
                iterationCount = 0;
            }

                /*
            lock (addContentPageLock)
            {

                if (lastTake != null)
                {
                    t.ContentPages = pageCount - lastTake.ContentPages;
                } else
                {
                    t.ContentPages = pageCount;
                }



                if (lastTake != null)
                {
                    t.ContentTerms = termCount - lastTake.ContentTerms;
                }
                else
                {
                    t.ContentTerms = termCount;
                }

            }*/
                // if (lastTake != null) t.ContentTerms -= lastTake.ContentTerms;
                /*
                3232 (addIterationLock)
                {

                    if (lastTake != null)
                    {
                        t.CrawlerIterations = iterationCount - lastTake.CrawlerIterations;
                    }
                    else
                    {
                        t.CrawlerIterations = iterationCount;
                    }

                } */


                // lastRead = totalBytes;


            }


        //public override void take()
        //{
        //    DateTime lastCall = DateTime.Now;
        //    Double lastMeasure = 0;
        //    if (lastTake != null)
        //    {
        //        lastCall = lastTake.samplingTime;
        //        lastMeasure = lastTake.reading;
        //    }

        //    var t = new performanceTake();


        //    t.reading = measure();

        //    t.secondsSinceLastTake = DateTime.Now.Subtract(lastCall).TotalSeconds;

        //    lastTake = t;
        //    takes.Add(t);
        //}

        public override void prepare()
        {
            
        }
    }

}