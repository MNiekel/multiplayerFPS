using UnityEngine;
using System.Collections;
using System.Linq;

public class BotShooting : MonoBehaviour {

	private NetworkCharacter networkCharacter;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
	}
	
	void Update () {
		networkCharacter.isShooting = DoTargeting ();
		//networkCharacter.isShooting = EvaluateShooting ();
	}

	private bool EvaluateShooting () {

		Ray ray = new Ray (transform.position, transform.forward);
		
		RaycastHit [] hits = Physics.RaycastAll (ray);
		

		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].collider.tag == "Player") {
				return true;
			}
		}

		return false;
	}

	private bool DoTargeting() {
		TeamMember[] teamMembers;
		teamMembers = GameObject.FindObjectsOfType <TeamMember> ();

		Transform target = null;

		foreach (TeamMember tm in teamMembers) {
			if (tm.transform.tag != "Player") {
				continue;
			}

			float distance = Vector3.Distance (transform.position, tm.transform.position);

			if (target == null || distance < Vector3.Distance (transform.position, target.position)) {
				target = tm.transform;
			}
		}

		if (target == null || Vector3.Distance (transform.position, target.position) > GetComponent<BotAI> ().aggroRange) {
			return false;
		}

		Vector3 shootingDirection = target.position - transform.position;

		Ray ray = new Ray (transform.position, shootingDirection);
		RaycastHit [] hits = Physics.RaycastAll (ray, Vector3.Distance (transform.position, target.position)).OrderBy(h=>h.distance).ToArray();

		foreach (RaycastHit hit in hits) {
			if (hit.collider.transform == transform) {
				continue;
			}
			if (hit.collider.tag != "Player") {
				break;
			} else {
				transform.rotation = Quaternion.LookRotation (shootingDirection);
				//transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation (shootingDirection), 0.1f);
				return true;
			}
		}
		return false;
	}
}

