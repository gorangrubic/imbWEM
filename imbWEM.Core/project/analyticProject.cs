// --------------------------------------------------------------------------------------------------------------------
// <copyright file="analyticProject.cs" company="imbVeles" >
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
    #region imbVELES USING

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
    using imbSCI.Data.interfaces;
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

    // // using imbAnalyticsEngine.webSiteComplexCrawler;
    // using imbScienceEngine.ai.modules;
    // using imbSemanticEngine.triplestore.core;

    #endregion




    /// <summary>
    /// ContextualModule [analyticProject] - 
    /// </summary>
    /// <remarks>
    /// Resurs koji koristi nove tehnike npr. nestedModules
    /// </remarks>
    /// [imb(imbAttributeName.diagnosticMode, "")] // AKTIVIRA dijagnostiku
    public class analyticProject : imbBindable, IObjectWithNameAndDescription
    {

        public string name { get; set; } = "analyticProject";
        public string description { get; set; } = "";

        public analyticProject()
        {

        }

        private int _devSampleLimit = 10; //= default(Int32); // = new Int32();
                                                      /// <summary>
                                                      /// Description of $property$
                                                      /// </summary>
        [Category("analyticProject")]
        [DisplayName("devSampleLimit")]
        [Description("Description of $property$")]
        public int devSampleLimit
        {
            get
            {
                return _devSampleLimit;
            }
            set
            {
                _devSampleLimit = value;
                OnPropertyChanged("devSampleLimit");
            }
        }


        //#region -----------  dictionaryBuilder  -------  [interni modul za izgradnju recnika]
        //private dictionaryLearningContextModule _dictionaryBuilder; // = new dictionaryLearningContextModule();
        //                            /// <summary>
        //                            /// interni modul za izgradnju recnika
        //                            /// </summary>
        //// [XmlIgnore]
        //[Category("analyticProject")]
        //[DisplayName("dictionaryBuilder")]
        //[Description("interni modul za izgradnju recnika")]
        //public dictionaryLearningContextModule dictionaryBuilder
        //{
        //    get
        //    {
        //        return _dictionaryBuilder;
        //    }
        //    set
        //    {
        //        // Boolean chg = (_dictionaryBuilder != value);
        //        _dictionaryBuilder = value;
        //        OnPropertyChanged("dictionaryBuilder");
        //        // if (chg) {}
        //    }
        //}
        //#endregion







        //#region -----------  nlpModule  -------  [Podesavanja NLP procesa]

        //private imbNLPModule _nlpModule = new imbNLPModule();

        ///// <summary>
        ///// Podesavanja NLP procesa
        ///// </summary>
        //// [XmlIgnore]
        //[Category("Project")]
        //[DisplayName("nlpModule")]
        //[Description("Podesavanja NLP procesa")]
        //public imbNLPModule nlpModule
        //{
        //    get { return _nlpModule; }
        //    set
        //    {
        //        _nlpModule = value;
        //        OnPropertyChanged("nlpModule");
        //    }
        //}

        //#endregion

/*
        private semanticDictionary _lexicon = new semanticDictionary();
        /// <summary> </summary>
        public semanticDictionary lexicon
        {
            get
            {
                return _lexicon;
            }
            protected set
            {
                _lexicon = value;
                OnPropertyChanged("lexicon");
            }
        }
        */

        //#region -----------  mainTripleStore  -------  [Glavni triplstor]

        //private triplestoreModule _mainTripleStore = new triplestoreModule();

        ///// <summary>
        ///// Glavni triplstor
        ///// </summary>
        //// [XmlIgnore]
        //[Category("analyticProject")]
        //[DisplayName("mainTripleStore")]
        //[Description("Glavni triplstor")]
        //public triplestoreModule mainTripleStore
        //{
        //    get { return _mainTripleStore; }
        //    set
        //    {
        //        // Boolean chg = (_mainTripleStore != value);
        //        _mainTripleStore = value;
        //        OnPropertyChanged("mainTripleStore");
        //        // if (chg) {}
        //    }
        //}

        //#endregion


      // #endregion

        //#region --- mainWebCrawler ------- Glavni web setac

        //private complexCrawlerModule _mainWebCrawler = new complexCrawlerModule("Main Web Crawler", "main_webcrawler",
        //                                                                        mainDBs.project);

        ///// <summary>
        ///// Glavni web setac
        ///// </summary>
        //public complexCrawlerModule mainWebCrawler
        //{
        //    get { return _mainWebCrawler; }
        //    set
        //    {
        //        _mainWebCrawler = value;
        //        OnPropertyChanged("mainWebCrawler");
        //    }
        //}

        //#endregion

        //#region -----------  aiProvider  -------  [Dostavljac AI jedinica]

        //private imbAiUnitProviderModule _aiProvider = new imbAiUnitProviderModule();
        //                                // = new aiUnitProviderModule("Main AI Provider", "main_aiprovider", mainDBs.project);

        ///// <summary>
        ///// Dostavljac AI jedinica
        ///// </summary>
        //// [XmlIgnore]
        //[Category("analyticProject")]
        //[DisplayName("aiProvider")]
        //[Description("Dostavljac AI jedinica")]
        
        //public imbAiUnitProviderModule aiProvider
        //{
        //    get { return _aiProvider; }
        //    set
        //    {
        //        // Boolean chg = (_aiProvider != value);
        //        _aiProvider = value;
        //        OnPropertyChanged("aiProvider");
        //        // if (chg) {}
        //    }
        //}

        //#endregion

        ///// <summary>
        ///// Initializes a new instance of the <see cref="analyticProject"/> class.
        ///// </summary>
        //public analyticProject():base("SemanticLab", "","")
        //{
        //    title = "SemanticLab Experiment";
        //    projectName = "SemanticLab";
        //   // test.versionCount = imbWEMManager.settings.testVersion;
        //  //  test.caption = imbWEMManager.settings.testPrefix.add(title, " "); //.ToUpper().toWidthMaximum(5, "-");

        //   // testLabeling.sampleBlockOrdinalNumber = imbWEMManager.settings.sampleTakeBlock;
        //    //test.description = __desc;

            
        //   // onAfterFinalDeploy += new imbProjectResourceEvent(projectModule_onAfterFinalDeploy);
        //}

        ///// <summary>
        ///// Projects the module on after load deploy.
        ///// </summary>
        ///// <param name="message">The message.</param>
        ///// <param name="sender">The sender.</param>
        //private void projectModule_onAfterLoadDeploy(string message, imbProjectResource sender)
        //{
            
        //}


        /// <summary>
        /// Izvrsava se automatski kada auto run workflow zavrsi
        /// </summary>
        public void onAutoRunWorkflowFinished()
        {
            //imbLanguageFramework.imbLanguageFrameworkManager.Prepare();
            //imbLanguageFramework.dictionary.dictionaryManager.prepare();
            //imbSemanticEngine.imbSemanticEngineManager.prepare();

            //lexicon.connect();


          //  dictionaryLearningContextModule mod = new dictionaryLearningContextModule(mainWebProfiler.sampleProcesReport.tokenizedCollection);
            //logSystem.logUniversal(imbConsoleLogInstruction.openToolEditor, mod, "Editor");

            //imbUniversalTool tool = imbUniversalToolEngine.openNewTool(mod, "MOD");
            //dictionaryLearningContextEditor editor = new dictionaryLearningContextEditor();
            //editor.controlTargetObject = mod;
            //imbUniversalToolEngine.openToolSynced(mod, "Mod",displayOption.defaultDisplay, false);
        }



       
        //public void onModuleEvent(imbModuleEventArgs args)
        //{
            
        //}

        //private void projectModule_onAfterFinalDeploy(string message, imbProjectResource sender)
        //{
            
            
        //}



        
    }
}