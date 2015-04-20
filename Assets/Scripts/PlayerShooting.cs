using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;

	private float cooldown = 0f;

	// Use this for initialization
	void Start () {
	
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
				break;
			}
		}

		cooldown = fireRate;
	}

	private void Hit (RaycastHit hit) {
		Debug.Log ("We hit: " + hit.collider.name);
		Health healthOfHit = hit.collider.GetComponent<Health> () as Health;
		if (healthOfHit != null) {
			healthOfHit.TakeDamage (5);
		}
	}
	
}
