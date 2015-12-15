using UnityEngine;
using System;
using System.Collections;

// what state is a message channel in. this controls how you can use it at a given time
public enum MessageChannelState
{
	Idle,				// the channel is ready to accept a message.

	MessagePending, 	// a message hes been recived and is waiting to be processed

	CancelMessageSent, 	// a special cancel message recived, 
					   	// letting interested parties know that they will 
					   	// not be getting the message they expected
}

// the core of the message based architecture
/* the way to listen for and respond to events:
 * create some regular c# class that you want to handle some logic
 * then create a monobehavior that has an instance of that class
 * if you want it to be data, have it inherit from ScriptableObject. then you can make instances of it and assign it in the editor
 * otherwise, just make it a regualr object and creat it by calling its constructor
 * in the monobehavior that carse about the logic class
 * have Start() fire off a corutine (nameing convention XMain where X is the mname of the Script)
 * that looks like this
 * 
 * 	private IEnumerator XMain()
 * 	{
 * 		//every frame the game object is active
 * 		while(true)
 * 		{
 * 			//wait for a message we care about
 * 			while(logicObject.MessageWeCareAbout.Idle)
 * 			{
 * 				yield return 0;
 * 			}
 * 
 * 			if(logicObject.MessageWeCareAbout.MessagePending)
 * 			{
 * 				logicObject.MessageWeCareAbout.BeginProcessing();
 * 				
 * 				//handle the message
 * 
 * 				logicObject.MessageWeCareAbout.EndProcessing();
 * 
 * 				yield return logicObject.MessageWeCareAbout.WaitUntilMessageProcesed();
 * 			}
 * 		}
 * 	}
 * 
 * hmmmmmmmm i should probably make a class that just does this
 */
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
		return MessageChannelCoroutineManager.StartManagedCoroutine(WaitForMessageRoutine());
	}

	//a client waits for the channel to return to the idle state
	public Coroutine WaitTillMessageProcessed()
	{
		return MessageChannelCoroutineManager.StartManagedCoroutine(WaitTillMessageProcessedRoutine());
	}

	// tell clients there is a message
	public void SendMessage()
	{
		// you can only send messages to an idle channel
		if (m_state != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"A message can only be sent to a message channel when it is idle. " +
				"Use WaitTillMessageProcessed to wait until the channel is avaliable"
			);

		m_state = MessageChannelState.MessagePending;
		ReturnToIdleWhenDoneProccesing ();
	}

	// notify clients they will not be getting the message they were waiting for
	// this is used in the odd case where some ui waiting for a message needs to be told to close
	// this might be removed if it gets confusing
	public void SendCancel()
	{
		// you can only send messages to an idle channel
		if (m_state != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"A Cancel message can only be sent to a message channel when it is idle. " +
				"Use WaitTillMessageProcessed to wait until the channel is avaliable"
			);

		m_state = MessageChannelState.CancelMessageSent;
		ReturnToIdleWhenDoneProccesing ();
	}

	// helper property to determin if a chanle is idle
	public bool Idle
	{
		get
		{
			return m_state == MessageChannelState.Idle;
		}
	}

	// helper property to determin if a chanle recived a message
	public bool MessagePending
	{
		get
		{
			return m_state == MessageChannelState.MessagePending;
		}
	}

	// helper property to determin if a chanle recived a cancle message
	public bool CancelMessageSent
	{
		get
		{
			return m_state == MessageChannelState.CancelMessageSent;
		}
	}

	// a client calls this function (and EndProccessMessage after proccesing) if it needs more than one frame to process a message
	public void BeginProccesMessage()
	{
		m_processors++;
	}

	public void EndProccesMessage()
	{
		//BeginProccesMessage and EndProccesMessage must be called in pairs
		if (m_processors == 0)
			throw new InvalidOperationException (
				"called EndProccesMessage Without " +
				"first Calling BeginProcessMessage");

		m_processors--;
	}

	// return to idle state after message is proccesed
	private void ReturnToIdleWhenDoneProccesing()
	{
		MessageChannelCoroutineManager.StartManagedCoroutine (ReturnToIdleRoutine ());
	}

	private IEnumerator ReturnToIdleRoutine()
	{
		// wait for a frame to give a chance to respond
		// without this no one would have a chance to say they are processing the message
		yield return 0; 

		// wait while the message is being processed
		while (m_processors > 0) 
			yield return 0;

		// return to idle state
		m_state = MessageChannelState.Idle; 
	}

	// called by WaitForMessage
	private IEnumerator WaitForMessageRoutine()
	{
		// halt while the channel is idel
		while (m_state == MessageChannelState.Idle) {
			yield return 0;
		}
	}

	// called by WaitTillMessageProcessed
	private IEnumerator WaitTillMessageProcessedRoutine ()
	{
		// halt until the channle is idle
		while (m_state != MessageChannelState.Idle) {
			yield return 0;
		}
	}

	//helper class so MessageChannels can start corutines
	//low level unit biolerplate stuff
	private class MessageChannelCoroutineManager : MonoBehaviour
	{
		public static Coroutine StartManagedCoroutine(IEnumerator routine)
		{
			if (instance == null) {
				instance = new GameObject("__MessageChannelCoroutineManager").AddComponent<MessageChannelCoroutineManager>();
			}
			
			return instance.StartCoroutine (routine);
		}
		
		private static MessageChannelCoroutineManager instance;
	}

	// how many clients are processing this message. this channel will not return to idle until this is zero
	private int m_processors = 0;
	// backing variable for State
	private MessageChannelState m_state;
}

// a message channel which contains message content
// the reguale MessageChannel can be considered a Channel with no content
public class MessageChannel<MessageContentType> : MessageChannel
{
	// what was in the envelope? :)
	public MessageContentType MessageContent
	{
		get
		{
			return m_messageContent;
		}
	}

	// put somthing in the envelope
	public void SetMessageContent(MessageContentType content)
	{
		// don't change the message content while someone is looking at it!
		if (State != MessageChannelState.Idle)
			throw new InvalidOperationException (
				"the content of a message channel can only be set " +
				"when it is idle. Use WaitTillMessageProcessed to wait until the channel is avaliable");

		m_messageContent = content;
	}

	// helper funtion to set the content and then send a message
	public void SendMessage(MessageContentType content)
	{
		SetMessageContent (content);
		SendMessage ();
	}

	// backing field for MessageContent
	private MessageContentType m_messageContent;
}