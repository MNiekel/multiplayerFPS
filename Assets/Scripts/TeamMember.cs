using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {

	private int ID = 0;

	[RPC]
	public void SetTeamID(int ID) {
		this.ID = ID;
		SkinnedMeshRenderer meshRenderer = this.transform.GetComponentInChildren <SkinnedMeshRenderer>();
		if (ID == 1) {
			meshRenderer.material.color = Color.yellow;
		} else {
			if (ID == 2) {
				meshRenderer.material.color = Color.red;
			}
		}
	}

	public int GetTeamID () {
		return ID;
	}

	public int teamID {
		get { return ID; }
	}
}
