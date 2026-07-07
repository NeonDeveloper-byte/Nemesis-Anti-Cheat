using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendModIDMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 5f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.modidsending || TimeReferences.TimeSinceStartup - _timeOfRequest <= 5f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		SendModIDData data = received.ReadData<SendModIDData>();
		if (data.smallId != received.Sender.Value)
		{
			return;
		}
		NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
		string stringplayername = Main.CleanedNAME(player);
		if (player != null)
		{
			Main.NotificationNowAlways("Fusion Protector", $"{stringplayername} Sent A Mod IO Mod For You To Download #{data.modid}!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
			{
				Main.DownloadModIOMod(data.modid);
				Main.NotificationNowAlways("Fusion Protector", "Downloading Mod.IO Mod From " + stringplayername, NotificationType.WARNING, 3.5f);
			});
		}
	}
}
