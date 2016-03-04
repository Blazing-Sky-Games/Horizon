/*
The MIT License (MIT)

https://opensource.org/licenses/MIT

Copyright (c) 2016 Matthew Draper

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), 
to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", 
WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE 
AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
		ServiceLocator.GetService<ILoggingService>().Log("enduring effect of type " + effect.GetType().Name + " recorded");
		var recordEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(m_effectRecorded.WaitSend(effect));

		yield return startEffect;
		yield return recordEffect;
	}

	public IEnumerator WaitEraseEffect (EnduringEffect effect)
	{
		activeEffects.Add(effect);
		var endEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(effect.WaitEnd());
		ServiceLocator.GetService<ILoggingService>().Log("enduring effect of type " + effect.GetType().Name + " erased");
		var eraseEffect = ServiceLocator.GetService<ICoroutineService>().StartCoroutine(m_effectErased.WaitSend(effect));

		yield return endEffect;
		yield return eraseEffect;
	}

	public IEnumerable<EffectType> ActiveEffectsOfType<EffectType> () where EffectType : EnduringEffect
	{
		return activeEffects.Where(effect => effect.GetType().IsSubclassOf(typeof(EffectType))).Cast<EffectType>();
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
			ServiceLocator.GetService<ILoggingService>().Log("updateing enduring effect of type " + effect.GetType().Name);
			yield return new Routine(effect.WaitUpdate());
		}
	}

	private List<EnduringEffect> activeEffects = new List<EnduringEffect>();
	private Message<EnduringEffect> m_effectRecorded = new Message<EnduringEffect>();
	private Message<EnduringEffect> m_effectErased = new Message<EnduringEffect>();
}


