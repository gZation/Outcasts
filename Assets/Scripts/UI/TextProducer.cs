using System.Collections;
using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Class <c>TextProducer</c> functions when attached directly to a game object with the TMP component
/// Supports audio production
/// </summary>
[RequireComponent(typeof(TMP_Text), typeof(AudioSource))]
public class TextProducer : MonoBehaviour
{
    [Header("Use Settings")]
    [SerializeField] private bool m_startOnEnable = false;
    [SerializeField, Range(0f, 10f)] protected float m_speed = 0.8f;
    [SerializeField] protected float m_delay = -1;
    [SerializeField] private ProduceEffect m_startEffect = ProduceEffect.None;

    [Header("Punctuation Adjustments")]

    protected TMP_Text m_textLabel;
    protected AudioClip typeSound;
    protected AudioSource soundSource;
    protected string initialText;

    public TMP_Text TMP_access => m_textLabel;

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),
        new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.3f)
    };
    public AudioClip TypeSound
    {
        set
        {
            typeSound = value;
            soundSource.clip = value;
        }
    }

    private void Awake()
    {
        m_textLabel = GetComponent<TextMeshProUGUI>();
        soundSource = GetComponent<AudioSource>();
        initialText = m_textLabel.text;
    }

    private void OnEnable()
    {
        if (m_startOnEnable)
        {
            m_textLabel.text = string.Empty;
            StartCoroutine(DelayEnableProduce());
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Method <c>WriteTextToLabel</c> Writes given string to a TMP's text value with a given effect and amplifier if neccessary. 
    /// </summary>
    /// <paramref name="a_text">The string going into the text label</param>
    /// <paramref name="effect">The Text effectType for writing the text to textLabel</param>
    /// <paramref name="a_amplifier">A float value that can be used for whatever the Text effect type requires it for</param>
    /// <returns></returns>
    public Coroutine WriteText(string a_text, ProduceEffect a_effect = ProduceEffect.None, float a_amplifier = 0, float a_delay = 0)
    {
        if (a_text == null)
        {
            throw new ArgumentNullException("Method WriteTextToLabel can only write non-null string values");
        }

        switch (a_effect)
        {
            case ProduceEffect.Typewriter:
                return StartCoroutine(TypeWriterEffect(a_text, a_amplifier, a_delay));
            case ProduceEffect.Fade:
                return StartCoroutine(FadeEffect(a_text, a_delay));
            default:
                return StartCoroutine(NoneEffect(a_text));
        }    
    }

    public Coroutine ReplaceTextWith(string a_text, ProduceEffect a_effect = ProduceEffect.None, float a_amplifier = 0, float a_delay = 0)
    {
        m_textLabel.text = string.Empty;
        return WriteText(a_text, a_effect, a_amplifier, a_delay);
    }
    public void StopProduction()
    {
        StopAllCoroutines();
        m_textLabel.text = string.Empty;
    }

    #region All Text Effects

    protected virtual IEnumerator NoneEffect(string a_text)
    {
        m_textLabel.text += a_text;
        yield return null;
    }
    protected virtual IEnumerator FadeEffect(string a_text, float a_delay)
    {
        m_textLabel.alpha = 0;
        m_textLabel.text += a_text;
        while (m_textLabel.alpha < 1)
        {
            m_textLabel.alpha += Time.fixedDeltaTime;
            yield return new WaitForSeconds((1 / (m_speed * 10f)));
        }
        m_textLabel.alpha = 1;

        // If a_delay was given a real value thus a fadeout is requested
        if (a_delay >= 0)
        {

            yield return new WaitForSeconds(a_delay);
            while (m_textLabel.alpha > 0)
            {
                m_textLabel.alpha -= Time.fixedDeltaTime;
                yield return new WaitForSeconds((1 / (m_speed * 10f)));
            }
            m_textLabel.alpha = 0;
            m_textLabel.text = string.Empty;
            m_textLabel.alpha = 1;
        }
    }
    protected virtual IEnumerator TypeWriterEffect(string a_text, float a_speed, float a_delay)
    {
        char c;
        for (int n = 0; n < a_text.Length; n++)
        {
            c = a_text[n];
            m_textLabel.text += c;
            soundSource?.Play();
            yield return new WaitForSeconds(PunctuationTime(c) + (1 / (a_speed * 10f)));
        }
        yield return new WaitForSeconds(a_delay);
    }

    #endregion

    /// <summary>
    /// <c>PunctuationTime</c> takes in a char value and checks if it is a valid punctuation that can be used to delay the waitTime
    /// for text production.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    protected float PunctuationTime(char character) 
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                return punctuationCategory.WaitTime;
            }
        }

        return 0f;
    }

    // Calls for Inspector Devs
    public void FadeText(string a_text)
    {
        ReplaceTextWith(a_text, ProduceEffect.Fade, 0, m_delay);
    }
    public void SetSpeed(float a_speed)
    {
        m_speed = a_speed;
    }
    public void SetDelay(float a_delay)
    {
        m_delay = a_delay;
    }
    private IEnumerator DelayEnableProduce()
    {
        yield return new WaitForSeconds(m_delay);
        ReplaceTextWith(initialText, m_startEffect, 5f * m_speed);
    }
}

public enum ProduceEffect
{
    None,
    Typewriter,
    Fade,
}

public readonly struct Punctuation
{
    public readonly HashSet<char> Punctuations;
    public readonly float WaitTime;

    public Punctuation(HashSet<char> punctuations, float waitTime)
    {
        Punctuations = punctuations;
        WaitTime = waitTime;
    }
}
