using UnityEngine;
using System.Collections;
using System.Linq;

public class BotControl : MonoBehaviour {

	private NetworkCharacter networkCharacter;
	private BotAI botAI;
	
	private Waypoint[] waypoints;
	private Waypoint targetWaypoint;
	private float distance = 0;

	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
		botAI = GetComponent <BotAI> ();
		
		networkCharacter.speed = botAI.movementSpeed;
		
		waypoints = GameObject.FindObjectsOfType<Waypoint> ();
		targetWaypoint = FindClosestWaypoint ();
	}
	
	void Update () {
		DoMovement ();
		networkCharacter.isShooting = DoTargeting ();

	}

	private void DoMovement () {
		if (Vector3.Distance (transform.position, targetWaypoint.transform.position) < 3f) {
			targetWaypoint = targetWaypoint.connectedWaypoints[ Random.Range (0, targetWaypoint.connectedWaypoints.Length)];
		}
		
		networkCharacter.direction = targetWaypoint.transform.position - transform.position;
		networkCharacter.direction.y = 0;
		networkCharacter.direction.Normalize ();
		
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation (networkCharacter.direction), 0.3f);
	}
	
	private Waypoint FindClosestWaypoint () {
		Waypoint closestWaypoint = null;
		foreach (Waypoint waypoint in waypoints) {
			if (closestWaypoint == null || Vector3.Distance (transform.position, waypoint.transform.position) < distance) {
				closestWaypoint = waypoint;
				distance = Vector3.Distance (transform.position, waypoint.transform.position);
			}
		}
		
		return closestWaypoint;
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
				Vector3 shootingDeviation = shootingDirection;
				shootingDeviation.z = shootingDeviation.z + Random.Range(-botAI.shootingAccuracy, botAI.shootingAccuracy);
				//transform.rotation = Quaternion.LookRotation (shootingDirection);

				transform.rotation = Quaternion.Lerp(Quaternion.LookRotation (shootingDeviation), Quaternion.LookRotation (shootingDirection), 0.5f);
				return true;
			}
		}
		return false;
	}
}
