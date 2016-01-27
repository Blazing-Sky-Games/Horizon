using System;
using System.Collections;
using Combat.Code.Services.TurnOrderService;

namespace Combat.Code.Data.Logic.Effects
{
	public abstract class EnduringEffect : CombatEffect
	{
		private UnitId m_attacker;
		private UnitId m_defender;
		private bool m_isCritical;

		private IEnduringEffectService enduringEffectService;

		public UnitId Caster{get{ return m_attacker;}}
		public UnitId Target{get{ return m_defender;}}
		public bool IsCritical{get{ return m_isCritical;}}

		public override IEnumerator WaitTrigger (UnitId attacker, UnitId defender, bool isCritical)
		{
			enduringEffectService = enduringEffectService == null ? ServiceLocator.GetService<IEnduringEffectService>() : enduringEffectService;

			m_attacker = attacker;
			m_defender = defender;
			m_isCritical = isCritical;

			EnduringEffect copy = (EnduringEffect)this.DeepCopy();
			yield return new Routine(enduringEffectService.RecordEffect(copy));
		}

		public abstract IEnumerator StartEffect();

		public abstract IEnumerator OnNewTurn();

		public abstract bool EndingCondition();

		public abstract IEnumerator EndEffect();
	}
}

