# UtopicSense

UtopicSense is a Unity-based tool that transforms audio signals into real-time visual feedback to improve accessibility in games.

## Overview

UtopicSense is based on the concept of algorithmic synesthesia, where audio and visual outputs are generated from the same data source. The goal is to make sound-driven information perceptible through alternative sensory channels, especially for deaf and hard-of-hearing players.

The system analyzes audio sources in real time and maps their properties into visual particle effects, allowing developers to integrate accessibility features with minimal changes to existing workflows.

## Features

- Real-time audio analysis (frequency, amplitude, spatial range)
- Audio-to-visual mapping using particle systems
- Customizable shaders and textures
- Unity editor integration with intuitive interface
- Multiple mapping modes (standard and simplified)

## How It Works

The core pipeline follows this structure:

- Frequency → Color  
- Volume → Opacity  
- Spatial Range → Effect Area  

## Installation

1. Import the package into your Unity project  
2. Add the UtopicSense component to your scene  
3. Assign audio sources  
4. Configure effects through the custom editor interface  

## Demo

- 🎥 Demo Video: [link](https://www.youtube.com/watch?v=_qz4LqVwwqs&t=4s)  
- 📄 Paper (SBGames 2021): [link](https://www.sbgames.org/proceedings2021/ComputacaoFull/218004.pdf)
- 🛒 Unity Asset Store: [link](https://assetstore.unity.com/packages/tools/particles-effects/utopicsense-216506?srsltid=AfmBOoqaprWpUu7WZ4KKV1sBC00fi3ont1cl62KfmmijLYSGNo2xHTxj)  

## Research Background

This project was developed as part of academic research on game accessibility and multimodal interaction. It received the Best Full Paper Award at SBGames 2021.

## Future Work

- Integration of haptic feedback (controller vibration)  
- 3D and spatial audio visualization  
- Multimodal interaction systems (visual + haptic)  
- Integration with VR environments  
- User studies with accessibility-focused audiences  

## License

MIT License

## Author

Adriel Silva
