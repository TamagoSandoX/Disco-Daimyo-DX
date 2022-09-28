using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom Colour 
//Jacob Nicholls-Smart

public class NoteColourSet : MonoBehaviour
{
    public Color objectsColour;
    // states based on whats required
    [SerializeField] private RequiredColour reqColour = RequiredColour.leftLane;
    private enum RequiredColour { leftLane, rightLane, middleLane };

    // Start is called before the first frame update
    void Start()
    {
        switch (reqColour)
        {
            case RequiredColour.leftLane:
                objectsColour = HexToColor(PlayerPrefs.GetString("leftLaneColour"));
                break;
            case RequiredColour.rightLane:
                objectsColour = HexToColor(PlayerPrefs.GetString("rightLaneColour"));
                break;
            case RequiredColour.middleLane:
                objectsColour = HexToColor(PlayerPrefs.GetString("middleLaneColour"));
                break;
            default:
                break;
        }

        // Set the colour of the note via init ALL should be RED for testing
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.EnableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", objectsColour);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    //void GetSavedColor()
    //{
    //    Color GetedColor = HexToColor(PlayerPrefs.GetString("SavedColor"));
    //}
    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    //

    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }
}
