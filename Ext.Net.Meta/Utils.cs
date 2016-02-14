/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System;
using Ext.Net.Utilities;

namespace Ext.Net.Meta
{
    class Utils
    {
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

        public static string RemoveNewLine(string text)
        {
            if (text.EndsWith(Environment.NewLine))
            {
                return text.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return text;
        }

        public static string GetClassName(string text)
        {
            // Ext.Updater.defaults
            // Ext
            // Ext.Window
            // Ext.form.TextField

            string[] parts = text.Trim().Split('.');

            if (parts.Length == 1)
            {
                return text;
            }

            string name = "";

            for (int i = parts.Length - 1; i >= 1; i--) 
            {
                if (parts[i].IsLowerCamelCase())
                {
                    if (parts.Length == 3 && !parts[1].IsLowerCamelCase() && parts[2].IsLowerCamelCase())
                    {
                        // continue
                    }
                    else
                    {
                        break;
                    }
                }

                name = string.Concat(parts[i], ".", name);
            }

            return name.TrimEnd('.');
        }
    }
}
