# Introduction

# [Buttons](#tab/buttons)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/Buttons/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/Buttons/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/Buttons/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<ButtonInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/Buttons/Partial/FirstModule.cs)]

Now, we will create a `button` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/Buttons/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [String Menus](#tab/string-menus)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/StringMenus/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/StringMenus/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/StringMenus/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<StringMenuInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/StringMenus/Partial/FirstModule.cs)]

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/StringMenus/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [User Menus](#tab/user-menus)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/UserMenus/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/UserMenus/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/UserMenus/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<UserMenuInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/UserMenus/Partial/FirstModule.cs)]

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/UserMenus/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Role Menus](#tab/role-menus)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/RoleMenus/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/RoleMenus/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/RoleMenus/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<RoleMenuInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/RoleMenus/Partial/FirstModule.cs)]

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/RoleMenus/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Mentionable Menus](#tab/mentionable-menus)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/MentionableMenus/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/MentionableMenus/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/MentionableMenus/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<MentionableMenuInteractionContext>`. The file should look like this:
[!code-cs[Program.cs](Introduction/MentionableMenus/Partial/Program.cs)]

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/MentionableMenus/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Channel Menus](#tab/channel-menus)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/ChannelMenus/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/ChannelMenus/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/ChannelMenus/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<ChannelMenuInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/ChannelMenus/Partial/FirstModule.cs)]

Now, we will create a `menu` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/ChannelMenus/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

# [Modal Submits](#tab/modal-submits)
Firstly, add the following line to using section.
[!code-cs[Program.cs](Introduction/ModalSubmits/Full/Program.cs#L4)]

Now, it's time to create @NetCord.Services.Interactions.InteractionService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/ModalSubmits/Full/Program.cs#L11-L12)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/ModalSubmits/Full/Program.cs#L14-L33)]

Ok, you have everything prepared! It's time to create first interactions!

Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord;` and `using NetCord.Services.Interactions;`. Make the class inheriting from `FirstModule<ModalSubmitInteractionContext>`. The file should look like this:
[!code-cs[FirstModule.cs](Introduction/ModalSubmits/Partial/FirstModule.cs)]

Now, we will create a `modal` interaction! Add the following lines to the class.
[!code-cs[FirstModule.cs](Introduction/ModalSubmits/Full/FirstModule.cs#L8-L12)]

Now, you have your first interaction working!

***

## The Final Product

# [Program.cs](#tab/program/buttons)
[!code-cs[Program.cs](Introduction/Buttons/Full/Program.cs)]
# [Program.cs](#tab/program/string-menus)
[!code-cs[Program.cs](Introduction/StringMenus/Full/Program.cs)]
# [Program.cs](#tab/program/user-menus)
[!code-cs[Program.cs](Introduction/UserMenus/Full/Program.cs)]
# [Program.cs](#tab/program/role-menus)
[!code-cs[Program.cs](Introduction/RoleMenus/Full/Program.cs)]
# [Program.cs](#tab/program/mentionable-menus)
[!code-cs[Program.cs](Introduction/MentionableMenus/Full/Program.cs)]
# [Program.cs](#tab/program/channel-menus)
[!code-cs[Program.cs](Introduction/ChannelMenus/Full/Program.cs)]
# [Program.cs](#tab/program/modal-submits)
[!code-cs[Program.cs](Introduction/ModalSubmits/Full/Program.cs)]

# [FirstModule.cs](#tab/first-module/buttons)
[!code-cs[FirstModule.cs](Introduction/Buttons/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/string-menus)
[!code-cs[FirstModule.cs](Introduction/StringMenus/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/user-menus)
[!code-cs[FirstModule.cs](Introduction/UserMenus/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/role-menus)
[!code-cs[FirstModule.cs](Introduction/RoleMenus/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/mentionable-menus)
[!code-cs[FirstModule.cs](Introduction/MentionableMenus/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/channel-menus)
[!code-cs[FirstModule.cs](Introduction/ChannelMenus/Full/FirstModule.cs)]
# [FirstModule.cs](#tab/first-module/modal-submits)
[!code-cs[FirstModule.cs](Introduction/ModalSubmits/Full/FirstModule.cs)]