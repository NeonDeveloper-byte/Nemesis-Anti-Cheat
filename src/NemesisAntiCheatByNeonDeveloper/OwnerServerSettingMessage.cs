using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class OwnerServerSettingMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 5f;

	public static float _timeOfRequest = -1000f;

	private static void AlertHost(NetworkPlayer nowPlayer, string settingName, object value)
	{
		Main.NotificationNow("Fusion Protector", $"{Main.CleanedNAME(nowPlayer)}\nEdited Server Setting {settingName} To Value : {value}", NotificationType.ERROR);
	}

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.OWNERSCANCHANGESERVER || TimeReferences.TimeSinceStartup - _timeOfRequest <= 5f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		OwnerServerSettingData ownerServerSettingData = received.ReadData<OwnerServerSettingData>();
		if (ownerServerSettingData.smallId != received.Sender.Value || !NetworkPlayerManager.TryGetPlayer(ownerServerSettingData.smallId, out var player))
		{
			return;
		}
		FusionPermissions.FetchPermissionLevel(player.ND_SteamID(), out var level, out var _);
		if (level == PermissionLevel.OWNER && !string.IsNullOrEmpty(ownerServerSettingData.serversettings))
		{
			bool flag = ownerServerSettingData.GetBool();
			PermissionLevel permissionLevel = (PermissionLevel)ownerServerSettingData.GetInt();
			switch (ownerServerSettingData.serversettings)
			{
			case "NameTags":
				Main.EditFusionPreferences("Server Nametags Enabled", flag);
				AlertHost(player, "NameTags", flag);
				break;
			case "VoiceChat":
				Main.EditFusionPreferences("Server Voicechat Enabled", flag);
				AlertHost(player, "VoiceChat", flag);
				break;
			case "Mortality":
				Main.EditFusionPreferences("Server Mortality", flag);
				AlertHost(player, "Mortality", flag);
				break;
			case "Friendly Fire":
				Main.EditFusionPreferences("Friendly Fire", flag);
				AlertHost(player, "Friendly Fire", flag);
				break;
			case "Knockout":
				Main.EditFusionPreferences("Knockout", flag);
				AlertHost(player, "Knockout", flag);
				break;
			case "Player Constraining":
				Main.EditFusionPreferences("Server Player Constraints Enabled", flag);
				AlertHost(player, "Player Constraining", flag);
				break;
			case "Dev Tools":
				Main.EditFusionPreferences("Dev Tools Allowed", permissionLevel);
				AlertHost(player, "Dev Tools", permissionLevel);
				break;
			case "Constrainer":
				Main.EditFusionPreferences("Constrainer Allowed", permissionLevel);
				AlertHost(player, "Constrainer", permissionLevel);
				break;
			case "Custom Avatars":
				Main.EditFusionPreferences("Custom Avatars Allowed", permissionLevel);
				AlertHost(player, "Custom Avatars", permissionLevel);
				break;
			case "Teleportation":
				Main.EditFusionPreferences("Teleportation", permissionLevel);
				AlertHost(player, "Teleportation", permissionLevel);
				break;
			}
			LobbyInfoManager.PushLobbyUpdate();
		}
	}
}
