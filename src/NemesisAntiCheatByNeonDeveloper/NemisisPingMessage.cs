using System;
using System.Collections;
using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using MelonLoader;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class ProtectorPingMessage : ModuleMessageHandler
{
	protected override void OnHandleMessage(ReceivedMessage received)
	{
		ProtectorPingData protectorPingData = received.ReadData<ProtectorPingData>();
		if (protectorPingData.smallId != received.Sender.Value || !NetworkPlayerManager.TryGetPlayer(protectorPingData.smallId, out var player))
		{
			return;
		}
		string message = Main.CleanedNAME(player) + "\nI'm Using v" + protectorPingData.versionoffusionprotector + "!";
		Main.NotificationNow("Fusion Protector", message, NotificationType.SUCCESS, 5f, showtitle: true, savetomenu: true, delegate
		{
			Main.CheckSteamID(player.ND_SteamID());
		});
	}
}
