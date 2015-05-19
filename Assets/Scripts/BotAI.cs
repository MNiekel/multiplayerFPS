using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotAI : MonoBehaviour {

	public float movementSpeed = 0.5f;
	public float shootingAccuracy = 0.5f;
	public float aggroRange = 30f;

	private int ID;

	public int botID {
		get { return ID; }
		set { ID = value; }
	}

	[System.NonSerialized]
	public string[] names = {"Bad Ass Bot", "Big Mama Bot", "Killa Bebe Bot", "ShootEm Bot", "Shoot To Kill Bot"};
}
