namespace NetCord;

/// <summary>
/// Determines the visual post layout of a <see cref="ForumGuildChannel"/>.
/// </summary>
public enum ForumLayoutType
{
    /// <summary>
    /// No default post layout set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// Posts are displayed as a sequential list.
    /// </summary>
    ListView = 1,

    /// <summary>
    /// Posts are displayed as a collection of tiles.
    /// </summary>
    GalleryView = 2,
}
