using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	public GameObject shootingFXPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC]
	private void ShootingFX (Vector3 startPoint, Vector3 endPoint) {
		Debug.Log ("BulletFX");
		GameObject shootingFX = Instantiate (shootingFXPrefab, startPoint, Quaternion.identity) as GameObject;
		LineRenderer lineFX = shootingFX.transform.Find ("LineFX").GetComponent<LineRenderer> ();
		lineFX.SetPosition (0, startPoint);
		lineFX.SetPosition (1, endPoint);
	}
}
