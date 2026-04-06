using System.Reflection;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BaseLib.Patches.Features;
using BaseLib.Patches.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BaseLib.Patches;

//Simplest patch that occurs after mod initialization, before anything else is done
[HarmonyPatch(typeof(LocManager), nameof(LocManager.Initialize))] 
class PostModInitPatch
{
    [HarmonyPrefix]
    private static void ProcessModdedTypes()
    {
        BaseLibMain.Logger.Info("PostModInitPatch.ProcessModdedTypes() called - LocManager.Initialize prefix");
        
        Harmony harmony = new("PostModInit");

        try
        {
            AddCustomEncounters.Patch(harmony);
        }
        catch (Exception e)
        {
            BaseLibMain.Logger.Error($"AddCustomEncounters.Patch failed: {e}");
        }
        
        ModInterop interop;
        try
        {
            interop = new();
        }
        catch (Exception e)
        {
            BaseLibMain.Logger.Error($"ModInterop creation failed: {e}");
            interop = null;
        }
        
        var modTypes = ReflectionHelper.ModTypes;
        BaseLibMain.Logger.Info($"Processing {modTypes?.Length ?? 0} mod types");
        
        if (modTypes != null)
        {
            foreach (var type in modTypes)
            {
                if (type == null) continue;
                
                try
                {
                    interop?.ProcessType(harmony, type);

                    bool hasSavedProperty = false;
                    foreach (var prop in type.GetProperties())
                    {
                        var savedPropertyAttr = prop.GetCustomAttribute<SavedPropertyAttribute>();
                        if (savedPropertyAttr == null) continue;
                        if (prop.DeclaringType == null) continue;

                        if (prop.DeclaringType.GetRootNamespace() != "MegaCrit")
                        {
                            var prefix = prop.DeclaringType.GetRootNamespace() + "_";
                            if (prop.Name.Length < 16 && !prop.Name.StartsWith(prefix))
                            {
                                BaseLibMain.Logger.Warn($"Recommended to add a prefix such as \"{prefix}\" to SavedProperty {prop.Name} for compatibility.");
                            }
                        }
                        
                        hasSavedProperty = true;
                    }

                    foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        SavedSpireFieldPatch.CheckSavedSpireField(field);
                    }

                    if (hasSavedProperty)
                    {
                        SavedPropertiesTypeCache.InjectTypeIntoCache(type);
                    }
                }
                catch (Exception e)
                {
                    BaseLibMain.Logger.Error($"Error processing type {type.FullName}: {e}");
                }
            }
        }

        try
        {
            SavedSpireFieldPatch.AddFieldsSorted();
        }
        catch (Exception e)
        {
            BaseLibMain.Logger.Error($"SavedSpireFieldPatch.AddFieldsSorted failed: {e}");
        }
        
        BaseLibMain.Logger.Info("PostModInitPatch.ProcessModdedTypes() completed");
    }

}