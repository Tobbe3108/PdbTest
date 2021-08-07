using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace PdbTest
{
  public class PluginLoadContext : AssemblyLoadContext
  {
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string path) : base(true)
    {
      _resolver = new AssemblyDependencyResolver(path);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
      var path = _resolver.ResolveAssemblyToPath(assemblyName);
      return path != null ? LoadFromStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Delete)) : null;
    }
  }
}