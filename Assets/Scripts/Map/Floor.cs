using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    /// <summary>
    /// Position of this floor at the map.
    /// </summary>
    public Vector2Int mapPos;
    public bool isGoalFloor = false;
    public IObject objOnFloor = null;
    public bool isPlayerOn = false;
    private bool isOnBefore = false;

    public Dictionary<Vector2, IObject> adjacentObject;

    [Header("Goal Floor Settings")]
    public Mesh goalFloorModel;
    public Mesh normalFloorModel;
    public Material[] goalFloorMats;
    public Material[] normalFloorMats;
    public Material goalActiveMat;
    public Material goalDisactiveMat;
    private MeshRenderer meshRenderer;

    public void RefreshGoal(bool changeModel = true)
    {
        if (changeModel)
        {
            GetComponent<MeshFilter>().mesh = isGoalFloor ? goalFloorModel : normalFloorModel;
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.materials = isGoalFloor ? goalFloorMats : normalFloorMats;
        }
        isOnBefore = !isPlayerOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGoalFloor && isPlayerOn != isOnBefore)
        {
            Material[] changed = goalFloorMats.Clone() as Material[];
            changed[1] = isPlayerOn ? goalActiveMat : goalDisactiveMat;
            meshRenderer.materials = changed;
            isOnBefore = isPlayerOn;
        }
    }
}
