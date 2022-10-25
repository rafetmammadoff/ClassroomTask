using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class HoldArea : MonoBehaviour
{
    // Start is called before the first frame update
    public  List<GameObject> StackedDropItems;

    void Start()
    {
        StackedDropItems = new List<GameObject>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     public IEnumerator SellDropedItems()  // Ienumerator.MoveNext()
    {
        if (StackedDropItems.Count>1)
        {
            foreach (var item in StackedDropItems)
            {
                yield return new WaitForSeconds(0.05f);
                item.transform.DOScale(1.5f, 0.1f).SetLoops(2, LoopType.Yoyo);
            }
        }

        while (StackedDropItems.Count > 1)
        {
            yield return new WaitForSeconds(0.5f);
            //GameObject go = StackedDropItems.Pop();
            GameObject go;
           
             if (StackedDropItems.Count!=0)
             {

                go =StackedDropItems[StackedDropItems.Count - 1];
                StackedDropItems.Remove(go);
                go.transform.DOScale(0, 0.3f);
             }
            
        }
    }
}
