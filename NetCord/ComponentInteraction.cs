using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class ComponentInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    public abstract override ComponentInteractionData Data { get; }
}

public class ComponentInteractionData(JsonComponentInteractionData jsonModel, ulong? guildId, RestClient client) : InteractionData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public InteractionResolvedData? Resolved { get; } = jsonModel.Resolved is { } resolved ? new(resolved, guildId, client) : null;
}

public abstract class MessageComponentInteraction : ComponentInteraction
{
    public MessageComponentInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
        var message = jsonModel.Message!;
        message.GuildId = jsonModel.GuildId;
        Message = new(message, guild, Channel, client);
    }

    public Message Message { get; }

    public abstract override MessageComponentInteractionData Data { get; }
}

public abstract class MessageComponentInteractionData(JsonMessageComponentInteractionData jsonModel, ulong? guildId, RestClient client) : ComponentInteractionData(jsonModel, guildId, client)
{
    public int Id { get; } = jsonModel.Id;

    public ComponentType ComponentType { get; } = jsonModel.Type;
}

public class UnknownMessageComponentInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : MessageComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override UnknownMessageComponentInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public class UnknownMessageComponentInteractionData(JsonMessageComponentInteractionData jsonModel, ulong? guildId, RestClient client) : MessageComponentInteractionData(jsonModel, guildId, client);

