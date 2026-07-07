using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.SDK.Points;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendBitMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 3f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.bitsending || TimeReferences.TimeSinceStartup - _timeOfRequest <= 3f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		SendBitData data = received.ReadData<SendBitData>();
		if (data.smallId != received.Sender.Value)
		{
			return;
		}
		NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
		string value = Main.CleanedNAME(player);
		if (player != null)
		{
			Main.NotificationNowAlways("Fusion Protector", $"{value} Sent You [{data.bits}] Bits Accept To GET!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
			{
				PointItemManager.RewardBits(data.bits);
			});
		}
	}
}
