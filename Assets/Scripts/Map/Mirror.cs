﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : Wall, IBulletInteractor, IBreakable
{
    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            Debug.Log("ldPos: " + ldPos + ", rdPos: " + rdPos);
            // Make reflected objects
            StartCoroutine(CopyObjects(PlayerController.inst.currentPlayer));
            //Destroy(gameObject, 1f);
        }
    }

    /// <summary>
    /// copy objects which reflected by this mirror
    /// </summary>
    /// <param name="_shooter">transform of shooter</param>
    IEnumerator CopyObjects(Player _shooter)
    {
        Vector2 stPos = _shooter.pos; // position of shooter's cell
        Debug.Log("stPos: " + stPos);
        List<Pair<float, float>> parRay = new List<Pair<float, float>>
        {
            new Pair<float, float>(0, 1)
        };

        // check before reflect (check walls and mirrors)
        foreach (var wall in MapManager.inst.currentMap.wallGrid)
        {
            if (wall.Value.GetInstanceID() != GetInstanceID())
            {
                Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
                if (pair.l > pair.r) pair = pair.Swap();
                SubtractRay(parRay, pair);
                yield return null;
            }
        }

        // check after reflect, if obj or floor, copy else if wall or mirror, Subtract
        int side, i;
        if (dir) // horizontal, parallel with x
        {
            side = (ldPos.y - stPos.y > 0) ? -1 : 1;
            i = ldPos.y;
        }
        else // vertical, parallel with y
        {
            side = (ldPos.x - stPos.x > 0) ? -1 : 1;
            i = ldPos.x;
        }
        yield return null;
        
        for (; Mathf.Abs(i) < MapManager.inst.currentMap.maxMapSize; i += side)
        {
            foreach (var floor in MapManager.inst.currentMap.floorGrid)
            {
                if ((dir ? floor.Key.y : floor.Key.x) == i)
                {
                    if (IsInRay(parRay, PointToParRay(stPos, floor.Key, true)))
                    {
                        /*copy floor*/
                        int nextx = dir ? floor.Key.x : 2 * ldPos.x - floor.Key.x;
                        int nexty = dir ? 2 * ldPos.y - floor.Key.y : floor.Key.y;
                        MapManager.inst.currentMap.CreateFloor(new Vector2Int(nextx, nexty));
                        yield return null;
                    }
                }
            }
            foreach (var obj in MapManager.inst.currentMap.objectGrid)
            {
                if ((dir ? obj.Key.y : obj.Key.x) == i)
                {
                    if (IsInRay(parRay, PointToParRay(stPos, obj.Value.GetPos(), true)))
                    {
                        /*copy object*/
                    }
                }
            }
            Dictionary<Vector2, Wall> copyGrid = new Dictionary<Vector2, Wall>(MapManager.inst.currentMap.wallGrid);
            foreach (var wall in copyGrid)
            {
                if ((dir ? wall.Key.y : wall.Key.x) == i)
                {
                    Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, true), PointToParRay(stPos, wall.Value.rdPos, true));
                    if (pair.l > pair.r) pair = pair.Swap();

                    /*copy wall*/
                    float nextx = dir ? wall.Key.x : 2 * ldPos.x - wall.Key.x;
                    float nexty = dir ? 2 * ldPos.y - wall.Key.y : wall.Key.y;
                    MapManager.inst.currentMap.CreateWall(new Vector2(nextx, nexty), wall.Value.type);
                    Debug.Log("created at " + nextx + ", " + nexty);

                    SubtractRay(parRay, pair);
                }
            }
        }
    }

    /// <summary>
    /// subtract _sub from _parRay
    /// </summary>
    /// <param name="_parRay">ray list to subtracted</param>
    /// <param name="_sub">ray to subtract</param>
    void SubtractRay(List<Pair<float, float>> _parRay, Pair<float, float> _sub)
    {
        foreach (Pair<float, float> pair in _parRay)
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
                _parRay.Add(new Pair<float, float>(_sub.r, pair.r));
                pair.r = _sub.l;
            }
        }
        for (int i = 0; i < _parRay.Count; i++)
        {
            if (_parRay[i].r - _parRay[i].l < 0.001f) _parRay.Remove(_parRay[i]);
        }

        Debug.Log("Ray count: " + _parRay.Count);
        foreach (var ray in _parRay)
        {
            Debug.Log("Ray: " + ray.l + "~" + ray.r);
        }
    }

    /// <summary>
    /// check if _range is included in _parRay
    /// </summary>
    /// <param name="_parRay">ray list to be checked</param>
    /// <param name="_range">range to check</param>
    /// <returns>if _range is included in _parRay, return true</returns>
    bool IsInRay(List<Pair<float, float>> _parRay, Pair<float, float> _range)
    {
        bool output = false;
        foreach (Pair<float, float> pair in _parRay)
        {
            if (pair.r <= _range.l || pair.l >= _range.r) continue;
            else
            {
                output = true;
                break;
            }
        }
        return output;
    }

    bool IsInRay(List<Pair<float, float>> _parRay, float _obj)
    {
        foreach (Pair<float, float> pair in _parRay)
        {
            if (pair.l <= _obj && pair.r >= _obj) return true;
        }
        return false;
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
            return px - ldPos.x;
        }
        else
        {
            float py = (_chPos.y - _stPos.y) * (ldPos.x - _stPos.x) / (_isRefl ? 2 * ldPos.x - _chPos.x - _stPos.x : _chPos.x - _stPos.x) + _stPos.y;
            return py - ldPos.y;
        }
    }
}
