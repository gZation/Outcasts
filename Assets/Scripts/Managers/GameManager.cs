using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    Debug.LogError("GameManager Prefab is required in level scene!");
                }
            }

            return instance;
        }
    }
    #endregion 

    [Header("Player Pawn Management")]
    [SerializeField] private PlayerInputManager m_pim;
    [SerializeField] private TinkerPawn m_tinker;
    [SerializeField] private AshePawn m_ashe;
    private PlayerController m_tinkerPC;
    private PlayerController m_ashePC;
    private SoloController m_SC;
    public TinkerPawn Tinker => m_tinker;
    public AshePawn Ashe => m_ashe;
    public PlayerController TinkerPC => m_tinkerPC;
    public PlayerController AshePC => m_ashePC;
    public SoloController SC => m_SC;

    [Header("UI Comoponents")]
    [SerializeField] private GameObject m_visualCanvas;
    [SerializeField] private CharacterSelection m_characterSelection;
    [SerializeField] private CharacterSelection m_characterSelection2;
    [SerializeField] private TextMeshProUGUI m_onScreenMessage;
    [SerializeField] private Animator m_cinematicCover;
    [SerializeField] private SkipIndicator m_skipIndicator;
    public SkipIndicator SkipIndicator => m_skipIndicator;

    [Header("Level Management")]
    [SerializeField] private Transform m_levelThings;
    [SerializeField] private RoomManager m_roomManager;
    [SerializeField] private LevelManager m_levelManager;
    [SerializeField] private Animator m_transitionAnimator;
    private TransitionType m_transitionEntry = TransitionType.Fade;
    private TransitionType m_transitionExit = TransitionType.Fade;
    public Animator TransitionAnimator => m_transitionAnimator;
    public RoomManager CurrRoomManager => m_roomManager;
    public LevelManager LevelManager
    {
        get { return m_levelManager; }
        set { m_levelManager = value; }
    }
    public TransitionType TranisitionEntry
    {
        get => m_transitionEntry; set { m_transitionEntry = value;}
    }
    public TransitionType TranisitionExit
    {
        get => m_transitionExit; set { m_transitionExit = value; }
    }
    private Queue<string> m_scenes;
    [SerializeField] private string m_currScene;
    

    [Header("Dev Settings")]
    [SerializeField] private string[] initialScenesToEnqueue;
    [SerializeField] private string m_currentProfile;
    [SerializeField] private float m_timeTracking;
    [SerializeField] private bool wasReloaded = false;
    public bool WasReloaded => wasReloaded;

    // Save Data Tracking
    private int wreckPoints;
    private int saved_wreckPoints;

    #region DETAILS
    private bool isPaused = false;
    private bool pauseTimer = false;
    private bool tranistionExited = false;
    private bool tranistionEntered = false;
    private bool cinematicActive = false;
    #endregion
    private void Update()
    {
        // Tracking the time
        if (isPaused || pauseTimer) return;
        m_timeTracking += Time.deltaTime;
    }
    private void Awake()
    {
        // Just in case the gameManager finds itself in the same scene
        if (instance != null && FindObjectOfType<GameManager>() != null)
        {
            Destroy(gameObject);
            return;
        }
        m_currScene = SceneManager.GetActiveScene().name;
        isPaused = false;

        m_scenes = new Queue<string>();

        AddSceneToQueue(initialScenesToEnqueue);

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void Start()
    {
        if (m_visualCanvas == null) { Debug.LogError("Error: VisualCanvas is Missing as an instance in GameManger!"); }

        // Default Loading Profile
        //if (m_currentProfile != null || m_currentProfile != "")
        //{
        //    LoadGameProfile(m_currentProfile);
        //}
        // Initial Unaffected Dont Destroys
        //DontDestroyOnLoad(gameObject);
        //DontDestroyOnLoad(m_visualCanvas);
        //DontDestroyOnLoad(transform.parent);
    }

    public void HandlePlayerControllerEnter(PlayerInput pi)
    {
        DontDestroyOnLoad(pi);

        // Player Controller Moment
        var pc = pi.GetComponent<PlayerController>();

        if (pc != null)
        { 
            if (m_tinkerPC == null && m_ashePC == null)
            {
                OpenUpPlayerSelecitonMenu(pi);
            }
            else if (m_ashePC == null)
            {
                m_ashePC = pc;
                m_ashePC.PlayerInput.actions["Join"].Disable();
                GivePawn(m_ashePC, m_ashe);
            }
            else if (m_tinkerPC == null)
            {
                m_tinkerPC = pc;
                m_tinkerPC.PlayerInput.actions["Join"].Disable();
                GivePawn(m_tinkerPC, m_tinker);
            }
            if (m_pim.playerCount >= m_pim.maxPlayerCount)
            {
                m_pim.DisableJoining();
            }
            return;
        } 
        
        // Solo Controller Moment
        var sc = pi.GetComponent<SoloController>();

        if (sc != null)
        {
            sc.ControlTnAPawns(m_tinker, m_ashe);
            m_SC = sc;   
            m_SC.PlayerInput.actions["Join"].Disable();
            m_pim.DisableJoining();
        }
    }

    public void GivePawn(PlayerController pc, Pawn pawn)
    {
        pc.PlayerInput.actions["Pause"].Disable();
        pc.ControlPawn(pawn);
        pc.EnablePawnControl();
    }

    public void HandlePlayerControllerExit(PlayerInput pi)
    {
        var pc = pi.GetComponent<PlayerController>();

        if (pc != null)
        {
            if (pc.ControlledPawn == m_tinker)
            {
                m_ashePC = null;
                m_characterSelection.Select_Tinker.interactable = true;
            }
            else if (pc.ControlledPawn == m_ashe)
            {
                m_tinkerPC = null;
                m_characterSelection.Select_Ashe.interactable = true;
            }

            pc.ControlPawn(null);
            Destroy(pc.gameObject);
            if (m_pim.playerCount < m_pim.maxPlayerCount)
            {
                m_pim.EnableJoining();
            }
            return;
        }

        var sc = pi.GetComponent<SoloController>();
        
        if (sc != null)
        {
            // TODO: Fill out details corresponding to solo controller input
            m_SC = null;
            Destroy(sc.gameObject);
        }
    }

    public void OpenUpPlayerSelecitonMenu(PlayerInput pi)
    {
        var pc = pi.GetComponent<PlayerController>();
        // Thank you for being nice PC
        //

        //This could have been just done through an array, but as we are only having two pawns to keep track of
        //This will do fine no matter its icckyniss
        if (pi.playerIndex == 0)
        {
            m_characterSelection.Display(false, pi.playerIndex + 1);
            m_characterSelection.Select_Tinker.onClick.AddListener(delegate {
                m_tinkerPC = pi.GetComponent<PlayerController>(); 
                GivePawn(m_tinkerPC, m_tinker); 
            });
            m_characterSelection.Select_Ashe.onClick.AddListener(delegate {
                m_ashePC = pi.GetComponent<PlayerController>();
                GivePawn(m_ashePC, m_ashe);
            });

            pc.GetComponentInChildren<MultiplayerEventSystem>().playerRoot = m_characterSelection.gameObject;
            pc.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(m_characterSelection.Select_Tinker.gameObject);
        }
        else
        {
            m_characterSelection2.Display(true, pi.playerIndex + 1);
            m_characterSelection2.Select_Tinker.onClick.AddListener(delegate {
                m_tinkerPC = pi.GetComponent<PlayerController>();
                GivePawn(m_tinkerPC, m_tinker);
            });
            m_characterSelection2.Select_Ashe.onClick.AddListener(delegate {
                m_ashePC = pi.GetComponent<PlayerController>();
                GivePawn(m_ashePC, m_ashe);
            });

            pc.GetComponentInChildren<MultiplayerEventSystem>().playerRoot = m_characterSelection2.gameObject;
            pc.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(m_characterSelection2.Select_Tinker.gameObject);
        }
    }

    public void TogglePause(PlayerController pc = null)
    {
        if (isPaused) UnPauseGame();
        else PauseGame(pc);
    }
    public void PauseGame(PlayerController pc = null)
    {
        if (isPaused) return; 
        //Time.timeScale = 0;
        isPaused = true;
        m_tinkerPC?.OnPausePawnControl();
        m_ashePC?.OnPausePawnControl();
        m_SC?.OnPausePawnControl();

        // UI Things that appear
        UIManager.Instance.OpenPauseMenu(pc);
        ChestTracker.Instance.ShowChestTracker();
        GemTracker.Instance.ShowAsheTracker();
        GemTracker.Instance.ShowTinkerTracker();
    }

    public void UnPauseGame()
    {
        if (!isPaused) return;
        //Time.timeScale = 1;
        isPaused = false;
        m_tinkerPC?.OnUnPausePawnControl();
        m_ashePC?.OnUnPausePawnControl();
        m_SC?.OnUnPausePawnControl();

        // UI Things to Hide
        UIManager.Instance.ClosePauseMenu();
        ChestTracker.Instance.HideChestTracker();
        GemTracker.Instance.HideAsheTracker();
        GemTracker.Instance.HideTinkerTracker();
    }

    #region Scene Management
    public void LoadToScene(string scene)
    {
        wasReloaded = m_currScene == scene;
        m_currScene = scene;        
        
        TransitionExit();
        StartCoroutine(LoadSceneWithDelay(1.2f));
    }
    public void TransitionToNextScene()
    {
        LoadToScene(m_scenes.Count > 0 ? m_scenes.Dequeue() : "Hub");
    }
    public void QuitToMainMenu()
    {
        UnPauseGame();
        if (m_ashePC != null)
        {
            //Debug.Log("Ashe Pre: " + m_ashePC.ControlledPawn.name);
            //Debug.Log("Tinker Pre: " + m_tinkerPC.ControlledPawn.name);
            //Debug.Log("Destroied Ashe");
            m_ashePC.ControlPawn(null);
            //Destroy(m_ashePC.gameObject);
            //Debug.Log("Ashe: " + m_ashePC != null ? m_ashePC.ControlledPawn.name : "");
            //Debug.Log("Tinker: " + m_tinkerPC != null ? m_tinkerPC.ControlledPawn.name : "");
            m_ashePC = null;
        }
        if (m_tinkerPC != null)
        {
            Debug.Log("Destroied Tinker");
            //Destroy(m_tinkerPC.gameObject);
            m_tinkerPC.ControlPawn(null);
            m_tinkerPC = null;
        }
        if (m_SC != null)
        {
            Destroy(m_SC.gameObject);
            m_SC = null;
        }
        LoadToScene("MainMenu");
    }
    public void QuitToDesktop()
    {
        Application.Quit();
    }
    // Callable For UI in LevelThings Only
    public void ReloadLevel()
    {
        m_levelManager.ReloadLevel();
    }
    //
    public void ReloadCurrentScene()
    {
        UnPauseGame();
        ChestTracker.Instance.ResetChestCollectionToLastSave();
        GemTracker.Instance.ResetGemCollectionToLastSave();
        LoadToScene(m_currScene);
    }

    public void AddSceneToQueue(string scene)
    {
        m_scenes.Enqueue(scene);
    }

    public void AddSceneToQueue(string[] scenes)
    {
        foreach (string scene in scenes)
            m_scenes.Enqueue(scene);
    }

    public void ClearSceneQueue()
    {
        m_scenes.Clear();
    }
    public void DisplayOnScreenMessage(string text)
    {
        m_onScreenMessage.text = text;
        m_onScreenMessage.GetComponent<Animator>().Play("TextScroll");
    }
    private void OnSceneChange(Scene current, Scene next)
    {
        //Dev Bs only
        DialogueManager.Instance.StopDialogue();
        if (m_currScene == "hub")
        {
            ChestTracker.Instance.ResetChestCount();
            GemTracker.Instance.ClearGemCount();
        }

        // This is wack and breaks the game
        // FUCK THE MAIN MENU!!!!! NO NEED!
        if (next.name == "MainMenu")
        {
            Destroy(transform.parent.gameObject);
            return;
        }
        m_ashe.IsLifting = false;
        m_tinker.IsHeld = false;
        m_tinker.transform.SetParent(m_levelThings, true);
        m_ashe.transform.SetParent(m_levelThings, true);

        //DontDestroyOnLoad(m_tinker);
        //DontDestroyOnLoad(m_ashe);
    }

    private IEnumerator LoadSceneWithDelay(float seconds)
    {
        m_ashe.SecondaryAction();
        m_ashe.DisableLiftingRegion();
        yield return new WaitForSeconds(seconds);
        m_ashe.gameObject.SetActive(false);
        m_tinker.gameObject.SetActive(false);
        m_ashe.EnableLiftingRegion();
        m_ashe.CurrentGroup?.StopMotionGroup();
        m_tinker.CurrentGroup?.StopMotionGroup();
        m_tinker.transform.SetParent(m_levelThings, true);
        m_ashe.transform.SetParent(m_levelThings, true);
        SceneManager.LoadSceneAsync(m_currScene);  
        //This doesn't do what you think it does
        //if (!SceneManager.GetSceneByName(m_currScene).IsValid())
        //{
        //    SceneManager.LoadSceneAsync("Hub");
        //} 
    }
    public void ActivateCinematic()
    {
        if (cinematicActive) return;
        cinematicActive = true;
        m_cinematicCover.Play("CinematicOpen");
    }

    public void DeactivateCinematic()
    {
        if (!cinematicActive) return;
        cinematicActive = false;
        m_cinematicCover.Play("CinematicClose");
    }
    public void TransitionEnter()
    {
        if (tranistionEntered) return; // If already entered, do not replay
        tranistionExited = false;
        tranistionEntered = true;
        switch (m_transitionEntry)
        {
            case TransitionType.None:
                // Just Do Nothing
                break;
            case TransitionType.Fade:
                m_transitionAnimator.Play("UnFade");
                break;
            case TransitionType.Doors:
                m_transitionAnimator.Play("OpenDoor");
                break;
            case TransitionType.BigDoor:
                m_transitionAnimator.Play("OpenSingleDoor");
                break;
        }
    }
    public void TransitionExit()
    {
        if (tranistionExited) return; // If already exited, do not replay
        tranistionExited = true;
        tranistionEntered = false;
        switch (m_transitionExit)
        {
            case TransitionType.None:
                // Just Do Nothing
                break;
            case TransitionType.Fade:
                m_transitionAnimator.Play("Fade");
                break;
            case TransitionType.Doors:
                m_transitionAnimator.Play("CloseDoor");
                break;
            case TransitionType.BigDoor:
                m_transitionAnimator.Play("CloseSingleDoor");
                break;
        }
    }
    #endregion

    #region Saving Current Data
    public void SaveGameToProfile(string a_profileName)
    {
        SaveData sd = new SaveData();
        this.PopulateSaveData(sd);
        FileManagment.WriteToSaveFile(a_profileName, sd.ToJson());
    }
    public void PopulateSaveData(SaveData a_SaveData)
    {
        a_SaveData.ProfileName = m_currentProfile;
        a_SaveData.CurrScene = m_currScene;
        a_SaveData.ChestCollected = ChestTracker.Instance.FoundChests;
        a_SaveData.ChestsCount = ChestTracker.Instance.NumberOfChestsOpened;
        a_SaveData.TinkerGemCount = GemTracker.Instance.TinkerGemsCollected;
        a_SaveData.AsheGemCount = GemTracker.Instance.AsheGemsCollected;
        a_SaveData.Points = saved_wreckPoints;
        a_SaveData.PlayerTimeInSeconds = m_timeTracking;
    }
    public void LoadGameProfile(string a_profile)
    {
        if (FileManagment.LoadFromSaveFile(a_profile, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);
            LoadFromSaveData(sd);
            LoadToScene(sd.CurrScene); // Better here to not confuse game I hope
        }
    }
    public void LoadFromSaveData(SaveData a_SaveData)
    {
        m_currentProfile = a_SaveData.ProfileName;
        m_timeTracking = a_SaveData.PlayerTimeInSeconds;
        ChestTracker.Instance.SetFoundChests(a_SaveData.ChestCollected, a_SaveData.ChestsCount);
        GemTracker.Instance.InitializeGemCount(a_SaveData.TinkerGemCount, a_SaveData.AsheGemCount);
        saved_wreckPoints = a_SaveData.Points;
        wreckPoints = saved_wreckPoints;
    }
    public void SaveGameToCurrentProfile()
    {
        if (m_currentProfile != null || m_currentProfile != "")
        ChestTracker.Instance.SaveRecentChestCollection();
        GemTracker.Instance.SaveRecentGemCollection();
        saved_wreckPoints = wreckPoints;
        SaveGameToProfile(m_currentProfile);
    }
    public void ChangeCurrentProfile(string newProfile)
    {
        m_currentProfile = newProfile;
    }
    public void PauseRecordTimer()
    {
        pauseTimer = true;
    }
    public void ResumeRecordTimer()
    {
        pauseTimer = false; 
    }
    public void MarkAchievement(AchievementType type)
    {
        SaveData sd = new SaveData();
        if (FileManagment.LoadFromSaveFile(m_currentProfile, out var json))
        {
            sd.LoadFromJson(json);
        } 
        else
        {
            return;
        }
            
        switch (type)
        {
            case AchievementType.BeatTheTutorial:
                saved_wreckPoints += 5;
                sd.BeatTheTutorial = true;
                break;
            case AchievementType.BeatTheCave:
                saved_wreckPoints += 5;
                sd.BeatTheCave = true;
                break;
            case AchievementType.BeatTheAirDungeon:
                saved_wreckPoints += 5;
                sd.BeatTheAirDungeon = true;
                break;
            case AchievementType.BeatTheFinalLevel:
                saved_wreckPoints += 10;
                sd.BeatTheFinalLevel = true;
                break;
            case AchievementType.MTB:
                saved_wreckPoints += 8;
                sd.MTB = true;
                break;
            case AchievementType.Funny:
                saved_wreckPoints += 8;
                sd.Funny = true;
                break;
            case AchievementType.RUNNN:
                saved_wreckPoints += 8;
                sd.RUNNN = true;
                break;
            case AchievementType.LightItUp:
                saved_wreckPoints += 8;
                sd.LightItUp = true;
                break;
            case AchievementType.TheFuture:
                saved_wreckPoints += 8;
                sd.TheFuture = true;
                break;
        }

        PopulateSaveData(sd);
        FileManagment.WriteToSaveFile(m_currentProfile, sd.ToJson());
    }
    #endregion
}

public enum TransitionType
{
    None,
    Fade,
    Doors,
    BigDoor,
}

public enum AchievementType
{
    BeatTheTutorial,
    BeatTheCave,
    BeatTheAirDungeon,
    BeatTheFinalLevel,
    MTB,
    Funny,
    RUNNN,
    LightItUp,
    TheFuture
}
