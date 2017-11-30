// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticJobExtensions.cs" company="imbVeles" >
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
namespace imbWEM.Core.project
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

    public static class analyticJobExtensions
    {

        /// <summary>
        /// Gets the samples from web site profile collection
        /// </summary>
        /// <param name="sciProject">The science project.</param>
        /// <param name="sampleTags">The sample tags.</param>
        /// <param name="sampleTakeLimit">The sample take limit.</param>
        /// <param name="usedStamp">The used stamp.</param>
        /// <returns></returns>
        //public static webSiteProfileSample getSamples(this analyticProject sciProject, IEnumerable<string> sampleTags, int sampleTakeLimit, string usedStamp="", int sampleBlockOrdinalNumber=0, bool random=false)
        //{
        //    List<imbCore.data.entity.relationship.IRelatedCollectionItem> allSites = sciProject.mainWebProfiler.webSiteProfiles.selectItemsAll();
        //    List<webSiteProfile> allInGroup = new List<webSiteProfile>();

        //    foreach (webSiteProfile sp in allSites)
        //    {
        //        if (sp.groupTags.ContainsAny(sampleTags))
        //        {
        //            allInGroup.Add(sp);
        //        }
        //    }
            
        //    if (random)
        //    {
        //        allInGroup.Randomize();
        //    }


        //    IEnumerable<webSiteProfile> wbp = new List<webSiteProfile>();
        //    int skipCount = 0;
        //    if (sampleBlockOrdinalNumber == 0) sampleBlockOrdinalNumber = 1;
        //    skipCount = (sampleBlockOrdinalNumber - 1) * sampleTakeLimit;
        //    int take = 0;
        //    if (allInGroup.Count() > skipCount)
        //    {
        //        wbp = allInGroup.Skip(skipCount);

        //        take = Math.Min(wbp.Count(), sampleTakeLimit);

        //        wbp = wbp.Take(take);
        //    }

        //    //var wbp = allInGroup.Skip()

        //    // = .Take(sampleTakeLimit);// wpGroups.selectGroup(wpGroups.primarySample, terminal, profiler.webSiteProfiles, runstamp, "", false);
        //    webSiteProfileSample sample = new webSiteProfileSample(wbp, sciProject.mainWebProfiler, sciProject.mainWebProfiler.webSiteProfiles);
        //    sample.usedGroups.AddRange(sampleTags);
        //    sample.usedStamp = usedStamp;
        //    sample.usedSettings = sciProject.mainWebProfiler.sampler;
        //    sample.logMessage = "Sample from groups [" + sampleTags.ToCSV(',') + "] block [" + sampleBlockOrdinalNumber + "] - start: " + skipCount + " - take: " + take;
        //    return sample;
        //}


        public static int getSampleTakeLimit(this analyticJobRunFlags aFlags)
        {
            int output = 0;

            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake2)) output += 2;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake5)) output += 5;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake10)) output += 10;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake15)) output += 15;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake25)) output += 25;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake40)) output += 40;
            if (aFlags.HasFlag(analyticJobRunFlags.sample_devTake100)) output += 100;


            return output;
        }

       

    }
}
