using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GetSocialSdk.Capture.Scripts;
using TMPro;

public class NewUIScript : MonoBehaviour
{
    public Button Exit;
    public Button Explode;
    [SerializeField] private Slider VelSlider;
    [SerializeField] private TextMeshProUGUI VelValue;
    [SerializeField] private Slider YSlider;
    [SerializeField] private TextMeshProUGUI YValue;
    [SerializeField] private Slider XSlider;
    [SerializeField] private TextMeshProUGUI XValue;
    [SerializeField] private Slider ZSlider;
    [SerializeField] private TextMeshProUGUI ZValue;
    [SerializeField] private Slider DistSlider;
    [SerializeField] private TextMeshProUGUI DistValue;
    [SerializeField] private Toggle RenderMode;

    public TestShower explosion1;
    public GetSocialCapture capture;
    public Timer timer1;


    // Start is called before the first frame update
    void Start()
    {
        capture.captureMode = GetSocialCapture.GetSocialCaptureMode.Manual;
        VelValue.text = VelSlider.value.ToString("0.0");
        YValue.text = YSlider.value.ToString("0.0");
        XValue.text = XSlider.value.ToString("0.0");
        ZValue.text = ZSlider.value.ToString("0.0");
        DistValue.text = DistSlider.value.ToString("0.0");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (explosion1.finished)
        {
            if (RenderMode.isOn)
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

                string filepath = capture.GenerateCapture(result);
                if (filepath != "")
                {
                    Debug.Log(filepath);
                    MyMessageBox.ShowMessage("Успешно! Сгенерированный файл расположен по адресу " + filepath);
                }
                else
                {
                    MyMessageBox.ShowMessage("Ошибка! Не удалось сгенерировать файл");
                }
            }
            explosion1.started = false;
            explosion1.finished = false;
        }
    }

    public void ExplodeClicked()
    {
        timer1.SetTime();
        explosion1.Explode();
        if (RenderMode.isOn)
        {
            capture.StartCapture();
        }
    }
    public void ExitClicked()
    {
        Application.Quit();
    }

    public void VelSliderChanged(float value)
    {
        explosion1.vel_mult = value;
        VelValue.text = VelSlider.value.ToString("0.0");
    }
    public void YSliderChanged(float value)
    {
        explosion1.init_speed[3] = (int)value;
        YValue.text = YSlider.value.ToString("0.0");
    }
    public void XSliderChanged(float value)
    {
        explosion1.init_speed[1] = (int)value;
        explosion1.init_speed[0] = -(int)value;
        XValue.text = XSlider.value.ToString("0.0");
    }
    public void ZSliderChanged(float value)
    {
        explosion1.init_speed[5] = (int)value;
        explosion1.init_speed[4] = -(int)value;
        ZValue.text = ZSlider.value.ToString("0.0");
    }
    public void DistSliderChanged(float value)
    {
        explosion1.dist_prob = (int)value;
        DistValue.text = DistSlider.value.ToString("0.0");
    }
    public void ModeChanged(bool value)
    {
        if(value)
        {
            explosion1.maxCount = 1000; //НЕ ЗАБЫТЬ ВЕРНУТЬ 6000
        }
        else
        {
            explosion1.maxCount = 1000;
        }
    }
}
