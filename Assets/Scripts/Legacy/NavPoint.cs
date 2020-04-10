using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Legacy shit, forget about it
 * 
public class NavPoint : MonoBehaviour, INavPoint, INavNet
{
    [SerializeField] string originName = "NavPoint0";
    [SerializeField] float distance = 1f;
    [SerializeField] GameObject navPointPrefab;
    [SerializeField] public Transform parent;
    [SerializeField] public static NavPoint staticRef;


    [SerializeField] Material waypointMaterial;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material overlapMaterial;

    [SerializeField] int radius = 20;

    private static int totalAmountOfNavPoints;

    [SerializeField] public NavPoint[] neighbours ;
    public static List<NavPoint> allNavPoints = new List<NavPoint>();
    private static Vector3[] relativeNavPointPositions = new Vector3[6];

    public bool overlapWithCollider = false;

    static Dictionary<NavPoint, NavPoint> pathDictionaryValueIsOrigin;
    static List<NavPoint> processedNavPoints;

    [SerializeField] private int distanceFromCentre = 0;
    private void Start()
    {
        if(staticRef == null)
        {
            staticRef = this;
            neighbours = new NavPoint[6];
            this.gameObject.name = originName;
        }
        if (allNavPoints.Count == 0)
        {
            allNavPoints.Add(this);
        }
        SetRelativePositionVectors();
    }
    private void SetRelativePositionVectors()
    {
        float sqrtThree = Mathf.Sqrt(3f);
        relativeNavPointPositions[0] = new Vector3(-distance, 0, 0);
        relativeNavPointPositions[1] = new Vector3(-distance / 2, 0, sqrtThree * distance / 2);
        relativeNavPointPositions[2] = new Vector3(distance / 2, 0, sqrtThree * distance / 2);
        relativeNavPointPositions[3] = new Vector3(distance, 0, 0);
        relativeNavPointPositions[4] = new Vector3(distance / 2, 0, -sqrtThree * distance / 2);
        relativeNavPointPositions[5] = new Vector3(-distance / 2, 0, -sqrtThree * distance / 2);
    }
    void Update()
    {
        //Processing input for test purposes
        if( staticRef == this)
        {
            if (Input.GetKeyDown(KeyCode.O) && allNavPoints.Count == 1)
            {
                CreateNavNet();
                CheckIfNavpointsOverlapWithColliders();
                TestFindAndShowPath.staticRef.keepFindingPath = true;
                //destination = TestFindAndShowPath.staticRef.FindNearestNavpoint();  
                Debug.Log("NavPointsCount is " + allNavPoints.Count);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                //FindPath(start, destination);
            }
        }
    }

    public void CheckIfNavpointsOverlapWithColliders()
    {
        foreach( NavPoint npoint in allNavPoints)
        {
            if (!overlapWithCollider)
            {
                for (int direction = 0; direction < 6; direction++)
                {
                    if (npoint.neighbours[direction] != null && 
                        Physics.Raycast(npoint.transform.position, relativeNavPointPositions[direction], out RaycastHit forgetAboutIT, staticRef.distance))
                    {
                        npoint.neighbours[direction].overlapWithCollider = true;
                        npoint.neighbours[direction].GetComponent<MeshRenderer>().material = staticRef.overlapMaterial;
                    }
                }
            }
        }
    }
    public List<INavPoint> FindPath(INavPoint start, INavPoint destination)
    {
        List<INavPoint> path = new List<INavPoint>();
        bool isPathFound = false;
        int processedNavPointID = 0;
        NavPoint nPoint;
        pathDictionaryValueIsOrigin = new Dictionary<NavPoint, NavPoint>();
        processedNavPoints = new List<NavPoint>();

        pathDictionaryValueIsOrigin.Add(start as NavPoint, start as NavPoint);   
        processedNavPoints.Add(start as NavPoint);
        while (!isPathFound && processedNavPointID < processedNavPoints.Count)
        {
            nPoint = processedNavPoints[processedNavPointID];
            for (int direction = 0; direction < 6; direction++)
            {
                if (nPoint.neighbours[direction] != null && !nPoint.neighbours[direction].overlapWithCollider)
                {
                    if (nPoint.neighbours[direction] == destination as NavPoint)
                    {
                        isPathFound = true;
                    }
                    if (pathDictionaryValueIsOrigin.TryGetValue(nPoint.neighbours[direction], out NavPoint forgetAboutIt) == false)
                    {
                        processedNavPoints.Add(nPoint.neighbours[direction]);
                        pathDictionaryValueIsOrigin.Add(nPoint.neighbours[direction], nPoint);
                    }
                }
            }
            processedNavPointID++;
        }
        if (isPathFound)
        {
            bool finished = false;
            NavPoint currentlyProcessedNavPoint = null;
            pathDictionaryValueIsOrigin.TryGetValue(destination as NavPoint, out currentlyProcessedNavPoint);
            while (!finished)
            {
                path.Add(currentlyProcessedNavPoint);
                currentlyProcessedNavPoint.GetComponent<MeshRenderer>().material = staticRef.waypointMaterial;
                if (currentlyProcessedNavPoint == start as NavPoint)
                {
                    finished = true;
                }
                pathDictionaryValueIsOrigin.TryGetValue(currentlyProcessedNavPoint, out currentlyProcessedNavPoint);
            }
            LineRenderingThing.staticRef.DrawPath(path);
            MarkThePath_OnlyForTesting______PLEASE_DELETE_THIS(start as NavPoint, destination as NavPoint);
        }
        return path;
    }
    private void CreateNavNet()
    {
        NavPoint proccessedNavPoint = this;
        proccessedNavPoint = allNavPoints[0];
        int proccessedNavPointId = 0;
        
        while ( proccessedNavPointId < allNavPoints.Count)
        {
            proccessedNavPoint = allNavPoints[proccessedNavPointId];
            proccessedNavPoint.CheckForExistingNeighbours();
            proccessedNavPoint.CreateNavPointsAround();
            proccessedNavPointId++; 
        }
    }
    private void CheckForExistingNeighbours()
    {
        for ( int direction = 0; direction < 6; direction++)
        {
            if (neighbours[direction] != null)
            {
                neighbours[direction].CheckForCommonNeighbours(direction, this);
            }
        }    
    }

