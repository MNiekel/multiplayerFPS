using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	private NetworkManager networkManager;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC]
	void SpawnSceneObject (string name, Vector3 position) {
		Debug.Log ("Spawner");
		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("isMasterClient");
			PhotonNetwork.InstantiateSceneObject (name, position, Quaternion.identity, 0, null);
		} else {
			Debug.Log ("not masterclient");
		}
		//PhotonNetwork.InstantiateSceneObject ("Barrel", new Vector3(-60f, 0.5f, 7f), Quaternion.identity, 0, null);
		//PhotonNetwork.InstantiateSceneObject ("Crate", new Vector3(-59f, 0.5f, -7f), Quaternion.identity, 0, null);
	}
}
