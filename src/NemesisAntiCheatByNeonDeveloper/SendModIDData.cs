using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendModIDData : INetSerializable
{
	public byte smallId;

	public int modid;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref modid);
	}

	public static SendModIDData Create(byte smallId, int modcid)
	{
		return new SendModIDData
		{
			smallId = smallId,
			modid = modcid
		};
	}
}
