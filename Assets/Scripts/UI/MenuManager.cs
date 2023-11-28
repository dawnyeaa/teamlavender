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
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject creditsCamera;
    [SerializeField] Button creditsButton, creditsIsland;
    [SerializeField] AudioClip menuSong, creditsSong;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator creditsAnimator;
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

    public void EnterCredits() {
        // change cameras
        mainCamera.SetActive(false);
        creditsCamera.SetActive(true);
        // change music
        audioSource.clip = creditsSong;
        audioSource.Play();
        // play animator
        creditsAnimator.SetTrigger("playCredits");
        // select island
        SelectButton(creditsIsland);
    }

    public void ExitCredits(bool fromAnimator = false) {
        // change cameras
        mainCamera.SetActive(true);
        creditsCamera.SetActive(false);
        // change music
        audioSource.clip = menuSong;
        audioSource.Play();
        // reset animator
        if (!fromAnimator)
            creditsAnimator.SetTrigger("exitCredits");
        // change menus
        SelectButton(creditsButton);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
