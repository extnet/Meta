/**
 * @version: 2.0.0
 * @author: Ext.NET, Inc. http://www.ext.net/
 * @date: 2012-03-05
 * @copyright: Copyright (c) 2007-2012, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license: See license.txt and http://www.ext.net/license/. 
 * @website: http://www.ext.net/
 */

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using Ext.Net.Utilities;

namespace Ext.Net.Meta.Parser
{
    public class Processor
    {
        public string Input { get; set; }
        string Output { get; set; }
        string FileName { get; set; }
        string[] FileList { get; set; }
        public string[] IgnoreFiles { get; set; }
        public string[] IgnoreFolders { get; set; }
        public Root Root;

        public FileInfo OutputFile;

        public Processor(string input, string output, string fileName, string rootName, string[] files, string[] ignoreFiles, string[] ignoreFolders, Root root)
        {
            this.Input = Path.GetFullPath(input);
            this.Output = Path.GetFullPath(output);
            this.FileName = fileName;
            this.FileList = files;
            this.IgnoreFiles = ignoreFiles;
            this.IgnoreFolders = ignoreFolders;

            this.Root = root ?? new Root();
        }

        public void Process()
        {
            this.GetFiles();
            this.ProcessFiles();
            //this.Root.Sweep();

            this.Finish();
        }

        public void Finish()
        {
            var ps = Path.DirectorySeparatorChar;

            this.Root.Classes.Sort(delegate(Class x, Class y)
                                     {
                                         return x.Name.CompareTo(y.Name);
                                     });

            string path = this.Output.TrimEnd('\\') + ps + this.FileName + ".xml";

            // Xml
            XmlSerializer serializer = new XmlSerializer(this.Root.GetType());
            TextWriter writer = new StreamWriter(string.Concat(path));

            serializer.Serialize(writer, this.Root);
            writer.Close();

            this.OutputFile = new FileInfo(path);
        }


        /*  File Discovery
            -----------------------------------------------------------------------------------------------*/

        private List<FileInfo> files;

        public List<FileInfo> Files
        {
            get
            {
                if (this.files == null)
                {
                    this.files = new List<FileInfo>();
                }

                return this.files;
            }
        }

        public void GetFiles()
        {
            this.GetFiles(this.Input);
        }

        public void GetFiles(string src)
        {
            string[] data = Directory.GetFileSystemEntries(src);

            foreach (string el in data)
            {
                if (Directory.Exists(el))
                {
                    DirectoryInfo di = new DirectoryInfo(el);

                    if (!Utils.IsMatch(di.Name, this.IgnoreFolders))
                    {
                        this.GetFiles(el);
                    }
                }
                else
                {
                    FileInfo fi = new FileInfo(el);

                    if (Utils.IsMatch(fi.Name.ToLower(), this.FileList) && !Utils.IsMatch(fi.Name.ToLower(), this.IgnoreFiles) && !Utils.IsMatch(fi.Name, this.IgnoreFiles))
                    {
                        this.Files.Add(fi);
                    }
                }
            }
        }


        /*  Processing
            -----------------------------------------------------------------------------------------------*/

        public void ProcessFiles()
        {
            this.Files.Each(file => this.ProcessFile(file));
        }

        public void ProcessFile(FileInfo file)
        {
            string[] lines = File.ReadAllLines(file.FullName);

            Class cls = null;
            string line = "";
            bool isMetaOrConfigOption = false; 

            List<string> comments = new List<string>();
            List<MetaAttribute> attributes = new List<MetaAttribute>();

            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i].Trim();

                if (Utils.IsComment(line))
                {
                    comments.Add(line.RightOf("///").Trim());
                    continue;
                }

                if (Utils.IsAttribute(line))
                {
                    MetaAttribute attr = new MetaAttribute(line);
                    attributes.Add(attr);

                    if (attr.Type == "Meta" || attr.Type == "ConfigOption")
                    {
                        isMetaOrConfigOption = true;
                    }

                    continue;
                }

                if (Utils.IsClass(line))
                {
                    if (cls != null)
                    {
                        this.Root.Classes.Add(cls);
                    }

                    cls = new Class(line);

                    cls.RawComments = comments;
                    cls.Attributes = attributes;

                    comments = new List<string>();
                    attributes = new List<MetaAttribute>();

                    continue;
                }

                if (Utils.IsXType(line))
                {
                    var temp = lines[i + 4].Trim();

                    if (temp.StartsWith("return"))
                    {
                        cls.XType = temp.RightOf('"').Replace("\"", "").Replace(" ", "").Replace(";", "").Replace(":", ",").Trim();
                    }

                    continue;
                }

                if (Utils.IsInstanceOf(line))
                {
                    var temp = lines[i + 4].Trim();

                    if (temp.StartsWith("return"))
                    {
                        cls.InstanceOf = temp.RightOf('"').Replace("\"", "").Replace(" ", "").Replace(";", "").Replace(":", ",").Trim();
                    } 

                    continue;
                }

                if (cls != null && isMetaOrConfigOption)
                {
                    isMetaOrConfigOption = false;

                    if (Utils.IsProperty(line))
                    {
                        ConfigOption member = new ConfigOption(line);

                        member.RawComments = comments;
                        member.Attributes = attributes;

                        cls.ConfigOptions.Add(member);

                        comments = new List<string>();
                        attributes = new List<MetaAttribute>();

                        continue;
                    }

                    if (Utils.IsMethod(line))
                    {
                        Method member = new Method(line);

                        member.RawComments = comments;
                        member.Attributes = attributes;

                        cls.Methods.Add(member);

                        comments = new List<string>();
                        attributes = new List<MetaAttribute>();

                        continue;
                    }
                }

                isMetaOrConfigOption = false;
                comments = new List<string>();
                attributes = new List<MetaAttribute>();
            }

            if (cls != null && cls.Name.IsNotEmpty())
            {
                this.Root.Classes.Add(cls);
            }
        }
    }
}