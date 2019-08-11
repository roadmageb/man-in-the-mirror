﻿using System.Collections;
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
            if (wall.Value.mapPos != mapPos && (side < 0 ? wallPos < mirrorPos && wallPos > stPosFix : wallPos > mirrorPos && wallPos < stPosFix))
            {
                Pair pair = new Pair(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
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
        for (; Mathf.Abs(i) < (MapManager.inst.currentMap.maxMapSize + 1); i += side)
        {
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
        }
        yield return null;
        // copy floors
        foreach (var floorCount in floorCountGrid)
        {
            Vector2Int oppoPos = GetOpposite(floorCount.Key);
            if (floorCount.Value > 0) // copy origin floor to opposite
            {
                Floor originFloor = MapManager.inst.currentMap.GetFloorAtPos(floorCount.Key);
                Floor oppoFloor = MapManager.inst.currentMap.GetFloorAtPos(oppoPos);
                MapManager.inst.currentMap.CreateFloor(oppoPos, originFloor.isGoalFloor);
                if (oppoFloor != null)
                {
                    if (oppoFloor.isPlayerOn) PlayerController.inst.RemovePlayer(oppoFloor);
                    if (oppoFloor.objOnFloor != null) MapManager.inst.currentMap.RemoveObject(oppoPos);
                }
                if (originFloor.isPlayerOn) PlayerController.inst.CreatePlayer(oppoPos, floorCount.Key, dir);
                else if (originFloor.objOnFloor != null)
                {
                    IObject obj = originFloor.objOnFloor;
                    switch (obj.GetType())
                    {
                        case ObjType.Mannequin:
                            MapManager.inst.currentMap.CreateObject(oppoPos, ObjType.Mannequin, (obj as Mannequin).isWhite);
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
                            MapManager.inst.currentMap.CreateObject(oppoPos, ObjType.Briefcase, (obj as Briefcase).dropBullet);
                            break;
                        default:
                            MapManager.inst.currentMap.CreateObject(oppoPos, obj.GetType());
                            break;
                    }
                }
            }
            else if (floorCount.Value < 0) // remove opposite floor
            {
                Floor oppoFloor = MapManager.inst.currentMap.GetFloorAtPos(oppoPos);
                if (oppoFloor != null)
                {
                    PlayerController.inst.RemovePlayer(oppoFloor);
                    MapManager.inst.currentMap.RemoveObject(oppoPos);
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
