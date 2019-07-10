using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatteredMirror : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Smaller());
    }
    
    IEnumerator Smaller()
    {
        yield return new WaitForSeconds(4f);
        for (int i = 100; i > 0; i--)
        {
            Vector3 scale = new Vector3(i,i,i);
            for (int j = 0; j < 100; j++) transform.GetChild(j).transform.localScale = scale;
            yield return new WaitForSeconds(0.03f);
        }
        Destroy(gameObject);
    }
}
