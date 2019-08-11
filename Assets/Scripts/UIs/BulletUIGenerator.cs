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

    [Header("Bullet Images")]
    public Sprite truthBullet;
    public Sprite falseBullet;
    public Sprite mirrBullet;

    int shootPosX = 895, shootPosY = -390;
    int posX = 822, posY = -430; // -55씩

    public void GenerateBulletUI(BulletCode code)
    {
        GameObject bulletUIInst = Instantiate(bulletUI, bulletUIParent);
        if (uiList.Count == 0)
        {
            bulletUIInst.transform.localPosition = new Vector3(shootPosX, shootPosY);
        }
        else
        {
            bulletUIInst.transform.localPosition = new Vector3(posX, posY);
            posX -= 55;
        }

        switch(code)
        {
        case BulletCode.True:
                bulletUIInst.GetComponent<Image>().sprite = truthBullet;
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.text = "진실탄";
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.color = Color.green;
                bulletUIInst.GetComponent<BulletHoverUI>().bodyText.text = "거울, 터렛을 파괴함\n초록 서류가방에서 제공";
                break;
        case BulletCode.False:
                bulletUIInst.GetComponent<Image>().sprite = falseBullet;
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.text = "거짓탄";
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.color = Color.red;
                bulletUIInst.GetComponent<BulletHoverUI>().bodyText.text = "거울의 상을 실제로 만듦\n빨간 서류가방에서 제공";
                break;
        case BulletCode.Mirror:
                bulletUIInst.GetComponent<Image>().sprite = mirrBullet;
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.text = "거울탄";
                bulletUIInst.GetComponent<BulletHoverUI>().headerText.color = Color.gray;
                bulletUIInst.GetComponent<BulletHoverUI>().bodyText.text = "일반 벽을 거울로 만듦\n회색 서류가방에서 제공";
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
        posX += 55;

        if (uiList.Count == 0)
        {
            posX -= 55;
            targetBulletUI.SetActive(false);
        }

        for (int i = 0; i < uiList.Count; i++)
        {
            if (i == 0)
            {
                uiList[i].transform.localPosition = new Vector3(shootPosX, shootPosY);
            }
            else
            {
                uiList[i].transform.localPosition += new Vector3(55, 0);
            }
        }
    }
}
