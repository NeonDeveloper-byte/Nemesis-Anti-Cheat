using System;
using LabFusion.Network.Serialization;

namespace NemesisAntiCheatByNeonDeveloper;

public class OwnerServerSettingData : INetSerializable
{
	public enum ValueType
	{
		Bool,
		Int,
		Float,
		String
	}

	public byte smallId;

	public string serversettings;

	public ValueType valueType;

	public bool boolValue;

	public int intValue;

	public float floatValue;

	public string stringValue;

	public void Serialize(INetSerializer serializer)
	{
		serializer.SerializeValue(ref smallId);
		serializer.SerializeValue(ref serversettings);
		serializer.SerializeValue(ref valueType);
		serializer.SerializeValue(ref boolValue);
		serializer.SerializeValue(ref intValue);
		serializer.SerializeValue(ref floatValue);
		serializer.SerializeValue(ref stringValue);
	}

	public static OwnerServerSettingData Create(byte smallId, string serversetting, object valuenow)
	{
		OwnerServerSettingData ownerServerSettingData = new OwnerServerSettingData
		{
			smallId = smallId,
			serversettings = serversetting
		};
		if (valuenow == null)
		{
			throw new Exception("Value cannot be null.");
		}
		Type type = valuenow.GetType();
		if (type.IsEnum)
		{
			ownerServerSettingData.valueType = ValueType.Int;
			ownerServerSettingData.intValue = Convert.ToInt32(valuenow);
			return ownerServerSettingData;
		}
		if (!(valuenow is bool flag))
		{
			if (!(valuenow is int num))
			{
				if (!(valuenow is float num2))
				{
					if (!(valuenow is string text))
					{
						throw new Exception($"Unsupported value type: {valuenow.GetType()}");
					}
					ownerServerSettingData.valueType = ValueType.String;
					ownerServerSettingData.stringValue = text;
				}
				else
				{
					ownerServerSettingData.valueType = ValueType.Float;
					ownerServerSettingData.floatValue = num2;
				}
			}
			else
			{
				ownerServerSettingData.valueType = ValueType.Int;
				ownerServerSettingData.intValue = num;
			}
		}
		else
		{
			ownerServerSettingData.valueType = ValueType.Bool;
			ownerServerSettingData.boolValue = flag;
		}
		return ownerServerSettingData;
	}

	public bool GetBool()
	{
		return valueType == ValueType.Bool && boolValue;
	}

	public int GetInt()
	{
		return (valueType == ValueType.Int) ? intValue : 0;
	}

	public float GetFloat()
	{
		return (valueType == ValueType.Float) ? floatValue : 0f;
	}

	public string GetString()
	{
		return (valueType == ValueType.String) ? stringValue : null;
	}
}
