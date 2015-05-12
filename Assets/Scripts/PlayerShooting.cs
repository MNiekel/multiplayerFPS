using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	// Only enabled for local player! Reads input from local player and pass results to NetworkCharacter
	private NetworkCharacter networkCharacter;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
	}
	
	void Update () {
		
		if (Input.GetButton ("Fire1")) {
			networkCharacter.isShooting = true;
		} else {
			networkCharacter.isShooting = false;
		}

	}
}
