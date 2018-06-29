Windows Azure Storage Cleanup Utility
=======================================

This utility was designed as a companion to the [SQLDatabaseBackup][1] project. The format of the command line arguments and documentation were created to be consistent with that project.

This utility will delete files stored in the specified Azure Storage account equal-to-or-older-than the specified number of days. Optionally, a regex may also be supplied in order to target a subset of file paths.

Please supply for following command line arguments:

```
    -storagename   [Blob Storage account name]
    -storagekey    [Blob Storage account key]
    -container     [Blob Storage container to use]
    -mindaysold    [Minimum age of files to delete]
    -searchpattern [Optional - Regex pattern to attempt to match against each blob name. (Default: match all files)]
    -recursive     [Optional - delete all files in virtual hierarchy]
    -whatif        [Optional - if this flag is included then only log output will be produced]
```

Example usage:

```
AzureStorageCleanup.exe 
    -storagename storageaccount
    -storagekey dmASdd1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFM...
    -container sqlbackup
    -mindaysold 60
    -searchpattern .*
    -recursive
    -whatif
```

### How it works

The utility will iterate through the blob files equal-to-or-older-than the specified number of days and delete them, logging information to the console.

### License

MIT

[1]: https://github.com/richorama/SQLDatabaseBackup
