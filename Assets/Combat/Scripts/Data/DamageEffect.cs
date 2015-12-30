using System.Collections;
using Random = UnityEngine.Random;

public class DamageEffect : AbilityEffect
{
	// the minimum damage a damage effect can cause
	private const int MINDMG = 5;
	// tuning variable
	private const float DMGCOEF = 0.6f;

	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, int abilityPower, bool IsCritical)
	{
		// roll a die to see how strong the attack is
		float attackRoll = Random.Range(0.8f, 1f);

		int dmg = 
			(int)(GetPotency(Attacker, Defender, IsCritical) 
			* abilityPower 
			* attackRoll 
			* DMGCOEF 
			+ MINDMG);

		yield return new Routine(Defender.WaitTakeDamage(dmg, IsCritical));
	}

}

