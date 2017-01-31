Personality
===========

## Summary
**From Wikipedia:** *Personality is a set of individual differences that are affected by the development of an individual: values, attitudes, personal memories, social relationships, habits, and skills. Different personality theorists present their own definitions of the word based on their theoretical positions. The term "personality trait" refers to enduring personal characteristics that are revealed in a particular pattern of behaviour in a variety of situations.*

**From us:** This is the component responsible to manage the Personality of our bot. Depending of the new feelings extracting from each messages in the [Networking](Networking.md) component, this will adjust and combine with the previously available matrix.

## Unity
The Personality is represented in Untiy by the BotPersonalityManager.cs behaviour.

## Configuration
![Configuration](Documentation/Pictures/Personality.png)

1. Built In Type: chose amongst the different allowed built in type of supported paersonality.
2. Custom Caracteristic: you can specify you own behaviour by extending the BaseBotPersonality and referencing the game object hosting your capability here.

## Details
The personality is mainly a way of combining all the different emotions extracted from each messages (through smiley, cognitive services, or network messages) in one matrix. From this matrix we can then extract the main emotion and apply this one to our digital representation. The emotion matrix is stored through the [Memory](Memory.md) component to give everybody a unique experience. A few built-in personality have been implemented as a show case. You could always reset the bot memory by using the Reset Memory Keyword to start fresh from a Neutral bot.