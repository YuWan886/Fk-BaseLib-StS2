using System.Reflection;
using System.Runtime.InteropServices;
using BaseLib.Config;
using BaseLib.Patches.Content;
using BaseLib.Utils.NodeFactories;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace BaseLib;

[ModInitializer(nameof(Initialize))]
public static class BaseLibMain
{
    [ThreadStatic]
    public static bool IsMainThread;
    
    public const string ModId = "BaseLib";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Libgcc();

        IsMainThread = true;
        
        Logger.Info("BaseLib initialization starting...");
        
        try
        {
            NodeFactory.Init();
        }
        catch (Exception e)
        {
            Logger.Error($"NodeFactory.Init failed: {e}");
        }
        
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        
        ModConfigRegistry.Register(ModId, new BaseLibConfig());
        
        Logger.Info("Creating Harmony instance and applying patches...");
        
        Harmony harmony = new(ModId);

        try
        {
            GetCustomLocKey.Patch(harmony);
            Logger.Info("GetCustomLocKey patch applied.");
        }
        catch (Exception e)
        {
            Logger.Error($"GetCustomLocKey patch failed: {e}");
        }
        
        try
        {
            TheBigPatchToCardPileCmdAdd.Patch(harmony);
            Logger.Info("TheBigPatchToCardPileCmdAdd patch applied.");
        }
        catch (Exception e)
        {
            Logger.Error($"TheBigPatchToCardPileCmdAdd patch failed: {e}");
        }

        try
        {
            harmony.PatchAll();
            Logger.Info("PatchAll completed successfully.");
        }
        catch (Exception e)
        {
            Logger.Error($"PatchAll failed: {e}");
        }
        
        Logger.Info("BaseLib initialization completed.");
    }

    //Hopefully temporary fix for linux
    [DllImport("libdl.so.2")]
    static extern IntPtr dlopen(string filename, int flags);
    
    [DllImport("libdl.so.2")]
    static extern IntPtr dlerror();
    
    [DllImport("libdl.so.2")]
    static extern IntPtr dlsym(IntPtr handle, string symbol);

    private static IntPtr _holder;
    private static void Libgcc()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Logger.Info("Running on Linux, manually dlopen libgcc for Harmony");
            _holder = dlopen("libgcc_s.so.1", 2 | 256);
            if (_holder == IntPtr.Zero)
            {
                Logger.Info("Or Nor: "+Marshal.PtrToStringAnsi(dlerror()));
            }
        }
    }
}
