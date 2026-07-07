using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class ShareDevToolPresetMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 3f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.sharedevtoolpresets || TimeReferences.TimeSinceStartup - _timeOfRequest <= 3f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		ShareDevToolPresetData data = received.ReadData<ShareDevToolPresetData>();
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
		Main.NotificationNowAlways("Fusion Protector", text + " Sent You [" + data.TitleOfPreset + " - DevToolPreset] Accept To GET!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
		{
			if (!Main.IsBarcodeInGame(data.Slot1Barcode))
			{
				Main.DownloadModIOMod(data.Slot1ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot2Barcode))
			{
				Main.DownloadModIOMod(data.Slot2ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot3Barcode))
			{
				Main.DownloadModIOMod(data.Slot3ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot4Barcode))
			{
				Main.DownloadModIOMod(data.Slot4ModID, noti: false);
			}
			if (!Main.IsBarcodeInGame(data.Slot5Barcode))
			{
				Main.DownloadModIOMod(data.Slot5ModID, noti: false);
			}
			Main.CreateDevToolPreset(data.TitleOfPreset, new Main.CreateCheatToolsPreset.Item
			{
				SpawnableName = data.Slot1BarCodeName,
				BarcodeId = data.Slot1Barcode,
				LocalSpawn = data.Slot1LocalSpawn,
				ModIoID = data.Slot1ModID
			}, new Main.CreateCheatToolsPreset.Item
			{
				SpawnableName = data.Slot2BarCodeName,
				BarcodeId = data.Slot2Barcode,
				LocalSpawn = data.Slot2LocalSpawn,
				ModIoID = data.Slot2ModID
			}, new Main.CreateCheatToolsPreset.Item
			{
				SpawnableName = data.Slot3BarCodeName,
				BarcodeId = data.Slot3Barcode,
				LocalSpawn = data.Slot3LocalSpawn,
				ModIoID = data.Slot3ModID
			}, new Main.CreateCheatToolsPreset.Item
			{
				SpawnableName = data.Slot4BarCodeName,
				BarcodeId = data.Slot4Barcode,
				LocalSpawn = data.Slot4LocalSpawn,
				ModIoID = data.Slot4ModID
			}, new Main.CreateCheatToolsPreset.Item
			{
				SpawnableName = data.Slot5BarCodeName,
				BarcodeId = data.Slot5Barcode,
				LocalSpawn = data.Slot5LocalSpawn,
				ModIoID = data.Slot5ModID
			});
		});
	}
}
