using System;
using System.IO;
using System.Linq;
using Base;

namespace PluginLoader
{
  internal static class Program
  {
    private static readonly string Path =
      @$"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent}\Plugin\bin\Debug\net5.0";
    
    private static void Main()
    {
      var assemblies = PluginService.LoadPluginAssemblies(Path);
      foreach (var (_, types) in assemblies)
      {
        var type = types.FirstOrDefault(type => typeof(IPlugin).IsAssignableFrom(type));
        var instance = (IPlugin)Activator.CreateInstance(type);
        Console.WriteLine(instance.Get());
      }
    }
  }
}