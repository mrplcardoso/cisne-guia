using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SenderTable = System.Collections.Generic.Dictionary<
	System.Object, System.Collections.Generic.List<System.EventHandler>>;

public class CommunicationCenter
{
	//Singleton Pattern
	public static readonly CommunicationCenter centerInstance = new CommunicationCenter();
	private CommunicationCenter() { }

	//Tabela que relaciona uma mensagem (notificação) de um 'sender' (object) 
	//com uma lista de observadores (List<EventHandler>)
	private Dictionary<string, SenderTable> table = new Dictionary<string, SenderTable>();

	//Se a mensagem não existe (e não existem observadores e enviadores)
	//cria-se uma entrada no dicionário
	private SenderTable GetSenderTable(string notificationName, bool createIfNull = true)
	{
		if (!table.ContainsKey(notificationName))
		{ 
			if (createIfNull) { table.Add(notificationName, new SenderTable()); }
			else { Debug.LogError("Can't find table"); return null; }
		}
		return table[notificationName];
	}

	//Pega a lista de observadores vinculada a um enviador
	//Se não existir um 'sender', cria-se uma entrada para ele
	private List<EventHandler> GetObservers(SenderTable subTable, System.Object sender, bool addTableIfNull = true)
	{
		if (!subTable.ContainsKey(sender))
		{
			if (addTableIfNull) { subTable.Add(sender, new List<EventHandler>()); }
			else { Debug.LogError("Can't find observers"); return null; }
		}
		return subTable[sender];
	}

	public void AddObserver(EventHandler handler, string notificationName, System.Object sender = null)
	{
		if (handler == null)
		{ Debug.LogError("Can't add a null event handler for notification"); return; }
		if (string.IsNullOrEmpty(notificationName))
		{ Debug.LogError("Can't observe an unnamed notification"); return; }

		SenderTable subtable = GetSenderTable(notificationName);
		System.Object key = (sender != null) ? sender : this;
		List<EventHandler> list = GetObservers(subtable, key);
		if (!list.Contains(handler)) { list.Add(handler); }
	}

	public void RemoveObserver(EventHandler handler, string notificationName, System.Object sender)
	{
		if (string.IsNullOrEmpty(notificationName))
		{ Debug.LogError("A notification name is required to stop observation"); return; }
		if (!table.ContainsKey(notificationName)) { return; }

		SenderTable subTable = GetSenderTable(notificationName);
		System.Object key = (sender != null) ? sender : this;
		if (!subTable.ContainsKey(key)) { return; }
		List<EventHandler> list = GetObservers(subTable, key);
		for (int i = list.Count - 1; i >= 0; --i)
		{
			if (list[i] == handler)
			{
				list.RemoveAt(i);
				break;
			}
		}
		if (list.Count == 0)
		{
			subTable.Remove(key);
			if (subTable.Count == 0)
			{ table.Remove(notificationName); }
		}
	}

	public void RemoveObserver(EventHandler handler, string notificationName)
	{
		if (handler == null)
		{ Debug.LogError("Can't remove a null event handler from notification"); return; }
		if (string.IsNullOrEmpty(notificationName))
		{ Debug.LogError("A notification name is required to stop observation"); return; }
		if (!table.ContainsKey(notificationName)) { return; }

		System.Object[] keys = new object[table[notificationName].Keys.Count];
		table[notificationName].Keys.CopyTo(keys, 0);
		for (int i = keys.Length - 1; i >= 0; --i)
		{ RemoveObserver(handler, notificationName, keys[i]); }
	}

	public void RemoveObserver(EventHandler handler)
	{
		string[] key = new string[table.Keys.Count];
		table.Keys.CopyTo(key, 0);
		for (int i = key.Length - -1; i >= 0; --i)
		{ RemoveObserver(handler, key[i]); }
	}

	public void PostNotification(string notificationName, System.Object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(notificationName))
		{
			Debug.LogError("A notification name is required to stop observation");
			return;
		}
		// No need to take action if we dont monitor this notification
		if (!table.ContainsKey(notificationName))
			return;
		// Post to subscribers who specified a sender to observe
		SenderTable subTable = GetSenderTable(notificationName);
		if (sender != null && subTable.ContainsKey(sender))
		{
			List<EventHandler> handlers = GetObservers(subTable, sender);
			for (int i = handlers.Count - 1; i >= 0; --i)
				handlers[i](sender, e);
		}
		// Post to subscribers who did not specify a sender to observe
		if (subTable.ContainsKey(this))
		{
			List<EventHandler> handlers = GetObservers(subTable, this);
			for (int i = handlers.Count - 1; i >= 0; --i)
				handlers[i](sender, e);
		}
	}

	public void PostNotification(string notificationName, System.Object sender)
	{
		PostNotification(notificationName, sender, EventArgs.Empty);
	}

	public void PostNotification(string notificationName)
	{
		PostNotification(notificationName, null);
	}
}
