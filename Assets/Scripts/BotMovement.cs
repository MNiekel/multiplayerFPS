using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	// Use this for initialization
	private NetworkCharacter networkCharacter;

	private Waypoint[] waypoints;
	private Waypoint targetWaypoint;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();

		waypoints = GameObject.FindObjectsOfType<Waypoint> ();
		targetWaypoint = FindClosestWaypoint ();
	}
	
	void Update () {
		networkCharacter.direction = targetWaypoint.transform.position - transform.position;
		networkCharacter.direction.y = 0;
		networkCharacter.direction.Normalize ();

		transform.rotation = Quaternion.LookRotation (networkCharacter.direction);
	}

	private Waypoint FindClosestWaypoint () {
		float distance = 0f;
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
