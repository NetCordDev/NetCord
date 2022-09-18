using System.Globalization;

using NetCord.Rest;

namespace NetCord;

public class TeamUser : User, IJsonModel<JsonModels.JsonTeamUser>
{
    JsonModels.JsonTeamUser IJsonModel<JsonModels.JsonTeamUser>.JsonModel => _jsonTeamModel;
    private readonly JsonModels.JsonTeamUser _jsonTeamModel;

    public MembershipState MembershipState => _jsonTeamModel.MembershipState;
    public IReadOnlyList<string> Permissions => _jsonTeamModel.Permissions;
    public Snowflake TeamId => _jsonTeamModel.TeamId;

    public override Snowflake Id => _jsonTeamModel.User.Id;
    public override string Username => _jsonTeamModel.User.Username;
    public override ushort Discriminator => _jsonTeamModel.User.Discriminator;
    public override string? AvatarHash => _jsonTeamModel.User.AvatarHash;
    public override bool IsBot => _jsonTeamModel.User.IsBot;
    public override bool? IsSystemUser => _jsonTeamModel.User.IsSystemUser;
    public override bool? MfaEnabled => _jsonTeamModel.User.MfaEnabled;
    public override CultureInfo? Locale => _jsonTeamModel.User.Locale;
    public override bool? Verified => _jsonTeamModel.User.Verified;
    public override string? Email => _jsonTeamModel.User.Email;
    public override UserFlags? Flags => _jsonTeamModel.User.Flags;
    public override PremiumType? PremiumType => _jsonTeamModel.User.PremiumType;
    public override UserFlags? PublicFlags => _jsonTeamModel.User.PublicFlags;

    public TeamUser(JsonModels.JsonTeamUser jsonModel, RestClient client) : base(jsonModel.User, client)
    {
        _jsonTeamModel = jsonModel;
    }
}
