using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoneLib.BoneMenu;
using UnityEngine;

namespace NemesisAntiCheatByNeonDeveloper;

internal static class PageEx
{
	public static List<(string entry, string page, string name, float value)> floatslogged = new List<(string, string, string, float)>();

	public static List<(string entry, string page, string name, int value)> intslogged = new List<(string, string, string, int)>();

	public static List<(string entry, string page, string name, Enum value)> enumvaluelogged = new List<(string, string, string, Enum)>();

	public static List<(string entry, string page, string name, string value)> stringslogged = new List<(string, string, string, string)>();

	public static List<(string entry, string page, string name, bool value)> boolslogged = new List<(string, string, string, bool)>();

	public static BoolElement Logsettings(this Page Page, string name, Color color, ref bool startingBool, Action<bool> ActionNow)
	{
		string item = $"Page({Page.Name})Option={name}:{startingBool}";
		boolslogged.Add((item, Page.Name, name, startingBool));
		boolslogged = (from x in boolslogged
			group x by (page: x.page, name: x.name) into g
			select g.Last()).ToList();
		if (File.Exists(Main.ProtectorSettings))
		{
			string text = File.ReadAllLines(Main.ProtectorSettings).FirstOrDefault((string l) => l.StartsWith($"Page({Page.Name})Option={name}:"));
			if (!string.IsNullOrEmpty(text))
			{
				string item2 = Main.ParseLine(text).right;
				if (bool.TryParse(item2, out var result))
				{
					startingBool = result;
					int num = boolslogged.FindIndex(((string entry, string page, string name, bool value) x) => x.page == Page.Name && x.name == name);
					if (num != -1)
					{
						boolslogged[num] = ($"Page({Page.Name})Option={name}:{result}", Page.Name, name, result);
					}
				}
			}
		}
		return Page.CreateBool(name, color, startingBool, delegate(bool newVal)
		{
			boolslogged.RemoveAll(((string entry, string page, string name, bool value) x) => x.page == Page.Name && x.name == name);
			ActionNow?.Invoke(newVal);
			boolslogged.Add(($"Page({Page.Name})Option={name}:{newVal}", Page.Name, name, newVal));
			if (Main.togglesavesbool && File.Exists(Main.ProtectorSettings))
			{
				Main.ManuallySave(notify: false);
			}
		});
	}

	public static FloatElement Logsettingsfloat(this Page Page, string name, Color color, ref float startingValue, float increment, float minValue, float maxValue, Action<float> ActionNow)
	{
		string item = $"Page({Page.Name})Option={name}:{startingValue}";
		floatslogged.Add((item, Page.Name, name, startingValue));
		floatslogged = (from x in floatslogged
			group x by (page: x.page, name: x.name) into g
			select g.Last()).ToList();
		if (File.Exists(Main.ProtectorSettings))
		{
			string text = File.ReadAllLines(Main.ProtectorSettings).FirstOrDefault((string l) => l.StartsWith($"Page({Page.Name})Option={name}:"));
			if (!string.IsNullOrEmpty(text))
			{
				string item2 = Main.ParseLine(text).right;
				if (float.TryParse(item2, out var result))
				{
					startingValue = result;
					int num = floatslogged.FindIndex(((string entry, string page, string name, float value) x) => x.page == Page.Name && x.name == name);
					if (num != -1)
					{
						floatslogged[num] = ($"Page({Page.Name})Option={name}:{result}", Page.Name, name, result);
					}
				}
			}
		}
		return Page.CreateFloat(name, color, startingValue, increment, minValue, maxValue, delegate(float newVal)
		{
			floatslogged.RemoveAll(((string entry, string page, string name, float value) x) => x.page == Page.Name && x.name == name);
			ActionNow?.Invoke(newVal);
			floatslogged.Add(($"Page({Page.Name})Option={name}:{newVal}", Page.Name, name, newVal));
			if (Main.togglesavesbool && File.Exists(Main.ProtectorSettings))
			{
				Main.ManuallySave(notify: false);
			}
		});
	}

