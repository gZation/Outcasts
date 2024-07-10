using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Punchable : MonoBehaviour
{
    private Rigidbody2D m_rigidbody2D;
    private void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var gauntlet = collision.gameObject.GetComponent<Gauntlet>();
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (gauntlet != null)
        {
            m_rigidbody2D.AddForce(gauntlet.ForcePower * Vector2.right);
        }

        if (projectile != null)
        {
            m_rigidbody2D.AddForce(projectile.ForcePower * Vector2.right);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var gauntlet = collision.GetComponent<Gauntlet>();
        var projectile = collision.gameObject.GetComponent<Projectile>();
        if (gauntlet != null)
        {
            m_rigidbody2D.AddForce(gauntlet.ForcePower * Vector2.right);
        }

        if (projectile != null)
        {
            m_rigidbody2D.AddForce(projectile.ForcePower * Vector2.right);
        }
    }
}
