Hearing
========

## Summary
**From Wikipedia:** *Hearing, auditory perception, or audition is the ability to perceive sound by detecting vibrations, changes in the pressure of the surrounding medium through time, through an organ such as the ear.*

**From us:** This is the component responsible to catch the input from the bot. It is usually a Speech to Text mecanism but could be simply implemented as a UI like in the editor if necessary.

## Unity
The Hearing is represented in Untiy by the BotHearingManager.cs behaviour.

## Configuration
![Configuration](blob/master/Documentation/Pictures/Hearing.png)

1. Built In Type: chose amongst the different allowed built in type of supported speech to text API.
2. Custom Caracteristic: you can specify you own behaviour by extending the BaseBotHearing and referencing the game object hosting your capability here.
3. Silence Timeout In Seconds: time to wait before timing out if no speech have been pronounced.

## Details
The hearing system currently implemented is not supported through the editor. In order to still authorize fast iteration and development, if the bot runs in the editor, a GUI is available to allow entering message as well as launching action as people would do in the deployed application through voice interaction.   