	public static IntElement Logsettingsint(this Page Page, string name, Color color, ref int startingValue, int increment, int minValue, int maxValue, Action<int> ActionNow)
	{
		string item = $"Page({Page.Name})Option={name}:{startingValue}";
		intslogged.Add((item, Page.Name, name, startingValue));
		intslogged = (from x in intslogged
			group x by (page: x.page, name: x.name) into g
			select g.Last()).ToList();
		if (File.Exists(Main.ProtectorSettings))
		{
			string text = File.ReadAllLines(Main.ProtectorSettings).FirstOrDefault((string l) => l.StartsWith($"Page({Page.Name})Option={name}:"));
			if (!string.IsNullOrEmpty(text))
			{
				string item2 = Main.ParseLine(text).right;
				if (int.TryParse(item2, out var result))
				{
					startingValue = result;
					int num = intslogged.FindIndex(((string entry, string page, string name, int value) x) => x.page == Page.Name && x.name == name);
					if (num != -1)
					{
						intslogged[num] = ($"Page({Page.Name})Option={name}:{result}", Page.Name, name, result);
					}
				}
			}
		}
		return Page.CreateInt(name, color, startingValue, increment, minValue, maxValue, delegate(int newVal)
		{
			intslogged.RemoveAll(((string entry, string page, string name, int value) x) => x.page == Page.Name && x.name == name);
			ActionNow?.Invoke(newVal);
			intslogged.Add(($"Page({Page.Name})Option={name}:{newVal}", Page.Name, name, newVal));
			if (Main.togglesavesbool && File.Exists(Main.ProtectorSettings))
			{
				Main.ManuallySave(notify: false);
			}
		});
	}

	public static EnumElement LogsettingsEnum<T>(this Page Page, string name, Color color, ref T startingValue, Action<T> ActionNow) where T : Enum
	{
		string item = $"Page({Page.Name})Option={name}:{startingValue}";
		enumvaluelogged.Add((item, Page.Name, name, startingValue));
		enumvaluelogged = (from x in enumvaluelogged
			group x by (page: x.page, name: x.name) into g
			select g.Last()).ToList();
		if (File.Exists(Main.ProtectorSettings))
		{
			string text = File.ReadAllLines(Main.ProtectorSettings).FirstOrDefault((string l) => l.StartsWith($"Page({Page.Name})Option={name}:"));
			if (!string.IsNullOrEmpty(text))
			{
				string item2 = Main.ParseLine(text).right;
				try
				{
					T val = (startingValue = (T)Enum.Parse(typeof(T), item2, ignoreCase: true));
					int num = enumvaluelogged.FindIndex(((string entry, string page, string name, Enum value) x) => x.page == Page.Name && x.name == name);
					if (num != -1)
					{
						enumvaluelogged[num] = ($"Page({Page.Name})Option={name}:{val}", Page.Name, name, val);
					}
				}
				catch
				{
				}
			}
		}
		return Page.CreateEnum(name, color, startingValue, delegate(Enum newVal)
		{
			T val2 = (T)newVal;
			enumvaluelogged.RemoveAll(((string entry, string page, string name, Enum value) x) => x.page == Page.Name && x.name == name);
			ActionNow?.Invoke(val2);
			enumvaluelogged.Add(($"Page({Page.Name})Option={name}:{val2}", Page.Name, name, val2));
			if (Main.togglesavesbool && File.Exists(Main.ProtectorSettings))
			{
				Main.ManuallySave(notify: false);
			}
		});
	}

	public static StringElement LogsettingsString(this Page Page, string name, Color color, ref string startingValue, Action<string> ActionNow)
	{
		if (File.Exists(Main.ProtectorSettings))
		{
			string text = File.ReadAllLines(Main.ProtectorSettings).FirstOrDefault((string l) => l.StartsWith($"Page({Page.Name})Option={name}:"));
			if (!string.IsNullOrEmpty(text))
			{
				startingValue = text.Substring(text.IndexOf(':') + 1);
			}
		}
		stringslogged.RemoveAll(((string entry, string page, string name, string value) x) => x.page == Page.Name && x.name == name);
		stringslogged.Add(($"Page({Page.Name})Option={name}:{startingValue}", Page.Name, name, startingValue));
		return Page.CreateString(name, color, startingValue, delegate(string newVal)
		{
			stringslogged.RemoveAll(((string entry, string page, string name, string value) x) => x.page == Page.Name && x.name == name);
			ActionNow?.Invoke(newVal);
			stringslogged.Add(($"Page({Page.Name})Option={name}:{newVal}", Page.Name, name, newVal));
			if (Main.togglesavesbool)
			{
				Main.ManuallySave(notify: false);
			}
		});
	}
}
