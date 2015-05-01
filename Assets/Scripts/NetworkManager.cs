using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameObject worldCamera;
	public float respawnTimer;

	private SpawnPoint[] spawnPoints;
	private bool connecting = false;
	private List<string> messages;
	private int maxNumberOfMessages = 5;
	private Spawner objectSpawner;
	private int teamID = -1;
	private bool teamSelected = false;

	// Use this for initialization
	void Start () {
		spawnPoints = FindObjectsOfType <SpawnPoint> ();
		if (spawnPoints == null) {
			Debug.LogError ("No Spawnpoints!");
		}
		objectSpawner = GameObject.FindObjectOfType <Spawner> ();
		if (objectSpawner == null) {
			Debug.LogError ("Could not find NetworkManager");
		}

		PhotonNetwork.player.name = PlayerPrefs.GetString ("USERNAME", "Sangatsuko");

		messages = new List<string> (maxNumberOfMessages);
	}

	void Update () {
		if (respawnTimer > 0) {
			respawnTimer -= Time.deltaTime;

			if (respawnTimer <= 0) {
				SpawnPlayer ();
			}
		}
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
				teamID = 0;
				OnJoinedLobby ();
			}

			if (GUILayout.Button ("Multi player")) {
				connecting = true;
				//Connect ();
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}

		if (!PhotonNetwork.connected && connecting && !teamSelected) {
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace ();
			
			if (GUILayout.Button ("Red Team")) {
				teamID = 2;
				teamSelected = true;
				Connect ();
			}
			
			if (GUILayout.Button ("Yellow Team")) {
				teamID = 1;
				teamSelected = true;
				Connect ();
			}
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}

		if (PhotonNetwork.connected && !connecting && teamSelected) {
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

		if (PhotonNetwork.isMasterClient) {
			SpawnSceneObjects();
		}
		SpawnPlayer ();
	}

	void OnLeftRoom() {
		Debug.Log ("Left room");
		connecting = true;
		//AddChatMessage (PhotonNetwork.player.name + " has left the room");
		PhotonNetwork.LoadLevel ("scene01");
	}

	void SpawnPlayer () {
		int spawnPointNumber = Random.Range(0, spawnPoints.Length);

		if (teamID == 1) {
			spawnPointNumber = Random.Range(spawnPoints.Length / 2, spawnPoints.Length);
		} else {
			if (teamID == 2) {
				spawnPointNumber = Random.Range(0, spawnPoints.Length / 2);
			}
		}

		SpawnPoint spawnPoint = spawnPoints [spawnPointNumber];
		
		GameObject myPlayer = PhotonNetwork.Instantiate ("Player Controller",
		           spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as GameObject;
		worldCamera.SetActive (false);
		myPlayer.GetComponent <PlayerMovement> ().enabled = true;
		myPlayer.GetComponent <PlayerShooting> ().enabled = true;
		myPlayer.GetComponent <MouseLook> ().enabled = true;

		myPlayer.GetComponent <PhotonView> ().RPC ("SetTeamID", PhotonTargets.AllBuffered, teamID);

		myPlayer.transform.FindChild ("Main Camera").gameObject.SetActive (true);

	}

	void SpawnSceneObjects () {
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "barrel", new Vector3 (-60f, 0.5f, 7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "crate", new Vector3 (-60f, 0.5f, -7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "barrel", new Vector3 (60f, 0.5f, 7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "crate", new Vector3 (60f, 0.5f, -7f));
	}

}
