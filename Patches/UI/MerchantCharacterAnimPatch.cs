using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace BaseLib.Patches.UI;

[HarmonyPatch(typeof(NMerchantCharacter), nameof(NMerchantCharacter.PlayAnimation))]
class MerchantCharacterAnimPatch
{
    [HarmonyPrefix]
    public static bool SkipAnimIfNotSpine(NMerchantCharacter __instance, string anim, bool loop)
    {
        return !CustomAnimation.PlayCustomAnimation(__instance, anim);
    }
}