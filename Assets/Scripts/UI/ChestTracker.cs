using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestTracker : MonoBehaviour
{
    #region Singleton
    private static ChestTracker instance;
    public static ChestTracker Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<ChestTracker>();
            }
            return instance;
        }
    }
    #endregion
    [SerializeField] private int maxNumberOfChests;
    [SerializeField] private int requiredChests;
    [SerializeField] private int currNumberOfChestOpened;
    [SerializeField] private TextMeshProUGUI m_trackerTextBox;
    private bool[] m_foundChests;
    public bool IsAllChestsOpen => (currNumberOfChestOpened / requiredChests) >= 1;
    public bool[] FoundChests => m_savedFoundChests;
    public int NumberOfChestsOpened => m_savedCurrNumberofChestOpened;

    private Animator m_animator;

    private bool[] m_savedFoundChests;
    private int m_savedCurrNumberofChestOpened;
    private void Awake()
    {
        m_foundChests = new bool[maxNumberOfChests];
        m_animator = GetComponentInChildren<Animator>();
        m_trackerTextBox.text = currNumberOfChestOpened.ToString() + "/" + requiredChests.ToString();
    }
    public void ResetChestCollectionToLastSave()
    {
        m_foundChests = (bool[])m_savedFoundChests?.Clone();
        currNumberOfChestOpened = m_savedCurrNumberofChestOpened;
        if (currNumberOfChestOpened < requiredChests) m_trackerTextBox.color = Color.white;
        UpdateUI();
    }
    public void SaveRecentChestCollection()
    {
        m_savedFoundChests = (bool[])m_foundChests.Clone();
        m_savedCurrNumberofChestOpened = currNumberOfChestOpened;
    }
    public void SetFoundChests(bool[] a_foundChests, int a_chestsOpenedCount)
    {
        if (a_foundChests == null) Debug.LogError("Can't send Null Chest's Array");
        ResetChestCount();
        for (int i = 0; i < m_foundChests.Length; i++)
        {
            m_foundChests[i] = a_foundChests[i];
        }
        currNumberOfChestOpened = a_chestsOpenedCount;
        UpdateUI();
    }
    public void FoundNewChest(int idx)
    {
        if (m_foundChests[idx]) return;
        m_foundChests[idx] = true;
        currNumberOfChestOpened++;
        StartCoroutine(UpdateChestTracker());
    }
    public bool IsChestFound(int idx)
    {
        if (idx >= m_foundChests.Length) return false;
        return m_foundChests[idx];
    }
    public void ResetChestCount()
    {
        m_foundChests = new bool[maxNumberOfChests];
        currNumberOfChestOpened = 0;
        m_trackerTextBox.color = Color.white;
    }
    public void ShowChestTracker()
    {
        m_animator.Play("ShowChestTracker");
    }
    public void HideChestTracker()
    {
        m_animator.Play("HideChestTracker");
    }
    private IEnumerator UpdateChestTracker()
    {
        m_animator.Play("ShowChestTracker");
        yield return new WaitForSeconds(2.3f);
        UpdateUI();
        // Hackey As Fucc Bruh
        yield return new WaitForSeconds(1.5f);
        m_animator.Play("HideChestTracker");
    }

    public void UpdateUI()
    {
        if (IsAllChestsOpen)
        {
            m_trackerTextBox.color = Color.green;
            m_trackerTextBox.text = currNumberOfChestOpened.ToString();
        }
        else
        {
            m_trackerTextBox.text = currNumberOfChestOpened.ToString() + "/" + requiredChests.ToString();
        }
    }
}
