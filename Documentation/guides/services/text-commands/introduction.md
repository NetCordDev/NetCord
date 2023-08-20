# Introduction

Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/Full/Program.cs#L3)]

Now, it's time to create @NetCord.Services.Commands.CommandService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/Full/Program.cs#L10-L11)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/Full/Program.cs#L13-L32)]

Now, if you write a message starting with `!`, a bot will reply with `Error: Command not found.`. It's time to create first commands!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord.Services.Commands;`. Make the class inheriting from `CommandModule<CommandContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/Partial/FirstModule.cs)]

Now, we will create a `ping` command! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/Full/FirstModule.cs#L7-L11)]

Now, you have your first command working!

## The Final Product

# [Program.cs](#tab/program)
[!code-cs[Program.cs](Introduction/Full/Program.cs)]

# [FirstModule.cs](#tab/first-module)
[!code-cs[FirstModule.cs](Introduction/Full/FirstModule.cs)]