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

# [String Menu Interactions](#tab/string-menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](string-menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](string-menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<StringMenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<StringMenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](string-menu-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [User Menu Interactions](#tab/user-menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](user-menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](user-menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<UserMenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<UserMenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](user-menu-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Role Menu Interactions](#tab/role-menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](role-menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](role-menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<RoleMenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<RoleMenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](role-menu-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Mentionable Menu Interactions](#tab/mentionable-menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](mentionable-menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](mentionable-menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<MentionableMenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<MentionableMenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](mentionable-menu-interactions/InteractionModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Channel Menu Interactions](#tab/channel-menu-interactions)
Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](channel-menu-interactions/Program.cs#L10-L11)]

We can add an interaction handler now.
[!code-cs[Program.cs](channel-menu-interactions/Program.cs#L13-L32)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `InteractionModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `InteractionModule<ChannelMenuInteractionContext>`. The file should look like this:
```cs
using NetCord;
using NetCord.Services.Interactions;

namespace MyBot;

public class InteractionModule : InteractionModule<ChannelMenuInteractionContext>
{
}
```

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[InteractionModule.cs](channel-menu-interactions/InteractionModule.cs#L8-L12)]

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
# [Program.cs](#tab/program/string-menu-interactions)
[!code-cs[Program.cs](string-menu-interactions/Program.cs)]
# [Program.cs](#tab/program/user-menu-interactions)
[!code-cs[Program.cs](user-menu-interactions/Program.cs)]
# [Program.cs](#tab/program/role-menu-interactions)
[!code-cs[Program.cs](role-menu-interactions/Program.cs)]
# [Program.cs](#tab/program/mentionable-menu-interactions)
[!code-cs[Program.cs](mentionable-menu-interactions/Program.cs)]
# [Program.cs](#tab/program/channel-menu-interactions)
[!code-cs[Program.cs](channel-menu-interactions/Program.cs)]
# [Program.cs](#tab/program/modal-submit-interactions)
[!code-cs[Program.cs](modal-submit-interactions/Program.cs)]

# [InteractionModule.cs](#tab/interaction-module/button-interactions)
[!code-cs[InteractionModule.cs](button-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/string-menu-interactions)
[!code-cs[InteractionModule.cs](string-menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/user-menu-interactions)
[!code-cs[InteractionModule.cs](user-menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/role-menu-interactions)
[!code-cs[InteractionModule.cs](role-menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/mentionable-menu-interactions)
[!code-cs[InteractionModule.cs](mentionable-menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/channel-menu-interactions)
[!code-cs[InteractionModule.cs](channel-menu-interactions/InteractionModule.cs)]
# [InteractionModule.cs](#tab/interaction-module/modal-submit-interactions)
[!code-cs[InteractionModule.cs](modal-submit-interactions/InteractionModule.cs)]