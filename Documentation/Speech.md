Speech
======

## Summary
**From Wikipedia:** *Visual perception is the ability to interpret the surrounding environment using light in the visible spectrum reflected by the objects in the environment.*

**From us:** This is the component responsible of the text to speech conversion inthe application. It helps reading out loud the messages receveived byt the bot.

## Unity
The Speech is represented in Untiy by the BotSpeechManager.cs behaviour.

## Configuration
![Configuration](Documentation/Pictures/Speech.png)

1. Built In Type: chose amongst the different allowed built in type of supported Text to Speech API.
2. Custom Caracteristic: you can specify you own behaviour by extending the BaseBotSpeech and referencing the game object hosting your capability here.