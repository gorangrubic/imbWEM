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
using imbACE.Core;
using System.Threading;

namespace imbWEM.Core.loader
{

    /// <summary>
    /// Loader subsystem
    /// </summary>
    public static class loaderSubsystem
    {

        private static loaderSubsystemSettings _settings;
        /// <summary>
        /// Loader sub system
        /// </summary>
        public static loaderSubsystemSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new loaderSubsystemSettings();
                    _settings.Load();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }



        private static loaderCacheManager _cache;
        /// <summary>
        /// Loader cache system
        /// </summary>
        public static loaderCacheManager cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new loaderCacheManager(appManager.Application.folder_cache, settings.cacheHoursLimit);
                }
                return _cache;
            }
        }


        private const String LOADER_CACHE   = "Url {0,-30} loaded from cache    - {1,-15}";
        private const String LOADER_FROMWEB = "Url {0,-30} loaded from web      - {1,-15}";
        private const String LOADER_FAIL    = "Url {0,-30} failed to load       - {1,-15}";

        /// <summary>
        /// Executes the request - 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static loaderRequest ExecuteRequest(this loaderRequest request)
        {



            if (settings.doAllowUseCache)
            {
                var cRequest = cache.loadObjectCacheOrNew(request.url);
                

                if (cRequest.executed)
                {
                    if (imbWEMManager.settings.executionLog.doPageLoadedFromCache) imbWEMManager.log.log(String.Format(LOADER_CACHE, request.url, request.statusCode.ToString()));
                    return cRequest;
                }
            }

            if (settings.doAllowWebLoader)
            {
                // calls politeness waiting request before time measurement
                settings.politeWait();


                DateTime starter = DateTime.Now;

                Thread th = new Thread(request.LoadFromInternet);

                th.Start();

                while (!request.executed)
                {
                    Double sec = DateTime.Now.Subtract(starter).TotalSeconds;

                    Thread.Sleep(settings.tickDelay);

                    if (sec > settings.timeoutSeconds)
                    {
                        request.executed = true;
                        request.statusCode = HttpStatusCode.RequestTimeout;
                        imbWEMManager.log.log("Timeout for [" + request.url + "]");
                    }
                }
                
                if (request.statusCode == HttpStatusCode.OK)
                {
                    if (imbWEMManager.settings.executionLog.doPageLoadedLog) imbWEMManager.log.log(String.Format(LOADER_FROMWEB, request.url, request.statusCode.ToString()));
                } else
                {
                    if (imbWEMManager.settings.executionLog.doPageErrorOrDuplicateLog) imbWEMManager.log.log(String.Format(LOADER_FAIL, request.url, request.statusCode.ToString()));
                }
                
            }

            if (request.statusCode == HttpStatusCode.OK)
            {
                if (request.executed)
                {
                    if (settings.doAllowUseCache)
                    {
                        cache.saveObjectCache(request.url, request);
                    }
                }
            }

            return request; // <------------- [ prolazi i OK --------------- timout
        }


        



        /// <summary>
        /// Prepares the loader subsystem 
        /// </summary>
        public static void prepare()
        {
            settings = new loaderSubsystemSettings();

        }
    }

}