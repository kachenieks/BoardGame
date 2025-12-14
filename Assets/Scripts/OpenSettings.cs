using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    public GameObject settingsPanel;

    public void Open()
    {
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
    }
}
