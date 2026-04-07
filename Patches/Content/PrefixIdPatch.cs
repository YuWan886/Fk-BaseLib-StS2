using System.Collections.Concurrent;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils.Attributes;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.GetEntry))]
public class PrefixIdPatch
{
    private static readonly ConcurrentDictionary<Type, string> IdCache = new();

    [HarmonyPostfix]
    static string AdjustID(string __result, Type type)
    {
        if (IdCache.TryGetValue(type, out var cachedId))
        {
            return cachedId;
        }
        
        var attr = type.GetCustomAttribute<CustomIDAttribute>();
        if (attr != null)
        {
            IdCache[type] = attr.ID;
            return attr.ID;
        }
        
        if (type.IsAssignableTo(typeof(ICustomModel)))
        {
            IdCache[type] = type.GetPrefix() + __result;
            return IdCache[type];
        }

        IdCache[type] = __result;
        return __result;
    }
}
