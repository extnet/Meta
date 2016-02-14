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

namespace Ext.Net.Meta
{
    public class Parameter : Member
    {
        [DefaultValue("")]
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("Optional")]
        public bool Optional { get; set; }
    }
}