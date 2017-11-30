// --------------------------------------------------------------------------------------------------------------------
// <copyright file="directReporterBase.cs" company="imbVeles" >
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
namespace imbWEM.Core.directReport.core
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
    using imbSCI.Core.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.unit;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.data.modelRecords;
    using imbSCI.DataComplex.extensions.data.formats;
    using imbSCI.DataComplex.extensions.text;
    using imbSCI.DataComplex.special;
    using imbWEM.Core.crawler.evaluators;
    using imbWEM.Core.crawler.model;
    using imbWEM.Core.crawler.modules.performance;
    using imbWEM.Core.crawler.rules.active;
    using imbWEM.Core.crawler.targets;
    using imbWEM.Core.directReport.enums;
    using imbWEM.Core.stage;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public abstract class directReporterBase:imbBindable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="directReporterBase"/> class.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        /// <param name="reportRootDir">The report root dir.</param>
        /// <param name="__parent">The parent.</param>
        /// <param name="__notation">The notation.</param>
        public directReporterBase(string reportName, string reportRootDir, aceAdvancedConsoleWorkspace __parent, aceAuthorNotation __notation)
        {
            name = reportName;
            rootPath = reportRootDir;
            reportPath = rootPath.add(reportName, "\\");

            parent = __parent;
            notation = __notation;
            deploy();
        }

        public directReporterBase(string reportName, folderNode _folder, aceAdvancedConsoleWorkspace __parent, aceAuthorNotation __notation)
        {
            name = reportName;

            rootPath = _folder.path.getPathVersion(1, "\\");// ; //reportRootDir;
            reportPath = _folder.path; // rootPath.add(reportName, "\\");

            parent = __parent;
            notation = __notation;
            deploy();
        }


        /// <summary>
        /// 
        /// </summary>
        public aceAuthorNotation notation { get; set; }

        public abstract void deployCustomFolders();

        protected void deploy()
        {
            DirectoryInfo di = Directory.CreateDirectory(reportPath);
            folder = new folderStructure(reportPath, name, "Direct reporter root folder");


            deployCustomFolders();
            folder.Add(DRFolderEnum.logs, "Logs", "Exported logs");
            folder.generateReadmeFiles(notation);


        }


        private string _name ;
        /// <summary> </summary>
        public string name
        {
            get
            {
                return _name;
            }
            protected set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }



        private string _reportPath ;
        /// <summary> </summary>
        public string reportPath
        {
            get
            {
                return _reportPath;
            }
            protected set
            {
                _reportPath = value;
                OnPropertyChanged("reportPath");
            }
        }


        private string _rootPath ;
        /// <summary> </summary>
        public string rootPath
        {
            get
            {
                return _rootPath;
            }
            protected set
            {
                _rootPath = value;
                OnPropertyChanged("rootPath");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public folderStructure folder { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public aceAdvancedConsoleWorkspace parent { get; protected set; }
    }

}