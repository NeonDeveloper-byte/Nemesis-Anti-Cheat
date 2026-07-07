using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Interaction;
using LabFusion.MonoBehaviours;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class MarrowEntityExtensions
{
	public static AIBrain ND_GetNPCAIBrain(this MarrowEntity entity)
	{
		return entity?.gameObject?.GetComponent<AIBrain>();
	}

	public static string ND_GetBarcodeID(this MarrowEntity entity)
	{
		return entity?._poolee?._SpawnableCrate_k__BackingField?.Barcode?.ID ?? "NULL";
	}

	public static bool ND_IsNetPlayer(this MarrowEntity entity)
	{
		return entity?.gameObject?.transform?.root?.GetComponent<AntiHasher>() != null;
	}
}
