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
        Debug.Log("init ray: " + parRay[0].l + ", " + parRay[0].r);

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

        // -1이면 반대편에서 삭제, 0이면 유지, 1 이상이면 반대편에 복사
        Dictionary<Vector2Int, int> floorCountGrid = new Dictionary<Vector2Int, int>();
        foreach (var floor in MapManager.inst.currentMap.floorGrid)
        {
            floorCountGrid.Add(floor.Key, 0);
        }
        Dictionary<Vector2, int> wallCountGrid = new Dictionary<Vector2, int>();
        foreach (var wall in MapManager.inst.currentMap.wallGrid)
        {
            wallCountGrid.Add(wall.Key, 0);
        }
        Dictionary<Vector2, int> objectCountGrid = new Dictionary<Vector2, int>();
        foreach (var obj in MapManager.inst.currentMap.objectGrid)
        {
            objectCountGrid.Add(obj.Key, 0);
        }

        // start reflection
        // Debug.Log("Start Reflection");
        if (parRay.Count > 0) // 자기 바로 앞의 floor를 복사
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
                Vector2Int floorPos = dir ? new Vector2Int(j, i) : new Vector2Int(i, j);
                #region check floors
                Pair floorPair = new Pair(
                    PointToParRay(stPos, floorPos + 0.5f * (dir ? new Vector2(reflectSide, -side) : new Vector2(-side, reflectSide)), true),
                    PointToParRay(stPos, floorPos + 0.5f * (dir ? new Vector2(-reflectSide, side) : new Vector2(side, -reflectSide)), true));
                if (IsInRay(parRay, floorPair))
                {
                    if (floorCountGrid.TryGetValue(floorPos, out int count)) // have floor on player's side
                    {
                        if (count == 0)
                        {
                            floorCountGrid[floorPos] = 1;
                        }
                    }
                    else if (floorCountGrid.ContainsKey(GetOpposite(floorPos))) // no floor on player's side, but have floor on opposite
                    {
                        floorCountGrid.Add(floorPos, -1);
                    }
                }
                #endregion

                #region check object & jackson
                bool isJackson = false;
                if (MapManager.inst.currentMap.GetFloorAtPos(floorPos) != null)
                {
                    isJackson = MapManager.inst.currentMap.GetFloorAtPos(floorPos).isPlayerOn;
                }
                bool haveObject = (objectCountGrid.TryGetValue(floorPos, out int val) && val == 0) || isJackson;
                float objRadius = haveObject ? (isJackson ?
                        PlayerController.inst.radius : MapManager.inst.currentMap.objectGrid[floorPos].GetRadius()) : 100;
                bool oppoObject = (objectCountGrid.TryGetValue(GetOpposite(floorPos), out val) && val == 0);
                float oppoRadius = oppoObject ? (isJackson ?
                    PlayerController.inst.radius : MapManager.inst.currentMap.objectGrid[GetOpposite(floorPos)].GetRadius()) : 100;
                if (haveObject) // have object on floorPos
                {
                    if (CheckObjectVisible(parRay, stPos, floorPos, objRadius))
                    {
                        if (isJackson)
                        {
                            objectCountGrid[floorPos] = 2;
                        }
                        else
                        {
                            objectCountGrid[floorPos] = 1;
                        }
                    }
                    else if (oppoObject && CheckObjectVisible(parRay, stPos, GetOpposite(floorPos), oppoRadius, false))
                    {
                        objectCountGrid[floorPos] = -1;
                    }
                    else
                    {
                        objectCountGrid.Remove(floorPos);
                    }
                }
                else if (oppoObject) // no object on floorPos, opposite have object
                {
                    if (CheckObjectVisible(parRay, stPos, GetOpposite(floorPos), oppoRadius, false) && !objectCountGrid.ContainsKey(floorPos))
                    {
                        objectCountGrid.Add(floorPos, -1);
                    }
                }
                #endregion

                Vector2 wallPos = dir ? new Vector2(j + 0.5f * reflectSide, i) : new Vector2(i, j + 0.5f * reflectSide);
                #region check object
                haveObject = objectCountGrid.TryGetValue(wallPos, out val) && val == 0;
                objRadius = haveObject ? MapManager.inst.currentMap.objectGrid[wallPos].GetRadius() : 100;
                oppoObject = objectCountGrid.TryGetValue(GetOpposite(wallPos), out val) && val == 0;
                oppoRadius = oppoObject ? MapManager.inst.currentMap.objectGrid[GetOpposite(wallPos)].GetRadius() : 100;
                if (haveObject) // have object on floorPos
                {
                    if (CheckObjectVisible(parRay, stPos, wallPos, objRadius))
                    {
                        objectCountGrid[wallPos] = 1;
                    }
                    else if (oppoObject && CheckObjectVisible(parRay, stPos, GetOpposite(wallPos), oppoRadius, false))
                    {
                        objectCountGrid[wallPos] = -1;
                    }
                    else
                    {
                        objectCountGrid.Remove(wallPos);
                    }
                }
                else if (oppoObject) // no object on floorPos, opposite have object
                {
                    if (CheckObjectVisible(parRay, stPos, GetOpposite(wallPos), oppoRadius, false) && !objectCountGrid.ContainsKey(wallPos))
                    {
                        objectCountGrid.Add(wallPos, -1);
                    }
                }
                #endregion

                #region check walls
                Pair wallPair = dir ?
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(0, -0.5f), true), PointToParRay(stPos, wallPos + new Vector2(0, 0.5f), true))) :
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(-0.5f, 0), true), PointToParRay(stPos, wallPos + new Vector2(0.5f, 0), true)));
                if (IsInRay(parRay, wallPair))
                {
                    if (wallCountGrid.TryGetValue(wallPos, out int count)) // have wall
                    {
                        if (count == 0)
                        {
                            wallCountGrid[wallPos] = 1;
                            if (MapManager.inst.currentMap.GetWallAtPos(wallPos).type != WallType.Glass) SubtractRay(parRay, wallPair);
                        }
                    }
                    else if (wallCountGrid.ContainsKey(GetOpposite(wallPos))) // no wall on player's side, but have wall on opposite
                    {
                        wallCountGrid.Add(wallPos, -1);
                    }
                }
                #endregion

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
            for (int j = mapRange; Mathf.Abs(j) <= (MapManager.inst.currentMap.maxMapSize + 1); j += reflectSide)
            {
                Vector2 wallPos = dir ? new Vector2(j, iMid) : new Vector2(iMid, j);
                #region check object
                bool haveObject = objectCountGrid.TryGetValue(wallPos, out int val) && val == 0;
                float objRadius = haveObject ? MapManager.inst.currentMap.objectGrid[wallPos].GetRadius() : 100;
                bool oppoObject = objectCountGrid.TryGetValue(GetOpposite(wallPos), out val) && val == 0;
                float oppoRadius = oppoObject ? MapManager.inst.currentMap.objectGrid[GetOpposite(wallPos)].GetRadius() : 100;
                if (haveObject) // have object on floorPos
                {
                    if (CheckObjectVisible(parRay, stPos, wallPos, objRadius))
                    {
                        objectCountGrid[wallPos] = 1;
                    }
                    else if (oppoObject && CheckObjectVisible(parRay, stPos, GetOpposite(wallPos), oppoRadius, false))
                    {
                        objectCountGrid[wallPos] = -1;
                    }
                    else
                    {
                        objectCountGrid.Remove(wallPos);
                    }
                }
                else if (oppoObject) // no object on floorPos, opposite have object
                {
                    if (CheckObjectVisible(parRay, stPos, GetOpposite(wallPos), oppoRadius, false) && !objectCountGrid.ContainsKey(wallPos))
                    {
                        objectCountGrid.Add(wallPos, -1);
                    }
                }
                #endregion

                #region check walls
                Pair wallPair = !dir ?
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(0, -0.5f), true), PointToParRay(stPos, wallPos + new Vector2(0, 0.5f), true))) :
                    (new Pair(PointToParRay(stPos, wallPos + new Vector2(-0.5f, 0), true), PointToParRay(stPos, wallPos + new Vector2(0.5f, 0), true)));
                if (IsInRay(parRay, wallPair))
                {
                    if (wallCountGrid.TryGetValue(wallPos, out int count)) // have wall
                    {
                        if (count == 0)
                        {
                            wallCountGrid[wallPos] = 1;
                            if (MapManager.inst.currentMap.GetWallAtPos(wallPos).type != WallType.Glass) SubtractRay(parRay, wallPair);
                        }
                    }
                    else if (wallCountGrid.ContainsKey(GetOpposite(wallPos))) // no wall on player's side, but have wall on opposite
                    {
                        wallCountGrid.Add(wallPos, -1);
                    }
                }
                #endregion

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
            for (int j = mapRange; Mathf.Abs(j) <= (MapManager.inst.currentMap.maxMapSize + 1); j += reflectSide)
            {
                Vector2 crossPoint = dir ? new Vector2(j + 0.5f * reflectSide, iMid) : new Vector2(iMid, j + 0.5f * reflectSide);
                #region check floor near to crossPoint
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
                        if (floorCountGrid.TryGetValue(floorPos, out int count)) // have floor on player's side
                        {
                            if (count == 0)
                            {
                                floorCountGrid[floorPos] = 1;
                            }
                        }
                        else if (floorCountGrid.ContainsKey(GetOpposite(floorPos)) && !floorCountGrid.ContainsKey(floorPos)) // no floor on player's side, but have floor on opposite
                        {
                            floorCountGrid.Add(floorPos, -1);
                        }
                    }
                }
                #endregion

                #region check object on crossPoint
                bool haveObject = objectCountGrid.TryGetValue(crossPoint, out int val) && val == 0;
                float objRadius = haveObject ? MapManager.inst.currentMap.objectGrid[crossPoint].GetRadius() : 100;
                bool oppoObject = objectCountGrid.TryGetValue(GetOpposite(crossPoint), out val) && val == 0;
                float oppoRadius = oppoObject ? MapManager.inst.currentMap.objectGrid[GetOpposite(crossPoint)].GetRadius() : 100;
                if (haveObject) // have object on floorPos
                {
                    if (CheckObjectVisible(parRay, stPos, crossPoint, objRadius))
                    {
                        objectCountGrid[crossPoint] = 1;
                    }
                    else if (oppoObject && CheckObjectVisible(parRay, stPos, GetOpposite(crossPoint), oppoRadius, false))
                    {
                        objectCountGrid[crossPoint] = -1;
                    }
                    else
                    {
                        objectCountGrid.Remove(crossPoint);
                    }
                }
                else if (oppoObject) // no object on floorPos, opposite have object
                {
                    if (CheckObjectVisible(parRay, stPos, GetOpposite(crossPoint), oppoRadius, false) && !objectCountGrid.ContainsKey(crossPoint))
                    {
                        objectCountGrid.Add(crossPoint, -1);
                    }
                }
                #endregion

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
            //else
            //{
            //    Debug.Log("Rays: ");
            //    foreach (var ray in parRay)
            //    {
            //        Debug.Log("Ray: " + ray.l + "~" + ray.r);
            //    }
            //}
        }

        while (!doReflect) yield return null;

        #region remove objects & jacksons
        foreach (var obj in objectCountGrid)
        {
            //Debug.Log(obj);
            Vector2 oppoPos = GetOpposite(obj.Key);
            Vector2Int oppoFloorPos = Vector2Int.RoundToInt(oppoPos);
            if (obj.Value != 0)
            {
                // remove opposite objects
                if (MapManager.inst.currentMap.objectGrid.ContainsKey(oppoPos))
                {
                    MapManager.inst.currentMap.RemoveObject(oppoPos);
                }
                if (MapManager.inst.currentMap.GetFloorAtPos(oppoFloorPos) != null &&
                    MapManager.inst.currentMap.GetFloorAtPos(oppoFloorPos).isPlayerOn &&
                    oppoPos.ManhattanDistance(oppoFloorPos) < 0.2f)
                {
                    PlayerController.inst.RemovePlayer(oppoFloorPos);
                }
            }
        }
        #endregion

        #region copy walls
        foreach (var wall in wallCountGrid)
        {
            //Debug.Log(wall);

            Wall mySideWall = MapManager.inst.currentMap.GetWallAtPos(wall.Key);
            Vector2 oppoPos = GetOpposite(wall.Key);
            if (wall.Value > 0) // create at opposite
            {
                MapManager.inst.currentMap.CreateWall(oppoPos, mySideWall.type, false);
            }
            else if (wall.Value < 0) // remove from opposite
            {
                MapManager.inst.currentMap.RemoveWall(oppoPos);
            }
        }
        #endregion

        #region copy floors
        foreach (var floor in floorCountGrid)
        {
            //Debug.Log(floor);
            Floor mySideFloor = MapManager.inst.currentMap.GetFloorAtPos(floor.Key);
            Vector2Int oppoPos = GetOpposite(floor.Key);
            if (floor.Value > 0 && mySideFloor != null) // create at opposite
            {
                MapManager.inst.currentMap.CreateFloor(oppoPos, mySideFloor.isGoalFloor);
            }
            else if (floor.Value < 0 && MapManager.inst.currentMap.GetFloorAtPos(oppoPos) != null) // remove from opposite
            {
                MapManager.inst.currentMap.RemoveFloor(oppoPos);
            }
        }
        #endregion

        #region copy objects & jacksons
        foreach (var obj in objectCountGrid)
        {
            //Debug.Log(obj);
            Vector2 oppoPos = GetOpposite(obj.Key);
            Vector2Int oppoFloorPos = Vector2Int.RoundToInt(oppoPos);
            if (obj.Value > 0) // create or remove
            {
                if (obj.Value > 0)
                {
                    if (obj.Value > 1) // create jackson on opposite
                    {
                        PlayerController.inst.CreatePlayer(oppoFloorPos, Vector2Int.RoundToInt(obj.Key), dir);
                    }
                    else
                    {
                        var iObjectOnPos = MapManager.inst.currentMap.GetObjectAtPos(obj.Key);
                        var oldObject = iObjectOnPos.GetObject();
                        IObject newIObject = MapManager.inst.currentMap.CreateObject(oppoPos, iObjectOnPos.GetType(), iObjectOnPos.GetObject().transform.rotation.eulerAngles.y, iObjectOnPos.GetAdditionals());
                        
                        if (newIObject != null)
                        {
                            var newObject = newIObject.GetObject();

                            // mirror new object
                            Quaternion mirroredRotation = oldObject.transform.rotation;
                            Vector3 mirroredScale = oldObject.transform.localScale;
                            mirroredRotation.w *= -1;
                            if (dir) { mirroredRotation.z *= -1; mirroredScale.z *= -1; }
                            else { mirroredRotation.x *= -1; mirroredScale.x *= -1; }
                            newObject.transform.rotation = mirroredRotation;
                            newObject.transform.localScale = mirroredScale;
                        }
                    }
                }
            }
        }
        #endregion

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

    float PointToRayDistanceSquare(Vector2 point, Vector2 stPos, float ray, bool useOpposite = true)
    {
        if (useOpposite) point = GetOpposite(point);
        Vector2 realPos = mapPos;
        if (dir)
        {
            realPos.x = stPos.x + ray;
        }
        else
        {
            realPos.y = stPos.y + ray;
        }

        // ax + by + c = 0
        float a = realPos.y - stPos.y;
        float b = stPos.x - realPos.x;
        float c = (realPos.x - stPos.x) * stPos.y + (stPos.y - realPos.y) * stPos.x;
        float distSq = (a * point.x + b * point.y + c) * (a * point.x + b * point.y + c) / (a * a + b * b);
        //Debug.Log("a = " + a + ", b = " + b + ", c = " + c);
        //Debug.Log("stPos: "+ stPos + ", realPos: " + realPos + ", point: " + point + ", distSq: " + distSq);
        return distSq;
    }

    bool CheckObjectVisible(List<Pair> parRay, Vector2 stPos, Vector2 pos, float radius, bool useOpposite = true)
    {
        if (IsInRay(parRay, PointToParRay(stPos, pos, useOpposite)))
        {
            return true;
        }

        //Debug.Log("checking " +pos + ", " + radius);

        radius *= radius;
        for (int i = 0; i < parRay.Count; ++i)
        {
            if (PointToRayDistanceSquare(pos, stPos, parRay[i].l, useOpposite) < radius ||
                PointToRayDistanceSquare(pos, stPos, parRay[i].r, useOpposite) < radius)
            {
                Debug.Log(true);
                return true;
            }
        }
        Debug.Log(false);
        return false;
    }
}
