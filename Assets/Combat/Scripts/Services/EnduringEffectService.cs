using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnduringEffectService : Service, IEnduringEffectService
{
	public Message<EnduringEffect> EffectRecorded
	{ 
		get { return m_effectRecorded; } 
	}

	public Message<EnduringEffect> EffectErased
	{ 
		get{ return m_effectErased; } 
	}

	public IEnumerator WaitRecordEffect (EnduringEffect effect)
	{
		activeEffects.Add(effect);
		var startEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(effect.WaitStart());
		var recordEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(m_effectRecorded.WaitSend(effect));

		yield return startEffect;
		yield return recordEffect;
	}

	public IEnumerator WaitEraseEffect (EnduringEffect effect)
	{
		activeEffects.Add(effect);
		var endEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(effect.WaitEnd());
		var eraseEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(m_effectErased.WaitSend(effect));

		yield return endEffect;
		yield return eraseEffect;
	}

	public IEnumerable<EffectType> ActiveEffectsOfType<EffectType> () where EffectType : EnduringEffect
	{
		return activeEffects.Where(effect => effect.GetType() == typeof(EffectType)).Cast<EffectType>();
	}

	public IEnumerator WaitUpdateEffects (IEnumerable<EnduringEffect> effects)
	{
		foreach(EnduringEffect effect in effects)
		{
			yield return new Routine(WaitUpdateEffect(effect));
		}
	}

	public IEnumerator WaitUpdateEffect (EnduringEffect effect)
	{
		if(effect.EndingCondition())
		{
			yield return new Routine(WaitEraseEffect(effect));
		}
		else
		{
			yield return new Routine(effect.WaitUpdate());
		}
	}

	private List<EnduringEffect> activeEffects = new List<EnduringEffect>();
	private Message<EnduringEffect> m_effectRecorded = new Message<EnduringEffect>();
	private Message<EnduringEffect> m_effectErased = new Message<EnduringEffect>();
}


