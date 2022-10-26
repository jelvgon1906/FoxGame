using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOpossum : MonoBehaviour
{
    [Header("Enemy Info")]
    public float speed;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public int damage;

    public bool movingForward;
    public SpriteRenderer spriteOpossum;

    public void Start()
    {

        startPosition = transform.position;
        movingForward = true;
    }

    public void Update()
    {

        EnemyMove();
    }

    void EnemyMove()
    {

        Vector3 targetPosition = (movingForward) ? endPosition : startPosition;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);


        if (transform.position == targetPosition)
        {

            movingForward = !movingForward;
        }

        if (movingForward == true) spriteOpossum.flipX = false; else { spriteOpossum.flipX = true; }
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage);
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            Destroy(gameObject);
        }
    }
}
