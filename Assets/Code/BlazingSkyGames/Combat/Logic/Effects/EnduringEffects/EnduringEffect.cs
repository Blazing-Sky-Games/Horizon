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

public abstract class EnduringEffect : CombatEffect
{
	private Unit m_attacker;
	private Unit m_defender;
	private bool m_isCritical;

	private IEnduringEffectService enduringEffectService;

	public Unit Caster{ get { return m_attacker; } }

	public Unit Target{ get { return m_defender; } }

	public bool IsCritical{ get { return m_isCritical; } }

	public override IEnumerator WaitTrigger (Unit attacker, Unit defender, bool isCritical)
	{
		enduringEffectService = enduringEffectService == null ? ServiceLocator.GetService<IEnduringEffectService>() : enduringEffectService;

		m_attacker = attacker;
		m_defender = defender;
		m_isCritical = isCritical;

		EnduringEffect copy = (EnduringEffect)this.DeepCopy();
		yield return new Routine(enduringEffectService.WaitRecordEffect(copy));
	}

	public abstract IEnumerator WaitStart ();

	public abstract IEnumerator WaitUpdate ();

	public abstract bool EndingCondition ();

	public virtual IEnumerator WaitEnd (){yield break;}
}