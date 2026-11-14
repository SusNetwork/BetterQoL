using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BetterQoL;

[BepInPlugin("BetterQoL", "BetterQoL", "1.0.0")]
[BepInProcess("Among Us.exe")]
public class BetterQoLPlugin : BasePlugin
{
	internal static new ManualLogSource Log;
	private static readonly Harmony harmony = new Harmony("BetterQoL");

	/// <summary>
	/// Chat cooldown in seconds, written as "sec.ms" (e.g. "2.2").
	/// </summary>
	internal static ConfigEntry<string> ChatCooldown;

	/// <summary>Applies <see cref="ChatCooldown"/> when true.</summary>
	internal static ConfigEntry<bool> CustomChatCooldown;

	/// <summary>When true the character limit is pushed to 9999999999, otherwise 120.</summary>
	internal static ConfigEntry<bool> BypassCharacterLimit;

	/// <summary>Dark theme for the chat input and chat bubbles.</summary>
	internal static ConfigEntry<bool> ChatDarkTheme;

	/// <summary>Marks cosmetics as free and treats local purchase checks as successful.</summary>
	internal static ConfigEntry<bool> UnlockCosmetics;


	public override void Load()
	{
		Log = base.Log;

		ChatCooldown = Config.Bind(
			"Chat",
			"ChatCooldown",
			"2.2",
			"# DO NOT EDIT THIS VALUE UNLESS YOU KNOW WHAT YOU'RE DOING.\nChat cooldown in seconds, written as sec.ms (e.g. \"2.2\" for 2.2s).");

		CustomChatCooldown = Config.Bind(
			"Chat",
			"CustomChatCooldown",
			true,
			"Apply ChatCooldown as the chat cooldown. When false, the game cooldown is left untouched.");

		BypassCharacterLimit = Config.Bind(
			"Chat",
			"BypassCharacterLimit",
			true,
			"When true the chat character limit becomes 9999999999, otherwise 120.");

		ChatDarkTheme = Config.Bind(
			"Chat",
			"ChatDarkTheme",
			true,
			"Use a dark background for the chat input and chat bubbles.");

		UnlockCosmetics = Config.Bind(
			"Cosmetics",
			"UnlockCosmetics",
			true,
			"Mark cosmetics as free and bypass local purchase checks. This does not grant ownership on your Among Us account.");


		harmony.PatchAll();

		Log.LogInfo($"BetterQoL loaded - chat QoL (dark theme: {ChatDarkTheme.Value}, custom cooldown: {CustomChatCooldown.Value}, bypass limit: {BypassCharacterLimit.Value}), cosmetics (unlock: {UnlockCosmetics.Value})");
	}

	/// <summary>
	/// Parses <see cref="ChatCooldown"/> ("sec.ms"). Falls back to 2.2f on invalid input.
	/// </summary>
	internal static float GetChatCooldown()
	{
		string raw = ChatCooldown?.Value;
		if(float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value) && value >= 0f)
		{
			return value;
		}

		Log?.LogWarning($"Invalid ChatCooldown '{raw}', falling back to 2.2");
		return 2.2f;
	}
}
