using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipIndicator : MonoBehaviour
{
    // Gamemanager could reference this 
    // Just let its values be contrable
    [SerializeField] private Image m_tinkerHalf;
    [SerializeField] private Image m_asheHalf;
    [SerializeField] private bool alwaysVisible;

    // Skip Indicator Components
    private Animator m_animator;
    private CanvasGroup m_canvasGroup;
    private bool isFading;
    public float TinkerHalf
    {
        get { return m_tinkerHalf.fillAmount; }
        set { m_tinkerHalf.fillAmount = value; }
    }
    public float AsheHalf 
    {
        get { return m_asheHalf.fillAmount; }
        set { m_asheHalf.fillAmount = value; }
    }

    private void Start()
    {
        if (!alwaysVisible) 
        { 
            m_animator = GetComponent<Animator>(); 
        }
        m_canvasGroup = GetComponent<CanvasGroup>();
        isFading = true;
        m_tinkerHalf.fillAmount = 0f;
        m_asheHalf.fillAmount = 0f;

        if (alwaysVisible)
        {
            m_canvasGroup.alpha = 1;
        }
    }

    private void Update()
    {
        if (alwaysVisible)
        {
            return;
        }
        if (m_tinkerHalf.fillAmount >= 0.495f && m_asheHalf.fillAmount >= 0.495f)
        {
            m_tinkerHalf.fillAmount = 0f;
            m_asheHalf.fillAmount = 0f;
            m_animator.Play("Fade");
            isFading = true;
        }
        else if (m_tinkerHalf.fillAmount > 0f || m_asheHalf.fillAmount > 0f)
        {
            m_animator.Play("Visible");
            isFading = false;
        }
        else if (!isFading)
        {
            m_animator.Play("DelayFade");
            isFading = true;
        }

        
    }
}
