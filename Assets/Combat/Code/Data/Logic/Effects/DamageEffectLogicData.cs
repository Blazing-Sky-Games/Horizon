using System.Collections;
using System;

public class DamageEffectLogicData : CombatEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageB = 5;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public float DamageLR = 0.8f;

	public override IEnumerator WaitTrigger(UnitId Attacker, UnitId Defender, bool IsCritical)
	{
		IUnitService unitService = ServiceLocator.GetService<IUnitService>();

		Random rand = new Random();

		// roll a die to see how strong the attack is
		float attackRoll = (float)rand.NextDouble() * (1f - DamageLR) + DamageLR;

		int dmg = 
			(int)(GetMatchUp(unitService.GetUnit(Attacker), unitService.GetUnit(Defender), IsCritical) 
			* DamageM 
			* attackRoll  
			+ DamageB);

		yield return new Routine(unitService.GetUnit(Defender).Health.WaitHurt(dmg));
	}

}