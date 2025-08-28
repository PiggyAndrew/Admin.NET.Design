using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.MVVM;

public class DSDApplication
{
    public DSDApplication()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Resolver = new AssemblyResolver(AppDomain.CurrentDomain, assembly);
    }

    private AssemblyResolver Resolver { get; }

    class AssemblyResolver
    {
        public AssemblyResolver(AppDomain domain, Assembly assembly)
        {
            domain.AssemblyResolve += Domain_AssemblyResolve;
            Assembly = assembly;
        }

        public Assembly Assembly { get; }

        private System.Reflection.Assembly? Domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly? assembly = default;

            var assemblyName = args.Name.Split(',')[0];
            if (assemblyName.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
            {
                return assembly;
            }

            string location = Assembly.Location;
            DirectoryInfo info = new DirectoryInfo(location);

            string filePath = @$"{info.Parent.FullName}\{assemblyName}.dll";
            if (File.Exists(filePath))
            {
                assembly = Assembly.LoadFrom(filePath);
            }

            return assembly;
        }
    }
}
