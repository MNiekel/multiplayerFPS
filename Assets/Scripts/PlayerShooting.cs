using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerShooting : MonoBehaviour {

	public float fireRate = 0.5f;
	public float shootingDistance = 50f;
	public float damage = 50f;

	//public NetworkManager networkManager;

	private float cooldown = 0f;
	private FXManager fxManager;
	private Animator anim;

	// Use this for initialization
	void Start () {
		fxManager = GameObject.FindObjectOfType <FXManager> ();
		if (fxManager == null) {
			Debug.LogError ("Could not find FXManager");
		}
		anim = GetComponent<Animator>();
		if (anim == null) {
			Debug.LogError ("Could not find Animator");
		}
	}
	
	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;

		if (Input.GetButton ("Fire1")) {
			if (cooldown < 0) {
				anim.SetBool ("Shooting", true);

				Shoot ();
			}
		} else {
			anim.SetBool ("Shooting", false);
		}
	}

	private void Shoot () {
		if (cooldown > 0) {
			return;
		}

		Transform start = Camera.main.transform;

		Ray ray = new Ray (start.position, start.forward);

		RaycastHit [] hits = Physics.RaycastAll (ray, shootingDistance).OrderBy(h=>h.distance).ToArray();

		if (hits.Length == 0) {
			fxManager.GetComponent <PhotonView> ().RPC("ShootingFX", PhotonTargets.All,
			                                   start.position, start.position + shootingDistance*start.forward);
		}

		if (hits.Length == 1 && hits [0].transform == this.transform) {
			fxManager.GetComponent <PhotonView> ().RPC("ShootingFX", PhotonTargets.All,
			                                           start.position, hits [0].point);
		}

		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].transform != this.transform) {
				Hit(hits [i]);
				fxManager.GetComponent <PhotonView> ().RPC("ShootingFX", PhotonTargets.All,
				                                           start.position, hits [i].point);
				break;
			}
		}

		cooldown = fireRate;
	}

	private void Hit (RaycastHit hit) {
		Debug.Log (PhotonNetwork.player.name +" shot " + hit.collider.name);
		if (hit.collider.tag == "Player") {
			TeamMember teamMember = hit.collider.GetComponent <TeamMember> () as TeamMember;
			if (teamMember.teamID != this.GetComponent <TeamMember> ().teamID || teamMember.teamID == 0) {
				teamMember.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, damage, PhotonNetwork.player.name);
			}
		} else {
			Health healthOfObject = hit.collider.GetComponent<Health> () as Health;
			if (healthOfObject != null) {
				healthOfObject.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, damage);
			}
		}
	}
	
}
