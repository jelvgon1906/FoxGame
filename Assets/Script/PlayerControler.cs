using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Burst.CompilerServices;

public class PlayerControler : MonoBehaviour
{
    [Header("Player Info")]
    int speed = 5;
    public int jumpPower = 45;
    public int livesLeft = 3;
    public int lives = 3;
    public int extraJumps = 0;
    public int jumpnumber = 1;
    public int sneakSlowing = 3;
    private int boostSpeed = 4;
    public Vector3 Origen;
    bool sneaking = false;
    bool god;
    bool ambushing = false;
    bool ambushCooldown = true;
    bool playerMovement = true;
    /*private bool isGrounded;*/
    float inputX, inputY;
    CapsuleCollider2D m_Collider;
   

    //Ladder
    public float distance;
    public LayerMask WhatIsLadder;
    bool isClimbing;
    bool isClimbingAnim;


    private SpriteRenderer sprite;
    Animator animator;
    private Rigidbody2D rb;
    public GameObject RaycastGround;
    GameObject GroundDetector;
    GameManager gameManager;

    [Header("Level Data")]
    public int powerUpsLeft;
    public int collectedPowerUps;
    private int totalPowerUps;
    public int levelTime;
    int timeLeft;
    public AudioSource audioHit, audioPick, audioLose;
    int waterDamage = 1;


    public TextMeshProUGUI txtTime, txtScore;
    public GameObject heart1,heart2,heart3;
    


    private void Start()
    {
        Origen = transform.position;
        rb = GetComponent<Rigidbody2D>();
        RaycastGround = GameObject.Find("RaycastGround");
        /*GroundDetector = GameObject.Find("GroundDetector");*/
        sprite = gameObject.transform.Find("Player_Idle").GetComponent<SpriteRenderer>();
        animator = gameObject.transform.Find("Player_Idle").GetComponent<Animator>();
        livesLeft = lives;
        powerUpsLeft = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        totalPowerUps = powerUpsLeft;
        gameManager = gameObject.GetComponent<GameManager>();
        m_Collider = GetComponent<CapsuleCollider2D>();
        
    }

    private void FixedUpdate()
    {
        timeLeft = levelTime - (int)(Time.time);
        int minutes = timeLeft / 60;
        int seconds = timeLeft % 60;

        /*Debug.Log(minutes.ToString("00") + ":" + seconds.ToString("00"));*/
        if (levelTime - (int)(Time.time) <= 0)
        {
            TimeOver();
        }

        
        //Time clock
        txtScore.text = collectedPowerUps + "/" + totalPowerUps ; 
        txtTime.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
        TimeOver();
        howMuchTime();

    }

    void howMuchTime()
    {
        timeLeft = levelTime;
        GameManager.instance.levelTimeLeft = timeLeft;
    }

    private void Update()
    {
        // movement from -1 to 1
        inputX = Input.GetAxis("Horizontal");

        //apply phisic velocity
        if (playerMovement)
        {
            rb.velocity = new Vector2(inputX * speed /** Time.deltaTime*/, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpnumber > 0 && playerMovement && !isClimbing && !sneaking)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); //fuerza como un impulso
            RaycastGround.SetActive(false);
            Invoke("spawnRay", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            speed = speed - sneakSlowing;
            sneaking = true;
            m_Collider.size = new Vector2(0.89f,1f);
            m_Collider.offset = new Vector2(0f, -0.48f);
        }

        else if (Input.GetKeyUp(KeyCode.S))
        {
            speed = speed + sneakSlowing;
            m_Collider.size = new Vector2(0.89f,1.4f);
            m_Collider.offset = new Vector2(0f,-0.3f);
            if (ambushCooldown && isGrounded() && !isClimbing)
            {
                sprite.color = Color.yellow;
                speed = speed + boostSpeed;
                ambushCooldown = false;
                ambushing = true;
                Invoke("ambushOff", 1f);
            }
            sneaking = false;
        }

        if (rb.velocity.x > 0f) sprite.flipX = false;

        else if (rb.velocity.x < 0f) sprite.flipX = true;

        /*if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();*/

        isGrounded();
        PlayerAnimate();
        AmbushMechanic();
        Ladder();
    }

    
    void ambushOff()
    {
        ambushCooldown = true;
        speed = speed - boostSpeed;
        ambushing = false;
        god = false;
        sprite.color = Color.white;
    }

