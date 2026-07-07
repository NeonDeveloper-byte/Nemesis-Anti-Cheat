using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.VRMK;
using LabFusion.Data;
using LabFusion.Entities;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class NetworkPlayerExtensions
{
	public static bool IsMe(this NetworkPlayer player)
	{
		return player.ND_SteamID() == Main.SteamIdYours();
	}

	public static PullCordDevice ND_PlayersBodyLog(this NetworkPlayer player)
	{
		Transform transform = player?.ND_PlayersPhysicsRig()?.m_elbowRt?.Find("BodyLogSlot/BodyLog");
		if (transform != null)
		{
			PullCordDevice component = transform.GetComponent<PullCordDevice>();
			if (component != null)
			{
				return component;
			}
		}
		Transform transform2 = player?.ND_PlayersPhysicsRig()?.m_elbowLf?.Find("BodyLogSlot/BodyLog");
		if (transform2 != null)
		{
			PullCordDevice component2 = transform2.GetComponent<PullCordDevice>();
			if (component2 != null)
			{
				return component2;
			}
		}
		return null;
	}

	public static bool ND_IsGrabbingNPC(this NetworkPlayer player, Main.WhichHand hand)
	{
		if (1 == 0)
		{
		}
		bool result = hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<AIBrain>() != null, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<AIBrain>() != null, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<AIBrain>() != null || player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<AIBrain>() != null, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static PuppetMaster ND_GetGrabbedPuppetMaster(this NetworkPlayer player, Main.WhichHand hand)
	{
		return player?.ND_GetMarrowEntityInHand(hand)?.ND_GetNPCAIBrain()?.puppetMaster;
	}

	public static bool ND_IsGrabbingSelf(this NetworkPlayer player, Main.WhichHand Hand)
	{
		if (1 == 0)
		{
		}
		bool result = Hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager || player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == player?.RigRefs?.RigManager, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static bool ND_IsGrabbingAnyThing(this NetworkPlayer player, Main.WhichHand Hand)
	{
		if (1 == 0)
		{
		}
		bool result = Hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left) != null, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right) != null, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left) != null || player?.ND_GetObjectInHand(Main.WhichHand.Right) != null, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static MarrowEntity ND_GetMarrowEntityInHand(this NetworkPlayer player, Main.WhichHand Hand)
	{
		if (1 == 0)
		{
		}
		MarrowEntity result = Hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.gameObject?.transform?.root?.gameObject?.GetComponent<MarrowEntity>(), 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.gameObject?.transform?.root?.gameObject?.GetComponent<MarrowEntity>(), 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static GameObject ND_GetObjectInHand(this NetworkPlayer player, Main.WhichHand Hand)
	{
		if (1 == 0)
		{
		}
		GameObject result = Hand switch
		{
			Main.WhichHand.Left => player?.ND_GetHand(Main.WhichHand.Left)?.m_CurrentAttachedGO, 
			Main.WhichHand.Right => player?.ND_GetHand(Main.WhichHand.Right)?.m_CurrentAttachedGO, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static NetworkPlayer ND_GrabbedPlayer(this NetworkPlayer player, Main.WhichHand hand)
	{
		if (1 == 0)
		{
		}
		Hand hand2 = hand switch
		{
			Main.WhichHand.Left => player?.ND_GetHand(Main.WhichHand.Left), 
			Main.WhichHand.Right => player?.ND_GetHand(Main.WhichHand.Right), 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		Hand hand3 = hand2;
		if (hand3?.m_CurrentAttachedGO == null)
		{
			return null;
		}
		RigManager rigManager = hand3.m_CurrentAttachedGO.transform?.root?.GetComponent<RigManager>();
		if (rigManager == null)
		{
			return null;
		}
		NetworkPlayer player2;
		return NetworkPlayerManager.TryGetPlayer(rigManager, out player2) ? player2 : null;
	}

	public static bool ND_IsGrabbingAnyNetPlayer(this NetworkPlayer player, Main.WhichHand Hand)
	{
		if (1 == 0)
		{
		}
		bool result = Hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform.root.GetComponent<RigManager>() != null, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform.root.GetComponent<RigManager>() != null, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform.root.GetComponent<RigManager>() != null || player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform.root.GetComponent<RigManager>() != null, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static bool ND_IsGrabbingYou(this NetworkPlayer player, Main.WhichHand hand)
	{
		if (player?.ND_PlayersPhysicsRig() == null || Player.RigManager == null)
		{
			return false;
		}
		if (1 == 0)
		{
		}
		bool result = hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager || player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<RigManager>() == Player.RigManager, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static Transform ND_PlayersHead(this NetworkPlayer player)
	{
		return player?.ND_PlayersPhysicsRig()?.m_head;
	}

	public static PhysicsRig ND_PlayersPhysicsRig(this NetworkPlayer player)
	{
		return player?.RigRefs?.RigManager?.physicsRig;
	}

	public static SerializedAvatarStats ND_SerializedAvatarStats(this NetworkPlayer player)
	{
		Il2CppSLZ.VRMK.Avatar avatar = player?.RigRefs?.RigManager?.avatar;
		if (avatar == null)
		{
			return null;
		}
		return new SerializedAvatarStats(avatar);
	}

	public static string ND_PlayersAvatarBarcodeID(this NetworkPlayer player)
	{
		return player?.RigRefs?.RigManager?.AvatarCrate?.Barcode?.ID ?? "NULL";
	}

	public static byte ND_SmallID(this NetworkPlayer player)
	{
		return (byte)(((int?)player?.PlayerID?.SmallID) ?? (-1));
	}

	public static ulong ND_SteamID(this NetworkPlayer player)
	{
		return (player?.PlayerID?.PlatformID).GetValueOrDefault();
	}

	public static int ND_AvatarMODIOID(this NetworkPlayer player)
	{
		return (player?.PlayerID?.Metadata?.AvatarModID?.GetValue()).GetValueOrDefault();
	}

	public static string ND_Username(this NetworkPlayer player)
	{
		return player?.PlayerID?.Metadata?.Username?.GetValue() ?? "NULL";
	}

	public static string ND_Nickname(this NetworkPlayer player)
	{
		return player?.PlayerID?.Metadata?.Nickname?.GetValue() ?? "NULL";
	}

	public static string ND_Description(this NetworkPlayer player)
	{
		return player?.PlayerID?.Metadata?.Description?.GetValue() ?? "NULL";
	}

	public static string ND_PermissionLevel(this NetworkPlayer player)
	{
		return player?.PlayerID?.Metadata?.PermissionLevel?.GetValue() ?? "NULL";
	}

	public static Hand ND_GetHand(this NetworkPlayer player, Main.WhichHand hand)
	{
		if (player?.ND_PlayersPhysicsRig() == null)
		{
			return null;
		}
		if (1 == 0)
		{
		}
		Hand result = hand switch
		{
			Main.WhichHand.Left => player.ND_PlayersPhysicsRig().leftHand, 
			Main.WhichHand.Right => player.ND_PlayersPhysicsRig().rightHand, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static bool ND_IsHoldingBarcode(this NetworkPlayer player, Main.WhichHand hand, string barcode)
	{
		if (player?.ND_PlayersPhysicsRig() == null)
		{
			return false;
		}
		if (1 == 0)
		{
		}
		bool result = hand switch
		{
			Main.WhichHand.Left => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<MarrowEntity>()?.ND_GetBarcodeID() == barcode, 
			Main.WhichHand.Right => player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<MarrowEntity>()?.ND_GetBarcodeID() == barcode, 
			Main.WhichHand.Both => player?.ND_GetObjectInHand(Main.WhichHand.Left)?.transform?.root?.GetComponent<MarrowEntity>()?.ND_GetBarcodeID() == barcode || player?.ND_GetObjectInHand(Main.WhichHand.Right)?.transform?.root?.GetComponent<MarrowEntity>()?.ND_GetBarcodeID() == barcode, 
			_ => false, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
