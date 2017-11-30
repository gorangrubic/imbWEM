// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dataUnitSpiderIteration.cs" company="imbVeles" >
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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbSCI.DataComplex.data.dataUnits.core.dataUnitRow{TInstance}.spider.model.modelSpiderSiteRecord}" />
    /// <seealso cref="imbSCI.DataComplex.data.dataUnits.core.IDataUnitSeriesEntry" />
    public class dataUnitSpiderIteration:dataUnitRow<modelSpiderSiteRecord>, IDataUnitSeriesEntry
    {
        public dataUnitSpiderIteration():base()
        {

        }

        private int _tc_detected_l = default(int); 
        [Category("timeline")]
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

                if (lastSpiderEntry.tc_detected_l == tc_detected_l)
                {
                    st_detected_l = lastSpiderEntry.st_detected_l+1;
                }
                else
                {
                    st_detected_l = 0;
                }

                OnPropertyChanged("tc_detected_p");
            }
        }


        private int _tc_loaded_p = default(int); // = new Int32();
        /// <summary>
        /// Total count of loaded pages
        /// </summary>
        [Category("summary,timeline")]
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
                _nw_loaded_p = _tc_loaded_p - lastSpiderEntry.tc_loaded_p;

                if (lastSpiderEntry.tc_loaded_p == tc_loaded_p)
                {
                    st_loaded_p = lastSpiderEntry.st_loaded_p+1;
                }
                else
                {
                    st_loaded_p = 0;
                }

                OnPropertyChanged("tc_loaded_p");
            }
        }


        private int _nw_loaded_p = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Pages loaded since last iteration
                                                      /// </summary>
        [Category("summary,dynamics,action")]
        [DisplayName("Pages discovered")]
        [Description("Pages loaded since last iteration")]
        public int nw_loaded_p
        {
            get
            {
                return _nw_loaded_p;
            }
            set
            {
                _nw_loaded_p = value;
                OnPropertyChanged("nw_loaded_p");
            }
        }



        private int _nw_processed_l = default(int); // = new Int32();
        /// <summary>
        /// Count of links were processed in last iteration
        /// </summary>
        [Category("summary,dynamica")]
        [DisplayName("Links processed")]
        [Description("Count of links were processed in last iteration")]
        public int nw_processed_l
        {
            get
            {
                return _nw_processed_l;
            }
            set
            {
                _nw_processed_l = value;
                OnPropertyChanged("nw_processed_l");
            }
        }


        private int _tc_detected_p = 0; // = new Int32();
                                                       /// <summary>
                                                       /// Total count of detected links
                                                       /// </summary>
        [Category("dataUnitSpiderIteration")]
        [DisplayName("Targets detected")]
        [Description("Total count of detected targets")]
        public int tc_detected_p
        {
            get
            {
                return _tc_detected_p;
            }
            set
            {
                _tc_detected_p = value;

                if (lastSpiderEntry.tc_detected_p == tc_detected_p)
                {
                    st_detected_p = lastSpiderEntry.st_detected_p + 1;
                }
                else
                {
                    st_detected_p = 0;
                }


                //OnPropertyChanged("tc_detected_p");
            }
        }

        public dataUnitSpiderIteration lastSpiderEntry
        {
            get
            {
                return lastEntry as dataUnitSpiderIteration;
                
            }
        }



        private int _tc_ingame_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Total count of links still in consideration to visit
                                                      /// </summary>
        [Category("summary,action,dynamics,timeline")]
        [DisplayName("Active links")]
        [Description("Total count of links still in consideration to visit")]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        public int tc_ingame_l
        {
            get
            {
                return _tc_ingame_l;
            }
            set
            {
                
                
                _tc_ingame_l = value;

                if (lastSpiderEntry.tc_ingame_l == tc_ingame_l)
                {
                    st_ingame_l++;
                } else
                {
                    st_ingame_l = 0;
                }
               

                OnPropertyChanged("tc_ingame_l");
            }
        }


        /// <summary>
        /// Stability of ingame links count
        /// </summary>
        /// <value>
        /// The st ingame l.
        /// </value>
        [Category("stability")]
        [DisplayName("Active links stability")]
        [Description("Total count of links still in consideration to visit")]
        [imb(imbAttributeName.measure_operand, nameof(tc_ingame_l))]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int st_ingame_l { get; set; }


        /// <summary>
        /// Stability of ruled out links in last iteration
        /// </summary>
        [Category("stability")]
        [DisplayName("Stab. r-out links")]
        [Description("Stability of ruled out links in last iteration")]
        [imb(imbAttributeName.measure_operand, nameof(nw_ruledout_l))]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        public int st_ruledout_l { get; set; } = default(int);


        /// <summary>
                                                      /// Stability of loaded pages count
                                                      /// </summary>
        [Category("stability")]
        [DisplayName("Stab. loaded")]
        [Description("Stability of loaded pages count")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        [imb(imbAttributeName.measure_operand, nameof(tc_loaded_p))]
        public int st_loaded_p { get; set; } = default(int);


        private int _st_detected_p;
        /// <summary> </summary>
        [Category("stability, timeline")]
        [DisplayName("Stab. targets")]
        [Description("Stability of target detected count ")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        [imb(imbAttributeName.measure_operand, nameof(tc_detected_p))]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        public int st_detected_p
        {
            get
            {
                return _st_detected_p;
            }
            set
            {
                _st_detected_p = value;
                OnPropertyChanged("st_detected_p");
            }
        }


        /// <summary>
                                                      /// Stability of links detected count 
                                                      /// </summary>
        [Category("stability, timeline")]
        [DisplayName("Stab. links")]
        [Description("Stability of links detected count ")]
        [imb(imbAttributeName.reporting_function, monitoringFunctionEnum.stability)]
        [imb(imbAttributeName.measure_operand, nameof(tc_detected_l))]
        [imb(imbAttributeName.reporting_agregate_function, monitoringFunctionEnum.final)]
        public int st_detected_l { get; set; } = default(int);


        private int _avg_score_l_trend = 0; //= default(Int32); // = new Int32();
                                              /// <summary>
                                              /// Description of $property$
                                              /// </summary>
        [Category("dataUnitSpiderIteration")]
        [DisplayName("avg_score_l_trend")]
        [Description("Description of $property$")]
        public int avg_score_l_trend
        {
            get
            {
                return _avg_score_l_trend;
            }
            protected set
            {
                _avg_score_l_trend = value;
                OnPropertyChanged("avg_score_l_trend");
            }
        }

        private float _avg_score_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Average link score over the web site - before drop out
                                                      /// </summary>
        [Category("complete")]
        [DisplayName("Avg b.r. link score")]
        [Description("Average link score over the web site - before drop out")]
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

                double lastScore = lastSpiderEntry.avg_score_l;
                int lastTrend = lastSpiderEntry.avg_score_l_trend;

                _avg_score_l_trend = lastTrend;
                if (lastScore < _avg_score_l)
                {
                    if (_avg_score_l_trend < 0) _avg_score_l_trend = 0;
                    _avg_score_l_trend++;
                }
                else if (lastScore > _avg_score_l) {
                    if (_avg_score_l_trend > 0) _avg_score_l_trend = 0;
                    _avg_score_l_trend--;

                } else 
                {
                    // <------- no changes in trend, just copy
                    
                }


                OnPropertyChanged("avg_score_l_trend");
            }
        }


        private int _tc_score_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Total active link score - before drop out
                                                      /// </summary>
        [Category("action,dynamics")]
        [DisplayName("Sum link score")]
        [Description("Total active link score - before drop out")]
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



        private float _avg_startScore_l = default(int); // = new Int32();
        /// <summary>
        /// Average link score at beginning of the iteration
        /// </summary>
        [Category("action")]
        [DisplayName("Avg link score")]
        [Description("Average link score at beginning of the iteration")]
        [imb(imbAttributeName.reporting_valueformat, "#0.00")]
        public float avg_scoreADO_l
        {
            get
            {
                return _avg_startScore_l;
            }
            set
            {
                _avg_startScore_l = value;
                OnPropertyChanged("avg_startScore_l");
            }
        }


        private int _tc_scoreADO_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Total active link score - after drop out
                                                      /// </summary>
        [Category("action")]
        [DisplayName("Total link score")]
        [Description("Total active link score - after drop out")]
        public int tc_scoreADO_l
        {
            get
            {
                return _tc_scoreADO_l;
            }
            set
            {
                _tc_scoreADO_l = value;
                OnPropertyChanged("tc_scoreADO_l");
            }
        }



        private int _nw_detected_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Links detected since last iteration
                                                      /// </summary>
        [Category("dynamics")]
        [DisplayName("Links detected")]
        [Description("Links detected since last iteration")]
        public int nw_detected_l
        {
            get
            {
                return _nw_detected_l;
            }
            set
            {
                _nw_detected_l = value;
                OnPropertyChanged("nw_detected_l");
            }
        }


        private int _lf_detected_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Links load failures
                                                      /// </summary>
        [Category("summary,dynamics")]
        [DisplayName("Links failed")]
        [Description("Links load failures")]
        public int nw_failed_l
        {
            get
            {
                return _lf_detected_l;
            }
            set
            {
                _lf_detected_l = value;
                OnPropertyChanged("lf_detected_l");
            }
        }


        private int _nw_ruledout_l = default(int); // = new Int32();
                                                      /// <summary>
                                                      /// Links ruled-out since last iteration
                                                      /// </summary>
        [Category("dynamics")]
        [DisplayName("Links ruled out")]
        [Description("Links ruled-out since last iteration")]
        public int nw_ruledout_l
        {
            get
            {
                return _nw_ruledout_l;
            }
            set
            {
                _nw_ruledout_l = value;

                if (lastSpiderEntry.nw_ruledout_l == nw_ruledout_l)
                {
                    st_ruledout_l = lastSpiderEntry.st_ruledout_l+1;
                }
                else
                {
                    st_ruledout_l = 0;
                }

                OnPropertyChanged("nw_ruledout_l");
            }
        }

        public override void setData(modelSpiderSiteRecord source)
        {
            // <----------- direktno primenjeno preuzimanje statistike
        }
    }
}