    private void AmbushMechanic()
    {
        if (ambushing)
        {
            god = true;
            animator.speed = 1.5F;
        }
        else
        {
            animator.speed = 1F;
        }
    }
    private bool isGrounded()
    {

        RaycastHit2D hit = Physics2D.Raycast(RaycastGround.transform.position, Vector2.down, 0.05f);


        if (hit.collider != null)
        {
            jumpnumber = 1 + extraJumps;
            return true;
        }
        else
        {
            return false;
        }

    }
    void Ladder()
    {
        //Ladder

        RaycastHit2D hitInfo = Physics2D.Raycast(RaycastGround.transform.position, Vector2.up, distance, WhatIsLadder);
        if (hitInfo.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                isClimbingAnim = true;
            }
            else if ((Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.Space))))) isClimbingAnim = false;
            if (Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.W)))
            {
                isClimbing = true;
            }
            else if ((Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.Space)))))
                {
                isClimbingAnim = false;
                isClimbing = false;
                rb.velocity = new Vector2(inputX * speed /** Time.deltaTime*/, rb.velocity.y);
                /*RaycastGround.SetActive(false);
                Invoke("spawnRay", 0.1f);*/
            }
            
        }

        if (isClimbing == true && hitInfo.collider != null)
        {
            inputY = Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(0, inputY * 3.5f);/*
            rb.AddForce(Vector2.up * speed, ForceMode2D.Force);*/
            rb.gravityScale = 0;
        }
        else rb.gravityScale = 1;

    }

    void spawnRay()
    {
        jumpnumber = 0 + extraJumps;
        RaycastGround.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        if (god == false)
        {
            livesLeft -= damage;
            /*Debug.Log("PlayerControler Lives:" + lives);*/
            sprite.color = new Color(0, 1, 1, 0.5f);
            audioHit.Play();
            god = true;
            Invoke("invulnerability", 1f);
            checkLives();
        }
    }

    void checkLives()
    {
        if (livesLeft >= 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
        }
        else if (livesLeft == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
        }
        else if (livesLeft == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        else
        {
            heart1.SetActive(false);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        if (livesLeft <= 0)
        {
            Invoke("loseLevel", 3f);
            audioLose.Play();
            /*GameManager.instance.Win = false;*/
        }
    }

    void TimeOver()
    {
        if(timeLeft <= 0)
        {
            audioLose.Play();
            SceneManager.LoadScene("Menu");
        }
        
    }
    void invulnerability(){
        god = false;
        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SALTO con groundDetector
        /*if (collision != null)
        {
            jumpnumber = 1 + extraJumps ;
            isGrounded = true;
        }else
        {
            isGrounded = false;
        }*/

        //DAÑO AGUA
        if (collision.gameObject.CompareTag("Water"))
        {
            transform.position = Origen;
            TakeDamage(waterDamage);
        }
        //DETECTAR POWERUP
        if(collision.gameObject.CompareTag("PowerUp"))
        {
            if (livesLeft < 3)
            {
                livesLeft++;
                checkLives();
            }
            else GameManager.instance.scorePlus();
            audioPick.Play();
            collectedPowerUps++;
            Destroy(collision.gameObject);
            Invoke("infoPowerUp", 0.1f); 
        }
        //DETECTAR ENEMIGO
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (ambushing)
            {
                Fight();
                collision.gameObject.SetActive(false);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //DETECTAR ENEMIGO
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (ambushing)
            {
                Fight();
                collision.gameObject.SetActive(false);
            }
        }
    }

    //EMBOSCADAS
    void Fight()
    {
        sprite.color = Color.white;
        playerMovement = false;
        rb.velocity = Vector3.zero;
        /*rb.useAutoMass = false;*/
        animator.Play("PlayerFight");
        audioHit.Play();
        Invoke("fightReset", 0.8f);
    }
    void fightReset()
    {
        /*rb.useAutoMass = true;*/
        if (!playerMovement) 
        {
            livesLeft--;
            checkLives();
            playerMovement = true; 
        }
    }

    //ANIMACIONES

    private void PlayerAnimate()
    {
        if (!isGrounded() && playerMovement)
        {
            animator.Play("PlayerJump");
        } 
        else if (isGrounded() && sneaking && playerMovement)
            animator.Play("PlayerSneak");
        else if (isGrounded() && Input.GetAxis("Horizontal") != 0 && playerMovement)
            animator.Play("PlayerRunning");
        else if (isGrounded() && Input.GetAxis("Horizontal") == 0 && playerMovement && !isClimbingAnim)
            animator.Play("PlayerIdle");
        else if (isClimbingAnim)
        {
            animator.Play("PlayerClimb");
        }




        /*((rb.velocity.x >= 0.1f) || (rb.velocity.x < 0.1f) && (rb.velocity.y == 0f)) animator.Play("PlayerRunning");*/
    }

    //CANTIDAD DE POWER UPS Y LOS RESTANTES

    private void infoPowerUp()
    {
        powerUpsLeft = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        /*Debug.Log("PowerUps" + GameObject.FindGameObjectsWithTag("PowerUp").Length);*/

        if (powerUpsLeft == 0)
        {
            Invoke("nextLevel", 3f);
            god = true;
            sprite.color = Color.cyan;

        }
    }

    //VICTORIA
    private void nextLevel()
    {
        audioPick.Play();
        GameManager.instance.SendMessage("NextLevel");

    }
    //DERRORTA
    private void loseLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


    

}
