﻿using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {

	private int ID = 0;
	
	[RPC]
	public void SetTeamID(int ID) {
		SkinnedMeshRenderer meshRenderer = this.transform.GetComponentInChildren <SkinnedMeshRenderer>();
		if (ID == 1) {
			meshRenderer.material.color = Color.blue;
		} else {
			if (ID == 2) {
				meshRenderer.material.color = Color.red;
			}
		}
	}

	public int teamID {
		get { return ID; }
	}
}
