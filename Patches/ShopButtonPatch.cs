using HarmonyLib;
using UnityEngine;

namespace BetterQoL.Patches;

/// <summary>Disables the Shop button using the game's own disabled visual state.</summary>
[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
internal static class ShopButtonPatch
{
	private static void Postfix(MainMenuManager __instance)
	{
		ShopButtonVisualState.Apply(__instance);
	}
}

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
internal static class ShopButtonVisualPatch
{
	private static void Postfix(MainMenuManager __instance) => ShopButtonVisualState.Apply(__instance);
}

internal static class ShopButtonVisualState
{
	private const float GrayAmount = 0.35f;
	private static MainMenuManager cachedManager;
	private static SpriteRenderer[] renderers;
	private static Color[] originalRendererColors;
	private static TMPro.TextMeshPro buttonText;
	private static Color originalTextColor;

	internal static void Apply(MainMenuManager manager)
	{
		if(manager == null || manager.shopButton == null) return;

		if(cachedManager != manager || renderers == null)
		{
			cachedManager = manager;
			PassiveButton shopButton = manager.shopButton;
			shopButton.OnClick = new();
			renderers = shopButton.GetComponentsInChildren<SpriteRenderer>(true);
			originalRendererColors = new Color[renderers.Length];
			for(int i = 0; i < renderers.Length; i++)
				originalRendererColors[i] = renderers[i] != null ? renderers[i].color : Color.white;

			buttonText = shopButton.buttonText;
			if(buttonText != null) originalTextColor = buttonText.color;
		}

		// PassiveButton refreshes its normal/hover colors during gameplay, so
		// reapply a mild tint after its updates while retaining the original colors.
		for(int i = 0; i < renderers.Length; i++)
		{
			if(renderers[i] != null)
				renderers[i].color = Color.Lerp(originalRendererColors[i], Color.gray, GrayAmount);
		}

		if(buttonText != null)
			buttonText.color = Color.Lerp(originalTextColor, Color.gray, GrayAmount);
	}
}

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.TransitionToShop))]
internal static class BlockShopTransitionPatch
{
	private static bool Prefix() => false;
}
