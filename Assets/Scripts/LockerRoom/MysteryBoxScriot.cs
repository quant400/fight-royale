using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxScriot : MonoBehaviour
{
    public GameObject lootBox;

    public GameObject lootReward;

    private Animator animator;

    private RaycastHit hit;

    private Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
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
        }
    }

    public void LootReward()
    {
        var loot = Instantiate(lootReward) as GameObject;

        Destroy(lootBox);
    }
}
