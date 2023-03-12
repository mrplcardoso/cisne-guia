using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : BaseObject
{
	public AudioSource damageAudio;
	public EnemyController enemyController;
	OceanFloating floating;

	public BaseObject target;
	PlayerObject playerTarget
	{ get { return (PlayerObject)target; } }
	Vector3 targetDirection
	{ 
		get 
		{ 
			Vector3 d = (target.transform.position - gameObject.transform.position);
			d.y = 0;
			return d.normalized;
		} 
	}
	float targetDistance
	{ get { return Vector3.Distance(target.transform.position, gameObject.transform.position); } }

	Vector3 targetNearPosition
	{
		get 
		{
			Vector3 n = target.transform.position;
			float x = target.transform.position.x - transform.position.x;
			x = (x / Mathf.Abs(x)) * -seekDistance;
			n.x += x;
			return n;
		}
	}

	Vector3 nearDirection
	{
		get
		{
			Vector3 d = (targetNearPosition - gameObject.transform.position);
			d.y = 0;
			return d.normalized;
		}
	}
	public float seekDistance;

	bool toTheLeft
	{
		get
		{
			float x = target.transform.position.x - transform.position.x;
			if (x > 0) { return true; } else { return false; }
		}
	}

	public int index;
	public int life;
	public bool deathState;
	public bool playerDead;

	void Awake()
	{
		enemyController = GetComponent<EnemyController>();
		floating = GetComponent<OceanFloating>();
		this.AddObserver(GetTarget, Notifications.PlayerReference);
		this.AddObserver(OnPlayerAttack, Notifications.PlayerAttack);
		this.AddObserver(OnPlayerDeath, Notifications.PlayerDeath);
		this.AddObserver(OnMeterDeath, Notifications.GeneralDeath);
		playerDead = false;
		life = 2;
	}

	public void GetTarget(object sender, EventArgs args)
	{
		target = (BaseObject)((MessageArgs)args).message;
	}

	public void OnPlayerDeath(object sender, EventArgs args)
	{
		playerDead = true;
	}

	public void OnPlayerAttack(object sender, EventArgs args)
	{
		int side = (int)((MessageArgs)args).message;
		if (targetDistance <= seekDistance+0.5f)
		{
			if (toTheLeft && side < 0)
			{
				if (!damageAudio.isPlaying) { damageAudio.Play(); }
				life--;
				if(life <= 0)
				{
					StartCoroutine(Death(-1));
				}
			}
			else if (!toTheLeft && side > 0)
			{
				if (!damageAudio.isPlaying) { damageAudio.Play(); }
				life--;
				if (life <= 0)
				{
					StartCoroutine(Death(1));
				}
			}
		}
	}

	public void OnMeterDeath(object sender, EventArgs args)
	{
		deathState = true;
		if (toTheLeft)
		{
			StartCoroutine(Death(-1));
		}
		else
		{
			StartCoroutine(Death(1));
		}
	}

	IEnumerator Death(int side)
	{
		deathState = true;
		float t = 0;
		Quaternion c = transform.rotation;
		Quaternion e = Quaternion.Euler(0, 0, side*180);
		while(t < 1.01f)
		{
			enemyController.body.MoveRotation(Quaternion.Lerp(c, e, t));
			t += 1f * Time.fixedDeltaTime;
			enemyController.body.MovePosition(enemyController.body.position + 
				new Vector3(side, 0, -1).normalized * 4f * Time.fixedDeltaTime);
			yield return null;
		}
		this.PostNotification(Notifications.EnemyDeath);
		gameObject.SetActive(false);
	}

	public override void FrameUpdate()
	{
		Debug.DrawLine(target.transform.position, targetNearPosition);
	}

	public override void PhysicsUpdate()
	{
		floating.PhysicsUpdate();

		if (deathState || playerDead) { return; }

		if (targetDistance > seekDistance)
		{ enemyController.Move(nearDirection); }
		else { enemyController.canAttack = true; }
	}

	public override void PostUpdate()
	{

	}
}
