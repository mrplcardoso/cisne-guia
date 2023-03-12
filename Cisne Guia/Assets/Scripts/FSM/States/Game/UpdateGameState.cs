using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpdateGameState : GameState
{
	[SerializeField]
	OceanController oceanPrefab;
	OceanController oceanController;

	[SerializeField]
	List<BaseObject> processList;

	public GameObject meteor;
	public AudioSource meteorAudio;

	public Image meteorSplash;
	public Image routeArrow;
	int routePoint;

	bool doUpdate = false;
	bool meteorInstanced = false;
	bool generalDeath = false;

	private void Start()
	{
		oceanController = Instantiate(oceanPrefab);
		this.PostNotification(Notifications.OceanReference, new MessageArgs(oceanController));

		processList = new List<BaseObject>();
		this.PostNotification(Notifications.ProcessList, new MessageArgs(processList));
	}

	public override void OnEnter()
	{
		this.PostNotification(Notifications.StartUpdate);
		routePoint = 162;
		StartCoroutine(MoveArrow());
		doUpdate = true;
	}

	IEnumerator MoveArrow()
	{
		while (routeArrow.rectTransform.localPosition.y < routePoint)
		{
			routeArrow.rectTransform.position += new Vector3(0, 2f * Time.deltaTime, 0);

			if(!meteorInstanced && routeArrow.rectTransform.localPosition.y > routePoint/2f)
			{ StartCoroutine(LaunchMeteor()); meteorInstanced = true; }

			yield return null;
		}
	}

	IEnumerator LaunchMeteor()
	{
		if (!meteorAudio.isPlaying) { meteorAudio.Play(); }
		yield return new WaitForSeconds(3f);
		meteor = Instantiate(meteor, new Vector3(0, 10, -10), Quaternion.identity);
		Vector3 d = (new Vector3(0, 0.5f, -35) - new Vector3(0, 10, -10)).normalized;
		while(meteor.transform.position.z > -50)
		{
			if(meteor.transform.position.z < -20)
			{
				Color c = meteorSplash.color;
				c.a += 0.3f*Time.deltaTime;
				meteorSplash.color = c;
			}
			if (!generalDeath && meteor.transform.position.z < -30)
			{
				generalDeath = true;
				this.PostNotification(Notifications.GeneralDeath);
			}
			meteor.transform.position += d * 15 * Time.deltaTime;
			yield return null;
		}

		//SceneManager.LoadScene("GameOver");
	}

	private void Update()
	{
		if (!doUpdate) return;
		oceanController.FrameUpdate();

		for(int i = 0; i < processList.Count; ++i)
		{
			if (!processList[i].gameObject.activeSelf) { continue; }

			processList[i].FrameUpdate(); 
		}
	}

	private void FixedUpdate()
	{
		if (!doUpdate) return;

		for (int i = 0; i < processList.Count; ++i)
		{
			if (!processList[i].gameObject.activeSelf) { continue; }

			processList[i].PhysicsUpdate(); 
		}
	}

	public override void OnExit()
	{
		doUpdate = false;
	}
}
