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
    public Text leftLaneText, middleLaneText, rightLaneText;
    //Private Reference for use within the script to determine the selected hotkey
    private GameObject selectedHotkey;
    //Colours used to change the button to another colour to show that its highlighted. Could ((and will)) be replaced with a sprite swap later on
    public Color defaultColour;
    public Color selectedColour;

    //Default Colours 
    public Color leftLanesColour, rightLanesColour, middleLanesColour;
    
    void Start()
    {
        hotkeys.Add("LeftLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftLane", "Q")));
        hotkeys.Add("MiddleLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MiddleLane", "W")));
        hotkeys.Add("RightLane", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightLane", "E")));

        leftLaneText.text = hotkeys["LeftLane"].ToString();
        middleLaneText.text = hotkeys["MiddleLane"].ToString();
        rightLaneText.text = hotkeys["RightLane"].ToString();
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
        PlayerPrefs.SetString("leftLaneColour", ColorToHex(leftLanesColour));
        PlayerPrefs.SetString("rightLaneColour", ColorToHex(rightLanesColour));
        PlayerPrefs.SetString("middleLaneColour", ColorToHex(middleLanesColour));

        PlayerPrefs.Save();
    }
<<<<<<< Updated upstream:Disco Daimyo/Assets/Scripts/Keybinding.cs
}
=======

    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
}
>>>>>>> Stashed changes:Disco Daimyo/Assets/Scripts/Management/Keybinding.cs
