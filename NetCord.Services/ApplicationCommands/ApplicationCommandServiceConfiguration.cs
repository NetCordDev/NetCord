using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public record ApplicationCommandServiceConfiguration<TContext> where TContext : IApplicationCommandContext
{
    public static ApplicationCommandServiceConfiguration<TContext> Default { get; } = new();

    private ApplicationCommandServiceConfiguration()
    {
    }

    public ImmutableDictionary<Type, SlashCommandTypeReader<TContext>> TypeReaders { get; init; } = new Dictionary<Type, SlashCommandTypeReader<TContext>>()
    {
    #region TypeReaders
        // text types
        { typeof(string), new TypeReaders.StringTypeReader<TContext>() },
        // integral numeric types
        { typeof(sbyte), new TypeReaders.SByteTypeReader<TContext>()},
        { typeof(byte), new TypeReaders.ByteTypeReader<TContext>() },
        { typeof(short), new TypeReaders.Int16TypeReader<TContext>() },
        { typeof(ushort), new TypeReaders.UInt16TypeReader<TContext>() },
        { typeof(int), new TypeReaders.Int32TypeReader<TContext>() },
        { typeof(uint), new TypeReaders.UInt32TypeReader<TContext>() },
        { typeof(long), new TypeReaders.Int64TypeReader<TContext>() },
        { typeof(Int128), new TypeReaders.Int128TypeReader<TContext>() },
        { typeof(ulong), new TypeReaders.UInt64TypeReader<TContext>() },
        { typeof(UInt128), new TypeReaders.UInt128TypeReader<TContext>() },
        { typeof(nint), new TypeReaders.IntPtrTypeReader<TContext>() },
        { typeof(nuint), new TypeReaders.UIntPtrTypeReader<TContext>() },
        // floating-point numeric types
        { typeof(Half), new TypeReaders.HalfTypeReader<TContext>() },
        { typeof(float), new TypeReaders.SingleTypeReader<TContext>() },
        { typeof(double), new TypeReaders.DoubleTypeReader<TContext>() },
        { typeof(decimal), new TypeReaders.DecimalTypeReader<TContext>() },
        // channels
        { typeof(CategoryGuildChannel), new TypeReaders.CategoryGuildChannelTypeReader<TContext>() },
        { typeof(Channel), new TypeReaders.ChannelTypeReader<TContext>() },
        { typeof(IGuildChannel), new TypeReaders.IGuildChannelTypeReader<TContext>() },
        { typeof(IInteractionChannel), new TypeReaders.ChannelTypeReader<TContext>() },
        { typeof(INamedChannel), new TypeReaders.INamedChannelTypeReader<TContext>() },

        { typeof(DMChannel), new TypeReaders.DMChannelTypeReader<TContext>() },
        { typeof(GroupDMChannel), new TypeReaders.GroupDMChannelTypeReader<TContext>() },
        { typeof(TextChannel), new TypeReaders.TextChannelTypeReader<TContext>() },

        { typeof(AnnouncementGuildChannel), new TypeReaders.AnnouncementGuildChannelTypeReader<TContext>() },
        { typeof(DirectoryGuildChannel), new TypeReaders.DirectoryGuildChannelTypeReader<TContext>() },
        { typeof(ForumGuildChannel), new TypeReaders.ForumGuildChannelTypeReader<TContext>() },
        { typeof(MediaForumGuildChannel), new TypeReaders.MediaForumGuildChannelTypeReader<TContext>() },
        { typeof(TextGuildChannel), new TypeReaders.TextGuildChannelTypeReader<TContext>() },

        { typeof(AnnouncementGuildThread), new TypeReaders.AnnouncementGuildThreadTypeReader<TContext>() },
        { typeof(GuildThread), new TypeReaders.GuildThreadTypeReader<TContext>() },
        { typeof(PrivateGuildThread), new TypeReaders.PrivateGuildThreadTypeReader<TContext>() },
        { typeof(PublicGuildThread), new TypeReaders.PublicGuildThreadTypeReader<TContext>() },

        { typeof(IVoiceGuildChannel), new TypeReaders.IVoiceGuildChannelTypeReader<TContext>() },
        { typeof(StageGuildChannel), new TypeReaders.StageGuildChannelTypeReader<TContext>() },
        { typeof(VoiceGuildChannel), new TypeReaders.VoiceGuildChannelTypeReader<TContext>() },
        // other types
        { typeof(bool), new TypeReaders.BooleanTypeReader<TContext>() },
        { typeof(User), new TypeReaders.UserTypeReader<TContext>() },
        { typeof(GuildUser), new TypeReaders.GuildUserTypeReader<TContext>() },
        { typeof(Role), new TypeReaders.RoleTypeReader<TContext>() },
        { typeof(Mentionable), new TypeReaders.MentionableTypeReader<TContext>() },
        { typeof(Attachment), new TypeReaders.AttachmentTypeReader<TContext>() },
    #endregion
    }.ToImmutableDictionary();

    public SlashCommandTypeReader<TContext> EnumTypeReader { get; init; } = new TypeReaders.EnumTypeReader<TContext>();

    public bool DefaultDMPermission { get; init; } = true;

    public IEnumerable<ApplicationIntegrationType>? DefaultIntegrationTypes { get; init; }

    public IEnumerable<InteractionContextType>? DefaultContexts { get; init; }

    public ISlashCommandParameterNameProcessor<TContext> ParameterNameProcessor { get; init; } = new SnakeCaseSlashCommandParameterNameProcessor<TContext>();

    /// <summary>
    /// {0} - parameter name
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    public string DefaultParameterDescriptionFormat { get; init; } = "No description provided.";

    public IResultResolverProvider<TContext> ResultResolverProvider { get; init; } = new ApplicationCommandResultResolverProvider<TContext>();

    public ILocalizationsProvider? LocalizationsProvider { get; init; }
}
