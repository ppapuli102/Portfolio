using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance; 

    static int currentSceneIndex;

    public static void LoadMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    public static void QuitGame() {
        Application.Quit();
    }

    public static void LoadTown() {
        SceneManager.LoadScene("Town");
    }

    public static void LoadOverworld() {
        SceneManager.LoadScene("Overworld");
    }

    public static void LoadBattleScene() {
        SceneManager.LoadScene("Battle");
    }

    public static void LoadTestScene() {
        SceneManager.LoadScene("Test Encounter");
    }

    public static void LoadRandomEncounter() {
        SceneManager.LoadScene("Random Encounter");
    }

    public static void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

}
