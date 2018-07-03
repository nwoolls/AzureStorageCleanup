using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AzureStorageCleanup
{
    class Program
    {
        const string DEFAULT_SEARCH_PATTERN = ".*";

        static class CommandLineArgument
        {
            internal const string StorageName = "-storagename";
            internal const string StorageKey = "-storagekey";
            internal const string Container = "-container";
            internal const string MinDaysOld = "-mindaysold";
            internal const string SearchPattern = "-searchpattern";
            internal const string Recursive = "-recursive";
            internal const string WhatIf = "-whatif";
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHelpToConsole();
                return;
            }

            if (!CheckArgumentsExist(CommandLineArgument.StorageName, CommandLineArgument.StorageKey, CommandLineArgument.Container, CommandLineArgument.MinDaysOld))
                return;

            string storageAccountName = GetArgumentValue(CommandLineArgument.StorageName);  // storage account
            string storageAccessKey = GetArgumentValue(CommandLineArgument.StorageKey);     // storage key
            string containerName = GetArgumentValue(CommandLineArgument.Container);         // storage container

            bool success = false;
            int minDaysOld = -1;
            success = int.TryParse(GetArgumentValue(CommandLineArgument.MinDaysOld), out minDaysOld);

            if (!success || (minDaysOld < 0))
            {
                Console.WriteLine(string.Format("Please supply a valid \"{0}\" argument", CommandLineArgument.MinDaysOld));
                return;
            }

            string searchPattern = GetArgumentValue(CommandLineArgument.SearchPattern);
            if (searchPattern == null)
            {
                searchPattern = DEFAULT_SEARCH_PATTERN;
            }

            try
            {
                var regex = new Regex(searchPattern);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("searchpattern regex compilation failed: {0}", ex.Message));
                return;
            }

            bool recursive = args.Contains(CommandLineArgument.Recursive);
            bool whatif = args.Contains(CommandLineArgument.WhatIf);

            StorageCleaner storageCleaner = new StorageCleaner(storageAccountName, storageAccessKey, containerName, minDaysOld, searchPattern, recursive, whatif);
            storageCleaner.Cleanup();
        }

        private static void WriteHelpToConsole()
        {
            Console.WriteLine(@"
    Windows Azure Storage Cleanup Utility

    This utility will delete the blob files within the specified Azure Storage container older than the specified number of days.

    Please supply for following command line arguments:

        -storagename   [Blob Storage account name]
        -storagekey    [Blob Storage account key]
        -container     [Blob Storage container to use]
        -mindaysold    [Minimum age of files to delete]
        -searchpattern [Optional - Regex pattern to attempt to match against each blob name. (Default: match all files)]
        -recursive     [Optional - delete all files in virtual hierarchy]
        -whatif        [Optional - if this flag is included then only log output will be produced]

    Example usage:

    AzureStorageCleanup.exe
        -storagename storageaccount
        -storagekey dmASdd1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFM...
        -container sqlbackup
        -mindaysold 60
        -searchpattern .*
        -recursive
        -whatif
    ");
        }

        public static string GetArgumentValue(string argumentName)
        {
            if (string.IsNullOrEmpty(argumentName)) throw new ArgumentNullException("argumentName");

            string[] arguments = Environment.GetCommandLineArgs();
            List<string> lowerArguments = new List<string>(arguments.Select(x => x.ToLower()));
            int index = lowerArguments.IndexOf(argumentName.ToLower());

            if (index == -1)
                return null;

            if (arguments.Length < index + 2)
                return null;

            return arguments[index + 1];
        }

        public static bool CheckArgumentsExist(params string[] argumentNames)
        {
            bool returnVal = true;

            foreach (string arg in argumentNames)
            {
                if (GetArgumentValue(arg) == null)
                {
                    Console.WriteLine(String.Format("Please supply the \"{0}\" argument", arg));
                    returnVal = false;
                }
            }

            return returnVal;
        }
    }
}
