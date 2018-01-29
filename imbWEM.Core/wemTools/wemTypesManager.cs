using imbACE.Core.core;
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
    public class wemTypesManager
    {

        private static Object wemTypesLock = new Object();


        private static wemTypesManager _wemTypes = null;

        /// <summary>
        /// Global and general type provider for crawler types, crawl plugin types and other types of the imbWEM
        /// </summary>
        /// <value>
        /// The wem types.
        /// </value>
        public static wemTypesManager wemTypes
        {
            get
            {
                if (_wemTypes == null)
                {
                    lock (wemTypesLock)
                    {
                        if (_wemTypes == null)
                        {
                            _wemTypes = new wemTypesManager();
                            _wemTypes.loadPlugins();
                        }
                    }
                }

                return _wemTypes;
            }
        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        public void loadPlugins()
        {
            builderForLog logger = new builderForLog();
            imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(logger, "wemTypes");

            crawlerTypes = new wemCrawlerTypeManager();
            crawlerTypes.LoadTypes(logger);

            crawlPluginTypes = new wemCrawlerPluginTypeManager();
            crawlPluginTypes.LoadTypes(logger);

            imbSCI.Core.screenOutputControl.logToConsoleControl.removeFromOutput(logger);

        }

        /// <summary>
        /// Gets or sets the crawler types.
        /// </summary>
        /// <value>
        /// The crawler types.
        /// </value>
        public wemCrawlerTypeManager crawlerTypes { get; set; }

        /// <summary>
        /// Manager with crawl plugin types
        /// </summary>
        /// <value>
        /// The crawl plugin types.
        /// </value>
        public wemCrawlerPluginTypeManager crawlPluginTypes { get; set; }

        



    }
}
