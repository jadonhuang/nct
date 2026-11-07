using Microsoft.CodeAnalysis;
using Nct.Modularity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Nct.Scripting
{

    [DependsOn<KernelModule>]
    public class ScriptingModule : BaseModule
    {

       
        public override void OnPreInitialize(ApplicationInitContext context)
        {
            IocManager.Register<Script>();

        }

        public override void OnAfterInitialize(ApplicationInitContext context)
        {

            var script = IocManager.Resolve<Script>();
            script.RunAfterCompiled = true;

            var assemblies = new List<Assembly>
            {
                typeof(ScriptingModule).Assembly
            };
            var imports = new List<string> {
                         "System",
                         "System.Collections.Generic",
                         "System.Linq",
                         "System.Text",
                         "System.Dynamic",
                         "System.Data",
                         "System.Threading.Tasks",
                         "System.Threading",
                         "System.Text.RegularExpressions",


                    };


            var moduleManager = IModuleManager.Instance;

            foreach (var module in moduleManager.Modules)
            {
                if (module.Type == moduleManager.StartupModule.Type) continue;

                foreach (var type in module.Assembly.GetTypes())
                {
                    var nameSpace = type.Namespace;
                    if (nameSpace == null) continue;
                    if (!imports.Contains(nameSpace))
                    {
                        imports.Add(nameSpace);
                    }

                }

            }

            var defAssemblies = AssemblyLoadContext.Default.Assemblies;

            foreach (var assembly in defAssemblies)
            {
                if (assembly.IsDynamic) continue;

                assemblies.Add(assembly);

                var referenceAsms = assembly.GetReferencedAssemblies();
                foreach (var asm in referenceAsms)
                {

                    if (assemblies.Any(m => m.GetName().Name == asm.Name)) continue;
                    assemblies.Add(Assembly.Load(asm));

                }



            }
            assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly);  //for can use dynamic
            script.Initialize(assemblies, imports);


        }


    }
}
