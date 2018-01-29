// --------------------------------------------------------------------------------------------------------------------
// <copyright file="nameComposer.cs" company="imbVeles" >
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
using imbSCI.Core.extensions.io;
using imbSCI.Core.reporting.template;
using imbWEM.Core.consolePlugin;
using imbWEM.Core.crawler.engine;
using imbWEM.Core.crawler.evaluators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;



namespace imbWEM.Core.console
{
    public static class nameComposer
    {
        public enum nameComposerFields
        {
            crawlerClassName,
            crawlerTitleName,
            crawlerFileFriendlyName,
            variableTCmax,
            variablePLmax,
            variableLT,
            sampleSize,
            sampleFileSource,
            sampleName
        }

        public static PropertyCollection GetData(crawlerDomainTaskMachineSettings crawlerJobEngineSettings, ISpiderEvaluatorBase crawler)
        {
            PropertyCollection data = new PropertyCollection();
            data[nameComposerFields.crawlerClassName] = crawler.GetType().Name;
            data[nameComposerFields.crawlerTitleName] = crawler.name;
            data[nameComposerFields.crawlerFileFriendlyName] = crawler.name.getCleanFilepath().Replace("-", "");
            data[nameComposerFields.variablePLmax] = crawler.settings.limitTotalPageLoad;
            data[nameComposerFields.variableLT] = crawler.settings.limitIterationNewLinks;
            data[nameComposerFields.variableTCmax] = crawlerJobEngineSettings.TC_max;
            

            return data;
        }

        public static PropertyCollection GetData(ICrawlJobContext state, ISpiderEvaluatorBase crawler)
        {
            PropertyCollection data = new PropertyCollection();
            data[nameComposerFields.crawlerClassName] = crawler.GetType().Name;
            data[nameComposerFields.crawlerTitleName] = crawler.name;
            data[nameComposerFields.crawlerFileFriendlyName] = crawler.name.getCleanFilepath().Replace("-", "");
            data[nameComposerFields.variablePLmax] = crawler.settings.limitTotalPageLoad;
            data[nameComposerFields.variableLT] = crawler.settings.limitIterationNewLinks;
            //data[nameComposerFields.variableTCmax] = state.crawlerJobEngineSettings.TC_max;
            data[nameComposerFields.sampleSize] = state.sampleList.Count();
            //data[nameComposerFields.sampleFileSource] = state.sampleFile;
            //data[nameComposerFields.sampleName] = state.sampleTags;
            
            return data;
        }

        public static String GetCrawlFolderName(ISpiderEvaluatorBase spider, crawlerDomainTaskMachineSettings crawlerJobEngineSettings , String templateString)
        {
            stringTemplate template = new stringTemplate(templateString);

            PropertyCollection data = GetData(crawlerJobEngineSettings, spider);

            return template.applyToContent(data);
        }


        public static String GetCrawlFolderName(ISpiderEvaluatorBase spider, ICrawlJobContext state, String templateString)
        {
            stringTemplate template = new stringTemplate(templateString);

            PropertyCollection data = GetData(state, spider);

            return template.applyToContent(data);
        }


    }
}
