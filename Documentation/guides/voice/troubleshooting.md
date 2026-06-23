# Troubleshooting

Working with audio can sometimes introduce unexpected issues. Below are common problems and their corresponding troubleshooting steps to help you resolve them.

## Missing Native Dependencies

If you encounter a @System.DllNotFoundException, it usually indicates missing or incompatible native dependencies. Refer to the [Native Dependencies](../basic-concepts/installing-native-dependencies.md) guide for steps on resolving this.

---

## Timeout When Connecting to Voice

If you experience a @System.TimeoutException when connecting to a voice channel, it typically stems from one of the following issues:

### Missing Permissions
Ensure the bot has the @NetCord.Permissions.Connect permission in the target voice channel.

### Channel Capacity
Check if the voice channel is full; the bot needs an available slot to join.

### Already Connected
Verify that the bot isn't already active in the channel it is trying to join.

### Slow Network Connection
@NetCord.Gateway.Voice.GatewayClientExtensions.JoinVoiceChannelAsync* has a default timeout of 2 seconds, which might be too brief for slower networks. You can extend this timeout (e.g., to 10 seconds) as shown below:
[!code-cs[Join Voice Channel with Extended Timeout](Troubleshooting/Examples.cs#L13)]

---

## Audio Not Playing

If the bot connects to a voice channel but no audio is heard, verify the following:

### Missing Permissions
Ensure the bot has the @NetCord.Permissions.Speak permission in the voice channel. The bot will appear as muted to other users if it lacks this permission.

### Speaking State
Make sure to call @NetCord.Gateway.Voice.VoiceClient.EnterSpeakingStateAsync* with the correct @NetCord.Gateway.Voice.SpeakingFlags before sending audio.

---

## Failed to Get External Socket Address

The log error `Failed to get the external socket address. Aborting the client.` indicates that external socket address discovery has failed. This is typically caused by firewall restrictions, router configurations, or slow network responses. 

Try connecting the bot to a mobile hotspot to quickly determine if your local router or firewall settings are the culprit.

Alternatively, try increasing the default 5-second discovery timeout directly in your configuration:

[!code-cs[External Socket Address Discovery Timeout](Troubleshooting/Examples.cs#L18-L21)]

---

## Receiving Audio Stops After Some Time

If audio suddenly cuts out after working normally for a while, consider these common causes:

### UDP Flood Protection
Your firewall or router might be blocking UDP packets due to a built-in flood protection feature. Try disabling this feature in your router settings, or temporarily switch to a mobile hotspot to see if the issue is strictly related to your router's configuration.

### Closed UDP Socket
Sockets can sometimes close automatically, especially if the bot is listening without transmitting any audio. You can prevent this by adjusting your firewall/NAT settings or by implementing a keep-alive mechanism. 

Sending a small, empty data packet at regular intervals will ensure the UDP connection stays open, as demonstrated below:
[!code-cs[Keep Alive](Troubleshooting/Examples.cs#L26-L29)]
