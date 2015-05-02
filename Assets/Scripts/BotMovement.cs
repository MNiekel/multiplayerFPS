using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	// Use this for initialization
	private NetworkCharacter networkCharacter;
	
	void Start () {
		networkCharacter = GetComponent <NetworkCharacter> ();
	}
	
	void Update () {
		networkCharacter.isJumping = true;
	}
}
