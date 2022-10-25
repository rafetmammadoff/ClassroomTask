using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Chicken : MonoBehaviour
{
    // Start is called before the first frame update
   public Sequence seq;
    void Start()
    {
        seq = DOTween.Sequence();
       seq.Append( transform.DORotate(new Vector3(0, 90, 0), 1f).SetLoops(-1, LoopType.Incremental));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
