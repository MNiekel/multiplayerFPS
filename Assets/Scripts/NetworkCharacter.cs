using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkCharacter : Photon.MonoBehaviour {

	// for remote characters
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// for local characters
	public float speed = 5f;
	public float jumpSpeed = 5f;

	[System.NonSerialized]
	public Vector3 direction = Vector3.zero;
	[System.NonSerialized]
	public bool isJumping = false;
	[System.NonSerialized]
	public bool isShooting = false;

	private float verticalVelocity = 0f;
	private float smoothing = 0.4f;

	private float cooldown = 0.0f;

	// weapon stats
	private float fireRate = 0.5f;
	private float shootingDistance = 50.0f;
	private float weaponDamage = 20.0f;

	private Animator anim;
	private CharacterController characterController;
	private FXManager fxManager;
	
	void Start () {
		CacheComponents();
	}

	void CacheComponents () {
		if (anim == null) {
			anim = GetComponent<Animator> ();
		}
		if (characterController == null) {
			characterController = GetComponent<CharacterController> ();
		}
		if (fxManager == null) {
			fxManager = GameObject.FindObjectOfType <FXManager> ();
		}
	}

	// Called once per physics loop
	// Do ALL movement and physics stuff here
	void FixedUpdate () {
		if (photonView.isMine) {
			DoLocalMovement ();
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, smoothing);
		}
	}

	void Update () {
		if (photonView.isMine) {
			cooldown -= Time.deltaTime;
		}
	}
		
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		CacheComponents ();

		if (stream.isWriting) {
			// This is local player, we need to send our position and rotation
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(anim.GetFloat("Speed"));
			stream.SendNext(anim.GetBool("Jump"));
			stream.SendNext(anim.GetBool("Shooting"));
		} else {
			// This is remote player, we need to receive position and rotation and update our version of that player
			realPosition = (Vector3) stream.ReceiveNext ();
			realRotation = (Quaternion) stream.ReceiveNext ();
			anim.SetFloat("Speed", (float) stream.ReceiveNext());
			anim.SetBool("Jump", (bool) stream.ReceiveNext());
			anim.SetBool("Shooting", (bool) stream.ReceiveNext());
		}
	}

	private void DoLocalMovement() {

		Vector3 moveDistance = direction * speed * Time.deltaTime;

		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		moveDistance.y = verticalVelocity * Time.deltaTime;

		anim.SetFloat ("Speed", direction.magnitude);

		characterController.Move (moveDistance);

		if (isJumping) {
			isJumping = false;
			if (characterController.isGrounded) {
				verticalVelocity = jumpSpeed;
				anim.SetBool ("Jump", true);
			}
		} else {
			if (characterController.isGrounded) {
				verticalVelocity = 0;
				anim.SetBool ("Jump", false);
			}
		}

		if (isShooting) {
			isShooting = false;
			if (characterController.isGrounded && cooldown < 0) {
				anim.SetBool ("Shooting", true);
				Shoot ();
			}
		} else {
			anim.SetBool ("Shooting", false);
		}

	}

	private void Shoot() {
		if (cooldown > 0) {
			return;
		}

		Transform start = photonView.gameObject.transform.FindChild ("Main Camera");

		Ray ray = new Ray (start.position, start.forward);
		
		RaycastHit [] hits = Physics.RaycastAll (ray, shootingDistance).OrderBy(h=>h.distance).ToArray();

		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].transform != this.transform) {
				fxManager.GetComponent <PhotonView> ().RPC("ShootingFX", PhotonTargets.All,
				                                           start.position, hits [i].point);
				Hit(hits [i]);
				break;
			}
		}
		
		cooldown = fireRate;
	}

	private void Hit (RaycastHit hit) {
		Debug.Log (photonView.name +" shot " + hit.collider.name);
		if (hit.collider.tag == "Player") {
			TeamMember teamMember = hit.collider.GetComponent <TeamMember> () as TeamMember;
			if (teamMember.teamID != this.GetComponent <TeamMember> ().teamID || teamMember.teamID == 0) {
				teamMember.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, weaponDamage, PhotonNetwork.player.name);
			}
		} else {
			Health healthOfObject = hit.collider.GetComponent<Health> () as Health;
			if (healthOfObject != null) {
				healthOfObject.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.AllBuffered, weaponDamage);
			}
		}
	}
}
