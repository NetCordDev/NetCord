﻿using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands.Administrative;

internal class CanManageAttribute : ParameterPreconditionAttribute<CommandContext>
{
    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, CommandContext context, IServiceProvider? serviceProvider)
    {
        var user = (GuildUser)value!;
        var contextUser = (GuildUser)context.User;
        var guild = context.Guild!;
        if (user.GetRoles(guild).Max(r => r.Position) >= contextUser.GetRoles(guild).Max(r => r.Position))
            return new(PreconditionResult.Fail("You can't manage this user!"));

        return new(PreconditionResult.Success);
    }
}
