using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

	public float Speed;
	public float JumpStrength;
    public int bounceMag;
    private int buildIndex;
    
	public bool isGrounded = false;
    public bool isReversed;
	public bool isDead = false;
	public bool isWhite = true;
    public bool soundHooks;
    public bool isFinished = false;
    public bool diedFlag = true;

    Animator GameOver; // Animator used in GameOver

    Transform endPoint;
    Text CoinText;
    GameObject endConfetti; // The levels end confetti
    GameObject JB; // Sound effects, babyyy
    
    // Creates the players Rigidbody2D for easy access
	Rigidbody2D rb2d;

    AudioSource JukeBox;
    Catalog SoundCatalog;

	// Loads the animation call script
	private List<Animator> anim = new List<Animator>();
	private List<SpriteRenderer> spriteRend = new List<SpriteRenderer>();

    SaveScript Saver;
    HUDController HUD;

    // Use this for initialization
    void Start () {        
		GetComponentsInChildren<Animator> (true, anim); // Grabs all animators (enabled or disabled)
		GetComponentsInChildren<SpriteRenderer> (true, spriteRend);	//Grabs all Sprite Renderers (enabled or disabled)
    
        HUD = GameObject.Find("HUDCanvas").GetComponent<HUDController>();		
        GameOver = HUD.gameObject.transform.GetChild(2).gameObject.GetComponent<Animator>();

        endPoint = GameObject.Find("EndPoint").transform;
        CoinText = GameObject.Find("CoinText").GetComponent<Text>();
        endConfetti = GameObject.Find("FX_confetti");
        JB = GameObject.Find("Jukebox");

        buildIndex = SceneManager.GetActiveScene().buildIndex; // Grabs the current level
        
        rb2d = GetComponent<Rigidbody2D>(); // Caches the players Rigidbody2D
	   	rb2d.velocity = new Vector2(Speed, 0); //Gives it the initial speed
	    JukeBox = JB.GetComponent<AudioSource>();
        SoundCatalog = JB.GetComponent<Catalog>();
    }

    IEnumerator LongJump()
    {
        float prevSpeed = Speed;

        Speed = Speed * 2;
        while (!isGrounded) {
            yield return null;
        }

        //Debug.Log("Jack Shit");

        Speed = prevSpeed;
    }

    // Update is called once per frame
    void Update () {
    	// If the player is dead, start the pop animation
		if (isDead){
			foreach (Animator anims in anim){
                rb2d.velocity = rb2d.velocity*(0);
                HUD.GameOver();
				anims.SetTrigger("isDead");
			}

            if (soundHooks){
                JukeBox.clip = SoundCatalog.death;
            }

            if(diedFlag)
            {
                diedFlag = false;
				SaveScript.TMD.died();

            }
            
		} else if (isFinished) {
			if (soundHooks){
                JukeBox.clip = SoundCatalog.endLevel;
				JukeBox.Play();
			}
			rb2d.velocity = rb2d.velocity*(0); // Stops player movement
			//Debug.Log("You've made it, you beautiful bastard");
			endConfetti.SetActive(true);

			foreach (Animator anims in anim){
				anims.SetBool("isFinished", true);
			}

		} else {

            // if the player reaches the end point of the level
            if (transform.position.x >= endPoint.position.x){
               isFinished = true;
            }


			// Keeps the velocity constant on each frame
			rb2d.velocity = new Vector2(Speed, rb2d.velocity.y);

			// This is the kill-floor
			if (rb2d.position.y <= -10){
                isDead = true;
			}

			
			

            #if !UNITY_ANDROID && !UNITY_IPHONE && !UNITY_BLACKBERRY && !UNITY_WINRT
                // Jumping (if tapped on the left side of the screen)
            if (Input.GetButtonDown("Jump")){
                Jump();
            }
			if (Input.GetKeyDown(KeyCode.Q)){
				Toggle();

			}
            #endif

            // Code used to swap animations mid frame (if tapped on the right side of the screen)
		}

		// Debug level reset (if tapped on the bottom half of the screen while dead)
        if (Input.GetKeyDown(KeyCode.R))
        {
			isDead = true;
            //SaveScript.TMD.died();
            //Debug.Log("triggered");
            SceneManager.LoadScene(buildIndex);
        }


        // Debug death key
        if (Input.GetKeyDown(KeyCode.D))
        {
            isDead = true;
        }

   //      // Returns to the main menu (if tapped on the top half of the screen while dead)
   //      if (Input.GetKeyDown(KeyCode.Escape) || ((Input.touchCount == 1) && Input.touches[0].position.y > Screen.height/2 && (isDead || isFinished)))
   //      {
   //      	if (soundHooks){
   //              JukeBox.clip = SoundCatalog.pause;
			// 	JukeBox.Play();
			// }
   //          SceneManager.LoadScene("MainMenu");
   //      }


    }
    public void Jump(){
        if (isGrounded){
            if (soundHooks){
                JukeBox.clip = SoundCatalog.jump;
                JukeBox.Play();
            }
            rb2d.AddForce(Vector2.up * JumpStrength);
            isGrounded = false;
            foreach (Animator animator in anim){
                animator.SetBool ("isGrounded", isGrounded);
            }

            
        }
        // NEED TO ADD FINAL IMAGE
    }

    public void Toggle(){
        if (soundHooks){
            JukeBox.clip = SoundCatalog.toggle;
            JukeBox.Play();
        }

        isWhite = !isWhite;
         // Toggles the sprite renderers so the animations stay in sync
        foreach (SpriteRenderer sprites in spriteRend){
            sprites.enabled = !sprites.enabled;
        }


        // NEED TO ADD ALTERNATING FINAL IMAGES
    }

    void OnTriggerEnter2D(Collider2D col){
    	// If the player collides with a coin!
    	if (col.gameObject.tag == "Coin"){
			//Debug.Log("We touched :O");
			col.gameObject.SetActive(false);

            int currentCoin = System.Convert.ToInt32(CoinText.text) + 1;
            CoinText.text = System.Convert.ToString(currentCoin);
            Debug.Log(HUD.transform.gameObject.name);
            HUD.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Text>().text = System.Convert.ToString(currentCoin) + "/5";
            if (currentCoin > (int)SaveScript.TMD.coinProgress(buildIndex))
            {
                SaveScript.TMD.addCoin(buildIndex);
            }
    	}

    	// If the player runs into the wrong color
    	if (col.gameObject.tag == "WhitePlatform" && !isWhite){
    		//Debug.Log("Fuck, I'm black, but its white");
            isDead = true;
    	}

    	// If the player runs into the wrong color
    	if (col.gameObject.tag == "BlackPlatform" && isWhite){
    		//Debug.Log("Fuck, I'm white, but its black");
            isDead = true;
    	}
    }

	void OnCollisionEnter2D(Collision2D blockCollision){
        // If the player hits a white or black platform, it affects the isGrounded condition for the animator
        if ((blockCollision.gameObject.tag == "WhitePlatform" && isWhite) || (blockCollision.gameObject.tag == "BlackPlatform" && !isWhite)){
			// if (soundHooks){
			// 	JukeBox.Play();
			// }

			isGrounded = true;

			foreach (Animator anims in anim){
				anims.SetBool ("isGrounded", isGrounded);
			}            
		}

		// Kills the player on collision with spikes
		if (blockCollision.gameObject.tag == "Spike" || (blockCollision.gameObject.tag == "WhitePlatform" && !isWhite) || (blockCollision.gameObject.tag == "BlackPlatform" && isWhite)){
			//Debug.Log("Fuck, that hurt");
            isDead = true;
		}

		// Currently unused block prefab that doubles speed
        if (blockCollision.gameObject.tag == "Speed2x")
        {
            Speed = Speed * 2;
        }

        // Code to reverse player, works both directions from all directions.
        if (blockCollision.gameObject.tag == "Reverse")
        {
        	if (soundHooks){
                JukeBox.clip = SoundCatalog.reversed;
				JukeBox.Play();
			}

            //Debug.Log("Reverse");
            isReversed = !isReversed;
            Speed = -Speed;
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y);

        }

        // Code for trampoline collision, hits player upward. 
        if (blockCollision.gameObject.tag == "Trampoline")
        {
        	if (soundHooks){
                JukeBox.clip = SoundCatalog.trampoline;
				JukeBox.Play();
			}
            //Debug.Log("Jumped");
            rb2d.AddForce(Vector2.up * bounceMag);
            isGrounded = false;
            foreach (Animator animator in anim)
            {
                animator.SetBool("isGrounded", isGrounded);
            }
        }

        //Code for long jump
        if (blockCollision.gameObject.tag == "LongJump")
        {
        	if (soundHooks){
				JukeBox.clip = SoundCatalog.trampoline;
                JukeBox.Play();
			}
            //Debug.Log("Longed");

            rb2d.AddForce(Vector2.right * bounceMag);
            rb2d.AddForce(Vector2.up * bounceMag);
            isGrounded = false;
            foreach (Animator animator in anim)
            {
                animator.SetBool("isGrounded", isGrounded);
            }

            StartCoroutine(LongJump());
        }

    }
}
