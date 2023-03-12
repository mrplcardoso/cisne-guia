using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	public List<EnemyObject> enemyPrefab;
	public List<EnemyObject> enemyList
	{ get; private set; }

	public Vector3 startPosition;
	public Vector3 enterScene;

	public int instances;
	int instanceIndex;
	int instancedCount;

	public float intervalTime;
	public bool endSpawn;
	public bool pauseSpawn;

	private void Start()
	{
		enemyList = new List<EnemyObject>();
		startPosition = Vector3.left * 1000f;

		instanceIndex = 0;
		instancedCount = 0;

		for(int i = 0; i < instances; ++i)
		{
			int r = UnityEngine.Random.Range(0, 2);
			EnemyObject en = Instantiate(enemyPrefab[r], startPosition, Quaternion.identity);
			en.index = i;
			enemyList.Add(en);
			en.deathState = false;
			en.gameObject.SetActive(false);
		}

		this.AddObserver(OnProcessList, Notifications.ProcessList);
		this.AddObserver(TriggerSpawn, Notifications.StartUpdate);
		this.AddObserver(OnEnemyDeath, Notifications.EnemyDeath);
	}

	public void OnEnemyDeath(object sender, EventArgs args)
	{
		if (instancedCount > 0)
		{ instancedCount--; }
	}

	public void OnProcessList(object sender, EventArgs args)
	{
		List<BaseObject> l = ((MessageArgs)args).message as List<BaseObject>;
		for(int i = 0; i < enemyList.Count; ++i)
		{
			if (!l.Contains(enemyList[i]))
			{ l.Add(enemyList[i]); }
		}
	}

	public void TriggerSpawn(object sender, EventArgs args)
	{
		endSpawn = pauseSpawn = false;
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		WaitForSeconds wait = new WaitForSeconds(intervalTime);
		float n = 5;
		while(!endSpawn)
		{
			if(!pauseSpawn)
			{
				if (instancedCount < 2)
				{
					if (instanceIndex >= enemyList.Count)
					{ instanceIndex = 0; }

					EnemyObject en = enemyList[instanceIndex];
					instanceIndex++;
					if (!en.gameObject.activeSelf)
					{ 
						en.gameObject.SetActive(true);
						enterScene = new Vector3(n, 0.5f, -35);
						en.transform.position = enterScene;
						en.transform.rotation = Quaternion.identity;
						en.deathState = false;
						n *= -1;
						instancedCount++; 
					}
				}
			}
			yield return wait;
		}
	}
}
