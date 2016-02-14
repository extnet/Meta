/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System;
using System.Text;
using Ext.Net.Utilities;

namespace Ext.Net.Meta.Parser
{
    public static class Utils
    {
        public const string START_META = "[Meta";

        public const string START_COMMENT = "/**";
        public const string END_COMMENT = "*/";
        public const string END_COMMENT2 = "**/";

        public static bool IsMatch(string value, string[] toMatch)
        {
            foreach (string match in toMatch)
            {
                if (match.StartsWith("*"))
                {
                    if (value.EndsWith(match.RightOfRightmostOf('*')))
                    {
                        return true;
                    }
                }
                else if (value.Equals(match))
                {
                    return true;
                }
            }

            return false;
        }


        /* Attributes and Comments */

        public static bool IsComment(string line)
        {
            return line.StartsWith("///");
        }

        public static bool IsAttribute(string line)
        {
            return line.StartsWith("[") && line.EndsWith("]");
        }

        public static bool IsMetaAttribute(string line)
        {
            return IsAttribute(line) && line.StartsWith("[Meta");
        }

        public static bool IsConfigOptionAttribute(string line)
        {
            return IsAttribute(line) && line.StartsWith("[ConfigOption");
        }

        public static bool IsDescriptionAttribute(string line)
        {
            return line.StartsWith("[Description");
        }

        public static bool IsDefalutValueAttribute(string line)
        {
            return line.StartsWith("[DefaultValue");
        }


        /* Class */

        public static bool IsClass(string line)
        {
            return IsPublic(line) && (line.StartsWith("class ") || line.Contains(" class "));
        }


        /* Members */

        public static bool IsMember(string line)
        {
            return !IsClass(line) && (IsPublic(line) || IsProtected(line));
        }

        public static bool IsProperty(string line)
        {
            return IsMember(line) && !IsClass(line) && !IsMethod(line);
        }

        public static bool IsMethod(string line)
        {
            return IsMember(line) && line.Contains("(") && line.Contains(")");
        }

        public static bool IsXType(string line)
        {
            return Utils.IsProperty(line) && line.EndsWith("string XType");
        }

        public static bool IsInstanceOf(string line)
        {
            return Utils.IsProperty(line) && line.EndsWith("string InstanceOf");
        }


        /* Access Modifiers */

        public static bool IsPublic(string line)
        {
            return line.StartsWith("public ") || line.StartsWith("new public");
        }

        public static bool IsProtected(string line)
        {
            return line.StartsWith("protected") || line.StartsWith("internal proctected");
        }

        public static bool IsAbstract(string line)
        {
            return IsClass(line) && line.Contains(" abstract ");
        }













        public static bool IsStartComment(string line)
        {
            return line.Trim().StartsWith(Utils.START_COMMENT);
        }

        public static bool IsEndComment(string line)
        {
            return (line.Trim().StartsWith(Utils.END_COMMENT) || line.Trim().StartsWith(Utils.END_COMMENT2));
        }

        public static bool IsPrivate(string line)
        {
            return (line.Contains("@ignore") || line.Contains("@hide") || line.Contains("@private"));
        }

        public static string RemoveNewLine(string text)
        {
            text = text.Trim();

            if (text.EndsWith(Environment.NewLine))
            {
                return text.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return text.Trim();
        }

        public static string CleanDescription(StringBuilder buffer)
        {
            return Utils.CleanDescription(buffer.ToString());
        }

        public static string CleanDescription(string text)
        {
            if (text.IsEmpty())
            {
                return "";
            }

            string desc = text.Trim().Replace("  ", " ");

            if (desc.Length > 15 && !desc.EndsWith("."))
            {
                desc = string.Concat(desc, ".");
            }

            return desc;
        }

        public static string GetTag(string text)
        {
            string temp = text.Trim();

            if (text.Trim().StartsWith("@"))
            {
                return temp.LeftOf(" ").Substring(1).Trim();
            }

            return "";
        }

        public static string GetValue(string text)
        {
            return text.RightOf(string.Concat("@", Utils.GetTag(text))).Trim();
        }

        public static string[] GetTypeValue(string text)
        {
            /**
             * @cfg {Number} x
             * @param {Object} config The config object
             * @param {Ext.Window} this
             * @param {Ext.direct.PollingProvider}
             * @param {Number} width The window's new width
             * @param {Object} config A config object containing the objects needed for the Store to access data,
             * @param name {String} Name of the Chart style value to change.
             * @param iPageX the current x position (optional, this just makes it so we
             * don't have to look it up again)
             * @param panel The {@link Ext.Panel} to proxy for
             * @param {String [Ext.data.Api.CREATE|READ|UPDATE|DESTROY]} action
             * @param {Record/Record{}} rs The records from Store
             * @param {Number} max (Optional) The maximum value to return.
             */

            string[] values = new string[] { "", "", "" };

            string temp = Utils.GetValue(text).Replace("}(Optional)", "} (optional)");

            if (temp.StartsWith("(Object)"))
            {
                temp = temp.Replace("(Object)", "{Object}");
            }

            temp = temp.Replace("(Optional)", "(optional)");

            string[] parts = temp.Split(' ');

            if (parts.Length > 0)
            {
                string type = "";

                temp = temp.Replace("{}", "##");

                if (parts[0].StartsWith("{"))
                {
                    type = temp.Substring(temp.IndexOf('{'), temp.IndexOf('}') + 1).Replace("##", "{}");
                    temp = temp.Substring(type.Length).Trim();
                    values[0] = type.Chop();
                }
                else if (parts.Length > 1 && parts[1].StartsWith("{"))
                {
                    type = temp.Substring(temp.IndexOf('{'), temp.IndexOf('}') + 1).Replace("##", "{}");
                    temp = temp.Replace(" " + type, "").Trim();
                    values[0] = type.Chop();
                }
            }

            parts = temp.Split(' ');

            string name = "";

            if (parts.Length > 1)
            {
                name = parts[0];
            }
            else if (!temp.IsEmpty())
            {
                name = temp;
            }

            if (name.IsLowerCamelCase() && !name.Equals("(optional)"))
            {
                temp = temp.Substring(name.Length).Trim();
                values[1] = name;
            }

            if (!temp.IsEmpty())
            {
                values[2] = temp;
            }

            if (values[1].Equals("this"))
            {
                values[1] = "el";
                values[2] = "this";
            }

            return values;
        }
    }
}
