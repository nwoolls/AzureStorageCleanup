using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureStorageCleanup
{
    class Program
    {
        static class CommandLineArgument
        {
            internal const string StorageName = "-storagename";
            internal const string StorageKey = "-storagekey";
            internal const string Container = "-container";
            internal const string MinDaysOld = "-mindaysold";
            internal const string Recursive = "-recursive";
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

            bool recursive = args.Contains(CommandLineArgument.Recursive);
            
            StorageCleaner storageCleaner = new StorageCleaner(storageAccountName, storageAccessKey, containerName, minDaysOld, recursive);
            storageCleaner.Cleanup();
        }

        private static void WriteHelpToConsole()
        {
            Console.WriteLine(@"
    Windows Azure Storage Cleanup Utility

    This utility will delete the blob files within the specified Azure Storage container older than the specified number of days.

    Please supply for following command line arguments:

        -storagename [Blob Storage account name]
        -storagekey  [Blob Storage account key]
        -container   [Blob Storage container to use]
        -mindaysold  [Minimum age of files to delete]
        -recursive   [Optional - delete all files in virtual hierarchy]

    Example usage:

    AzureStorageCleanup.exe
        -storagename storageaccount
        -storagekey dmASdd1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFM...
        -container sqlbackup
        -mindaysold 60
        -recursive
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
