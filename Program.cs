
using DiskCardGame;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace ProjectName;

[BepInPlugin( PluginGuid, PluginName, PluginVersion )]
public class Plugin : BaseUnityPlugin
{
	private const string PluginGuid = "MADH.inscryption.ConduitAbilityFix";
	private const string PluginName = "ConduitAbilityFix";
	private const string PluginVersion = "1.0.0";

	internal static ManualLogSource Log;

	private void Awake ()
	{
		Logger.LogInfo( $"Loaded {PluginName}!" );
		Plugin.Log = base.Logger;

		Harmony harmony = new(PluginGuid);
		harmony.PatchAll();

	}
}

[HarmonyPatch( typeof( PlayableCard ) )]
internal static class PlayableCard_GetPassiveAttackBuffs
{
    [HarmonyILManipulator]
    [HarmonyPatch( nameof( PlayableCard.GetPassiveAttackBuffs ) )]
    private static void EnablePart1IL ( ILContext il )
    {
        ILCursor c = new(il);

        ILLabel label = null;
        c.GotoNext( MoveType.After,
            x => x.MatchCallvirt( AccessTools.PropertyGetter( typeof( SaveFile ), nameof( SaveFile.IsPart2 ) ) ),
            x => x.MatchBrtrue( out label )
        );

        c.Emit( OpCodes.Call, AccessTools.PropertyGetter( typeof( SaveManager ), nameof( SaveManager.SaveFile ) ) );
        c.Emit( OpCodes.Callvirt, AccessTools.PropertyGetter( typeof( SaveFile ), nameof( SaveFile.IsPart1 ) ) );
        c.Emit( OpCodes.Brtrue, label );
    }
}