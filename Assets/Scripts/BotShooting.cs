using UnityEngine;
using System.Collections;

public class BotShooting : MonoBehaviour {

	private NetworkCharacter networkCharacter;

	private float shotTimer;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
		shotTimer = 2f; //Random.Range (3f, 5f);
	}
	
	void Update () {
		
		shotTimer -= Time.deltaTime;

		if (shotTimer < 0) {
			networkCharacter.isShooting = true;
			shotTimer = Random.Range (0.5f, 2.5f);
		} else {
			networkCharacter.isShooting = false;
		}
	}
}
