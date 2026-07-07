using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class SendBodyLogData : INetSerializable
{
	public byte smallId;

	public string BodyLogAvatar1;

	public string BodyLogAvatar2;

	public string BodyLogAvatar3;

	public string BodyLogAvatar4;

	public string BodyLogAvatar5;

	public string BodyLogAvatar6;

	public int BodyLogAvatar1ModID;

	public int BodyLogAvatar2ModID;

	public int BodyLogAvatar3ModID;

	public int BodyLogAvatar4ModID;

	public int BodyLogAvatar5ModID;

	public int BodyLogAvatar6ModID;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref BodyLogAvatar1);
		serializer.SerializeValue(ref BodyLogAvatar2);
		serializer.SerializeValue(ref BodyLogAvatar3);
		serializer.SerializeValue(ref BodyLogAvatar4);
		serializer.SerializeValue(ref BodyLogAvatar5);
		serializer.SerializeValue(ref BodyLogAvatar6);
		serializer.SerializeValue(ref BodyLogAvatar1ModID);
		serializer.SerializeValue(ref BodyLogAvatar2ModID);
		serializer.SerializeValue(ref BodyLogAvatar3ModID);
		serializer.SerializeValue(ref BodyLogAvatar4ModID);
		serializer.SerializeValue(ref BodyLogAvatar5ModID);
		serializer.SerializeValue(ref BodyLogAvatar6ModID);
	}

	public static SendBodyLogData Create(byte smallId, string slot1, string slot2, string slot3, string slot4, string slot5, string slot6, int slot1id, int slot2id, int slot3id, int slot4id, int slot5id, int slot6id)
	{
		return new SendBodyLogData
		{
			smallId = smallId,
			BodyLogAvatar1 = slot1,
			BodyLogAvatar2 = slot2,
			BodyLogAvatar3 = slot3,
			BodyLogAvatar4 = slot4,
			BodyLogAvatar5 = slot5,
			BodyLogAvatar6 = slot6,
			BodyLogAvatar1ModID = slot1id,
			BodyLogAvatar2ModID = slot2id,
			BodyLogAvatar3ModID = slot3id,
			BodyLogAvatar4ModID = slot4id,
			BodyLogAvatar5ModID = slot5id,
			BodyLogAvatar6ModID = slot6id
		};
	}
}
