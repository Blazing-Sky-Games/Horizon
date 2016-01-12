using System.Collections;

public class WeakenEffectLogicData : AbilityEffectLogicData
{
	[UnityEngine.Tooltip("which stat will be changed")]
	public UnitStatatistic StatToWeaken = UnitStatatistic.Strength;

	[UnityEngine.Tooltip("Drop = M*potency")]
	public int DropM = 20;
	[UnityEngine.Tooltip("Duration = M*potency + B")]
	public int DurationB = 2;
	[UnityEngine.Tooltip("Duration = M*potency + B")]
	public int DurationM = 2;

	public override IEnumerator WaitTrigger(UnitLogicData Attacker, UnitLogicData Defender, bool IsCritical)
	{
		yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitWeakenEffect(Attacker, Defender, IsCritical)));
	}

	IEnumerator WaitWeakenEffect(UnitLogicData attacker, UnitLogicData defender, bool isCritical)
	{
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Weakened, true));

		float Potency = GetPotency(attacker, defender, isCritical);

		int drop = DropM * (int)Potency;
		int duration = DurationB + (int)Potency * DurationM;

		int oldval = defender.GetStatistic(StatToWeaken);
		defender.SetStatistic(StatToWeaken, oldval - drop);

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		defender.SetStatistic(StatToWeaken, oldval);
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Weakened, false));
	}
}



