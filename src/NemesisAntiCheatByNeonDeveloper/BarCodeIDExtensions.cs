using Il2CppSLZ.Marrow.Warehouse;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class BarCodeIDExtensions
{
	public static string ND_BarcodeCrateName(this string idnow)
	{
		if (Main.IsAvatarCrateExist(idnow))
		{
			return Main.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.name);
		}
		if (Main.IsLevelCrateExist(idnow))
		{
			return Main.StripColorTags(new LevelCrateReference(idnow)?.Crate?.name);
		}
		if (Main.IsSpawnableCrateExist(idnow))
		{
			return Main.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.name);
		}
		return "NULL";
	}

	public static string ND_BarcodePalletName(this string idnow)
	{
		if (Main.IsAvatarCrateExist(idnow))
		{
			return Main.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.name);
		}
		if (Main.IsLevelCrateExist(idnow))
		{
			return Main.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.name);
		}
		if (Main.IsSpawnableCrateExist(idnow))
		{
			return Main.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.name);
		}
		return "NULL";
	}

	public static string ND_BarcodeAuthor(this string idnow)
	{
		if (Main.IsAvatarCrateExist(idnow))
		{
			return Main.StripColorTags(new AvatarCrateReference(idnow)?.Crate?.Pallet?.Author);
		}
		if (Main.IsLevelCrateExist(idnow))
		{
			return Main.StripColorTags(new LevelCrateReference(idnow)?.Crate?.Pallet?.Author);
		}
		if (Main.IsSpawnableCrateExist(idnow))
		{
			return Main.StripColorTags(new SpawnableCrateReference(idnow)?.Crate?.Pallet?.Author);
		}
		return "NULL";
	}
}
