using UnityEngine;
using System.Collections;

public class WeaponData : MonoBehaviour {
	public float range = 50f;
	public float fireRate = 0.5f;
	public float damage = 50f;

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
