using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSelected : MonoBehaviour {
    
    [SerializeField] GameObject Menu;
    [SerializeField] Image lastGamesButtonImage;
    [SerializeField] Button[] buttons;
    [SerializeField] MenuObj[] menus;
    [SerializeField] GameObject[] Tables;

    public static Color choosed = new Color(1f, 0.964f, 0.164f, 1);
    public static Color unChoosed = new Color(1, 1, 1, 0.5f);

    private static int numMenu = 0;
    private static Vector3 menuPos = Vector3.zero;

    void Start () {
        MenuObj.OnMenuChanged += MoveMenu;
	}

    private void MoveMenu(Button obj)
    {
        MenuObj choosedMenu = obj.GetComponent<MenuObj>();
        StartCoroutine(Mooving(choosedMenu));
    }

    IEnumerator Mooving(MenuObj obj)
    {
        for (int i = 0; i < Tables.Length; i++)
            Tables[i].SetActive(true);

        while (true)
        {
            if (Menu.transform.localPosition != obj.menuPosition)
                Menu.transform.localPosition = Vector3.MoveTowards(Menu.transform.localPosition, obj.menuPosition, Time.deltaTime * 5000);
            else
            {
                numMenu = obj.menuNum;
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

    public void SlideRightToLeft()
    {
        float tapPos = Input.mousePosition.x - 1080 + menuPos.x * 10;
        Menu.transform.localPosition = new Vector3(tapPos, Menu.transform.localPosition.y, Menu.transform.localPosition.z) / 10f;
    }

    public void SlideLeftToRight()
    {
        float tapPos = Input.mousePosition.x + menuPos.x * 10;
        Menu.transform.localPosition = new Vector3(tapPos, Menu.transform.localPosition.y, Menu.transform.localPosition.z) / 10f;
    }

    public void OnEndDragRight()
    {
        if (Menu.transform.localPosition.x - menuPos.x < -50f && numMenu < 2)
            numMenu++;

        MenuObj obj = menus[numMenu];
        StartCoroutine(Mooving(obj));
    }

    public void OnEndDragLeft()
    {
        if (Menu.transform.localPosition.x > menuPos.x + 50f && numMenu > 0)
            numMenu--;

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
