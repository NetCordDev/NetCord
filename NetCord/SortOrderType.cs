namespace NetCord;

/// <summary>
/// Determines the sort order for posts within a <see cref="ForumGuildChannel"/>.
/// </summary>
public enum SortOrderType
{
    /// <summary>
    /// Sort forum posts by recent activity.
    /// </summary>
    LatestActivity = 0,

    /// <summary>
    /// Sort forum posts by their creation date, from newest to oldest.
    /// </summary>
    CreationDate = 1,
}
