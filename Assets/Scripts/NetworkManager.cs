using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public Camera worldCamera;

	private SpawnPoint[] spawnPoints;

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType <SpawnPoint> ();
		if (spawnPoints == null) {
			Debug.LogError ("No Spawnpoints!");
		}
		Connect ();
	}

	void Connect () {
		//PhotonNetwork.offlineMode = true;
		PhotonNetwork.ConnectUsingSettings ("MultiplayerFPS v0.0.1");
	}

	void OnGUI () {
		GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
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
		worldCamera.enabled = false;
		myPlayer.GetComponent <MouseLook> ().enabled = true;
		((MonoBehaviour) myPlayer.GetComponent("FPSInputController")).enabled = true;
		myPlayer.GetComponentInChildren <Camera> ().enabled = true;
	}
}
