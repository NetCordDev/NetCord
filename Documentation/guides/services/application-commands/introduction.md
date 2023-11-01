# Introduction

# [Slash Commands](#tab/slash-commands)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/SlashCommands/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/SlashCommands/Full/Program.cs#L11-L12)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](Introduction/SlashCommands/Full/Program.cs#L20-L21)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/SlashCommands/Full/Program.cs#L23-L42)]

Ok, you have everything prepared. It's time to create first slash commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<SlashCommandContext>`. The file should look like this:
[!code-cs[Program.cs](Introduction/SlashCommands/Partial/FirstModule.cs)]

> [!IMPORTANT]
> Please note that names of:
> - slash commands
> - sub slash commands
> - slash command parameters
> 
> **must** be lowercase.

Now, we will create a `ping` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/SlashCommands/Full/FirstModule.cs#L8-L12)]

Now, you have your first slash command working!

# [User Commands](#tab/user-commands)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/UserCommands/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/UserCommands/Full/Program.cs#L11-L12)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](Introduction/UserCommands/Full/Program.cs#L20-L21)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/UserCommands/Full/Program.cs#L23-L42)]

Ok, you have everything prepared. It's time to create first user commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<UserCommandContext>`. The file should look like this:
[!code-cs[Program.cs](Introduction/UserCommands/Partial/FirstModule.cs)]

Now, we will create a `Username` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/UserCommands/Full/FirstModule.cs#L8-L12)]

Now, you have your first user command working!

# [Message Commands](#tab/message-commands)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/MessageCommands/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/MessageCommands/Full/Program.cs#L11-L12)]

Now, we should send the commands to Discord, to make them usable. Add the following lines under `await client.StartAsync();` line:
[!code-cs[Program.cs](Introduction/MessageCommands/Full/Program.cs#L20-L21)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/MessageCommands/Full/Program.cs#L23-L42)]

Ok, you have everything prepared. It's time to create first message commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.ApplicationCommands;`. Make the class inheriting from `ApplicationCommandModule<MessageCommandContext>`. The file should look like this:
[!code-cs[Program.cs](Introduction/MessageCommands/Partial/FirstModule.cs)]

Now, we will create a `Get Length` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/MessageCommands/Full/FirstModule.cs#L8-L12)]

Now, you have your first message command working!

***

## The Final Product

# [Program.cs](#tab/program/slash-commands)
[!code-cs[Program.cs](Introduction/SlashCommands/Full/Program.cs)]
# [Program.cs](#tab/program/user-commands)
[!code-cs[Program.cs](Introduction/UserCommands/Full/Program.cs)]
# [Program.cs](#tab/program/message-commands)
[!code-cs[Program.cs](Introduction/MessageCommands/Full/Program.cs)]

# [FirstModule.cs](#tab/first-module/slash-commands)
[!code-cs[FirstModule.cs](Introduction/SlashCommands/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/user-commands)
[!code-cs[FirstModule.cs](Introduction/UserCommands/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/message-commands)
[!code-cs[FirstModule.cs](Introduction/MessageCommands/Full/FirstModule.cs)]