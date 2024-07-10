using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float m_speed = 10f;
    protected SpriteRenderer m_sr;
    protected Rigidbody2D m_rb;
    protected Collider2D m_cldr;
    protected Transform poolerParent;
    protected bool impacted = false;
    protected float direction = 1;
    [SerializeField] protected float forcePower;
    public float ForcePower => direction * forcePower;
    public void Awake()
    {
        m_sr = GetComponent<SpriteRenderer>();
        m_rb = GetComponent<Rigidbody2D>();
        m_cldr = GetComponent<Collider2D>();
        poolerParent = transform.parent;
    }

    public void Fire(Vector2 spawnPos, Vector2 direction)
    {
        impacted = false;
        transform.SetParent(poolerParent, true);
        transform.position = spawnPos;
        m_rb.velocity = direction * m_speed;
        this.direction = direction.x;
        //enabled = true;

        m_sr.flipX = direction == Vector2.left;
        m_cldr.enabled = true;
        m_cldr.offset = new Vector2(Mathf.Sign(direction.x) * Mathf.Abs(m_cldr.offset.x), m_cldr.offset.y);
        transform.eulerAngles = new Vector3(0, 0, 0);

        // Old and farting
        //if (direction == new Vector2(0.5f, 0.5f))
        //{
        //    transform.eulerAngles = new Vector3(0f, 0f, 45);
        //    return;
        //}
        //if (direction == new Vector2(0.5f, -0.5f))
        //{
        //    transform.eulerAngles = new Vector3(0f, 0f, -45);
        //    return;
        //}
        //if (direction == new Vector2(-0.5f, 0.5f))
        //{
        //    transform.eulerAngles = new Vector3(0f, 0f, 135);
        //    return;
        //}
        //if (direction == new Vector2(-0.5f, -0.5f))
        //{
        //    transform.eulerAngles = new Vector3(0f, 0f, -135);
        //    return;
        //}
        //transform.eulerAngles = new Vector3(0, 0, direction == Vector2.up ? 90 : direction == Vector2.down ? -90 : 0);
    }

    protected virtual void ImpactHandler(GameObject cogo)
    {
        if (!impacted && cogo.gameObject.layer == LayerMask.NameToLayer("Platforms") && cogo.tag != "head")
        {
            m_rb.velocity = Vector3.zero;
            m_cldr.enabled = false;
            transform.SetParent(cogo.transform, true);
            impacted = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("I was entered trigger!");
        if (collider.isTrigger) return;
        ImpactHandler(collider.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("I was entered Collision!");
        //ImpactHandler(collision.gameObject);
    }
}
