/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.ComponentModel;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ext.Net.Meta
{
    public class Package
    {
        [DefaultValue("")]
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Package Parent { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Package Root
        {
            get
            {
                if (this.Parent == null)
                {
                    return this;
                }

                return this.Parent.Root ?? this;
            }
        }

        private Classes classes;

        [XmlArray("Classes")]
        [XmlArrayItem("Class", typeof(Class))]
        public Classes Classes
        {
            get
            {
                if (this.classes == null)
                {
                    this.classes = new Classes(this);
                }

                return this.classes;
            }
            set
            {
                this.classes = value;
            }
        }

        public List<Class> GetAllClasses()
        {
            List<Class> classes = new List<Class>();

            return classes;
        }

        public List<Package> GetAllPackages()
        {
            List<Package> packages = new List<Package>();

            return packages;
        }
    }
}