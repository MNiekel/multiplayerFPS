using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	// for remote characters
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	// for local characters
	public float speed = 10f;
	public float jumpSpeed = 5f;

	[System.NonSerialized]
	public Vector3 direction = Vector3.zero;
	[System.NonSerialized]
	public bool isJumping = false;

	private float verticalVelocity = 0f;
	private float smoothing = 0.4f;
	private Animator anim;
	private CharacterController characterController;
	
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
	}

	// Called once per physics loop
	// Do ALL movement and physics stuff here
	void FixedUpdate () {
		if (photonView.isMine) {
			DoLocalMovement ();
		} else {
			transform.position = Vector3.Lerp (transform.position, realPosition, smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, smoothing);
			//transform.position =  realPosition;
			//transform.rotation = realRotation;
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
		/*
		if (direction.magnitude > 1) {
			direction = direction.normalized;
		}
		*/

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

	}
}
