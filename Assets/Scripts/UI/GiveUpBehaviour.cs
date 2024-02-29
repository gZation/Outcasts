using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveUpBehaviour : MonoBehaviour
{
    [SerializeField] private SkipIndicator agreeIndicator;
    [SerializeField] private SkipIndicator disagreeIndicator;
    [SerializeField] private string returnToScene;

    private void Start()
    {
        if (agreeIndicator == null || disagreeIndicator == null)
        {
            Debug.LogError("Bruh where the fucking indicators at?");
            return;
        }
    }

    public void StartBehaviour()
    {
        StartCoroutine(agreeEvent());
        StartCoroutine(disagreeEvent());
    }

    private IEnumerator agreeEvent()
    {
        float tinkerHeld = 0f;
        float asheHeld = 0f;
        if (GameManager.Instance.AshePC != null && GameManager.Instance.TinkerPC != null)
        {
            yield return new WaitUntil(() =>
            {
                if (GameManager.Instance.TinkerPC.PlayerInput.actions["Submit"].inProgress)
                {
                    tinkerHeld += Time.deltaTime;
                }
                else
                {
                    tinkerHeld -= Time.deltaTime;
                }
                if (GameManager.Instance.AshePC.PlayerInput.actions["Submit"].inProgress)
                {
                    asheHeld += Time.deltaTime;
                }
                else
                {
                    asheHeld -= Time.deltaTime;
                }

                // Clamp Value
                tinkerHeld = Mathf.Clamp(tinkerHeld, 0f, 0.5f);
                asheHeld = Mathf.Clamp(asheHeld, 0f, 0.5f);

                // Notify UI
                agreeIndicator.TinkerHalf = tinkerHeld;
                agreeIndicator.AsheHalf = asheHeld;

                return tinkerHeld == 0.5f && asheHeld == 0.5f;
            });
        }
        else if (GameManager.Instance.SC != null)
        {
            yield return new WaitUntil(() =>
            {
                if (GameManager.Instance.SC.PlayerInput.actions["Navigate"].ReadValue<Vector2>() == Vector2.up) // wack
                {
                    tinkerHeld += Time.deltaTime;
                }
                else
                {
                    tinkerHeld -= Time.deltaTime;
                }
                if (GameManager.Instance.SC.PlayerInput.actions["Navigate"].ReadValue<Vector2>() == Vector2.up) // wack
                {
                    asheHeld += Time.deltaTime;
                }
                else
                {
                    asheHeld -= Time.deltaTime;
                }

                // Clamp Value
                tinkerHeld = Mathf.Clamp(tinkerHeld, 0f, 0.5f);
                asheHeld = Mathf.Clamp(asheHeld, 0f, 0.5f);

                // Notify UI
                agreeIndicator.TinkerHalf = tinkerHeld;
                agreeIndicator.AsheHalf = asheHeld;

                return tinkerHeld == 0.5f && asheHeld == 0.5f;
            });
        }
        else
        {
            Debug.LogError("No Pawn Controllers Available To Skip Pawn Event");
            yield return null;
        }
        StopAllCoroutines();
        GameManager.Instance.UnPauseGame();
        GameManager.Instance.LoadToScene(returnToScene);
    }

    private IEnumerator disagreeEvent()
    {
        float tinkerHeld = 0f;
        float asheHeld = 0f;
        if (GameManager.Instance.AshePC != null && GameManager.Instance.TinkerPC != null)
        {
            yield return new WaitUntil(() =>
            {
                if (GameManager.Instance.TinkerPC.PlayerInput.actions["Cancel"].inProgress)
                {
                    tinkerHeld += Time.deltaTime;
                }
                else
                {
                    tinkerHeld -= Time.deltaTime;
                }
                if (GameManager.Instance.AshePC.PlayerInput.actions["Cancel"].inProgress)
                {
                    asheHeld += Time.deltaTime;
                }
                else
                {
                    asheHeld -= Time.deltaTime;
                }

                // Clamp Value
                tinkerHeld = Mathf.Clamp(tinkerHeld, 0f, 0.5f);
                asheHeld = Mathf.Clamp(asheHeld, 0f, 0.5f);

                // Notify UI
                disagreeIndicator.TinkerHalf = tinkerHeld;
                disagreeIndicator.AsheHalf = asheHeld;

                return tinkerHeld == 0.5f && asheHeld == 0.5f;
            });
        }
        else if (GameManager.Instance.SC != null)
        {
            yield return new WaitUntil(() =>
            {
                if (GameManager.Instance.SC.PlayerInput.actions["Navigate"].ReadValue<Vector2>() == Vector2.down) // wack
                {
                    tinkerHeld += Time.deltaTime;
                }
                else
                {
                    tinkerHeld -= Time.deltaTime;
                }
                if (GameManager.Instance.SC.PlayerInput.actions["Navigate"].ReadValue<Vector2>() == Vector2.down) // wack
                {
                    asheHeld += Time.deltaTime;
                }
                else
                {
                    asheHeld -= Time.deltaTime;
                }

                // Clamp Value
                tinkerHeld = Mathf.Clamp(tinkerHeld, 0f, 0.5f);
                asheHeld = Mathf.Clamp(asheHeld, 0f, 0.5f);

                // Notify UI
                disagreeIndicator.TinkerHalf = tinkerHeld;
                disagreeIndicator.AsheHalf = asheHeld;

                return tinkerHeld == 0.5f && asheHeld == 0.5f;
            });
        }
        else
        {
            Debug.LogError("No Pawn Controllers Available To Skip Pawn Event");
            yield return null;
        }
        StopAllCoroutines();
        GameManager.Instance.UnPauseGame();
    }

    public void OnEnable()
    {
        StartBehaviour();
    }
}
