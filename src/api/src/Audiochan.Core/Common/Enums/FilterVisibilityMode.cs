namespace Audiochan.Core.Common.Enums
{
    public enum FilterVisibilityMode
    {
        /// <summary>
        /// Only show entities that are public, unless the user owns the entity
        /// </summary>
        OnlyPublic,
            
        /// <summary>
        /// Only show entities that are either public or private, unless the user owns the entity
        /// </summary>
        Unlisted
    }
}