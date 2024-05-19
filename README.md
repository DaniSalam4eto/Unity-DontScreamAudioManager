# Don't Scream Audio Manager

## Overview
The Don't Scream Audio Manager script provides functionality to manage microphone input in Unity. It includes features for selecting a microphone, monitoring volume levels, and detecting audio peaks.

## Features
- Microphone selection via dropdown menu.
- Real-time volume monitoring with a slider and label.
- Threshold crossing detection for volume levels.
- Audio peak detection for sudden spikes in volume.

## Setup
1. Attach the `MicrophoneManager` script to a GameObject in your Unity scene.
2. Assign UI elements to the script's public variables:
   - `microphoneDropdown`: A TMP_Dropdown UI element for selecting microphones.
   - `volumeSlider`: A Slider UI element for displaying volume levels.
   - `volumeLabel`: A TMP_Text UI element for displaying volume information.
3. Adjust sensitivity and threshold settings in the Inspector as needed.
4. Run the scene.

## Usage
1. Launch the Unity scene.
2. Use the dropdown menu to select an available microphone.
3. Adjust the volume slider to monitor microphone input volume.
4. Threshold crossing events and audio peak detections will trigger corresponding actions, which can be customized in the script.

## Notes
- Ensure microphone permissions are granted on the target platform (especially for mobile).
- Customize peak detection and volume threshold settings to suit your application's requirements.
- Consider optimizing sensitivity settings for different microphone setups and environments.

## Dependencies
- Unity 3D (compatible with Unity 20XX)
- TextMeshPro (TMP) package for UI elements

## Credits
This script was developed by [Daniel Chakarov/Sleeping Impulse].
