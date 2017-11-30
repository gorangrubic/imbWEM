// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sampleGroupEngine.cs" company="imbVeles" >
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
namespace imbWEM.Core.sampleGroup
{
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

    /// <summary>
    /// Engine for group operations
    /// </summary>
    public static class sampleGroupEngine
    {

        ///// <summary>
        ///// Vrsi selektovanje profila na osnovu podesavanja i SampleTaker algoritma
        ///// </summary>
        ///// <param name="runStamp"></param>
        ///// <param name="groups"></param>
        ///// <param name="sampler"></param>
        ///// <param name="customWhere"></param>
        ///// <returns></returns>
        //public List<webSiteProfile> selectProfiles(this IGrou String runStamp = null, List<String> groups = null,
        //                                           sampleSettings sampler = null, String customWhere = "", Boolean onlyCurrentTest = false)
        ////where T : IEnumerable<webSiteProfile>, new()
        //{
        //    List<webSiteProfile> output = new List<webSiteProfile>();
        //    String wSql = makeSqlWhere(runStamp, groups, customWhere, onlyCurrentTest);

        //    // source
        //    //IRelationEnabledCollection rl = this as IRelationEnabledCollection;

        //    output = this.selectItems<webSiteProfile>(wSql, -1, selectItemsMode.sqlWhere, selectItemsResultType.managed);


        //    return output;
        //}

        ///// <summary>
        ///// 2017c: Selects the group.
        ///// </summary>
        ///// <param name="groupSet">The group set.</param>
        ///// <param name="group">The group.</param>
        ///// <param name="loger">The loger.</param>
        ///// <param name="source">The source.</param>
        ///// <param name="runStamp">The run stamp.</param>
        ///// <param name="customWhere">The custom where.</param>
        ///// <param name="onlyCurrentTest">if set to <c>true</c> [only current test].</param>
        ///// <returns></returns>
        //public static IEnumerable<webSiteProfile> selectGroup(this sampleGroupSet groupSet, sampleGroupItem group, ILogBuilder loger,webSiteProfileCollection source, String runStamp = null, String customWhere = "", Boolean onlyCurrentTest = false)
        //{
        //    //List<IReal> wbp = new List<IProfile>();


        //    //String whereQ = groupSet.makeSqlWhere(group, loger, runStamp, customWhere, onlyCurrentTest);

            

        //     ////.selectItems(whereQ); // webSiteProfiles.selectItems<webSiteProfile>(sql, -1, selectItemsMode.sqlWhere,  selectItemsResultType.managed);



            

        //   // return wbp;
        //}



        /// <summary>
        /// Pravi WHERE dodatak SQL upita - 
        /// </summary>
        /// <param name="runStamp">Ako nije definisan napravice trenuitni</param>
        /// <param name="groups">Grupe - ako nije definisan ucitace sve</param>
        /// <param name="customWhere">Proizvoljan WHERE upit</param>
        /// <param name="onlyCurrentTest">Da li proverava RunStamp</param>
        /// <returns></returns>
        public static string makeSqlWhere(this sampleGroupSet groupSet, sampleGroupItem group, ILogBuilder loger, string runStamp = null, string customWhere = "", bool onlyCurrentTest = false)
        {
            loger.log("Creating sample filter query for: " + group.groupTitle + " (" + group.groupTag + ")");



            string output = "";
            string whereRunStamp = "";
            string whereGroups = "";


            if (onlyCurrentTest)
            {
                if (string.IsNullOrEmpty(runStamp))
                {
                   
                }
                whereRunStamp = "((lastRunStamp <> \"" + runStamp + "\") OR (lastRunStamp IS NULL))";
            }

            
                
            whereGroups = groupSet.db_groupTagFieldName + " LIKE '%" + group.groupTag + "%'";
                
            

            if (!string.IsNullOrEmpty(whereRunStamp)) output += whereRunStamp + "";

            if (!string.IsNullOrEmpty(whereGroups))
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += " OR " + whereGroups;
                }
                else
                {
                    output = whereGroups;
                }
            }

            if (!string.IsNullOrEmpty(customWhere))
            {
                if (!string.IsNullOrEmpty(output))
                {
                    output += " OR " + customWhere;
                }
                else
                {
                    output = customWhere;
                }
            }

            loger.log("Query to filter from " + group.count + " entries: " + output);


