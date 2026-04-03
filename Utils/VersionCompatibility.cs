using System.Reflection;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Modding;

namespace BaseLib.Utils;

internal static class VersionCompatibility
{
    private static bool? _isLegacyVersion;
    private static PropertyInfo? _loadedModsProperty;
    private static string? _gameVersion;

    internal static string? GameVersion
    {
        get
        {
            if (_gameVersion != null)
                return _gameVersion;

            try
            {
                _gameVersion = ReleaseInfoManager.Instance.ReleaseInfo?.Version;
                return _gameVersion;
            }
            catch
            {
                return null;
            }
        }
    }

    internal static bool IsLegacyVersion
    {
        get
        {
            if (_isLegacyVersion.HasValue)
                return _isLegacyVersion.Value;

            _loadedModsProperty = typeof(ModManager).GetProperty("LoadedMods", BindingFlags.Public | BindingFlags.Static);
            _isLegacyVersion = _loadedModsProperty == null;
            return _isLegacyVersion.Value;
        }
    }

    internal static IEnumerable<Mod> GetLoadedMods()
    {
        if (!IsLegacyVersion && _loadedModsProperty != null)
        {
            var result = _loadedModsProperty.GetValue(null);
            if (result is IEnumerable<Mod> mods)
                return mods;
        }

        var getLoadedModsMethod = typeof(ModManager).GetMethod("GetLoadedMods", BindingFlags.Public | BindingFlags.Static);
        if (getLoadedModsMethod != null)
        {
            var result = getLoadedModsMethod.Invoke(null, null);
            if (result is IEnumerable<Mod> mods)
                return mods;
        }

        return [];
    }

    internal static string GetThemeConstantName(string baseName)
    {
        if (IsLegacyVersion)
        {
            return char.ToUpper(baseName[0]) + baseName.Substring(1);
        }
        return baseName;
    }

    private static StringName? _fontSize;
    private static StringName? _fontColor;
    private static StringName? _fontShadowColor;
    private static StringName? _fontOutlineColor;
    private static StringName? _outlineSize;
    private static StringName? _font;

    internal static StringName FontSize => _fontSize ??= GetThemeConstant("fontSize");
    internal static StringName FontColor => _fontColor ??= GetThemeConstant("fontColor");
    internal static StringName FontShadowColor => _fontShadowColor ??= GetThemeConstant("fontShadowColor");
    internal static StringName FontOutlineColor => _fontOutlineColor ??= GetThemeConstant("fontOutlineColor");
    internal static StringName OutlineSize => _outlineSize ??= GetThemeConstant("outlineSize");
    internal static StringName Font => _font ??= GetThemeConstant("font");

    private static StringName GetThemeConstant(string baseName)
    {
        var fieldName = GetThemeConstantName(baseName);
        var field = typeof(ThemeConstants.Label).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        return field?.GetValue(null) as StringName ?? new StringName(baseName);
    }
}
