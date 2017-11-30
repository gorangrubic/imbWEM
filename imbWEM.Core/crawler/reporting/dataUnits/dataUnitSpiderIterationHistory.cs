// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dataUnitSpiderIterationHistory.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.reporting.dataUnits
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
    using imbSCI.DataComplex.data.dataUnits;
    using imbSCI.DataComplex.data.dataUnits.core;
    using imbSCI.DataComplex.data.dataUnits.enums;
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

    public class dataUnitSpiderIterationHistory:dataUnitSeries<dataUnitSpiderIteration, modelSpiderSiteRecord>
    {
        public override dataUnitPresenter complete_Table
        {
            get
            {
                if (_complete_Table == null)
                {
                    _complete_Table = new dataUnitPresenter(dataUnitMap.DEFGROUP, "Iterations {{{spider_name}}} for {{{site_name}}}", "Complete iteration timeline table of {{{spider_name}}} for {{{site_name}}}");
                    _complete_Table.setFlags(
                        dataDeliveryPresenterTypeEnum.tableHorizontal,
                        dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.sourceForGlobalAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(complete_Table)] = _complete_Table;
                }
                return _complete_Table;
            }
            
        }

        private dataUnitPresenter _action_Chart;
        /// <action>Defines Chart that is showint all properties having "action" in Category description</action>
        public dataUnitPresenter action_Chart
        {
            get
            {
                if (_action_Chart == null)
                {
                    _action_Chart = new dataUnitPresenter("action", "{{{spider_name}}} actions", "Actions that the spider made and effects on the discovery results for {{{site_domain}}}");
                    _action_Chart.setFlags(
                        dataDeliveryPresenterTypeEnum.spLineGraph,
                        dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.globalAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(action_Chart)] = _action_Chart;
                }
                return _action_Chart;
            }

            protected set
            {
                _action_Chart = value;
                OnPropertyChanged("action_Chart");
            }
        }


        private dataUnitPresenter _dynamics_Chart;
        /// <dynamics>Defines Chart that is showint all properties having "dynamics" in Category description</dynamics>
        public dataUnitPresenter dynamics_Chart
        {
            get
            {
                if (_dynamics_Chart == null)
                {
                    _dynamics_Chart = new dataUnitPresenter("dynamics", "{{{spider_name}}} dynamics", "Metrics of change between each iteration for {{{site_domain}}} ");
                    _dynamics_Chart.setFlags(
                        dataDeliveryPresenterTypeEnum.spLineGraph,
                        dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.globalAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(dynamics_Chart)] = _dynamics_Chart;
                }
                return _dynamics_Chart;
            }

            protected set
            {
                _dynamics_Chart = value;
                OnPropertyChanged("dynamics_Chart");
            }
        }


        private dataUnitPresenter _timeline_Linechart;
        /// <timeline>Defines Linechart that is showint all properties having "timeline" in Category description</timeline>
        public dataUnitPresenter timeline_Linechart
        {
            get
            {
                if (_timeline_Linechart == null)
                {
                    _timeline_Linechart = new dataUnitPresenter("timeline", "{{{spider_name}}} discovery", "Spider algorithm results as iteration ({{{it_count}}}) timeline chart");
                    _timeline_Linechart.setFlags(
                        dataDeliveryPresenterTypeEnum.spLineGraph,
                        dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.globalAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(timeline_Linechart)] = _timeline_Linechart;
                }
                return _timeline_Linechart;
            }

            protected set
            {
                _timeline_Linechart = value;
                OnPropertyChanged("timeline_Linechart");
            }
        }



        private dataUnitPresenter _stability_Table;
        /// <stability>Defines Table that is showint all properties having "stability" in Category description</stability>
        public dataUnitPresenter stability_Table
        {
            get
            {
                if (_stability_Table == null)
                {
                    _stability_Table = new dataUnitPresenter("stability", "Stability score", "Stability score per selected measures");
                    _stability_Table.setFlags(
                        dataDeliveryPresenterTypeEnum.tableHorizontal,
                        dataDeliverFormatEnum.includeAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(stability_Table)] = _stability_Table;
                }
                return _stability_Table;
            }

            protected set
            {
                _stability_Table = value;
                OnPropertyChanged("stability_Table");
            }
        }



        private dataUnitPresenter _summaryTable ;
        /// <summary>Defines table that is showint all properties having "summary" in Category description</summary>
        public dataUnitPresenter summaryTable
        {
            get
            {
                if (_summaryTable == null)
                {
                    _summaryTable = new dataUnitPresenter("summary", "{{{spider_name}}} timeline report", "Iteration metrics for {{{site_domain}}} in {{{it_count}}} iterations");
                    _summaryTable.setFlags(
                        dataDeliveryPresenterTypeEnum.tableHorizontal,
                        dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.globalAttachment | dataDeliverFormatEnum.sourceForGlobalAttachment,
                        dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
                    presenters[nameof(summaryTable)] = _summaryTable;
                }
                return _summaryTable;
            }
            protected set
            {
                _summaryTable = value;
                OnPropertyChanged("summaryTable");
            }
        }


        public dataUnitSpiderIterationHistory(int length = -1):base(dataDeliveryAcquireEnum.collectionLimitShowCase10, length)
        {
            buildMap();
            
        }
    }

}