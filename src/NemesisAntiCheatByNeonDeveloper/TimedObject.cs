using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NemesisAntiCheatByNeonDeveloper;

internal class TimedObject
{
	public static Dictionary<object, TimedObject> ActiveTimers = new Dictionary<object, TimedObject>();

	public object Value;

	private readonly Stopwatch Timer;

	private readonly TimeSpan Duration;

	public TimedObject(object value, int minutes)
	{
		Value = value;
		Duration = TimeSpan.FromMinutes(minutes);
		Timer = Stopwatch.StartNew();
		ActiveTimers[value] = this;
	}

	public bool IsExpired()
	{
		bool flag = Timer.Elapsed >= Duration;
		if (flag)
		{
			ActiveTimers.Remove(Value);
		}
		return flag;
	}

	public TimeSpan TimeLeft()
	{
		return Duration - Timer.Elapsed;
	}
}
