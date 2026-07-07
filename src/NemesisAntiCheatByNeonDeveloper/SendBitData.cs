using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendBitData : INetSerializable
{
	public byte smallId;

	public int bits;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref bits);
	}

	public static SendBitData Create(byte smallId, int bits)
	{
		return new SendBitData
		{
			smallId = smallId,
			bits = bits
		};
	}
}
