/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.Xml.Serialization;
using Ext.Net.Utilities;

namespace Ext.Net.Meta
{
    [XmlRoot(ElementName = "Ext.Net", Namespace = "")]
    public class Root : Package
    {
        public Root() { }

        public Root(string name)
        {
            this.Name = name;
        }

        public Class FindClass(string path)
        {
            return this.FindClass(this, path);
        }

        public Class FindClass(Package root, string path)
        {
            return null;
        }

        public void AddClass(Class cls) { }
    }

    [XmlRoot(ElementName = "Ext.Net.UX", Namespace = "")]
    public class UXRoot : Root
    {
        public UXRoot() { }

        public UXRoot(string name) : base(name) { }
    }
}