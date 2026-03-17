using MegaCrit.Sts2.Core.Models;
using BaseLib.Patches.Content;
using BaseLib.Patches.UI;

namespace BaseLib.Abstracts;

public abstract class CustomPotionPoolModel : PotionPoolModel, ICustomModel, ICustomEnergyIconPool
{
    public CustomPotionPoolModel()
    {
        if (IsShared) ModelDbSharedPotionPoolsPatch.Register(this);
    }

    protected override IEnumerable<PotionModel> GenerateAllPotions() => []; //Content added through ModHelper.ConcatModelsFromMods

    /// <summary>
    /// You shouldn't need this (just use SharedRelicPool), but it is allowed.
    /// </summary>
    public virtual bool IsShared => false;

    public override string EnergyColorName => CustomEnergyIconPatches.GetEnergyColorName(Id);
    public virtual string? BigEnergyIconPath => null;
    public virtual string? TextEnergyIconPath => null;
}
