using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Only enabled for local player! Reads input from local player and pass results to NetworkCharacter
	private NetworkCharacter networkCharacter;

	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
	}

	void Update () {

		networkCharacter.direction = transform.rotation * new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));

		if (networkCharacter.direction.magnitude > 1) {
			networkCharacter.direction = networkCharacter.direction.normalized;
		}

		if (Input.GetButton ("Jump")) {
			networkCharacter.isJumping = true;
		} else {
			networkCharacter.isJumping = false;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, 100f, PhotonNetwork.player.name);
		}
	}
}
