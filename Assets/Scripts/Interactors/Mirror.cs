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
            // Make reflected objects
            CopyObjects(PlayerController.inst.currentPlayer);
        }
        else if (bullet is TruthBullet)
        {
            // Break Mirror
        }
    }

    /// <summary>
    /// copy objects which reflected by this mirror
    /// </summary>
    /// <param name="_shooter">transform of shooter</param>
    private void CopyObjects(Player _shooter)
    {
        Vector2Int stPos = _shooter.pos; // position of shooter's cell
        List<Pair<float, float>> parRay = new List<Pair<float, float>>
        {
            new Pair<float, float>(0, 1)
        };

        // check before reflect (check walls and mirrors)
        foreach (var wall in MapManager.inst.currentMap.wallGrid)
        {
            Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, false), PointToParRay(stPos, wall.Value.rdPos, false));
            if (pair.l > pair.r) pair = pair.Swap();
            SubtractRay(parRay, pair);
        }
        foreach (var mirr in MapManager.inst.currentMap.mirrorGrid)
        {
            if (mirr.Value != this)
            {
                Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, mirr.Value.ldPos, false), PointToParRay(stPos, mirr.Value.rdPos, false));
                if (pair.l > pair.r) pair = pair.Swap();
                SubtractRay(parRay, pair);
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
        for (; i < MapManager.inst.currentMap.maxMapSize; i += side)
        {
            foreach (var floor in MapManager.inst.currentMap.floorGrid)
            {
                if ((dir ? floor.Key.y : floor.Key.x) == i)
                {
                    if (IsInRay(parRay, PointToParRay(stPos, floor.Value.mapPos, true)))
                    { // copy floor
                        int nextx = dir ? floor.Key.x : 2 * ldPos.x - floor.Key.x;
                        int nexty = dir ? 2 * ldPos.y - floor.Key.y : floor.Key.y;
                        MapManager.inst.currentMap.CreateFloor(nextx, nexty);
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
            foreach (var wall in MapManager.inst.currentMap.wallGrid)
            {
                if ((dir ? wall.Key.y : wall.Key.x) == i)
                {
                    Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, wall.Value.ldPos, true), PointToParRay(stPos, wall.Value.rdPos, true));
                    if (pair.l > pair.r) pair = pair.Swap();
                    /*copy wall*/
                    SubtractRay(parRay, pair);
                }
            }
            foreach (var mirr in MapManager.inst.currentMap.mirrorGrid)
            {
                if (mirr.Value != this && (dir ? mirr.Key.y : mirr.Key.x) == i)
                {
                    Pair<float, float> pair = new Pair<float, float>(PointToParRay(stPos, mirr.Value.ldPos, true), PointToParRay(stPos, mirr.Value.rdPos, true));
                    if (pair.l > pair.r) pair = pair.Swap();
                    /*copy mirror*/
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
                float smallest = arr[i];
                int smallIdx = i;
                for (int j = i + 1; j < 4; j++)
                {
                    if (smallest > arr[j])
                    {
                        smallest = arr[j];
                        smallIdx = j;
                    }
                }
                float temp = arr[i];
                arr[i] = smallest;
                arr[smallIdx] = temp;
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
                _parRay.Add(new Pair<float, float>(pair.r, _sub.r));
                pair.r = _sub.l;
            }
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
        if (dir) // horizontal
        {
            float dist = _chPos.y - _stPos.y + (_isRefl ? (ldPos.y - _chPos.y) * 2 : 0);
            float spreadLen = len * dist / (ldPos.y - _stPos.y);
            float rayStPos = _stPos.x + (ldPos.x - _stPos.x) * dist / (ldPos.y - _stPos.y);
            return (_chPos.x - rayStPos) / spreadLen;
        }
        else // vertical
        {
            float dist = _chPos.x - _stPos.x + (_isRefl ? (ldPos.x - _chPos.x) * 2 : 0);
            float spreadLen = len * dist / (ldPos.x - _stPos.x);
            float rayStPos = _stPos.y + (ldPos.y - _stPos.y) * dist / (ldPos.x - _stPos.x);
            return (_chPos.y - rayStPos) / spreadLen;
        }
    }
}
