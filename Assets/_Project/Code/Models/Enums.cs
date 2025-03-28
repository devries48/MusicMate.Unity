﻿    public enum Statuses : short
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

    public enum WorkflowStatus : short
    {
        Imported = 0,
        Incomplete,
        Processed,
    }

public enum ErrorType:short
{
    Connection=0,
    Api
}
