using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidingImage : MonoBehaviour {

    [SerializeField] RectTransform underTextRT;
    [SerializeField] RectTransform thisRT;
    [SerializeField] Image mainImage;
    [SerializeField] GameObject newsPrefab;

    private Vector3 newsPos;
    private Vector3 needPos = new Vector3(-2160, 0f, 0f);
    private Vector2 startSize;
    private Vector3 instPosition;

    private FullNewsPage fullPage;
    private Transform currentMenu;
    private GameObject BottomMenu;

    private GameObject MENU;
    private GameObject initNews;

    private float addHeight = 0;

    private NewsStruct choosenNews;

    private void Start()
    {
        MENU = GameObject.FindGameObjectWithTag("MENU");
        currentMenu = GameObject.FindGameObjectWithTag("NEWS").transform;
        BottomMenu = GameObject.FindGameObjectWithTag("BOTTOM");
        newsPos = currentMenu.transform.localPosition;
        StartCoroutine(wait());
    }

    public void Dragging()
    {
        thisRT.sizeDelta += new Vector2(-EventSys.mouseDelta.x, 0f) * 2.5f;
    }

    public void OnEndDrag()
    {
        if (thisRT.sizeDelta.x > 2500)
        {
            ShowNews();
        }
        else
            StartCoroutine(GetBack());
    }

    private void ShowNews()
    {
        BottomMenu.SetActive(false);
        choosenNews = thisRT.gameObject.GetComponentInParent<NewsStruct>();
        initNews = Instantiate(newsPrefab, MENU.transform);

        fullPage = initNews.GetComponent<FullNewsPage>();
        fullPage.mainImage.sprite = choosenNews.mainImage.sprite;
        fullPage.mainText.text = choosenNews.headerText.text;
        fullPage.backButton.onClick.AddListener(StartGetBack);

        fullPage.UpdatePage(choosenNews.link);

        Vector3 needsPos = new Vector3(-1080, 0, 0f);
        initNews.transform.localPosition = needsPos;
        StartCoroutine(GoToScreen());
    }
    
    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.05f);
        addHeight = underTextRT.sizeDelta.y / 2;
        thisRT.offsetMin = new Vector2(thisRT.offsetMin.x, thisRT.offsetMin.y - addHeight);
        startSize = thisRT.sizeDelta;
        instPosition = new Vector3(currentMenu.localPosition.x + 1080, 0f, currentMenu.localPosition.z);
        StopAllCoroutines();
    }

    IEnumerator GoToScreen()
    {
        while (initNews.transform.localPosition != needPos)
        {
            float distance = (initNews.transform.localPosition - needPos).magnitude;

            currentMenu.transform.localPosition = Vector2.MoveTowards(currentMenu.transform.localPosition,
                new Vector2(currentMenu.transform.localPosition.x - 1080, currentMenu.transform.localPosition.y),
                Time.deltaTime * (distance + 5) * 10);

            initNews.transform.localPosition = Vector2.MoveTowards(initNews.transform.localPosition, needPos, Time.deltaTime * (distance + 5) * 10);

            if (initNews.transform.localPosition.x <= needPos.x)
            {
                initNews.transform.localPosition = needPos;
                thisRT.sizeDelta = startSize;
                StopAllCoroutines();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void StartGetBack()
    {
        StartCoroutine(BackToNews());
    }

    IEnumerator GetBack()
    {
        while (thisRT.sizeDelta != startSize)
        {
            float distance = (thisRT.sizeDelta - startSize).magnitude;

            thisRT.sizeDelta = Vector2.MoveTowards(thisRT.sizeDelta, startSize, Time.deltaTime * (distance + 5) * 10);

            if (thisRT.sizeDelta == startSize)
                StopAllCoroutines();

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator BackToNews()
    {
        BottomMenu.SetActive(true);
        while (currentMenu.localPosition != needPos)
        {
            Destroy(initNews);
            float distance = (currentMenu.localPosition - needPos).magnitude;

            currentMenu.localPosition = Vector2.MoveTowards(currentMenu.localPosition, newsPos, Time.deltaTime * (distance + 5) * 10);

            if (currentMenu.transform.localPosition.x >= needPos.x)
                StopAllCoroutines();

            yield return new WaitForFixedUpdate();
        }
    }
}
