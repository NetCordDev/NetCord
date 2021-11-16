namespace NetCord
{
    public class TeamUser : User
    {
        private readonly JsonModels.JsonTeamUser _jsonTeamEntity;

        public MembershipState MembershipState => _jsonTeamEntity.MembershipState;
        public IEnumerable<string> Permissions => _jsonTeamEntity.Permissions;
        public DiscordId TeamId => _jsonTeamEntity.TeamId;

        public override DiscordId Id => _jsonTeamEntity.User.Id;
        public override string Username => _jsonTeamEntity.User.Username;
        public override ushort Discriminator => _jsonTeamEntity.User.Discriminator;
        public override string? AvatarHash => _jsonTeamEntity.User.AvatarHash;
        public override bool IsBot => _jsonTeamEntity.User.IsBot;
        public override bool? IsOfficialDiscordUser => _jsonTeamEntity.User.IsOfficialDiscordUser;
        public override bool? MFAEnabled => _jsonTeamEntity.User.MFAEnabled;
        public override string? Locale => _jsonTeamEntity.User.Locale;
        public override bool? Verified => _jsonTeamEntity.User.Verified;
        public override string? Email => _jsonTeamEntity.User.Email;
        public override UserFlags? Flags => _jsonTeamEntity.User.Flags;
        public override PremiumType? PremiumType => _jsonTeamEntity.User.PremiumType;
        public override UserFlags? PublicFlags => _jsonTeamEntity.User.PublicFlags;

        internal TeamUser(JsonModels.JsonTeamUser jsonEntity, BotClient client) : base(jsonEntity.User, client)
        {
            _jsonTeamEntity = jsonEntity;
        }
    }
}