            return output;
        }


        /// <summary>
        /// Automatic group creation - using default settings
        /// </summary>
        /// <param name="numberOfGroups">Number of groups to autocreate</param>
        /// <returns></returns>
        public static sampleGroupSet createGroupSet(int numberOfGroups)
        {
            sampleGroupSet output = new sampleGroupSet();

            //Int32 i = 1;

            for (var a=0;a<numberOfGroups;a++)
            {
                sampleGroupItem group = new sampleGroupItem();
                group.groupTitle = "Group " + a.ToString();
                group.groupTag = "G" + a.ToString();
                group.weight = 1;
                output.Add(group);
                
            }

            return output;
        }


        ///// <summary>
        ///// Brise sve group tagove iz objekata
        ///// </summary>
        ///// <param name="groups"></param>
        ///// <param name="dbCollection"></param>
        //public static void unassignGroups(this sampleGroupSet groups, imbSqlEntityCollectionBase dbCollection)
        //{
        //    List<IRelatedCollectionItem> items; // = dbCollection.selectItems(groups.db_groupTagFieldName + " = '' OR " + groups.db_groupTagFieldName + " IS NULL", -1, imbCore.data.entity.enums.selectItemsMode.sqlWhere, imbCore.data.entity.enums.selectItemsResultType.managed);

        //    items = dbCollection.selectItemsAll();

        //    List<DataRow> rows = new List<DataRow>();
        //    rows = dbCollection.selectRowsAll(-1, "", "");

        //    foreach (IRelatedCollectionItem item in items)

        //    //   foreach (DataRow row in rows)
        //    {
        //       // var group = groups.getGroup(rnd.Next(totalWeight));
        //        item.imbSetPropertySafe(groups.db_groupTagFieldName, "");

        //        item.saveItem();
        //        //row.AcceptChanges();

        //    }


        //}

       

        //public static void checkIfGroupsAssigned(this sampleGroupSet groups, imbSqlEntityCollectionBase dbCollection)
        //{
            
        //}

       

        ///// <summary>
        ///// Automatically applies <see cref="sampleGroupSet"/> onto specified collection. Ratio between group assigments is defined by <see cref="sampleGroupItem.weight"/> property of each <see cref="sampleGroupItem"/>
        ///// </summary>
        ///// <param name="groups">Definition of groups</param>
        ///// <param name="dbCollection">Kolekcija za koju dodeljuje grupu</param>
        ///// <param name="distribution">Tip distribucije grupa</param>
        ///// <param name="onlyWithoutGroupTags">Ako je TRUE onda ce dodeliti grupe samo onim unosima koji imaju groupTags NULL</param>
        //public static void assignToGroups(this sampleGroupSet groups, imbSqlEntityCollectionBase dbCollection, sampleGroupDistribution distribution, Boolean onlyWithoutGroupTags)
        //{
        //    if (groups.Count() == 0)
        //    {
        //        logSystem.log("Group set is empty - can't assign group to: " + dbCollection.tableName + " (db table)", logType.CriticalWarning);
        //        return;
        //    }

        //    Int32 totalWeight = 0;
        //    foreach (sampleGroupItem group in groups)
        //    {
        //        totalWeight = totalWeight + group.weight;
        //        if (group.groupSizeLimit == -1)
        //        {
        //            group._groupBorder = totalWeight;
        //            //group._groupBorder = group.groupSizeLimit;
        //        }
        //        else
        //        {
        //            group._groupBorder = totalWeight;
        //        }
        //    }
        //    //List<DataRow> rows = new List<DataRow>();

        //    List<IRelatedCollectionItem> items; // = dbCollection.selectItems(groups.db_groupTagFieldName + " = '' OR " + groups.db_groupTagFieldName + " IS NULL", -1, imbCore.data.entity.enums.selectItemsMode.sqlWhere, imbCore.data.entity.enums.selectItemsResultType.managed);

        //    if (onlyWithoutGroupTags)
        //    {
        //      //  rows = dbCollection.selectRows(groups.db_groupTagFieldName + " = '' OR " + groups.db_groupTagFieldName + " IS NULL", -1, imbCore.data.entity.enums.selectItemsMode.sqlWhere, imbCore.data.entity.enums.selectItemsResultType.managed);
        //        items = dbCollection.selectItems(groups.db_groupTagFieldName + " = '' OR " + groups.db_groupTagFieldName + " IS NULL", -1, imbCore.data.entity.enums.selectItemsMode.sqlWhere, imbCore.data.entity.enums.selectItemsResultType.managed);

        //    } else
        //    {
        //        //rows = dbCollection.selectRowsAll(-1, "", "");
        //        items = dbCollection.selectItemsAll();
        //    }


        //    Int32 groupId = 0;
        //    Random rnd = new Random(totalWeight);
        //    foreach (IRelatedCollectionItem item in items)

        //     //   foreach (DataRow row in rows)
        //    {
        //        var group = groups.getGroup(rnd.Next(totalWeight));


        //        item.imbSetPropertySafe(groups.db_groupTagFieldName, group.groupTag);

        //        item.saveItem();
        //        //row.AcceptChanges();

        //    }
            
        //    //dbCollection.saveItems()
        //}

    }
}
