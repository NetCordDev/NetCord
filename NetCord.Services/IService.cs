using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Services;

/// <summary>
/// Base interface for services that provides module management functionality.
/// Implemented by <see cref="ApplicationCommandService{TContext}"/>, <see cref="ComponentInteractionService{TContext}"/>, and <see cref="CommandService{TContext}"/>.
/// </summary>
public interface IService
{
    /// <summary>
    /// Scans the specified assembly for public modules and registers them with the service.
    /// </summary>
    /// <param name="assembly">The assembly to scan for modules.</param>
    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly);
}
