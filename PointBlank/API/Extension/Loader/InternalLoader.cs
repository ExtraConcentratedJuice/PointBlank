﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API.Logging;

namespace PointBlank.API.Extension.Loader
{
    /// <summary>
    /// InternalLoader manages loading, unloading and reloading of types and assemblies that use InternalObject
    /// </summary>
    public static class InternalLoader
    {
        #region Public Properties
        public static List<Type> Blacklist { get; private set; }
        #endregion

        static InternalLoader()
        {
            // Add properties/variables
            Blacklist = new List<Type>
            {
                typeof(InternalObject)
            };
        }

        #region Public Functions
        /// <summary>
        /// Load all types that extend InternalObject inside the specified Assembly
        /// </summary>
        /// <param name="assembly">The assembly to search in</param>
        /// <returns>Was the loading successful or not</returns>
        public static bool LoadAssembly(Assembly assembly)
        {
            try
            {
                foreach(Type t in assembly.GetTypes())
                    LoadType(t);

                PointBlankLogging.Log("Loaded assembly: " + assembly.GetName().Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to load assembly: " + assembly.GetName().Name, ex, false, false);
                return false;
            }
        }
        /// <summary>
        /// Load a specific type that extends InternalObject
        /// </summary>
        /// <param name="type">The type to load</param>
        /// <returns>Was the type loaded successfully</returns>
        public static bool LoadType(Type type)
        {
            if (!type.IsClass || !typeof(InternalObject).IsAssignableFrom(type))
                return false;
            if (Blacklist.Contains(type))
                return false;
            if (PointBlankEnvironment.ModLoaderInternals.ContainsKey(type))
                return false;

            try
            {
                InternalObject obj = (InternalObject)Activator.CreateInstance(type);

                obj.Load();
                PointBlankEnvironment.ModLoaderInternals.Add(type, obj);

                PointBlankLogging.Log("Loaded type: " + type.Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to load type: " + type.FullName, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Unload all types that extend InternalObject inside the specified Assembly
        /// </summary>
        /// <param name="assembly">The assembly to search in</param>
        /// <returns>Was the unloading successful or not</returns>
        public static bool UnloadAssembly(Assembly assembly)
        {
            try
            {
                foreach (Type t in assembly.GetTypes())
                    UnloadType(t);

                PointBlankLogging.Log("Unloaded assembly: " + assembly.GetName().Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to unload assembly: " + assembly.GetName().Name, ex, false, false);
                return false;
            }
        }
        /// <summary>
        /// Unload a specific type that extends InternalObject
        /// </summary>
        /// <param name="type">The type to unload</param>
        /// <returns>Was the type unloaded successfully</returns>
        public static bool UnloadType(Type type)
        {
            if (!type.IsClass || !typeof(InternalObject).IsAssignableFrom(type))
                return false;
            if (Blacklist.Contains(type))
                return false;
            if (!PointBlankEnvironment.ModLoaderInternals.ContainsKey(type))
                return false;

            try
            {
                KeyValuePair<Type, InternalObject> obj = PointBlankEnvironment.ModLoaderInternals.FirstOrDefault(a => a.Key == type);
                if (obj.Equals(default(KeyValuePair<Type, InternalObject>)))
                    return false;

                obj.Value.Unload();
                PointBlankEnvironment.ModLoaderInternals.Remove(type);

                PointBlankLogging.Log("Unloaded type: " + type.Name, false);
                return true;
            }
            catch (Exception ex)
            {
                PointBlankLogging.LogError("Unable to unload type: " + type.FullName, ex, false, false);
                return false;
            }
        }

        /// <summary>
        /// Reload all types that extend InternalObject inside the specified Assembly
        /// </summary>
        /// <param name="assembly">The assembly to search in</param>
        /// <returns>Was the reloading successful or not</returns>
        public static bool ReloadAssembly(Assembly assembly) =>
            UnloadAssembly(assembly) && LoadAssembly(assembly);
        /// <summary>
        /// Reload a specific type that extends InternalObject
        /// </summary>
        /// <param name="type">The type to reload</param>
        /// <returns>Was the type reloaded successfully</returns>
        public static bool ReloadType(Type type) =>
            UnloadType(type) && LoadType(type);
        #endregion
    }
}
