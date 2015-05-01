using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	[RPC]
	void SpawnSceneObject (string name, Vector3 position) {
		if (PhotonNetwork.isMasterClient) {
			PhotonNetwork.InstantiateSceneObject (name, position, Quaternion.identity, 0, null);
		}
	}
}
