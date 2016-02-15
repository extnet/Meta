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
using System.IO;
using System.Text;

namespace Ext.Net.Meta.Factory
{
    public interface IFactory
    {
        string GetTemplate(string type);
        string TemplateRoot { get; set; }
    }

    public abstract class MetaFactory : IFactory
    {
        public MetaFactory(string templateRoot)
        {
            this.TemplateRoot = templateRoot;

            this.MethodFactory = new MethodFactory(templateRoot.ConcatWith(@"Methods\"));
            this.ConfigOptionFactory = new ConfigOptionFactory(templateRoot.ConcatWith(@"ConfigOptions\"));
        }

        public virtual string TemplateRoot { get; set; }

        public MethodFactory MethodFactory { get; set; }
        public ConfigOptionFactory ConfigOptionFactory { get; set; }

        public virtual string GetTemplate(string type)
        {
            string path = this.TemplateRoot.ConcatWith(type, ".txt");

            if (File.Exists(path))
            {
                return FileUtils.ReadFile(path);
            }

            return this.GetTemplate("AbstractClass");
        }

        protected virtual string CompileConfigOptions(Class cls)
        {
            StringBuilder configOptions = new StringBuilder(256);

            foreach (ConfigOption member in cls.ConfigOptions)
            {
                if (member.IsMeta())
                {
                    member.Owner = cls;
                    configOptions.Append(this.ConfigOptionFactory.Compile(member));
                }
            }

            return configOptions.ToString();
        }

        protected virtual string CompileMethods(Class cls)
        {
            StringBuilder methods = new StringBuilder(256);

            foreach (Method member in cls.Methods)
            {
                member.Owner = cls;
                methods.Append(this.MethodFactory.Compile(member));
            }

            return methods.ToString();
        }
    }

    public class BuilderClass : MetaFactory
    {
        public BuilderClass(string templateRoot) : base(templateRoot) { }

        public virtual string Compile(Class cls)
        {
            var obj = new
            {
                Name = cls.Name,
                FileName = cls.FileName,
                PrivateName = cls.Name.ToLowerCamelCase(),
                Extends = cls.Extends,
                ConfigOptions = this.CompileConfigOptions(cls),
                Methods = this.CompileMethods(cls)
            };

            string tpl = cls.Abstract ? "AbstractClass" : "ConcreteClass";

            return this.GetTemplate(tpl).FormatWith(obj);
        }

        protected override string CompileConfigOptions(Class cls)
        {
            StringBuilder buffer = new StringBuilder(256);

            foreach (ConfigOption member in cls.ConfigOptions)
            {
                if (member.IsMeta())
                {
                    string privateName = member.Name.ToLowerCamelCase();

                    var obj = new
                    {
                        Name = member.Name,
                        PrivateName = privateName.IsReservedWord() ? "_" + privateName : privateName,
                        Type = member.Type,
                        Return = "TBuilder", // cls.Abstract ? "TBuilder" : cls.Name.ConcatWith(".Builder"),
                        DefaultValue = member.DefaultValue ?? "null",
                        PrivateValue = member.PrivateValue,
                        Summary = member.Summary,
                        Modifier = member.Modifier.ToString().ToLower()
                    };

                    string template = member.DefaultValue != null ? member.Type : "instance";

                    if (member.Template.IsNotEmpty())
                    {
                        template = member.Template;
                    }

                    member.Owner = cls;
                    buffer.Append(this.ConfigOptionFactory.Compile(obj, template));
                }
            }

            return buffer.ToString();
        }

        protected override string CompileMethods(Class cls)
        {
            StringBuilder buffer = new StringBuilder(256);

            foreach (Method member in cls.Methods)
            {
                if (member.IsMeta())
                {
                    string privateName = member.Name.ToLowerCamelCase();

                    var obj = new
                    {
                        Name = member.Name,
                        PrivateName = privateName.IsReservedWord() ? "_" + privateName : privateName,
                        Return = "TBuilder", // cls.Abstract ? "TBuilder" : cls.Name.ConcatWith(".Builder"),
                        Summary = member.Summary,
                        Modifier = member.Modifier.ToString().ToLower(),
                        ParamsRaw = member.ParamsRaw,
                        ParamValues = member.ParamValues
                    };

                    string template = cls.Abstract ? "method" : "method.concrete";

                    if (member.Template.IsNotEmpty())
                    {
                        template = member.Template;
                    }

                    member.Owner = cls;
                    buffer.Append(this.MethodFactory.Compile(obj, template));
                }
            }

            return buffer.ToString();
        }
    }

    public class ConfigClass : MetaFactory
    {
        public ConfigClass(string templateRoot) : base(templateRoot) { }

        public virtual string Compile(Class cls)
        {
            var obj = new
            {
                Name = cls.Name,
                FileName = cls.FileName,
                PrivateName = cls.Name.ToLowerCamelCase(),
                Extends = cls.Extends,
                Summary = cls.Summary,
                ConfigOptions = this.CompileConfigOptions(cls),
                Methods = ""
            };

            string tpl = cls.Abstract ? "AbstractClass" : "ConcreteClass";

            return this.GetTemplate(tpl).FormatWith(obj);
        }
    }

    public class ConfigOptionsProperty : MetaFactory
    {
        public ConfigOptionsProperty(string templateRoot) : base(templateRoot) { }

        public virtual string Compile(Class cls)
        {
            var obj = new
            {
                Name = cls.Name,
                FileName = cls.FileName,
                PrivateName = cls.Name.ToLowerCamelCase(),
                Extends = cls.Extends,
                Summary = cls.Summary,
                ConfigOptions = this.CompileConfigOptions(cls),
                Methods = "",
                Abstract = cls.Abstract ? "abstract " : ""
            };

            return this.GetTemplate("Class").FormatWith(obj);
        }

        protected override string CompileConfigOptions(Class cls)
        {
            StringBuilder configOptions = new StringBuilder(256);

            foreach (ConfigOption member in cls.ConfigOptions)
            {
                if (member.IsConfigOption())
                {
                    //MetaAttribute configOptionAttribute = member.Attributes.Find(delegate(MetaAttribute attr) { return attr.Type.Equals("ConfigOption"); });

                    //if (configOptionAttribute != null && configOptionAttribute.Value != null && !configOptionAttribute.Value.Contains("JsonMode.Ignore"))
                    //{
                        member.Owner = cls;
                        configOptions.Append(this.ConfigOptionFactory.Compile(member));
                    //}
                }
            }

            return configOptions.ToString();
        }
    }

    public abstract class MemberFactoryBase : IFactory
    {
        public MemberFactoryBase(string templateRoot)
        {
            this.TemplateRoot = templateRoot;
        }

        public virtual string TemplateRoot { get; set; }

        public virtual string GetTemplate(string type)
        {
            string path = this.TemplateRoot.ConcatWith(type, ".txt");

            if (File.Exists(path))
            {
                return FileUtils.ReadFile(path);
            }

            return this.GetTemplate("object");
        }
    }

    public class MethodFactory : MemberFactoryBase
    {
        public MethodFactory(string templateRoot) : base(templateRoot) { }

        public virtual string Compile(object obj, string template)
        {
            return this.GetTemplate(template).FormatWith(obj);
        }

        public virtual string Compile(Method member)
        {
            string privateName = member.Name.ToLowerCamelCase();

            var obj = new
            {
                Name = member.Name,
                PrivateName = privateName.IsReservedWord() ? "_" + privateName : privateName,
                Return = member.Return,
                Summary = member.Summary,
                Modifier = member.Modifier.ToString().ToLower(),
                ParamsRaw = member.ParamsRaw,
                ParamValues = member.ParamValues
            };

            string template = "method";

            if (member.Template.IsNotEmpty())
            {
                template = member.Template;
            }

            return this.Compile(obj, template);
        }
    }

    public class ConfigOptionFactory : MemberFactoryBase
    {
        public ConfigOptionFactory(string templateRoot) : base(templateRoot) { }

        public virtual string Compile(object obj, string template)
        {
            return this.GetTemplate(template).FormatWith(obj);
        }

        public virtual string Compile(ConfigOption member)
        {
            string privateName = member.Name.ToLowerCamelCase();
            string serializationOptions = "null";

            MetaAttribute configOptionAttribute = member.Attributes.Find(delegate(MetaAttribute attr) { return attr.Type.Equals("ConfigOption"); });

            if (configOptionAttribute != null && configOptionAttribute.Value.IsNotEmpty())
            {
                serializationOptions = "new SerializationOptions({0})".FormatWith(configOptionAttribute.Value);
            }

            var obj = new
            {
                Name = member.Name,
                PrivateName = privateName.IsReservedWord() ? "_" + privateName : privateName,
                LowerCamelName = privateName,
                Type = member.Type,
                DefaultValue = member.DefaultValue ?? "null",
                PrivateValue = member.PrivateValue ?? "null",
                Summary = member.Summary,
                Modifier = member.Modifier.ToString().ToLower(),
                SerializationOptions = serializationOptions
            };

            string template = member.DefaultValue != null ? member.Type : "instance";

            if (member.Template.IsNotEmpty())
            {
                template = member.Template;
            }

            return this.Compile(obj, template);
        }
    }
}