using HarmonyLib;
using TMPro;
using UnityEngine;

namespace BetterQoL.Patches;

/// <summary>
/// Port of AUR's ChatControllerUpdatePatch (AUR/Patches/ChatPatches.cs).
/// AUR re-applies the colours and limits every frame because the game resets them.
/// </summary>
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
internal static class ChatControllerUpdatePatch
{
	private const int DefaultCharacterLimit = 120;
	private const int UnlimitedCharacterLimit = 999999999;

	private static void Postfix(ChatController __instance)
	{
		if(!__instance || !__instance.freeChatField || !__instance.freeChatField.textArea || !__instance.freeChatField.background || __instance.freeChatField.textArea.compoText == null || !__instance.freeChatField.textArea.outputText) return;
		if(!__instance.quickChatField || !__instance.quickChatField.background || __instance.quickChatField.text == null) return;

		if(BetterQoLPlugin.ChatDarkTheme.Value)
		{
			__instance.freeChatField.background.color = new Color32(40, 40, 40, byte.MaxValue);
			__instance.freeChatField.textArea.compoText.Color(Color.white);
			__instance.freeChatField.textArea.outputText.color = Color.white;

			__instance.quickChatField.background.color = new Color32(40, 40, 40, byte.MaxValue);
			__instance.quickChatField.text.color = Color.white;
		}
		else
		{
			__instance.freeChatField.background.color = Color.white;
			__instance.freeChatField.textArea.compoText.Color(Color.black);
			__instance.freeChatField.textArea.outputText.color = Color.black;

			__instance.quickChatField.background.color = Color.white;
			__instance.quickChatField.text.color = Color.black;
		}

		if(BetterQoLPlugin.CustomChatCooldown.Value)
		{
			float cooldown = BetterQoLPlugin.GetChatCooldown();

			// AUR clamps with "< 0.9f" against the default; we clamp against our own value
			// so the game never resets the timer below what the user asked for.
			if(__instance.timeSinceLastMessage < cooldown)
			{
				__instance.timeSinceLastMessage = cooldown;
			}
		}

		__instance.freeChatField.textArea.AllowSymbols = true;
		__instance.freeChatField.textArea.AllowEmail = true;
		__instance.freeChatField.textArea.characterLimit = BetterQoLPlugin.BypassCharacterLimit.Value
			? UnlimitedCharacterLimit
			: DefaultCharacterLimit;
	}
}

/// <summary>
/// Port of AUR's ChatBubbleSetNamePatch: dark bubble background + white text.
/// </summary>
[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
internal static class ChatBubbleSetNamePatch
{
	private static void Postfix(ChatBubble __instance)
	{
		if(!BetterQoLPlugin.ChatDarkTheme.Value) return;
		if(!__instance || !__instance.playerInfo || !__instance.playerInfo.Object || !__instance.playerInfo.Object.Data || !__instance.TextArea || !__instance.Background) return;

		__instance.Background.color = new Color(0.1f, 0.1f, 0.1f, 1f);
		__instance.TextArea.color = Color.white;

		if(__instance.playerInfo.Object.Data.IsDead)
		{
			__instance.Background.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
		}
	}
}
