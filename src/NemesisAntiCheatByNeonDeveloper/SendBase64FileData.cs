using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendBase64FileData : INetSerializable
{
	public byte smallId;

	public string Link;

	public string FileName;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref Link);
		serializer.SerializeValue(ref FileName);
	}

	public static SendBase64FileData Create(byte smallId, string kink, string filename)
	{
		return new SendBase64FileData
		{
			smallId = smallId,
			Link = kink,
			FileName = filename
		};
	}
}
