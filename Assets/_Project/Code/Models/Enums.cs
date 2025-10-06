    public enum Statuses : short
    {
        Ok = 0,
        New = 1,
        Complete = 2,
        Incomplete = 3,
        Missing = 4,
        Wishlist = 5,
        AdminRemoved = 6,
        Expired = 7,
        ReadyToMigrate = 8,
        Migrated = 9,
        Deleted = 99
    }

    public enum ReleaseStatus : short
    {
        Ok = 0,          // Release with playable tracks
        Virtual = 1,     // Imported from Last.fm or other external source
        Incomplete = 2,  // Needs more work, metadata or files missing
        Hidden = 3,      // Hidden from collections
        Deleted = 99     // Soft delete
    }

    public enum ReleaseType
    {
        Album,
        Single,
        EP,
        Live,
        Compilation,
        Other
    }
    
public enum ErrorType:short
{
    Connection=0,
    Api
}

public enum LookupSuggestion : short
{
    Artists,
    Genres,
    Labels
}
