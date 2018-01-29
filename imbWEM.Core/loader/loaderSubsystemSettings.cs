using System;
using System.Linq;
using System.Collections.Generic;
using imbACE.Core.cache;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.extensions.io;
using imbSCI.Data;
using System.ComponentModel;
using imbACE.Core.core;
using System.IO;
using imbSCI.Core.attributes;
using System.Threading;

namespace imbWEM.Core.loader
{

    public class loaderSubsystemSettings : aceSettingsStandaloneBase
    {
        public loaderSubsystemSettings()
        {

        }


        /// <summary>
        /// Does the politness wait in defined limits
        /// </summary>
        public void politeWait()
        {
            if (politeRequestModeOn)
            {
                Random rnd = new Random();

                Int32 wp = rnd.Next(politeRequestMin, politeRequestMax);
                Thread.Sleep(wp);
            }
        }


        /// <summary> Seconds to stop the web request for timeout </summary>
        [Category("Count")]
        [DisplayName("timeoutSeconds")]
        [Description("Seconds to stop the web request for timeout")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 timeoutSeconds { get; set; } = 60;




        public Int32 webRequestRetryCount { get; set; } = 5;


        [Description("webLoadRequest politness pre-wait minimum in miliseconds")]
        public Int32 politeRequestMin { get; set; } = 1500;

        public Int32 politeRequestMax { get; set; } = 6000;

        public Boolean politeRequestModeOn { get; set; } = true;


        /// <summary> Tick time miliseconds between checks </summary>
        [Category("Count")]
        [DisplayName("tickDelay")]
        [Description("Tick time miliseconds between checks")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 tickDelay { get; set; } = 100;



        /// <summary> Number of hours for cache record to last </summary>
        [Category("Count")]
        [DisplayName("cacheHoursLimit")]
        [Description("Number of hours for cache record to last")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 cacheHoursLimit { get; set; } = 720;



        /// <summary> If true it will allow loader to use cache for resolving loaderRequest </summary>
        [Category("Switch")]
        [DisplayName("doAllowUseCache")]
        [Description("If true it will allow loader to use cache for resolving loaderRequest")]
        public Boolean doAllowUseCache { get; set; } = true;



        /// <summary> If true it will allow loader to use web as source for resolving loaderRequest </summary>
        [Category("Switch")]
        [DisplayName("doAllowWebLoader")]
        [Description("If true it will allow loader to use web as source for resolving loaderRequest")]
        public Boolean doAllowWebLoader { get; set; } = true;

        public override string settings_filepath
        {
            get
            {
                return "config" + Path.DirectorySeparatorChar + "loaderSubsystemSettings.xml";
            }
        }
    }


}