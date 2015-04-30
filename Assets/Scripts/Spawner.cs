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
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.InstantiateSceneObject (name, position, Quaternion.identity, 0, null);
		}
	}
}
