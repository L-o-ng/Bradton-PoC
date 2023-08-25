using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class InitMap : MonoBehaviour
{
    public TileBase tile;
    public Tilemap tileMap;
    public GameObject building;

    public static GameObject[,] map;
    public static List<Building> buildings = new();

    private void Awake()
    {
        map = new GameObject[100, 100]; //stores the data of the map for other scripts to reference
    }
    private void Start()
    {
        initNewMap();
    }

    private void initNewMap()
    {
        BuildMap.instance.placeBuilding("https://unibradfordac.sharepoint.com/sites/bradtonpoc", "Town Hall", MapObjects.StructureType.TownHall, 50, 50);
        BuildMap.instance.placeRoad(50, 49);
    }//places a town hall and road to build off of


    //private void GetMap()
    //{
    //    throw new NotImplementedException();
    //}

    //public static void loadMap(GameObject[,] loadedMap)
    //{
    //    map = loadedMap;
    //    for (int y = 0; y < 100; y++)
    //    {
    //        for (int x = 0; x < 100; x++)
    //        {
    //            if (map[y,x] != null)
    //            {
    //                switch (map[y,x].tag)
    //                {
    //                    case "GreenSpace":
    //                        BuildMap.instance.tileizeSpace(x, y);
    //                        break;
    //                    case "Road":
    //                        BuildMap.instance.placeRoad(x, y); 
    //                        break;
    //                    case "House":
    //                        BuildMap.instance.placeBuilding(map[y, x].GetComponent<Building>().url, map[y, x].GetComponent<Building>().structureType, x, y);
    //                        break;
    //                }
    //            }
    //        }
    //    }
    //}
}
