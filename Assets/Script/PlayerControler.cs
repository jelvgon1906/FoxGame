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
    private bool isGrounded;


    private SpriteRenderer sprite;
    Animator animator;
    private Rigidbody2D rb;
    /*public GameObject RaycastGround;*/
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
        /*RaycastGround = GameObject.Find("RaycastGround");*/
        GroundDetector = GameObject.Find("GroundDetector");
        sprite = gameObject.transform.Find("Player_Idle").GetComponent<SpriteRenderer>();
        animator = gameObject.transform.Find("Player_Idle").GetComponent<Animator>();
        livesLeft = lives;
        powerUpsLeft = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        totalPowerUps = powerUpsLeft;
        gameManager = gameObject.GetComponent<GameManager>();

    }

    private void FixedUpdate()
    {
        timeLeft = levelTime - (int)(Time.time);
        int minutes = timeLeft / 60;
        int seconds = timeLeft % 60;

        /*Debug.Log(minutes.ToString("00") + ":" + seconds.ToString("00"));*/
        if (levelTime - (int)(Time.time) <= 0)
        {
            Invoke("loseLevel", 3f);
            /*GameManager.instance.Win = false;*/
            audioLose.Play();
            /*Debug.Log("loose times up");*/
        }



        txtScore.text = collectedPowerUps + "/" + totalPowerUps ; 
        txtTime.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
    }

    void howMuchTime()
    {
        timeLeft = levelTime;
        GameManager.instance.levelTimeLeft = timeLeft;
    }

    private void Update()
    {
        // movement from -1 to 1
        float inputX = Input.GetAxis("Horizontal");

        //apply phisic velocity
        if (playerMovement)
        {
            rb.velocity = new Vector2(inputX * speed /** Time.deltaTime*/, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpnumber > 0 && playerMovement)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); //fuerza como un impulso
            /*RaycastGround.SetActive(false);*/
            /*Invoke("spawnObject", 0.1f);*/
            jumpnumber--;  
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            speed = speed - sneakSlowing;
            sneaking = true;
        }

        else if (Input.GetKeyUp(KeyCode.S))
        {
            speed = speed + sneakSlowing;
            if (ambushCooldown)
            {
                speed = speed + boostSpeed;
                ambushCooldown = false;
                ambushing = true;
                Invoke("ambushOff", 1f);
            }
            sneaking = false;
        }

        if (rb.velocity.x >= 0f) sprite.flipX = false;

        else if (rb.velocity.x < 0f) sprite.flipX = true;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        /*isGrounded();*/
        PlayerAnimate();
        AmbushMechanic();
    }

    void ambushOff()
    {
        ambushCooldown = true;
        speed = speed - boostSpeed;
        ambushing = false;
        god = false;
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


    //RAYCAST FALLABA(checks demasiado rápidos) Y LO HE SUSTITUIDO POU UN TRIGGER MUY AJUSTADO

    /*void spawnRay() {
        jumpnumber--;
        RaycastGround.SetActive(true);}*/
    /*private bool isGrounded()
    {
        *//*RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,-1.6f,0), Vector2.down, 0.2f);*//*
        RaycastHit2D hit = Physics2D.Raycast(RaycastGround.transform.position, Vector2.down, 0.2f);


        if (hit.collider != null)
        {
            jumpnumber = extraJumps;
            return true;
        }
        else
        {
            return false;
        }
       
    }*/


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
    void invulnerability(){
        god = false;
        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //SALTO
        if (collision != null)
        {
            jumpnumber = 1 + extraJumps ;
            isGrounded = true;
        }else
        {
            isGrounded = false;
        }
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
            }else GameManager.instance.SendMessage("scorePlus");
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
        livesLeft--;
        checkLives();
        playerMovement = true;
    }

    //ANIMACIONES

    private void PlayerAnimate()
    {
        if (!isGrounded && playerMovement) 
            animator.Play("PlayerJump");
        else if (isGrounded && sneaking && playerMovement)
            animator.Play("PlayerSneak");
        else if (isGrounded && Input.GetAxis("Horizontal") != 0 && playerMovement)
            animator.Play("PlayerRunning");
        else if (isGrounded && Input.GetAxis("Horizontal") == 0 && playerMovement)
            animator.Play("PlayerIdle");
            



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
