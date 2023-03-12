using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerObject : BaseObject
{
	public AudioSource damageAudio;
	PlayerController playerController;
	OceanFloating floating;

	public Slider lifeBar;
	public float life;

	bool deathState;
	public Bounds stageBound;

	private void Awake()
	{
		playerController = GetComponent<PlayerController>();
		floating = GetComponent<OceanFloating>();
		life = lifeBar.value;
		deathState = false;

		this.AddObserver(OnProcessList, Notifications.ProcessList);
		this.AddObserver(NotifyReference, Notifications.StartUpdate);
		this.AddObserver(OnEnemyAttack, Notifications.EnemyAttack);
		this.AddObserver(OnMeteorDeath, Notifications.GeneralDeath);
	}

	public void OnProcessList(object sender, EventArgs args)
	{
		List<BaseObject> l = ((MessageArgs)args).message as List<BaseObject>;
		if (!l.Contains(this))
		{ l.Add(this); }
	}

	public void NotifyReference(object sender, EventArgs args)
	{
		this.PostNotification(Notifications.PlayerReference, new MessageArgs(this));
	}

	public void OnEnemyAttack(object sender, EventArgs args)
	{
		if (life <= 0.1f) { life = 0; }
		else { life -= 0.1f; }
		lifeBar.value = life;
		if (!damageAudio.isPlaying) { damageAudio.Play(); }

		if (life == 0)
		{
			deathState = true;
			this.PostNotification(Notifications.PlayerDeath);
			StartCoroutine(Death());
		}
	}

	public void OnMeteorDeath(object sender, EventArgs args)
	{
		deathState = true;
		StartCoroutine(Death());
	}

	IEnumerator Death()
	{
		float t = 0;
		Quaternion c = transform.rotation;
		Quaternion e = Quaternion.Euler(180, 0, 0);
		while (t < 1.01f)
		{
			playerController.body.MoveRotation(Quaternion.Lerp(c, e, t));
			playerController.Move(Vector3.back);
			t += 1f * Time.fixedDeltaTime;
			yield return null;
		}
		SceneManager.LoadScene("GameOver");
	}

	public override void FrameUpdate()
	{
		if (deathState) return;

		playerController.ReadInputs();
		
		if(!stageBound.Contains(playerController.body.position))
		{ playerController.body.position = stageBound.ClosestPoint(playerController.body.position); }
	}

	public override void PhysicsUpdate()
	{
		floating.PhysicsUpdate();

		if (deathState) return;

		playerController.Move();
	}

	public override void PostUpdate()
	{

	}
}
