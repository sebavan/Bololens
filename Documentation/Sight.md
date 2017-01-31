Sight
=====

## Summary
**From Wikipedia:** *Visual perception is the ability to interpret the surrounding environment using light in the visible spectrum reflected by the objects in the environment.*

**From us:** This is the component responsible of the bot vision. It helps us capturing and sending picture to the bot framework. The main difference with a real Sight is it represents the player's sight and not the bot one.

## Unity
The Sight is represented in Untiy by the BotSightManager.cs behaviour.

## Configuration
![Configuration](blob/master/Documentation/Pictures/Sight.png)

1. Built In Type: chose amongst the different allowed built in type of supported camera capture API.
2. Custom Caracteristic: you can specify you own behaviour by extending the BaseBotSight and referencing the game object hosting your capability here.