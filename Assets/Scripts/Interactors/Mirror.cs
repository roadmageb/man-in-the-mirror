using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : MonoBehaviour, IBulletInteractor, IBreakable
{
    private Camera camera;
    private RenderTexture rt;

    // data about this mirror
    private Vector2Int pos; // position of mirror(left down)
    private int len; // length of mirror
    private bool dir; // false: ver, true: hor

    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            // Make reflected objects
            CopyObjects(gameObject.transform);
        }
        else if (bullet is TruthBullet)
        {
            // Break Mirror
        }
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        //TODO : Create RenderTexture and put it into Camera's targeTexture
    }

    private void Update()
    {
        //TODO :Calculate Camera's Position and Rotation
    }

    /// <summary>
    /// copy objects which reflected by this mirror
    /// </summary>
    /// <param name="_shooter">transform of shooter</param>
    private void CopyObjects(Transform _shooter)
    {
        Vector2Int stPos; // position of shooter's cell
        Vector2 rstPos;  // real position of shooter
        List<Pair<float, float>> parRay = new List<Pair<float, float>>
        {
            new Pair<float, float>(0, 1)
        };

        // TODO: stPos부터 pos까지 검사해 벽이나 거울이 있을경우 SubtractRay
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

    /// <summary>
    /// calculate where _chPos is from _stPos
    /// </summary>
    /// <param name="stPos">position of shooter</param>
    /// <param name="chPos">position of object</param>
    /// <param name="isRefl">if we calculate after reflecting, true</param>
    /// <returns>float value of _chPos is posed</returns>
    float PointToParRay(Vector2 _stPos, Vector2 _chPos, bool _isRefl)
    {
        if (dir) // horizontal
        {
            float dist = _chPos.y - _stPos.y + ( _isRefl ? (pos.y - _chPos.y) * 2 : 0);
            float spreadLen = len * dist / (pos.y - _stPos.y);
            float rayStPos = _stPos.x + (pos.x - _stPos.x) * dist / (pos.y - _stPos.y);
            return (_chPos.x - rayStPos) / spreadLen;
        }
        // TODO: when dir is vertical
        return 0;
    }
}
