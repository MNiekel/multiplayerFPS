using UnityEngine;
using System.Collections;

public class PickupData : MonoBehaviour {

	public float health = 50f;

	[RPC]
	public void DestroyObject() {
		if (GetComponent <PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} else {
			if (GetComponent <PhotonView> ().isMine) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}
}
