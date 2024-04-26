using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using System.Linq;
using Yarn.Unity;
using Mono.Cecil.Cil;
using System;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(menuName = "Data/PawnEventData")]
// Restricted to tinker n Ashe only
public class PawnEventData : ScriptableObject
{
    [SerializeField] private PawnEvent[] pawnEvents;
    [SerializeField] private SkipEvent skipEvent;
    [SerializeField] private bool hideDialogueAfterEvent;
    [SerializeField] private bool skippable;
    // Skipping Behvaiour Needs to be implemented here
    public PawnEvent[] PawnEvents => pawnEvents;
    public SkipEvent SkipEvent => skipEvent;
    public bool HideDialogueAfterEvent => hideDialogueAfterEvent;
    public bool Skippable => skippable;
}

[System.Serializable]
public struct PawnEvent
{
    [Header("Level General")] // More Efficent in a new editor
    [SerializeField] private string pawnEventTitle;
    [SerializeField] private bool notCinematic;
    [SerializeField] private bool pausePawnControl;
    [SerializeField] private bool resumePawnControl;
    [SerializeField] private float transitionTime;

    [Header("Pawn Specific")]
    [SerializeField] private PawnSelection pawnSelection;
    [SerializeField] private EventAction eventAction;
    [Header("Movement Event")]
    [SerializeField] private Direction moveDirection;
    [SerializeField, Range(0.2f, 1f)] private float moveSpeed;
    [SerializeField] private float timeDuration;

    [Header("Jump Event")]
    [SerializeField] private float jumpForce;

    [Header("Dialogue")]
    [SerializeField] private bool activeDialogueAtTime;
    [SerializeField, Tooltip("Transitions till dialogue finished as apposed to Transition Time")] private bool waitOnDialogue;
    [SerializeField] private Dialogue[] dialogues; // Cleaner than using a DialogueObject seperatly

    [Header("Invoke Event")]
    [SerializeField] private bool invoke;
    [SerializeField] private bool activate;
    [SerializeField] private int id;

    // Properties
    public string Title => pawnEventTitle;
    public bool NotCinematic => notCinematic;
    public bool PausePawnControl => pausePawnControl;
    public bool ResumePawnControl => resumePawnControl;
    public PawnSelection PawnSelection => pawnSelection;
    public EventAction EventAction => eventAction;
    public float TimeDuration => timeDuration;
    public Direction MoveDirection => moveDirection;
    public float MoveSpeed => moveSpeed;
    public float Delay => transitionTime;
    public float JumpForce => jumpForce;  
    public bool ActiveDialogueAtTime => activeDialogueAtTime;
    public bool WaitOnDialogue => waitOnDialogue;
    public Dialogue[] Dialogues => dialogues;
    public bool Invoke => invoke;
    public bool Activate => activate;
    public int Id => id;    
}

[System.Serializable]
public struct SkipEvent
{
    [Header("General")]
    [SerializeField] private bool changeMusic;
    [SerializeField] private bool changeCameraPosition;
    [SerializeField] private bool hideDialogue;
    [SerializeField] private bool skipWithFade;
    [SerializeField] private AudioClip musicSelection;
    [SerializeField] private Vector3 cameraNewLocation;
    
    [Header("Pawn Specific")]
    [SerializeField] private bool pausePawnControl;
    [SerializeField] private bool notCinematic;
    [SerializeField] private bool resumePawnControl;
    [SerializeField] private bool changeTinkerLocation;
    [SerializeField] private bool changeAsheLocation;
    [SerializeField] private Vector3 tinkerNewLocation;
    [SerializeField] private Vector3 asheNewLocation;

    [Header("Invoke Events")]
    [SerializeField] private bool invoke;
    [SerializeField] private bool activate;
    [SerializeField] private int id;


