using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject worldCamera;

	private SpawnPoint[] spawnPoints;
	private bool connecting = false;

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType <SpawnPoint> ();
		if (spawnPoints == null) {
			Debug.LogError ("No Spawnpoints!");
		}
		PhotonNetwork.player.name = PlayerPrefs.GetString ("USERNAME", "Sangatsuko");
	}

	void OnDestroy () {
		PlayerPrefs.SetString ("USERNAME", PhotonNetwork.player.name);
	}

	void Connect () {
		PhotonNetwork.ConnectUsingSettings ("MultiplayerFPS v0.0.1");
	}

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());



		if (!PhotonNetwork.connected && !connecting) {

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Your username: ");
			PhotonNetwork.player.name = GUILayout.TextField (PhotonNetwork.player.name, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Single player")) {
				connecting = true;
				PhotonNetwork.offlineMode = true;
				OnJoinedLobby ();
			}

			if (GUILayout.Button ("Multi player")) {
				connecting = true;
				Connect ();
			}
		}

	}

	void OnJoinedLobby () {
		Debug.Log ("Joined lobby");
		PhotonNetwork.JoinRandomRoom ();
	}

	void OnPhotonRandomJoinFailed () {
		Debug.Log ("Joining room failed, creating new room");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom () {
		Debug.Log ("Joined room");

		SpawnPlayer ();
	}

	void SpawnPlayer () {
		SpawnPoint spawnPoint = spawnPoints [Random.Range(0, spawnPoints.Length)];
		GameObject myPlayer = PhotonNetwork.Instantiate ("Player Controller",
		           spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as GameObject;
		worldCamera.SetActive (false);
		myPlayer.GetComponent <MouseLook> ().enabled = true;
		myPlayer.GetComponent <PlayerMovement> ().enabled = true;
		myPlayer.GetComponent <PlayerShooting> ().enabled = true;
		//((MonoBehaviour) myPlayer.GetComponent("FPSInputController")).enabled = true;
		//((MonoBehaviour) myPlayer.GetComponent("CharacterMotor")).enabled = true;
		myPlayer.GetComponentInChildren <Camera> ().enabled = true;
	}
}
