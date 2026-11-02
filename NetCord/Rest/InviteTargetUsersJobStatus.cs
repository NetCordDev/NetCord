using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class InviteTargetUsersJobStatus(JsonInviteTargetUsersJobStatus jsonModel)
{
    public InviteTargetUsersJobStatusCode Status { get; } = jsonModel.Status;

    public int TotalUsers { get; } = jsonModel.TotalUsers;

    public int ProcessedUsers { get; } = jsonModel.ProcessedUsers;

    public DateTimeOffset CreatedAt { get; } = jsonModel.CreatedAt;

    public DateTimeOffset? CompletedAt { get; } = jsonModel.CompletedAt;

    public string? ErrorMessage { get; } = jsonModel.ErrorMessage;
}

public enum InviteTargetUsersJobStatusCode
{
    Unspecified = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
}
