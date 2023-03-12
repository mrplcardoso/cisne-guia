using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
	public EnemyObject enemy;
	public Rigidbody body { get; private set; }
	public AudioSource attackAudio;
	public Animator weaponAnim;
	public Animator swamAnim;

	public float speed;
	public float interval;
	bool loopAttack;
	public bool canAttack;

	bool toTheLeft
	{
		get
		{
			float x = enemy.target.transform.position.x - transform.position.x;
			if (x < 0) { return true; } else { return false; }
		}
	}
	private void Start()
	{
		enemy = GetComponent<EnemyObject>();
		body = GetComponent<Rigidbody>();
		canAttack = false;
		loopAttack = true;
		StartCoroutine(Attack());
	}

	public void Move(Vector3 direction)
	{
		body.MovePosition(body.position + direction.normalized * speed * Time.fixedDeltaTime);
	}

	IEnumerator Attack()
	{
		WaitForSeconds wait = new WaitForSeconds(interval);

		while (loopAttack)
		{
			if (canAttack)
			{
				if (toTheLeft)
				{
					if (Random.Range(0, 2) > 0)
					{ weaponAnim.SetTrigger("attackLeft"); }
					else { swamAnim.SetTrigger("attackLeft"); }
				}
				else
				{
					if (Random.Range(0, 2) > 0)
					{ weaponAnim.SetTrigger("attackRight"); }
					else { swamAnim.SetTrigger("attackRight"); }
				}
				//this.PostNotification(Notifications.EnemyAttack);
				StartCoroutine(PostAttack());
				canAttack = false;
			}
			yield return wait;
		}
	}

	IEnumerator PostAttack()
	{
		yield return new WaitForSeconds(0.5f);
		if (!attackAudio.isPlaying) { attackAudio.Play(); }
		this.PostNotification(Notifications.EnemyAttack);
	}
}
