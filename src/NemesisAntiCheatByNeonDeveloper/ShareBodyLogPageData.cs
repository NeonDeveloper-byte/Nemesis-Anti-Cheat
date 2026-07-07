using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class ShareBodyLogPageData : INetSerializable
{
	public byte smallId;

	internal string TitleOfPreset;

	internal string Slot1;

	internal string Slot2;

	internal string Slot3;

	internal string Slot4;

	internal string Slot5;

	internal string Slot6;

	internal int _modIoID1;

	internal int _modIoID2;

	internal int _modIoID3;

	internal int _modIoID4;

	internal int _modIoID5;

	internal int _modIoID6;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref TitleOfPreset);
		serializer.SerializeValue(ref Slot1);
		serializer.SerializeValue(ref Slot2);
		serializer.SerializeValue(ref Slot3);
		serializer.SerializeValue(ref Slot4);
		serializer.SerializeValue(ref Slot5);
		serializer.SerializeValue(ref Slot6);
		serializer.SerializeValue(ref _modIoID1);
		serializer.SerializeValue(ref _modIoID2);
		serializer.SerializeValue(ref _modIoID3);
		serializer.SerializeValue(ref _modIoID4);
		serializer.SerializeValue(ref _modIoID5);
		serializer.SerializeValue(ref _modIoID6);
	}

	public static ShareBodyLogPageData Create(byte smallId, string titleOfPreset, string slot1, string slot2, string slot3, string slot4, string slot5, string slot6, int modIoID1, int modIoID2, int modIoID3, int modIoID4, int modIoID5, int modIoID6)
	{
		return new ShareBodyLogPageData
		{
			smallId = smallId,
			TitleOfPreset = titleOfPreset,
			Slot1 = slot1,
			Slot2 = slot2,
			Slot3 = slot3,
			Slot4 = slot4,
			Slot5 = slot5,
			Slot6 = slot6,
			_modIoID1 = modIoID1,
			_modIoID2 = modIoID2,
			_modIoID3 = modIoID3,
			_modIoID4 = modIoID4,
			_modIoID5 = modIoID5,
			_modIoID6 = modIoID6
		};
	}
}
