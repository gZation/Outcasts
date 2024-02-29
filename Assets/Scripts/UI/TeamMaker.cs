using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamMaker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TMP_InputField inputField;
    public void CreateNewTeam()
    {
        GameManager.Instance.ChangeCurrentProfile(inputField.text.ToLower());
        GameManager.Instance.SaveGameToCurrentProfile();
    }

    public void LoadOriginalTeam()
    {
        if (!GameManager.Instance.LoadGameProfile(inputField.text.ToLower()))
        { 
            message.color = Color.red;
            message.text = "Team Not Found";
            message.enabled = true;
        }
    }
}
