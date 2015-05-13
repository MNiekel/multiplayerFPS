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

	/*
	void OnGUI() {
		if (GetComponent <PhotonView> ().isMine && gameObject.tag == "Player") {
			if (GUI.Button (new Rect (10, 20, 60, 20), "Suicide")) {
				Debug.Log (PhotonNetwork.player.name + " commits suicide");
				Die ();
			}
		}
	}
	*/

	[RPC]
	public void TakeDamage (float damage) {
		currentHitPoints -= damage;

		if (currentHitPoints <= 0) {
			Die ();
		}
	}

	[RPC]
	public void TakeDamage (float damage, string killer) {
		currentHitPoints -= damage;
		
		if (currentHitPoints <= 0) {
			Die (killer);
		}
	}

	private void Die (string killer = "") {
		if (gameObject.tag == "Exploder") {
			if (GetComponent <PhotonView>().isMine) {
				fxManager.GetComponent <PhotonView> ().RPC("ExplosionFX", PhotonTargets.All,
			                                           transform.position);

				WeaponData weaponData = GetComponent <WeaponData> ();
				TeamMember[] teamMembers = FindObjectsOfType<TeamMember> ();
				foreach (TeamMember teamMember in teamMembers) {
					if (Vector3.Distance(gameObject.transform.position, teamMember.transform.position) < weaponData.range) {

						teamMember.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, weaponData.damage, "Explosion");
					}
				}
			}
		}
		
		if (GetComponent <PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} else {
			if (GetComponent <PhotonView> ().isMine) {
				if (gameObject.tag == "Player") {
					networkManager.AddChatMessage(killer +" has killed "+ PhotonNetwork.player.name);
					networkManager.worldCamera.SetActive (true);
					networkManager.respawnTimer = 3f;
				} else {
					if (gameObject.tag == "Bot") {
						networkManager.AddChatMessage(killer +" has killed "+gameObject.GetPhotonView().name);
						networkManager.botRespawnTimer = 3f;
					}
				}

				PhotonNetwork.Destroy (gameObject);
			}
		}
	}

}
