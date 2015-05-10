using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	// Use this for initialization
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
		if (Vector3.Distance (transform.position, targetWaypoint.transform.position) < 1f) {
			targetWaypoint = targetWaypoint.connectedWaypoints[ Random.Range (0, targetWaypoint.connectedWaypoints.Length)];
		}

		networkCharacter.direction = targetWaypoint.transform.position - transform.position;
		networkCharacter.direction.y = 0;
		networkCharacter.direction.Normalize ();
		
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation (networkCharacter.direction), 0.1f);
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
}
