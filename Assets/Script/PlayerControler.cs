using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerControler : MonoBehaviour
{
    [Header("Player Info")]
    public int speed = 5;
    int startingSpeed;
    public int jumpPower = 45;
    public int livesLeft = 3;
    public int lives = 3;
    public int extraJumps = 0;
    int jumpnumber;
    public int sneakSlowing = 3;
    private int boostSpeed = 4;
    public Vector3 Origen;
    bool sneaking = false;
    bool god;
    bool ambushing = false;


    private SpriteRenderer sprite;
    Animator animator;
    private Rigidbody2D rb;
    public GameObject RaycastGround;

    [Header("Level Data")]
    public int powerUpsLeft;
    public int collectedPowerUps;
    private int totalPowerUps;
    public int levelTime;
    public AudioSource audioHit, audioPick, audioLose;
    int waterDamage = 1;



    public TextMeshProUGUI txtTime, txtScore;
    public GameObject heart1,heart2,heart3;
    


    private void Start()
    {
        transform.position = Origen;
        rb = GetComponent<Rigidbody2D>();
        RaycastGround = GameObject.Find("RaycastGround");
        sprite = gameObject.transform.Find("Player_Idle").GetComponent<SpriteRenderer>();
        animator = gameObject.transform.Find("Player_Idle").GetComponent<Animator>();
        livesLeft = lives;
        powerUpsLeft = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        totalPowerUps = powerUpsLeft;
        startingSpeed = speed;

    }

    private void FixedUpdate()
    {
        int timeLeft = levelTime - (int)(Time.time);
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

    private void Update()
    {
        // movement from -1 to 1
        float inputX = Input.GetAxis("Horizontal");

        //apply phisic velocity
        rb.velocity = new Vector2(inputX * speed /** Time.deltaTime*/, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && jumpnumber >= 0)
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); //fuerza como un impulso
            RaycastGround.SetActive(false);
            Invoke("spawnObject", 0.1f);
            jumpnumber--;  
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            speed = speed - sneakSlowing;
            sneaking = true;
        }

        else if (Input.GetKeyUp(KeyCode.S))
        {
            speed = speed + sneakSlowing + boostSpeed;
            sneaking = false;
            ambushing = true;
            Invoke("ambushOff", 1f);
        }

        if (rb.velocity.x >= 0f) sprite.flipX = false;

        else if (rb.velocity.x < 0f) sprite.flipX = true;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        isGrounded();
        PlayerAnimate();
        AmbushMechanic();
    }

    void ambushOff()
    {
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

    void spawnObject() {
        jumpnumber--;
        RaycastGround.SetActive(true);}
    private bool isGrounded()
    {
        /*RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,-1.6f,0), Vector2.down, 0.2f);*/
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
        /* return hit.collider != null;*/
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
            if (livesLeft >= 3) {
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true); }
            else if (livesLeft == 2)
            {
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(false);
            } else if (livesLeft == 1)
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
            if (livesLeft <= 0) {
                Invoke("loseLevel", 3f);
                audioLose.Play();
                /*GameManager.instance.Win = false;*/
            }
        }
    }
    void invulnerability(){
        god = false;
        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            transform.position = Origen;
            TakeDamage(waterDamage);
        }

        if(collision.gameObject.CompareTag("PowerUp"))
        {
            audioPick.Play();
            collectedPowerUps++;
            Destroy(collision.gameObject);
            Invoke("infoPowerUp", 0.1f); 
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (ambushing)
            {
                Fight();
                /*gameObject.SetActive(false);*/
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (ambushing)
            {
                Fight();
                /*gameObject.SetActive(false);*/
            }
        }
    }
    void Fight()
    {
        
        animator.Play("PlayerFight");
        speed = 0;
        Invoke("speedReset", 1f);
    }
    void speedReset()
    {
        speed = 5;/*startingSpeed;*/
    }

    private void PlayerAnimate()
    {
        if (!isGrounded()) 
            animator.Play("PlayerJump");
        else if (isGrounded() && sneaking)
            animator.Play("PlayerSneak");
        else if (isGrounded() && Input.GetAxis("Horizontal") != 0)
            animator.Play("PlayerRunning");
        else if (isGrounded() && Input.GetAxis("Horizontal") == 0)
            animator.Play("PlayerIdle");
            



        /*((rb.velocity.x >= 0.1f) || (rb.velocity.x < 0.1f) && (rb.velocity.y == 0f)) animator.Play("PlayerRunning");*/
    }

    private void infoPowerUp()
    {
        powerUpsLeft = GameObject.FindGameObjectsWithTag("PowerUp").Length;
        Debug.Log("PowerUps" + GameObject.FindGameObjectsWithTag("PowerUp").Length);

        if (powerUpsLeft == 0)
        {
            Invoke("winLevel", 3f);
            god = true;
        }
    }

    

    private void winLevel(/*bool win*/)
    {
        SceneManager.LoadScene("End");
        /*GameManager.instance.Win = win;
        GameManager.instance.Score = (livesLeft * 100) + (levelTime * 10);*/

    }

    private void loseLevel()
    {
        SceneManager.LoadScene("Level 1");
    }


    

}
