# Introduction

Firstly, add the following line to using section.
```cs
using NetCord.Services.Interactions;
```

# [Button Interactions](#tab/button-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](button-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](button-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<ButtonInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<ButtonInteractionContext>
{
}
```

Now, we will create a `button` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](button-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Menu Interactions](#tab/menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<MenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<MenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](menu-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Modal Submit Interactions](#tab/modal-submit-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](modal-submit-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](modal-submit-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<ModalSubmitInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<ModalSubmitInteractionContext>
{
}
```

Now, we will create a `modal` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](modal-submit-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

***

## The Final Product

# [Program.cs](#tab/program/button-interactions)
[!code-cs[Program.cs](button-interactions/Program.cs)]
# [Program.cs](#tab/program/menu-interactions)
[!code-cs[Program.cs](menu-interactions/Program.cs)]
# [Program.cs](#tab/program/modal-submit-interactions)
[!code-cs[Program.cs](modal-submit-interactions/Program.cs)]

# [InteractionModule.cs](#tab/interaction-module/button-interactions)
[!code-cs[InteractionModule.cs](button-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/menu-interactions)
[!code-cs[InteractionModule.cs](menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/modal-submit-interactions)
[!code-cs[InteractionModule.cs](modal-submit-interactions/InteractionModule.cs)]