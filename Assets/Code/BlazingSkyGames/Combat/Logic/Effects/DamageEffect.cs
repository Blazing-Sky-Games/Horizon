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

using System.Collections;
using System;

public class DamageEffect : CombatEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageB = 5;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public float DamageLR = 0.8f;

	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, bool IsCritical)
	{
		Random rand = new Random();

		// roll a die to see how strong the attack is
		float attackRoll = (float)rand.NextDouble() * (1f - DamageLR) + DamageLR;

		int dmg = 
			(int)(GetMatchUp(Attacker, Defender, IsCritical) 
			* DamageM 
			* attackRoll  
			+ DamageB);

		yield return new Routine(Defender.Health.WaitHurt(dmg));
	}

}