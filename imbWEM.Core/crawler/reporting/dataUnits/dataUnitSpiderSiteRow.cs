// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dataUnitSpiderSiteRow.cs" company="imbVeles" >
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
    using imbSCI.DataComplex.data.dataUnits.core;
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

    public class dataUnitSpiderSiteRow : dataUnitRow<modelSpiderSiteRecord>
    {


        //private dataUnitPresenter _stability_Table;
        ///// <stability>Defines Table that is showint all properties having "stability" in Category description</stability>
        //public dataUnitPresenter stability_Table
        //{
        //    get
        //    {
        //        if (_stability_Table == null)
        //        {
        //            _stability_Table = new dataUnitPresenter("stability", "Iteration stability report", "-- stability Table --");
        //            _stability_Table.setFlags(
        //                dataDeliveryPresenterTypeEnum.tableVertical,
        //                dataDeliverFormatEnum.includeAttachment | dataDeliverFormatEnum.includeLegend,
        //                dataDeliverAttachmentEnum.attachCSV | dataDeliverAttachmentEnum.attachExcel | dataDeliverAttachmentEnum.attachJSON);
        //            presenters[nameof(stability_Table)] = _stability_Table;
        //        }
        //        return _stability_Table;
        //    }

        //    protected set
        //    {
        //        _stability_Table = value;
        //        OnPropertyChanged("stability_Table");
        //    }
        //}


        private float _avg_score_l = default(int); // = new Int32();
        /// <summary>
        /// Average link score over the web site - before drop out
        /// </summary>
        [Category("summary")]
        [DisplayName("Avg link score")]
        [Description("Average link score over the web site")]
        [imb(imbAttributeName.reporting_valueformat, "#0.00")]
        public float avg_score_l
        {
            get
            {
                return _avg_score_l;
            }
            set
            {
                _avg_score_l = value;
                OnPropertyChanged("avg_score_l");
            }
        }


        private int _tc_score_l = default(int); // = new Int32();
        /// <summary>
        /// Total active link score - before drop out
        /// </summary>
        [Category("summary")]
        [DisplayName("Sum link score")]
        [Description("Total active link score")]
        public int tc_score_l
        {
            get
            {
                return _tc_score_l;
            }
            set
            {
                _tc_score_l = value;
                OnPropertyChanged("tc_score_l");
            }
        }



        private int _st_ingame_l;
        /// <summary>
        /// Stability of ingame links count
        /// </summary>
        /// <value>
        /// The st ingame l.
        /// </value>
        [Category("stability")]
        [DisplayName("Active links stability")]
        [Description("Total count of links still in consideration to visit")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        public int st_ingame_l
        {
            get
            {
                return _st_ingame_l;
            }
            set
            {
                _st_ingame_l = value;
                OnPropertyChanged("st_ingame_l");
            }
        }


        private int _st_ruledout_l = default(int); // = new Int32();
        /// <summary>
        /// Stability of ruled out links in last iteration
        /// </summary>
        [Category("stability")]
        [DisplayName("Stab. r-out links")]
        [Description("Stability of ruled out links in last iteration")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int st_ruledout_l
        {
            get
            {
                return _st_ruledout_l;
            }
            set
            {
                _st_ruledout_l = value;
                OnPropertyChanged("st_ruledout_l");
            }
        }



        private int _st_loaded_p = default(int); // = new Int32();
        /// <summary>
        /// Stability of loaded pages count
        /// </summary>
        [Category("stability,comparative,")]
        [DisplayName("Stab. loaded")]
        [Description("Stability of loaded pages count")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        [imb(imbAttributeName.measure_operand, nameof(tc_loaded_p))]
        public int st_loaded_p
        {
            get
            {
                return _st_loaded_p;
            }
            set
            {
                _st_loaded_p = value;
                OnPropertyChanged("st_loaded_p");
            }
        }


        private int _st_detected_l = default(int); // = new Int32();
        /// <summary>
        /// Stability of links detected count 
        /// </summary>
        [Category("stability,comparative")]
        [DisplayName("Stab. links")]
        [Description("Stability of links detected count ")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        [imb(imbAttributeName.measure_operand, nameof(tc_detected_l))]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        public int st_detected_l
        {
            get
            {
                return _st_detected_l;
            }
            set
            {
                _st_detected_l = value;
                OnPropertyChanged("st_detected_l");
            }
        }



        private int _tc_detected_l = default(int);
        [Category("linkspie")]
        [DisplayName("Links detected")]
        [Description("Total number of links detected")]
        public int tc_detected_l
        {
            get
            {
                return _tc_detected_l;
            }
            set
            {
                _tc_detected_l = value;
                OnPropertyChanged("tc_detected_p");
            }
        }


        private int _tc_loaded_p = default(int); // = new Int32();
        /// <summary>
        /// Total count of loaded pages
        /// </summary>
        [Category("summary,linkspie")]
        [DisplayName("Pages loaded")]
        [Description("Total count of loaded pages")]
        public int tc_loaded_p
        {
            get
            {
                return _tc_loaded_p;
            }
            set
            {
                _tc_loaded_p = value;
                OnPropertyChanged("tc_loaded_p");
            }
        }

        private int _tc_leftActive_l = default(int); // = new Int32();
        /// <summary>
        /// Number of links left active when spider finished the stage
        /// </summary>
        [Category("linkspie,comparative")]
        [DisplayName("tc_leftActive_l")]
        [Description("Number of links left active when spider finished the stage")]
        public int tc_leftActive_l
        {
            get
            {
                return _tc_leftActive_l;
            }
            set
            {
                _tc_leftActive_l = value;
                OnPropertyChanged("tc_leftActive_l");
            }
        }


        private int _tc_selected_p = 0;
        [Category("summary,comparactive")]
        [DisplayName("tc_selected_p")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_selected_p
        {
            get
            {
                return _tc_selected_p;
            }
            set
            {
                _tc_selected_p = value;
                OnPropertyChanged("tc_selected_p");
            }
        }


        private int _tc_ruledout_p = 0;
        [Category("crosspie")]
        [DisplayName("tc_ruledout_p")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_ruledout_p
        {
            get
            {
                return _tc_ruledout_p;
            }
            set
            {
                _tc_ruledout_p = value;
                OnPropertyChanged("tc_ruledout_p");
            }
        }


        private int _tc_score_p = 0;
        [Category("summary")]
        [DisplayName("tc_score_p")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_score_p
        {
            get
            {
                return _tc_score_p;
            }
            set
            {
                _tc_score_p = value;
                OnPropertyChanged("tc_score_p");
            }
        }



        private int _ob_achieved = 0;
        [Category("summary,comparative")]
        [DisplayName("ob_achieved")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int ob_achieved
        {
            get
            {
                return _ob_achieved;
            }
            set
            {
                _ob_achieved = value;
                OnPropertyChanged("ob_achieved");
            }
        }


        private int _ob_aborted = 0;
        [Category("summary,comparative")]
        [DisplayName("ob_aborted")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int ob_aborted
        {
            get
            {
                return _ob_aborted;
            }
            set
            {
                _ob_aborted = value;
                OnPropertyChanged("ob_aborted");
            }
        }



        private int _tc_crosslinks_p = 0;
        [Category("core")]
        [DisplayName("Crosslinks")]
        [Description("Total count of crosslinks detected")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_crosslinks_p
        {
            get
            {
                return _tc_crosslinks_p;
            }
            set
            {
                _tc_crosslinks_p = value;
                OnPropertyChanged("tc_crosslinks_p");
            }
        }


        private int _tc_crosslink_pageset_p = 0;
        [Category("crosspie")]
        [DisplayName("tc_crosslink_pageset_p")]
        [Description("Number of pages in the selected set that have crosslinks with all other pages in the selected set")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_crosslink_pageset_p
        {
            get
            {
                return _tc_crosslink_pageset_p;
            }
            set
            {
                _tc_crosslink_pageset_p = value;
                OnPropertyChanged("tc_crosslink_pageset_p");
            }
        }


        private int _tc_noCrossLink_each_p = 0;
        [Category("crosspie")]
        [DisplayName("tc_noCrossLink_each_p")]
        [Description("Number of pages in the selected set that have crosslinks with all other pages in the selected set")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_noCrossLink_each_p
        {
            get
            {
                return _tc_noCrossLink_each_p;
            }
            set
            {
                _tc_noCrossLink_each_p = value;
                OnPropertyChanged("tc_noCrossLink_each_p");
            }
        }


        private int _tc_loadNoCross_p = 0;
        [Category("crosspie")]
        [DisplayName("tc_loadNoCross_p")]
        [Description("Loaded pages being without crosslinks")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int tc_loadNoCross_p
        {
            get
            {
                return _tc_loadNoCross_p;
            }
            set
            {
                _tc_loadNoCross_p = value;
                OnPropertyChanged("tc_loadNoCross_p");
            }
        }


        private int _freq_crosslinks_max = 0;
        [Category("core")]
        [DisplayName("freq_crosslinks_max")]
        [Description("The highest frequency of crosslink count, in set of selected pages")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.maxFreq)]
        //[imb(imbAttributeName.measure_operand, nameof())]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int freq_crosslinks_max
        {
            get
            {
                return _freq_crosslinks_max;
            }
            set
            {
                _freq_crosslinks_max = value;
                OnPropertyChanged("freq_crosslinks_max");
            }
        }


        private float _freq_crosslink_entropy = 0;
        [Category("core")]
        [DisplayName("freq_crosslink_entropy")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public float freq_crosslink_entropy
        {
            get
            {
                return _freq_crosslink_entropy;
            }
            set
            {
                _freq_crosslink_entropy = value;
                OnPropertyChanged("freq_crosslink_entropy");
            }
        }


        private int _iterations = 0;
        [Category("summary")]
        [DisplayName("iterations")]
        [Description("")]
        //[imb(imbAttributeName.reporting_valueformat, "#0.00")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        // [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        // [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int iterations
        {
            get
            {
                return _iterations;
            }
            set
            {
                _iterations = value;
                OnPropertyChanged("iterations");
            }
        }



        public override void setData(modelSpiderSiteRecord source)
        {
            //<----

        }
    }
}