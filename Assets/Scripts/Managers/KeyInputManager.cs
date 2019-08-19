using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputManager : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.inst.GameOver(true);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.inst.isPlayerMoving && !GameManager.inst.isZooming)
        {
            if (GameManager.inst.isPlayerShooting)
            {
                StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomOutFromPlayer(PlayerController.inst.currentPlayer));
            }
            else if (PlayerController.inst.currentPlayer != null)
            {
                StartCoroutine(PlayerController.inst.currentPlayer.CountPlayerClick(Time.time - 1f, true));
            }
        }
    }
}
