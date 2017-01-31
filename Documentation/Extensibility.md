Extensibility
=============

## Summary
All of the bot caracteristic have been developped with extensibility in mind. Two paths for extension are available in most of the component (except for the brain who orchestrates the other ones). Before introducing the different configuration ways, let's take a closer look ar the caracteristics and their managers.

![Personality](Documentation/Pictures/Personality.png)

## Base Caracteristic
Each caracteristic is creating from an abstract base caracteristic. This level of abastraction helps implementing decoupled version of the caracteristics. One could think of text to speech being represented either by the UWP one in the application or by a simple log in the editor. Going further it would be easy to add a new Bing based one if we need without having to code back the entire application.

## Caracteristic manager
All the carateristics are represented by so called manager in the application. Unlike the usual Unity manager who are Singleton, those manager are more acting like factory responsible of instantiating the configured Caracteristic. In case of the personnality for instance, depending on the enum choice against the built-in available ones (Crazy, Neutral...) the corresponding concrete implementation of the personnality would be instanted and attached to the bot at run-time.

## Custom
As the caracteristic manager is responsible of "spawing senses" in the bot, a non intrusive way for extension is to inherit from a base caracteristic in your app and attach it to the custom section of the manager, in this case you do not have to modify the code of the bot client itself but you can easily add any wished behaviour. 

## BuiltIn
In order to exppand the BuiltIn functionality, you will usually have to first create your caracteristic, add a new entry in the related enum, and finally change the manager to allow instantiating your new caracteristic of the configuration is targetting your new caracteristic.
 
This is the way to go if you wish to contribute a new integration for the bot.
