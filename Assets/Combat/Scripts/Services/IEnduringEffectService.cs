using System.Collections;
using System.Collections.Generic;

interface IEnduringEffectService : IService
{
	Message<EnduringEffect> EffectRecorded{ get; }

	Message<EnduringEffect> EffectErased{ get; }

	IEnumerator WaitRecordEffect (EnduringEffect effect);

	IEnumerator WaitEraseEffect (EnduringEffect effect);

	IEnumerable<EffectType> ActiveEffectsOfType<EffectType> ()
			where EffectType : EnduringEffect;

	IEnumerator WaitUpdateEffects (IEnumerable<EnduringEffect> effects);

	IEnumerator WaitUpdateEffect (EnduringEffect effect);
}