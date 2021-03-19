using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerGravity : MonoBehaviour {
	
	PlanetGravity planet;
	Rigidbody rigidbody;
	
	void Awake () {
		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
		rigidbody = GetComponent<Rigidbody> ();

		// Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	void FixedUpdate () {
		// Allow this body to be influenced by planet's gravity
		planet.Attract(rigidbody);
	}
}