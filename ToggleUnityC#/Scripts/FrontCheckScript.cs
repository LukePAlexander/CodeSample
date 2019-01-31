using UnityEngine;
using System.Collections;

public class FrontCheckScript : MonoBehaviour {

	void OnCollisionStay2D(Collision2D col){
		transform.parent.gameObject.GetComponent<PlayerMovement>().isDead = true;
	}
}
