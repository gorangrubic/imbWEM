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

    public class wemCrawlerPluginTypeManager : internalPluginManager<IPlugInCommonBase>
    {
        protected override bool supportDirtyNaming
        {
            get
            {
                return true;
            }
        }

        public void LoadTypes(ILogBuilder loger)
        {

            try
            {
                loadPlugins(loger);
            } catch (TypeLoadException tex)
            {

            } catch (Exception ex)
            {

            }
        }

        public IPlugInCommonBase GetInstance(String plugin_classname, ILogBuilder loger)
        {
            return GetPluginInstance(plugin_classname, "", loger, null);
        }
    }

}