    private void CheckForCommonNeighbours(int direction, NavPoint callerNavPoint)
    {
        NavPoint commonNeighbour = null;
        if (neighbours[(direction + 2) % 6] != null)
        {
            commonNeighbour = neighbours[(direction + 2) % 6];
            commonNeighbour.neighbours[(direction + 4) % 6] = callerNavPoint;
            callerNavPoint.neighbours[(direction + 1) % 6] = commonNeighbour;
        }
        if( neighbours[(direction + 4) % 6] != null)
        {
            commonNeighbour = neighbours[(direction + 4) % 6];
            commonNeighbour.neighbours[(direction + 2) % 6] = callerNavPoint;
            callerNavPoint.neighbours[(direction + 5) % 6] = commonNeighbour;
        }
    }
    private void CreateNavPointsAround()
    {
        distanceFromCentre++;
        if (distanceFromCentre <= staticRef.radius)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                if (neighbours[direction] == null)
                {
                    NavPoint newNavPoint = GameObject.Instantiate(staticRef.navPointPrefab.gameObject).AddComponent<NavPoint>();
                    newNavPoint.neighbours = new NavPoint[6];
                    this.neighbours[direction] = newNavPoint;
                    this.neighbours[direction].transform.position = this.transform.position + relativeNavPointPositions[direction];
                    newNavPoint.distanceFromCentre = this.distanceFromCentre;
                    this.neighbours[direction].transform.parent = staticRef.parent;
                    newNavPoint.neighbours[(direction + 3) % 6] = this;
                    newNavPoint.gameObject.name = this.gameObject.name + direction.ToString();
                    allNavPoints.Add(neighbours[direction]);
                    // TESTING PATHFINDING, eventually please remove that
                    //SetStartAndDestinationForTestingPurposes___PLEASE_DELETE_THIS(newNavPoint);
                }
            }
        }
    }

public static void ResetMesh_Test_PLEASE_DELETE_THIS()
    {
        foreach (NavPoint point in allNavPoints)
        {
            if (!point.overlapWithCollider)
            {
                point.GetComponent<MeshRenderer>().material = NavPoint.staticRef.defaultMaterial;
            }
        }
    }
    private void MarkThePath_OnlyForTesting______PLEASE_DELETE_THIS(NavPoint start, NavPoint destination)
    {
        NavPoint currentlyProcessedNavPoint = null;
        pathDictionaryValueIsOrigin.TryGetValue(destination, out currentlyProcessedNavPoint);
        while (true)
        {
                currentlyProcessedNavPoint.GetComponent<MeshRenderer>().material = staticRef.waypointMaterial;
            if (currentlyProcessedNavPoint == start)
            {
                return;
            }
            pathDictionaryValueIsOrigin.TryGetValue(currentlyProcessedNavPoint, out currentlyProcessedNavPoint);
        }
    }

    public Vector3 GetPosition()
    {
        Debug.Log("przekazuję pozycję");
        return this.transform.position;
    }
    public bool OverlapWithCollider()
    {
        return overlapWithCollider;
    }
}*/
