using System;

public class MessageArgs : EventArgs
{
	public readonly object message;

	public MessageArgs(object message)
	{
		this.message = message;
	}
}
