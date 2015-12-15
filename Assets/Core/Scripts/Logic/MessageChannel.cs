using UnityEngine;
using System;
using System.Collections;

public enum MessageChannelState
{
	Idle,				// the channel is ready to accept a message.
	MessagePending, 	// a message hes been recived and is waiting to be processed
	CancelMessageSent, 	// a special cancel message recived, 
					   	// letting interested parties know that they will 
					   	// not be getting the message they expected
}

public class MessageChannel 
{
	//the logical state of the channel
	public MessageChannelState State
	{
		get
		{
			return m_state;
		}
	}

	//a client waits for the channel to recive a message
	public Coroutine WaitForMessage()
	{
		//if (m_waitForMessageRoutine == null) {
			//m_waitForMessageRoutine = MessageChannelManager.StartManagedCoroutine(WaitForMessageRoutine());
		//}

		//return m_waitForMessageRoutine;
		return MessageChannelManager.StartManagedCoroutine(WaitForMessageRoutine());
	}

	//a client waits for the channel to return to the idle state
	public Coroutine WaitTillMessageProcessed()
	{
		//if (m_waitTillMessageProcessedRoutine == null) {
			//m_waitTillMessageProcessedRoutine = MessageChannelManager.StartManagedCoroutine(WaitTillMessageProcessedRoutine());
		//}
		
		//return m_waitTillMessageProcessedRoutine;
		return MessageChannelManager.StartManagedCoroutine(WaitTillMessageProcessedRoutine());
	}

	// tell clients there is a message
	public void SendMessage()
	{
		if (m_state != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"A message can only be sent to a message channel when it is idle. " +
				"Use WaitTillMessageProcessed to wait until the channel is avaliable"
			);

		m_state = MessageChannelState.MessagePending;
		ReturnToIdle ();
	}

	// notify clients they will not be getting the message they were waiting for
	public void SendCancel()
	{
		if (m_state != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"A Cancel message can only be sent to a message channel when it is idle. " +
				"Use WaitTillMessageProcessed to wait until the channel is avaliable"
			);

		m_state = MessageChannelState.CancelMessageSent;
		ReturnToIdle ();
	}

	public bool Idle
	{
		get
		{
			return m_state == MessageChannelState.Idle;
		}
	}

	// a client calls this function (and EndProccessMessage after proccesing) if it needs more than one frame to process a message
	public void BeginProccesMessage()
	{
		m_processors++;
	}

	public void EndProccesMessage()
	{
		if (m_processors == 0)
			throw new InvalidOperationException (
				"called EndProccesMessage Without " +
				"first Calling BeginProcessMessage");

		m_processors--;
	}

	//helper class so MessageChannels can start corutines
	private class MessageChannelManager : MonoBehaviour
	{
		public static Coroutine StartManagedCoroutine(IEnumerator routine)
		{
			if (instance == null) {
				instance = new GameObject("__MessageChannelManager").AddComponent<MessageChannelManager>();
			}

			return instance.StartCoroutine (routine);
		}

		private static MessageChannelManager instance;
	}

	// return to idle state after message is proccesed
	private void ReturnToIdle()
	{
		MessageChannelManager.StartManagedCoroutine (ReturnToIdleRoutine ());
	}

	private IEnumerator ReturnToIdleRoutine()
	{
		yield return 0; //wait for a frame to give a chance to respond

		while (m_processors > 0) // wait while the message is being processed
			yield return 0;

		m_state = MessageChannelState.Idle; // return to idle state
	}
	
	private IEnumerator WaitForMessageRoutine()
	{
		while (m_state == MessageChannelState.Idle) {
			yield return 0;
		}
		//m_waitForMessageRoutine = null;
	}

	private IEnumerator WaitTillMessageProcessedRoutine ()
	{
		while (m_state != MessageChannelState.Idle) {
			yield return 0;
		}
		//m_waitTillMessageProcessedRoutine = null;
	}

	//private Coroutine m_waitForMessageRoutine;
	//private Coroutine m_waitTillMessageProcessedRoutine;

	private int m_processors = 0;

	private MessageChannelState m_state;
}

public class MessageChannel<MessageContentType> : MessageChannel
{
	public MessageContentType Content
	{
		get
		{
			return m_messageContent;
		}
	}

	public void SetMessageContent(MessageContentType content)
	{
		if (State != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"the content of a message channel can only be set " +
				"when it is idle. Use WaitTillMessageProcessed to wait until the channel is avaliable");

		m_messageContent = content;
	}

	public void SendMessage(MessageContentType content)
	{
		SetMessageContent (content);
		SendMessage ();
	}

	private MessageContentType m_messageContent;
}