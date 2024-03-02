using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemTracker : MonoBehaviour
{
    #region Singleton
    private static GemTracker m_instance;
    public static GemTracker Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GemTracker>();
            }
            return m_instance;
        }
    }
    #endregion
    [Header("UIComponents")]
    [SerializeField] private TextMeshProUGUI m_tinkerGemTracking;
    [SerializeField] private TextMeshProUGUI m_asheGemTracking;
    [SerializeField] private Animator m_tinkerTrackerAnimator;
    [SerializeField] private Animator m_asheTrackerAnimator;
    // Gem Data
    private int m_tinkerGems = 0;
    private int m_asheGems = 0;
    public int TinkerGemsCollected => m_savedTinkerGems;
    public int AsheGemsCollected => m_savedAsheGems;
    private int m_savedTinkerGems = 0;
    private int m_savedAsheGems = 0;
    public void Awake()
    {
        UpdateUI();
    }
    public void ResetGemCollectionToLastSave()
    {
        m_asheGems = m_savedAsheGems;
        m_tinkerGems = m_savedTinkerGems;
        UpdateUI();
    }
    public void SaveRecentGemCollection()
    {
        m_savedAsheGems = m_asheGems;
        m_savedTinkerGems = m_tinkerGems;
        for (int i = 1; i <= 3; i++)
        {
            if ((m_savedAsheGems + m_savedTinkerGems) >= (i * 10))
            {
                GameManager.Instance.MarkAchievement((AchievementType)((int)AchievementType.Gem10 + (i - 1)));
            }
        }
    }
    public void TinkerCollectsGem()
    {
        //m_tinkerGems++;
        //m_tinkerGemTracking.text = m_tinkerGems.ToString();
        StartCoroutine(ShowTinkerTrackerFor(2f));
    }
    public void AsheCollectsGem()
    {
        //m_asheGems++;
        //m_asheGemTracking.text = m_asheGems.ToString();
        StartCoroutine(ShowAsheTrackerFor(2f));
    }
    public void InitializeGemCount(int a_tinkerGems, int a_asheGems)
    {
        m_tinkerGems = a_tinkerGems;
        m_asheGems = a_asheGems;
        UpdateUI();
    }
    public void ClearGemCount()
    {
        m_tinkerGems = 0;
        m_asheGems = 0;
        UpdateUI();
    }
    public void UpdateUI()
    {
        m_tinkerGemTracking.text = m_tinkerGems.ToString();
        m_tinkerGemTracking.color = m_tinkerGems >= 15 ? Color.green : Color.white;
        m_asheGemTracking.text = m_asheGems.ToString();
        m_asheGemTracking.color = m_asheGems >= 15 ? Color.green : Color.white;
    }
    public void ShowTinkerTracker()
    {
        m_tinkerTrackerAnimator.Play("ShowChestTracker");
    }
    public void HideTinkerTracker()
    {
        m_tinkerTrackerAnimator.Play("HideChestTracker");
    }
    public void ShowAsheTracker()
    {
        m_asheTrackerAnimator.Play("ShowChestTracker");
    }
    public void HideAsheTracker()
    {
        m_asheTrackerAnimator.Play("HideChestTracker");
    }
    private IEnumerator ShowBothTrackersFor(float delay)
    {
        m_asheTrackerAnimator.Play("ShowChestTracker");
        m_tinkerTrackerAnimator.Play("ShowChestTracker");
        yield return new WaitForSeconds(delay);
        m_asheTrackerAnimator.Play("HideChestTracker");
        m_tinkerTrackerAnimator.Play("HideChestTracker");
    }

    private IEnumerator ShowTinkerTrackerFor(float delay)
    {
        m_tinkerTrackerAnimator.Play("ShowChestTracker");
        yield return new WaitForSeconds(delay / 2f);
        m_tinkerGems++;
        m_tinkerGemTracking.text = m_tinkerGems.ToString();
        yield return new WaitForSeconds(delay / 2f);
        m_tinkerTrackerAnimator.Play("HideChestTracker");
    }

    private IEnumerator ShowAsheTrackerFor(float delay)
    {
        m_asheTrackerAnimator.Play("ShowChestTracker");
        yield return new WaitForSeconds(delay / 2f);
        m_asheGems++;
        m_asheGemTracking.text = m_asheGems.ToString();
        yield return new WaitForSeconds(delay / 2f);
        m_asheTrackerAnimator.Play("HideChestTracker");
    }
}
