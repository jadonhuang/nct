
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Nct.Extentions;
using Nct.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nct.Scripting
{

    public class Context
    {
        public dynamic context { get; set; }

    }

    public class Script
    {

        private ScriptState<object> instance;

        private List<Assembly> referenceAssemblies;

        private List<string> importNameSpaces;

        private ScriptOptions scriptOptions;

        private static object LockObj = new object();


        private static string[] ForbiddenNameSpaces = new string[] { "System.IO" };

        private const string DllFolder = "App_Data/ScriptDll";

        private const string ScriptClassName = "ScriptClass";

        private const string ContextArgDefine = "dynamic context = new ExpandoObject();";

        public bool RunAfterCompiled { get; set; } = false;

        public List<string> ImportNameSpaces
        {
            get
            {

                return importNameSpaces;

            }
        }




        public Script Initialize(List<Assembly> references, List<string> imports)
        {
            referenceAssemblies = references;
            importNameSpaces = imports;
            scriptOptions ??= ScriptOptions.Default.AddReferences(references).AddImports(imports);

            if (!RunAfterCompiled)
            {
                instance ??= CSharpScript.RunAsync(ContextArgDefine, scriptOptions).Result;
            }
            return this;
        }


        private CSharpCompilation CreateCompilation(string code, string assemblyName = null, List<string> addionalAssemblyPaths = null)
        {

            CheckCodePermission(code);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            assemblyName = assemblyName ?? Path.GetRandomFileName();

            List<MetadataReference> metadataReferences = new List<MetadataReference>();

            foreach (var assembly in referenceAssemblies)
            {
                var metadataReference = MetadataReference.CreateFromFile(assembly.Location);
                metadataReferences.Add(metadataReference);

            }

            if (addionalAssemblyPaths != null)
            {
                foreach (var assemblyPath in addionalAssemblyPaths)
                {
                    var metadataReference = MetadataReference.CreateFromFile(assemblyPath);
                    metadataReferences.Add(metadataReference);

                }

            }

            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName).AddReferences(metadataReferences)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddSyntaxTrees(syntaxTree);
            return compilation;

        }

        public string BuildImportString()
        {
            var sb = new StringBuilder();
            foreach (string import in importNameSpaces)
            {
                sb.AppendLine($"using {import};");
            }

            return sb.ToString();
        }

        public string BuildCodeToClass(string code)
        {
            var sb = new StringBuilder();
            sb.AppendLine(BuildImportString());
            sb.AppendLine($"public class {ScriptClassName}:Nct.Scripting.BaseScript");
            sb.AppendLine("{");
            sb.AppendLine(code);
            sb.AppendLine("}");
            return sb.ToString();
        }

        public BaseScript GetBaseScriptInstance(string code)
        {
            Assembly assembly = GenerateOrGetAssemblyFromCode(code, true);
            var instance = assembly.CreateInstance(ScriptClassName);
            return (BaseScript)instance;
        }



        private void CheckCodePermission(string code)
        {
            foreach (var np in ForbiddenNameSpaces)
            {
                if (code.Contains(np))
                    throw new System.Security.SecurityException($"No Permission of {np}");
            }

        }

        public RetResult Compile(string code, string assemblyName = null, bool isClassCode = false, List<string> additionalAssemblyPaths = null)
        {
            var retResult = new RetResult();
            if (!isClassCode)
            {
                code = BuildCodeToClass(code);
            }

            var compilation = CreateCompilation(code, assemblyName, additionalAssemblyPaths);
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                                diagnostic.IsWarningAsError ||
                                diagnostic.Severity == DiagnosticSeverity.Error);
                    string msg = string.Empty;
                    foreach (Diagnostic diagnostic in failures)
                    {

                        var d = diagnostic.Location.GetType().FullName;
                        msg += $"{diagnostic.Id}：{diagnostic.GetMessage()}{Environment.NewLine}";
                    }

                    return retResult.Error(msg);
                }

                var bytes = ms.ToArray();

                return retResult.Success(bytes);


            }


        }

        public Assembly GenerateOrGetAssemblyFromCode(string code, bool isClassCode = false, List<string> addionalAssemblyPaths = null)
        {


            var key = code.ToMd5();

            var dllDir = PathHelper.ContentPath(DllFolder);

            var dllName = Path.Combine(dllDir, $"{key}.dll");

            if (File.Exists(dllName))
            {
                return Assembly.LoadFile(dllName);
            }


            var ret = Compile(code, null, isClassCode, addionalAssemblyPaths);

            if (ret.IsSuccess)
            {

                var bytes = (byte[])ret.Data;

                try
                {
                    Directory.CreateDirectory(dllDir);
                    using (FileStream fs = new FileStream(dllName, FileMode.Create))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    return Assembly.LoadFile(dllName);
                }
                catch { }

            }

            return null;

        }



        public T DebugCode<T>(string code, dynamic context)
        {

            CheckCodePermission(code);
            var sb = new StringBuilder();
            sb.AppendLine(BuildImportString());

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(code);
            code = sb.ToString();

            string tmpFile = Path.GetTempFileName() + ".cs";
            var encoding = Encoding.UTF8;
            File.WriteAllText(tmpFile, code, encoding);

            var options = scriptOptions
                  .WithEmitDebugInformation(true)
                  .WithFilePath(tmpFile)
                  .WithFileEncoding(encoding);

            try
            {

                //此处F11即可调试动态代码
                var result = CSharpScript.EvaluateAsync(code, options, new Context { context = context }).Result;

                return (T)result;

            }
            finally
            {
                File.Delete(tmpFile);
            }


        }


        public T Evaluate<T>(string code, dynamic context = null)
        {


            //if (Debugger.IsAttached)
            //{              
            //    return DebugCode<T>(code, context);
            //}


            if (RunAfterCompiled)
            {

                Assembly assembly = GenerateOrGetAssemblyFromCode(code);
                Type type = assembly.GetType(ScriptClassName);
                dynamic obj = Activator.CreateInstance(type);
                return (T)obj.Execute(context);

            }


            lock (LockObj)
            {
                if (instance == null)
                    throw new ArgumentException("instance 请先初始化");

                CheckCodePermission(code);

                if (context != null)
                {
                    var arg = instance.GetVariable("context");
                    arg.Value = context;
                }
                var state = instance.ContinueWithAsync<T>(code);
                var value = state.Result.ReturnValue;
                return value;
            }
        }


    }
}
