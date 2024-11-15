using Based2.TimeManagement.Core;
using Newtonsoft.Json;
using Roro.Scripts.Serialization;

namespace Roro.TimeManagement.Utility
{
	[System.Serializable]
	public class OfflineTimer : IJsonSerializable
	{
		[JsonIgnore]
		public long updateInterval = 1000;
		
		[JsonRequired]
		public long executedAt;
		
		[JsonRequired]
		public bool isActive = false;
		
		[JsonIgnore]
		public DiscreteTimeDeltaListener updateCallback;
		
		[JsonIgnore]
		private int handle = -1;
		
		public void Initialize(DiscreteTimeDeltaListener updateCallback = null)
		{
			this.updateCallback = updateCallback;
		}
		
		protected virtual void AfterFirstNewSessionInvocation()
		{
			handle = TimeManager.StartPeriodicInvocation(Update, updateInterval);
		}
		
		public virtual bool TryContinueNewSession()
		{
			if (!isActive)
			{
				// Is not active after deserialization, ignore
				return false;
			}
		
			var timeSinceLastInvocation = TimeManager.GetTime() - executedAt;
			Update(timeSinceLastInvocation);
		
			AfterFirstNewSessionInvocation();
		
			return true;
		}
		
		public virtual void Update(long dt)
		{
			executedAt += dt;
			updateCallback?.Invoke(dt);
		}
		
		public virtual void Start()
		{
			if (isActive)
			{
				TimeManager.StopPeriodicInvocation(handle);
			}
		
			isActive = true;
			executedAt = TimeManager.GetTime();
		
			handle = TimeManager.StartPeriodicInvocation(Update, updateInterval);
		}
		
		public virtual void Stop()
		{
			isActive = false;
			TimeManager.StopPeriodicInvocation(handle);
		}
		
		public virtual void Dispose()
		{
			TimeManager.StopPeriodicInvocation(handle);
		}
		
		public virtual void Serialize(JsonWriter jw)
		{
			jw.WritePropertyName("executedAt");
			jw.WriteValue(executedAt);
			jw.WritePropertyName("isActive");
			jw.WriteValue(isActive);
		}
	}
}
