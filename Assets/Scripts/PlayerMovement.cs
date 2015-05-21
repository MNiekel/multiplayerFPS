using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
			NetworkManager networkManager = GameObject.FindObjectOfType <NetworkManager> ();
			//networkManager.AddChatMessage("Someone has left the game"); //does not work
			networkManager.Leave(PhotonNetwork.player);

		}
	}
}
