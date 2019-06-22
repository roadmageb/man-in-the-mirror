using System.Collections;
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
            Debug.Log("ldPos: " + ldPos + ", rdPos: " + rdPos + ", dir: " + dir);
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
        Debug.Log("stPos: " + stPos);
        List<Pair<float, float>> parRay = new List<Pair<float, float>>
        {
            new Pair<float, float>(0, 1)
        };

        int side, i, iReset, stCheck;
        if (dir) // horizontal, parallel with x
        {
            side = (mapPos.y - stPos.y > 0) ? -1 : 1;
            i = side > 0 ? Mathf.CeilToInt(mapPos.y) : Mathf.FloorToInt(mapPos.y);
            iReset = i;
            stCheck = (int)stPos.y;
        }
        else // vertical, parallel with y
        {
            side = (mapPos.x - stPos.x > 0) ? -1 : 1;
            i = side > 0 ? Mathf.CeilToInt(mapPos.x) : Mathf.FloorToInt(mapPos.x);
            iReset = i;
            stCheck = (int)stPos.x;
        }
        yield return null;

        // check before reflect (check walls and mirrors)
        for (; Mathf.Abs(i) < MapManager.inst.currentMap.maxMapSize && (side > 0 ? i < stCheck : i > stCheck); i += side)
        {
            foreach (var wall in MapManager.inst.currentMap.wallGrid)
            {
                if (wall.Value.GetInstanceID() != GetInstanceID() && (dir ? wall.Key.y : wall.Key.x) == i)
                {
                    Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
                    if (pair.l > pair.r) pair = pair.Swap();
                    Debug.Log("wall at " + wall.Key);
                    SubtractRay(parRay, pair);
                    yield return null;
                }
            }
        }
        Debug.Log("Start Reflecting.");
        // check after reflect, if obj or floor, copy else if wall or mirror, Subtract
        Dictionary<Vector2Int, Floor> copyFloorGrid = new Dictionary<Vector2Int, Floor>(MapManager.inst.currentMap.floorGrid);
        Dictionary<Vector2Int, IObject> copyObjGrid = new Dictionary<Vector2Int, IObject>(MapManager.inst.currentMap.objectGrid);
        Dictionary<Vector2, Wall> copyWallGrid = new Dictionary<Vector2, Wall>(MapManager.inst.currentMap.wallGrid);

        for (i = iReset; Mathf.Abs(i) < MapManager.inst.currentMap.maxMapSize; i += side)
        {
            foreach (var floor in copyFloorGrid)
            {
                if ((dir ? floor.Key.y : floor.Key.x) == i)
                {
                    if (IsInRay(parRay, PointToParRay(stPos, floor.Key, true)))
                    {
                        /*copy floor*/
                        int nextx = dir ? floor.Key.x : Mathf.RoundToInt(2 * ldPos.x - floor.Key.x);
                        int nexty = dir ? Mathf.RoundToInt(2 * ldPos.y - floor.Key.y) : floor.Key.y;
                        MapManager.inst.currentMap.CreateFloor(new Vector2Int(nextx, nexty));
                        yield return null;
                    }
                }
            }
            foreach (var obj in copyObjGrid)
            {
                if ((dir ? obj.Key.y : obj.Key.x) == i)
                {
                    if (IsInRay(parRay, PointToParRay(stPos, obj.Key, true)))
                    {
                        /*copy object*/
                        int nextx = dir ? obj.Key.x : Mathf.RoundToInt(2 * ldPos.x - obj.Key.x);
                        int nexty = dir ? Mathf.RoundToInt(2 * ldPos.y - obj.Key.y) : obj.Key.y;
                        ObjType type = obj.Value.GetType();

                        MapManager.inst.currentMap.CreateObject(new Vector2Int(nextx, nexty), type, (type == ObjType.Mannequin ? ((Mannequin)(obj.Value)).isWhite : true));
                        yield return null;
                    }
                }
            }
            foreach (var wall in copyWallGrid)
            {
                if ((dir ? wall.Key.y : wall.Key.x) == i)
                {
                    Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, true), PointToParRay(stPos, wall.Value.rdPos, true));
                    if (pair.l > pair.r) pair = pair.Swap();

                    if (IsInRay(parRay, pair.l) || IsInRay(parRay, pair.r))
                    {
                        /*copy wall*/
                        float nextx = dir ? wall.Key.x : 2 * mapPos.x - wall.Key.x;
                        float nexty = dir ? 2 * mapPos.y - wall.Key.y : wall.Key.y;
                        MapManager.inst.currentMap.CreateWall(new Vector2(nextx, nexty), wall.Value.type);

                        SubtractRay(parRay, pair);
                        yield return null;
                    }
                }
            }
        }
        MapManager.inst.currentMap.RemoveWall(mapPos);
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

        Debug.Log("ray to subtract: " + _sub.l + "~" + _sub.r + "\nRay count: " + _parRay.Count);
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
