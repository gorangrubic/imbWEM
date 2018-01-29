// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sampleGroupSetInPhd.cs" company="imbVeles" >
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

//using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace imbWEM.Core.sampleGroup
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
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// SampleGroupSet as defined by Goran Grubic PhD thesis research
    /// </summary>
    /// <seealso cref="sampleGroupSet" />
    public class sampleGroupSetInPhd : sampleGroupSet
    {
        /// <summary>
        /// Gets the primary sample.
        /// </summary>
        /// <value>
        /// The primary sample.
        /// </value>
        public sampleGroupItem primarySample { get; private set; }


        /// <summary>
        /// Gets the evaluation set a.
        /// </summary>
        /// <value>
        /// The evaluation set a.
        /// </value>
        public sampleGroupItem evaluationSetA { get; private set; }


        /// <summary>
        /// Gets or sets the evaluation set b.
        /// </summary>
        /// <value>
        /// The evaluation set b.
        /// </value>
        public sampleGroupItem evaluationSetB { get; private set; }


        /// <summary> </summary>
        public sampleGroupItem problem { get; protected set; } = new sampleGroupItem();


        /// <summary> </summary>
        public sampleGroupItem big { get; protected set; } = new sampleGroupItem();


        /// <summary>
        /// Constructs <see cref="sampleGroupSet"/> preset for my particular research
        /// </summary>
        public sampleGroupSetInPhd()
        {

            name = "Groups inside the Main Sample";

            primarySample = new sampleGroupItem("Primary sample", sampleGroupSetInPhdEnum.primary, 1, 50);
            primarySample.groupDescription = "Human post-processed, domain expert analysed and used as machine-learning training set";

            evaluationSetA = new sampleGroupItem("Evaluation set A", sampleGroupSetInPhdEnum.evaluation_a, 1, -1);
            evaluationSetA.groupDescription = "Dataset A - utilized during algorithm evaluation, meant for machine-only processing";

            evaluationSetB = new sampleGroupItem("Evaluation set B", sampleGroupSetInPhdEnum.evaluation_b, 2, -1);
            evaluationSetB.groupDescription = "Dataset B - utilized during algorithm evaluation, meant for machine-only processing";

            problem = new sampleGroupItem("Problem", sampleGroupSetInPhdEnum.problem, 1, 10);
            problem.groupDescription = "Cases where the crawler runs into blockade or crashes.";

            big = new sampleGroupItem("Big web sites", sampleGroupSetInPhdEnum.big, 1, 20);
            big.groupDescription = "Dataset with the biggest web sites in volume.";

            Add(primarySample);
            Add(evaluationSetA);
            Add(evaluationSetB);
            Add(problem);
            Add(big);
        }
    }

}