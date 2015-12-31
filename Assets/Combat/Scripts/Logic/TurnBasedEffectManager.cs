using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedEffectManager
{
	public static IEnumerator WaitStartTurnBasedEffect(IEnumerator effectRoutine)
	{
		TurnBasedEffect effect = new TurnBasedEffect(effectRoutine, CallerInformation.MethodName, CallerInformation.FilePath, CallerInformation.LineNumber);
		m_effects.Add(effect);
		yield return new Routine(effect.WaitUpdate());
	}

	public static IEnumerator WaitUpdateTurnBasedEffects()
	{
		foreach(TurnBasedEffect effect in m_effects)
		{
			yield return new Routine(effect.WaitUpdate());
		}

		m_effects = m_effects.Where(x => !x.Done).ToList();
	}

	private static List<TurnBasedEffect> m_effects = new List<TurnBasedEffect>();
}