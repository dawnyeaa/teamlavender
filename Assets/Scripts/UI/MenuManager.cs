using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] menus; //mainMenu, settingsMenu, controlsMenu, characterMenu;
    [SerializeField] ButtonSelectionHandler[] menuDefaults;
    [SerializeField] int currentMenu;
    [SerializeField] Animator menuAnimator;
    [SerializeField] BoardCustoFlowManager boardCustoFlowManager;
    private EventSystem eventsys;

    void Awake() {
        eventsys = FindObjectOfType<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (BoardCustoFlowManager.returningFromBoardCusto) {
            BoardCustoFlowManager.returningFromBoardCusto = false;
            menuAnimator.SetTrigger("returnFromBoardCusto");
            ChangeMenu(3);
        }
    }

    public void EnableMenu() 
    {
        ChangeMenu(0);
    }

    public void ChangeMenu(int menu)
    {
        // menus[currentMenu].SetActive(false);
        currentMenu = menu;
        // menus[menu].SetActive(true);
        if (menuDefaults.Length > menu)
            menuDefaults[menu].ManualSelect();
        menuAnimator.SetInteger("menu", menu);
    }

    public void SelectButton(Button button) {
        eventsys.SetSelectedGameObject(button.gameObject);
        button.Select();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
