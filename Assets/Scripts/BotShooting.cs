using UnityEngine;
using System.Collections;

public class BotShooting : MonoBehaviour {

	private NetworkCharacter networkCharacter;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
	}
	
	void Update () {

		networkCharacter.isShooting = EvaluateShooting ();
	}

	private bool EvaluateShooting () {

		Ray ray = new Ray (transform.position, transform.forward);
		
		RaycastHit [] hits = Physics.RaycastAll (ray);
		

		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].collider.tag == "Player") {
				return true;
			}
		}

		return false;
	}
}

