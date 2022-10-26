using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrog : MonoBehaviour
{
    [Header("Enemy Info")]
    public float jump;
    /*public Vector3 startPosition;
    public Vector3 endPosition;*/
    public int damage;
    

    public bool movingForward;
    public SpriteRenderer Frog;
    public Rigidbody2D rbFrog; 
    public float minJumpForce = 100;
    public float maxJumpForce = 150;
    Animator animator;

    public void Start()
    {

        /*startPosition = transform.position;*/
        movingForward = true;
        animator = gameObject.transform.Find("frog-idle-1").GetComponent<Animator>();
        StartCoroutine(JumpLogic());
    }

    public void Update()
    {


        
    }

    IEnumerator JumpLogic()
    {
        float force = Random.Range(minJumpForce, maxJumpForce);
        float minWaitTime = 3;
        float maxWaitTime = 6;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            
            Jump();
            
        }
    }

    void Jump()
    {
        int lado;
        lado = Random.Range(0,1) ;

        if (lado == 0)
        {
            rbFrog.AddForce(new Vector2(-1, 1) * jump, ForceMode2D.Impulse);
            Frog.flipX = false;
        }
        else 
        {
            Frog.flipX = true;
            rbFrog.AddForce(new Vector2(1, 1) * jump, ForceMode2D.Impulse); 
        }

        animator.Play("FrogJump");

    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            Destroy(gameObject);
        }
    }
}
