using System;
using Roro.TimeManagement.Utility;
//using Newtonsoft.Json;
using UnityEngine;

namespace TimeManagement.Utility
{
	public class OfflineCounter : OfflineTimer
	{
		// [JsonIgnore]
		// public long initialCountdown;
		//
		// [JsonRequired]
		// public long remainingTime;
		//
		// [JsonIgnore]
		// public Action onComplete;
		//
		// private bool canStop = false;
		//
		// protected override void AfterFirstNewSessionInvocation()
		// {
		// 	if (remainingTime > 0)
		// 	{
		// 		base.AfterFirstNewSessionInvocation();
		// 		canStop = true;
		// 	}
		// }
		//
		// public override void Update(long dt)
		// {
		// 	base.Update(dt);
		//
		// 	remainingTime -= dt;
		//
		// 	if (remainingTime <= 0)
		// 	{
		// 		int n = (int) ((-remainingTime + initialCountdown) / initialCountdown);
		// 		for (var i = 0; i < n; i++)
		// 		{
		// 			Complete();
		// 		}
		// 	}
		// }
		//
		// public override void Start()
		// {
		// 	base.Start();
		//
		// 	remainingTime = initialCountdown;
		//
		// 	canStop = true;
		// }
		//
		// public virtual void Complete()
		// {
		// 	onComplete?.Invoke();
		//
		// 	if (canStop)
		// 		Stop();
		// }
		//
		// public override void Serialize(JsonWriter jw)
		// {
		// 	base.Serialize(jw);
		//
		// 	jw.WritePropertyName("remainingTime");
		// 	jw.WriteValue(remainingTime);
		// }
	}
}
