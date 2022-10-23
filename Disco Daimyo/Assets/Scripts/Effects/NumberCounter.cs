using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{

    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    private int _value;
    public string NumberFormat = "N0";
    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }

    private Coroutine CountingCoroutine;

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(int newValue)
    {
        if(CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }
        StartCoroutine(CountText(newValue));
    }

    private IEnumerator CountText(int newValue)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _value;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));
        }

        if (previousValue < newValue)
        {
            while(previousValue < newValue)
            {
                previousValue += stepAmount;
                if( previousValue > newValue)
                {
                    previousValue = newValue;
                }
            }

            Text.SetText(previousValue.ToString(NumberFormat));

            yield return Wait;
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount;
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }
            }

            Text.SetText(previousValue.ToString(NumberFormat));

            yield return Wait;
        }
    }
}
