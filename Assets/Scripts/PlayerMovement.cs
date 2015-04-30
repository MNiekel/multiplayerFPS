using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Only enabled for local player!

	float speed = 10f;
	float jumpSpeed = 5f;
	Vector3 direction = Vector3.zero;

	private CharacterController cc;
	private Animator anim;
	private float verticalVelocity = 0f;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		direction = transform.rotation * new Vector3 (Input.GetAxis ("Horizontal"),
		                         0,
		                         Input.GetAxis ("Vertical"));

		if (direction.magnitude > 1) {
			direction = direction.normalized;
		}

		anim.SetFloat ("Speed", direction.magnitude);

		if (cc.isGrounded) {
			anim.SetBool ("Jump", false);
			if (Input.GetButton ("Jump")) {
				verticalVelocity = jumpSpeed;
				anim.SetBool ("Jump", true);
			} else {
				verticalVelocity = 0;
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			PhotonNetwork.DestroyPlayerObjects (PhotonNetwork.player);
			PhotonNetwork.LeaveRoom ();
		}
	}

	// Called once per physics loop
	// Do ALL movement and physics stuff here
	void FixedUpdate () {
		Vector3 moveDistance = direction * speed * Time.deltaTime;

		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		moveDistance.y = verticalVelocity * Time.deltaTime;
		cc.Move (moveDistance);
	}
}
