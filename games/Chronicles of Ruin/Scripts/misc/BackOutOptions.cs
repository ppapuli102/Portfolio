using UnityEngine;

public class BackOutOptions : MonoBehaviour
{
    public Canvas OptionsMenu2;
    public Canvas MainMenu2;

    public void ExitMenu() {
        OptionsMenu2.gameObject.SetActive(false);
        MainMenu2.gameObject.SetActive(true);
    }
}
