using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnBasedEffectManager
{
	public static IEnumerator WaitStartTurnBasedEffect(IEnumerator effectRoutine)
	{
		TurnBassedEffect effect = new TurnBassedEffect(effectRoutine);
		m_effects.Add(effect);
		yield return CoroutineManager.Main.StartCoroutine(effect.WaitUpdate());
	}

	public static IEnumerator WaitUpdateTurnBasedEffects()
	{
		foreach(TurnBassedEffect effect in m_effects)
		{
			yield return CoroutineManager.Main.StartCoroutine(effect.WaitUpdate());
		}

		m_effects = m_effects.Where(x => !x.Done).ToList();
	}

	private static List<TurnBassedEffect> m_effects = new List<TurnBassedEffect>();
}