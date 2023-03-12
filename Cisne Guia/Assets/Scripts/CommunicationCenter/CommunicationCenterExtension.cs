using System;

public static class CommunicationCenterExtension
{
	public static void PostNotification(this object obj, string notificationName)
	{
		CommunicationCenter.centerInstance.PostNotification(notificationName, obj);
	}
	public static void PostNotification(this object obj, string notificationName, EventArgs e)
	{
		CommunicationCenter.centerInstance.PostNotification(notificationName, obj, e);
	}

	public static void AddObserver(this object obj, EventHandler handler, string notificationName)
	{
		CommunicationCenter.centerInstance.AddObserver(handler, notificationName);
	}
	public static void AddObserver(this object obj, EventHandler handler, string notificationName, object sender)
	{
		CommunicationCenter.centerInstance.AddObserver(handler, notificationName, sender);
	}

	public static void RemoveObserver(this object obj, string notificationName, EventHandler handler)
	{
		CommunicationCenter.centerInstance.RemoveObserver(handler, notificationName);
	}

	public static void RemoveObserver(this object obj, string notificationName, EventHandler handler, object sender)
	{
		CommunicationCenter.centerInstance.RemoveObserver(handler, notificationName, sender);
	}
}