public class ButtonInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : MessageComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override ButtonInteractionData Data { get; } = new((JsonButtonInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class ButtonInteractionData(JsonButtonInteractionData jsonModel, ulong? guildId, RestClient client) : MessageComponentInteractionData(jsonModel, guildId, client);

public class StringSelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : MessageComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override StringSelectInteractionData Data { get; } = new((JsonStringSelectInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class StringSelectInteractionData(JsonStringSelectInteractionData jsonModel, ulong? guildId, RestClient client) : MessageComponentInteractionData(jsonModel, guildId, client)
{
    public IReadOnlyList<string> Values { get; } = jsonModel.Values;
}

public abstract class EntitySelectInteraction : MessageComponentInteraction
{
    private protected EntitySelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
    }

    public abstract override EntitySelectInteractionData Data { get; }
}

public abstract class EntitySelectInteractionData(JsonEntitySelectInteractionData jsonModel, ulong? guildId, RestClient client) : MessageComponentInteractionData(jsonModel, guildId, client)
{
    public IReadOnlyList<ulong> Values { get; } = jsonModel.Values;
}

public class UserSelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntitySelectInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override UserSelectInteractionData Data { get; } = new((JsonUserSelectInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class UserSelectInteractionData : EntitySelectInteractionData
{
    public UserSelectInteractionData(JsonUserSelectInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        var values = jsonModel.Values;
        if (values.Length > 0)
        {
            var users = Resolved!.Users!;

            Values = [.. values.Select(v => users[v])];
        }
        else
            Values = [];
    }

    public new IReadOnlyList<User> Values { get; }
}

public class RoleSelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntitySelectInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override RoleSelectInteractionData Data { get; } = new((JsonRoleSelectInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class RoleSelectInteractionData : EntitySelectInteractionData
{
    public RoleSelectInteractionData(JsonRoleSelectInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        var values = jsonModel.Values;
        if (values.Length > 0)
        {
            var roles = Resolved!.Roles!;

            Values = [.. values.Select(v => roles[v])];
        }
        else
            Values = [];
    }

    public new IReadOnlyList<Role> Values { get; }
}

public class MentionableSelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntitySelectInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override MentionableSelectInteractionData Data { get; } = new((JsonMentionableSelectInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class MentionableSelectInteractionData : EntitySelectInteractionData
{
    public MentionableSelectInteractionData(JsonMentionableSelectInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        var values = jsonModel.Values;
        if (values.Length > 0)
        {
            var resolved = Resolved!;

            Values = resolved.Users is { } users
                ? resolved.Roles is { } roles
                    ? [.. values.Select(v => (Mentionable)(users.TryGetValue(v, out var user) ? new Mentionable.User(user) : new Mentionable.Role(roles[v])))]
                    : [.. values.Select(v => (Mentionable)new Mentionable.User(users[v]))]
                : [.. values.Select(v => (Mentionable)new Mentionable.Role(resolved.Roles![v]))];
        }
        else
            Values = [];
    }

    public new IReadOnlyList<Mentionable> Values { get; }
}

public class ChannelSelectInteraction(JsonMessageComponentInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : EntitySelectInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override ChannelSelectInteractionData Data { get; } = new((JsonChannelSelectInteractionData)jsonModel.Data, jsonModel.GuildId, client);
}

public class ChannelSelectInteractionData : EntitySelectInteractionData
{
    public ChannelSelectInteractionData(JsonChannelSelectInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        if (jsonModel.Values.Length > 0)
        {
            var channels = Resolved!.Channels!;

            Values = [.. jsonModel.Values.Select(v => channels[v])];
        }
        else
            Values = [];
    }

    public new IReadOnlyList<Channel> Values { get; }
}

public class ModalSubmitInteraction(JsonModalSubmitInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : ComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    public override ModalInteractionData Data { get; } = new(jsonModel.Data, jsonModel.GuildId, client);
}

public class ModalInteractionData : ComponentInteractionData
{
    public IReadOnlyList<IModalChildComponentData> Components { get; }

    public ModalInteractionData(JsonModalSubmitInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        var resolved = Resolved;

        Components = [.. jsonModel.Components.Select(c => IModalChildComponentData.Create(c, resolved))];
    }
}

public interface IModalChildComponentData
{
    public int Id { get; }

    public ComponentType Type { get; }

    public static IModalChildComponentData Create(JsonComponentData jsonModel, InteractionResolvedData? resolved)
    {
        return jsonModel switch
        {
            JsonLabelComponentData labelData => new LabelComponentData(labelData, resolved),
            JsonTextDisplayComponentData textDisplayData => new TextDisplayComponentData(textDisplayData),
            _ => new UnknownModalChildComponentData((JsonUnknownComponentData)jsonModel)
        };
    }
}

public class UnknownModalChildComponentData(JsonUnknownComponentData jsonModel) : IModalChildComponentData
{
    public int Id { get; } = jsonModel.Id;

    public ComponentType Type { get; } = jsonModel.Type;
}

public interface ILabelChildComponentData
{
    public int Id { get; }

    public ComponentType Type { get; }

    public static ILabelChildComponentData Create(JsonComponentData jsonModel, InteractionResolvedData? resolved)
    {
        return jsonModel switch
        {
            JsonStringSelectComponentData stringSelectData => new StringSelectComponentData(stringSelectData),
            JsonTextInputComponentData textInputData => new TextInputComponentData(textInputData),
            JsonUserSelectComponentData userSelectData => new UserSelectComponentData(userSelectData),
            JsonRoleSelectComponentData roleSelectData => new RoleSelectComponentData(roleSelectData),
            JsonMentionableSelectComponentData mentionableSelectData => new MentionableSelectComponentData(mentionableSelectData),
            JsonChannelSelectComponentData channelSelectData => new ChannelSelectComponentData(channelSelectData),
            JsonFileUploadComponentData fileUploadData => new FileUploadComponentData(fileUploadData, resolved),
            JsonRadioGroupComponentData radioGroupData => new RadioGroupComponentData(radioGroupData),
            JsonCheckboxGroupComponentData checkboxGroupData => new CheckboxGroupComponentData(checkboxGroupData),
            JsonCheckboxComponentData checkboxGroupItem => new CheckboxComponentData(checkboxGroupItem),
            _ => new UnknownLabelChildComponentData((JsonUnknownComponentData)jsonModel)
        };
    }
}

public class UnknownLabelChildComponentData(JsonUnknownComponentData jsonModel) : ILabelChildComponentData
{
    public int Id { get; } = jsonModel.Id;

    public ComponentType Type { get; } = jsonModel.Type;
}

public abstract class ComponentData(JsonComponentData jsonModel)
{
    public ComponentType Type { get; } = jsonModel.Type;

    public int Id { get; } = jsonModel.Id;
}

public class ButtonComponentData(JsonButtonComponentData jsonModel) : ComponentData(jsonModel)
{
    public string CustomId { get; } = jsonModel.CustomId;
}

public class StringSelectComponentData(JsonStringSelectComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public IReadOnlyList<string> Values { get; } = jsonModel.Values;
}

public class TextInputComponentData(JsonTextInputComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public string Value { get; } = jsonModel.Value;
}

public abstract class EntitySelectComponentData(JsonEntitySelectComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public IReadOnlyList<ulong> Values { get; } = jsonModel.Values;
}

public class UserSelectComponentData(JsonUserSelectComponentData jsonModel) : EntitySelectComponentData(jsonModel);

public class RoleSelectComponentData(JsonRoleSelectComponentData jsonModel) : EntitySelectComponentData(jsonModel);

public class MentionableSelectComponentData(JsonMentionableSelectComponentData jsonModel) : EntitySelectComponentData(jsonModel);

public class ChannelSelectComponentData(JsonChannelSelectComponentData jsonModel) : EntitySelectComponentData(jsonModel);

public class TextDisplayComponentData(JsonTextDisplayComponentData jsonModel) : ComponentData(jsonModel), IModalChildComponentData;

public class LabelComponentData(JsonLabelComponentData jsonModel, InteractionResolvedData? resolved) : ComponentData(jsonModel), IModalChildComponentData
{
    public ILabelChildComponentData Component { get; } = ILabelChildComponentData.Create(jsonModel.Component, resolved);
}

public class FileUploadComponentData(JsonFileUploadComponentData jsonModel, InteractionResolvedData? resolved) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public IReadOnlyList<Attachment> Values { get; } = CreateValues(jsonModel.Values, resolved);

    private static IReadOnlyList<Attachment> CreateValues(ulong[] values, InteractionResolvedData? resolved)
    {
        if (values.Length > 0)
        {
            var attachments = resolved!.Attachments!;

            return [.. values.Select(v => attachments[v])];
        }
        else
            return [];
    }
}

public class RadioGroupComponentData(JsonRadioGroupComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public string? Value { get; } = jsonModel.Value;
}

public class CheckboxGroupComponentData(JsonCheckboxGroupComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public IReadOnlyList<string> Values { get; } = jsonModel.Values;
}

public class CheckboxComponentData(JsonCheckboxComponentData jsonModel) : ComponentData(jsonModel), ILabelChildComponentData
{
    public string CustomId { get; } = jsonModel.CustomId;

    public bool Value { get; } = jsonModel.Value;
}

public class UnknownComponentData(JsonUnknownComponentData jsonModel) : ComponentData(jsonModel);
