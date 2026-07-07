using System;
using System.IO;
using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;
using MelonLoader;
using MelonLoader.Utils;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendBase64FileMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 3f;

	public static float _timeOfRequest = -1000f;

	public static string codemodname = "";

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (!Main.base64files || TimeReferences.TimeSinceStartup - _timeOfRequest <= 3f)
		{
			return;
		}
		_timeOfRequest = TimeReferences.TimeSinceStartup;
		SendBase64FileData data = received.ReadData<SendBase64FileData>();
		if (data.smallId != received.Sender.Value)
		{
			return;
		}
		NetworkPlayerManager.TryGetPlayer(received.Sender.Value, out var player);
		string value = Main.CleanedNAME(player);
		if (player == null)
		{
			return;
		}
		Main.NotificationNowAlways("Fusion Protector", $"{value} Sent You [{data.Link} - A Code Mod {data.FileName}] Accept To GET!", NotificationType.SUCCESS, 4f, showtitle: true, savetomenu: true, delegate
		{
			MelonCoroutines.Start(Main.SiteStuff.ReadFromSite(data.Link, delegate(string sitetext)
			{
				if (File.Exists(Path.Combine(MelonEnvironment.ModsDirectory, data.FileName + ".dll")))
				{
					Main.NotificationNowAlways("Fusion Protector", "File " + data.FileName + ".dll EXIST IN YOUR MOD FOLDER ALREADY CAN'T DO THIS!", NotificationType.ERROR, 3.5f);
				}
				else
				{
					byte[] bytes = Convert.FromBase64String(sitetext);
					File.WriteAllBytes(Path.Combine(MelonEnvironment.ModsDirectory, data.FileName + ".dll"), bytes);
					Main.NotificationNowAlways("Fusion Protector", "Installed " + data.FileName + ".dll To Your Mods Folder Restart The Game For Mod To LOAD!", NotificationType.SUCCESS, 3.5f);
				}
			}));
		});
	}
}
