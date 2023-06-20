using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyMessageBox : MonoBehaviour
{
    private static MyMessageBox instance;
    public GameObject Template;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public static void ShowMessage(string message)
    {
        Debug.Log(message);
        GameObject messageBox = Instantiate(instance.Template);
        // Set as child of the Screen object
        //messageBox.transform.parent = instance.canvas.transform;

        Transform panel = messageBox.transform.Find("Panel");

        TextMeshProUGUI messageBoxText = panel.Find("Text").GetComponent<TextMeshProUGUI>();
        messageBoxText.text = message;

        Button ok = panel.Find("OKButton").GetComponent<Button>();
        ok.onClick.AddListener(() =>
        {
            Destroy(messageBox);
        });
    }
}
