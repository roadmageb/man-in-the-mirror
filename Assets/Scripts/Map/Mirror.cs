using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : Wall, IBulletInteractor, IBreakable
{
    [Space(15)]
    public GameObject scatteredMirror;
    public bool doReflect = false;

    public void Break()
    {
        Instantiate(scatteredMirror, transform.position, transform.rotation);
        MapManager.inst.currentMap.RemoveWall(this.mapPos);
    }

    public void StartCopy()
    {
        StartCoroutine(CopyObjects(PlayerController.inst.currentPlayer));
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            doReflect = true;
            //Debug.Log("ldPos: " + ldPos + ", rdPos: " + rdPos + ", dir: " + dir);
            // Make reflected objects
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
            new Pair(PointToParRay(stPos, ldPos, false), PointToParRay(stPos, rdPos, false))
        };
        Debug.Log(parRay[0].l + ", " + parRay[0].r);

        int side, i, reflectSide, mapRange;
        bool isSameRSide;
        if (dir) // horizontal, parallel with x
        {
            side = (mapPos.y - stPos.y > 0) ? -1 : 1;
            reflectSide = (mapPos.x - stPos.x > 0) ? 1 : -1;
            if (isSameRSide = mapPos.x == stPos.x) mapRange = Mathf.RoundToInt(mapPos.x);
            else mapRange = -reflectSide * (MapManager.inst.currentMap.maxMapSize + 1);
            i = side > 0 ? Mathf.CeilToInt(mapPos.y) : Mathf.FloorToInt(mapPos.y);
        }
        else // vertical, parallel with y
        {
            side = (mapPos.x - stPos.x > 0) ? -1 : 1;
            reflectSide = (mapPos.y - stPos.y > 0) ? 1 : -1;
            if (isSameRSide = mapPos.y == stPos.y) mapRange = Mathf.RoundToInt(mapPos.y);
            else mapRange = -reflectSide * (MapManager.inst.currentMap.maxMapSize + 1);
            i = side > 0 ? Mathf.CeilToInt(mapPos.x) : Mathf.FloorToInt(mapPos.x);
        }

        //Debug.Log("side: " + side + ", reflectSide: " + reflectSide + ", i: " + i + ", isSameRSide: " + isSameRSide);
        //Debug.Log("minRange: " + mapRange);
        yield return null;

        // check before reflect (check walls and mirrors)
        float mirrorPos = (dir ? mapPos.y : mapPos.x);
        float stPosFix = (dir ? stPos.y : stPos.x);
        foreach (var wall in MapManager.inst.currentMap.wallGrid)
        {
            float wallPos = (dir ? wall.Key.y : wall.Key.x);
            if (wall.Key != mapPos && (side < 0 ? wallPos < mirrorPos && wallPos > stPosFix : wallPos > mirrorPos && wallPos < stPosFix))
            {
                Pair pair = new Pair(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
                //Debug.Log(wall.Key);
                if (IsInRay(parRay, pair)) SubtractRay(parRay, pair);
                yield return null;
            }
        }
        
        Dictionary<Vector2Int, Floor> copyFloorGrid = new Dictionary<Vector2Int, Floor>(MapManager.inst.currentMap.floorGrid);
        Dictionary<Vector2, Wall> copyWallGrid = new Dictionary<Vector2, Wall>(MapManager.inst.currentMap.wallGrid);
        Dictionary<Vector2Int, int> floorCountGrid = new Dictionary<Vector2Int, int>();
        foreach (var floor in copyFloorGrid)
        {
            floorCountGrid.Add(floor.Key, 0);
        }

        // start reflection
        Debug.Log("Start Reflection");
        if (parRay.Count > 0)
        {
            Vector2Int frontFloorPos = dir ?
                new Vector2Int(Mathf.RoundToInt(mapPos.x), Mathf.RoundToInt(mapPos.y + 0.5f * side))
                : new Vector2Int(Mathf.RoundToInt(mapPos.x + 0.5f * side), Mathf.RoundToInt(mapPos.y));
            if (floorCountGrid.TryGetValue(frontFloorPos, out int frontFloorCount))
            {
                if (frontFloorCount == 0) floorCountGrid[frontFloorPos]++; // have floor
            }
            else // no floor on there
            {
                floorCountGrid.Add(frontFloorPos, -1);
            }
        }
        for (; Mathf.Abs(i) < (MapManager.inst.currentMap.maxMapSize + 1); i += side)
        {
            yield return null;
            bool anotherSide = false;
            for (int j = mapRange; Mathf.Abs(j) <= (MapManager.inst.currentMap.maxMapSize + 1); j += reflectSide)
            {
                // check floors
                Vector2Int floorPos = dir ? new Vector2Int(j, i) : new Vector2Int(i, j);
                Pair floorPair = new Pair(
                    PointToParRay(stPos, floorPos + 0.5f * (dir ? new Vector2(reflectSide, -side) : new Vector2(-side, reflectSide)), true),
                    PointToParRay(stPos, floorPos + 0.5f * (dir ? new Vector2(-reflectSide, side) : new Vector2(side, -reflectSide)), true));
                if (IsInRay(parRay, floorPair))
                {
                    int floorCount;
                    if (floorCountGrid.TryGetValue(floorPos, out floorCount))
                    {
                        if (floorCount == 0) floorCountGrid[floorPos]++; // have floor
                    }
                    else // no floor on there
                    {
                        floorCountGrid.Add(floorPos, -1);
                    }
                }
                
                // check walls and copy
                Vector2 wallPos = dir ? new Vector2(j + 0.5f * reflectSide, i) : new Vector2(i, j + 0.5f * reflectSide);
                //Debug.Log(wallPos);
                Vector2 oppoPos = GetOpposite(wallPos);
                Pair wallPair = dir ?
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(0, -0.5f), true), PointToParRay(stPos, wallPos + new Vector2(0, 0.5f), true))) :
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(-0.5f, 0), true), PointToParRay(stPos, wallPos + new Vector2(0.5f, 0), true)));
                if (IsInRay(parRay, wallPair))
                {
                    if (copyWallGrid.ContainsKey(wallPos))
                    {
                        Wall originWall = copyWallGrid[wallPos];
                        MapManager.inst.currentMap.CreateWall(oppoPos, originWall.type, false);
                        SubtractRay(parRay, wallPair);
                    }
                    else
                    {
                        if (copyWallGrid.ContainsKey(oppoPos)) MapManager.inst.currentMap.RemoveWall(oppoPos);
                    }
                }

                if (isSameRSide && Mathf.Abs(j) == (MapManager.inst.currentMap.maxMapSize + 1))
                {
                    if (anotherSide)
                    {
                        reflectSide *= -1;
                        anotherSide = false;
                        break;
                    }
                    else
                    {
                        anotherSide = true;
                        reflectSide *= -1;
                        j = mapRange - reflectSide;
                    }
                }
            }
            float iMid = i + 0.5f * side;
            // check walls and copy
            for (int j = mapRange; Mathf.Abs(j) <= (MapManager.inst.currentMap.maxMapSize + 1); j += reflectSide)
            {
                Vector2 wallPos = dir ? new Vector2(j, iMid) : new Vector2(iMid, j);
                //Debug.Log(wallPos);
                Vector2 oppoPos = GetOpposite(wallPos);
                Pair wallPair = !dir ?
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(0, -0.5f), true), PointToParRay(stPos, wallPos + new Vector2(0, 0.5f), true))) :
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(-0.5f, 0), true), PointToParRay(stPos, wallPos + new Vector2(0.5f, 0), true)));
                if (IsInRay(parRay, wallPair))
                {
                    if (copyWallGrid.ContainsKey(wallPos))
                    {
                        Wall originWall = copyWallGrid[wallPos];
                        MapManager.inst.currentMap.CreateWall(oppoPos, originWall.type, false);
                        SubtractRay(parRay, wallPair);
                    }
                    else
                    {
                        if (copyWallGrid.ContainsKey(oppoPos)) MapManager.inst.currentMap.RemoveWall(oppoPos);
                    }
                }

                if (isSameRSide && Mathf.Abs(j) == (MapManager.inst.currentMap.maxMapSize + 1))
                {
                    if (anotherSide)
                    {
                        reflectSide *= -1;
                        anotherSide = false;
                        break;
                    }
                    else
                    {
                        anotherSide = true;
                        reflectSide *= -1;
                        j = mapRange - reflectSide;
                    }
                }
            }
            // check floors
            for (int j = mapRange; Mathf.Abs(j) <= (MapManager.inst.currentMap.maxMapSize + 1); j += reflectSide)
            {
                Vector2 crossPoint = dir ? new Vector2(j + 0.5f * reflectSide, iMid) : new Vector2(iMid, j + 0.5f * reflectSide);
                if (IsInRayWeak(parRay, PointToParRay(stPos, crossPoint, true)))
                {
                    Vector2Int[] floorPoses =
                    {
                        new Vector2Int(Mathf.FloorToInt(crossPoint.x), Mathf.FloorToInt(crossPoint.y)),
                        new Vector2Int(Mathf.FloorToInt(crossPoint.x), Mathf.CeilToInt(crossPoint.y)),
                        new Vector2Int(Mathf.CeilToInt(crossPoint.x), Mathf.FloorToInt(crossPoint.y)),
                        new Vector2Int(Mathf.CeilToInt(crossPoint.x), Mathf.CeilToInt(crossPoint.y))
                    };
                    foreach (var floorPos in floorPoses)
                    {
                        int floorCount;
                        if (floorCountGrid.TryGetValue(floorPos, out floorCount))
                        {
                            if (floorCount == 0) floorCountGrid[floorPos]++; // have floor
                        }
                        else // no floor on there
                        {
                            floorCountGrid.Add(floorPos, -1);
                        }
                    }
                }

                if (isSameRSide && Mathf.Abs(j) == (MapManager.inst.currentMap.maxMapSize + 1))
                {
                    if (anotherSide)
                    {
                        reflectSide *= -1;
                        anotherSide = false;
                        break;
                    }
                    else
                    {
                        anotherSide = true;
                        reflectSide *= -1;
                        j = mapRange - reflectSide;
                    }
                }
            }
            if (parRay.Count == 0) break;
        }

        while (!doReflect) yield return null;

        // copy floors
        foreach (var floorCount in floorCountGrid)
        {
            Vector2Int oppoPos = GetOpposite(floorCount.Key);
            if (floorCount.Value > 0) // copy origin floor to opposite
            {
                Floor originFloor = MapManager.inst.currentMap.GetFloorAtPos(floorCount.Key);
                Floor oppoFloor = MapManager.inst.currentMap.GetFloorAtPos(oppoPos);
                MapManager.inst.currentMap.CreateFloor(oppoPos, originFloor.isGoalFloor);

                bool isOriginObjVisible = false;
                bool isOppoObjVisible = false;

                IObject obj = null;
                if (originFloor.objOnFloor != null || originFloor.isPlayerOn)
                {
                    obj = originFloor.objOnFloor;
                    var pos = PointToParRay(stPos, originFloor.mapPos, true);
                    if (IsInRay(parRay, pos))
                    {
                        isOriginObjVisible = true;
                    }
                    else
                    {
                        for (int r = 0; r < parRay.Count; ++r)
                        {
                            float radSq = originFloor.isPlayerOn ?
                                PlayerController.inst.radius * PlayerController.inst.radius :
                                obj.GetRadius() * obj.GetRadius();
                            //Debug.Log("radSquare: " + radSq);
                            if (radSq > PointToRayDistanceSquare(originFloor.mapPos, stPos, parRay[r].l) ||
                                radSq > PointToRayDistanceSquare(originFloor.mapPos, stPos, parRay[r].r))
                            {
                                isOriginObjVisible = true;
                                break;
                            }
                        }
                    }
                }
                if (isOriginObjVisible)
                {
                    if (oppoFloor != null) // obj를 카피해야하므로 무조건 반대편거를 지워야한다.
                    {
                        if (oppoFloor.isPlayerOn) PlayerController.inst.RemovePlayer(oppoFloor);
                        if (oppoFloor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppoPos);
                    }

                    if (originFloor.isPlayerOn) PlayerController.inst.CreatePlayer(oppoPos, floorCount.Key, dir); // player의 radius 체크해야됨.
                    else
                    {
                        switch (obj.GetType())
                        {
                            case ObjType.Mannequin:
                                MapManager.inst.currentMap.CreateObject(oppoPos, ObjType.Mannequin, 0, (obj as Mannequin).isWhite);
                                GameObject tempMann = MapManager.inst.currentMap.GetObjectAtPos(floorCount.Key).GetObject();
                                GameObject oppoMann = MapManager.inst.currentMap.GetObjectAtPos(oppoPos).GetObject();
                                Quaternion mirroredRotation = tempMann.transform.rotation;
                                Vector3 mirroredScale = tempMann.transform.localScale;
                                mirroredRotation.w *= -1;
                                if (dir) { mirroredRotation.z *= -1; mirroredScale.z *= -1; }
                                else { mirroredRotation.x *= -1; mirroredScale.x *= -1; }
                                oppoMann.transform.rotation = mirroredRotation;
                                oppoMann.transform.localScale = mirroredScale;
                                break;
                            case ObjType.Briefcase:
                                MapManager.inst.currentMap.CreateObject(oppoPos, ObjType.Briefcase, 0, (obj as Briefcase).dropBullet);
                                break;
                            default:
                                MapManager.inst.currentMap.CreateObject(oppoPos, obj.GetType(), 0);
                                break;
                        }
                    }
                }
                else if (oppoFloor != null && oppoFloor.objOnFloor != null)
                { // obj가 없거나 보이지 않으므로, 가만히 두거나, 거울이 없어졌을때 보인다면 없애야한다.
                    obj = oppoFloor.objOnFloor;
                    if (IsInRay(parRay, PointToParRay(stPos, oppoFloor.mapPos, false)))
                    {
                        isOppoObjVisible = true;
                    }
                    else
                    {
                        for (int r = 0; r < parRay.Count; ++r)
                        {
                            float radSq = oppoFloor.isPlayerOn ?
                                PlayerController.inst.radius * PlayerController.inst.radius :
                                obj.GetRadius() * obj.GetRadius();
                            //Debug.Log("radSquare: " + radSq);
                            if (radSq > PointToRayDistanceSquare(oppoFloor.mapPos, stPos, parRay[r].l, false) ||
                                radSq > PointToRayDistanceSquare(oppoFloor.mapPos, stPos, parRay[r].r, false))
                            {
                                isOppoObjVisible = true;
                                break;
                            }
                        }
                    }
                    if (isOppoObjVisible)
                    {
                        if (oppoFloor.isPlayerOn) PlayerController.inst.RemovePlayer(oppoFloor);
                        if (oppoFloor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppoPos);
                    }
                }
            }
            else if (floorCount.Value < 0) // remove opposite floor
            {
                Floor oppoFloor = MapManager.inst.currentMap.GetFloorAtPos(oppoPos);
                if (oppoFloor != null)
                {
                    if (oppoFloor.isPlayerOn) PlayerController.inst.RemovePlayer(oppoFloor);
                    if (oppoFloor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppoPos);
                    MapManager.inst.currentMap.RemoveFloor(oppoPos);
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
            List<float> arr = new List<float>()
            { pair.l, pair.r, _sub.l, _sub.r };

            arr.Sort();

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
            else if (arr[0] == _sub.l && arr[3] == _sub.r)
            {
                pair.r = pair.l;
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

    bool IsInRayWeak(List<Pair> _parRay, float _obj)
    {
        foreach (Pair pair in _parRay)
        {
            //Debug.Log("IsinRay (" + pair.l + ", " + pair.r + ") " + _obj);
            if (pair.l < _obj && pair.r > _obj) return true;
        }
        return false;
    }

    Vector2 GetOpposite(Vector2 objPos)
    {
        Vector2 output = new Vector2(objPos.x, objPos.y);
        if (dir) output.y = mapPos.y * 2 - objPos.y;
        else output.x = mapPos.x * 2 - objPos.x;
        return output;
    }

    Vector2Int GetOpposite(Vector2Int objPos)
    {
        Vector2 output = GetOpposite(new Vector2(objPos.x, objPos.y));
        return new Vector2Int(Mathf.RoundToInt(output.x), Mathf.RoundToInt(output.y));
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
            float px = (_chPos.x - _stPos.x) * (mapPos.y - _stPos.y) / (_isRefl ? 2 * mapPos.y - _chPos.y - _stPos.y : _chPos.y - _stPos.y) + _stPos.x;
            //Debug.Log("chPos: " + _chPos + ", output: " + (px - ldPos.x));
            return px;
        }
        else
        {
            float py = (_chPos.y - _stPos.y) * (mapPos.x - _stPos.x) / (_isRefl ? 2 * mapPos.x - _chPos.x - _stPos.x : _chPos.x - _stPos.x) + _stPos.y;
            //Debug.Log("chPos: " + _chPos + ", output: " + (py - ldPos.y));
            return py;
        }
    }

    float PointToRayDistanceSquare(Vector2Int point, Vector2 stPos, float ray, bool useOpposite = true)
    {
        if (useOpposite) point = GetOpposite(point);
        Vector2 realPos = dir ? mapPos + new Vector2(ray, 0) : mapPos + new Vector2(0, ray);
        // ax + by + c = 0
        float a = realPos.x - stPos.x;
        float b = stPos.y - realPos.y;
        float c = (stPos.x - realPos.x) * stPos.y + (realPos.y - stPos.y) * stPos.x;
        float distSq = (a * point.x + b * point.y + c) * (a * point.x + b * point.y + c) / (a * a + b * b);
        Debug.Log("point: " + point + ", ray: " + ray + ", distSq: " + distSq);
        return distSq;
    }
}
