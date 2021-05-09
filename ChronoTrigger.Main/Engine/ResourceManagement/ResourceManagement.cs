using System;
using System.Collections.Generic;
using System.IO;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ResourceManagement
{
    public static class ResourceManager
    {
        public static T GetFile<T>(string filename)
        {
            var hashcode = filename.GetHashCode();
            var obj = ResourceManager<int, object>.Get(hashcode);
            if (obj != null)
                return (T) obj;
            ResourceManager<int, object>.Store((T) Activator.CreateInstance(typeof(T), filename), hashcode);
            return (T) ResourceManager<int, object>.Get(hashcode);
        }

        public static void LoadFonts()
        {
            foreach (var font in Directory.EnumerateFiles(GameDirectories.FontsDirectory, "*.ttf"))
            {
                var f = GetFile<Font>(font);
                ResourceManager<string, Font>.Store(f, Path.GetFileName((ReadOnlySpan<char>)font).ToString());
            }
        }
    }

    public static class ResourceManager<TK, TV>
    {
        private static readonly Dictionary<TK, TV> ManagedResources = new();

        public static TV Get(TK key)
        {
            return ManagedResources.TryGetValue(key, out var v) ? v : default;
        }

        public static void Store(TV resource, TK key)
        {
            ManagedResources[key] = resource;
        }
    }
}