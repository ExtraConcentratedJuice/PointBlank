﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Logging;
using PointBlank.API.Extension.Loader;

namespace PointBlank.API.Extension
{
    public static class ExtensionLoader
    {
        #region Public Properties
        public static List<Type> Blacklist { get; private set; }
        #endregion

        static ExtensionLoader()
        {
            // Add properties/variables
            Blacklist = new List<Type>
            {
                typeof(PointBlankExtension)
            };
        }

        #region Public Functions
        public static bool LoadExtension(Assembly assembly)
        {
            try
            {
                PointBlankExtensionAttribute att = (PointBlankExtensionAttribute)Attribute.GetCustomAttribute(assembly, typeof(PointBlankExtensionAttribute));
                PointBlankExtension ext = null;

                if (!att.RawExtension)
                {
                    Type t = assembly.GetTypes().FirstOrDefault(a => a != typeof(PointBlankExtension) && typeof(PointBlankExtension).IsAssignableFrom(a));

                    if(t != null)
                    {
                        ext = (PointBlankExtension)Activator.CreateInstance(t);

                        ext.Load();
                    }
                }
                if (att.LoadInternals)
                    InternalLoader.LoadAssembly(assembly);

                PointBlankEnvironment.ModLoaderExtensions.Add(assembly, ext);

                PointBlankLogging.Log("Loaded extension: " + assembly.GetName().Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to load extension: " + assembly.GetName().Name, ex, false, false);
                return false;
            }
        }

        public static bool UnloadExtension(Assembly assembly)
        {
            if (!PointBlankEnvironment.ModLoaderExtensions.ContainsKey(assembly))
                return false;

            try
            {
                PointBlankExtensionAttribute att = (PointBlankExtensionAttribute)Attribute.GetCustomAttribute(assembly, typeof(PointBlankExtensionAttribute));

                if (!att.RawExtension)
                    PointBlankEnvironment.ModLoaderExtensions[assembly].Unload();
                if (att.LoadInternals)
                    InternalLoader.UnloadAssembly(assembly);

                PointBlankEnvironment.ModLoaderExtensions.Remove(assembly);

                PointBlankLogging.Log("Unloaded extension: " + assembly.GetName().Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to unload extension: " + assembly.GetName().Name, ex, false, false);
                return false;
            }
        }

        public static bool ReloadExtension(Assembly assembly) =>
            UnloadExtension(assembly) && LoadExtension(assembly);
        #endregion
    }
}