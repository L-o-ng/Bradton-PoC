using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public MapObjects.StructureType structureType;
    public string url;
    public string keyword;
    public Vector3Int position;
}//data container for houses
