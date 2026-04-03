using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class InviteTargetUsersJobStatus(JsonInviteTargetUsersJobStatus jsonModel) : IJsonModel<JsonInviteTargetUsersJobStatus>
{
    JsonInviteTargetUsersJobStatus IJsonModel<JsonInviteTargetUsersJobStatus>.JsonModel => jsonModel;

    public InviteTargetUsersJobStatusCode Status => jsonModel.Status;

    public int TotalUsers => jsonModel.TotalUsers;

    public int ProcessedUsers => jsonModel.ProcessedUsers;

    public DateTimeOffset CreatedAt => jsonModel.CreatedAt;

    public DateTimeOffset? CompletedAt => jsonModel.CompletedAt;

    public string? ErrorMessage => jsonModel.ErrorMessage;
}

public enum InviteTargetUsersJobStatusCode
{
    Unspecified = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
}
