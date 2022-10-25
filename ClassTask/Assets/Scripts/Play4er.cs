using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
public class Play4er : MonoBehaviour
{

    float Horizontal;
    float Vertical;
    Vector3 direction;
    [SerializeField] float MovementSpeed = 10;
    Rigidbody rb;
    SplineFollower splineFollower;
    Animator anim;
    Collider[] colliders;
    [SerializeField] Transform detectTransform;
    [SerializeField] float DetectionRange = 1;
    [SerializeField] LayerMask layer;
    [SerializeField] Transform holdTransform;
    [SerializeField] int itemCount = 0;
    [SerializeField] float ItemDistanceBetween = 0.1f;
    [SerializeField] List<GameObject> CollectedItems;



    float NextDropTime;
    [SerializeField] float DropRate = 1;
    [SerializeField] float DropSecond = 1;
    [SerializeField] Transform DropArea;
    int dropCount = 0;
    [SerializeField] float DropDistanceBetween = 1f;
   [SerializeField] GameObject[] toplanabilirler;
    bool isGround=true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        toplanabilirler=GameObject.FindGameObjectsWithTag("Collectable");
        var seq=DOTween.Sequence();
        foreach (var item in toplanabilirler)
        {
           
        item.transform.DORotate(new Vector3(0,90,0),1f).SetLoops(-1,LoopType.Incremental);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(detectTransform.position, DetectionRange);
    }

    void Update()
    {
        colliders = Physics.OverlapSphere(detectTransform.position, DetectionRange, layer);
        foreach (var hit in colliders)
        {

            if (hit.CompareTag("Collectable"))
            {
                Debug.Log(hit.name);
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;


                hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween, 0), 2, 1, 0.2f).OnComplete(() =>
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });


                var seq = DOTween.Sequence();

                seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.2f))
                    .Join(hit.transform.DOScale(0.3f, 0.2f));
                    
                seq.AppendCallback(() =>
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });
                itemCount++;
                CollectedItems.Add(hit.gameObject);
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            isGround=false;
        }


    }


    private void OnTriggerStay(Collider other)
    {
        anim.SetTrigger("Idle");
        Debug.Log("AAAAAAAAAAAAA");
        if (Time.time >= NextDropTime)
        {

            if (CollectedItems.Count > 0)
            {

                GameObject go = CollectedItems[CollectedItems.Count - 1];
                go.transform.parent = null;
                var Seq = DOTween.Sequence();
                Seq.Append(go.transform.DOJump(DropArea.position + new Vector3(0, (dropCount * DropDistanceBetween), 0), 2, 1, 0.3f))
                   .Join(go.transform.DOScale(1.5f, 0.1f))
                   .Insert(0.1f, go.transform.DOScale(1, 0.2f))
                   .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(0, 0, 0); });
                other.GetComponent<HoldArea>().StackedDropItems.Add(go);
                dropCount++;
                itemCount--;
                CollectedItems.Remove(go);
            }



            NextDropTime = Time.time + DropSecond / DropRate;

        }
        if (CollectedItems.Count == 0)
        {
            StartCoroutine(other.GetComponent<HoldArea>().SellDropedItems());
            dropCount = 0;

        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGround=true;
        }
    }
}
