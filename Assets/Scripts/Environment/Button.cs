using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Invoker
{
    [SerializeField] private bool pressOnce = false;
    [SerializeField]
    private bool heavy = false;
    [SerializeField]
    private float lengthOfTime = 0.5f;
    private Vector3 buttonPos;
    private int entered;
    private float timer;
    private bool buttonPressed;
    [SerializeField] private Transform m_pushableButton;
    private AudioSource buttonPressSound;

    // Start is called before the first frame update
    void Start()
    {
        buttonPos = m_pushableButton.localPosition;
        entered = 0;
        buttonPressed = false;
        buttonPressSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= lengthOfTime) {
            if (buttonPressed) {
                m_pushableButton.localPosition = Vector3.LerpUnclamped(m_pushableButton.localPosition, Vector2.zero, timer/lengthOfTime);
            } else {
                m_pushableButton.localPosition = Vector3.LerpUnclamped(m_pushableButton.localPosition, buttonPos, timer/lengthOfTime);
            }
            timer += Time.deltaTime;
        }   
    }

    private void OnTriggerEnter2D(Collider2D other) {
        string otherTag = other.gameObject.tag;
        if ((otherTag == "Ashe" || (!heavy && otherTag == "Tinker") || (otherTag == "physical"))) {
            if (!buttonPressed && entered == 0)
            {
                buttonPressed = true;
                timer = 0f;
                buttonPressSound.Play();
                Activate();
            }
            entered++;
        }      
    }


    private void OnTriggerExit2D(Collider2D other) {
        string otherTag = other.gameObject.tag;
        if ((otherTag == "Ashe" || (!heavy && otherTag == "Tinker") || (otherTag == "physical"))) {
            if (!pressOnce && buttonPressed && entered == 1)
            {
                buttonPressed = false;
                timer = 0f;
                Deactivate();
            }
            entered--;   
        }      
    }
}
