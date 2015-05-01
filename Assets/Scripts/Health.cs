using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;

	private float currentHitPoints;
	private FXManager fxManager;
	private NetworkManager networkManager;

	void Start () {
		currentHitPoints = hitPoints;

		fxManager = GameObject.FindObjectOfType <FXManager> ();
		if (fxManager == null) {
			Debug.LogError ("Could not find FXManager");
		}
		networkManager = GameObject.FindObjectOfType <NetworkManager> ();
		if (networkManager == null) {
			Debug.LogError ("Could not find NetworkManager");
		}
	}

	void OnGUI() {
		if (GetComponent <PhotonView> ().isMine && gameObject.tag == "Player") {
			if (GUI.Button (new Rect (Screen.width - 100, 10, 80, 40), "Suicide")) {
				Debug.Log (PhotonNetwork.player.name + " commits suicide");
				Die ();
			}
		}
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
			fxManager.GetComponent <PhotonView> ().RPC("ExplosionFX", PhotonTargets.All,
			                                           transform.position);
		}

		if (GetComponent <PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} else {
			if (GetComponent <PhotonView> ().isMine) {
				if (gameObject.tag == "Player") {
					networkManager.AddChatMessage(PhotonNetwork.player.name + " has died...");
					networkManager.worldCamera.SetActive (true);
					networkManager.respawnTimer = 3f;
				}
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}
}
