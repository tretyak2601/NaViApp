using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowList : MonoBehaviour {

    [SerializeField] GameObject List;
    [SerializeField] GameObject showButton;
    [SerializeField] Image Blur;
    [SerializeField] Image choosenGame;

    private bool isOnPosition = false;
    private static Vector3 mainPosition;
    private static Vector3 rightPosition;

	void Start () {
        mainPosition = new Vector3(List.transform.localPosition.x, List.transform.localPosition.y, List.transform.localPosition.z);
        rightPosition = new Vector3(List.transform.localPosition.x + 250, List.transform.localPosition.y, List.transform.localPosition.z);
    }

    private IEnumerator Mooving()
    {
        showButton.SetActive(false);
        Blur.gameObject.SetActive(true);
        while (!isOnPosition)
        {
            List.transform.localPosition = Vector3.MoveTowards(List.transform.localPosition, rightPosition, Time.deltaTime * 1000f);

            if (List.transform.localPosition == rightPosition)
            {
                isOnPosition = true;
                StopAllCoroutines();
            }

            yield return isOnPosition;
        } 
    }

    private IEnumerator GetBack()
    {
        showButton.SetActive(true);
        Blur.gameObject.SetActive(false);
        while (isOnPosition)
        {
            List.transform.localPosition = Vector3.MoveTowards(List.transform.localPosition, mainPosition, Time.deltaTime * 1000f);

            if (List.transform.localPosition == mainPosition)
            {
                isOnPosition = false;
                StopAllCoroutines();
            }

            yield return isOnPosition;
        }
    }

    public void StartCour()
    {
        if (!isOnPosition)
            StartCoroutine(Mooving());
    }

    public void StartBack()
    {
        if (isOnPosition)
            StartCoroutine(GetBack());
    }
}
