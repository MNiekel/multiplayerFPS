using UnityEngine;
using System.Collections;

public class KillByDrop : MonoBehaviour {

	void OnTriggerEnter (Collider collider) {

		string killer;

		Health healthOfObject = collider.GetComponent<Health> () as Health;
		if (healthOfObject != null) {
			healthOfObject.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.All, 1000f, "Edge");
		}
	}
}
