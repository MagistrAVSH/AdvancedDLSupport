﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Mono.DllMap
{
    /// <summary>
    /// Helper class for resolving library paths and alternate symbol names through Mono's DllMap files.
    /// </summary>
    [PublicAPI]
    public class DllMapResolver
    {
        /// <summary>
        /// Finds the matching remapping entry, if any, for the given library name and type, and returns the
        /// remapped library name. If no match is found, the library name is returned unchanged.
        /// </summary>
        /// <typeparam name="T">A type defined in the assembly to search the DllMap for.</typeparam>
        /// <param name="libraryName">The original name of the library.</param>
        /// <returns>The remapped name.</returns>
        [Pure, NotNull, PublicAPI]
        public string MapLibraryName<T>(string libraryName) => MapLibraryName(typeof(T), libraryName);

        /// <summary>
        /// Finds the matching remapping entry, if any, for the given library name and type, and returns the
        /// remapped library name. If no match is found, the library name is returned unchanged.
        /// </summary>
        /// <param name="type">A type defined in the assembly to search the DllMap for.</param>
        /// <param name="libraryName">The original name of the library.</param>
        /// <returns>The remapped name.</returns>
        [Pure, NotNull, PublicAPI]
        public string MapLibraryName(Type type, string libraryName) => MapLibraryName(type.Assembly, libraryName);

        /// <summary>
        /// Finds the matching remapping entry, if any, for the given library name and assembly, and returns the
        /// remapped library name. If no match is found, the library name is returned unchanged.
        /// </summary>
        /// <param name="assembly">The assembly to search the DllMap for.</param>
        /// <param name="libraryName">The original name of the library.</param>
        /// <returns>The remapped name.</returns>
        [Pure, NotNull, PublicAPI]
        public string MapLibraryName(Assembly assembly, string libraryName)
        {
            if (!HasDllMapFile(assembly))
            {
                return libraryName;
            }

            var map = GetDllMap(assembly);

            return MapLibraryName(map, libraryName);
        }

        /// <summary>
        /// Finds the matching remapping entry, if any, for the given library name, and returns the remapped library
        /// name. If no match is found, the library name is returned unchanged.
        /// </summary>
        /// <param name="configuration">The DllMap to search.</param>
        /// <param name="libraryName">The original name of the library.</param>
        /// <returns>The remapped name.</returns>
        [Pure, NotNull, PublicAPI]
        public string MapLibraryName(DllConfiguration configuration, string libraryName)
        {
            var mapEntry = configuration.GetRelevantMaps().FirstOrDefault(m => m.SourceLibrary == libraryName);
            if (mapEntry is null)
            {
                return libraryName;
            }

            return mapEntry.TargetLibrary;
        }

        /// <summary>
        /// Determines whether or not the assembly that the given type is declared in has a Mono DllMap configuration
        /// file.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>true if it has a file; otherwise, false.</returns>
        [Pure, PublicAPI]
        public bool HasDllMapFile<T>() => HasDllMapFile(typeof(T));

        /// <summary>
        /// Determines whether or not the assembly that the given type is declared in has a Mono DllMap configuration
        /// file.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if it has a file; otherwise, false.</returns>
        [Pure, PublicAPI]
        public bool HasDllMapFile([NotNull] Type type) => HasDllMapFile(type.Assembly);

        /// <summary>
        /// Determines whether or not the given assembly has a Mono DllMap configuration file.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>true if it has a file; otherwise, false.</returns>
        [Pure, PublicAPI]
        public bool HasDllMapFile([NotNull] Assembly assembly)
        {
            var mapPath = GetDllMapPath(assembly);
            return File.Exists(mapPath) && DllConfiguration.TryParse(File.ReadAllText(mapPath), out _);
        }

        /// <summary>
        /// Gets the DllMap file for the assembly that the given type is declared in.
        /// </summary>
        /// <typeparam name="T">The type to get the configuration for.</typeparam>
        /// <returns>The DllMap.</returns>
        [Pure, NotNull, PublicAPI]
        public DllConfiguration GetDllMap<T>() => GetDllMap(typeof(T));

        /// <summary>
        /// Gets the DllMap file for the assembly that the given type is declared in.
        /// </summary>
        /// <param name="type">The type to get the configuration for.</param>
        /// <returns>The DllMap.</returns>
        [Pure, NotNull, PublicAPI]
        public DllConfiguration GetDllMap(Type type) => GetDllMap(type.Assembly);

        /// <summary>
        /// Gets the DllMap file for the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to get the configuration for.</param>
        /// <returns>The DllMap.</returns>
        [Pure, NotNull, PublicAPI]
        public DllConfiguration GetDllMap([NotNull] Assembly assembly)
        {
            return DllConfiguration.Parse(File.ReadAllText(GetDllMapPath(assembly)));
        }

        [Pure, NotNull]
        private string GetDllMapPath(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;
            var assemblyDirectory = Directory.GetParent(assembly.Location).FullName;
            var assemblyExtension = Path.GetExtension(assembly.Location);

            var mapPath = Path.Combine(assemblyDirectory, $"{assemblyName}{assemblyExtension}.config");
            return mapPath;
        }
    }
}
