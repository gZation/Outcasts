using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseStateManager : MonoBehaviour
{
    [SerializeField] private Chase leftPaw;
    [SerializeField] private Chase rightPaw;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    private bool chasing = false;

    private void Start()
    {
        if (leftPaw == null || rightPaw == null || leftPoint == null || rightPoint == null)
            Debug.LogError("ChaseStateManager Requires paws and transforms to be initialized");
    }
    private void Update()
    {
        if (!chasing)
        {
            chasing = true;
            StartCoroutine(ChaseEndChecker());
        }
    }

    private IEnumerator ChaseEndChecker()
    {
        float timeDelay = Time.time + 0.2f;
        while (Time.time < timeDelay || !leftPaw.IsGrabbing || !rightPaw.IsGrabbing)
        {
            if (!leftPaw.IsGrabbing || !rightPaw.IsGrabbing) timeDelay = Time.time + 0.2f;
            yield return new WaitForFixedUpdate();
        }
        leftPaw.ChangeSpeed(13f);
        rightPaw.ChangeSpeed(13f);

        leftPaw.ChangeTargets(leftPoint);
        rightPaw.ChangeTargets(rightPoint);

        leftPaw.StartChase();
        rightPaw.StartChase();
        
        while (Vector2.Distance(leftPoint.position, leftPaw.transform.position) > 0.01f
            || Vector2.Distance(rightPoint.position, rightPaw.transform.position) > 0.01f)
        {
            yield return new WaitForFixedUpdate();
        }
        leftPaw.StopChase();
        rightPaw.StopChase();
        yield return new WaitForSecondsRealtime(0.2f);
        leftPaw.UnGrabTarget();
        rightPaw.UnGrabTarget();

        yield return new WaitForSecondsRealtime(2f);

        leftPaw.ChangeTargetsToTna(false);
        rightPaw.ChangeTargetsToTna(true);

        chasing = false; // Chase Restarts
    }
    public void SetLeftPoint(Transform point)
    {
        leftPoint = point;
    }
    public void SetRightPoint(Transform point)
    {
        rightPoint = point;
    }
    public void CheckAchievmentCondition()
    {
        if (!leftPaw.GetComponent<Paw>().Stunned && !rightPaw.GetComponent<Paw>())
        {
            GameManager.Instance.MarkAchievement(AchievementType.RUNNN);
        }
    }
}
