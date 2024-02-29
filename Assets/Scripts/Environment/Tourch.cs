using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tourch : MonoBehaviour
{
    [Header("Tourch Specs")]
    [SerializeField] private float speed_reaction = 0f;
    [SerializeField] private bool unlit;
    [SerializeField] private GameObject lightItSelf;
    private bool isUnlit = false;
    private Animator m_animator;
    private float duration_swap;
    private float duration_wind;
    private TourchDetection detector;
    public TourchDetection Detector
    {
        get { return detector; }
        set { detector = value; }
    }

    private void Start()
    {
       m_animator = GetComponent<Animator>();
       duration_swap = Random.Range(5f, 12f) + Time.time;
       duration_wind = Random.Range(35f, 50f) + Time.time;
    }

    private void Update()
    {
        if (!isUnlit && unlit)
        {
            m_animator.Play("Touch_unlit");
            lightItSelf.SetActive(false);
            isUnlit = true;
            return;
        }
        else if (isUnlit && !unlit)
        {
            m_animator.Play("Touch_idle_left");
            lightItSelf.SetActive(true);
            isUnlit = false;
        }
        if (Time.time >= duration_wind)
        {
            m_animator.ResetTrigger("Wind");
            m_animator.SetTrigger("Wind");
            duration_wind = Random.Range(35f, 50f) + Time.time;
            duration_swap = Random.Range(5f, 12f) + Time.time;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Tinker" || collision.gameObject.tag == "Ashe") && unlit)
        {
            unlit = false;
            detector.LitTourch();
        }
        if (collision.gameObject.tag == "Gauntlet")
        {
            bool punchLeft = collision.gameObject.transform.parent.GetComponent<Animator>().GetFloat("MoveX") < 0;
            m_animator.Play(punchLeft ? "Touch_wind_RL" : "Touch_wind_LR");
            m_animator.SetBool("left", !punchLeft);
            duration_wind = Random.Range(35f, 50f) + Time.time;
            duration_swap = Random.Range(5f, 12f) + Time.time;
            unlit = false;
        }
        else if ((collision.gameObject.tag == "Tinker" || collision.gameObject.tag == "Ashe") && collision.GetComponent<Rigidbody2D>().velocity.magnitude > speed_reaction)
        {
            bool movementLeft = collision.gameObject.transform.GetComponentInChildren<Animator>().GetFloat("MoveX") < 0;
            m_animator.Play(movementLeft ? "Touch_wind_RL" : "Touch_wind_LR");
            m_animator.SetBool("left", !movementLeft);
            duration_wind = Random.Range(35f, 50f) + Time.time;
            duration_swap = Random.Range(5f, 12f) + Time.time;
            unlit = false;
        }
        else if (collision.gameObject.tag == "projectile")
        {
            bool projectileLeft = collision.gameObject.transform.GetComponent<SpriteRenderer>().flipX;
            m_animator.Play(projectileLeft ? "Touch_wind_RL" : "Touch_wind_LR");
            m_animator.SetBool("left", !projectileLeft);
            duration_wind = Random.Range(35f, 50f) + Time.time;
            duration_swap = Random.Range(5f, 12f) + Time.time;
            unlit = false;
        }
    }
}
