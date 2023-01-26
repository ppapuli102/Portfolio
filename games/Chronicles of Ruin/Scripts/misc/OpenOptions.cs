using UnityEngine;

public class OpenOptions : MonoBehaviour
{
    public Canvas OptionsCanvas;
    public Canvas MainMenu;

    public void OpenMenu () {
        OptionsCanvas.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false);
    }
}
