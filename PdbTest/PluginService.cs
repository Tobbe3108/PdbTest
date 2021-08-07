using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace PdbTest
{
  public static class PluginService
  {
    public static void LoadPluginAssemblyParts(this IServiceCollection services, IEnumerable<(Assembly,Type[])> assemblies)
    {
      var mvcBuilder = services.AddControllers();
      foreach (var assembly in assemblies)
      {
        mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(assembly.Item1));
      }
    }

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

    public static void RegisterPlugins(this ContainerBuilder builder, IEnumerable<(Assembly,Type[])> assemblies)
    {
      foreach (var assembly in assemblies)
      {
        foreach (var type in assembly.Item2.Where(type => typeof(ControllerBase).IsAssignableFrom(type)))
        {
          builder.RegisterType(type).As<ControllerBase>();
        }
      }
    }
  }
}