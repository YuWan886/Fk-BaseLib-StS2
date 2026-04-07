using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace BaseLib.Patches.Localization;

class CustomTooltips
{
    [HarmonyPatch(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))]
    static class DynamicKeywordTips
    {
        [HarmonyPrefix]
        public static bool CustomKeyword(CardKeyword keyword, ref IHoverTip __result)
        {
            if (CustomKeywords.KeywordIDs.TryGetValue((int) keyword, out var info))
            {
                //HoverTip with model attached or add dictionary manually
                if (info.RichKeyword)
                {
                    LocString description = keyword.GetDescription();
                    description.Add("energyPrefix", "");
                    __result = new HoverTip(keyword.GetTitle(), description);
                    return false;
                }
            }
            
            return true;
        }
    }
}