using System.Collections;
using Random = UnityEngine.Random;

public class DamageEffect : AbilityEffect
{
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageM = 10;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public int DamageB = 5;
	[UnityEngine.Tooltip("dmg = M*Potency*Rand(LR,1)+B")]
	public float DamageLR = 0.8f;

	public override IEnumerator WaitTrigger(Unit Attacker, Unit Defender, bool IsCritical)
	{
		// roll a die to see how strong the attack is
		float attackRoll = Random.Range(DamageLR, 1f);

		int dmg = 
			(int)(GetPotency(Attacker, Defender, IsCritical) 
			* DamageM 
			* attackRoll  
			+ DamageB);

		yield return new Routine(Defender.WaitTakeDamage(dmg, IsCritical));
	}

}

