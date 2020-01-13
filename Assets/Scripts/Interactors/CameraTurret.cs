using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurret : MonoBehaviour, IObject, IBreakable, IPlayerInteractor
{
	[SerializeField]
    public float radius = 0.5f;
    public Vector2 position;
    [Space(15)]
    public GameObject scatteredTurret;

    /// <param name="additonal">
    /// <br/>No additional data
    /// </param>
    #region IObject
    public void Init(Vector2 pos, params object[] additonal)
    {
        position = pos;
		PlayerController.inst.OnPlayerMove += Interact;
        if (GameManager.aTurret >= 0) MapManager.inst.currentMap.clearConditions[GameManager.aTurret].IsDone(0, 1);
    }

    public object[] GetAdditionals()
    {
        return new object[0];
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2 GetPos()
    {
        return position;
    }

    ObjType IObject.GetType()
    {
        return ObjType.Camera;
    }

    public float GetRadius()
    {
        return radius;
    }
    #endregion

    #region IBreakable
    public void Break()
    {
        Instantiate(scatteredTurret, transform.position + new Vector3(0, 0.3f), transform.rotation);
        if (GameManager.nTurret >= 0) MapManager.inst.currentMap.clearConditions[GameManager.nTurret].IsDone(1);
        MapManager.inst.currentMap.RemoveObject(position);
    }
    #endregion

    #region IPlayerInteractor
    public void Interact(Vector2Int pos)
    {
        if(!GameManager.inst.isGameOver && PlayerController.inst.currentPlayer != null)
        {
            if (position.IsInAdjacentArea(pos, 1) && MapManager.inst.currentMap.GetWallAtPos((Vector2)(position + pos) / 2) == null)
            {
                GameManager.inst.GameOver();
                //TODO : Restart Level
            }
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (FindObjectOfType<PlayerController>() != null)
        {
            PlayerController.inst.OnPlayerMove -= Interact;
        }
    }
}
