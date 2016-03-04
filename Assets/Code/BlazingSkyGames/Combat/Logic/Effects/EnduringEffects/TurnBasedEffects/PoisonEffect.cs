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

class PoisonEffect : TurnBasedEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationB = 1;
	[UnityEngine.Tooltip("Duration = M*Potency + B")]
	public int DurationM = 3;

	private int dmg;

	public override IEnumerator WaitStart ()
	{
		yield return new Routine(base.WaitStart());

		float potency = GetMatchUp(Caster, Target, IsCritical);

		dmg = (int)(DamageM * potency);

		int duration = (int)(DurationB + DurationM * potency);

		yield return new Routine(DurationSetter.WaitSet(duration));
	}

	public override IEnumerator WaitUpdate ()
	{
		yield return new Routine(base.WaitUpdate());

		yield return new Routine(Target.Health.WaitHurt(dmg));
	}
}

