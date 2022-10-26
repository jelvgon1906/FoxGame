using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThrought : MonoBehaviour
{
    private Collider2D _colider;
    public bool _playerOnPlataform;

    private void Start()
    {
        _colider = GetComponent<Collider2D>();
    }

    private void Update()
    {
     if (_playerOnPlataform && Input.GetKey(KeyCode.S) )
        {
            _colider.enabled = true;
            StartCoroutine(EnableCollider());
        }   
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        _colider.enabled = true;
    }
    private void SetPlayerOnPlatafotm(Collision2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerControler>();
        if (player != null)
        {
            _playerOnPlataform = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetPlayerOnPlatafotm(collision, value: true);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetPlayerOnPlatafotm(collision, value: true);

    }
}
