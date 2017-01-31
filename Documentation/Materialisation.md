Materialisation
===============

## Summary
**From Wikipedia:** *In spiritualism, paranormal literature and some religions, materialization (or manifestation) is the creation or appearance of matter from unknown sources. The existence of materialization has not been confirmed by laboratory experiments. Numerous cases of fraudulent materialization demonstrations by mediums have been exposed.*

**From us:** The materialisation is the digital appearance of the bot. It groups the 3D Assets, animations as well as the message and feedback panels available in the bot. 

## Unity
The Materialisation is represented in Untiy by the BotMaterialisationManager.cs behaviour.

## Configuration
![Configuration](Documentation/Pictures/Materialisation.png)

1. Body Type: chose amongst the different allowed built in type of body.
2. Custom Body: you can specify you own behaviour by simply dragging in a GameObject. This will then be used as your bot representation (Please read the detail section for more information)
3. Custom Positioning: drop in a game object containing a behaviour inheriting from BaseBotMaterialisationPositioning.cs. The Position method of this component will then be called in order to let you position the body of the body during the "spawining" phase. This can be usefull to interact with spatial mapping or chosing the desired loaction in your scene.
4. Enable Feedback: the feedback panel providing connectivity information can be easily hidden by disabling this parameter. 

## Details
In order to implement your own visualisation, the game object you will drag in the Custom Body field needs to follow a few rules:
1. The GameObject needs to contain an AudioSource. It will be used by the speech to help you listen to the bot answers and the materialisation dematerialisation sound effects.
2. One of the children needs to have a BotSoundEffectsContainer behaviour referencing both the materialisation and dematerialisation sound effects.
3. One of the children needs to have a tag "BotBodyMessage" and contains one Text and a RawImage components. They are used to display the answer of your bot.
4. One of the children needs to have a tag "BotBodyFeedback" and contains one Text component. It is used to display the feedback of your bot.
5. One of the children needs to hav an Animator component. All the different state of the bot will be passed to the animator through triggers:
 - *Materialize*: Indicates that the bot needs to become visible
 - *DeMaterialize*: Indicates that the bot needs to become hidden
 - *ShowMessage*: Indicates that the bot needs to show an incoming message
 - *Neutral*, *Anger*, *Contempt*, *Disgust*, *Fear*, *Happiness*, *Sadness*, *Surprise*: An emotion corresponding to the name needs to be represented