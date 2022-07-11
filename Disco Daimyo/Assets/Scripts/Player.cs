using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float minX = -4f;
    [SerializeField] private float maxX = 4f;
    public float move;

    private float initialYAxis;

    // Start is called before the first frame update
    void Start()
    {
        initialYAxis = transform.position.y;
    }

    // Update is called once per frame
    
    void Update()
    {
        move += Input.GetAxis("Mouse X");
        //var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        transform.position = new Vector3(Mathf.Clamp(move, minX, maxX), initialYAxis, 0f); //Mouse movement along x-axis and y-axis is fixed
    }
}
