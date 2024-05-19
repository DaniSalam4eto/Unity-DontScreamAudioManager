using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public  class UnityDontScreamAudioManager : MonoBehaviour
{
    [Header("UI Settings ")]
    public TMP_Dropdown microphoneDropdown;
    public Slider volumeSlider;
    public TMP_Text volumeLabel;
    [Header("Sensitivity Settings")]
    [Range(0.1f, 50f)] public float sensitivity = 10.0f; // Increased sensitivity adjustment from the Inspector
    [Range(0f, 1f)] public float threshold = 0.75f; // Threshold for triggering the function

    private AudioClip microphoneClip;
    private string selectedMicrophone;
    private int sampleRate = 44100;
    private float[] samples;
    private const int sampleDataLength = 1024;
    private bool thresholdCrossed = false;

    // Peak detection variables
    private const int peakSampleCount = 5; // Number of samples to consider for peak detection
    private float[] previousSamples;
    private float peakThreshold = 0.2f; // Adjust as needed for sensitivity
    private float peakSensitivity = 5.0f; // Adjust as needed for sensitivity

    void Start()
    {
        PopulateMicrophoneDropdown();
        microphoneDropdown.onValueChanged.AddListener(delegate { OnMicrophoneSelected(); });
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 1;

        // Load selected microphone from PlayerPrefs
        string savedMicrophone = PlayerPrefs.GetString("SelectedMicrophone", null);
        if (!string.IsNullOrEmpty(savedMicrophone) && Microphone.devices.Contains(savedMicrophone))
        {
            selectedMicrophone = savedMicrophone;
            microphoneDropdown.value = Microphone.devices.ToList().IndexOf(savedMicrophone);
            StartMicrophone(selectedMicrophone);
        }
        else if (Microphone.devices.Length > 0)
        {
            selectedMicrophone = Microphone.devices[0];
            StartMicrophone(selectedMicrophone);
        }

        // Initialize peak detection variables
        previousSamples = new float[peakSampleCount];
        for (int i = 0; i < peakSampleCount; i++)
        {
            previousSamples[i] = 0f;
        }

        // Start checking audio volume every 5 milliseconds
        InvokeRepeating("CheckMicrophoneVolume", 0f, 0.005f);
    }

    void CheckMicrophoneVolume()
    {
        if (Microphone.IsRecording(selectedMicrophone))
        {
            UpdateMicrophoneVolume();
            DetectAudioPeak();
        }
    }

    private void PopulateMicrophoneDropdown()
    {
        microphoneDropdown.ClearOptions();
        var options = Microphone.devices.ToList();
        microphoneDropdown.AddOptions(options);
    }

    private void OnMicrophoneSelected()
    {
        selectedMicrophone = microphoneDropdown.options[microphoneDropdown.value].text;
        PlayerPrefs.SetString("SelectedMicrophone", selectedMicrophone); // Save selected microphone to PlayerPrefs
        StartMicrophone(selectedMicrophone);
    }

    private void StartMicrophone(string microphone)
    {
        if (Microphone.IsRecording(selectedMicrophone))
        {
            Microphone.End(selectedMicrophone);
        }
        microphoneClip = Microphone.Start(microphone, true, 1, sampleRate);
        samples = new float[sampleDataLength];
    }

    private void UpdateMicrophoneVolume()
    {
        microphoneClip.GetData(samples, 0);
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        float rmsValue = Mathf.Sqrt(sum / samples.Length) * sensitivity; // Apply increased sensitivity adjustment
        rmsValue = Mathf.Clamp(rmsValue, 0, 1); // Clamp the value to be between 0 and 1

        volumeSlider.value = rmsValue;
        volumeLabel.text = $"Volume: {rmsValue:F2}";

        if (rmsValue > threshold && !thresholdCrossed)
        {
            thresholdCrossed = true;
            OnVolumeThresholdCrossed();
        }
        else if (rmsValue < threshold)
        {
            thresholdCrossed = false;
        }
    }

    private void DetectAudioPeak()
    {
        // Calculate current peak value
        float peakValue = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            peakValue = Mathf.Max(peakValue, Mathf.Abs(samples[i]));
        }

        for (int i = 0; i < peakSampleCount - 1; i++)
        {
            previousSamples[i] = previousSamples[i + 1];
        }
        previousSamples[peakSampleCount - 1] = peakValue;

        // Check if current peak exceeds previous samples
        bool peakDetected = true;
        for (int i = 0; i < peakSampleCount - 1; i++)
        {
            if (previousSamples[i] >= previousSamples[peakSampleCount - 1] * peakSensitivity)
            {
                peakDetected = false;
                break;
            }
        }

        // If peak detected, trigger action
        if (peakDetected)
        {
            OnAudioPeakDetected();
        }
    }

    private void OnAudioPeakDetected()
    {
        Debug.Log("Audio peak detected!");
        // Put your action here
    }

    private void OnVolumeThresholdCrossed()
    {
        Debug.Log("Volume threshold crossed!");
        // Put your function here
    }

    private void OnDestroy()
    {
        if (Microphone.IsRecording(selectedMicrophone))
        {
            Microphone.End(selectedMicrophone);
        }
    }
}
