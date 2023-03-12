using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
	public Rigidbody body { get; private set; }
	public AudioSource attackAudio;
	public Animator weaponAnim;
	public Animator swamAnim;

	Vector3 velocity;
	Vector3 forward, strafe;

	public float speed;
	public bool lockForward;
	bool canAttack;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		canAttack = true;
	}

	public void ReadInputs()
	{
		strafe = Input.GetAxis("Horizontal") * transform.right;
		if (!lockForward)
		{ forward = Input.GetAxis("Vertical") * transform.forward; }

		velocity = lockForward ? strafe : forward + strafe;

		ReadAttack();
	}

	void ReadAttack()
	{
		if (Input.GetAxisRaw("Fire1") != 0 && canAttack)
		{ 
			canAttack = false; 
			weaponAnim.SetTrigger("attackLeft");
			//this.PostNotification(Notifications.PlayerAttack, new MessageArgs(-1));
			StartCoroutine(PostAttack(-1));
		}
		else if (Input.GetAxisRaw("Fire2") != 0 && canAttack) 
		{ 
			canAttack = false; 
			weaponAnim.SetTrigger("attackRight");
			//this.PostNotification(Notifications.PlayerAttack, new MessageArgs(1));
			StartCoroutine(PostAttack(1));
		}
		else if (Input.GetAxisRaw("Fire3") != 0 && canAttack) 
		{ 
			canAttack = false; 
			swamAnim.SetTrigger("attackLeft");
			//this.PostNotification(Notifications.PlayerAttack, new MessageArgs(-1));
			StartCoroutine(PostAttack(-1));
		}
		else if (Input.GetAxisRaw("Fire4") != 0 && canAttack) 
		{ 
			canAttack = false; 
			swamAnim.SetTrigger("attackRight");
			//this.PostNotification(Notifications.PlayerAttack, new MessageArgs(1));
			StartCoroutine(PostAttack(1));
		}
		
		/*if(Input.GetAxisRaw("Fire1") == 0 && Input.GetAxisRaw("Fire2") == 0
			&& Input.GetAxisRaw("Fire3") == 0 && Input.GetAxisRaw("Fire4") == 0)
		{ canAttack = true; }*/
	}

	public void Move()
	{
		body.MovePosition(body.position + velocity.normalized * speed * Time.fixedDeltaTime);
	}

	public void Move(Vector3 newVelocity)
	{
		body.MovePosition(body.position + newVelocity.normalized *
			speed * Time.fixedDeltaTime);
	}

	IEnumerator PostAttack(int side)
	{
		yield return new WaitForSeconds(0.5f);
		this.PostNotification(Notifications.PlayerAttack, new MessageArgs(side));
		if (!attackAudio.isPlaying) { attackAudio.Play(); }
		yield return new WaitForSeconds(0.25f);
		canAttack = true;
	}
}
