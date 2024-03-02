using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FireVent : Invokee
{
    [Header("Fire Vent Specific")]
    [SerializeField]
    private Collider2D m_flameCollider;
    [SerializeField, Tooltip("Auto turns flames on an off")] 
    private bool m_autoFlame = true;
    [SerializeField] private bool flameRise = false;
    [SerializeField, Tooltip("Used when Auto Flame active"), Range(0.5f, 10f)] 
    private float activeDuration = 3f;
    [SerializeField, Tooltip("Used when Auto Flame active"), Range(0.5f, 10f)] 
    private float inactiveDuration = 3f;
    [SerializeField] private float yFlameMax;
    [SerializeField] private float yFlamePrep = -4.38f;
    [SerializeField] private float yFlameMin = -4.246f;
    [SerializeField] private float speed = 3f;
    
    private Fire m_associatedFire;
    #region Technical
    private float durationSwitch = 0f;
    private bool flamePrep = false;
    #endregion
    private new void Start()
    {
        base.Start();
        m_associatedFire = GetComponentInChildren<Fire>();
        durationSwitch = Time.time + delay; 
    }
    private void Update()
    {
        if (m_autoFlame && Time.time >= durationSwitch)
        {
            if (flameRise)
            {
                DropFlame();
            }
            else
            {
                RaiseFlame();
            }
        }

        m_flameCollider.enabled = flameRise;
    }
    private void FixedUpdate()
    {
        m_associatedFire.transform.localPosition = new Vector2(m_associatedFire.transform.localPosition.x, Mathf.Lerp(m_associatedFire.transform.localPosition.y, flameRise ? yFlameMax : (flamePrep ? yFlamePrep :  yFlameMin), Time.deltaTime * speed));
    }

    protected override void OnActivate()
    {
        flameRise = !flameRise;
        durationSwitch = Time.time + (flameRise ? activeDuration : inactiveDuration);
    }

    protected override void OnDeactivate()
    {
        flameRise = !flameRise;
        durationSwitch = Time.time + (flameRise ? activeDuration : inactiveDuration);
    }
    public void EnableVent()
    {
        m_autoFlame = true;
        RaiseFlame();
    }
    // Very Permeant Disabbling
    public void DisableVent()
    {
        m_autoFlame = false;
        flameRise = false;
    }
    public void RaiseFlame()
    {
        StartCoroutine(FlamePrep());
    }
    public void DropFlame()
    {
        flameRise = false;
        durationSwitch = Time.time + inactiveDuration;
    }

    private IEnumerator FlamePrep()
    {
        float tempSpeed = speed;
        speed = 5f;
        flameRise = false;
        flamePrep = true;
        yield return new WaitForSeconds(0.6f); 
        speed = tempSpeed;
        flamePrep = false;
        flameRise = true;
        durationSwitch = Time.time + activeDuration;
    }
}
