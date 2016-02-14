/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

using Ext.Net.Utilities;

namespace Ext.Net.Meta
{
    public class UniqueMemberList : List<Member>
    {
        new public void Add(Member member)
        {
            member.Compile();

            base.Add(member);
        }

        public void AddRange(List<Member> collection)
        {
            foreach (Member item in collection)
            {
                 if (this.FindIndex(delegate(Member mem) { return mem.Name.Equals(item.Name); }) == -1)
                {
                    this.Add(item);
                }
            }
        }

        new public void Sort()
        {
            base.Sort(delegate(Member o1, Member o2)
            {
                return (o1.Name.CompareTo(o2.Name));
            });
        }
    }

    public class ConfigOptionsList : UniqueMemberList { }
    public class MethodsList : UniqueMemberList { }
    public class ParametersList : UniqueMemberList { }

    public class UniqueList : List<ListItem>
    {
        public void Add(string name)
        {
            if (name.IsEmpty())
            {
                return;
            }

            if (this.FindIndex(delegate(ListItem item) { return item.Name.Equals(name); }) == -1)
            {
                base.Add(new ListItem(name));
            }
        }
    }

    public class ListItem
    {
        public ListItem() { }

        public ListItem(string name)
        {
            this.Name = name;
        }

        [DefaultValue("")]
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    public class ClassList : UniqueList { }
}