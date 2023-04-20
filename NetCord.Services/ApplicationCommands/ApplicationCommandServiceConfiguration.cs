namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandServiceConfiguration<TContext> where TContext : IApplicationCommandContext
{
    public Dictionary<Type, SlashCommandTypeReader<TContext>> TypeReaders { get; } = new()
    #region TypeReaders
    {
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
        { typeof(ulong), new TypeReaders.UInt64TypeReader<TContext>() },
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
    };
    #endregion

    public SlashCommandTypeReader<TContext> EnumTypeReader { get; init; } = new TypeReaders.EnumTypeReader<TContext>();

    public bool DefaultDMPermission { get; init; } = true;

    /// <summary>
    /// {0} - parameter name
    /// </summary>
    public string DefaultParameterDescriptionFormat { get; init; } = "No description provided.";
}
