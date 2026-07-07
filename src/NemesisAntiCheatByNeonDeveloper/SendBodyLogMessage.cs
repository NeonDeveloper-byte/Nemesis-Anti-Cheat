using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendBodyLogMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 5f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.bodylogsending || TimeReferences.TimeSinceStartup - _timeOfRequest <= 5f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		SendBodyLogData data = received.ReadData<SendBodyLogData>();
		if (data.smallId != received.Sender.Value)
		{
			return;
		}
		NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
		string stringplayername = Main.CleanedNAME(player);
		if (player == null)
		{
			return;
		}
		Main.NotificationNowAlways("Fusion Protector", stringplayername + " Sent There Bodylog To You!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
		{
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar1))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar1ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar2))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar2ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar3))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar3ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar4))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar4ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar5))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar5ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.BodyLogAvatar6))
			{
				Main.DownloadModIOMod(data.BodyLogAvatar6ModID, noti: false);
			}
			Main.ChangeBodyLogAvatarSlot(1, data.BodyLogAvatar1, notification: false);
			Main.ChangeBodyLogAvatarSlot(2, data.BodyLogAvatar2, notification: false);
			Main.ChangeBodyLogAvatarSlot(3, data.BodyLogAvatar3, notification: false);
			Main.ChangeBodyLogAvatarSlot(4, data.BodyLogAvatar4, notification: false);
			Main.ChangeBodyLogAvatarSlot(5, data.BodyLogAvatar5, notification: false);
			Main.ChangeBodyLogAvatarSlot(6, data.BodyLogAvatar6, notification: false);
			Main.NotificationNowAlways("Fusion Protector", "Applied " + stringplayername + " Bodylog To Yours!", NotificationType.SUCCESS, 3.5f);
		});
	}
}
