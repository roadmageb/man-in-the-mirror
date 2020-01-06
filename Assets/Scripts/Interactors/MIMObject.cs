using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMObject : MonoBehaviour
{
    [Header("Object Setting")]
    public Vector2 position;
    public float radius;
    public ObjType type;

    public virtual void Init()
    {
        // 주변의 floor에 자기가 관련되어있다고 표시
        // 나머지는 override해서 구현하시오(승리조건이나 moveAction 추가 등)
    }

    // 필요한 함수 있으면 이아래로 추가하면됨
}
