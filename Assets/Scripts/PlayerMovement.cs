using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Only enabled for local player! Reads input from local player and pass results to NetworkCharacter
	private CharacterController cc;
	private Animator anim;
	private NetworkCharacter networkCharacter;


	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
		anim = GetComponent<Animator> ();
	}

	void Update () {

		networkCharacter.direction = transform.rotation * new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));

		if (networkCharacter.direction.magnitude > 1) {
			networkCharacter.direction = networkCharacter.direction.normalized;
		}

		anim.SetFloat ("Speed", networkCharacter.direction.magnitude);

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
