using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MysteryBoxScript : MonoBehaviour
{
    /*
    public GameObject lootBox;

    public GameObject lootReward;
    */
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private LockerRoomManager lockerRoomManager;

    [SerializeField]
    private LockerRoomAPI lockerRoomApi;

    private Vector3 originalPositionMysteryBox;
    private Quaternion originalRotationMysteryBox;

    private Vector3 originalPositionLoot;
    private Quaternion originalRotationLoot;

    private RectTransform rectTransform;

    [SerializeField]
    private GameObject loot;

    /*
    private RaycastHit hit;

    private Ray ray;
    */

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        originalPositionMysteryBox = rectTransform.transform.localPosition;
        originalRotationMysteryBox = rectTransform.transform.localRotation;

        originalPositionLoot = loot.GetComponent<RectTransform>().transform.localPosition;
        originalRotationLoot = loot.GetComponent<RectTransform>().transform.localRotation;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        rectTransform.transform.localPosition = originalPositionMysteryBox;
        rectTransform.transform.localRotation = originalRotationMysteryBox;

        loot.GetComponent<RectTransform>().transform.localPosition = originalPositionLoot;
        loot.GetComponent<RectTransform>().transform.localRotation = originalRotationLoot;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hitting");

            if(hit.transform.gameObject == gameObject.transform.GetChild(0).gameObject)
            {
                animator.SetBool("isIdle", false);
                animator.SetBool("isHover", true);

                if(Input.GetMouseButtonDown(0))
                {
                    animator.SetBool("isOpened", true);
                }
            }
            else
            {
                animator.SetBool("isIdle", true);
                animator.SetBool("isHover", false);
            }
        }*/
    }

    
    public void LootReward()
    {
        lockerRoomApi.MintWearable(lockerRoomManager.currentCharacter.nftID.ToString(), "rand");

        StartCoroutine(LootOpeningAnimation());
    }

    IEnumerator LootOpeningAnimation()
    {
        //loot.GetComponent<RectTransform>().DOLocalMoveY(loot.GetComponent<RectTransform>().position.y + 156, 1f);


        //loot.GetComponent<RectTransform>().DOLocalMoveZ(loot.GetComponent<RectTransform>().position.z - 221, 2f);


        yield return new WaitForSeconds(0.2f);

        animator.SetBool("isOpen", true);


        float origY = loot.GetComponent<RectTransform>().localPosition.y;

        float origZ = loot.GetComponent<RectTransform>().localPosition.z;

        loot.GetComponent<Image>().enabled = true;

        loot.GetComponent<RectTransform>().localScale = Vector3.zero;


        Debug.Log("origY = " + origY);

        Debug.Log("origZ = " + origZ);

        yield return new WaitForSeconds(0.1f);

        loot.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 0.5f);

        loot.GetComponent<RectTransform>().DOLocalMoveY(origY + 156, 0.5f);

        loot.GetComponent<RectTransform>().DOLocalMoveZ(origZ - 221, 1f);
    }



}
