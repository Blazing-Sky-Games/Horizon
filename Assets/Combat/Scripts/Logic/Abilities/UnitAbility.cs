using System.Collections.Generic;
using System.Collections;
using System;

public class UnitAbility
{
	public readonly Message CriticalHit = new Message();

	private readonly List<CombatEffect> combatEffects;
	private readonly List<CombatEffect> criticalEffects;
	private readonly float critChanceBonus;

	public readonly string Name;

	public UnitAbility(UnitAbilityData Data)
	{
		combatEffects = Data.CombatEffects;
		criticalEffects = Data.CriticalEffects;
		critChanceBonus = Data.CritChanceBonus;
		Name = Data.DebugName;
	}

	public IEnumerator WaitUse(Unit caster, Unit target)
	{
		ServiceLocator.GetService<ILoggingService>().Log(caster.Name + " Used ability " + Name + " on " + target.Name );
		yield return new Routine(caster.AbilityUsedMessage.WaitSend(caster, this, target));

		foreach(CombatEffect effect in combatEffects)
		{
			yield return new Routine(effect.WaitTrigger(caster, target, false));
		}

		float criticalSuccessThreshold = (caster.GetCriticalAccuracy() / target.GetCriticalAvoidance()) * 0.2f; 

		Random rand = new Random();

		if(rand.NextDouble() <= criticalSuccessThreshold + critChanceBonus)
		{
			ServiceLocator.GetService<ILoggingService>().Log("critical hit!");
			yield return new Routine(CriticalHit.WaitSend());

			foreach(CombatEffect effect in criticalEffects)
			{
				yield return new Routine(effect.WaitTrigger(caster, target, true));
			}
		}
	}
}