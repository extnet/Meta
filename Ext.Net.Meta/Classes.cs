/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.Collections.Generic;
using Ext.Net.Utilities;
using System.Linq;

namespace Ext.Net.Meta
{
    public class Classes : List<Class>
    {
        public Classes() { }

        public Classes(Package parent)
        {
            this.parent = parent;
        }

        private Package parent;

        public Package Parent
        {
            get
            {
                return this.parent;
            }
        }

        new public void Add(Class cls)
        {
            cls.Compile();

            base.Add(cls);
        }

        public virtual bool Contains(string name)
        {
            return this.FindIndex(delegate(Class o) { return o.Name.Equals(name); }) > -1;
        }

        public virtual Class Find(string name)
        {
            return this.Find(delegate(Class cls)
            {
                return cls.Name.Equals(name);
            });
        }

        public virtual Class FindByXType(string xtype)
        {
            return this.Find(delegate(Class cls)
            {
                if (cls.XType.IsNotEmpty())
                {
                    if (cls.XType.Contains(','))
                    {
                        var xtypes = cls.XType.Split(',');

                        foreach (string x in xtypes)
                        {
                            if (x.Equals(xtype))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return cls.XType.Equals(xtype);
                    }
                }

                return false;
            });
        }

        public virtual Class FindByInstanceOf(string instanceOf)
        {
            return this.Find(delegate(Class cls)
            {
                if (cls.InstanceOf.IsNotEmpty())
                {
                    if (cls.InstanceOf.Contains(','))
                    {
                        var instanceofs = cls.InstanceOf.Split(',');

                        foreach (string x in instanceofs)
                        {
                            if (x.Equals(instanceOf))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return cls.XType.Equals(instanceOf);
                    }
                }

                return false;
            });
        }
    }
}