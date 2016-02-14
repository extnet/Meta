/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System;
using System.Collections.Generic;
using Ext.Net.Utilities;

namespace Ext.Net.Meta
{
    public partial class Class
    {
        public Class SortAll()
        {
            this.ConfigOptions.Sort();
            this.Methods.Sort();

            return this;
        }

        public Class Merge(Class cls)
        {
            this.Summary = string.Concat(this.Summary, Environment.NewLine, cls.Summary);

            if (this.Extends == null && cls.Extends != null)
            {
                this.Extends = cls.Extends;
            }

            this.ConfigOptions.AddRange(cls.ConfigOptions);
            this.Methods.AddRange(cls.Methods);

            return this;
        }
    }
}