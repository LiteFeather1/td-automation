#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenuItems
{
    [MenuItem("Scene Go/Main Menu")]
    public static void LoadMainMenu() => LoadScene("MainMenu");

    [MenuItem("Scene Go/Main")]
    public static void LoadMain() => LoadScene("Main");

    private static void LoadScene(string sceneName)
    {
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
    }
}
#endif
