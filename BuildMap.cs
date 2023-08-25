using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildMap : MonoBehaviour
{
    public Grid grid;
    public Tilemap map;
    public TileBase road;
    public GameObject buildingPrefab;
    public GameObject greenSpace;
    public static List<SpriteRenderer> greenSpaces = new();

    private bool buildingSelected = false;
    private bool currentlyBuilding = false;
    public static bool buildMode = false;
    public static BuildMap instance;

    public TMP_Text buildModeText;
    public Image buildMenu;
    public TMP_InputField url;
    public TMP_InputField keyword;
    public TMP_Dropdown structureType;
    public Button placeButton;
    public Button cancelButton;

    private void Awake()
    {
        buildMenu.gameObject.SetActive(currentlyBuilding);
        buildModeText.gameObject.SetActive(buildMode);
        instance = this; //allows nonstatic public properties/methods to be accessed via a static instance of the class
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            buildMode = !buildMode;
            buildModeText.gameObject.SetActive(buildMode);
        }//toggles buildMode boolean

        if (Input.GetMouseButtonDown(0) && !buildMode)
        {
            Vector3Int pos = getCoords();
            if (InitMap.map[pos.y, pos.x] != null)
                try
                {
                    Application.OpenURL(InitMap.map[pos.y, pos.x].GetComponent<Building>().url);
                }
                catch { }
        }//opens link associated with the clicked building

        
        if (Input.GetMouseButtonDown(1) && !buildMode)
        {
            Vector3Int start = getCoords();

            if (!InitMap.map[start.y, start.x].CompareTag("House")) { return; }

            Building startBuilding = InitMap.map[start.y, start.x].GetComponent<Building>();
            LineRenderer lr = startBuilding.GetComponent<LineRenderer>();

            if (!buildingSelected)
            {
                buildingSelected = true;
                drawLines(startBuilding, lr);
            }
            else
            {
                lr.positionCount = 0;
                buildingSelected = false;
            }
        }//renders lines between houses with the same keyword on right click. Rightclick same house to turn off (somewhat buggy)
        

        //buildMode return gate
        if (!buildMode) { return; }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            enableGreenSpace();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            disableGreenSpace();
        }
        //holding left shift shows available build space

        if (Input.GetMouseButtonDown(0) && !currentlyBuilding)
        {
            placeRoad();
        }//places a road on left click

        if (Input.GetMouseButtonDown(1))
        {
            Vector3Int pos = getCoords();
            if (InitMap.map[pos.y, pos.x] != null)
            {
                if (InitMap.map[pos.y, pos.x].CompareTag("GreenSpace"))
                {
                    placeBuilding();
                }
            }
        }//opens build menu on right click
    }

    private void drawLines(Building startBuilding, LineRenderer lr)
    {
        int count = 0;
        for (int k = 0; k < InitMap.buildings.Count; k++)
        {
            if (InitMap.buildings[k].keyword == startBuilding.keyword)
            {
                count++;
            }
        }//how many buildings share the keyword?

        lr.positionCount = count * 2 + 1;
        //how many lines are necessary?

        lr.SetPosition(0, new Vector3(startBuilding.position.x + 0.5f, startBuilding.position.y + 0.5f, -0.1f));

        int i = 0;
        int j = 1;
        while (i < InitMap.buildings.Count)
        {
            if (InitMap.buildings[i].keyword == startBuilding.keyword)
            {
                lr.SetPosition(j, new Vector3(InitMap.buildings[i].position.x + 0.5f, InitMap.buildings[i].position.y + 0.5f, -0.1f));
                j++;
                lr.SetPosition(j, new Vector3(startBuilding.position.x + 0.5f, startBuilding.position.y + 0.5f, -0.1f));
                j++;
            }
            i++;
        }//populates array of points for line to render between
    }//logic for rendering lines between buildings which share keyword

    private void disableGreenSpace()
    {
        foreach (SpriteRenderer s in greenSpaces)
        {
            try
            {
                s.enabled = false;
            }
            catch { }
        }
    }
    private void enableGreenSpace()
    {
        foreach (SpriteRenderer s in greenSpaces)
        {
            try
            {
                s.enabled = true;
            }
            catch { }
        }
    }

    public void placeBuilding(string url, string keyword, MapObjects.StructureType structureType, int x, int y)
    {
        if (InitMap.map[y, x] != null)
            if (InitMap.map[y, x].CompareTag("GreenSpace"))
                Destroy(InitMap.map[y, x]);

        GameObject building = Instantiate(buildingPrefab, new Vector3(x + 0.5f, y + 0.5f, -0.1f), Quaternion.identity);

        building.GetComponent<Building>().structureType = structureType;
        building.GetComponent<Building>().url = url;
        building.GetComponent<Building>().position = new Vector3Int(x, y, 0);
        building.GetComponent<Building>().keyword = keyword;

        building.GetComponent<SpriteRenderer>().sprite = structureType switch
        {
            MapObjects.StructureType.House => MapObjects.instance.house,
            MapObjects.StructureType.Hospital => MapObjects.instance.hospital,
            MapObjects.StructureType.PoliceStation => MapObjects.instance.policeStation,
            MapObjects.StructureType.TownHall => MapObjects.instance.townHall,
            _ => null
        };

        InitMap.buildings.Add(building.GetComponent<Building>());
        
        InitMap.map[y, x] = building;

        tileizeArea(x, y);
    }
    private void placeBuilding()
    {
        currentlyBuilding = true;
        buildMenu.gameObject.SetActive(true);

        Vector3Int tempPos = getCoords();
        Vector3 pos = new(tempPos.x + 0.5f, tempPos.y + 0.5f, 0);

        Debug.Log("waiting for button press");
        

        placeButton.onClick.AddListener(delegate { placeBuildingHandler(pos); });
        cancelButton.onClick.AddListener(cancelPlace);
    }
    private void placeBuildingHandler(Vector3 pos)
    {
        string url = this.url.text;
        string dropdownStructureChoice = structureType.options[structureType.value].text;
        string keyword = this.keyword.text;

        MapObjects.StructureType type = dropdownStructureChoice switch
        {
            "House" => MapObjects.StructureType.House,
            "Hospital" => MapObjects.StructureType.Hospital,
            "Police Station" => MapObjects.StructureType.PoliceStation,
            "Town Hall" => MapObjects.StructureType.TownHall,
            _ => throw new Exception()
        };
        placeBuilding(url, keyword, type, (int)pos.x, (int)pos.y);
        cancelPlace();
    }   
    private void cancelPlace()
    {
        url.text = string.Empty;
        structureType.value = 0;
        buildMenu.gameObject.SetActive(false);
        cancelButton.onClick.RemoveAllListeners();
        placeButton.onClick.RemoveAllListeners();
        currentlyBuilding = false;
    }
    //logic for handling userinput via the menu to place a building. To hardcode a building placement call the top placeBuilding(,,,,)

    public void placeRoad()
    {
        Vector3Int pos = getCoords();

        if (InitMap.map[pos.y, pos.x] != null)
            if (InitMap.map[pos.y, pos.x].CompareTag("GreenSpace"))
            {
                Destroy(InitMap.map[pos.y, pos.x]);
                map.SetTile(pos, road);
                InitMap.map[pos.y, pos.x] = new GameObject("road");
                tileizeArea(pos.x, pos.y);
                enableGreenSpace();
                disableGreenSpace();
                if (Input.GetKey(KeyCode.LeftShift))
                    enableGreenSpace();

            }
    }
    public void placeRoad(int x, int y)
    {
        if (InitMap.map[y, x] != null)
            if (InitMap.map[y, x].CompareTag("GreenSpace"))
            {
                Destroy(InitMap.map[y, x]);
                map.SetTile(new Vector3Int(x, y, 0), road);
                InitMap.map[y, x] = new GameObject("road");
                tileizeArea(x, y);
            }
    }
    //Logic for placing roads. To hardcode road placement call the bottom placeRoad(,)

    public void tileizeSpace(int x, int y)
    {
        InitMap.map[y, x] = Instantiate(greenSpace, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
        greenSpaces.Add(InitMap.map[y, x].GetComponent<SpriteRenderer>());
    }
    public void tileizeArea(int x, int y)
    {
        if (InitMap.map[y + 1, x] == null)
        {
            InitMap.map[y + 1, x] = Instantiate(greenSpace, new Vector3(x + 0.5f, y + 1.5f, 0), Quaternion.identity);
            greenSpaces.Add(InitMap.map[y + 1, x].GetComponent<SpriteRenderer>());
        }
        if (InitMap.map[y - 1, x] == null)
        {
            InitMap.map[y - 1, x] = Instantiate(greenSpace, new Vector3(x + 0.5f, y - 0.5f, 0), Quaternion.identity);
            greenSpaces.Add(InitMap.map[y - 1, x].GetComponent<SpriteRenderer>());
        }
        if (InitMap.map[y, x + 1] == null)
        {
            InitMap.map[y, x + 1] = Instantiate(greenSpace, new Vector3(x + 1.5f, y + 0.5f, 0), Quaternion.identity);
            greenSpaces.Add(InitMap.map[y, x + 1].GetComponent<SpriteRenderer>());
        }
        if (InitMap.map[y, x - 1] == null)
        {
            InitMap.map[y, x - 1] = Instantiate(greenSpace, new Vector3(x - 0.5f, y + 0.5f, 0), Quaternion.identity);
            greenSpaces.Add(InitMap.map[y, x - 1].GetComponent<SpriteRenderer>());
        }
    }
    //adds greenspace on a tile (top) or around a tile (bottom)

    public Vector3Int getCoords()
    { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // get the collision point of the ray with the z = 0 plane
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        return grid.WorldToCell(worldPoint);
    }
    //returns the coordinates of the tile the mouse is hovering as a Vector3Int(x, y, 0)

    #region bad save script
    //if (Input.GetKeyDown(KeyCode.P))
    //{
    //    TileClass save = new();
    //    save.map = InitMap.map;
    //    string saveData = JsonUtility.ToJson(save);
    //    File.WriteAllText("C:\\Users\\dthomps7\\OneDrive - University of Bradford\\Bradton\\Bradton Unity PoC\\Assets\\savedMap.txt", saveData);
    //}
    //if (Input.GetKeyDown(KeyCode.O))
    //{
    //    string saveData = File.ReadAllText("C:\\Users\\dthomps7\\OneDrive - University of Bradford\\Bradton\\Bradton Unity PoC\\Assets\\savedMap.txt");
    //    TileClass loadedMap = JsonUtility.FromJson<TileClass>(saveData);
    //    InitMap.loadMap(loadedMap.map);
    //}
    #endregion
}
