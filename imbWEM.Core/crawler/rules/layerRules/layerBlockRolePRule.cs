// --------------------------------------------------------------------------------------------------------------------
// <copyright file="layerBlockRolePRule.cs" company="imbVeles" >
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
namespace imbWEM.Core.crawler.rules.layerRules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.contentBlock;
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
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class layerBlockRolePRule : layerDistributionPassiveRuleBase
    {
        public layerBlockRolePRule(nodeBlockSemanticRoleEnum __semanticRole, int __layerID, ISpiderEvaluatorBase __parent, int __layer2ID = -1)
             : base("Block Role ({0})", "Links found in a block with semantic role [{0}] are set to layerID [{1}], otherwise to [{2}].", __layerID, __parent, __layer2ID)
            //: base(__name, __description, __layerID, __parent, __layer2ID)
        {
            semanticRole = __semanticRole;
            name = string.Format(name, semanticRole);
            description = string.Format(description, semanticRole, layerID, layer2ID);

            __parent.settings.doEnableDLC_BlockTree = true;
        }


        /// <summary>
        /// 
        /// </summary>
        public nodeBlockSemanticRoleEnum semanticRole { get; set; }

        /// <summary>
        /// Evaluates the specified link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns></returns>
        public override spiderEvalRuleResult evaluate(spiderLink link)
        {
            try
            {
                spiderEvalRuleResult result = new spiderEvalRuleResult(this);
                spiderTarget target = wRecord.context.targets.GetByOrigin(link);

                if (target == null)
                {
                    result.layer = layer2ID;
                    return result;
                }

                nodeBlock bl = target.contentBlocks.GetBlockByXPath(link.link.xPath);

                if (bl == null)
                {
                    result.layer = layer2ID;
                    return result;
                }

                if (bl.role == semanticRole)
                {
                    result.layer = layerID;
                }
                else
                {
                    result.layer = layer2ID;
                }

                return result;
            } catch (Exception ex)
            {
                throw new aceGeneralException(ex.Message, ex, this, "layerBlockRolePRule broken");
            }
            return new spiderEvalRuleResult(this);
        }

        public override void prepare()
        {
            //
        }

        public override void onStartIteration()
        {
            
        }
    }

}