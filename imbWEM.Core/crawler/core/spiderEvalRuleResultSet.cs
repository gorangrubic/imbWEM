// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderEvalRuleResultSet.cs" company="imbVeles" >
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

namespace imbWEM.Core.crawler.core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Text;
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;
    using imbSCI.DataComplex.extensions.data;

    /// <summary>
    /// Rule result record set
    /// </summary>
    public class spiderEvalRuleResultSet
    {
        public const int BANSCORE = -100;

        public string GetLayerAssociation(string format = "{0} => ({1})")
        {
            format = format + Environment.NewLine;
            StringBuilder output = new StringBuilder();
            output.AppendLine("Layer distribution by passive rules:");
            foreach (var pair in passive)
            {
                output.AppendFormat(format, pair.Key, pair.Value.layer);
            }
            output.AppendLine("Layer distribution by active rules:");
            foreach (var pair in active)
            {
                output.AppendFormat(format, pair.Key, pair.Value.layer);
            }
            return output.ToString();
        }

        /// <summary>
        /// Gets the active results as String
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public string GetActiveResults(string format = "{0}=>({1})")
        {
            string output = "";
            foreach (var pair in active)
            {
                if (pair.Value.ban)
                {
                    output = output.add(string.Format(format, pair.Key, "ban"));
                } else
                {
                    output = output.add(string.Format(format, pair.Key, pair.Value.score));
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the passive results as String
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public string GetPassiveResults(string format = "{0}=>({1})")
        {
            string output = "";
            foreach (var pair in passive)
            {
                if (pair.Value.ban)
                {
                    output = output.add(string.Format(format, pair.Key, "ban"));
                }
                else
                {
                    output = output.add(string.Format(format, pair.Key, pair.Value.score));
                }
            }
            return output;
        }

        public int getStability()
        {
            KeyValuePair<int, int> last = new KeyValuePair<int, int>(0, 0);
            int lastChange = 0;
            int lastStability = 0;
            foreach (KeyValuePair<int, int> it in history)
            {
                PropertyCollection row = new PropertyCollection();
                row.Add(templateResultColumn.res_iteration, it.Key);
                row.Add(templateResultColumn.res_score, it.Value);
                int change = it.Value - last.Value;
                int stability = lastStability;
                row.Add(templateResultColumn.res_change, change);
                if (it.Value > 0)
                {
                    if (change == lastChange)
                    {
                        stability++;
                    }
                    else
                    {
                        stability--;
                    }
                }
                row.Add(templateResultColumn.res_stability, stability);
                lastChange = change;
                lastStability = stability;
                last = it;
            }
            return lastStability;
        }
        /// <summary>
        /// Gets time series of changes
        /// </summary>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public DataTable getHistory(string instanceID, string process)
        {
            PropertyCollectionExtended shema = new PropertyCollectionExtended();
            shema.Add(templateResultColumn.res_iteration, 0, "Iteration", "Time coordinate");
            shema.Add(templateResultColumn.res_score, 0, "Score", "Total score sum");
            shema.Add(templateResultColumn.res_change, 0, "Change", "Diference between current value and last iteration");
            shema.Add(templateResultColumn.res_stability, 0, "Stability", "On score unchanged adds +1 per each iteration. If change occoured -1 is applied. Value is alyways above 0");

            //PropertyEntryDictionary columns = shema.buildColumnDictionary(PropertyEntryColumn.entry_name | PropertyEntryColumn.entry_value )

            KeyValuePair<int, int> last = new KeyValuePair<int, int>(0, 0);
            List<PropertyCollection> rows = new List<PropertyCollection>();
            DataTable dt = shema.buildDataTableVertical("Score history", "Data for [" + instanceID + "] during execution of [" + process + "]");
            int lastChange = 0;
            int lastStability = 0;
            foreach (KeyValuePair<int, int> it in history)
            {
                PropertyCollection row = new PropertyCollection();
                row.Add(templateResultColumn.res_iteration, it.Key);
                row.Add(templateResultColumn.res_score, it.Value);
                int change = it.Value - last.Value;
                int stability = lastStability;
                row.Add(templateResultColumn.res_change, change);
                if (it.Value > 0) {
                    if (change == lastChange)
                    {
                        stability++;
                    } else
                    {
                        stability--;
                    }
                }
                row.Add(templateResultColumn.res_stability, stability);
                lastChange = change;
                lastStability = stability;
                last = it;
                rows.Add(row);
            }

            dt.addTableExtendedRows(rows);
            return dt;
        }


        /// <summary> </summary>
        public Dictionary<int, int> history { get; protected set; } = new Dictionary<int, int>();


        /// <summary>
        /// 
        /// </summary>
        public int score { get; protected set; }


        /// <summary>
        /// Calculates this instance.
        /// </summary>
        /// <returns></returns>
        public int calculate(int iteration=0)
        {
            if (passiveSum == -1)
            {
                passiveSum = 0;
                foreach (spiderEvalRuleResult pr in passive.Values)
                {
                    if (pr.role == spiderEvalRuleRoleEnum.rankScoring)
                    {
                        passiveSum += pr.score;
                        if (pr.ban)
                        {
                            passiveSum = BANSCORE;
                            break;
                        }
                    }
                }
            }

            activeSum = 0;
            foreach (spiderEvalRuleResult pr in passive.Values)
            {
                if (pr.role == spiderEvalRuleRoleEnum.rankScoring)
                {
                    activeSum += pr.score;
                    if (pr.ban)
                    {
                        activeSum = BANSCORE;
                        break;
                    }
                }
            }
            if ((passiveSum == BANSCORE) || (activeSum == BANSCORE)) return BANSCORE;
            int sum = passiveSum + activeSum;

            if (history.ContainsKey(iteration))
            {
                history[iteration] = sum;
            }
            else
            {
                history.Add(iteration, sum);
            }

            score = sum;
            return sum;
        }


        public void cycleRegistration(int iteration)
        {
            cycleCount++;
            cycleLastIteration = iteration;
        }


        /// <summary>
        /// 
        /// </summary>
        public int cycleLastIteration { get; protected set; }

        /// <summary>
        /// Number of cycles this link had passed
        /// </summary>
        public int cycleCount { get; protected set; } = 0;


        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void clear()
        {
            passive.Clear();
            active.Clear();
            passiveSum = -1;
            activeSum = -1;
        }


        /// <summary>
        /// Deploys the specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        public void deploy(spiderEvalRuleResult result)
        {
            if (result == null) return;
            
                switch (result.mode)
                {
                    default:
                    case spiderEvalRuleResultEnum.passive:
                        if (passive.ContainsKey(result.ruleTagName))
                        {
                            passive[result.ruleTagName] = result;
                        } else
                        {
                            passive.Add(result.ruleTagName, result);
                        }
                        passiveSum = -1;
                        break;
                    case spiderEvalRuleResultEnum.active:
                        if (active.ContainsKey(result.ruleTagName))
                        {
                            active[result.ruleTagName] = result;
                        }
                        else
                        {
                            active.Add(result.ruleTagName, result);
                        }
                        break;
                }
            
        }

        public spiderEvalRuleResult this[IRuleBase rule]
        {
            get
            {
                string rKey = rule.tagName;
                if (passive.ContainsKey(rKey)) return passive[rKey];
                if (active.ContainsKey(rKey)) return active[rKey];
                return null;
            }
        }


        /// <summary> </summary>
        public Dictionary<string, spiderEvalRuleResult> passive { get; protected set; } = new Dictionary<string, spiderEvalRuleResult>();


        /// <summary>
        /// Known value of passive sum
        /// </summary>
        public int passiveSum { get; protected set; } = -1;


        /// <summary> </summary>
        public Dictionary<string, spiderEvalRuleResult> active { get; protected set; } = new Dictionary<string, spiderEvalRuleResult>();


        /// <summary>
        /// 
        /// </summary>
        public int activeSum { get; protected set; } = -1;
    }

}