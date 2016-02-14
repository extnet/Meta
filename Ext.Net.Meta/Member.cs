/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using Ext.Net.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ext.Net.Meta
{
    public interface IMemberAttribute
    {
        string Type { get; set; }
        string Value { get; set; }
    }

    public class MetaAttribute : IMemberAttribute
    {
        public MetaAttribute() { }

        public MetaAttribute(string line)
        {
            if (line.Contains("(") && line.Contains(")"))
            {
                this.Type = line.LeftOf("(").RightOf("[").Trim();

                string value = line.RightOf("(").LeftOfRightmostOf(")").Trim();

                if (this.Type.Equals("Description"))
                {
                    value = value.Chop();
                }

                this.Value = value;
            }
            else
            {
                this.Type = line.Trim().Chop();
            }
        }

        [DefaultValue("")]
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [DefaultValue("")]
        [XmlAttribute("RawValue")]
        public string Value { get; set; }
    }

    public abstract class MetaObject
    {
        [DefaultValue("")]
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [DefaultValue(Modifier.Virtual)]
        [XmlAttribute("Modifier")]
        public Modifier Modifier { get; set; }

        [DefaultValue(Access.Public)]
        [XmlAttribute("Access")]
        public Access Access { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("Abstract")]
        public bool Abstract { get; set; }

        [DefaultValue("")]
        [XmlAttribute("Template")]
        public string Template { get; set; }

        [XmlIgnore]
        public string Summary { get; set; }

        [JsonIgnore]
        [XmlElement("Summary", typeof(XmlCDataSection))]
        public XmlCDataSection SummaryCDATA
        {
            get
            {
                string temp = this.Summary;

                if (temp.IsEmpty())
                {
                    return null;
                }

                temp = Utils.RemoveNewLine(temp);

                XmlDocument doc = new XmlDocument();

                return doc.CreateCDataSection(temp);
            }
            set
            {
                this.Summary = value.Value;
            }
        }

        private List<string> rawComments = new List<string>();

        [XmlIgnore]
        public List<string> RawComments
        {
            get
            {
                return this.rawComments;
            }
            set
            {
                this.rawComments = value;
            }
        }

        [XmlIgnore]
        public List<MetaAttribute> members = new List<MetaAttribute>();

        [XmlArray("Attributes")]
        [XmlArrayItem(ElementName = "Attribute", Type = typeof(MetaAttribute))]
        public List<MetaAttribute> Attributes
        {
            get
            {
                return this.members;
            }
            set
            {
                this.members = value;
            }
        }

        public bool IsMeta()
        {
            return this.IsTypeOfAttribute("Meta");
        }

        public bool IsConfigOption()
        {
            return this.IsTypeOfAttribute("ConfigOption");
        }

        public bool IsTypeOfAttribute(string type)
        {
            return this.Attributes.Find(delegate(MetaAttribute attr) { return attr.Type.Equals(type); }) != null;
        }

        public void Compile()
        {
            this.Attributes.Each(attr =>
            {
                if (attr.Type == "Description")
                {
                    this.Summary = attr.Value;
                }

                if (this is ConfigOption)
                {
                    if (attr.Type == "DefaultValue")
                    {
                        ((ConfigOption)this).DefaultValue = attr.Value;
                    }
                }
            });
        }
    }

    public abstract class Member : MetaObject
    {
        [XmlIgnore]
        public Class Owner { get; set; }
    }

    public enum Modifier
    {
        Virtual,
        Override
    }

    public enum Access
    {
        Public,
        Protected,
        Internal,
        ProtectedInternal,
        Private
    }
}