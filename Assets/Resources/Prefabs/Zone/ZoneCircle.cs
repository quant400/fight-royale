using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCircle : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float timeWait = 0.075f;
    [SerializeField] private List<GameObject> _listZone;
    private List<Vector3> _listResetZone;
    private Coroutine _runCoroutine;


    public void Run(Action onFinish = null)
    {
        this.gameObject.SetActive(true);
        target.SetActive(false);

        //Save Initial Position
        _listResetZone = new List<Vector3>();
        _listZone.ForEach(aux => _listResetZone.Add(aux.transform.position));



        _runCoroutine = StartCoroutine(ClosingZone(() =>
        {
            this.gameObject.SetActive(false);
            StopCoroutine(_runCoroutine);
            //Reset Position
            for (int i = 0; i < _listZone.Count; i++)
            {
                _listZone[i].transform.position = _listResetZone[i];
            }
            onFinish?.Invoke();

        }));
    }

    private void MoveZone(GameObject zone)
    {
        Vector3 moveDir = (target.transform.position - zone.transform.position).normalized;
        zone.transform.position += moveDir * timeWait;
    }

    private IEnumerator ClosingZone(Action onFinish)
    {
        var run = true;
        do
        {
            _listZone.ForEach(MoveZone);
            yield return new WaitForSeconds(timeWait / 2);
            if (Math.Abs(_listZone[0].transform.position.x - target.transform.position.x) < 1f)
            {
                run = false;
            }

        } while (run);

        onFinish?.Invoke();

    }

}
