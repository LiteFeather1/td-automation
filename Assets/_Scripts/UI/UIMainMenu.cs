using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _quitButton;

    [SerializeField] private TextMeshProUGUI t_version;

    private void Awake()
    {
        t_version.text = Application.version;

#if UNITY_WEBGL
        _quitButton.SetActive(false);
#endif
    }

    public void ButtonNewGame()
    {
        SceneManager.LoadScene(SceneNamesIDs.MAIN_ID);
    }

    public void ButtonSetCreditsState(bool state)
    {
        _credits.SetActive(state);
    }

    public void ButtonQuit()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
            UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
