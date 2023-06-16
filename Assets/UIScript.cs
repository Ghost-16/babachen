using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GetSocialSdk.Capture.Scripts;


public class UIScript : MonoBehaviour
{
    public TestShower explosion1;
    //public TestShower explosion2;
    public Timer timer1;

    public GetSocialCapture capture;   //Запись экрана

    private void OnEnable()
    {

        capture.captureMode = GetSocialCapture.GetSocialCaptureMode.Manual;    //Запись экрана


        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button explode = root.Q<Button>("ExplButton");
        Button quit = root.Q<Button>("QuitButton");

        explode.clicked += () => explosion1.Explode();

        explode.clicked += () => capture.StartCapture();  //Запись экрана

        //explode.clicked += () => explosion2.Explode();
        explode.clicked += () => timer1.SetTime();
        quit.clicked += () => Application.Quit();
    }
    
    void Update()
    {
        if (explosion1.finished == true)
        {
            capture.StopCapture();
            // generate gif
            System.Action<byte[]> result = bytes =>
            {
                // generated gif returned as byte[]
                byte[] gifContent = bytes;

                // use content, like send it to your friends by using GetSocial Sdk
            };

            capture.GenerateCapture(result);
            explosion1.started = false;
            explosion1.finished = false;
        }
    }
}
