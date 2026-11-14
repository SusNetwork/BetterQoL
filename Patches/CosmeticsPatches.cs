using HarmonyLib;

namespace BetterQoL.Patches;

/// <summary>
/// Client-side cosmetics options adapted from AUnlocker's CosmeticsPatches.
/// </summary>
[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
internal static class HatManagerInitializePatch
{
	private static void Postfix(HatManager __instance)
	{
		if(!BetterQoLPlugin.UnlockCosmetics.Value) return;

		foreach(var bundle in __instance.allBundles) bundle.Free = true;
		foreach(var bundle in __instance.allFeaturedBundles) bundle.Free = true;
		foreach(var cube in __instance.allFeaturedCubes) cube.Free = true;
		foreach(var item in __instance.allFeaturedItems) item.Free = true;
		foreach(var hat in __instance.allHats) hat.Free = true;
		foreach(var nameplate in __instance.allNamePlates) nameplate.Free = true;
		foreach(var pet in __instance.allPets) pet.Free = true;
		foreach(var skin in __instance.allSkins) skin.Free = true;
		foreach(var bundle in __instance.allStarBundles) bundle.price = 0;
		foreach(var visor in __instance.allVisors) visor.Free = true;
	}
}

[HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
internal static class PlayerPurchasesDataGetPurchasePatch
{
	private static bool Prefix(ref bool __result)
	{
		if(!BetterQoLPlugin.UnlockCosmetics.Value) return true;

		__result = true;
		return false;
	}
}
