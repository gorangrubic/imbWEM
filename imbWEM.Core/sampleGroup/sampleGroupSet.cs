// --------------------------------------------------------------------------------------------------------------------
// <copyright file="sampleGroupSet.cs" company="imbVeles" >
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

//using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

/// <summary>
/// Tools for automatic group association of arbritary <see cref="imbSqlEntityCollectionBase"/> sample group tagging
/// </summary>
namespace imbWEM.Core.sampleGroup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.core.exceptions;
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
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
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
    /// Set of sample group definitions
    /// </summary>
    public class sampleGroupSet : ObservableCollection<sampleGroupItem>
    {
        public int TotalCount
        {
            get
            {
                int c = 0;
                foreach (sampleGroupItem item in this)
                {
                    c = c + item.count;

                }
                return c;
            }
        }


        /// <summary>
        /// Number of items without any group association
        /// </summary>
        public int countNoGroup { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public int countHasGroup { get; set; } = 0;


        /// <summary>
        /// Describes the sample group to <c>output</c>
        /// </summary>
        /// <param name="output">The output object</param>
        public void describe(ILogBuilder output=null)
        {
            if (output == null) return;
            //output.log();
            int tl = output.tabLevel;
            output.rootTabLevel();

            //  output.AppendHeading("SampleGroup description", 1);

            output.open("desc", name, "");

          //  output.AppendHeading(name, 2);
           // output.AppendHorizontalLine();
            
            int ci = 1;
            foreach (sampleGroupItem item in this)
            {
                item.parent = this;
                output.open("group", item.groupTitle, item.groupDescription);
                //output.AppendHeading(, 3);


//                output.AppendPair("Description", );

                output.AppendPair("ID", ci);
                output.AppendPair("Tag", item.groupTag);
                if (item.groupSizeLimit == -1)
                {
                    output.AppendPair("Size (ratio)",item.weight + " / " + totalWeight.ToString());
                } else
                {
                    output.AppendPair("Size (limit)", item.groupSizeLimit);
                }
                
                output.AppendPair("Count", item.count);
                output.AppendPair("Border", item.groupBorder);
                if (item.isClosed)
                {
                    output.AppendLine("The group is closed for new members");
                } else
                {
                    output.AppendLine("The group may receive new members");
                }
                ci++;

                output.close();
            }
            
            output.AppendHorizontalLine();

            output.open("info", "Info", "");
            output.AppendPair("Counted collection", countedCollectionName);
            output.AppendPair("Items with group tag/s", countHasGroup);
            output.AppendPair("Items without group tag", countNoGroup);
            output.close();

            output.log("-- end");

            output.close();

            output.tabLevel = tl;
        }


       // public abstract List<IGroupTag> getGroupTags();


        /// <summary>
        /// Clears existing counts and searches the complete collection to get current counts
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <param name="dbCollection">The database collection.</param>
        public void setGroupCounts(IGroupTagCollection tags=null)
        {
            clearCounts();
           
            countHasGroup = 0;
            countNoGroup = 0;
            setGroupBorders();
            string name = "";

           // List<IGroupTag> items = dbCollection.selectItemsAll();
           if (tags == null)
            {


                aceLog.log("Failed to recount the sample data set because no group tags were specified");

            }
            else
            {
                foreach (IGroupTag item in tags)
                {
                    if (collectionExtensions.isNullOrEmpty(name)) name = item.GetType().Name;

                    // String groupTags =  //item.imbGetPropertySafe(db_groupTagFieldName, "").toStringSafe();
                    countTags(item.groupTags);
                }

            }

            countedCollectionName = name;
        }


        //public IRelationEnabledCollection getCollectionGroup(IRelationEnabledCollection collection, sampleGroupItem groupItem)
        //{
        //    IRelationEnabledCollection relationEnabled = collection.selectItems<>
        //}

        /// <summary>
        /// Sets the groups to collection according to policy and defined groups in this set. It will call <see cref="setGroupCounts(IRelationEnabledCollection)"/> if <see cref="isCountCalledYet"/> is <c>false</c>
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="policy">The policy.</param>
        public void setGroupsToCollection(IGroupTagCollection collection, sampleGroupAssignPolicy policy)
        {
            if (!isCountCalledYet) setGroupCounts(collection);

            List<IGroupTag> items = new List<IGroupTag>();
            sampleGroupItem group = pickRandomGroup();

            switch (policy)
            {
                case sampleGroupAssignPolicy.unassignAll:
                    items.AddRange(collection.Where<IGroupTag>(x => !x.groupTags.isNullOrEmpty())); // .selectItems(db_groupTagFieldName + " != '' AND " + db_groupTagFieldName + " IS NOT NULL", -1, selectItemsMode.sqlWhere, selectItemsResultType.managed);
                    break;
                case sampleGroupAssignPolicy.onlyNonAssigned:
                    items.AddRange(collection.Where<IGroupTag>(x => x.groupTags.isNullOrEmpty())); // = collection.selectItems(db_groupTagFieldName + " = '' OR " + db_groupTagFieldName + " IS NULL", -1, selectItemsMode.sqlWhere, selectItemsResultType.managed);
                    break;
                case sampleGroupAssignPolicy.overAssignAll:
                case sampleGroupAssignPolicy.reAssignAll:
                    items.AddRange((IEnumerable<IGroupTag>) collection);
                    break;
                default:
                    return;
                    break;
            }
            string tagline = "";

            foreach (IGroupTag item in items)
            {
                
                switch (policy)
                {
                    case sampleGroupAssignPolicy.overAssignAll:
                        group = pickRandomGroup();
                        tagline = item.imbGetPropertySafe(db_groupTagFieldName).toStringSafe();

                        if (!item.groupTags.Contains(group.groupTag)) item.groupTags = item.groupTags.add(group.groupTag,",");
                        //if (!tagline.Contains(group.groupTag)) tagline = tagline.add(group.groupTag, ",");
                        break;
                    case sampleGroupAssignPolicy.onlyNonAssigned:
                        group = pickRandomGroup();

                        item.groupTags = item.groupTags.add(group.groupTag, ",");
                        
                        group.count++;

                        break;

                    case sampleGroupAssignPolicy.reAssignAll:

                        group = pickRandomGroup();

                        item.groupTags = group.groupTag; //.groupTag, ",");
                        //tagline = item.imbGetPropertySafe(db_groupTagFieldName).toStringSafe();
                       // countTags(tagline, true);

                        //item.imbSetPropertySafe(db_groupTagFieldName, group.groupTag);
                        group.count++;

                        break;
                    case sampleGroupAssignPolicy.unassignAll:
                        item.groupTags = "";
                        break;
                }

               // item.updateRowAll(true);

                //item.setModified();

                //item.saveItem(true);
            }


          //  collection.saveItems(true, true);
        }

        /// <summary>
        /// Picks the random group accorting to <see cref="sampleGroupItem.weight"/> for groups that have <see cref="sampleGroupItem.isClosed"/> FALSE
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Failed to pick random groups after [" + Count + "] retries!</exception>
        public sampleGroupItem pickRandomGroup() 
        {
            int retry = Count;

            sampleGroupItem group = getGroup(rnd.Next(totalWeight));

            while (retry > 0)
            {
                if (group.isClosed)
                {
                    group = getGroup(rnd.Next(totalWeight));
                } else
                {
                    break;
                }
                retry--;
            }
            if (group.isClosed)
            {
                throw new aceGeneralException("Failed to pick random groups after [" + Count + "] retries!");
            } else
            {
                return group;
            }
            return null;
        }

        /// <summary>
        /// Vraca grupu iz seta grupa na osnovu datog randomResult broja koji ne sme biti veci od zbira weight parametara svake grupe u setu
        /// </summary>
        /// <param name="groups">Set grupa koji se dodeljuje uzorku</param>
        /// <param name="randomResult">Broj kojim se odredjuje grupa</param>
        /// <returns></returns>
        protected sampleGroupItem getGroup(int randomResult)
        {
            sampleGroupItem groupOut = null;
            foreach (sampleGroupItem group in this)
            {
                groupOut = group;
                if (group.groupBorder > randomResult)
                {
                    return group;
                }
            }
            return groupOut;

        }

        /// <summary>
        /// Total weight of the groups
        /// </summary>
        public int totalWeight { get; set; }


        /// <summary>
        /// Indicating whether <see cref="setGroupCounts(IRelationEnabledCollection)"/> called since construction of this instance
        /// </summary>
        /// <value>
        /// <c>true</c> if it seems a collection was already counted; otherwise, <c>false</c>.
        /// </value>
        public bool isCountCalledYet
        {
            get
            {
                if ((countNoGroup == 0) && (countHasGroup == 0)) return false;
                return true;
            }
        }

        protected Random rnd;

        /// <summary>
        /// Prepares groups for random sample association
        /// </summary>
        protected void setGroupBorders()
        {
            totalWeight = 0;
            foreach (sampleGroupItem group in this)
            {
                totalWeight = totalWeight + group.weight;
                group._groupBorder = totalWeight;
            }
            rnd = new Random(totalWeight);
        }


        /// <summary>
        /// Descriptive name of sample group set
        /// </summary>
        public string name { get; set; } = "Sample grouping definition";


        /// <summary>
        /// Resets all <see cref="sampleGroupItem.count"/> values to <c>0</c>
        /// </summary>
        public void clearCounts()
        {
            countNoGroup = 0;
            countHasGroup = 0;
            foreach (sampleGroupItem sgi in this)
            {
                sgi.count = 0;
            }
            countedCollectionName = "unknown";
        }


        /// <summary>
        /// 
        /// </summary>
        protected string countedCollectionName { get; set; } = "unknown";


        /// <summary>
        /// Increases count for each <see cref="sampleGroupItem"/> witch has tag found in <c>tagLine</c>
        /// </summary>
        /// <param name="tagline">The tagline from database or <see cref="imbSqlEntityCollectionBase"/></param>
        protected void countTags(string tagline, bool doReduction=false)
        {
            if (!tagline.isNullOrEmptyString())
            {
                bool hasGroup = false;
                foreach (sampleGroupItem sgi in this)
                {
                    
                    if (tagline.Contains(sgi.groupTag))
                    {
                        if (doReduction)
                        {
                            if (sgi.count > 0) sgi.count--;
                        } else
                        {
                            sgi.count++;
                        }
                        
                        hasGroup = true;
                    }
                }
                if (hasGroup)
                {
                    countHasGroup++;
                } else
                {
                    countNoGroup++;
                }
            } else
            {
                countNoGroup++;
            }
        }
        
        #region -----------  db_groupTagFieldName  -------  [Naziv polja u tabeli baze podataka koje sadrzi podatak o pripadnosti grupi]

        /// <summary>
        /// Database table field containing group tags
        /// </summary>
        /// <value>
        /// The name of the database group tag field.
        /// </value>
        // [XmlIgnore]
        [Category("sampleGroupSet")]
        [DisplayName("Tag Field Name")]
        [Description("Naziv polja u tabeli baze podataka koje sadrzi podatak o pripadnosti grupi")]
        public string db_groupTagFieldName { get; protected set; } = "groupTags";

        #endregion


    }
}
