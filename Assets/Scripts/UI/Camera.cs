using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField, Range(1f, 10f)] private float m_moveSpeed = 1f;
    [SerializeField] private Vector3 targetPosition;

    [Header("Playful")]
    [SerializeField] private CameraShake m_cameraShaker;

    public CameraShake CamShaker => m_cameraShaker;

    #region Technical

    #endregion

    public void ShiftTo(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, m_moveSpeed * Time.deltaTime);
        }
    }
}
