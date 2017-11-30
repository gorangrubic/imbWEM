// --------------------------------------------------------------------------------------------------------------------
// <copyright file="experimentSessionRegistry.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.experimentSession
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
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.console;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    public class experimentSessionRegistry:objectTable<experimentSessionEntry>
    {



        public experimentSessionRegistry(string newSessionId) : base("TestID", newSessionId)
        {
            SessionID = newSessionId;
        }


        public experimentSessionRegistry(string newSessionId, string __filePath, bool autoLoad) : base(__filePath, autoLoad, "TestID", newSessionId)
        {
            SessionID = newSessionId;

 
           
        }

        [Category("Session")]
        [DisplayName("SessionID")]
        [imb(imbAttributeName.measure_letter, "SID")]
        [imb(imbAttributeName.measure_setUnit, "-")]
        [Description("ID of the session")] // [imb(imbAttributeName.reporting_escapeoff)]
        public string SessionID { get; set; } = default(string);

        public string GetRecordID(string crawlId)
        {
            return SessionID + "-" + crawlId;
        }

        //  public webPageTF AddDomain()


        public const string PATH_CompiledFTIDF = "tf_idf.xml";
        public const string PATH_AggregateFTIDF = "tf_idf.xml";

        [XmlIgnore]
        public experimentSessionEntry CurrentSession { get; set; }
        


        public experimentSessionEntry StartSession(string CrawlID, indexPerformanceEntry indexID, analyticConsoleState state)
        {
            var experiment = GetOrCreate(GetRecordID(CrawlID));
            experiment.StartSession(CrawlID, indexID, SessionID,state);
            
            CurrentSession = experiment;
            UpdateRecord(experiment);
            return experiment;
        }

        public void UpdateRecord(experimentSessionEntry performance=null)
        {
            if (performance == null) performance = CurrentSession;
            AddOrUpdate(performance);
        }

        public void CloseSession(IEnumerable<modelSpiderTestRecord> tRecords)
        {
            if (CurrentSession != null)
            {
                CurrentSession.CloseSession(tRecords);

                AddOrUpdate(CurrentSession);
            }
            // <---- should be called when session is getting closed
        }
        
       



        public const string GENERAL_SESSIONID = "General";

    }

}