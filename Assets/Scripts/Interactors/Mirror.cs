using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : MonoBehaviour, IBulletInteractor, IBreakable
{
    public Vector2 mapPos;
    // data about this mirror
    public Vector2Int ldPos // left down pos
    {
        get { return new Vector2Int((int)mapPos.x, (int)mapPos.y); }
    }
    public Vector2Int rdPos // right down pos
    {
        get { return ldPos + (dir ? new Vector2Int(1, 0) : new Vector2Int(0, 1)); }
    }
    public bool dir // false: ver, true: hor
    {
        get { return (int)(transform.rotation.eulerAngles.y / 90) % 2 != 1; }
    }
    private int len = 1; // length of mirror

    public void SetmapPos(Vector2 pos)
    {
        mapPos = pos;
    }

    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            // Make reflected objects
            CopyObjects(null);
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

        // TODO: pos부터 맵의 끝까지 검사해 맵의 각 요소가 IfInRay면 거울 반대편에 복사, 벽이나 거울이면 SubtractRay
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
