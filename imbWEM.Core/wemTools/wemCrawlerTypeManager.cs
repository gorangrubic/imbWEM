using imbACE.Core.plugins;
using imbSCI.Core.reporting;
using imbWEM.Core.crawler.evaluators;
using imbWEM.Core.crawler.rules.core;
using imbWEM.Core.plugins.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace imbWEM.Core.wemTools
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbACE.Core.plugins.internalPluginManager{imbWEM.Core.crawler.evaluators.ISpiderEvaluatorBase}" />
    public class wemCrawlerTypeManager : internalPluginManager<ISpiderEvaluatorBase>
    {
        protected override bool supportDirtyNaming {  get
            {
                return true;
            }
        }

        public void LoadTypes(ILogBuilder loger)
        {
            loadPlugins(loger);
        }

        public ISpiderEvaluatorBase GetInstance(String crawler_classname, ILogBuilder loger)
        {
            return GetPluginInstance(crawler_classname, "", loger, null);
        }
    }

}