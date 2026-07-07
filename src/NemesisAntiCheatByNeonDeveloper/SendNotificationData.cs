using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendNotificationData : INetSerializable
{
	public byte smallId;

	public string messagedata;

	public string title;

	public float length;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref messagedata);
		serializer.SerializeValue(ref title);
		serializer.SerializeValue(ref length);
	}

	public static SendNotificationData Create(byte smallId, string messagedatax, string title, float length)
	{
		return new SendNotificationData
		{
			smallId = smallId,
			messagedata = messagedatax,
			title = title,
			length = length
		};
	}
}
