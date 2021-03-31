using UnityEngine;
using System.Collections;


public class PlayerGravity : MonoBehaviour 
{	
    PlanetGravity planet;
	Rigidbody rb;

	private static PlayerGravity instance;
	
	void Awake () 
	{
		instance = this;

		planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
		rb = GetComponent<Rigidbody> ();

		// Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
		rb.useGravity = false;
		rb.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	void FixedUpdate () 
	{
		// Allow this body to be influenced by planet's gravity
		if (planet != null)
			planet.Attract(rb);
	}

	public static void UpdatePlanetGravity()
    {
		instance.planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
	}
}