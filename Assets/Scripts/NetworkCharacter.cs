using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {
	
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	
	public float smoothing = 0.01f;
	private Animator anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			// Do nothing, CharacterMotor Script does everything
		} else {
			transform.position = Vector3.Lerp(transform.position, realPosition, smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, smoothing);
		}
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			// This is local player, we need to send our position and rotation
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(anim.GetFloat("Speed"));
			stream.SendNext(anim.GetBool("Jump"));
		} else {
			// This is remote player, we need to receive position and rotation and update our version of that player
			realPosition = (Vector3) stream.ReceiveNext ();
			realRotation = (Quaternion) stream.ReceiveNext ();
			anim.SetFloat("Speed", (float) stream.ReceiveNext());
			anim.SetBool("Jump", (bool) stream.ReceiveNext());
		}
	}
}
