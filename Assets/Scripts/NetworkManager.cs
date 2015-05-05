using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviour {

	public GameObject worldCamera;

	private float respawn = 0;
	private SpawnPoint[] spawnPoints;
	private Waypoint[] wayPoints;
	private bool connecting = false;
	private List<string> messages;
	private int maxNumberOfMessages = 5;
	private Spawner objectSpawner;

	void Start () {
		spawnPoints = FindObjectsOfType <SpawnPoint> ();
		if (spawnPoints == null) {
			Debug.LogError ("No Spawnpoints!");
		}
		objectSpawner = GameObject.FindObjectOfType <Spawner> ();
		wayPoints = FindObjectsOfType <Waypoint> ();
		if (objectSpawner == null) {
			Debug.LogError ("Could not find NetworkManager");
		}

		PhotonNetwork.player.name = PlayerPrefs.GetString ("USERNAME", "Sangatsuko");

		messages = new List<string> (maxNumberOfMessages);
	}

	void Update () {
		if (respawn > 0) {
			respawn -= Time.deltaTime;

			if (respawn <= 0) {
				SpawnPlayer ();
			}
		}
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

		if (respawn > 0) {
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();

			GUILayout.Label ("Respawn in "+respawn.ToString("0.00")+" seconds");

			GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}

	}

	void OnJoinedLobby () {
		Debug.Log ("Joined lobby");
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 8 };
		PhotonNetwork.JoinOrCreateRoom ("Nez FPS", roomOptions, TypedLobby.Default);
	}

	void OnPhotonRandomJoinFailed () {
		Debug.Log ("Joining room failed, creating new room");
		PhotonNetwork.CreateRoom (null);
	}

	void OnJoinedRoom () {
		Debug.Log ("Joined room " +PhotonNetwork.room.name);
		connecting = false;
		AddChatMessage (PhotonNetwork.player.name + " has entered the room");

		if (PhotonNetwork.isMasterClient) {
			SpawnSceneObjects();
			SpawnBots ();
		}

		if (!PhotonNetwork.offlineMode) {
			AutoTeamSelect ();
		}
		SpawnPlayer ();
	}

	void SpawnPlayer () {
		int spawnPointNumber = Random.Range(0, spawnPoints.Length);

		int teamID = 0;
		object ID;
		
		if (PhotonNetwork.player.customProperties.TryGetValue ("Team", out ID)) {
			teamID = (int)ID;
		}

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

	private void SpawnBots () {
		SpawnPoint spawnPoint = spawnPoints [Random.Range(spawnPoints.Length / 2, spawnPoints.Length)];
		GameObject bot = PhotonNetwork.Instantiate ("Bot Controller",
		                                                spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as GameObject;
		bot.GetComponent <BotMovement> ().enabled = true;
	}

	private void SpawnSceneObjects () {
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "barrel", new Vector3 (-60f, 0.5f, 7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "crate", new Vector3 (-60f, 0.5f, -7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "barrel", new Vector3 (60f, 0.5f, 7f));
		objectSpawner.GetComponent <PhotonView> ().RPC ("SpawnSceneObject", PhotonTargets.All,
		                                                "crate", new Vector3 (60f, 0.5f, -7f));
	}

	private void AutoTeamSelect () {
		PhotonPlayer[] players = PhotonNetwork.playerList;
		int players1 = 0;
		int players2 = 0;
		int teamID = Random.Range (1, 3);
		Hashtable setPlayerTeam = new Hashtable ();

		Debug.Log ("Number of players: " + players.Length);

		foreach (PhotonPlayer player in players) {

			object ID;
			if (player.customProperties.TryGetValue("Team", out ID)) {
				if ((int)ID == 1) {
					players1++;
				}
				if ((int)ID == 2) {
					players2++;
				}

				Debug.Log(player.name +" has team ID "+ID.ToString());
			}
		}

		if (players1 > 0 || players2 > 0) {
			if (players1 > players2) {
				teamID = 2;
			} else {
				if (players2 > players1) {
					teamID = 1;
				}
			}
		}

		setPlayerTeam.Add ("Team", teamID);
		PhotonNetwork.player.SetCustomProperties (setPlayerTeam);

		Debug.Log(PhotonNetwork.player.name +" has team ID "+ teamID.ToString());
		AddChatMessage (PhotonNetwork.player.name + " plays for team " + teamID);
	}

	public float respawnTimer {
		get { return respawn; }
		set { respawn = value; }
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

}
