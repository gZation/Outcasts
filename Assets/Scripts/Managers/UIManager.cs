using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using TMPro;
using System.Drawing.Text;
using UnityEngine.InputSystem.Composites;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager instance;
    public static UIManager Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    Debug.LogError("UIManager Prefab is required in Game!");
                }
            }
            return instance; 
        }
    }
    #endregion

    [SerializeField] private EventSystem m_eventSystem;
    [SerializeField] private InputSystemUIInputModule m_inputSystemUIInputModule;

    [Header("Pause Components")]
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_pauseButt;
    [SerializeField] private GameObject m_optionButt;
    [SerializeField] private GameObject m_giveUpButt;

    [Header("Flip Panels")]
    [SerializeField] private GameObject m_menuPanel;
    [SerializeField] private GameObject m_optionsPanel;
    [SerializeField] private GameObject m_giveUpDefault;
    [SerializeField] private GameObject m_giveUpTinker;
    [SerializeField] private GameObject m_giveUpAshe;

    [SerializeField] private TextMeshProUGUI m_playerPaused;
    public EventSystem EventSystem => m_eventSystem;
    public InputSystemUIInputModule ISUIIM => m_inputSystemUIInputModule;

    private void Start()
    {
        if (m_eventSystem == null) m_eventSystem = GetComponentInChildren<EventSystem>();
        if (m_inputSystemUIInputModule == null) m_inputSystemUIInputModule = GetComponent<InputSystemUIInputModule>();
    }

    public void OpenPauseMenu(PlayerController pc = null)
    {
        m_pauseMenu.SetActive(true);
        m_menuPanel.SetActive(true);
        m_optionsPanel.SetActive(false);
        m_giveUpDefault.SetActive(false);
        m_giveUpAshe.SetActive(false);
        m_giveUpTinker.SetActive(false);
        if (pc != null)
        {
            m_eventSystem.SetSelectedGameObject(m_pauseButt);
            m_inputSystemUIInputModule.actionsAsset = pc.PlayerInput.actions;
            m_playerPaused.text = pc.ControlledPawn.Data.Name + " Paused";
            SetGiveUpReactionType(pc.ControlledPawn.Data.Name == "Tinker" ? GiveUpType.Tinker : GiveUpType.Ashe);
        }
        else
        {
            SetGiveUpReactionType(GiveUpType.Both);
        }
    }

    public void ClosePauseMenu()
    {
        m_pauseMenu.SetActive(false);
    }

    public void SetGiveUpReactionType(GiveUpType type)
    {
        var button = m_giveUpButt.GetComponent<UnityEngine.UI.Button>();
        button.onClick.RemoveAllListeners();
        switch (type)
        {
            case GiveUpType.Tinker:
                button.onClick.AddListener(delegate {
                    m_menuPanel.SetActive(false);
                    m_giveUpTinker.SetActive(true);
                });
                break;
            case GiveUpType.Ashe:
                button.onClick.AddListener(delegate {
                    m_menuPanel.SetActive(false);
                    m_giveUpAshe.SetActive(true);
                });
                break;
            default:
                button.onClick.AddListener(delegate {
                    m_menuPanel.SetActive(false);
                    m_giveUpDefault.SetActive(true);
                });
                break;
        }
    }
}

public enum GiveUpType
{
    Tinker,
    Ashe,
    Both
}
