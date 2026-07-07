using LabFusion.Network;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;
using LabFusion.Utilities;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

[Net.SkipHandleWhileLoading]
public class SendNotificationMessage : ModuleMessageHandler
{
	public const float _requestCooldown = 5f;

	public static float _timeOfRequest = -1000f;

	protected override void OnHandleMessage(ReceivedMessage received)
	{
		if (Main.playermessaging && !(TimeReferences.TimeSinceStartup - _timeOfRequest <= 5f))
		{
			_timeOfRequest = TimeReferences.TimeSinceStartup;
			SendNotificationData sendNotificationData = received.ReadData<SendNotificationData>();
			byte value = received.Sender.Value;
			if (sendNotificationData.smallId == received.Sender.Value)
			{
				Notifier.Send(new Notification
				{
					Title = sendNotificationData.title,
					Message = new NotificationText(sendNotificationData.messagedata, Color.yellow),
					SaveToMenu = true,
					ShowPopup = true,
					PopupLength = sendNotificationData.length,
					Type = NotificationType.WARNING,
					OnAccepted = delegate
					{
					}
				});
			}
		}
	}
}
