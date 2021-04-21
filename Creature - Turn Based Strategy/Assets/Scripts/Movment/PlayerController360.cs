using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerGravity))]
public class PlayerController360 : MonoBehaviour
{
    [SerializeField] Sprite playerSprite;
    [SerializeField] string playerName;

    public string PlayerName { get => playerName; }
    public Sprite PlayerSprite { get => playerSprite; }

    // public vars
    public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 200;
	public LayerMask groundedMask;

	private Animator animator;
	
	// System vars
	bool grounded;
    bool isJumping;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody rb;
	
    private List<string> accessiblePlanets;

    public GameObject GrassPlanet;
    public GameObject WaterPlanet;
    public GameObject DesertPlanet;
    public GameObject FirePlanet;

    public GameObject TeleportScreen;
    public GameObject pressZToTeleport;

    public static PlayerController360 instance;
    public static PlanetGravity planetGravity;

    public event Action onEncounter;

    public bool inGrass;
    public bool inDialogue;
    public bool duringDialogue;

    public GameObject Battle;
    private CharacterController characterController;

    void Start()
    {
        //Find attached character controllers on player
        characterController = GetComponent<CharacterController>();
    }


    void Awake() 
	{
        instance = this;

        accessiblePlanets = new List<string>();
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cameraTransform = Camera.main.transform;
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator>();
		planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();
        accessiblePlanets.Add(planetGravity.name);
        TeleportScreen.SetActive(false);
	}
    
    public void Update()
    {
		if (Input.GetKeyDown(KeyCode.Z) && TeleporterActivator.canTeleport)
        {
            // Open the Teleport Menu
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pressZToTeleport.SetActive(false);
            TeleportScreen.SetActive(true);

            //StartCoroutine(Teleport());
        }


		// Look rotation:
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

		//Controls Movement
		Move();

		// Jump
		if (Input.GetKeyDown(KeyCode.Space)) 
        {
			if (grounded) 
            {
                isJumping = true;
				rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(SetJumpingFalse());
			}
		}
		
		// Grounded check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		
		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask)) 
        {
			grounded = true;
        }
		else 
        {
			grounded = false;
        }

		
	}

    public IEnumerator SetJumpingFalse()
    {
        yield return new WaitForSeconds(1.5f);
        isJumping = false;
    }

    public IEnumerator Teleport()
    {
        GrassPlanet.gameObject.tag = "Untagged";
        WaterPlanet.gameObject.tag = "Planet";
        DesertPlanet.gameObject.tag = "Untagged";
        FirePlanet.gameObject.tag = "Untagged";

        yield return new WaitForSeconds(0.5f);

        PlayerGravity.UpdatePlanetGravity();
        planetGravity = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetGravity>();

        rb.velocity = rb.angularVelocity = Vector3.zero;
        transform.position = Teleporter.WaterPlayerSpawnPosition;
        transform.eulerAngles = Teleporter.WaterPlayerSpawnEulerAngles;
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

        float maxDifference = isJumping ? 60f : 50f;

		if (difference > maxDifference)
			rb.MovePosition(rb.position);
		else
			rb.MovePosition(futurePos);
		
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

    public static void AddAccessiblePlanet(string _planetName)
    {
        instance.accessiblePlanets.Add(_planetName);
    }

}
