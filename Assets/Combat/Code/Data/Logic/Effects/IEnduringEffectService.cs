using System;
using System.Collections;
using System.Collections.Generic;

namespace Combat.Code.Data.Logic.Effects
{
	interface IEnduringEffectService : IService
	{
		IEnumerator RecordEffect (EnduringEffect effect);

		IEnumerator EraseEffect (EnduringEffect effect);

		IEnumerable<EffectType> ActiveEffectsOfType<EffectType> ()
			where EffectType : EnduringEffect;
	}

}

