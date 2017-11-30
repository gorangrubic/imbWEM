// --------------------------------------------------------------------------------------------------------------------
// <copyright file="moduleIterationRecord.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.modules.performance
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
    using imbSCI.Core.enums;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.math;
    using imbSCI.Core.math.aggregation;
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
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.index.core;
    using imbWEM.Core.stage;

    [imb(imbAttributeName.reporting_categoryOrder, "report,execution,input,workload,distribution,flush,output,result")]
    public class moduleIterationRecord: frontierEntryCommonBase
    {

        public void reportEvaluateStart(spiderModuleData<spiderLink> input, modelSpiderSiteRecord wRecord, spiderModuleBase moduleInstance)
        {

            
            start = DateTime.Now;
            iteration = wRecord.iteration;

            int cyclers_c = 0;
            int recyclers_c = 0;
            int cyclers_age_c = 0;
            int input_age = 0;

            foreach (spiderLink link in input.active)
            {
                inputTargets_collection.Add(link.url);

                if (link.marks.cycleCount > 0)
                {
                    if (link.marks.cycleLastIteration == (iteration-1))
                    {
                        cyclers_c++;
                        cyclers_age_c += iteration - link.iterationDiscovery;

                    } else if (link.marks.cycleLastIteration < (iteration-1))
                    {
                        recyclers_c++;
                    }
                    
                }

                input_age += iteration - link.iterationDiscovery;
            }
            
            inputTargets = input.active.Count();
            processed = inputTargets; // <-- razlika je samo u agregaciji


            age = input_age.GetRatio(inputTargets);
            

            inputTargets_assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(inputTargets_collection);
            inputPotentialPrecission = inputTargets_assertion.relevant;
            evaluationCertainty = inputTargets_assertion.certainty;
            inputTargets_assertion.performInfoGainEstimation();

            PotInputIP = inputTargets_assertion.IPnominal;


            targets = inputTargets;

            layerModule = moduleInstance as spiderLayerModuleBase;

            cyclers = cyclers_c.GetRatio(inputTargets);
            recyclers = recyclers_c.GetRatio(inputTargets);

            if (layerModule != null)
            {
                accumulation = layerModule.layers.CountAll;

                targets += accumulation;
            }


        }
        
        public void reportEvaluateEnd(IList<spiderLink> __output, modelSpiderSiteRecord wRecord, spiderModuleBase moduleInstance)
        {
            indexURLAssertionResult L01_assertion = new indexURLAssertionResult();
            indexURLAssertionResult L02_assertion = new indexURLAssertionResult();
            indexURLAssertionResult L03_assertion = new indexURLAssertionResult();
            layerModule = moduleInstance as spiderLayerModuleBase;
            if (layerModule != null)
            {
                accumulated = layerModule.layers.CountAll - accumulation;

                // << --- AGE
                int input_age = 0;
                primaryLayerTargets = layerModule.layers[0].Count;
                foreach (spiderLink link in layerModule.layers[0])
                {
                    input_age += (iteration - link.iterationDiscovery);
                    L01_assertion.Add(imbWEMManager.index.pageIndexTable.GetPageAssertion(link.url), link.url);
                }
                primaryLayerTargetAge = input_age.GetRatio(primaryLayerTargets);
                primaryLayerInproperDistribution = L01_assertion.notRelevant;
                input_age = 0;

                if (layerModule.layers.Count > 1)
                {
                    secondaryLayerTargets = layerModule.layers[1].Count;
                    foreach (spiderLink link in layerModule.layers[1])
                    {
                        input_age += (iteration - link.iterationDiscovery);
                        L02_assertion.Add(imbWEMManager.index.pageIndexTable.GetPageAssertion(link.url), link.url);
                    }
                    secondaryLayerTargetAge = input_age.GetRatio(secondaryLayerTargets);
                    if (primaryLayerTargets > 0)
                    {
                        secondaryLayerInproperDistribution = L02_assertion.relevant;
                    }
                }

                input_age = 0;
                if (layerModule.layers.Count > 2)
                {
                    reserveLayerTargets = layerModule.layers[2].Count;
                    foreach (spiderLink link in layerModule.layers[2])
                    {
                        input_age += (iteration - link.iterationDiscovery);
                        L03_assertion.Add(imbWEMManager.index.pageIndexTable.GetPageAssertion(link.url), link.url);
                    }
                    reserveLayerTargetAge = input_age.GetRatio(reserveLayerTargets);

                    if (secondaryLayerTargets > 0)
                    {
                        reserveLayerInproperDistribution = L03_assertion.relevant;
                    }
                }
                
            }

            output = __output.Count;

            if (inputTargets > output)
            {
                Reduction = (inputTargets - output).GetRatio(inputTargets);
            }
            else
            {
                drain = output - inputTargets; //).GetRatio(inputTargets);
            }

            out_assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(__output);
            out_assertion.performInfoGainEstimation();

            LTn = wRecord.context.GetNextIterationLTSize(__output);

            outCut_assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(__output.Take(LTn));


            PotOutputIP = out_assertion.IPnominal;

            PotChangeIP = PotOutputIP - PotInputIP;

            // out_preRanking.AddRange(__output.active.Take(wRecord.tRecord.instance.settings.limitIterationNewLinks));

            outputPotentialPrecission = out_assertion.relevant;

            potentialPrecissionChange = outputPotentialPrecission - inputPotentialPrecission;
        }

        protected int LTn = 0;
        protected List<string> out_preRanking = new List<string>();


        public void reportEvaluateAlterRanking(IList<spiderLink> output, modelSpiderSiteRecord wRecord, spiderModuleBase moduleInstance)
        {
            
            if (layerModule != null)
            {
                switch (layerModule.layers.layer_id)
                {
                    case 0:
                        
                        primaryLayerForOutput = 1;
                        break;
                    case 1:
                        
                        secondaryLayerForOutput = 1;
                        break;
                    case 2:
                        
                        reserveLayerForOutput = 1;
                        break;
                }
            }


            // Single Output -- reducing the chain
            if (output.Count == wRecord.tRecord.instance.settings.limitIterationNewLinks)
            {
               
               singleTargetOutput = 1;
            }


            indexURLAssertionResult LT_assertion = imbWEMManager.index.pageIndexTable.GetUrlAssertion(output);
            potentialLoadTakePrecission = LT_assertion.relevant;

            rankingEffects = potentialLoadTakePrecission - outCut_assertion.relevant;

            
            duration = DateTime.Now.Subtract(start).TotalSeconds;

            
        }

        public void disposeResources()
        {
            layerModule = null;
            outCut_assertion = null;
            out_assertion = null;
            targets_collection = null;
            targets_assertion = null;
            inputTargets_collection = null;
            inputTargets_assertion = null;
            out_preRanking = null;
        }


        private spiderLayerModuleBase layerModule { get; set; }
        private indexURLAssertionResult out_assertion { get; set; }
        private indexURLAssertionResult outCut_assertion { get; set; }
        private List<string> targets_collection { get;  set; } = new List<string>();
        private indexURLAssertionResult targets_assertion { get;  set; } = new indexURLAssertionResult();
        private List<string> inputTargets_collection { get;  set; } = new List<string>();
        private indexURLAssertionResult inputTargets_assertion { get; set; } = null;

        /// <summary>
        /// How long it took for this module to process the targets, in seconds
        /// </summary>
        [Category("Execution")]
        [DisplayName("Duration")]
        [Description("How long it took for this module to process the targets, in seconds")]
        [imb(imbAttributeName.measure_letter, "MT_t")]
        [imb(imbAttributeName.measure_setUnit, "s")]
        //[imb(imbAttributeName.viewPriority, 3)]
        public double duration { get; set; } = 0;


        /// <summary>
        /// Number of targets processed by this module so far
        /// </summary>
        [Category("Execution")]
        [DisplayName("Processed")]
        [Description("Number of target Cycles processed by this module so far")]
        [imb(imbAttributeName.measure_letter, "Φ(T)")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        //[imb(imbAttributeName.viewPriority, 4)]
        public int processed { get; set; } = 0;


        /// <summary>
        /// Number of targets module received
        /// </summary>
        [Category("Execution")]
        [DisplayName("Targets")]
        [Description("Total number of targets available for consideration: Active Targets + Accumulation")]
        [imb(imbAttributeName.measure_letter, "|T_c| = |AT| + ∑|FL|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        //[imb(imbAttributeName.viewPriority, 5)]
        [imb(imbAttributeName.reporting_hide)]
        public int targets { get; set; } = 0;


        /// <summary>
        /// Number of targets module received
        /// </summary>
        [Category("Input")]
        [DisplayName("Input")]
        [Description("Number of input targets sent to this module")]
        [imb(imbAttributeName.measure_letter, "|MT|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        //[imb(imbAttributeName.viewPriority, 6)]
        public int inputTargets { get; set; } = 0;


        [Category("Input")]
        [DisplayName("Certainty")]
        [Description("Measure of potential precision certainty: T_known / T_evl (targets having evaluation meta data attached)")]
        [imb(imbAttributeName.measure_letter, "≈γ")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 7)]
        public double evaluationCertainty { get; set; } = 0;

        [Category("Input")]
        [DisplayName("Pot. Precision")]
        [Description("Potential Precision value of targets that entered the module")]
        [imb(imbAttributeName.measure_letter, "γ(MT)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 8)]
        public double inputPotentialPrecission { get; set; } = 0;

        [Category("Input")]
        [DisplayName("Input Age")]
        [imb(imbAttributeName.measure_letter, "MT_age")]
        [imb(imbAttributeName.measure_setUnit, "n of iterations")]
        [Description("Average age of targets contained in the layers")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 9)]
        public double age { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("Accumulation")]
        [Description("Number of targets that was accumulated to the modules' layers in this iteration")]
        [imb(imbAttributeName.measure_letter, "∑|FL|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 10)]
        public int accumulation { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("Accumulated")]
        [Description("Number of targets that was accumulated to the modules' layers in this iteration")]
        [imb(imbAttributeName.measure_letter, "∆∑|FL|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 11)]
        public int accumulated { get; set; } = 0;



        [Category("Workload")]
        [DisplayName("Cyclers")]
        [imb(imbAttributeName.measure_letter, "|MT_c|")]
        [imb(imbAttributeName.measure_setUnit, "% in |T_c|")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [Description("% of Targets (in |T_c|) that made more than one cycle trough this module")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 12)]
        public double cyclers { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("reCyclers")]
        [imb(imbAttributeName.measure_letter, "|MT_c^i-1|")]
        [imb(imbAttributeName.measure_setUnit, "n of links")]
        [Description("Links that were not in the cycle in the last iteration - but droped from a layer and came back to the cycle")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 13)]
        public double recyclers { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("Cyclers Age")]
        [imb(imbAttributeName.measure_letter, "MT_c(∆I)")]
        [imb(imbAttributeName.measure_setUnit, "Summed age of the cycling targets")]
        [Description("Average age of targets contained in the layers")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 14)]
        public double cyclersAge { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("FL0 Primary")]
        [imb(imbAttributeName.measure_letter, "|FL0|")]
        [imb(imbAttributeName.measure_setUnit, "n of links")]
        [Description("Total number of targets held in the primary layer - before ranking")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        //[imb(imbAttributeName.viewPriority, 15)]
        [imb(imbAttributeName.reporting_hide)]
        public int primaryLayerTargets { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("FL1 Secondary")]
        [imb(imbAttributeName.measure_letter, "|FL1|")]
        [imb(imbAttributeName.measure_setUnit, "n of links")]
        [Description("Total number of targets held in the primary layer - before ranking")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 16)]
        public int secondaryLayerTargets { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("FL2 Reserve")]
        [imb(imbAttributeName.measure_letter, "|FL2|")]
        [imb(imbAttributeName.measure_setUnit, "n of links")]
        [Description("Total number of targets held in the primary layer - before ranking")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 17)]
        public int reserveLayerTargets { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("FL0 Primary")]
        [imb(imbAttributeName.measure_letter, "age(FL0)")]
        [imb(imbAttributeName.measure_setUnit, "|i|/|FL0|")]
        [Description("Average age since discovery (number of iterations)")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 18)]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.hidden)]
        [imb(imbAttributeName.reporting_hide)]
        public double primaryLayerTargetAge { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("FL1 Secondary")]
        [imb(imbAttributeName.measure_letter, "age(FL1)")]
        [imb(imbAttributeName.measure_setUnit, "|i|/|FL0|")]
        [Description("Average age since discovery (number of iterations)")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 19)]
        public double secondaryLayerTargetAge { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("FL2 Reserve")]
        [imb(imbAttributeName.measure_letter, "age(FL2)")]
        [imb(imbAttributeName.measure_setUnit, "|i|/|FL0|")]
        [Description("Average age since discovery (number of iterations)")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 20)]
        public double reserveLayerTargetAge { get; set; } = 0;



        [Category("Distribution")]
        [DisplayName("FL0 Primary λ")]
        [imb(imbAttributeName.measure_letter, "λ(FL0)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Percentage of wrong distributions to the primary layer, criterion: if it is known to be irrelevant")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 24)]
        public double primaryLayerInproperDistribution { get; set; } = 0;


        [Category("Distribution")]
        [DisplayName("FL1 Secondary λ")]
        [imb(imbAttributeName.measure_letter, "λ(FL1)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Percentage of wrong distributions to the secondary layer, criterion: if it is relevant and the Primary layer is not empty")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 25)]
        public double secondaryLayerInproperDistribution { get; set; } = 0;


        [Category("Distribution")]
        [DisplayName("FL2 Reserve λ")]
        [imb(imbAttributeName.measure_letter, "λ(FL2)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Percentage of wrong distributions to the reserve layer, criterion: if it is relevant and the Secondary layer is not empty")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 26)]
        public double reserveLayerInproperDistribution { get; set; } = 0;


        [Category("Workload")]
        [DisplayName("Ranking effects")]
        [imb(imbAttributeName.measure_letter, "σ(MT) = γ(MT_rnk)-γ(MT_out)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [Description("Measure how much the ranking step changed pot. precision of the first LT targets in output")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 27)]
        public double rankingEffects { get; set; } = 0;


        //[Category("Workload")]
        //[DisplayName("Ranking effects")]
        //[imb(imbAttributeName.measure_letter, "σ(MT)=γ(MT_rnk)-γ(MT_out)")]
        //[imb(imbAttributeName.measure_setUnit, "%")]
        //[Description("Measure how much the ranking step changed content of first LT targets in output")]
        //[imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //public Double rankingEffects { get; set; } = 0;

        [Category("Workload")]
        [DisplayName("Drain")]
        [Description("When no targets at input - the modules will release their targets from secundary and reserve module layers - this number is> output - input, when output > input")]
        [imb(imbAttributeName.measure_letter, "∆|MT→|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 28)]
        public int drain { get; set; } = 0;

        /// <summary>
        /// Percentage of input targets that the module took out in this iteration (output / input) - i.e. amount of work reduction, only pozitive value is allowed
        /// </summary>
        [Category("Workload")]
        [DisplayName("Reduction")]
        [imb(imbAttributeName.measure_letter, "φ(MT) = ∆∑|FL| / |MT|")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        //[imb(imbAttributeName.viewPriority, 29)]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        [Description("Percentage of input targets that the module took out in this iteration (output / input) - i.e. amount of work reduction, only positive value is allowed")]
        public double Reduction { get; set; } = 0;




        [Category("Flush")]
        [DisplayName("Primary")]
        [imb(imbAttributeName.measure_letter, "FL0→")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Total number of targets held in the primary layer - before ranking")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        //[imb(imbAttributeName.viewPriority, 21)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        public int primaryLayerForOutput { get; set; } = 0;



        /// <summary>
        /// how many times the module forwarder targets from the secondary layer
        /// </summary>
        [Category("Flush")]
        [DisplayName("Secondary")]
        [imb(imbAttributeName.measure_letter, "FL1→")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Iterations the module forwarded targets from this layer")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        //[imb(imbAttributeName.viewPriority, 22)]
        public int secondaryLayerForOutput { get; set; } = 0;


        /// <summary>
        /// how many times the module drained the reserve layer for output
        /// </summary>
        [Category("Flush")]
        [DisplayName("Reserve")]
        [imb(imbAttributeName.measure_letter, "FL2→")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Iterations module drained the reserve layer for output")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        //[imb(imbAttributeName.viewPriority, 23)]
        public int reserveLayerForOutput { get; set; } = 0;


        [Category("Input")]
        [DisplayName("→ Potential IP")]
        [imb(imbAttributeName.measure_letter, "IPn_i")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP_i}/∆|PL|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score of the input, average on MIN(∆|PL|,|AT|) targets in the output")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotInputIP { get; set; } = 0;


        [Category("Result")]
        [DisplayName("→ IP Change →")]
        [imb(imbAttributeName.measure_letter, "∆γ(IPn)")]
        [imb(imbAttributeName.measure_setUnit, "IPn_o - IPn_i")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score difference, between input and the output, for MIN(∆|PL|,|LT|) targets")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotChangeIP { get; set; } = 0;


        [Category("Output")]
        [DisplayName("Potential IP →")]
        [imb(imbAttributeName.measure_letter, "IPn_o")]
        [imb(imbAttributeName.measure_setUnit, "∑{IP}/∆|MT|")]
        [imb(imbAttributeName.reporting_valueformat, "F4")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [Description("Potential nominal IP score of the output, average on |MT| targets in the output")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double PotOutputIP { get; set; } = 0;


        [Category("Output")]
        [DisplayName("Output")]
        [Description("Number of targets sent for the ranking and LT selection")]
        [imb(imbAttributeName.measure_letter, "|MT_out|")]
        [imb(imbAttributeName.measure_setUnit, "n of Targets")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 32)]
        public int output { get; set; } = 0;

        /// <summary> Ratio </summary>
        [Category("Output")]
        [DisplayName("Pot. Precission")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        [imb(imbAttributeName.measure_letter, "ω(MT_out)")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P2")]
        //[imb(imbAttributeName.viewPriority, 30)]
        [Description("Potential precision of the Targets forwarded as Load Take input")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public double outputPotentialPrecission { get; set; } =0;


        [Category("Result")]
        [DisplayName("Pot. Prec. Change")]
        [Description("What difference the frontier layers made from MT_ipp to MT_opp. Positive value means the pot. precision  is increased.")]
        [imb(imbAttributeName.measure_setUnit, "%")]
        [imb(imbAttributeName.reporting_valueformat, "P3")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 31)]
        [imb(imbAttributeName.measure_important, dataPointImportance.important)]
        public double potentialPrecissionChange { get; set; } = 0;

        


        [Category("Result")]
        [DisplayName("Potential LT Precission")]
        [Description("If the output of the module is forwarded to the Loader component - what would be the precission of such load? Different to outputPrecissionOutput because it respects the load take size ")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.avg)]
        //[imb(imbAttributeName.viewPriority, 33)]
        public double potentialLoadTakePrecission { get; set; } = 0;




        /// <summary>
        /// How many times this module forwarded only one target
        /// </summary>
        [Category("Result")]
        [DisplayName("Exact Target Output ")]
        [Description("How many times this module forwarded only one target - preventing workload for the rest of the modules")]
        [imb(dataPointAggregationAspect.overlapMultiTable, dataPointAggregationType.sum)]
        //[imb(imbAttributeName.viewPriority, 34)]
        public int singleTargetOutput { get; set; } = 0;








    }

}