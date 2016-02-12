using System;
using System.Collections;
using Slash.Unity.DataBind.Core.Data;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityState
{
	Normal,
	Targeting,
	Confirm,
	Disabled
}

public class AbilityContext : Context
{
	#region DataBinding 
	MessageProperty<AbilityState> state = new MessageProperty<AbilityState>();
	public EventQueue CancelTargeting = new EventQueue();
	public EventQueue Cancel = new EventQueue();
	public EventQueue Confirm = new EventQueue();

	Property<Sprite> Icon;

	public void StartTargeting()
	{
		ServiceLocator.GetService<ICoroutineService>().StartCoroutine(WaitStartTargeting());
	}
	#endregion

	public AbilityContext(UnitAbility ability, Unit caster)
	{
		this.ability = ability;
		this.caster = caster;
	}

	// sent by hotbar context
	EventQueue<Unit> UnitTargeted = new EventQueue<Unit>();

	public IEnumerator WaitStartTargeting ()
	{
		while(true)
		{
			//enter targeting button state
			yield return new Routine(state.WaitSet(AbilityState.Targeting));

			//ignore any events sent before we cared
			CancelTargeting.Clear();
			UnitTargeted.Clear();

			//remain in targeting state until we get a unit or targeting is canceled
			while(!UnitTargeted.EventPending && !CancelTargeting.EventPending)
			{
				yield return new WaitForNextUpdate();
			}

			//if targeting is canceled, return to normal state
			if(CancelTargeting.EventPending)
			{
				CancelTargeting.ConsumeEvent();
				yield return new Routine(state.WaitSet(AbilityState.Normal));
				yield break;
			}

			Unit target = UnitTargeted.ConsumeEvent();

			//enter confirm button state
			yield return new Routine(state.WaitSet(AbilityState.Confirm));

			//ignore any events sent before we cared
			Confirm.Clear();
			Cancel.Clear();

			// remian in confirm state until a confirm or cancel event is sent
			while(!Confirm.EventPending && !Cancel.EventPending)
			{
				yield return new WaitForNextUpdate();
			}

			// if canceled, go back to targeting
			if(Cancel.EventPending)
			{
				Cancel.ConsumeEvent();
				continue;
			}

			//if confirmed, disable the button, use the ability, then reenable the button
			Confirm.ConsumeEvent();
			yield return new Routine(state.WaitSet(AbilityState.Disabled));
			yield return new Routine(ability.WaitUse(caster, target));
			yield return new Routine(state.WaitSet(AbilityState.Normal));
			yield break;
		}
	}

	private UnitAbility ability;
	private Unit caster;
}