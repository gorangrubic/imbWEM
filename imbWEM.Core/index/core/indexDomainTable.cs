// --------------------------------------------------------------------------------------------------------------------
// <copyright file="indexDomainTable.cs" company="imbVeles" >
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
namespace imbWEM.Core.index.core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbCommonModels.webPage;
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
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbSCI.DataComplex.tables;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport;
    using imbWEM.Core.stage;

    public class indexDomainTable:objectTable<indexDomain>
    {

        public indexPerformanceEntry indexSessionEntry { get; set; }


        public indexDomainTable(string __filePath, bool __autoLoad) :base(__filePath, __autoLoad)
        {
            
            
        }

        /// <summary>
        /// Rechecks the values for each domain entry using information in the pageIndex table, calls save -- updates the domain table if any new discovered from the domain
        /// </summary>
        /// <param name="pages">The pages.</param>
        public void recheck(indexPageTable pages, ILogBuilder loger)
        {
            int i = 0;
            int c = 0;
            int imax = Count / 10;

            foreach (indexDomain iDomain in GetList())
            {
                var iPages = pages.GetPagesForDomain(iDomain.domain);
                iDomain.recheck(iPages);
                AddOrUpdate(iDomain, objectTableUpdatePolicy.overwrite);
                if (i > imax)
                {
                    i = 0;
                    loger.log("Domain index updated [" + ((double)c / (double)Count).ToString("P2") + "]");
                }
                i++;
                c++;
            }

            Save();
        }


        /// <summary>
        /// Performs domain index assertion
        /// </summary>
        /// <param name="domainList">The domain list.</param>
        /// <param name="completeRecheck">if set to <c>true</c> [complete recheck].</param>
        /// <returns></returns>
        public indexDomainAssertionResult GetDomainIndexAssertion(List<string> domainList = null, bool completeRecheck = false)
        {
            indexDomainAssertionResult output = new indexDomainAssertionResult();
            List<indexDomain> iList = GetList();

            if (domainList == null) domainList = new List<string>();
            if (!domainList.Any())
            {
                iList.ForEach(x => domainList.Add(x.domain));

                //domainList = GetDomainUrls(indexDomainContentEnum.indexed);
            }

            double IPs = 0;
            int Lemmas = 0;

            

            foreach (string domainUrl in domainList)
            {
                indexDomainContentEnum flags = indexDomainContentEnum.none;

                indexDomain idomain = GetDomain(domainUrl);
                if (idomain == null)
                {
                    output.Add(flags, domainUrl);
                    continue;
                } else
                {
                    flags = indexDomainContentEnum.indexed;

                    List<indexPage> pageList = imbWEMManager.index.pageIndexTable.GetPagesForDomain(domainUrl);
                    if (completeRecheck)
                    {
                        idomain.recheck(pageList);
                    }


                    List<string> pageUrls = new List<string>();
                    pageList.ForEach(x => pageUrls.Add(x.url));
                    indexURLAssertionResult pageListResult = imbWEMManager.index.pageIndexTable.GetUrlAssertion(pageUrls);
                    
                    if (pageListResult[indexPageEvaluationEntryState.inTheIndex].Count() == pageListResult[indexPageEvaluationEntryState.haveEvaluationEntry].Count())
                    {
                        flags |= indexDomainContentEnum.completeEvaluationPages;
                    } else
                    {
                        flags |= indexDomainContentEnum.uncompleteEvaluationPages;
                    }

                    FileInfo dlc_tf_idf = imbWEMManager.index.experimentManager.CurrentSession.GetTFIDF_DLC_File(idomain,getWritableFileMode.existing);

                    if (dlc_tf_idf.Exists)
                    {
                        flags |= indexDomainContentEnum.uncompleteDomainTFIDF;

                        idomain.TFIDFcompiled = false;

                    } else
                    {
                        flags |= indexDomainContentEnum.completeDomainTFIDF;
                        
                        idomain.TFIDFcompiled = true;
                    }

                    bool appUncomplete = false;

                    double IPd = 0;
                    foreach (indexPage p in pageList)
                    {
                        if ((p.Lemmas ==0) && (p.InfoPrize==0) && (p.DistinctLemmas.isNullOrEmpty())) {
                            appUncomplete = true;
                        }
                        //IPd += p.InfoPrize;
                    }
                    if (appUncomplete)
                    {
                        flags |= indexDomainContentEnum.uncompleteTFDFApplicationToPages;
                    } else
                    {
                        flags |= indexDomainContentEnum.completeTFDFApplicationToPages;
                    }

                   // idomain.InfoPrize = IPd;

                    output.Add(flags, domainUrl);

                   // AddOrUpdate(idomain);
                }

            }

            return output;
        }


        /// <summary>
        /// Gets the domains having specified contentType set
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public List<indexDomain> GetDomains(indexDomainContentEnum contentType)
        {
            List<indexDomain> output = new List<indexDomain>();


            foreach (indexDomain iDomain in this)
            {
                if (contentType == indexDomainContentEnum.any)
                {
                    output.Add(iDomain);
                }
                else
                {
                    if (iDomain.contentType == contentType)
                    {
                        output.Add(iDomain);
                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the domain.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public indexDomain GetDomain(string domainName)
        {
            return this[domainName];
        }

        public List<indexDomain> GetDomains(IEnumerable<string> domainName)
        {
            List<indexDomain> output = new List<indexDomain>();
            domainName.ToList().ForEach(x => output.Add(GetDomain(x)));

            return output;
        }


       


        /// <summary>
        /// Deploys the session.
        /// </summary>
        public void deploySession()
        {
            

        }


      

        ///// <summary>
        ///// Sets the site tf compiled.
        ///// </summary>
        ///// <param name="compiledTF">The compiled tf.</param>
        //public void SetSiteTFCompiled(termDocumentSet compiledTF, String domain)
        //{
        //    indexDomain idomain = GetDomain(domain);

            
        //    //objectSerialization.saveObjectToXML(compiledTF.GetAggregateDataTable(), GetTFFileName(idomain));

        //    compiledTF.AggregateDocument.name = idomain.HashCode;

        //    /*
        //    IWeightTable domainDocument = imbWEMManager.index.experimentManager.globalTFIDFSet.AddTable(idomain.HashCode);

        //    domainDocument.AddExternalDocument(compiledTF.AggregateDocument, true);
        //    */


        //    AddOrUpdate(domain);
        //}

        ///// <summary>
        ///// Gets the site tf for build.
        ///// </summary>
        ///// <param name="domainName">Name of the domain.</param>
        ///// <returns></returns>
        //public webSitePageTFSet GetSiteTFForBuild(String domainName)
        //{
        //    indexDomain idomain = GetDomain(domainName);
        //    String path = GetTFFileName(idomain);
        //    webSitePageTFSet output = new webSitePageTFSet(domainName, "TFIDF table set for this domain");
        //    return output; 
        //}

/*
        /// <summary>
        /// Provides feedback if the precompiled TF-IDF exists
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns></returns>
        public Boolean GetSiteLemmaTFExists(String domainName)
        {
            indexDomain idomain = GetDomain(domainName);
            


            //String file = tf_folder.findFile("*" + idomain.HashCode + "*.xml");
            idomain.TFIDFcompiled = !file.isNullOrEmpty();
            AddOrUpdate(idomain);

            return idomain.TFIDFcompiled;
        }
        */
        ///// <summary>
        ///// Gets the site lemma tf for use.
        ///// </summary>
        ///// <param name="domainName">Name of the domain.</param>
        ///// <returns></returns>
        //public weightTableCompiled GetSiteLemmaTFForUse(String domainName)
        //{
        //    indexDomain idomain = GetDomain(domainName);

        //    FileInfo file =  //tf_folder.findFile("*" + idomain.HashCode + "*.xml");

        //    Boolean found = !file.isNullOrEmpty();
        //    if (found)
        //    {
        //        weightTableCompiled tf_lemmaCompiled = new weightTableCompiled(file, true, domainName);

        //        return tf_lemmaCompiled;
        //    } else
        //    {
        //        //String path = GetTFFileName(idomain);

        //        //bSitePageTFSet output = new webSitePageTFSet(domainName, "");
        //        return null;
        //    }
        //}
        

        /// <summary>
        /// Gets the domain urls.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public List<string> GetDomainUrls(indexDomainContentEnum contentType)
        {
            List<indexDomain> output = new List<indexDomain>();
            List<string> urls = new List<string>();
            foreach (indexDomain iDomain in this)
            {
                if (iDomain.contentType == contentType)
                {
                    urls.Add(iDomain.url);
                }
            }
            return urls;
        }

    }

}