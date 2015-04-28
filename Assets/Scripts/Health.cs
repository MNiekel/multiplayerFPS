using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;

	private float currentHitPoints;
	private FXManager fxManager;

	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;

		fxManager = GameObject.FindObjectOfType <FXManager> ();
		if (fxManager == null) {
			Debug.LogError ("Could not find FXManager");
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	[RPC]
	public void TakeDamage (float damage) {
		currentHitPoints -= damage;

		if (currentHitPoints <= 0) {
			Die ();
		}
	}

	public void Die () {
		if (gameObject.tag == "Exploder") {
			Debug.Log ("BOOM!");
			fxManager.GetComponent <PhotonView> ().RPC("ExplosionFX", PhotonTargets.All,
			                                           transform.position);
		}

		if (GetComponent <PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} else {
			if (GetComponent <PhotonView> ().isMine) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}
}
