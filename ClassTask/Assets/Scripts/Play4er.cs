using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
public class Play4er : MonoBehaviour
{

    float Horizontal;
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
    bool isGround=true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        var seq=DOTween.Sequence();
        
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
                var cube = hit.GetComponent<Chicken>();
                cube.seq.Kill(true);
                hit.transform.GetChild(2).gameObject.SetActive(false);
                Debug.Log(hit.name);
                hit.tag = "Collected";
                hit.transform.parent = holdTransform;


                hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween, 0), 2, 1, 0.2f).OnComplete(() =>
                {
                    hit.transform.localRotation = Quaternion.Euler(0, 0, 0);
                });


                var seq = DOTween.Sequence();

                seq.Append(hit.transform.DOLocalJump(new Vector3(0, itemCount * ItemDistanceBetween), 2, 1, 0.2f))
                    .Join(hit.transform.DOScale(0.01f, 0.2f));
                    
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

        Horizontal = Input.GetAxis("Horizontal");
        if(transform.localPosition.x+Horizontal>-3 && transform.localPosition.x + Horizontal <3)
        {
            transform.localPosition += new Vector3(Horizontal * 5, 0, 0) * Time.deltaTime;
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
                Seq.Append(go.transform.DOJump(DropArea.position + new Vector3((dropCount * DropDistanceBetween),0, 0), 2, 1, 0.3f))
                   .Join(go.transform.DOScale(0.05f, 0.1f))
                   .Insert(0.1f, go.transform.DOScale(0.02f, 0.2f))
                   .AppendCallback(() => { go.transform.rotation = Quaternion.Euler(0, 0, 0); });
                dropCount++;
                itemCount--;
                CollectedItems.Remove(go);
            }
            else
            {
                anim.SetTrigger("Dance");
            }



            NextDropTime = Time.time + DropSecond / DropRate;

        }
        

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGround=true;
        }

        
        if (collision.transform.CompareTag("Engel"))
        {
            Debug.Log("Oldu");
            foreach (var item in CollectedItems)
            {
                Destroy(item);
            }
            transform.parent.GetComponent<SplineFollower>().followSpeed = 0f;
            anim.SetTrigger("Dead");
        }
    }
}
