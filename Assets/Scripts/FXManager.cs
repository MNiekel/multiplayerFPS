using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	public GameObject shootingFXPrefab;
	public GameObject explosionFXPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC]
	private void ShootingFX (Vector3 startPoint, Vector3 endPoint) {
		Vector3 offset = new Vector3 (0f, -0.25f);
		GameObject shootingFX = Instantiate (shootingFXPrefab, startPoint + offset,
		                                     Quaternion.LookRotation(endPoint-startPoint)) as GameObject;
		LineRenderer lineFX = shootingFX.transform.Find ("LineFX").GetComponent<LineRenderer> ();
		lineFX.SetPosition (0, startPoint + offset);
		lineFX.SetPosition (1, endPoint);
	}

	[RPC]
	private void ExplosionFX (Vector3 point) {
		GameObject explosionFX = Instantiate (explosionFXPrefab, point,
		                                     Quaternion.identity) as GameObject;
	}
}
