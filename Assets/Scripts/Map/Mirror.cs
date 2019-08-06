using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : Wall, IBulletInteractor, IBreakable
{
    [Space(15)]
    public GameObject scatteredMirror;

    public void Break()
    {
        Instantiate(scatteredMirror, transform.position, transform.rotation);
        MapManager.inst.currentMap.RemoveWall(this.mapPos);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            //Debug.Log("ldPos: " + ldPos + ", rdPos: " + rdPos + ", dir: " + dir);
            // Make reflected objects
            StartCoroutine(CopyObjects(PlayerController.inst.currentPlayer));
        }
    }

    /// <summary>
    /// copy objects which reflected by this mirror
    /// </summary>
    /// <param name="_shooter">transform of shooter</param>
    IEnumerator CopyObjects(Player _shooter)
    {
        Vector2 stPos = _shooter.pos; // position of shooter's cell
        //Debug.Log("stPos: " + stPos);
        List<Pair> parRay = new List<Pair>
        {
            new Pair(0, 1)
        };

        int side, i, iBack, stCheck;
        if (dir) // horizontal, parallel with x
        {
            side = (mapPos.y - stPos.y > 0) ? -1 : 1;
            i = side > 0 ? Mathf.CeilToInt(mapPos.y) : Mathf.FloorToInt(mapPos.y);
            iBack = side < 0 ? Mathf.CeilToInt(mapPos.y) : Mathf.FloorToInt(mapPos.y);
            stCheck = (int)stPos.y;
        }
        else // vertical, parallel with y
        {
            side = (mapPos.x - stPos.x > 0) ? -1 : 1;
            i = side > 0 ? Mathf.CeilToInt(mapPos.x) : Mathf.FloorToInt(mapPos.x);
            iBack = side < 0 ? Mathf.CeilToInt(mapPos.x) : Mathf.FloorToInt(mapPos.x);
            stCheck = (int)stPos.x;
        }
        yield return null;

        // check before reflect (check walls and mirrors)
        float mirrorPos = (dir ? mapPos.y : mapPos.x);
        float stPosFix = (dir ? stPos.y : stPos.x);
        foreach (var wall in MapManager.inst.currentMap.wallGrid)
        {
            float wallPos = (dir ? wall.Key.y : wall.Key.x);
            if (wall.Value.GetInstanceID() != GetInstanceID() && (side < 0 ? wallPos < mirrorPos && wallPos > stPosFix : wallPos > mirrorPos && wallPos < stPosFix))
            {
                Pair pair = new Pair(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
                if (pair.l > pair.r) pair = pair.Swap();
                SubtractRay(parRay, pair);
                yield return null;
            }
        }
        
        Dictionary<Vector2Int, Floor> copyFloorGrid = new Dictionary<Vector2Int, Floor>(MapManager.inst.currentMap.floorGrid);
        Dictionary<Vector2Int, int> floorCountGrid = new Dictionary<Vector2Int, int>();
        foreach (var floor in copyFloorGrid)
        {
            floorCountGrid.Add(floor.Key, 0);
        }
        Dictionary<Vector2Int, IObject> copyObjGrid = new Dictionary<Vector2Int, IObject>(MapManager.inst.currentMap.objectGrid);
        Dictionary<Vector2, Wall> copyWallGrid = new Dictionary<Vector2, Wall>(MapManager.inst.currentMap.wallGrid);
        List<GameObject> copyPlayers = new List<GameObject>(MapManager.inst.players);

        float minMap = -1 * MapManager.inst.currentMap.maxMapSize - 1.5f;
        float maxMap = MapManager.inst.currentMap.maxMapSize + 1.5f;

        Debug.Log("Start Reflecting.");
        // check after reflect, if obj or floor, copy else if wall or mirror, Subtract
        for (; Mathf.Abs(i) < MapManager.inst.currentMap.maxMapSize; i += side)
        {
            for (float j = minMap; j < maxMap; j++)
            {
                // copy / remove wall
                Vector2 wallPos = dir ? new Vector2(j, i) : new Vector2(i, j);
                float nextx = dir ? wallPos.x : 2 * mapPos.x - wallPos.x;
                float nexty = dir ? 2 * mapPos.y - wallPos.y : wallPos.y;
                Vector2 oppWallPos = new Vector2(nextx, nexty);
                Wall wallAtPos = MapManager.inst.currentMap.GetWallAtPos(wallPos);
                if (wallAtPos != null) // have wall at wallpos
                {
                    Pair wallPair = new Pair(PointToParRay(stPos, wallAtPos.ldPos, true), PointToParRay(stPos, wallAtPos.rdPos, true));
                    if (wallPair.l > wallPair.r) wallPair = wallPair.Swap();
                    
                    if (IsInRay(parRay, wallPair))
                    {
                        MapManager.inst.currentMap.CreateWall(oppWallPos, wallAtPos.type);
                        SubtractRay(parRay, wallPair);
                    }
                }
                else if (MapManager.inst.currentMap.GetWallAtPos(oppWallPos) != null) // no wall at wallPos but have at opposite
                {
                    Pair tempPair = new Pair(PointToParRay(stPos, wallPos + (dir ? new Vector2(-0.5f, 0) : new Vector2(0, -0.5f)), true), PointToParRay(stPos, wallPos + (dir ? new Vector2(0.5f, 0) : new Vector2(0, 0.5f)), true));
                    if (IsInRay(parRay, tempPair)) MapManager.inst.currentMap.RemoveWall(oppWallPos);
                }
            }
            float iMid = i + 0.5f * side;
            for (float j = minMap; j < maxMap; j++)
            {
                //Debug.Log("iMid:" + iMid + " j:" + j);
                // copy / remove wall
                Vector2 wallPos = dir ? new Vector2(j - 0.5f, iMid) : new Vector2(iMid, j - 0.5f);
                float nextx = dir ? wallPos.x : 2 * mapPos.x - wallPos.x;
                float nexty = dir ? 2 * mapPos.y - wallPos.y : wallPos.y;
                Vector2 oppWallPos = new Vector2(nextx, nexty);
                Wall wallAtPos = MapManager.inst.currentMap.GetWallAtPos(wallPos);
                if (wallAtPos != null) // have wall at wallpos
                {
                    Pair wallPair = new Pair(PointToParRay(stPos, wallAtPos.ldPos, true), PointToParRay(stPos, wallAtPos.rdPos, true));
                    if (wallPair.l > wallPair.r) wallPair = wallPair.Swap();

                    if (IsInRay(parRay, wallPair))
                    {
                        MapManager.inst.currentMap.CreateWall(oppWallPos, wallAtPos.type);
                        SubtractRay(parRay, wallPair);
                    }
                }
                else if (MapManager.inst.currentMap.GetWallAtPos(oppWallPos) != null) // no wall at wallPos but have at opposite
                {
                    Pair tempPair = new Pair(PointToParRay(stPos, wallPos + (dir ? new Vector2(-0.5f, 0) : new Vector2(0, -0.5f)), true), PointToParRay(stPos, wallPos + (dir ? new Vector2(0.5f, 0) : new Vector2(0, 0.5f)), true));
                    if (IsInRay(parRay, tempPair)) MapManager.inst.currentMap.RemoveWall(oppWallPos);
                }
                // copy / remove floor and object
                Vector2 pointPos = dir ? new Vector2(j, iMid) : new Vector2(iMid, j);
                if (IsInRay(parRay, PointToParRay(stPos, pointPos, true)))
                {
                    //Debug.Log("inside " + pointPos);
                    Vector2Int floorPos = new Vector2Int(Mathf.FloorToInt(pointPos.x), Mathf.FloorToInt(pointPos.y));
                    int nextFloorx = dir ? floorPos.x : Mathf.RoundToInt(2 * ldPos.x - floorPos.x);
                    int nextFloory = dir ? Mathf.RoundToInt(2 * ldPos.y - floorPos.y) : floorPos.y;
                    Vector2Int oppFloorPos = new Vector2Int(nextFloorx, nextFloory);
                    Floor floor = MapManager.inst.currentMap.GetFloorAtPos(floorPos);
                    if (floor != null)
                    {
                        //Debug.Log(oppFloorPos);
                        if (floorCountGrid[floor.mapPos] == 0 && IsInRay(parRay, PointToParRay(stPos, floor.mapPos, true)))
                        {
                            floorCountGrid[floor.mapPos] = 1;
                        }
                        if (floorCountGrid[floor.mapPos] == 1)
                        {
                            MapManager.inst.currentMap.CreateFloor(oppFloorPos, floor.isGoalFloor);
                            MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                            if (floor.objOnFloor != null)
                            {
                                if (floor.objOnFloor.GetType() == ObjType.Briefcase)
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, ObjType.Briefcase, ((Briefcase)floor.objOnFloor).dropBullet);
                                else
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, floor.objOnFloor.GetType(), (floor.objOnFloor.GetType() != ObjType.Mannequin ? true : ((Mannequin)floor.objOnFloor).isWhite));
                            }
                            if (floor.isPlayerOn)
                                PlayerController.inst.CreatePlayer(oppFloorPos);
                        }
                        floorCountGrid[floor.mapPos]++;
                    }
                    else if ((floor = MapManager.inst.currentMap.GetFloorAtPos(oppFloorPos)) != null)
                    {
                        if (floor.isPlayerOn) PlayerController.inst.RemovePlayer(floor);
                        if (floor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                        MapManager.inst.currentMap.RemoveFloor(oppFloorPos);
                    }
                    floorPos = new Vector2Int(Mathf.FloorToInt(pointPos.x), Mathf.CeilToInt(pointPos.y));
                    nextFloorx = dir ? floorPos.x : Mathf.RoundToInt(2 * ldPos.x - floorPos.x);
                    nextFloory = dir ? Mathf.RoundToInt(2 * ldPos.y - floorPos.y) : floorPos.y;
                    oppFloorPos = new Vector2Int(nextFloorx, nextFloory);
                    floor = MapManager.inst.currentMap.GetFloorAtPos(floorPos);
                    if (floor != null)
                    {
                        //Debug.Log(oppFloorPos);
                        if (floorCountGrid[floor.mapPos] == 0 && IsInRay(parRay, PointToParRay(stPos, floor.mapPos, true)))
                        {
                            floorCountGrid[floor.mapPos] = 1;
                        }
                        if (floorCountGrid[floor.mapPos] == 1)
                        {
                            MapManager.inst.currentMap.CreateFloor(oppFloorPos, floor.isGoalFloor);
                            MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                            if (floor.objOnFloor != null)
                            {
                                if (floor.objOnFloor.GetType() == ObjType.Briefcase)
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, ObjType.Briefcase, ((Briefcase)floor.objOnFloor).dropBullet);
                                else
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, floor.objOnFloor.GetType(), (floor.objOnFloor.GetType() != ObjType.Mannequin ? true : ((Mannequin)floor.objOnFloor).isWhite));
                            }
                            if (floor.isPlayerOn)
                                PlayerController.inst.CreatePlayer(oppFloorPos);
                        }
                        floorCountGrid[floor.mapPos]++;
                    }
                    else if ((floor = MapManager.inst.currentMap.GetFloorAtPos(oppFloorPos)) != null)
                    {
                        if (floor.isPlayerOn) PlayerController.inst.RemovePlayer(floor);
                        if (floor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                        MapManager.inst.currentMap.RemoveFloor(oppFloorPos);
                    }
                    floorPos = new Vector2Int(Mathf.CeilToInt(pointPos.x), Mathf.FloorToInt(pointPos.y));
                    nextFloorx = dir ? floorPos.x : Mathf.RoundToInt(2 * ldPos.x - floorPos.x);
                    nextFloory = dir ? Mathf.RoundToInt(2 * ldPos.y - floorPos.y) : floorPos.y;
                    oppFloorPos = new Vector2Int(nextFloorx, nextFloory);
                    floor = MapManager.inst.currentMap.GetFloorAtPos(floorPos);
                    if (floor != null)
                    {
                        //Debug.Log(oppFloorPos);
                        if (floorCountGrid[floor.mapPos] == 0 && IsInRay(parRay, PointToParRay(stPos, floor.mapPos, true)))
                        {
                            floorCountGrid[floor.mapPos] = 1;
                        }
                        if (floorCountGrid[floor.mapPos] == 1)
                        {
                            MapManager.inst.currentMap.CreateFloor(oppFloorPos, floor.isGoalFloor);
                            MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                            if (floor.objOnFloor != null)
                            {
                                if (floor.objOnFloor.GetType() == ObjType.Briefcase)
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, ObjType.Briefcase, ((Briefcase)floor.objOnFloor).dropBullet);
                                else
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, floor.objOnFloor.GetType(), (floor.objOnFloor.GetType() != ObjType.Mannequin ? true : ((Mannequin)floor.objOnFloor).isWhite));
                            }
                            if (floor.isPlayerOn)
                                PlayerController.inst.CreatePlayer(oppFloorPos);
                        }
                        floorCountGrid[floor.mapPos]++;
                    }
                    else if ((floor = MapManager.inst.currentMap.GetFloorAtPos(oppFloorPos)) != null)
                    {
                        if (floor.isPlayerOn) PlayerController.inst.RemovePlayer(floor);
                        if (floor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                        MapManager.inst.currentMap.RemoveFloor(oppFloorPos);
                    }
                    floorPos = new Vector2Int(Mathf.CeilToInt(pointPos.x), Mathf.CeilToInt(pointPos.y));
                    nextFloorx = dir ? floorPos.x : Mathf.RoundToInt(2 * ldPos.x - floorPos.x);
                    nextFloory = dir ? Mathf.RoundToInt(2 * ldPos.y - floorPos.y) : floorPos.y;
                    oppFloorPos = new Vector2Int(nextFloorx, nextFloory);
                    floor = MapManager.inst.currentMap.GetFloorAtPos(floorPos);
                    if (floor != null)
                    {
                        //Debug.Log(oppFloorPos);
                        if (floorCountGrid[floor.mapPos] == 0 && IsInRay(parRay, PointToParRay(stPos, floor.mapPos, true)))
                        {
                            floorCountGrid[floor.mapPos] = 1;
                        }
                        if (floorCountGrid[floor.mapPos] == 1)
                        {
                            MapManager.inst.currentMap.CreateFloor(oppFloorPos, floor.isGoalFloor);
                            MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                            if (floor.objOnFloor != null)
                            {
                                if (floor.objOnFloor.GetType() == ObjType.Briefcase)
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, ObjType.Briefcase, ((Briefcase)floor.objOnFloor).dropBullet);
                                else
                                    MapManager.inst.currentMap.CreateObject(oppFloorPos, floor.objOnFloor.GetType(), (floor.objOnFloor.GetType() != ObjType.Mannequin ? true : ((Mannequin)floor.objOnFloor).isWhite));
                            }
                            if (floor.isPlayerOn)
                                PlayerController.inst.CreatePlayer(oppFloorPos);
                        }
                        floorCountGrid[floor.mapPos]++;
                    }
                    else if ((floor = MapManager.inst.currentMap.GetFloorAtPos(oppFloorPos)) != null)
                    {
                        if (floor.isPlayerOn) PlayerController.inst.RemovePlayer(floor);
                        if (floor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppFloorPos);
                        MapManager.inst.currentMap.RemoveFloor(oppFloorPos);
                    }
                }
            }
        }
        Break();
    }

    /// <summary>
    /// subtract _sub from _parRay
    /// </summary>
    /// <param name="_parRay">ray list to subtracted</param>
    /// <param name="_sub">ray to subtract</param>
    void SubtractRay(List<Pair> _parRay, Pair _sub)
    {
        Pair toAdd = null;
        foreach (Pair pair in _parRay)
        {
            if (pair.r < _sub.l || pair.l > _sub.r) continue;
            float[] arr = { pair.l, pair.r, _sub.l, _sub.r };

            for (int i = 0; i < 4; i++) // sort arr
            {
                int smallest = i;
                for (int j = i + 1; j < 4; j++)
                {
                    if (arr[smallest] > arr[j])
                    {
                        smallest = j;
                    }
                }
                float temp = arr[i];
                arr[i] = arr[smallest];
                arr[smallest] = temp;
            }

            // subtract
            if      (arr[0] == _sub.l && arr[2] == _sub.r)
            {
                pair.l = _sub.r;
            }
            else if (arr[1] == _sub.l && arr[3] == _sub.r)
            {
                pair.r = _sub.l;
            }
            else if (arr[1] == _sub.l && arr[2] == _sub.r)
            {
                toAdd = new Pair(_sub.r, pair.r);
                pair.r = _sub.l;
            }
        }
        if (toAdd != null) _parRay.Add(toAdd);
        for (int i = 0; i < _parRay.Count; i++)
        {
            if (_parRay[i].r - _parRay[i].l < 0.001f) _parRay.Remove(_parRay[i]);
        }

        //Debug.Log("ray to subtract: " + _sub.l + "~" + _sub.r + "\nRay count: " + _parRay.Count);
        //foreach (var ray in _parRay)
        //{
        //    Debug.Log("Ray: " + ray.l + "~" + ray.r);
        //}
    }

    /// <summary>
    /// check if _range is included in _parRay
    /// </summary>
    /// <param name="_parRay">ray list to be checked</param>
    /// <param name="_range">range to check</param>
    /// <returns>if _range is included in _parRay, return true</returns>
    bool IsInRay(List<Pair> _parRay, Pair _range)
    {
        bool output = false;
        foreach (Pair pair in _parRay)
        {
            //Debug.Log("IsinRay (" + pair.l + ", " + pair.r + ") " + _range.l + ", " + _range.r);
            if (pair.r <= _range.l || pair.l >= _range.r) continue;
            else
            {
                output = true;
                break;
            }
        }
        return output;
    }

    bool IsInRay(List<Pair> _parRay, float _obj)
    {
        foreach (Pair pair in _parRay)
        {
            //Debug.Log("IsinRay (" + pair.l + ", " + pair.r + ") " + _obj);
            if (pair.l <= _obj && pair.r >= _obj) return true;
        }
        return false;
    }

    bool IsInRayWeak(List<Pair> _parRay, Pair _range)
    {
        bool output = false;
        foreach (Pair pair in _parRay)
        {
            //Debug.Log("IsinRay (" + pair.l + ", " + pair.r + ") " + _range.l + ", " + _range.r);
            if (pair.r < _range.l || pair.l > _range.r) continue;
            else
            {
                output = true;
                break;
            }
        }
        return output;
    }

    /// <summary>
    /// calculate where _chPos is from _stPos
    /// </summary>
    /// <param name="_stPos">position of shooter</param>
    /// <param name="_chPos">position of object</param>
    /// <param name="_isRefl">if we calculate after reflecting, true</param>
    /// <returns>float value of _chPos is posed</returns>
    float PointToParRay(Vector2 _stPos, Vector2 _chPos, bool _isRefl)
    {
        if (dir)
        {
            float px = (_chPos.x-_stPos.x)*(ldPos.y-_stPos.y)/(_isRefl ? 2*ldPos.y-_chPos.y-_stPos.y : _chPos.y-_stPos.y) + _stPos.x;
            //Debug.Log("chPos: " + _chPos + ", output: " + (px - ldPos.x));
            return px - ldPos.x;
        }
        else
        {
            float py = (_chPos.y - _stPos.y) * (ldPos.x - _stPos.x) / (_isRefl ? 2 * ldPos.x - _chPos.x - _stPos.x : _chPos.x - _stPos.x) + _stPos.y;
            //Debug.Log("chPos: " + _chPos + ", output: " + (py - ldPos.y));
            return py - ldPos.y;
        }
    }
}
