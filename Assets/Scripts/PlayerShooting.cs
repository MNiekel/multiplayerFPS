using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;
	public float damage = 50f;

	//public NetworkManager networkManager;

	private float cooldown = 0f;
	private FXManager fxManager;

	// Use this for initialization
	void Start () {
		fxManager = GameObject.FindObjectOfType <FXManager> ();
		if (fxManager == null) {
			Debug.LogError ("Could not find FXManager");
		}
	}
	
	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;

		if (Input.GetButton ("Fire1")) {
			Shoot ();
		}
	}

	private void Shoot () {
		if (cooldown > 0) {
			return;
		}
		Ray ray = new Ray (Camera.main.transform.position, Camera.main.transform.forward);

		RaycastHit hit;

		RaycastHit [] hits = Physics.RaycastAll (ray).OrderBy(h=>h.distance).ToArray();

		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].transform != this.transform) {
				Hit(hits [i]);
				fxManager.GetComponent <PhotonView> ().RPC("ShootingFX", PhotonTargets.All,
				                                           Camera.main.transform.position,
				                                           hits [i].transform.position);
				break;
			}
		}

		cooldown = fireRate;
	}

	private void Hit (RaycastHit hit) {
		Debug.Log ("We hit: " + hit.collider.name);
		Health healthOfHit = hit.collider.GetComponent<Health> () as Health;
		if (healthOfHit != null) {
			healthOfHit.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, damage);
		}
	}
	
}
