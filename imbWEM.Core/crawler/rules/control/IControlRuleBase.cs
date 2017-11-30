// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControlRuleBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.control
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
    using imbWEM.Core.crawler.core;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.rules.core;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public interface IControlRuleBase:IRuleBase
    {
        /// <summary> </summary>
        modelSpiderSiteRecord wRecord { get; }

        /// <summary>
        /// 
        /// </summary>
        spiderObjectiveType type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        spiderObjectiveStatus afirmative { get; set; }

        /// <summary>
        /// 
        /// </summary>
        spiderObjectiveStatus denial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        spiderObjectiveEnum objective { get; set; }

        /// <summary> </summary>
        int treshold { get; }

        /// <summary> </summary>
        int iteration { get; }

        /// <summary> </summary>
        spiderEvalRuleResultEnum mode { get; }

        /// <summary>
        /// Score reward for a link meeting criteria
        /// </summary>
        int scoreUnit { get; set; }

        /// <summary>
        /// Score penalty for a link not meeting criteria
        /// </summary>
        int penaltyUnit { get; set; }

        /// <summary>
        /// Reference to the evaluator that hosts this rule
        /// </summary>
        ISpiderEvaluatorBase parent { get;  }

        /// <summary>
        /// Priority of this rule
        /// </summary>
        int priority { get; set; }

        /// <summary>
        /// Caption name of the rule
        /// </summary>
        string name { get; set; }

        /// <summary>
        /// Describes how this rule affects the link or page - static part of description
        /// </summary>
        string description { get; set; }

        void onStartIteration();

        /// <summary>
        /// Starts new iteration
        /// </summary>
        /// <param name="currentIteration">The current iteration.</param>
        /// <param name="__wRecord">The w record.</param>
        void startIteration(int currentIteration, modelSpiderSiteRecord __wRecord);

        /// <summary>
        /// Appends its data points into new or existing property collection
        /// </summary>
        /// <param name="data">Property collection to add data into</param>
        /// <returns>Updated or newly created property collection</returns>
        PropertyCollectionExtended AppendDataFields(PropertyCollectionExtended data = null);

        /// <summary>
        /// Prepares this instance - clears temporary data
        /// </summary>
        void prepare();
    }

}