Debug Configuration
===================

## Summary
A couple of settings ar helping with troubleshooting and debugging your bot.

## Unity
The two main parts of Unity helping with this are the Core/BotDebug.cs which is a proxy for the unity Debug Log system. It only helps easily enabling disabling the log as well as displaying timestamp information for the log.

The second part is the feedback system configurable on the Bot Body which can help knowing visually what is happening when some delays are experienced.

## Configuration
1. On the BotBrain component, Enable Debug Log: in order to help troubleshooting the bot the log can be quite verbose. If you would like it being smaller to not interfer with your experience, you can turn it off with this settings.
2. On the BotMaterialisationManager, Enable Feedback: if this setting is active, an in experience visual feedback helps waiting and understanding what is happening when latency is high.  