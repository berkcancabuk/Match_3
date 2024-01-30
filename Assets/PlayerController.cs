using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{



    private const string HORIZONTAL_INPUT_AXIS_NAME = "Mouse X";
    private const string VERTICAL_INPUT_AXIS_NAME = "Mouse Y";
    
    public static PlayerController Instance;
    //private GameObject _selectedObject;
    GameObject _selectedObject;

    private void Awake()
    {
        Instance = this;
    }


    Vector2 targetPos;
    Transform initialPosition;
    bool checkedTransform = false;

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            //Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            //if (targetObject)

            //{
                
            //    _selectedObject = targetObject.transform.gameObject;
            //    if (!checkedTransform)
            //    {
            //        initialPosition = _selectedObject.transform;
            //        checkedTransform = true;
            //    }
            //}
        }

        if (_selectedObject)
        {

           
            //Debug.Log(mousePosition.normalized);
            ////mousePosition.normalized;
            //// Clamp is needed
            //Vector2 position = _selectedObject.transform.position;
            //bool right = false;
            //bool up = false;

            //if (Mathf.Abs(initialPosition.position.x - mousePosition.x) > 0.2f)
            //{
            //   right = true;
            //    targetPos.x = 1;
            //}
            //else if (Mathf.Abs(initialPosition.position.x - mousePosition.x) > 0.2f)
            //{
            //    right = false;
            //    targetPos.x = -1;
            //}
            //if (Mathf.Abs(initialPosition.position.y - mousePosition.y) > 0.2f)
            //{
            //    up = true;
            //    targetPos.y = 1;
            //}
            //else
            //{
            //    up = false;
            //    targetPos.y = -1;
            //}
            

           
        }

        if (Input.GetMouseButtonUp(0) && _selectedObject)
        {
            _selectedObject = null;
            checkedTransform = false;
        }

    }



    public void SelectObject(Tile matchObject)
    {
        _selectedObject = matchObject.gameObject;
    }

    public void ReleaseSelectedObject()
    {
        _selectedObject = null;
    }
}
