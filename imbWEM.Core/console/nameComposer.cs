using imbSCI.Core.extensions.io;
using imbSCI.Core.reporting.template;
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

        public static PropertyCollection GetData(analyticConsoleState state, ISpiderEvaluatorBase crawler)
        {
            PropertyCollection data = new PropertyCollection();
            data[nameComposerFields.crawlerClassName] = crawler.GetType().Name;
            data[nameComposerFields.crawlerTitleName] = crawler.name;
            data[nameComposerFields.crawlerFileFriendlyName] = crawler.name.getCleanFilepath().Replace("-", "");
            data[nameComposerFields.variablePLmax] = crawler.settings.limitTotalPageLoad;
            data[nameComposerFields.variableLT] = crawler.settings.limitIterationNewLinks;
            data[nameComposerFields.variableTCmax] = state.crawlerJobEngineSettings.TC_max;
            data[nameComposerFields.sampleSize] = state.sampleList.Count();
            data[nameComposerFields.sampleFileSource] = state.sampleFile;
            data[nameComposerFields.sampleName] = state.sampleTags;
            
            return data;
        }

        public static String GetCrawlFolderName(ISpiderEvaluatorBase spider, analyticConsoleState state, String templateString)
        {
            stringTemplate template = new stringTemplate(templateString);

            PropertyCollection data = GetData(state, spider);

            return template.applyToContent(data);
        }


    }
}
