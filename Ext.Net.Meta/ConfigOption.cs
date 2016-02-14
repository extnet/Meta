/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Ext.Net.Utilities;
using System;

namespace Ext.Net.Meta
{
    public class ConfigOption : Member
    {
        public ConfigOption() { }

        public ConfigOption(string line)
        {
            if (line.Contains(" override "))
            {
                this.Modifier = Modifier.Override;
            }

            if (line.Contains(" abstract "))
            {
                this.Abstract = true;
            }

            if (line.StartsWith("private "))
            {
                this.Access = Access.Private;
            }

            if (line.Contains("protected "))
            {
                this.Access = Access.Protected;
            }

            if (line.Contains("internal "))
            {
                this.Access = Access.Internal;
            }

            if (line.Contains("protected internal "))
            {
                this.Access = Access.ProtectedInternal;
            }

            line = Regex.Replace(line, ", +", ",");

            string[] parts = line
                                .Replace("private ", "")
                                .Replace("internal ", "")
                                .Replace("protected ", "")
                                .Replace("new public", "")
                                .Replace("abstract ", "")
                                .Replace("public ", "")
                                .Replace("virtual ", "")
                                .Replace("override ", "")
                                .Trim()
                                .Split(' ');

            if (parts.Length == 2)
            {
                this.Name = parts[1];
                this.Type = parts[0];
            }

            if (this.Name.IsEmpty())
            {
                throw new ArgumentException(line);
            }
        }

        [DefaultValue("")]
        [XmlAttribute("ClientName")]
        public string ClientName { get; set; }

        [DefaultValue("")]
        [XmlAttribute("Type")]
        public string Type { get; set; }

        private string privateValue = "";

        [XmlIgnore]
        public string PrivateValue
        {
            get
            {
                if (this.privateValue.IsEmpty())
                {
                    return this.DefaultValue;
                }

                return this.privateValue;
            }
            set
            {
                this.privateValue = value;
            }
        }

        private string defaultValue = null;

        [DefaultValue("")]
        [XmlAttribute("DefaultValue")]
        public string DefaultValue 
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                /*  Size
                    -----------------------------------------------------------------------------------------------*/

                if (value.Equals("typeof(Size), \"\""))
                {
                    this.PrivateValue = "Size.Empty";
                }


                /*  DateTime
                    -----------------------------------------------------------------------------------------------*/

                if (value.Equals("typeof(DateTime), \"9999-12-31\""))
                {
                    this.PrivateValue = "new DateTime(9999, 12, 31)";
                }

                if (value.Equals("typeof(DateTime), \"0001-01-01\""))
                {
                    this.PrivateValue = "new DateTime(0001, 01, 01)";
                }


                /*  TimeSpan
                    -----------------------------------------------------------------------------------------------*/

                if (value.Equals("typeof(TimeSpan), \"-9223372036854775808\""))
                {
                    this.PrivateValue = "new TimeSpan(-9223372036854775808)";
                }

                if (value.Equals("typeof(TimeSpan), \"9223372036854775807\""))
                {
                    this.PrivateValue = "new TimeSpan(9223372036854775807)";
                }


                /*  Unit
                    -----------------------------------------------------------------------------------------------*/

                if (value.StartsWith("typeof(Unit), \""))
                {
                    string temp = value.RightOf("typeof(Unit), \"").LeftOf("\"");

                    this.PrivateValue = "Unit." + (temp.IsEmpty() ? "Empty" : "Pixel(".ConcatWith(temp, ")"));
                }


                /*  Misc
                    -----------------------------------------------------------------------------------------------*/

                if (value == "\"-1 -1 -1 -1\"")
                {
                    this.PrivateValue = "new Margins(-1, -1, -1, -1)";
                }

                if (value == "1.0d")
                {
                    this.PrivateValue = "1M";
                }

                this.defaultValue = value;
            }
        }
    }
}