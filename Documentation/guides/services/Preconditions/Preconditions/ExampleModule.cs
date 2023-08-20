using NetCord.Services.Commands;

namespace MyBot;

[RequireDiscriminator<CommandContext>(1234)]
public class ExampleModule : CommandModule<CommandContext>
{
    // All commands here will require 1234 discriminator
}
