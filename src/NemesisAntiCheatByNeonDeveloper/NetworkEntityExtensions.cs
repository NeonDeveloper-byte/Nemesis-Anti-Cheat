using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Interaction;
using LabFusion.Entities;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class NetworkEntityExtensions
{
	public static void ND_Despawn(this NetworkEntity entity)
	{
		Main.DespawnNow(entity);
	}

	public static bool ND_IsNetPlayer(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity() != null && (entity.ND_GetMarrowEntity()?.ND_IsNetPlayer() ?? false);
	}

	public static bool ND_IsMagazine(this NetworkEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		MarrowEntity marrowEntity = entity.ND_GetMarrowEntity();
		if (marrowEntity == null)
		{
			return false;
		}
		GameObject gameObject = marrowEntity.gameObject;
		if (gameObject == null)
		{
			return false;
		}
		Magazine magazine = null;
		try
		{
			magazine = gameObject.GetComponent<Magazine>();
		}
		catch
		{
			return false;
		}
		if (magazine == null)
		{
			return false;
		}
		bool flag = false;
		try
		{
			flag = entity.ND_IsGun();
		}
		catch
		{
			flag = false;
		}
		return !flag;
	}

	public static bool ND_IsGun(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<Gun>() != null;
	}

	public static bool ND_IsMelee(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<StabSlash>() != null;
	}

	public static bool ND_IsNPC(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<AIBrain>() != null;
	}

	public static Gun ND_GetGun(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<Gun>();
	}

	public static StabSlash ND_GetMelee(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<StabSlash>();
	}

	public static AIBrain ND_GetNPCAIBrain(this NetworkEntity entity)
	{
		return entity?.ND_GetMarrowEntity()?.gameObject?.GetComponent<AIBrain>();
	}

	public static MarrowEntity ND_GetMarrowEntity(this NetworkEntity entity)
	{
		return entity?.GetExtender<IMarrowEntityExtender>()?.MarrowEntity;
	}
}