    // Properties
    public bool ChangeMusic => changeMusic;
    public bool ChangeCameraPosition => changeCameraPosition;
    public bool HideDialogue => hideDialogue;
    public bool SkipWithFade => skipWithFade;
    public AudioClip MusicSelection => musicSelection;
    public Vector3 CameraNewLocation => cameraNewLocation;
    public bool PausePawnControl => pausePawnControl;
    public bool NotCinematic => notCinematic;
    public bool ResumePawnControl => resumePawnControl;
    public bool ChangeTinkerLocation => changeAsheLocation;
    public bool ChangeAsheLocation => changeAsheLocation;
    public Vector3 TinkerNewLocation => tinkerNewLocation;
    public Vector3 AsheNewLocation => asheNewLocation;
    public bool Invoke => invoke;
    public bool Activate => activate;
    public int Id => id;
}

public enum PawnSelection
{
    Tinker,
    Ashe
}

public enum EventAction
{
    None,
    Move,
    Jump,
    Punch,
    Shoot,
    Grab,
    MoveTo
}

public enum Direction
{
    Right,
    Left,
}

#if UNITY_EDITOR
[CustomEditor(typeof(PawnEventData)), CanEditMultipleObjects]
public class PawnEventDataEditor : Editor
{
    SerializedProperty pawnEventArray;
    int currPawnEventIdx;
    private bool areYouSure = false;
    private bool displaySkipEvent = false;
    private Vector2 eventScroll;
    private List<string> m_eventNames;
    private void OnEnable()
    {
        pawnEventArray = serializedObject.FindProperty("pawnEvents");
        m_eventNames = new List<string>();
        for (int i = 0; i < pawnEventArray.arraySize; i++)
        {
            m_eventNames.Add(pawnEventArray.GetArrayElementAtIndex(i).FindPropertyRelative("pawnEventTitle").stringValue);
        }
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.LabelField("Pawn Event Data");
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Events: " + pawnEventArray.arraySize, EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var skippableButton = serializedObject.FindProperty("skippable");
        skippableButton.boolValue = EditorGUILayout.Toggle("Skippable", skippableButton.boolValue);
        if (skippableButton.boolValue)
        {
            displaySkipEvent = EditorGUILayout.Toggle("Display Skip Event", displaySkipEvent);
        }
        // Interface Each PawnEvent by Title
        EditorGUILayout.Space();
        EventTitleInterfacer();
        EditorGUILayout.Space();
        PawnEventAdjustment();
        EditorGUILayout.Space();

        // Interfacing TOP
        HorizontalPagingInterfacer();
        EditorGUILayout.Separator();

        if (displaySkipEvent)
        {
            SkipEventSpace();
        }
        else if (pawnEventArray.arraySize > 0)
        {
            SerializedProperty selectedElement = pawnEventArray.GetArrayElementAtIndex(currPawnEventIdx);
            //selectedElement.isExpanded = true;
            //EditorGUILayout.PropertyField(selectedElement, true);
            LevelGeneralSpace(selectedElement);
            EditorGUILayout.Space(20f);
            PawnControlSpace(selectedElement);
            EditorGUILayout.Space(20f);
            DialogueSpace(selectedElement);
            EditorGUILayout.Space(20f);
            InvokeGameEventSpace(selectedElement);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Currently no Pawn Events, Add a new Pawn Event to Begin!", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
        // Interfacing Bot
        EditorGUILayout.Separator();
        HorizontalPagingInterfacer();

        EditorGUILayout.Space();

        PawnEventAdjustment();
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }

    // So that we can just have our struct initializes with wanted defaults
    private void InitializePawnEvent(SerializedProperty pawnEventProperty)
    {
        // Level General
        pawnEventProperty.FindPropertyRelative("pawnEventTitle").stringValue = "New Event " + pawnEventArray.arraySize;
        pawnEventProperty.FindPropertyRelative("pausePawnControl").boolValue = false;
        pawnEventProperty.FindPropertyRelative("resumePawnControl").boolValue = false;
        // Pawn Specific
        pawnEventProperty.FindPropertyRelative("pawnSelection").enumValueIndex = 0;
        pawnEventProperty.FindPropertyRelative("eventAction").enumValueIndex = 0;
        // Movement Event
        pawnEventProperty.FindPropertyRelative("timeDuration").floatValue = 0;
        pawnEventProperty.FindPropertyRelative("moveDirection").enumValueIndex = 0;
        pawnEventProperty.FindPropertyRelative("moveSpeed").floatValue = 0.2f;
        pawnEventProperty.FindPropertyRelative("transitionTime").floatValue = 0;
        // Jump Event
        pawnEventProperty.FindPropertyRelative("jumpForce").floatValue = 0;
        // Dialogue
        pawnEventProperty.FindPropertyRelative("activeDialogueAtTime").boolValue = false;
        pawnEventProperty.FindPropertyRelative("waitOnDialogue").boolValue = false;
        pawnEventProperty.FindPropertyRelative("dialogues").arraySize = 0;
        // Invoke Event
        pawnEventProperty.FindPropertyRelative("invoke").boolValue = false;
        pawnEventProperty.FindPropertyRelative("activate").boolValue = false;
        pawnEventProperty.FindPropertyRelative("id").intValue = -1;
    }

    private void PawnEventAdjustment()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+ New Pawn Event To End", GUILayout.MaxHeight(20f), GUILayout.ExpandWidth(true)))
        {
            currPawnEventIdx = pawnEventArray.arraySize++;
            SerializedProperty newElementProperty = pawnEventArray.GetArrayElementAtIndex(pawnEventArray.arraySize - 1);
            InitializePawnEvent(newElementProperty);
            m_eventNames.Insert(currPawnEventIdx, newElementProperty.FindPropertyRelative("pawnEventTitle").stringValue);
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+ New Pawn Event Before Curr ", GUILayout.MaxHeight(20f), GUILayout.ExpandWidth(true)))
        {
            pawnEventArray.InsertArrayElementAtIndex(currPawnEventIdx);
            SerializedProperty newElementProperty = pawnEventArray.GetArrayElementAtIndex(currPawnEventIdx);
            InitializePawnEvent(newElementProperty);
            m_eventNames.Insert(currPawnEventIdx, newElementProperty.FindPropertyRelative("pawnEventTitle").stringValue);
        }
        GUILayout.FlexibleSpace();
        if (!areYouSure)
        {
            EditorGUI.BeginDisabledGroup(pawnEventArray.arraySize <= 0);
            areYouSure = GUILayout.Button("- Delete Pawn Event " + currPawnEventIdx, GUILayout.MaxHeight(20f), GUILayout.ExpandWidth(true));
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.LabelField("Are you sure? ", GUILayout.MaxWidth(80f), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Yes", GUILayout.MaxHeight(20f), GUILayout.ExpandWidth(true)))
            {
                pawnEventArray.DeleteArrayElementAtIndex(currPawnEventIdx);
                currPawnEventIdx--;
                m_eventNames.RemoveAt(currPawnEventIdx);
                areYouSure = false;
            }
            else if (GUILayout.Button("No", GUILayout.MaxHeight(20f), GUILayout.ExpandWidth(true)))
            {
                areYouSure = false;
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void HorizontalPagingInterfacer()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        EditorGUI.BeginDisabledGroup(currPawnEventIdx <= 0);
        if (GUILayout.Button("<<<=", EditorStyles.miniButtonRight, GUILayout.MaxWidth(50f))) currPawnEventIdx = 0;
        if (GUILayout.Button("<<", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(50f))) currPawnEventIdx--;
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(20);
        bool skippable = serializedObject.FindProperty("skippable").boolValue;
        int[] optionValues = Enumerable.Range(0, pawnEventArray.arraySize).ToArray();
        string[] displayedOptions = new string[pawnEventArray.arraySize];
        for (int i = 0; i < optionValues.Length; i++)
        {
            displayedOptions[i] = optionValues[i] + " : " + m_eventNames[i];
        }
        
        currPawnEventIdx = EditorGUILayout.IntPopup(currPawnEventIdx, displayedOptions, optionValues);
        //currPawnEventIdx = EditorGUILayout.IntField(currPawnEventIdx, GUILayout.MaxWidth(50f));

        GUILayout.Space(20);

        EditorGUI.BeginDisabledGroup(currPawnEventIdx >= pawnEventArray.arraySize - 1);
        if (GUILayout.Button(">>", EditorStyles.miniButtonRight, GUILayout.MaxWidth(50f))) currPawnEventIdx++;
        if (GUILayout.Button("=>>>", EditorStyles.miniButtonRight, GUILayout.MaxWidth(50f))) currPawnEventIdx = pawnEventArray.arraySize - 1;
        EditorGUI.EndDisabledGroup();
        // Bound the index
        currPawnEventIdx = pawnEventArray.arraySize > 0
            ? (currPawnEventIdx % pawnEventArray.arraySize + pawnEventArray.arraySize) % pawnEventArray.arraySize : 0;

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void EventTitleInterfacer()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Title Interfacer Coming Soon");
        //for (int i = 0; i < pawnEventArray.arraySize; i++)
        //{
        //    EditorGUILayout.SelectableLabel(pawnEventArray.GetArrayElementAtIndex(i).FindPropertyRelative("pawnEventTitle").stringValue);
        //}
        EditorGUILayout.EndHorizontal();
    }

    // Details of the Pawn Event Data
    private void LevelGeneralSpace(SerializedProperty pawnEvent)
    {
        EditorGUILayout.LabelField("Level General", EditorStyles.boldLabel);
        var PawnEventTitle = pawnEvent.FindPropertyRelative("pawnEventTitle");
        var TransitionTime = pawnEvent.FindPropertyRelative("transitionTime");
        PawnEventTitle.stringValue = EditorGUILayout.TextField("Pawn Event Title", PawnEventTitle.stringValue);
        m_eventNames[currPawnEventIdx] = PawnEventTitle.stringValue;
        TransitionTime.floatValue = EditorGUILayout.FloatField("Transition Time", TransitionTime.floatValue);
        var PausePC = pawnEvent.FindPropertyRelative("pausePawnControl");
        var ResumePC = pawnEvent.FindPropertyRelative("resumePawnControl");
        var NotCinem = pawnEvent.FindPropertyRelative("notCinematic");

        EditorGUI.BeginChangeCheck();
        bool pause = EditorGUILayout.Toggle("Pause Pawn Control", PausePC.boolValue);
        bool resume = EditorGUILayout.Toggle("Resume Pawn Control", ResumePC.boolValue);
        if (EditorGUI.EndChangeCheck())
        {
            if (pause && !PausePC.boolValue)
            {
                PausePC.boolValue = true;
                ResumePC.boolValue = false;
            }
            else if (resume && !ResumePC.boolValue)
            {
                ResumePC.boolValue = true;
                PausePC.boolValue = false;
            }
            else
            {
                ResumePC.boolValue = false;
                PausePC.boolValue = false;
            }
        }
        if (PausePC.boolValue)
        {
            EditorGUI.indentLevel++;
            NotCinem.boolValue = EditorGUILayout.Toggle("Not Cinematic", NotCinem.boolValue);
            EditorGUI.indentLevel--;
        }
    }

    private void PawnControlSpace(SerializedProperty pawnEvent)
    {
        EditorGUILayout.LabelField("Pawn Control", EditorStyles.boldLabel);
        // Display a dropdown for an enum Pawn
        var pawnSelection = pawnEvent.FindPropertyRelative("pawnSelection");
        var EventAction = pawnEvent.FindPropertyRelative("eventAction");
        pawnSelection.enumValueIndex = EditorGUILayout.Popup("Pawn", pawnSelection.enumValueIndex, pawnSelection.enumDisplayNames);
        EventAction.enumValueIndex = EditorGUILayout.Popup("Action", EventAction.enumValueIndex, EventAction.enumDisplayNames);

        // Select The Action to display
        EditorGUI.indentLevel++;
        switch ((EventAction)EventAction.enumValueIndex) 
        {
            case global::EventAction.Move:
                MovementActionSpace(pawnEvent);
                break;
            case global::EventAction.Jump:
                JumpActionSpace(pawnEvent);
                break;
            default: 
                break;
        }
        EditorGUI.indentLevel--;
    }
    private void DialogueSpace(SerializedProperty pawnEvent)
    {
        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        var DialougeActive = pawnEvent.FindPropertyRelative("activeDialogueAtTime");   
        DialougeActive.boolValue = EditorGUILayout.Toggle("Dialogue Active", DialougeActive.boolValue);

        if (DialougeActive.boolValue)
        {
            var WaitOnDialogue = pawnEvent.FindPropertyRelative("waitOnDialogue");
            WaitOnDialogue.boolValue = EditorGUILayout.Toggle("Wait On Dialogue", WaitOnDialogue.boolValue);
            EditorGUILayout.PropertyField(pawnEvent.FindPropertyRelative("dialogues"), true);
        }
    }

    private void InvokeGameEventSpace(SerializedProperty pawnEvent)
    {
        EditorGUILayout.LabelField("Invoke Game Event", EditorStyles.boldLabel);
        var id = pawnEvent.FindPropertyRelative("id");
        var Invoke = pawnEvent.FindPropertyRelative("invoke");
        var Activate = pawnEvent.FindPropertyRelative("activate");

        id.intValue = EditorGUILayout.IntField("ID", id.intValue);
        if (id.intValue >= 0)
        {
            Invoke.boolValue = EditorGUILayout.Toggle("Invoke", Invoke.boolValue);
            Activate.boolValue = EditorGUILayout.Toggle("Activate", Activate.boolValue);
        } 
        else
        {
            Invoke.boolValue = false;
            Activate.boolValue = false;
        }
    }

    #region Pawn Actions
    private void MovementActionSpace(SerializedProperty pawnEvent)
    {
        //EditorGUILayout.LabelField("Movement Action", EditorStyles.boldLabel);
        var Direction = pawnEvent.FindPropertyRelative("moveDirection");
        Direction.enumValueIndex = EditorGUILayout.Popup("Direction", Direction.enumValueIndex, Direction.enumDisplayNames);
        var Speed = pawnEvent.FindPropertyRelative("moveSpeed");
        Speed.floatValue = EditorGUILayout.Slider("Speed", Speed.floatValue, 0.2f, 1f);
        var TimeDuration = pawnEvent.FindPropertyRelative("timeDuration");
        TimeDuration.floatValue = EditorGUILayout.FloatField("Duration", TimeDuration.floatValue);
    }
    private void JumpActionSpace(SerializedProperty pawnEvent)
    {
        //EditorGUILayout.LabelField("Jump Action", EditorStyles.boldLabel);
        var JumpForce = pawnEvent.FindPropertyRelative("jumpForce");
        JumpForce.floatValue = EditorGUILayout.FloatField("Jump Force", JumpForce.floatValue);
    }
    #endregion

    private void SkipEventSpace()
    {
        SerializedProperty skipEvent = serializedObject.FindProperty("skipEvent");
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        var skipWithFade = skipEvent.FindPropertyRelative("skipWithFade");
        skipWithFade.boolValue = EditorGUILayout.Toggle("Fade on Skip", skipWithFade.boolValue);

        var changeMusic = skipEvent.FindPropertyRelative("changeMusic");
        changeMusic.boolValue = EditorGUILayout.Toggle("Change Music", changeMusic.boolValue);
        if (changeMusic.boolValue)
        {
            EditorGUI.indentLevel++;
            var musicSelection = skipEvent.FindPropertyRelative("musicSelection");
            musicSelection.objectReferenceValue = EditorGUILayout.ObjectField(musicSelection.objectReferenceValue, typeof(AudioClip), true);
            EditorGUI.indentLevel--;
        }

        var changeCameraPosition = skipEvent.FindPropertyRelative("changeCameraPosition");
        changeCameraPosition.boolValue = EditorGUILayout.Toggle("Change Camera Position", changeCameraPosition.boolValue);
        if (changeCameraPosition.boolValue)
        {
            EditorGUI.indentLevel++;
            var cameraNewLocation = skipEvent.FindPropertyRelative("cameraNewLocation");
            EditorGUILayout.BeginHorizontal();
            float x = EditorGUILayout.FloatField(cameraNewLocation.vector3Value.x);
            float y = EditorGUILayout.FloatField(cameraNewLocation.vector3Value.y);
            float z = EditorGUILayout.FloatField(cameraNewLocation.vector3Value.z);
            EditorGUILayout.EndHorizontal();
            cameraNewLocation.vector3Value = new Vector3(x, y, z);
            EditorGUI.indentLevel--;
        }

        var hideDialogue = skipEvent.FindPropertyRelative("hideDialogue");
        hideDialogue.boolValue = EditorGUILayout.Toggle("Hide Dialouge", hideDialogue.boolValue);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Pawn Specific", EditorStyles.boldLabel);

        var PausePC = skipEvent.FindPropertyRelative("pausePawnControl");
        var ResumePC = skipEvent.FindPropertyRelative("resumePawnControl");
        var NotCinem = skipEvent.FindPropertyRelative("notCinematic");

        EditorGUI.BeginChangeCheck();
        bool pause = EditorGUILayout.Toggle("Pause Pawn Control", PausePC.boolValue);
        bool resume = EditorGUILayout.Toggle("Resume Pawn Control", ResumePC.boolValue);
        if (EditorGUI.EndChangeCheck())
        {
            if (pause && !PausePC.boolValue)
            {
                PausePC.boolValue = true;
                ResumePC.boolValue = false;
            }
            else if (resume && !ResumePC.boolValue)
            {
                ResumePC.boolValue = true;
                PausePC.boolValue = false;
            }
            else
            {
                ResumePC.boolValue = false;
                PausePC.boolValue = false;
            }
        }
        if (PausePC.boolValue)
        {
            EditorGUI.indentLevel++;
            NotCinem.boolValue = EditorGUILayout.Toggle("Not Cinematic", NotCinem.boolValue);
            EditorGUI.indentLevel--;
        }

        var changeTinkerLocation = skipEvent.FindPropertyRelative("changeTinkerLocation");
        changeTinkerLocation.boolValue = EditorGUILayout.Toggle("Change Tinker Location", changeTinkerLocation.boolValue);
        if (changeTinkerLocation.boolValue)
        {
            EditorGUI.indentLevel++;
            var tinkerNewLocation = skipEvent.FindPropertyRelative("tinkerNewLocation");
            EditorGUILayout.BeginHorizontal();
            float x = EditorGUILayout.FloatField(tinkerNewLocation.vector3Value.x);
            float y = EditorGUILayout.FloatField(tinkerNewLocation.vector3Value.y);
            float z = EditorGUILayout.FloatField(tinkerNewLocation.vector3Value.z);
            EditorGUILayout.EndHorizontal();
            tinkerNewLocation.vector3Value = new Vector3(x,y,z);
            EditorGUI.indentLevel--;
        }

        var changeAsheLocation = skipEvent.FindPropertyRelative("changeAsheLocation");
        changeAsheLocation.boolValue = EditorGUILayout.Toggle("Change Ashe Location", changeAsheLocation.boolValue);
        if (changeAsheLocation.boolValue)
        {
            EditorGUI.indentLevel++;
            var asheNewLocation = skipEvent.FindPropertyRelative("asheNewLocation");
            EditorGUILayout.BeginHorizontal();
            float x = EditorGUILayout.FloatField(asheNewLocation.vector3Value.x);
            float y = EditorGUILayout.FloatField(asheNewLocation.vector3Value.y);
            float z = EditorGUILayout.FloatField(asheNewLocation.vector3Value.z);
            EditorGUILayout.EndHorizontal();
            asheNewLocation.vector3Value = new Vector3(x, y, z);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
        InvokeGameEventSpace(skipEvent);

    }
}
#endif
