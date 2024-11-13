using System;
using System.Collections.Generic;
using System.Text;
using UnityCommon.Runtime.Utility;
using UnityCommon.Singletons;
using UnityCommon.Utilities;
using UnityCommon.Variables;
using UnityEngine;

namespace Based2.TimeManagement.Core
{
	public delegate void DiscreteTimeDeltaListener(long elapsedTimeMilliseconds);

	[DefaultExecutionOrder(-10000)]
	public class TimeManager : SingletonBehaviour<TimeManager>
	{
		[SerializeField]
		private bool m_OfflineProgressionInEditor = false;

		private List<PeriodicInvocation> m_Invocations;
		private Dictionary<int, PeriodicInvocation> m_InvocationsMap;

		[SerializeField]
		private LongVariable m_TimeMilliseconds;
		private long m_TimeMillisecondsPrev;

		private long m_LastSessionEndTimeMilliseconds;

		private long m_UnixMillisecondsPrev;
		private long m_UnixMilliseconds;

		private int m_AvailableInvocationHandle = int.MinValue;

		private TimedAction m_SerializeTimeDataTimer;


		private void Awake()
		{
			if (!SetupInstance())
				return;

			m_Invocations = new List<PeriodicInvocation>();
			m_InvocationsMap = new Dictionary<int, PeriodicInvocation>();

			// Get current unix timestamp
			m_UnixMilliseconds = TimeUtility.UnixMilliseconds;

			// Get last session end unix timestamp
			GetLastKnownUnixTime();

			// Get last session end game timestamp
			//m_TimeMilliseconds = Var.Get<LongVariable>("GameTime");

			// Cache last session end game timestamp 
			m_LastSessionEndTimeMilliseconds = m_TimeMilliseconds.Value;

			// Update game timestamp by adding time passed since last session

			if (!Application.isEditor || m_OfflineProgressionInEditor)
				m_TimeMilliseconds.Value += m_UnixMilliseconds - m_UnixMillisecondsPrev;

			m_TimeMillisecondsPrev = m_TimeMilliseconds.Value;

			m_SerializeTimeDataTimer = new TimedAction(() =>
			{
				SetLastKnownUnixTime();
				Variable.SavePlayerPrefs();
			}, 1f, 1f);
		}

		private void OnDestroy()
		{
		}

		private void Start()
		{
		}

		private void LateUpdate()
		{
			m_UnixMilliseconds = TimeUtility.UnixMilliseconds;
			var frameDelta = m_UnixMilliseconds - m_UnixMillisecondsPrev;
			m_UnixMillisecondsPrev = m_UnixMilliseconds;
			// SetLastKnownUnixTime();

			m_TimeMilliseconds.Value += frameDelta;

			for (int i = m_Invocations.Count - 1; i >= 0; i--)
			{
				var invocation = m_Invocations[i];
				var dt = m_TimeMilliseconds.Value - invocation.executedAt;
				while (!invocation.isDone && dt >= invocation.interval)
				{
					invocation.executedAt += invocation.interval;
					dt -= invocation.interval;
					try
					{
						invocation.listener(invocation.interval);
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}
				}
			}

			m_TimeMillisecondsPrev = m_TimeMilliseconds.Value;

			// Variable.SavePlayerPrefs();
			
			m_SerializeTimeDataTimer.Update(Time.deltaTime);
		}

		private void GetLastKnownUnixTime()
		{
			var timeStr = PlayerPrefs.GetString("LastKnownUnixTime", "0");
			if (!long.TryParse(timeStr, out m_UnixMillisecondsPrev))
			{
				m_UnixMillisecondsPrev = 0;
			}

			if (m_UnixMillisecondsPrev < 1)
			{
				m_UnixMillisecondsPrev = m_UnixMilliseconds - 1;
			}
			else if (m_UnixMillisecondsPrev > m_UnixMilliseconds - 1)
			{
				m_UnixMillisecondsPrev = m_UnixMilliseconds - 1;
			}
		}

		private void SetLastKnownUnixTime()
		{
			PlayerPrefs.SetString("LastKnownUnixTime", m_UnixMillisecondsPrev.ToString());
			PlayerPrefs.Save();
		}

		public static long GetLastSessionEndTime()
		{
			return Instance.m_LastSessionEndTimeMilliseconds;
		}

		public static long GetTime()
		{
			return Instance.m_TimeMilliseconds.Value;
		}

		public static long GetTimeDelayedDays(float days)
		{
			return GetTime() + (long) (days * 86400000);
		}

		public static long GetTimeDelayedHours(float hours)
		{
			return GetTime() + (long) (hours * 3600000);
		}

		public static long GetTimeDelayedMinutes(float minutes)
		{
			return GetTime() + (long) (minutes * 60000);
		}

		public static long GetTimeDelayedSeconds(float seconds)
		{
			return GetTime() + (long) (seconds * 1000);
		}

		public static long GetElapsedTimeAfterLastSession()
		{
			return GetTime() - GetLastSessionEndTime();
		}

		public static void FormatShort(long ms, StringBuilder sb)
		{
			int minutes = (int) (ms / 60000);
			int seconds = (int) (ms % 60000 / 1000f);
			sb.Append(minutes.ToString("00")).Append(':').Append(seconds.ToString("00"));
		}

		public static void WhatIsFormatShort(long ms, StringBuilder sb)
		{
			int minutes = (int) (ms / 60000);
			int hours = (minutes / 60);
			minutes = (minutes % 60);
			int seconds = (int) (ms % 60000 / 1000f);
			sb.Append(hours.ToString("00")).Append(':').Append(minutes.ToString("00")).Append(':')
			  .Append(seconds.ToString("00"));
		}

		/// <summary>
		/// Returns periodic invocation handle which can be used when cancelling the invocation
		/// </summary>
		/// <param name="listener">Callback to be invoked</param>
		/// <param name="interval">Invocation interval in milliseconds</param>
		/// <returns></returns>
		public static int StartPeriodicInvocation(DiscreteTimeDeltaListener listener, long interval)
		{
			var handle = Instance.m_AvailableInvocationHandle++;
			var invocation = new PeriodicInvocation
			                 {
				                 executedAt = Instance.m_TimeMilliseconds, interval = interval,
				                 listener = listener
			                 };
			Instance.m_Invocations.Add(invocation);
			Instance.m_InvocationsMap[handle] = invocation;
			return handle;
		}

		public static void StopPeriodicInvocation(int handle)
		{
			if (!Instance.m_InvocationsMap.ContainsKey(handle))
				return;

			var invocation = Instance.m_InvocationsMap[handle];
			invocation.isDone = true;
			Instance.m_InvocationsMap.Remove(handle);
			Instance.m_Invocations.Remove(invocation);
		}
	}

	public class PeriodicInvocation
	{
		public long executedAt;
		public long interval;
		public bool isDone;
		public DiscreteTimeDeltaListener listener;
	}
}
