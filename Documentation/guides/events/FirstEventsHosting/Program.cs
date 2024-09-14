﻿using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.GuildMessages
                      | GatewayIntents.DirectMessages
                      | GatewayIntents.MessageContent
                      | GatewayIntents.DirectMessageReactions
                      | GatewayIntents.GuildMessageReactions;
    })
    .AddGatewayEventHandlers(typeof(Program).Assembly);

var host = builder.Build()
    .UseGatewayEventHandlers();

await host.RunAsync();
