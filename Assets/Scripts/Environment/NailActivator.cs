using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class NailActivator : Invoker
{
    [Header("NailActivator Specific")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private bool destroyProjectileWhenShot;


    private SpriteRenderer m_spriteRenderer;
    private AudioSource m_audioSource;

    #region Technical
    private bool isActive = false;
    //private int count = 0;
    #endregion

    private void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = inactiveSprite;
        m_audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //count++;
        if (collision.GetComponent<Projectile>() != null || collision.GetComponent<Gauntlet>() != null)
        {
            m_audioSource.Play();
            if (!isActive)
            {
                isActive = true;
                Activate();
                m_spriteRenderer.sprite = activeSprite;
                // Play Aniation of Squish State
            } 
            if (destroyProjectileWhenShot)
            {
                collision.gameObject.SetActive(false);
            }
        }  
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Will Not Deactivate for the time being
        //count--;
        //if (count == 0 && collision.GetComponent<Projectile>() != null) 
        //{
        //    isActive = false;
        //    Deactivate();
        //    m_spriteRenderer.sprite = inactive_sprite;
        //}
    }
}
