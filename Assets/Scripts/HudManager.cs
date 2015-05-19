using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {

	private GameObject myPlayer;

	public Slider healthBar;
	public Image fillImage;
	public Image crossHair;

	void Update () {
		if (myPlayer) {
			healthBar.gameObject.SetActive (true);
			crossHair.gameObject.SetActive (true);
			Health health = myPlayer.GetComponent<Health> () as Health;
			healthBar.value = health.currentHitPoints / health.maxHitPoints * 100;
			fillImage.color = Color.Lerp (Color.red, Color.green, health.currentHitPoints / health.maxHitPoints);
		} else {
			healthBar.gameObject.SetActive (false);
			crossHair.gameObject.SetActive (false);
		}
	}
}
