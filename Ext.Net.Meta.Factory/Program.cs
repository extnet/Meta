/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Ext.Net.Utilities;
using System;

namespace Ext.Net.Meta.Factory
{
    class Program
    {
        static int Main(string[] args)
        {
            Meta meta = new Meta { XMLFileName = "ext.net.meta.xml", RootType = typeof(Root) };

            Uri root = new Uri(new Uri(AppDomain.CurrentDomain.BaseDirectory, UriKind.Absolute), @"..\..\..\");
            Uri extnet = new Uri(root, @"Factory\");
            Uri factory = new Uri(root, @"Build\Meta\Factory\Templates\");

            meta.OutputRoot = extnet.LocalPath;
            meta.TemplateRoot = factory.LocalPath;

            if (!Directory.Exists(meta.OutputRoot))
            {
                Console.WriteLine("Invalid output path: " + meta.OutputRoot);
                return 1;
            }

            if (!Directory.Exists(meta.TemplateRoot))
            {
                Console.WriteLine("Invalid template path: " + meta.TemplateRoot);
                return 1;
            }

            try
            {
                Console.Write("Parsing meta information into Factory code: ");
                meta.Run();
                Console.WriteLine("done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("error.");
                Console.WriteLine("The program did not finish successfully.");
                Console.WriteLine("Exception details:");
                Console.WriteLine(e.ToString());
                return 1;
            }

            return 0;
        }
    }

    public class Meta
    {
        internal BuilderClass BuilderClass  { get; set; }
        internal ConfigClass ConfigClass    { get; set; }
        internal ConfigOptionsProperty ConfigOptionsProperty { get; set; }

        internal Root Root                  { get; set; }
        internal string TemplateRoot        { get; set; }
        internal string OutputRoot          { get; set; }
        internal string BuilderRoot         { get; set; }
        internal string ConfigRoot          { get; set; }
        internal string ConfigOptionsRoot   { get; set; }
        internal string XMLFileName         { get; set; }
        public   Type   RootType            { get; set; }

        public void Run()
        {
            this.BuilderRoot = this.OutputRoot.ConcatWith(@"Builder\");
            this.ConfigRoot = this.OutputRoot.ConcatWith(@"Config\");
            this.ConfigOptionsRoot = this.OutputRoot.ConcatWith(@"ConfigOptions\");

            this.BuilderClass = new BuilderClass(this.TemplateRoot.ConcatWith(@"Builder\"));
            this.ConfigClass = new ConfigClass(this.TemplateRoot.ConcatWith(@"Config\"));
            this.ConfigOptionsProperty = new ConfigOptionsProperty(this.TemplateRoot.ConcatWith(@"ConfigOptions\"));

            Directory.GetFiles(this.BuilderRoot).Each(file => File.Delete(file));
            Directory.GetFiles(this.ConfigRoot).Each(file => File.Delete(file));
            Directory.GetFiles(this.ConfigOptionsRoot).Each(file => File.Delete(file));

            this.GetMetaClassData();

            this.CreateBuilderClasses();
            this.CreateConfigClasses();
            this.CreateConfigOptionsProperties();
        }

        internal void GetMetaClassData()
        {
            XmlSerializer deserializer = new XmlSerializer(this.RootType);
            FileStream fs = new FileStream(this.OutputRoot.ConcatWith(this.XMLFileName), FileMode.Open);
            this.Root = (Root)deserializer.Deserialize(new XmlTextReader(fs));
            fs.Close();
        }

        internal void CreateBuilderClasses() 
        {
            this.Root.Classes.Each(cls => {
                if (cls.IsMeta())
                {
                    FileUtils.WriteFile(this.BuilderRoot.ConcatWith(cls.FileName, "Builder.cs"), this.BuilderClass.Compile(cls));
                }
            });
        }

        internal void CreateConfigClasses()
        {
            this.Root.Classes.Each(cls => {
                if (cls.IsMeta())
                {
                    FileUtils.WriteFile(this.ConfigRoot.ConcatWith(cls.FileName, "Config.cs"), this.ConfigClass.Compile(cls));
                }
            });
        }

        internal void CreateConfigOptionsProperties()
        {
            this.Root.Classes.Each(cls =>
            {
                if (cls.ConfigOptions.Count > 0)
                {
                    FileUtils.WriteFile(this.ConfigOptionsRoot.ConcatWith(cls.FileName, "ConfigOptions.cs"), this.ConfigOptionsProperty.Compile(cls));
                }
            });
        }
    }
}
