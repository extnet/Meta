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
using System.Xml.Serialization;

using Ext.Net.Utilities;
using System.Text;
using System;
using System.Text.RegularExpressions;

namespace Ext.Net.Meta
{
    public class Method : Member
    {
        public Method() { }

        public Method(string line)
        {
            this.ParamsRaw = line.RightOf('(').LeftOfRightmostOf(')').Trim();

            string[] parts = line.Trim()
                                    .Replace("new public", "")
                                    .Replace("public ", "")
                                    .Replace("override ", "")
                                    .Replace("virtual ", "")
                                    .Replace("protected ", "")
                                    .LeftOf('(')
                                    .Trim()
                                    .Split(' ');

            this.Name = parts[1];
            this.Return = parts[0];

            if (line.Contains(" override "))
            {
                this.Modifier = Modifier.Override;
            }
        }

        [DefaultValue("")]
        [XmlAttribute("ParamsRaw")]
        public string ParamsRaw { get; set; }

        private ParametersList parameters;

        [XmlArray("Parameters")]
        [XmlArrayItem(ElementName = "Param", Type = typeof(Parameter))]
        public ParametersList Parameters
        {
            get
            {
                if (this.parameters == null)
                {
                    this.parameters = new ParametersList();
                }

                return this.parameters;
            }
            set
            {
                this.parameters = value;
            }
        }

        [XmlIgnore]
        public string ParamValues
        {
            get
            {
                if (this.ParamsRaw.IsNotEmpty())
                {

                    string p = Regex.Replace(this.ParamsRaw, @"\w+<.*>", "PLACEHOLDER");

                    string[] args = p.Split(',');

                    List<string> temp = new List<string>();

                    foreach (string s in args)
                    {
                        temp.Add(s.Trim().RightOfRightmostOf(" ").Trim());
                    }

                    return temp.Join(", ");
                }

                return "";
            }
        }

        [DefaultValue("void")]
        [XmlAttribute("Return")]
        public string Return { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("Hide")]
        public bool Hide { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("Private")]
        public bool Private { get; set; }
    }
}