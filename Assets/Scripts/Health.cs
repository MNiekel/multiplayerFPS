using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;

	private float currentHitPoints;

	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage (float damage) {
		currentHitPoints -= damage;

		if (currentHitPoints <= 0) {
			Die ();
		}
	}

	public void Die () {
		Destroy (gameObject);
	}
}
