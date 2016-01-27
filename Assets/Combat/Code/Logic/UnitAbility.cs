using System;
using System.Linq;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class UnitAbility
{
	public readonly Message CriticalHit = new Message();

	private readonly List<CombatEffect> combatEffects;
	private readonly List<CombatEffect> criticalEffects;
	private readonly float critChanceBonus;

	public UnitAbility(UnitAbilityData Data)
	{
		combatEffects = Data.CombatEffects;
		criticalEffects = Data.CriticalEffects;
		critChanceBonus = Data.CritChanceBonus;
	}

	public IEnumerator WaitUse(UnitId casterId, UnitId targetId)
	{
		IUnitService unitService = ServiceLocator.GetService<IUnitService>();
		Unit caster = unitService.GetUnit(casterId);
		Unit target = unitService.GetUnit(targetId);

		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(CombatEffect effect in combatEffects)
		{
			yield return new Routine(effect.WaitTrigger(casterId, targetId, false));
		}

		float criticalSuccessThreshold = (caster.GetCriticalAccuracy() / target.GetCriticalAvoidance()) * 0.2f; 

		if(Random.value <= criticalSuccessThreshold + critChanceBonus)
		{
			yield return new Routine(CriticalHit.WaitSend());

			foreach(CombatEffect effect in criticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(casterId, targetId, true));
			}
		}
	}
}


