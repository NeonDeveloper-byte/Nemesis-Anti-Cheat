using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class ProtectorPingData : INetSerializable
{
	public string versionoffusionprotector;

	public byte smallId;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref versionoffusionprotector);
	}

	public static ProtectorPingData Create(byte smallId, string versionoffp)
	{
		return new ProtectorPingData
		{
			smallId = smallId,
			versionoffusionprotector = versionoffp
		};
	}
}
