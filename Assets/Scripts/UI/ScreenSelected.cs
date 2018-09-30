using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenSelected : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    [SerializeField] GameObject leftMenu;
    [SerializeField] Image lastGamesButtonImage;
    [SerializeField] Button[] buttons;
    [SerializeField] MenuObj[] menus;
    [SerializeField] GameObject[] Tables;
    [SerializeField] ScrollRect currentScroll;

    public static Color choosed = new Color(1f, 0.964f, 0.164f, 1);
    public static Color unChoosed = new Color(1, 1, 1, 1f);
    private const int enabledSpeed = 30;

    private Vector3 tapPos;
    public float deltaPosX;
    public float deltaPosY;
    public bool isScrolling = false;
    public bool isSliding = false;

    private const string greyHexColor = "#717686";

    private static int numMenu = 2;
    private static Vector3 menuPos = Vector3.zero;
    public float showX;

    void Start()
    {
        MenuObj.OnMenuChanged += MoveMenu;
        ColorUtility.TryParseHtmlString(greyHexColor, out unChoosed);
    }

    private void MoveMenu(Button obj)
    {
        MenuObj choosedMenu = obj.GetComponent<MenuObj>();
        StartCoroutine(Mooving(choosedMenu));
    }

    IEnumerator Mooving(MenuObj obj)
    {
        leftMenu.SetActive(false);

        for (int i = 0; i < Tables.Length; i++)
            Tables[i].SetActive(true);

        while (true)
        {
            if (Menu.transform.localPosition != obj.menuPosition)
                Menu.transform.localPosition = Vector3.MoveTowards(Menu.transform.localPosition, obj.menuPosition, Time.deltaTime * 5000);
            else
            {
                if (obj.menuNum == 2)
                    leftMenu.SetActive(true);

                numMenu = obj.menuNum;
                currentScroll = Tables[numMenu].transform.GetComponentInChildren<ScrollRect>();
                menuPos = Menu.transform.localPosition;
                OffOthersButton(obj);
                obj.mainImage.color = choosed;
                obj.bottomImage.gameObject.SetActive(true);
                StopAllCoroutines();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void OffOthersButton(MenuObj obj)
    {
        Button but = obj.Button;

        for (int i = 0; i < buttons.Length; i++)
        {
            MenuObj temp = buttons[i].GetComponent<MenuObj>();
            if (buttons[i] != but)
            {
                temp.mainImage.color = unChoosed;
                temp.bottomImage.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < Tables.Length; i++)
        {
            if (i != obj.menuNum)
                Tables[i].SetActive(false);
        }
    }
    
    public void MouseUp()
    {
        currentScroll.vertical = true;
        isScrolling = false;
        isSliding = false;
        deltaPosX = 0;
        deltaPosY = 0;
    }

    private void Update()
    {
        deltaPosX = EventSys.mouseDelta.x;
        deltaPosY = EventSys.mouseDelta.y;
    }

    public void SlideRightToLeft()
    {
        if (!isSliding && !isScrolling)
        {
            if (Mathf.Abs(deltaPosX) > Mathf.Abs(deltaPosY))
                isSliding = true;
            else
                isScrolling = true;
        }
        else if (isSliding)
        {
            currentScroll.vertical = false;
            Menu.transform.localPosition = new Vector3(Menu.transform.localPosition.x + deltaPosX * 0.3f, Menu.transform.localPosition.y, Menu.transform.localPosition.z);
        }
        else if (isScrolling)
            isSliding = false;
    }

    public void OnEndDragRight()
    {
        if (Menu.transform.localPosition.x - menuPos.x < -35f && numMenu < menus.Length - 1)
            numMenu++;
        else if (Menu.transform.localPosition.x > menuPos.x + 35f && numMenu > 0)
            numMenu--;
        else
            leftMenu.SetActive(true);

        MenuObj obj = menus[numMenu];
        StartCoroutine(Mooving(obj));
    }

    public void SetColor()
    {
        if (ParseFromHTML.isPast)
            lastGamesButtonImage.color = choosed;
        else
            lastGamesButtonImage.color = unChoosed;
    }

    void OnDisable()
    {
        MenuObj.OnMenuChanged -= MoveMenu;
    }
}
