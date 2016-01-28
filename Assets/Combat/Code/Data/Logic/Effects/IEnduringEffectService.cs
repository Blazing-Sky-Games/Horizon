using System.Collections;
using System.Collections.Generic;

interface IEnduringEffectService : IService
{
	IEnumerator RecordEffect (EnduringEffect effect);

	IEnumerator EraseEffect (EnduringEffect effect);

	IEnumerable<EffectType> ActiveEffectsOfType<EffectType> ()
			where EffectType : EnduringEffect;
}