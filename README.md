Windows Azure Storage Cleanup Utility
=======================================

This utility was designed as a companion to the [SQLDatabaseBackup][1] project. The format of the command line arguments and documentation were created to be consistent with that project.

This utility will delete the files stored in the specified Azure Storage account equal-to-or-older-than the specified number of days.

Please supply for following command line arguments:

```
    -storagename [Blob Storage account name]
    -storagekey  [Blob Storage account key]
    -container   [Blob Storage container to use]
    -mindaysold  [Minimum age of files to delete]
```

Example usage:

```
AzureStorageCleanup.exe 
    -storagename storageaccount
    -storagekey dmASdd1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFM...
    -container sqlbackup
    -mindaysold 60
```

### How it works

The utility will iterate through the blob files equal-to-or-older-than the specified number of days and delete them, logging information to the console.

### License

MIT

[1]: https://github.com/richorama/SQLDatabaseBackup
