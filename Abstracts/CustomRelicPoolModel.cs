using MegaCrit.Sts2.Core.Models;
using BaseLib.Patches.Content;
using BaseLib.Patches.UI;

namespace BaseLib.Abstracts;

public abstract class CustomRelicPoolModel : RelicPoolModel, ICustomModel, ICustomEnergyIconPool
{
    public CustomRelicPoolModel()
    {
        if (IsShared) ModelDbSharedRelicPoolsPatch.Register(this);
    }

    protected override IEnumerable<RelicModel> GenerateAllRelics() => []; //Content added through ModHelper.ConcatModelsFromMods

    /// <summary>
    /// You shouldn't need this (just use SharedRelicPool), but it is allowed.
    /// </summary>
    public virtual bool IsShared => false;

    public override string EnergyColorName => CustomEnergyIconPatches.GetEnergyColorName(Id);
    public virtual string? BigEnergyIconPath => null;
    public virtual string? TextEnergyIconPath => null;
}
