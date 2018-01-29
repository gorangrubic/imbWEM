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
using imbCommonModels.pageAnalytics.core;
using System.Xml.Serialization;
using imbACE.Core.core;

namespace imbWEM.Core.loader
{



    /// <summary>
    /// Loader request, that contains retrieved content once loaded
    /// </summary>
    public class loaderRequest:IWebResult
    {


        public String url { get; set; } = "";

        public loaderRequest()
        {

        }

        public loaderRequest(String _url)
        {
            url = _url;
        }


        /// <summary>
        /// Used to resolve request using internet
        /// </summary>
        internal void LoadFromInternet()
        {

            Int32 ri = 0;

            while (!executed)
            {
                if (ri >= loaderSubsystem.settings.webRequestRetryCount)
                {
                    statusCode = HttpStatusCode.BadRequest;
                    executed = true;
                }
                else
                {

                    try
                    {

                        HtmlAgilityPack.HtmlWeb web = new HtmlWeb();

                        

                        HtmlDocument htmlDoc = new HtmlDocument();

                        
                        htmlDoc = web.Load(url);

                        if (statusCode == HttpStatusCode.RequestTimeout) return;

                        sourceCode = htmlDoc.DocumentNode.InnerHtml;

                        UTF8Encoding enc = new UTF8Encoding();

                        byteSize = enc.GetByteCount(sourceCode);
                        statusCode = web.StatusCode;
                        responseUrl = web.ResponseUri.AbsolutePath;
                        responseServer = web.ResponseUri.Host;
                        requestDuration = web.RequestDuration;

                        executed = true;

                    }
                    catch (Exception ex)
                    {
                        ri++;
                        aceLog.log("Load failed [" + ex.Message + "] " + url + " retry (" + ri + ")");
                    }
                }
            }

            executed = true;

        }

        public void releaseDocumentFromMemory()
        {
            
        }

        private HtmlDocument _htmlDocument;

        [XmlIgnore]
        public HtmlDocument HtmlDocument
        {
            get
            {
                if (_htmlDocument == null)
                {
                    _htmlDocument = new HtmlDocument();
                    if (!sourceCode.isNullOrEmpty()) _htmlDocument.LoadHtml(sourceCode);
                }
                
                return _htmlDocument;
            }
        }
    

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="loaderRequest"/> is executed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if executed; otherwise, <c>false</c>.
        /// </value>
        public Boolean executed { get; set; } = false;


        /// <summary>
        /// Source code of the response
        /// </summary>
        /// <value>
        /// The content of the response.
        /// </value>
        public String sourceCode { get; set; } = "";


        /// <summary>
        /// Server that responded
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public String responseServer { get;  set; } = "";

        /// <summary>
        /// Url that actually responded
        /// </summary>
        /// <value>
        /// The response URI.
        /// </value>
        public String responseUrl { get; set; } = "";

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode statusCode { get; set; } = HttpStatusCode.Created;

        /// <summary>
        /// Content byte count
        /// </summary>
        /// <value>
        /// The bytes.
        /// </value>
        public Int32 byteSize { get; set; } = 0;

        public String contentEncoding { get; set; } = "";

        public String contentCharSet { get; set; } = "";

        public String contentType { get; set; } = "";

        public Int32 requestDuration { get; set; } = 0;


    }

}