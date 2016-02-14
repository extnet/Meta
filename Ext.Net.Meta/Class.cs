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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

using Ext.Net.Utilities;
using Newtonsoft.Json;

namespace Ext.Net.Meta
{
    public partial class Class : MetaObject
    {
        public Class() { }

        public Class(string line)
        {
            string pattern = @"class\s(\w*<\w*>|\w*)(?:\s*:\s*(\w*<\w*>|\w*))?";
            //string pattern = @"class\s(\w*<.*>|\w*)(?:\s*:\s*(\w*<.*>|\w*))?";

            Match match = Regex.Match(line, pattern);

            if (match.Groups.Count == 3)
            {
                this.Name = match.Groups[1].Value.Trim();

                this.Extends = match.Groups[2].Value.Trim();
            }
            else
            {
                this.Name = match.Groups[0].Value.RightOf("class").Trim();
            }

            string inherits = line.RightOf(this.Extends);

            if (inherits.IsNotEmpty())
            {
                string[] parts = inherits.Replace(" ", "").Split(',');

                for (int i = 1; i < parts.Length; i++)
                {
                    this.Inherits.Add(parts[i]);
                }
            }

            this.Abstract = line.Contains(" abstract ");
        }

        [XmlIgnore]
        public string FileName 
        {
            get
            {
                if (this.Name.Contains("<"))
                {
                    return this.Name.LeftOf("<");
                }

                return this.Name;
            }
        }

        [DefaultValue(false)]
        [XmlAttribute("Abstract")]
        public bool Abstract { get; set; }

        [JsonProperty("Extends")]
        [DefaultValue("")]
        [XmlAttribute("Extends")]
        public string Extends { get; set; }

        [JsonProperty("XType")]
        [DefaultValue("")]
        [XmlAttribute("XType")]
        public string XType { get; set; }

        [JsonProperty("InstanceOf")]
        [DefaultValue("")]
        [XmlAttribute("InstanceOf")]
        public string InstanceOf { get; set; }

        private List<string> inherits;

        [JsonIgnore]
        [XmlIgnore]
        public List<string> Inherits
        {
            get
            {
                if (this.inherits == null)
                {
                    this.inherits = new List<string>();
                }

                return this.inherits;
            }
        }

        [DefaultValue("")]
        [JsonProperty("Inherits")]
        [XmlAttribute("Inherits")]
        public string InheritsProxy
        {
            get
            {
                if (this.Inherits.Count > 0)
                {
                    return this.Inherits.Join(",");
                }

                return "";
            }
            set
            {
                this.Inherits.AddRange(value.Replace(" ", "").Split(','));
            }
        }

        private ConfigOptionsList configOptions;

        [XmlArray("ConfigOptions")]
        [XmlArrayItem(ElementName = "ConfigOption", Type = typeof(ConfigOption))]
        public ConfigOptionsList ConfigOptions
        {
            get
            {
                if (this.configOptions == null)
                {
                    this.configOptions = new ConfigOptionsList();
                }

                return this.configOptions;
            }
            set
            {
                this.configOptions = value;
            }
        }

        private MethodsList methods;

        [XmlArray("Methods")]
        [XmlArrayItem(ElementName = "Method", Type = typeof(Method))]
        public MethodsList Methods
        {
            get
            {
                if (this.methods == null)
                {
                    this.methods = new MethodsList();
                }

                return this.methods;
            }
            set
            {
                this.methods = value;
            }
        }
    }
}