// --------------------------------------------------------------------------------------------------------------------
// <copyright file="spiderTargetTokens.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.targets
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
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
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.data.modify;
    using imbSCI.DataComplex.extensions.data.schema;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    /// <summary>
    /// Not expanded TF-IDF collection
    /// </summary>
    /// <seealso cref="aceCommonTypes.collection.tf_idf.weightTable{aceCommonTypes.collection.tf_idf.weightTableGenericTerm}" />
    public class spiderTargetTokens : weightTable<weightTableGenericTerm>
    {
        public override bool termSingleAddAllowed
        {
            get
            {
                return false;
            }
        }

        public override DataRow buildTableRow(DataRow dr, weightTableGenericTerm t)
        {
            dr.SetData(termTableColumns.termName, t.name);
            dr.SetData(termTableColumns.freqAbs, termsAFreq[t.name]);
            dr.SetData(termTableColumns.freqNorm, GetNFreq(t.name));
            dr.SetData(termTableColumns.df, GetBDFreq(t.name));
            dr.SetData(termTableColumns.idf, GetIDF(t.name));
            dr.SetData(termTableColumns.tf_idf, GetTF_IDF(t.name));
            
           // dr.SetData(termTableColumns.words, t.Count());
            dr.SetData(termTableColumns.cw, GetWeight(t.name));
            dr.SetData(termTableColumns.ncw, GetNWeight(t.name));
            return dr;
        }

        public override DataTable buildTableShema(DataTable output)
        {
            output.Add(termTableColumns.termName, "Nominal form of the term", "T_n", typeof(string), dataPointImportance.normal);
            output.Add(termTableColumns.freqAbs, "Absolute frequency - number of occurences", "T_af", typeof(int), dataPointImportance.normal, "", "Abs. freq.");
            output.Add(termTableColumns.freqNorm, "Normalized frequency - abs. frequency divided by the maximum", "T_nf", typeof(double), dataPointImportance.important, "#0.00000");
            output.Add(termTableColumns.df, "Document frequency - number of documents containing the term", "T_df", typeof(int), dataPointImportance.normal);
            output.Add(termTableColumns.idf, "Inverse document frequency - logaritmicly normalized T_df", "T_idf", typeof(double), dataPointImportance.normal, "#0.00000");
            output.Add(termTableColumns.tf_idf, "Term frequency Inverse document frequency - calculated as TF-IDF", "T_tf-idf", typeof(double), dataPointImportance.important, "#0.00000");
           // output.Add(termTableColumns.words, "Number of words in the expanded term", "T_c", typeof(Int32), dataPointImportance.normal, "");  // , "Cumulative weight of term", "T_cw", typeof(Double), dataPointImportance.normal, "#0.00000");
            output.Add(termTableColumns.cw, "Cumulative weight of all TermInstance-s of the term spark that were found in the query", "T_cw", typeof(double), dataPointImportance.normal, "#0.00000");
            output.Add(termTableColumns.ncw, "Normalized cumulative weight of term", "T_ncw", typeof(double), dataPointImportance.important, "#0.00000");
            return output;
        }
    }

}