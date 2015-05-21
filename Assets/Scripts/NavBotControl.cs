using UnityEngine;
using System.Collections;
using System.Linq;

public class NavBotControl : MonoBehaviour {

	private BotAI botAI;
	private NavMeshAgent agent;
	private GameObject player;
	private Animator animator;
	private WeaponData weaponData;
	private FXManager fxManager;

	private float cooldown = 0f;

	void Start () {
		botAI = GetComponent <BotAI> ();
		weaponData = GetComponent <WeaponData> ();
		agent = GetComponent <NavMeshAgent> ();
		animator = GetComponent<Animator> ();
		fxManager = GameObject.FindObjectOfType <FXManager> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	void Update () {
		DoMovement ();
		cooldown -= Time.deltaTime;
	}
	
	private void DoMovement () {

		if (!player) {
			Debug.Log ("Looking for player");
			player = GameObject.FindGameObjectWithTag ("Player");
			return;
		}
		Vector3 target = player.transform.position;
		agent.SetDestination (target);
		if (agent.remainingDistance <= agent.stoppingDistance) {
			animator.SetFloat("Speed", 0f);
		} else {
			animator.SetFloat("Speed", 1f);
		}

		if (Vector3.Distance (transform.position, target) <= botAI.aggroRange) {
			agent.updateRotation = false;
			animator.SetBool("Shooting", true);
			transform.rotation = Quaternion.LookRotation (Shoot (target));

		} else {
			agent.updateRotation = true;
			animator.SetBool("Shooting", false);

		}
	}

	private Vector3 Shoot (Vector3 target) {
		Vector3 origin = transform.Find("Shooting Point").position;
		Vector3 shootingDirection = target - origin;
		shootingDirection.y = 0;
		shootingDirection.Normalize();

		if (cooldown > 0) {
			return shootingDirection;
		} else {
			cooldown = weaponData.fireRate;
		}

		Ray ray = new Ray (origin, shootingDirection);
		RaycastHit [] hits = Physics.RaycastAll (ray, weaponData.range).OrderBy(h=>h.distance).ToArray();

		if (hits.Length == 0) {
			ShowEffects(origin, origin + weaponData.range * shootingDirection);
		}

		foreach (RaycastHit hit in hits) {
			if (hit.collider.transform == transform) {
				continue;
			}

			ShowEffects(origin, hit.point);
			Hit (hit);
			break;
		}

		return shootingDirection;
	}

	private void Hit (RaycastHit hit) {
		string killer = this.gameObject.GetPhotonView().name;

		Health healthOfObject = hit.collider.GetComponent<Health> () as Health;
		if (healthOfObject != null) {
			healthOfObject.GetComponent <PhotonView> ().RPC ("TakeDamage", PhotonTargets.MasterClient, weaponData.damage, killer);
		}
	}

	private void ShowEffects(Vector3 start, Vector3 end) {
		fxManager.GetComponent <PhotonView> ().RPC ("ShootingFX", PhotonTargets.All,
		                                            start, end);
	}
}
