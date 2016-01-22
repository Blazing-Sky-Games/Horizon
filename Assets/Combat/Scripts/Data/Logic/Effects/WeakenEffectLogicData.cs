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

	public override IEnumerator WaitTrigger(UnitLogic Attacker, UnitLogic Defender, bool IsCritical)
	{
		yield return new Routine(TurnBasedEffectManager.WaitStartTurnBasedEffect(WaitWeakenEffect(Attacker, Defender, IsCritical)));
	}

	IEnumerator WaitWeakenEffect(UnitLogic attacker, UnitLogic defender, bool isCritical)
	{
		yield return new Routine(defender.WaitSetStatus(UnitStatus.Weakened, true));

		float Potency = GetMatchUp(attacker, defender, isCritical);

		int drop = DropM * (int)Potency;
		int duration = DurationB + (int)Potency * DurationM;

		Statistic stat = defender.GetStatistic(StatToWeaken);
		yield return new Routine(stat.WaitModify(-drop));

		while(duration > 0)
		{
			yield return new WaitForNextTurn();
			duration--;
		}

		Coroutine removeModifyerRoutine = CoroutineUtility.StartCoroutine(stat.WaitModify(drop));
		Coroutine removeStatusRoutine = CoroutineUtility.StartCoroutine(defender.WaitSetStatus(UnitStatus.Weakened, false));

		yield return removeModifyerRoutine;
		yield return removeStatusRoutine;
	}
}



