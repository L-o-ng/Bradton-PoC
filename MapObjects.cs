using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapObjects : MonoBehaviour
{
    public static MapObjects instance;
    private void Awake()
    {
        instance = this;
    }

    public Sprite house;
    public Sprite hospital;
    public Sprite policeStation;
    public Sprite townHall;
    
    public enum StructureType
    {
        Road,
        Tileable,
        House,
        TownHall,
        PoliceStation,
        Hospital
    }
}
