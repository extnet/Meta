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
using System.Collections.Generic;
using System.IO;

namespace Ext.Net.Meta.Parser
{
    class Program
    {
        static void Main(string[] arguments)
        {
            Dictionary<string, string> args = Program.ProcessArgs(arguments);

            string input = "";
            string output = "";

            foreach (KeyValuePair<string, string> arg in args)
            {
                switch (arg.Key)
                {
                    case "input":
                        input = arg.Value;
                        break;
                    case "output":
                        output = arg.Value;
                        break;
                }

            }

            var inputPath = new Uri(input);
            var outputPath = new Uri(output);


            //Uri nsroot = new Uri(new Uri(Environment.CurrentDirectory, UriKind.Absolute), @"..\..\");

            string root = "Ext.Net";
            string fileName = root.ToLower().ConcatWith(".meta");
            string[] files = "*.cs".Split(',');
            string[] ignoreFiles = "".Split(',');
            string[] ignoreFolders = "Build,Designers,Factory,Interfaci,Core,Enums,.svn,_svn,svn".Split(',');

            new Processor(inputPath.LocalPath, outputPath.LocalPath, fileName, root, files, ignoreFiles, ignoreFolders, null).Process();

            //input = new Uri(nsroot, @"Ext.Net.UX").LocalPath;
            //output = new Uri(nsroot, @"Ext.Net.UX\Factory").LocalPath;

            //root = "Ext.Net.UX";
            //fileName = root.ToLower().ConcatWith(".meta");

            //new Processor(input, output, fileName, root, files, ignoreFiles, ignoreFolders, new UXRoot()).Process();
        }

        static Dictionary<string, string> ProcessArgs(string[] arguments)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            string name = "";
            string value = "NOVALUE";

            for (int i = 0; i < arguments.Length; i++)
            {
                name = arguments[i].TrimStart('-');

                if (i + 1 < arguments.Length && !arguments[i + 1].StartsWith("-"))
                {
                    value = arguments[i + 1];
                    i++;
                }

                args.Add(name, value);
            }

            return args;
        }
    }
}