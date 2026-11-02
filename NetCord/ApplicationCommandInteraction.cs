using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Acts as a base class for application commands, such as slash commands and message commands.
/// </summary>
public abstract class ApplicationCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    /// <summary>
    /// Holds the containing application command's data.
    /// </summary>
    public abstract override ApplicationCommandData Data { get; }
}

/// <summary>
/// Represents an application command interaction that is not recognized.
/// </summary>
public sealed class UnknownApplicationCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override ApplicationCommandData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

/// <summary>
/// Contains data for an invoked <see cref="ApplicationCommand"/>.
/// </summary>
public class ApplicationCommandData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : InteractionData
{
    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s ID.
    /// </summary>
    public ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s name.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s type.
    /// </summary>
    public ApplicationCommandType Type { get; } = jsonModel.Type;

    /// <summary>
    /// Resolved data for the invoked <see cref="ApplicationCommand"/>, if any.
    /// </summary>
    public InteractionResolvedData? Resolved { get; } = jsonModel.Resolved is { } resolved ? new(resolved, guildId, client) : null;

    /// <summary>
    /// The ID of the guild the <see cref="ApplicationCommand"/> is registered to.
    /// </summary>
    public ulong? GuildId { get; } = jsonModel.GuildId;
}

public sealed class SlashCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override SlashCommandInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public class SlashCommandInteractionData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : ApplicationCommandData(jsonModel, guildId, client)
{
    public IReadOnlyList<ApplicationCommandInteractionDataOption> Options { get; } = [.. jsonModel.Options!.Select(o => new ApplicationCommandInteractionDataOption(o))];
}

public sealed class UserCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override UserCommandInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public sealed class UserCommandInteractionData : ApplicationCommandData
{
    public UserCommandInteractionData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        TargetUser = Resolved!.Users![jsonModel.TargetId.GetValueOrDefault()];
    }

    public User TargetUser { get; }
}

public sealed class MessageCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override MessageCommandInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public sealed class MessageCommandInteractionData : ApplicationCommandData
{
    public MessageCommandInteractionData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        TargetMessage = Resolved!.Messages![jsonModel.TargetId.GetValueOrDefault()];
    }

    public RestMessage TargetMessage { get; }
}

public sealed class EntryPointCommandInteraction(JsonApplicationCommandInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ApplicationCommandInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override EntryPointCommandInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public sealed class EntryPointCommandInteractionData(JsonApplicationCommandInteractionData jsonModel, ulong? guildId, RestClient client) : ApplicationCommandData(jsonModel, guildId, client);
