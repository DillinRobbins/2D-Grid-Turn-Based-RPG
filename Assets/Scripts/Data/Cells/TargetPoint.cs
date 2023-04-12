using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [SerializeField] private Vector2 coord;

    public void Initialize(int x, int y)
    {
        coord = new Vector2(x, y);
    }

    public Vector2 GetPointCoordinate()
    {
        return coord;
    }
}
