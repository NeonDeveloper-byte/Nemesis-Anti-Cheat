using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Pool;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class GameObjectExtensions
{
	public static void DestroyNow(this GameObject target)
	{
		Object.DestroyImmediate(target, allowDestroyingAssets: true);
	}

	public static void ToggleActive(this GameObject target)
	{
		target?.SetActive(!target.activeSelf);
	}

	public static bool IsTooClose(this GameObject target, Transform closetoobject, float meters)
	{
		if (target == null)
		{
			return false;
		}
		if (closetoobject == null)
		{
			return false;
		}
		float num = Vector3.Distance(closetoobject.position, target.transform.position);
		return num < meters;
	}

	public static AIBrain ND_GetNPCAIBrain(this GameObject entity)
	{
		return entity?.GetComponentInChildren<AIBrain>();
	}

	public static bool ND_IsNPC(this GameObject entity)
	{
		return entity?.GetComponentInChildren<AgentLinkControl>() != null || entity?.GetComponentInChildren<AIBrain>() != null;
	}

	public static bool ND_HasPoolee(this GameObject entity)
	{
		return entity?.GetComponentInChildren<Poolee>() != null;
	}

	public static Poolee ND_GetPoolee(this GameObject entity)
	{
		return entity?.GetComponentInChildren<Poolee>();
	}

	public static bool ND_IsWeapon(this GameObject entity)
	{
		return entity?.GetComponentInChildren<Gun>() != null || entity?.GetComponentInChildren<Gun>() != null;
	}

	public static bool ND_IsMelee(this GameObject entity)
	{
		return entity?.GetComponentInChildren<StabSlash>() != null || entity?.GetComponentInChildren<StabSlash>() != null;
	}
}
