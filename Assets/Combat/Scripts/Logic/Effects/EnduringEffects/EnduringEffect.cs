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