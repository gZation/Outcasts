using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dialogue Manager 1.0 -> Maybe Let's refactor and change this guy up to be uncoupled from monobehaviour
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region Singleton
    private static DialogueManager instance; 
    public static DialogueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueManager>().GetComponent<DialogueManager>();
            }

            return instance;
        }
    }
    #endregion

    [SerializeField] private TextProducer m_dialogueProducer;
    [SerializeField] private Image profile;
    [SerializeField] private GameObject dialogueBox;
    private bool inProduction;

    private void Start()
    {
        HideDialogue();
    }
    private void Update()
    {
        
    }
    public void DisplayDialogue(DialogueObject a_dialogueObject)
    {
        if (inProduction)
        {
            StopAllCoroutines();
            m_dialogueProducer.StopProduction();
        }
        inProduction = true;
        dialogueBox.GetComponent<Animator>().Play("Appear");
        StartCoroutine(RunThroughDialogue(a_dialogueObject));
    }
    public void HideDialogue()
    {
         dialogueBox.GetComponent<Animator>().Play("Disappear");
         inProduction = false;
    }
    public void StopDialogue()
    {
        if (inProduction)
        {
            StopAllCoroutines();
            HideDialogue();
        }        
    }
    private IEnumerator RunThroughDialogue(DialogueObject a_dialogueObject)
    {
        foreach (Dialogue dialogue in a_dialogueObject.Dialogue)
        {
            AdjustProfileSegment(dialogue.Profile, dialogue.Alignment);
            m_dialogueProducer.TypeSound = dialogue.TypeSound;
            m_dialogueProducer.TMP_access.margin = new Vector4(25f, 0, 25f, 0);
            dialogue.OnDialogue.Invoke();
            yield return m_dialogueProducer.ReplaceTextWith(dialogue.Text, ProduceEffect.Typewriter, dialogue.Speed, dialogue.Delay);
        }

        HideDialogue();
        yield return null;
    }
    private void AdjustProfileSegment(Sprite a_profile, ProfileAlignment alignment)
    {
        profile.gameObject.SetActive(true);
        profile.sprite = null;
        profile.CrossFadeAlpha(0f, 0f, true);
        m_dialogueProducer.TMP_access.alignment = TextAlignmentOptions.Center;
        profile.sprite = a_profile;

        profile.transform.localPosition = new Vector3(Mathf.Abs(profile.transform.localPosition.x) * (alignment == ProfileAlignment.Right ? 1 : -1)
                , profile.transform.localPosition.y, profile.transform.localPosition.z);
        m_dialogueProducer.TMP_access.alignment = alignment == ProfileAlignment.Right ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
        
        if (a_profile != null)
        {
            profile.CrossFadeAlpha(1f, 0f, true);
        }      
    }
}
