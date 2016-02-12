using System;
using Slash.Unity.DataBind.Core.Utils;
using Slash.Unity.DataBind.Foundation.Commands;
using Slash.Unity.DataBind.Foundation.Setters;
using UnityEngine;
using Slash.Unity.DataBind.Core.Presentation;
using System.Collections.Generic;

using Object = UnityEngine.Object;
using System.Collections;

[Serializable]
public class MessageDataBinding
{
	// possible binding values
	public string Constant;
	[ContextPath]
	public string Path;
	public DataProvider Provider;
	public MessageDataProvider MessageProvider;
	public Object Reference;

	//determins which binding value to use
	public MessageDataBindingType Type;

	//used to get value from context
	private MessageContextNode contextNode;

	public bool IsInitialized { get; set; }

	public MessageProperty<object> Value;
		
	public void Deinit()
	{
		if (this.Provider != null)
		{
			this.Provider.ValueChanged -= this.OnTargetValueChanged;
		}
		if(MessageProvider != null)
		{
			this.MessageProvider.Value.Changing.RemoveHandler(this.WaitOnTargetValueChanged);
		}
		if (this.contextNode != null)
		{
			this.contextNode.SetValueListener(null);
			this.contextNode = null;
		}
	}

	public T GetValue<T>()
	{
		var rawValue = this.Value.ObjectValue;
		if (rawValue == null)
		{
			return default(T);
		}
		try
		{
			return (T)Convert.ChangeType(rawValue, typeof(T));
		}
		catch (Exception)
		{
			try
			{
				return (T)rawValue;
			}
			catch (InvalidCastException)
			{
				throw new InvalidCastException(
					string.Format(
						"Can't cast value '{0}' of binding '{1}' to type '{2}'",
						rawValue,
						this.Path,
						typeof(T)));
			}
		}
	}

	public void Init(GameObject gameObject)
	{
		switch (this.Type)
		{
			case MessageDataBindingType.Context:
				{
					this.contextNode = new MessageContextNode(gameObject, this.Path);
					var initialValue = this.contextNode.SetValueListener(this.WaitOnTargetValueChanged);
					if (this.contextNode.IsInitialized)
					{
						this.IsInitialized = true;
						this.Value = new MessageProperty<object>(initialValue);
					}
				}
				break;
			case MessageDataBindingType.Provider:
				{
					if (this.Provider != null)
					{
						this.Provider.ValueChanged += this.OnTargetValueChanged;
						if (this.Provider.IsInitialized)
						{
							this.IsInitialized = true;
							this.Value = new MessageProperty<object>(this.Provider.Value);
						}
					}
					else
					{
						this.IsInitialized = true;
					}
				}
				break;
			case MessageDataBindingType.MessageProvider:
				{
					if (this.MessageProvider != null)
					{
						this.MessageProvider.Value.Changing.AddHandler(this.WaitOnTargetValueChanged);
						if (this.MessageProvider.IsInitialized)
						{
							this.IsInitialized = true;
							this.Value = new MessageProperty<object>(this.MessageProvider.Value.ObjectValue);
						}
					}
					else
					{
						this.IsInitialized = true;
					}
				}
				break;
			case MessageDataBindingType.Constant:
				{
					this.IsInitialized = true;
					this.Value = new MessageProperty<object>(this.Constant);
				}
				break;
			case MessageDataBindingType.Reference:
				{
					this.IsInitialized = true;
					this.Value = new MessageProperty<object>(this.Reference == null ? null : this.Reference);
				}
				break;
		}
	}

	public IEnumerator WaitOnContextChanged()
	{
		if (this.contextNode != null)
		{
			yield return new Routine(this.contextNode.WaitOnHierarchyChanged());
		}
	}

	private void OnTargetValueChanged(object newValue)
	{
		this.IsInitialized = true;
		ServiceLocator.GetService<ICoroutineService>().StartCoroutine(this.Value.WaitSet(newValue));
	}

	private IEnumerator WaitOnTargetValueChanged(object newValue)
	{
		this.IsInitialized = true;
		yield return new Routine(this.Value.WaitSet(newValue));
	}
}




