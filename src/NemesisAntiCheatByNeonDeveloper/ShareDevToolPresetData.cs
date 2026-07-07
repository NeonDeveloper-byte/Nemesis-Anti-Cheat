using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class ShareDevToolPresetData : INetSerializable
{
	public byte smallId;

	internal string TitleOfPreset;

	internal string Slot1Barcode;

	internal int Slot1ModID;

	internal string Slot1BarCodeName;

	internal bool Slot1LocalSpawn;

	internal string Slot2Barcode;

	internal int Slot2ModID;

	internal string Slot2BarCodeName;

	internal bool Slot2LocalSpawn;

	internal string Slot3Barcode;

	internal int Slot3ModID;

	internal string Slot3BarCodeName;

	internal bool Slot3LocalSpawn;

	internal string Slot4Barcode;

	internal int Slot4ModID;

	internal string Slot4BarCodeName;

	internal bool Slot4LocalSpawn;

	internal string Slot5Barcode;

	internal int Slot5ModID;

	internal string Slot5BarCodeName;

	internal bool Slot5LocalSpawn;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref TitleOfPreset);
		serializer.SerializeValue(ref Slot1Barcode);
		serializer.SerializeValue(ref Slot1ModID);
		serializer.SerializeValue(ref Slot1BarCodeName);
		serializer.SerializeValue(ref Slot1LocalSpawn);
		serializer.SerializeValue(ref Slot2Barcode);
		serializer.SerializeValue(ref Slot2ModID);
		serializer.SerializeValue(ref Slot2BarCodeName);
		serializer.SerializeValue(ref Slot2LocalSpawn);
		serializer.SerializeValue(ref Slot3Barcode);
		serializer.SerializeValue(ref Slot3ModID);
		serializer.SerializeValue(ref Slot3BarCodeName);
		serializer.SerializeValue(ref Slot3LocalSpawn);
		serializer.SerializeValue(ref Slot4Barcode);
		serializer.SerializeValue(ref Slot4ModID);
		serializer.SerializeValue(ref Slot4BarCodeName);
		serializer.SerializeValue(ref Slot4LocalSpawn);
		serializer.SerializeValue(ref Slot5Barcode);
		serializer.SerializeValue(ref Slot5ModID);
		serializer.SerializeValue(ref Slot5BarCodeName);
		serializer.SerializeValue(ref Slot5LocalSpawn);
	}

	public static ShareDevToolPresetData Create(byte smallId, string titleOfPreset, string slot1Barcode, int slot1ModID, string slot1Name, bool slot1LocalSpawn, string slot2Barcode, int slot2ModID, string slot2Name, bool slot2LocalSpawn, string slot3Barcode, int slot3ModID, string slot3Name, bool slot3LocalSpawn, string slot4Barcode, int slot4ModID, string slot4Name, bool slot4LocalSpawn, string slot5Barcode, int slot5ModID, string slot5Name, bool slot5LocalSpawn)
	{
		return new ShareDevToolPresetData
		{
			smallId = smallId,
			TitleOfPreset = titleOfPreset,
			Slot1Barcode = slot1Barcode,
			Slot1ModID = slot1ModID,
			Slot1BarCodeName = slot1Name,
			Slot1LocalSpawn = slot1LocalSpawn,
			Slot2Barcode = slot2Barcode,
			Slot2ModID = slot2ModID,
			Slot2BarCodeName = slot2Name,
			Slot2LocalSpawn = slot2LocalSpawn,
			Slot3Barcode = slot3Barcode,
			Slot3ModID = slot3ModID,
			Slot3BarCodeName = slot3Name,
			Slot3LocalSpawn = slot3LocalSpawn,
			Slot4Barcode = slot4Barcode,
			Slot4ModID = slot4ModID,
			Slot4BarCodeName = slot4Name,
			Slot4LocalSpawn = slot4LocalSpawn,
			Slot5Barcode = slot5Barcode,
			Slot5ModID = slot5ModID,
			Slot5BarCodeName = slot5Name,
			Slot5LocalSpawn = slot5LocalSpawn
		};
	}
}
