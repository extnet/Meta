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
using System.Text;

namespace Ext.Net.Meta.Factory
{
    public static class Helpers
    {
        private static string[] reservedWords = "ref,bool,object,string,int,checked,fixed,delegate,params".Split(',');

        public static bool IsReservedWord(this string text)
        {
            foreach(string key in Helpers.reservedWords)
            {
                if (key.Equals(text))
                {
                    return true;
                }
            }

            return false;
        }

        private static string[] baseTypes = "string,bool,int,object,DateTime,decimal,double,Unit,string[]".Split(',');

        public static bool IsBaseType(this string type)
        {
            foreach (string key in Helpers.baseTypes)
            {
                if (key.Equals(type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
