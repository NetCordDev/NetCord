using System.Collections.ObjectModel;
using System.Reflection;

namespace NetCord.Services.SlashCommands;

public class SlashCommandInfo<TContext> where TContext : BaseSlashCommandContext
{
    public Type DeclaringType { get; }
    public string Name { get; }
    public string Description { get; }
    public bool DefaultPermission { get; init; }
    public DiscordId? GuildId { get; init; }
    public IEnumerable<DiscordId>? AllowedRolesIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedRolesIds { get; init; }
    public IEnumerable<DiscordId>? AllowedUsersIds { get; init; }
    public IEnumerable<DiscordId>? DisallowedUsersIds { get; init; }
    public Func<object, object[], Task> InvokeAsync { get; }
    public ReadOnlyCollection<SlashCommandParameter<TContext>> Parameters { get; }
    public Dictionary<string, Autocomplete> Autocompletes { get; } = new();
    public Permission RequiredBotPermissions { get; }
    public Permission RequiredBotChannelPermissions { get; }
    public Permission RequiredUserPermissions { get; }
    public Permission RequiredUserChannelPermissions { get; }

    internal SlashCommandInfo(MethodInfo methodInfo, SlashCommandAttribute attribute, SlashCommandServiceOptions<TContext> options)
    {
        DeclaringType = methodInfo.DeclaringType!;
        Name = attribute.Name;
        Description = attribute.Description;
        DefaultPermission = attribute.DefaultPermission;
        GuildId = attribute.GuildId == default ? null : attribute.GuildId;
        if (attribute.AllowedRolesIds != null)
            AllowedRolesIds = attribute.AllowedRolesIds.Select(id => new DiscordId(id));
        if (attribute.DisallowedRolesIds != null)
            DisallowedRolesIds = attribute.DisallowedRolesIds.Select(id => new DiscordId(id));
        if (attribute.AllowedUsersIds != null)
            AllowedUsersIds = attribute.AllowedUsersIds.Select(id => new DiscordId(id));
        if (attribute.DisallowedUsersIds != null)
            DisallowedUsersIds = attribute.DisallowedUsersIds.Select(id => new DiscordId(id));
        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;
        var parameters = methodInfo.GetParameters();
        var parametersLength = parameters.Length;
        var p = new SlashCommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters", methodInfo);
            SlashCommandParameter<TContext> newP = new(parameter, options, out var autocomplete);
            p[i] = newP;
            if (autocomplete != null)
                Autocompletes.Add(newP.Name, autocomplete);
        }
        Parameters = Array.AsReadOnly(p);

        SlashCommandModuleAttribute? moduleAttribute = DeclaringType.GetCustomAttribute<SlashCommandModuleAttribute>();
        if (moduleAttribute != null)
        {
            RequiredBotPermissions = attribute.RequiredBotPermissions | moduleAttribute.RequiredBotPermissions;
            RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions | moduleAttribute.RequiredBotChannelPermissions;
            RequiredUserPermissions = attribute.RequiredUserPermissions | moduleAttribute.RequiredUserPermissions;
            RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions | moduleAttribute.RequiredUserChannelPermissions;
        }
        else
        {
            RequiredBotPermissions = attribute.RequiredBotPermissions;
            RequiredBotChannelPermissions = attribute.RequiredBotChannelPermissions;
            RequiredUserPermissions = attribute.RequiredUserPermissions;
            RequiredUserChannelPermissions = attribute.RequiredUserChannelPermissions;
        }
    }

    public ApplicationCommandProperties GetRawValue() => new(Name, Description)
    {
        DefaultPermission = DefaultPermission,
        Options = Parameters.Select(p => p.GetRawValue()).ToList(),
    };

    public IEnumerable<ApplicationCommandPermissionProperties> GetRawPermissions()
    {
        if (AllowedRolesIds != null)
        {
            foreach (var r in AllowedRolesIds)
                yield return new(r, ApplicationCommandPermissionType.Role, true);
        }
        if (DisallowedRolesIds != null)
        {
            foreach (var r in DisallowedRolesIds)
                yield return new(r, ApplicationCommandPermissionType.Role, false);
        }
        if (AllowedUsersIds != null)
        {
            foreach (var u in AllowedUsersIds)
                yield return new(u, ApplicationCommandPermissionType.User, true);
        }
        if (DisallowedUsersIds != null)
        {
            foreach (var u in DisallowedUsersIds)
                yield return new(u, ApplicationCommandPermissionType.User, false);
        }
    }
}