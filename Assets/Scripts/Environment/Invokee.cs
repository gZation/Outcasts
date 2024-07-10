using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Invokee : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] protected GameObject targetColor;

    [Header("Inovkee Details")]
    [SerializeField, Tooltip("ID to be invokeed by")] 
    protected int id;
    [SerializeField, Tooltip("Once Invoked, wait delay seconds")] 
    protected float delay = 0f;
    [SerializeField, Tooltip("Cannot Be Deactivated")] 
    protected bool ActivateOnly;
    [SerializeField, Tooltip("Runs Activate Once")] 
    protected bool ActivateOnce;
    [SerializeField, Tooltip("Runs Activate during scene load")] 
    private bool activateOnStart = false;
    [SerializeField, Tooltip("Runs Deactivate during scene load")] 
    private bool deactivateOnStart = false;
    [SerializeField, Tooltip("Require Multiple Id invokes to activate")]
    private bool requireMultipleActive = false;
    [SerializeField, Tooltip("The number of calls required to activate")]
    protected int requireActiveCount = 1;

    private int activeCount = 0;

    protected void Awake()
    {
        EventManager.GetEventManager.Activated += ReactOnActivate;

        if (ActivateOnly) return;

        EventManager.GetEventManager.Deactivated += ReactOnDeactivate;
    }
    protected void Start()
    {
        if (activateOnStart) ReactOnActivate(id);
        if (deactivateOnStart) ReactOnDeactivate(id);
    }
    private void OnDestroy()
    {
        EventManager.GetEventManager.Activated -= ReactOnActivate;
        EventManager.GetEventManager.Deactivated -= ReactOnDeactivate;
    }
    protected void ReactOnActivate(int other_id)
    {
        if (!enabled) return;

        if (other_id == id) 
        {
            if (requireMultipleActive)
            {
                activeCount++;
                if (activeCount < requireActiveCount) return;   
            }
            StartCoroutine(DelayActivate());
        }    
    }
    protected void ReactOnDeactivate(int other_id)
    {
        if (!enabled) return;

        if (other_id == id)
        {
            // Current Implementation Never Calls deactivate
            if (requireMultipleActive)
            {
                activeCount--;
                return;
            }
            OnDeactivate();
        }

        if (ActivateOnce) EventManager.GetEventManager.Deactivated -= ReactOnDeactivate;
    }

    protected abstract void OnActivate();

    protected abstract void OnDeactivate();

    private IEnumerator DelayActivate()
    {
        yield return new WaitForSeconds(delay);
        OnActivate();
        if (ActivateOnce) EventManager.GetEventManager.Activated -= ReactOnActivate;
    }

    #if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Handles.Label(transform.position, this.id < 0 ? "Not Invokable" : "Invoker ID: " + this.id);
    }
    #endif
}
