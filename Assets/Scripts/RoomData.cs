using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    X,
    N,
    E,
    S,
    W,
    NE,
    NS,
    NW,
    ES,
    EW,
    SW,
    NES,
    NEW,
    NSW,
    ESW
}

public class RoomData : MonoBehaviour
{
    public Dictionary<RoomType, GameObject> prefabDict = new Dictionary<RoomType, GameObject>();
}
