using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendGameModeOverData : INetSerializable
{
	public byte smallId;

	public string gamemode;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref gamemode);
	}

	public static SendGameModeOverData Create(byte smallId, string gamemodex)
	{
		return new SendGameModeOverData
		{
			smallId = smallId,
			gamemode = gamemodex
		};
	}
}
