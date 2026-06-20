# Troubleshooting

Sometimes, you may encounter issues when working with audio.

Below you can find some common issues, as well as some troubleshooting steps you can take to resolve them.

## Failed to Get External Socket Address

If you see the error "Failed to get the external socket address. Aborting the client." in your logs, it means that something blocks the IP Discovery response from Discord. This can be caused by various reasons, such as firewall settings or router configurations. If you encounter this issue, try using for example your mobile phone's hotspot to see if the issue is related to your router configuration. If it works on the hotspot, you may need to check your router's settings.

## Audio Stops After Some Time

If you receive audio for some time and then it suddenly stops, it may be caused by your router blocking the packets after some time. This can be caused by the router's UDP flood protection. You can try disabling this feature in your router's settings to see if it resolves the issue.
