using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    private const string HORIZONTAL_INPUT_AXIS_NAME = "Mouse X";
    private const string VERTICAL_INPUT_AXIS_NAME = "Mouse Y";
    
    public static PlayerController Instance;
    private Tile _selectedObject;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectObject(Tile matchObject)
    {
        _selectedObject = matchObject;
    }

    public void ReleaseSelectedObject()
    {
        _selectedObject = null;
    }
}
