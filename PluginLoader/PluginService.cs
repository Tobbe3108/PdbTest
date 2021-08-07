using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PdbTest;

namespace PluginLoader
{
  public static class PluginService
  {
    public static IEnumerable<(Assembly, Type[])> LoadPluginAssemblies(string path)
    {
      var assembliesAndTypes = new List<(Assembly, Type[])>();
      var files = Directory.EnumerateFiles(path, "Plugin.*.dll");
      foreach (var file in files)
      {
        var loadContext = new PluginLoadContext(file);
        try
        {
          using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Delete);
          var assembly = loadContext.LoadFromStream(stream);
          var types = assembly.GetTypes();
          assembliesAndTypes.Add((assembly, types));
        }
        catch
        {
          // ignored
        }
        finally
        {
          loadContext.Unload();
        }
      }
    
      return assembliesAndTypes;
    }
  }
}