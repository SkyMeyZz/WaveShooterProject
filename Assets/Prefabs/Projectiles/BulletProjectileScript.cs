using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectileScript : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float BulletMoveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * BulletMoveSpeed, ForceMode2D.Impulse);
    }
}