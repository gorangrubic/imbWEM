// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pluginSupport.cs" company="imbVeles" >
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
namespace imbWEM.Core.plugins
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
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.plugins.content;
    using imbWEM.Core.plugins.crawler;
    using imbWEM.Core.plugins.engine;
    using imbWEM.Core.plugins.index;
    using imbWEM.Core.plugins.report;
    using imbWEM.Core.stage;
    using imbSCI.Core.extensions.typeworks;

    public static class pluginSupport
    {


        public static plugIn_base GetPluginInstance(string plugin_className, builderForLog loger, IAPlugInCollectionBase collection)
        {
            plugIn_base plug = null;

            if (imbWEMManager.settings.supportEngine.plugins.Keys.Contains(plugin_className))
            {
                plug = imbWEMManager.settings.supportEngine.plugins[plugin_className].getInstance()  as plugIn_base;


                if (plug is indexPlugIn_base)
                {
                    indexPlugIn_base plug_indexPlugIn_base = plug as indexPlugIn_base;
                    loger.log("Plugin instance [" + plug.name + "] for Index Engine created");
                    

                }
                else if (plug is enginePlugIn_base)
                {
                    loger.log("Plugin instance [" + plug.name + "] for Crawl Job Engine created");
                    //imbWEMManager.index.plugins.installPlugIn(plug as IPlugInCommonBase);
                }
                else if (plug is crawlerPlugIn_base)
                {
                    loger.log("Plugin instance [" + plug.name + "] for Crawler created");
                    //imbWEMManager.index.plugins.installPlugIn(plug as IPlugInCommonBase);
                }
                else if (plug is reportPlugIn_base)
                {
                    loger.log("Plugin instance [" + plug.name + "] for Reporting created");
                //    imbWEMManager.index.plugins.installPlugIn(plug as IPlugInCommonBase);
                } else
                {
                    loger.log("Plugin instance [" + plug.name + "] of unknown category created... ");
                }


                if (collection!=null) collection.installPlugIn(plug);

            } else
            {
                loger.AppendLine("Plugin [" + plugin_className + "] not found... ");
            }

            return plug;
        }
    }
}
