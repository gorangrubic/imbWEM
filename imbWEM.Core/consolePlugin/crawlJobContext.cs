using System;
using System.Linq;
using System.Collections.Generic;
namespace imbWEM.Core.consolePlugin
{
using System.Text;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using imbACE.Core;
    using imbACE.Core.application;
    using imbACE.Core.plugins;
    using imbACE.Core.operations;
    using imbACE.Services.application;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.files.search;
    using imbWEM.Core.crawler.evaluators.modular;
    using imbWEM.Core.crawler.evaluators;
    using imbSCI.Core.extensions.typeworks;
    using imbWEM.Core.project;
    using imbWEM.Core.plugins.collections;
    using imbWEM.Core.stage;
    using imbWEM.Core.project.records;
    using imbWEM.Core.crawler.engine;
    using imbWEM.Core.crawler;
    using System.ComponentModel;
    using imbMiningContext;
    using imbACE.Core.core.exceptions;
    using imbSCI.Core.extensions.io;
    using imbWEM.Core.console;
    using imbWEM.Core.index.core;
    using System.Xml.Serialization;


    public class crawlJobContext:ICrawlJobContext
    {
        public crawlJobContext()
        {

        }
        public ISpiderEvaluatorBase crawler { get; set; }

        public analyticJob job { get; set; }


        public analyticJobRecord aRecord { get; set; }

        public pluginStackCollection pluginStack { get; set; } = new pluginStackCollection();

        public spiderStageControl stageControl { get; set; }

        public crawlerDomainTaskMachineSettings crawlerJobEngineSettings { get; set; }

        public String setupHash_crawler { get; set; }

        public String setupHash_global { get; set; }



        public spiderUnit unit { get; set; }

        public indexPerformanceEntry indexSession { get;  set; }

        [Category("analyticConsoleState")]
        [DisplayName("Sample")]
        [Description("Currently loaded list of domains to load")]
        [XmlIgnore]
        public webSiteSimpleSample sampleList { get; set; } = new webSiteSimpleSample();
    }

}