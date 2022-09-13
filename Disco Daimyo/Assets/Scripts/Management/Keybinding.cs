// Jacob Nicholls-Smart 
// Keybinding Script using PlayerPrefs to Store the values within the project

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keybinding : MonoBehaviour
{
    private Dictionary<string, KeyCode> hotkeys = new Dictionary<string, KeyCode>();
    // Expandable with however many hotkeys need to be set
    public Text leftLaneText, middleLaneText, rightLaneText, navigateLeftText, navigateRightText, navigateUpText, navigateDownText, confirmSelectionText;
    //Private Reference for use within the script to determine the selected hotkey
    private GameObject selectedHotkey;
    //Colours used to change the button to another colour to show that its highlighted. Could ((and will)) be replaced with a sprite swap later on
    public Color defaultColour;
    public Color selectedColour;
    
    void Start()
    {
        // Ingame HotKeys
        hotkeys.Add("LeftLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftLane", "A")));
        hotkeys.Add("MiddleLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MiddleLane", "S")));
        hotkeys.Add("RightLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightLane", "D")));
        // Menu Related Hotkeys 
        hotkeys.Add("NavigateLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateLeft", "A")));
        hotkeys.Add("NavigateRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateRight", "D")));
        hotkeys.Add("NavigateUp", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateUp", "W")));
        hotkeys.Add("NavigateDown", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("NavigateDown", "S")));
        hotkeys.Add("ConfirmSelection", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ConfirmSelection", "Return")));

        // Setting the Text to show the values
        leftLaneText.text = hotkeys["LeftLane"].ToString();
        middleLaneText.text = hotkeys["MiddleLane"].ToString();
        rightLaneText.text = hotkeys["RightLane"].ToString();
        navigateLeftText.text = hotkeys["NavigateLeft"].ToString();
        navigateRightText.text = hotkeys["NavigateRight"].ToString();
        navigateUpText.text = hotkeys["NavigateUp"].ToString();
        navigateDownText.text = hotkeys["NavigateDown"].ToString();
        confirmSelectionText.text = hotkeys["ConfirmSelection"].ToString();
    }

    void OnGUI(){
        if (selectedHotkey != null){
            Event e = Event.current;
            if (e.isKey){
                hotkeys[selectedHotkey.name] = e.keyCode;
                selectedHotkey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                selectedHotkey.GetComponent<Image>().color = defaultColour;
                selectedHotkey = null;
            }
        }
    }

    public void ChangeHotkey(GameObject clicked){
        if (selectedHotkey != null){
            selectedHotkey.GetComponent<Image>().color = defaultColour;
        }

        selectedHotkey = clicked;
        selectedHotkey.GetComponent<Image>().color = selectedColour;
    }

    public void SaveHotkeys(){
        foreach (var hotkey in hotkeys)
        {
            PlayerPrefs.SetString(hotkey.Key, hotkey.Value.ToString());
        }
        PlayerPrefs.Save();
    }
}