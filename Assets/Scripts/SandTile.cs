using Abstracts;
using DG.Tweening;
using Enums;
using UnityEngine;

public class SandTile : Tile
{
    public void AssignCandyType()
    {
        throw new System.NotImplementedException();
    }

    public override Tween ExplodingTile()
    {
        throw null;
    }

    public CandyType GetCandyType()
    {

        return CandyType.Empty;
    }


    Vector3 _mousePosition;
    Vector2 _startPos;
    private void OnMouseDown()
    {
        if (Camera.main != null) _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _startPos = _mousePosition;
        SandboxBoard.Instance.InstantiateSand(arrayPos);
    }






}