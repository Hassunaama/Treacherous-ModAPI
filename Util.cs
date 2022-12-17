using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;

using HarmonyLib;

using JetBrains.Annotations;

using UnityEngine;

using Object = UnityEngine.Object;

// Copyright cgytrus (ConfiG) / SixDash 2022, unless said otherwise. SixDash is licensed under the MIT license. (https://github.com/cgytrus/SixDash)

namespace Treacherous_Modder.Misc;

/// <summary>
/// Utilities.
/// </summary>
[PublicAPI]
public static class Util
{
    internal static ManualLogSource? logger { get; set; }

    /// <summary>
    /// Finds a resource of the specified type with a specified name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <returns>The found resource. Null if no resource was found.</returns>
    public static Object? FindResourceOfTypeWithName(Type type, string name)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (Object obj in Resources.FindObjectsOfTypeAll(type))
        {
            if (obj.name != name)
                continue;
            return obj;
        }

        return null;
    }

    /// <summary>
    /// Finds a resource of the specified type with a specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The found resource. Null if no resource was found.</returns>
    public static T? FindResourceOfTypeWithName<T>(string name) where T : Object =>
        (T?)FindResourceOfTypeWithName(typeof(T), name);


    /// <summary>
    /// Iterates each implementation of an type in all assemblies.
    /// </summary>
    /// <param name="baseType">The type.</param>
    /// <param name="action">Execute for each found type.</param>
    public static void ForEachImplementation(Type baseType, Action<Type> action)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            ForEachImplementation(assembly, baseType, action);
    }

    /// <summary>
    /// Iterates each implementation of an type in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <param name="baseType">The type.</param>
    /// <param name="action">Execute for each found type.</param>
    public static void ForEachImplementation(Assembly assembly, Type baseType, Action<Type> action)
    {
        try
        {
            foreach (Type type in assembly.GetTypes())
                if (new List<Type>(type.GetNestedTypes(AccessTools.all)).Contains(baseType) ||
                   new List<Type>(type.GetInterfaces()).Contains(baseType))
                    action(type);
        }
        catch (ReflectionTypeLoadException ex)
        {
            LogReflectionTypeLoadException(ex);
        }
    }

    private static void LogReflectionTypeLoadException(ReflectionTypeLoadException ex)
    {
        if (logger is null)
            Debug.LogWarning(ex);
        else
            logger.LogWarning(ex);
        foreach (Exception loaderException in ex.LoaderExceptions)
            if (logger is null)
                Debug.LogWarning(loaderException);
            else
                logger.LogWarning(loaderException);
    }

    /// <summary>
    /// Loads an AssetBundle from <see cref="Application.streamingAssetsPath"/> with the specified path with
    /// a platform identifier appended at the end.
    /// </summary>
    /// <param name="path">AssetBundle path inside <see cref="Application.streamingAssetsPath"/>.</param>
    /// <returns>The loaded AssetBundle.</returns>
    public static AssetBundle LoadPlatformAssetBundle(string path)
    {
        string platform = Application.platform switch
        {
            RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer or RuntimePlatform.WindowsServer =>
                "StandaloneWindows64",
            RuntimePlatform.LinuxEditor or RuntimePlatform.LinuxPlayer or RuntimePlatform.LinuxServer =>
                "StandaloneLinux64",
            RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer or RuntimePlatform.OSXServer => "StandaloneOSX",
            _ => "Unknown"
        };
        return AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, $"{path}-{platform}"));
    }
}