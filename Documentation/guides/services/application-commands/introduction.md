# Introduction

Firstly, add the following line to using section.
```cs
using NetCord.Services.ApplicationCommands;
```

# [Slash Commands](#tab/slash-commands)
Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](slash-commands/Program.cs#L10-L11)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](slash-commands/Program.cs#L19-L20)]

We can add a command handler now.
[!code-cs[Program.cs](slash-commands/Program.cs#L22-L41)]

Ok, you have everything prepared. It's time to create first slash commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<SlashCommandContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<SlashCommandContext>
{
}
```

Now, we will create a `ping` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](slash-commands/FirstModule.cs#L8-L12)]

Now, you have your first slash command working!

# [User Commands](#tab/user-commands)
Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](user-commands/Program.cs#L10-L11)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](user-commands/Program.cs#L19-L20)]

We can add a command handler now.
[!code-cs[Program.cs](user-commands/Program.cs#L22-L41)]

Ok, you have everything prepared. It's time to create first user commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<UserCommandContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<UserCommandContext>
{
}
```

Now, we will create a `Username` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](user-commands/FirstModule.cs#L8-L12)]

Now, you have your first user command working!

# [Message Commands](#tab/message-commands)
Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](message-commands/Program.cs#L10-L11)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](message-commands/Program.cs#L19-L20)]

We can add a command handler now.
[!code-cs[Program.cs](message-commands/Program.cs#L22-L41)]

Ok, you have everything prepared. It's time to create first message commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<MessageCommandContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class FirstModule : ApplicationCommandModule<MessageCommandContext>
{
}
```

Now, we will create a `Get Length` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](message-commands/FirstModule.cs#L8-L12)]

Now, you have your first message command working!

***

## The Final Product

# [Program.cs](#tab/program/slash-commands)
[!code-cs[Program.cs](slash-commands/Program.cs)]
# [Program.cs](#tab/program/user-commands)
[!code-cs[Program.cs](user-commands/Program.cs)]
# [Program.cs](#tab/program/message-commands)
[!code-cs[Program.cs](message-commands/Program.cs)]

# [FirstModule.cs](#tab/first-module/slash-commands)
[!code-cs[FirstModule.cs](slash-commands/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/user-commands)
[!code-cs[FirstModule.cs](user-commands/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/message-commands)
[!code-cs[FirstModule.cs](message-commands/FirstModule.cs)]