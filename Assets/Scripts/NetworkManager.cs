using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameObject worldCamera;

	private SpawnPoint[] spawnPoints;
	private bool connecting = false;
	private List<string> messages;
	private int maxNumberOfMessages = 5;

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType <SpawnPoint> ();
		if (spawnPoints == null) {
			Debug.LogError ("No Spawnpoints!");
		}
		PhotonNetwork.player.name = PlayerPrefs.GetString ("USERNAME", "Sangatsuko");

		messages = new List<string> (maxNumberOfMessages);
	}


	public void AddChatMessage (string message) {
		GetComponent<PhotonView> ().RPC ("AddChatMessage_RPC", PhotonTargets.AllBuffered, message);
	}

	[RPC]
	private void AddChatMessage_RPC (string message) {
		while (messages.Count >= maxNumberOfMessages) {
			messages.RemoveAt (0);
		}

		messages.Add (message);
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

			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();

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

			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}

		if (PhotonNetwork.connected && !connecting) {
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			foreach (string message in messages) {
				GUILayout.Label (message);
			}
			GUILayout.EndVertical ();
			GUILayout.EndArea();
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
		connecting = false;
		AddChatMessage (PhotonNetwork.player.name + " has entered the room");

		SpawnPlayer ();
		SpawnObjects ();
	}

	void SpawnPlayer () {
		SpawnPoint spawnPoint = spawnPoints [Random.Range(0, spawnPoints.Length)];
		GameObject myPlayer = PhotonNetwork.Instantiate ("Player Controller",
		           spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as GameObject;
		worldCamera.SetActive (false);
		myPlayer.GetComponent <PlayerMovement> ().enabled = true;
		myPlayer.GetComponent <PlayerShooting> ().enabled = true;
		myPlayer.GetComponentInChildren <Camera> ().enabled = true;
		myPlayer.GetComponentInChildren <AudioListener> ().enabled = true;
		MouseLook[] mouseScriptList = myPlayer.GetComponentsInChildren <MouseLook> ();
		foreach (MouseLook script in mouseScriptList) {
			script.enabled = true;
		}
	}

	void SpawnObjects () {
		Debug.Log ("Spawning Barrel");
		//PhotonNetwork.Instantiate  ("Barrel", new Vector3(-65f, -1f, 8f), Quaternion.identity, 1);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, -7f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, -5f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, -3f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, 3f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, 5f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, 7f), Quaternion.identity, 0, null);
		PhotonNetwork.InstantiateSceneObject ("Crate", new Vector3(-59f, 0.5f, 7f), Quaternion.identity, 0, null);
	}
}
