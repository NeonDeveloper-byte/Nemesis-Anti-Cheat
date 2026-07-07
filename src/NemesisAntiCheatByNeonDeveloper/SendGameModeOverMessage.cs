using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.SDK.Gamemodes;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendGameModeOverMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 5f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.ownerscanchangegamemode || TimeReferences.TimeSinceStartup - _timeOfRequest <= 5f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		SendGameModeOverData sendGameModeOverData = received.ReadData<SendGameModeOverData>();
		if (sendGameModeOverData.smallId != received.Sender.Value || !NetworkPlayerManager.TryGetPlayer(sendGameModeOverData.smallId, out var player))
		{
			return;
		}
		FusionPermissions.FetchPermissionLevel(player.ND_SteamID(), out var level, out var _);
		if (level != PermissionLevel.OWNER || string.IsNullOrEmpty(sendGameModeOverData.gamemode))
		{
			return;
		}
		if (sendGameModeOverData.gamemode == "sandbox")
		{
			Main.NotificationNow("Fusion Protector", "Gamemode Was Changed By " + Main.CleanedNAME(player) + " To [Sandbox]", NotificationType.WARNING, 5f);
			GamemodeManager.DeselectGamemode();
			return;
		}
		string value = "";
		switch (sendGameModeOverData.gamemode)
		{
		case "Lakatrazz.Deathmatch":
			value = "Deathmatch";
			break;
		case "Lakatrazz.Juggernaut":
			value = "Juggernaut";
			break;
		case "Lakatrazz.Entangled":
			value = "Entangled";
			break;
		case "Lakatrazz.Smash Bones":
			value = "Smash Bones";
			break;
		case "Lakatrazz.Hide And Seek":
			value = "Hide And Seek";
			break;
		case "sandbox":
			value = "Sandbox";
			break;
		case "Lakatrazz.Team Deathmatch":
			value = "Team Deathmatch";
			break;
		}
		if (GamemodeManager.TryGetGamemode(sendGameModeOverData.gamemode, out var gamemode))
		{
			Main.NotificationNow("Fusion Protector", $"Gamemode Was Changed By {Main.CleanedNAME(player)} To [{value}]", NotificationType.WARNING, 5f);
			GamemodeManager.SelectGamemode(gamemode);
		}
	}
}
