using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIGenerator : MonoBehaviour
{
    public GameObject bulletUI;
    public Transform bulletUIParent;
    public GameObject targetBulletUI;
    public List<GameObject> uiList;

    Color truthBulletColor = Color.green;
    Color falseBulletColor = Color.red;
    Color mirrBulletColor = Color.gray;

    int posX = 895, posY = -430; // -60씩

    public void GenerateBulletUI(BulletCode code)
    {
        GameObject bulletUIInst = Instantiate(bulletUI, bulletUIParent);
        bulletUIInst.transform.localPosition = new Vector3(posX, posY);
        posX -= 60;

        switch(code)
        {
        case BulletCode.True:
            bulletUIInst.GetComponent<Image>().color = truthBulletColor;
            break;
        case BulletCode.False:
            bulletUIInst.GetComponent<Image>().color = falseBulletColor;
            break;
        case BulletCode.Mirror:
            bulletUIInst.GetComponent<Image>().color = mirrBulletColor;
            break;
        default:
            Debug.Log("이상한 불릿 코드임");
            break;
        }

        uiList.Add(bulletUIInst);
        targetBulletUI.SetActive(true);
    }


    public void RemoveBulletUI()
    {
        GameObject shootedBullet = uiList[0];
        Destroy(shootedBullet);
        uiList.RemoveAt(0);
        posX += 60;

        if (uiList.Count == 0)
        {
            targetBulletUI.SetActive(false);
        }

        foreach (var ui in uiList)
        {
            ui.transform.localPosition += new Vector3(60, 0);
        }
    }
}
