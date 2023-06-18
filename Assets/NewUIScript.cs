using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GetSocialSdk.Capture.Scripts;

public class NewUIScript : MonoBehaviour
{
    public Button Exit;
    public Button Explode;

    public TestShower explosion1;
    public GetSocialCapture capture;
    public Timer timer1;


    // Start is called before the first frame update
    void Start()
    {
        capture.captureMode = GetSocialCapture.GetSocialCaptureMode.Manual;
    }

    // Update is called once per frame
    void Update()
    {
        if (explosion1.finished)
        {
            //Debug.Log("UI thinks sim is done");
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

    public void ExplodeClicked()
    {
        timer1.SetTime();
        explosion1.Explode();
        capture.StartCapture();
    }
    public void ExitClicked()
    {
        Application.Quit();
    }
}
