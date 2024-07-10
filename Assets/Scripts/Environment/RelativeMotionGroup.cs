using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class RelativeMotionGroup : MonoBehaviour
{
    private Dictionary<Transform, Transform> objectsOnGroup;

    private void Awake()
    {
        objectsOnGroup = new Dictionary<Transform, Transform>();
        //SceneManager.activeSceneChanged += OnSceneUnload;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "feet" && !objectsOnGroup.ContainsKey(collision.transform.parent))
        {
            objectsOnGroup.Add(collision.transform.parent, collision.transform.parent.parent);
            collision.transform.parent.GetComponent<Pawn>().CurrentGroup = this;
        }
        if (collision.gameObject.tag == "physical")
        {
            objectsOnGroup.Add(collision.transform, collision.transform.parent);
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Pawn pawn = collision.transform.parent?.GetComponent<Pawn>();
        if (pawn != null && objectsOnGroup.ContainsKey(collision.transform.parent) && pawn.IsGrounded)
        {
            collision.transform.parent.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "feet" && objectsOnGroup.ContainsKey(collision.transform.parent))
        {
            collision.transform.parent.GetComponent<Pawn>().CurrentGroup = null;
            collision.transform.parent.SetParent(objectsOnGroup[collision.transform.parent]);
            objectsOnGroup.Remove(collision.transform.parent);
        }

        if (collision.gameObject.tag == "physical")
        {
            collision.transform.SetParent(objectsOnGroup[collision.transform]);
            objectsOnGroup.Remove(collision.transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "physical")
        {
            objectsOnGroup.Add(collision.transform, collision.transform.parent);
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "physical")
        {
            collision.transform.SetParent(objectsOnGroup[collision.transform]);
            objectsOnGroup.Remove(collision.transform);
        }
    }
    public void StopMotionGroup()
    {
        //Disable All Colliders On Platform
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }

        //Remove all transform in group
        foreach (Transform obj in objectsOnGroup.Keys)
        {
            obj.SetParent(objectsOnGroup[obj]);
            objectsOnGroup.Remove(obj);
        }

    }
    //private void OnSceneUnload(Scene curr, Scene next)
    //{
    //    Debug.Log("Scene Unloaded");
    //    // Disable All Colliders On Platform
    //    foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
    //    {
    //        collider.enabled = false;
    //    }
    //    // Remove all transform in group
    //    foreach (Transform obj in objectsOnGroup.Keys)
    //    {
    //        obj.SetParent(objectsOnGroup[obj]);
    //        objectsOnGroup.Remove(obj);
    //    }
    //    DontDestroyOnLoad(GameManager.Instance.Tinker);
    //    DontDestroyOnLoad(GameManager.Instance.Ashe);
    //}
}
