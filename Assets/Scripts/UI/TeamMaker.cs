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
        if (inputField.text == "")
        {
            message.color = Color.red;
            message.text = "Enter a valid team name";
            message.enabled = true;
            return;
        }
        if (FileManagment.GetProfileFileNames().Contains(inputField.text))
        {
            message.color = Color.red;
            message.text = "Team Name Already Exists";
            message.enabled = true;
            return;
        }
        GameManager.Instance.ChangeCurrentProfile(inputField.text.ToLower());
        GameManager.Instance.SaveGameToCurrentProfile();
        GameManager.Instance.LevelManager.ResumePawnControl();
        inputField.text = "";
        message.enabled = false;
        gameObject.SetActive(false);
    }

    public void LoadOriginalTeam()
    {
        if (!GameManager.Instance.LoadGameProfile(inputField.text.ToLower()))
        { 
            message.color = Color.red;
            message.text = "Team Not Found";
            message.enabled = true;
            return;
        }

        GameManager.Instance.LevelManager.ResumePawnControl();
        inputField.text = "";
        message.enabled = false;
        gameObject.SetActive(false);
    }
}
