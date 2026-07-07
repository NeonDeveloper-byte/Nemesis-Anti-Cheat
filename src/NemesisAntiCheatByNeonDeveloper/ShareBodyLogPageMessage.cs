using System;
using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class ShareBodyLogPageMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 3f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.sharebodylogpagenow || TimeReferences.TimeSinceStartup - _timeOfRequest <= 3f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		ShareBodyLogPageData data = received.ReadData<ShareBodyLogPageData>();
		if (data.smallId != received.Sender.Value)
		{
			return;
		}
		NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
		string text = Main.CleanedNAME(player);
		if (player == null)
		{
			return;
		}
		Main.NotificationNowAlways("Fusion Protector", text + " Sent You [" + data.TitleOfPreset + " - Bodylog Page] Accept To GET!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
		{
			if (!Main.IsBarcodeInGame(data.Slot1))
			{
				Main.DownloadModIOMod(data._modIoID1, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot2))
			{
				Main.DownloadModIOMod(data._modIoID2, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot3))
			{
				Main.DownloadModIOMod(data._modIoID3, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot4))
			{
				Main.DownloadModIOMod(data._modIoID4, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot5))
			{
				Main.DownloadModIOMod(data._modIoID5, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot6))
			{
				Main.DownloadModIOMod(data._modIoID6, noti: false);
			}
			bool flag = false;
			foreach (Main.BodyLogPage bodyLogPage in Main.BodyLogPage.BodyLogPages)
			{
				if (string.Equals(bodyLogPage.TitleOfPreset, data.TitleOfPreset, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Main.BodyLogPage.BodyLogPages.Add(new Main.BodyLogPage(data.TitleOfPreset, data.Slot1, data.Slot2, data.Slot3, data.Slot4, data.Slot5, data.Slot6, data._modIoID1, data._modIoID2, data._modIoID3, data._modIoID4, data._modIoID5, data._modIoID6));
				Main.BodyLogPage.SavePresets();
				Main.NotificationNow("Fusion Protector", "Added Preset " + data.TitleOfPreset + "!", NotificationType.SUCCESS, 2.5f);
			}
			else
			{
				Main.NotificationNow("Fusion Protector", "This Preset Name Exists Already!", NotificationType.WARNING, 2.5f);
			}
		});
	}
}
