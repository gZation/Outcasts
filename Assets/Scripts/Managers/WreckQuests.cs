using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckQuests : MonoBehaviour
{
    #region Singleton
    private static WreckQuests m_instance;
    public static WreckQuests Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<WreckQuests>();
            }
            return m_instance;
        }
    }
    #endregion
    [SerializeField] private GameObject BTT;
    [SerializeField] private GameObject BTC;
    [SerializeField] private GameObject BTA;
    [SerializeField] private GameObject BTF;
    [SerializeField] private GameObject C1G;
    [SerializeField] private GameObject C2G;
    [SerializeField] private GameObject C3G;
    [SerializeField] private GameObject MTB;
    [SerializeField] private GameObject SLW;
    [SerializeField] private GameObject DRK;
    [SerializeField] private GameObject ALT;
    private Animator m_animator;
    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }
    public void UpdateUI()
    {
        if (FileManagment.LoadFromSaveFile(GameManager.Instance.CurrentProfile, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);
            BTT.SetActive(sd.BeatTheTutorial);
            BTC.SetActive(sd.BeatTheCave);
            BTA.SetActive(sd.BeatTheAirDungeon);
            BTF.SetActive(sd.BeatTheFinalLevel);
            C1G.SetActive(sd.TenGems);
            C2G.SetActive(sd.TwentyGems);
            C3G.SetActive(sd.ThirtyGems);
            MTB.SetActive(sd.MTB);
            SLW.SetActive(sd.RUNNN);
            DRK.SetActive(sd.LightItUp);
            ALT.SetActive(sd.TheFuture);
        }
    }
    public void ShowQuests()
    {
        m_animator.Play("QuestShow");
    }
    public void HideQuests()
    {
        m_animator.Play("QuestHide");
    }
}
