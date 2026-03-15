---
title: Next Steps - Continue Building with NetCord
description: Suggested next steps after setting up your first NetCord bot. Explore key topics and guides to deepen your NetCord knowledge.
omitAppTitle: true
keywords: Discord, NetCord, next steps, bot development, C#, .NET, what's next
section: Getting Started
published_time: '2025-12-15T00:00:00Z'
modified_time: '2025-12-15T00:00:00Z'
---

# What's Next?

Now that you've successfully created and run your first Discord bot with NetCord, here's a roadmap to continue your journey.

## Choose Your Path

Depending on your goals, explore these sections:

### For Command-Heavy Bots

If you're building a bot with lots of commands:
1. [Commands Overview](../commands/index.md) - Learn about different command types
2. [Services Framework](../services-framework/index.md) - Build scalable, modular commands
3. [Type Readers](../services-framework/type-readers.md) - Parse command parameters automatically

### For Interactive Bots

If you want buttons, menus, and modals:
1. [Components v2](../components-v2/index.md) - Modern Discord components
2. [Component Interactions](../component-interactions/index.md) - Handle user interactions
3. [Modal Forms](../component-interactions/modals.md) - Collect user input with modals

### For Event-Driven Bots

If you need to react to server events:
1. [Events Overview](../events/index.md) - Understand the event model
2. [Gateway Events](../events/gateway-events.md) - Handle all Discord events
3. [Intents](../events/intents.md) - Optimize which events your bot receives

### For Advanced Features

If you need specialized functionality:

**Voice Features:**
- [Voice Overview](../voice/index.md) - Music bots, audio processing
- [Audio Playback](../voice/sending-voice.md) - Stream audio to Discord

**Webhooks:**
- [Webhooks Overview](../webhooks/index.md) - External integrations

**User Management:**
- [Discord Entities](../discord-entities/index.md) - Work with users, guilds, roles
- [Permissions](../discord-entities/roles-and-permissions.md) - Permission checks and enforcement

## Deep Dive Into Core Concepts

### Understand the Fundamentals

- [Discord Entities](../discord-entities/index.md) - Users, guilds, channels, roles
- [Events System](../events/index.md) - How NetCord communicates with Discord
- [.NET Integration](../dotnet-integration/index.md) - Hosting, configuration, logging

### Master the Services Framework

The Services Framework is key to professional bot development:

1. Start with [Dependency Injection](../services-framework/dependency-injection.md)
2. Explore [Type Readers](../services-framework/type-readers.md) for automatic parameter parsing
3. Learn [Preconditions](../services-framework/preconditions.md) for permission checks
4. Create [Custom Contexts](../services-framework/custom-contexts.md) for your specific needs

## Performance & Production

When you're ready to scale:

1. [Advanced Topics](../advanced-topics/index.md)
   - [Caching Strategies](../advanced-topics/caching-strategies.md)
   - [Rate Limiting](../advanced-topics/rate-limiting.md)
   - [Sharding](../advanced-topics/sharding.md) (for 2500+ guilds)
   - [Connection Resilience](../advanced-topics/connection-resilience.md)

2. [Deployment](../deployment/index.md)
   - [Docker](../deployment/docker.md) - Containerize your bot
   - [Cloud Hosting](../deployment/cloud-hosting.md) - AWS, Azure, or VPS

## Troubleshooting

If you encounter issues, see:
- [Common Issues](../troubleshooting/common-issues.md) - Solutions to frequent problems
- [FAQ](../troubleshooting/faq.md) - Answers to common questions

## Community & Resources

- Join the [NetCord Community](https://discord.gg/netcord) for help and discussions
- Check out example bots in the [NetCord repository](https://github.com/NetCordDev/NetCord)
- Read the [API documentation](https://netcord.dev/api) for detailed class references

## Best Practices

As you build your bot:

1. **Use Dependency Injection** - Makes your code testable and maintainable
2. **Handle Errors Gracefully** - Always provide useful error messages to users
3. **Respect Rate Limits** - Discord throttles requests; plan accordingly
4. **Cache When Possible** - Reduce API calls by caching data
5. **Keep Tokens Secure** - Never commit tokens to version control
6. **Log Everything** - Use proper logging to debug issues in production

## What's New?

For release information and ongoing development, see:
- [GitHub Releases](https://github.com/NetCordDev/NetCord/releases)
- [Changelog](https://github.com/NetCordDev/NetCord/blob/main/CHANGELOG.md)
- [Discussions](https://github.com/NetCordDev/NetCord/discussions)

---

## Navigation

← **Previous:** [Your First Response](your-first-response.md) | **Next:** [.NET Integration Overview](../dotnet-integration/index.md) →

## See Also

- [.NET Integration](../dotnet-integration/index.md) - Hosting, configuration, and logging
- [Commands Overview](../commands/index.md) - Build slash and text commands
- [Events Overview](../events/index.md) - Handle Discord gateway events

