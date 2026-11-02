using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents an account as part of an <see cref="Integration"/> object.
/// </summary>
/// <param name="jsonModel">The JSON model to create an <see cref="Integration"/> object from.</param>
public class IntegrationAccount(JsonIntegrationAccount jsonModel)
{
    /// <summary>
    /// The ID of the account.
    /// </summary>
    public string Id { get; } = jsonModel.Id;

    /// <summary>
    /// The name of the account.
    /// </summary>
    public string Name { get; } = jsonModel.Name;
}
