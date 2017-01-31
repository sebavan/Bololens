Brain
========

## Summary
**From Wikipedia:** *The brain is an organ that serves as the center of the nervous system in all vertebrate and most invertebrate animals. The brain is located in the head, usually close to the sensory organs for senses such as vision. The brain is the most complex organ in a vertebrate's body. In a human, the cerebral cortex contains approximately 15–33 billion neurons,[1] each connected by synapses to several thousand other neurons. These neurons communicate with one another by means of long protoplasmic fibers called axons, which carry trains of signal pulses called action potentials to distant parts of the brain or body targeting specific recipient cells.*
*Physiologically, the function of the brain is to exert centralized control over the other organs of the body. The brain acts on the rest of the body both by generating patterns of muscle activity and by driving the secretion of chemicals called hormones. This centralized control allows rapid and coordinated responses to changes in the environment. Some basic types of responsiveness such as reflexes can be mediated by the spinal cord or peripheral ganglia, but sophisticated purposeful control of behavior based on complex sensory input requires the information integrating capabilities of a centralized brain.*

**From us:** Almost the same, the brains is responsible of orchestrating all the different components of the bot :-) It ensures that we are not speaking during listening, reacts to order such as activation or desactivation. It also is responsible of storing the feelings extracted from the different messages to passs them to the body. Main difference is the number of neurons probably reduced by a factor close to 20 billions.

## Unity
The brain is represented in Untiy by the BotBrain.cs behaviour.

## Configuration
![Configuration](blob/master/Documentation/Pictures/Brain.png)

1. Url Or Token: you can either input here the token of your bot or the url of a service delivering a token when called. The service will be called without authentication and should return a JSON formatted like this { "Token": "abc", "Expires_In": <ExpirationInSeconds> }.
2. Activation Keywords: a list of keywords to listen to in order to activate (materialize) the bot.
3. Desactivation Keyword: the keyword the bot is listening in order to stop the conversation.
4. Take Picture Keyword: the keyword responsible of making the bot take a picture and sending it to the bot framework. This picture does not contain holograms.
5. Take Holographic Picture Keyword: the keyword responsible of making the bot take a picture and sending it to the bot framework. This picture contains holograms.
6. Reset Memory Keyword: the keyword responsible of flushing the bot memory. Any information recorded locally by the bot would be deleted.
7. Wait For First Message: if true, the bot will wait for a first introduction message coming from the server before begin to listen to your voice.
8. Enable Debug Log: in order to help troubleshooting the bot the log can be quite verbose. If you would like it being smaller to not interfer with your experience, you can turn it off with this settings.

The other configuration and settings are available through there respective components.

## Details
The component has a dependency on all the required orchestrated components creating a bot. Adding to any GameObject and configurating the Url or Token is enough to get started. Actually the main Bololens Prefab has been created by simply adding the BotBrain behaviour to a game object.  