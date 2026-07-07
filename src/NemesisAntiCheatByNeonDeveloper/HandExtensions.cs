using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Interaction;
using LabFusion.Entities;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class HandExtensions
{
	public static SpawnGun ND_HandGrabbedSpawnGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponentInChildren<SpawnGun>();
	}

	public static bool ND_IsGrabbedSpawnGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponentInChildren<SpawnGun>() != null;
	}

	public static FlyingGun ND_HandGrabbedNimbusGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<FlyingGun>();
	}

	public static bool ND_IsGrabbedNimbusGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<FlyingGun>() != null;
	}

	public static AIBrain ND_IsHandGrabbedNPCAIBrain(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<AIBrain>();
	}

	public static bool ND_IsHandGrabbingNPC(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<AIBrain>() != null;
	}

	public static bool ND_IsHandGrabbingAnyNetPlayer(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() != null;
	}

	public static bool ND_IsHandGrabbingNetPlayer(this Hand Handnow, NetworkPlayer PlayerNow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() == PlayerNow?.RigRefs?.RigManager;
	}

	public static bool ND_IsHandGrabbingYou(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<RigManager>() == Player.RigManager;
	}

	public static GameObject ND_GetAttachedObject(this Hand Handnow)
	{
		return Handnow?.m_CurrentAttachedGO;
	}

	public static MarrowEntity ND_GetMarrowEntity(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<MarrowEntity>();
	}

	public static Gun ND_GetGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<Gun>();
	}

	public static bool ND_HasGun(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<Gun>() != null;
	}

	public static StabSlash ND_GetMelee(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<StabSlash>();
	}

	public static bool ND_HasMelee(this Hand Handnow)
	{
		return Handnow?.ND_GetAttachedObject()?.transform?.root?.GetComponent<StabSlash>() != null;
	}
}
