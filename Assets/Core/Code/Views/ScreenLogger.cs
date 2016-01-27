using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core.Code.Services.LoggingService;

public class ScreenLogger : MonoBehaviour
{
	public enum LogAnchor
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	[Tooltip("Height of the log area as a percentage of the screen height")]
	[Range(0.3f, 1.0f)]
	public float Height = 0.5f;

	[Tooltip("Width of the log area as a percentage of the screen width")]
	[Range(0.3f, 1.0f)]
	public float Width = 0.5f;

	public int Margin = 20;

	public LogAnchor AnchorPosition = LogAnchor.BottomLeft;

	public int FontSize = 14;

	[Range(0f, 01f)]
	public float BackgroundOpacity = 0.5f;
	public Color BackgroundColor = Color.black;

	public Color MessageColor = Color.white;

	static Queue<LogMessage> queue = new Queue<LogMessage>();

	GUIStyle styleContainer, styleText;
	int padding = 5;

	public void Awake()
	{
		Texture2D back = new Texture2D(1, 1);
		BackgroundColor.a = BackgroundOpacity;
		back.SetPixel(0, 0, BackgroundColor);
		back.Apply();

		styleContainer = new GUIStyle();
		styleContainer.normal.background = back;
		styleContainer.wordWrap = true;
		styleContainer.padding = new RectOffset(padding, padding, padding, padding);

		styleText = new GUIStyle();
		styleText.fontSize = FontSize;

		loggingService = ServiceLocator.GetService<ILoggingService>();
	}

	void OnEnable()
	{
		queue = new Queue<LogMessage>();
		//TODO HACK fix this hack. meh, it works for now
		loggingService.ScreenLog.OnLog += HandleLog;
	}

	void OnDisable()
	{
		loggingService.ScreenLog.OnLog -= HandleLog;
	}

	void Update()
	{
		while(queue.Count > ((Screen.height - 2 * Margin) * Height - 2 * padding) / styleText.lineHeight)
		{
			queue.Dequeue();
		}
	}

	void OnGUI()
	{
		float w = (Screen.width - 2 * Margin) * Width;
		float h = (Screen.height - 2 * Margin) * Height;
		float x = 1, y = 1;

		switch(AnchorPosition)
		{
			case LogAnchor.BottomLeft:
				x = Margin;
				y = Margin + (Screen.height - 2 * Margin) * (1 - Height);
				break;

			case LogAnchor.BottomRight:
				x = Margin + (Screen.width - 2 * Margin) * (1 - Width);
				y = Margin + (Screen.height - 2 * Margin) * (1 - Height);
				break;

			case LogAnchor.TopLeft:
				x = Margin;
				y = Margin;
				break;

			case LogAnchor.TopRight:
				x = Margin + (Screen.width - 2 * Margin) * (1 - Width);
				y = Margin;
				break;
		}

		GUILayout.BeginArea(new Rect(x, y, w, h), styleContainer);

		foreach(LogMessage m in queue)
		{
			styleText.normal.textColor = MessageColor;

			GUILayout.Label(m.Message, styleText);
		}

		GUILayout.EndArea();
	}

	void HandleLog(string message)
	{
		queue.Enqueue(new LogMessage(message));
	}

	ILoggingService loggingService;
}

class LogMessage
{
	public string Message;

	public LogMessage(string msg)
	{
		Message = msg;
	}
}