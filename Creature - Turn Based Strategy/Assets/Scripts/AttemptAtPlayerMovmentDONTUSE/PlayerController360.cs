using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerGravity))]
public class PlayerController360 : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;

	private Animator animator;
	
	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody rb;
	PlanetGravity planetGravity;
	
	
	void Awake() 
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cameraTransform = Camera.main.transform;
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator>();
		planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
	}
    
    public void Update()
    {
		
		// Look rotation:
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

		//Controls Movement
		Move();

		// Jump
		if (Input.GetButtonDown("Jump")) {
			if (grounded) {
				rb.AddForce(transform.up * jumpForce);
			}
		}
		
		// Grounded check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) {
			grounded = true;
		}
		else {
			grounded = false;
		}
		
	}

	private void Move()
    {
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");

		Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
		
		//Animation
		if(inputX != 0 || inputY != 0)
        {
			animator.SetBool("IsRunning", true);
			animator.SetBool("IsIdle", false);
        }
		else
        {
			animator.SetBool("IsRunning", false);
			animator.SetBool("IsIdle", true);
		}
	}
	
	void FixedUpdate() 
	{
		// Apply movement to rigidbody
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		Vector3 futurePos = rb.position + localMove;
		float difference = Vector3.Distance(futurePos, planetGravity.getPosition());

		if (difference > 50)
			rb.MovePosition(rb.position);
		else
			rb.MovePosition(futurePos);

		//Debug.Log(difference);
		//rb.MovePosition(futurePos);
		
	}

    CharacterController characterController;

    [SerializeField] Sprite sprite;
    [SerializeField] string name;

    public event Action onEncounter;

    public bool inGrass;
    public bool inDialogue;
    public bool duringDialogue;

    public GameObject Battle;

    void Start()
    {
        //Find attached character controllers on player
        characterController = GetComponent<CharacterController>();
    }

    //See if the player is in long grass(where enemys hide)
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Grass" && !inGrass)
        {
            inGrass = true;
        }

        if (inGrass == true)
        {
            StartCoroutine(EnemyEncounter());
        }

        //Enemy encounter chance, 1 in 10 every .5 seconds
        //we will use a technique called observer design pattern so as to not double call this inside the gamecontroller
        IEnumerator EnemyEncounter()
        {
            while (inGrass)
            {
                yield return new WaitForSeconds(.5f);

                if (GameController.State == GameState.Freeroam)
                {
                    //had to specifyy the random because system and unity both have a random function
                    if (UnityEngine.Random.Range(1, 101) <= 10)
                    {
                        //Debug.Log("EnemyEncountered");
                        onEncounter();
                    }
                    //StartCoroutine(EnemyEncounter());
                }
            }
        }

        //Dialogue Collider enter
        if (collider.tag == "Dialogue")
        {
            inDialogue = true;
        }

        //on collision i am trying to run this. It should clear the dialogue
        if (GameController.State == GameState.Freeroam && collider.gameObject.GetComponent<DialogueNPC>())
        {
            //duringDialogue = true;
            collider.GetComponent<Interactable>()?.Interact();
        }

        //on collision i am trying to run this. It should clear the dialogue
        if (GameController.State == GameState.Freeroam && collider.gameObject.GetComponent<TrainerController>())
        {
            var trainer = collider.GetComponent<TrainerController>();
            if (trainer != null)
            {
                StartCoroutine(trainer.TriggerTrainerBattle());
            }
        }
    }


    //Player no longer in long grass or dialogue
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Grass" && inGrass)
        {
            inGrass = false;
        }


        //SAM - this stops all coroutines, see if it effects launching the fight sequences
        //if not in grass stop runnings into 
        if (inGrass == false)
        {
            StopAllCoroutines();
        }


        //Dialogue Collider exit
        if (collider.tag == "Dialogue")
        {
            inDialogue = false;
        }
    }

    public string Name
    { get => name; }

    public Sprite Sprite
    { get => sprite; }

}
