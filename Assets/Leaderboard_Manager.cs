using CFC.Serializable.Leaderboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard_Manager : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private ScoreItem ScoreItem_Prefab;
    [SerializeField] private TypeScoreboard Type;

    [SerializeField] private List<GameObject> Slides;

    private Coroutine slide_Coroutine;

    public void OnCreate(List<Scoreboard> scoreboards)
    {
        
        var i = 1;
        ClearContent();
        StartActionSlide();

        foreach (var item in scoreboards)
        {
            if (i > GetLimite())
            {
                break;
            }

            if (Type == TypeScoreboard.AllTime)
                ScoreItem_Prefab.OnCreate(i.ToString(), item.id.ToString(), item.name.ToString(), item.allTimeScore.ToString());  
            else
                ScoreItem_Prefab.OnCreate(i.ToString(), item.id.ToString(), item.name.ToString(), item.dailyScore.ToString());
                
            Instantiate(ScoreItem_Prefab.gameObject, Content);
            i++;
        }

    }

    private int GetLimite()
    {
        switch (Type)
        {
            case TypeScoreboard.AllTime:
                return 21;
            case TypeScoreboard.Daily:
                return 10;
            default:
                return 1;
        }
    }

    private void ClearContent()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
    }

    private void StartActionSlide()
    {
        if (Slides != null && Slides.Count > 0)
        {
            if (slide_Coroutine == null)
            {
                Slides.ForEach(aux => aux.SetActive(false));
                slide_Coroutine = StartCoroutine(ActionSlide());
            }
            else
            {
                StopCoroutine(ActionSlide());
                slide_Coroutine = null;
            }
        }
        else
            Debug.Log("Cancel Slide");

        
    }
    private IEnumerator ActionSlide()
    {
        int i = 0;
        int loop = 0;
        int maxLoop = 32;

        do
        {
            Slides[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(Random.Range(8, 16));
            Slides[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.32f);

            if (i >= (Slides.Count-1) )
            {
                i = 0;
                loop++;
            }
            else
                i++;


        } while (loop < maxLoop);
            
    }





}
