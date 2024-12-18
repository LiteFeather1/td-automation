using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _credits;

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
