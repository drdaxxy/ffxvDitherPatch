using Craf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrappyCrafCrafter
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("CrappyCrafCrafter.exe list <archive path>");
            Console.WriteLine("CrappyCrafCrafter.exe extract <archive path> <destination folder>");
            Console.WriteLine("CrappyCrafCrafter.exe replace-single <archive path> <file-in-archive path> <replacement file on disk> <new archive path>");
            Console.WriteLine("CrappyCrafCrafter.exe replace-all <archive path> <folder with replacement files> <new archive path>");
            Console.WriteLine("");
            Console.WriteLine("replace-all leaves files not present in folder untouched");

            Environment.Exit(0);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("CrappyCrafCrafter 1.0 by drdaxxy");
            Console.WriteLine("Wholly untested, use at your own risk");
            Console.WriteLine("Reads whole archive and all replacement files into memory, have fun");
            Console.WriteLine("Not intended for public distribution because it's crappy\r\n\r\n");

            if (args.Length < 2) PrintUsage();
            switch (args[0].ToLowerInvariant())
            {
                case "list":
                    if (args.Length != 2) PrintUsage();
                    List(args[1]);
                    break;
                case "extract":
                    if (args.Length != 3) PrintUsage();
                    Extract(args[1], args[2]);
                    break;
                case "replace":
                    if (args.Length != 5) PrintUsage();
                    Replace(args[1], args[2], args[3], args[4]);
                    break;
                case "replace-all":
                    if (args.Length != 4) PrintUsage();
                    ReplaceAll(args[1], args[2], args[3]);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        static void List(string path)
        {
            try
            {
                var archive = CrafArchive.Open(path);
                for (var i = 0; i < archive.Count(); i++)
                {
                    Console.WriteLine(archive.DiskPath(i));
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open archive");
                Environment.Exit(-1);
            }
        }

        static void Extract(string archivePath, string destinationFolder)
        {
            CrafArchive archive = null;
            try
            {
                archive = CrafArchive.Open(archivePath);
                var task = archive.LoadAsync(new Progress<int>());
                task.Wait();
                archive.CloseReader();
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open archive");
                Environment.Exit(-1);
            }

            var ct = archive.Count();

            try
            {
                Directory.CreateDirectory(destinationFolder);
                for (var i = 0; i < ct; i++)
                {
                    var outputPath = Path.Combine(destinationFolder, archive.DiskPath(i));
                    Console.WriteLine(string.Format("{0} ({1}/{2})", outputPath, i + 1, ct));
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                    File.WriteAllBytes(outputPath, archive.Get(i));
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error while extracting archive");
                Console.WriteLine("Details: " + ex.Message);
                Environment.Exit(-1);
            }
        }

        static void Replace(string archivePath, string fileInArchive, string fileOnDisk, string newArchivePath)
        {
            CrafArchive archive = null;
            try
            {
                archive = CrafArchive.Open(archivePath);
                var task = archive.LoadAsync(new Progress<int>());
                task.Wait();
                archive.CloseReader();
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open archive");
                Environment.Exit(-1);
            }

            var id = archive.IndexOfDiskPath(fileInArchive);
            if (id == -1)
            {
                Console.WriteLine("File in archive not found");
                Environment.Exit(-1);
            }

            Console.WriteLine("Loading new file...");
            try
            {
                archive.Replace(id, File.ReadAllBytes(fileOnDisk));
            }
            catch (IOException ex)
            {
                Console.WriteLine("Could not read replacement file");
                Console.WriteLine("Details: " + ex.Message);
                Environment.Exit(-1);
            }

            Console.WriteLine("Writing back files...");
            var fivePercent = archive.Count() / 20;
            if (fivePercent == 0) fivePercent = 1;
            try
            {
                var task2 = archive.SaveAsync(newArchivePath, new Progress<int>((i) =>
                {
                    if (i % fivePercent == 0) Console.Write(".");
                }));
                task2.Wait();
            }
            catch (IOException ex)
            {
                Console.WriteLine("");
                Console.WriteLine("Could not write new archive");
                Console.WriteLine("Details: " + ex.Message);
                Environment.Exit(-1);
            }
            Console.WriteLine("");
        }

        static void ReplaceAll(string archivePath, string replacementFolder, string newArchivePath)
        {
            CrafArchive archive = null;
            try
            {
                archive = CrafArchive.Open(archivePath);
                var task = archive.LoadAsync(new Progress<int>());
                task.Wait();
                archive.CloseReader();
            }
            catch (IOException)
            {
                Console.WriteLine("Could not open archive");
                Environment.Exit(-1);
            }

            var ct = archive.Count();

            Console.WriteLine("Loading replacement files...");
            for (var i = 0; i < ct; i++)
            {
                var fileInArchive = archive.DiskPath(i);
                var combined = Path.Combine(replacementFolder, fileInArchive);
                if (File.Exists(combined))
                {
                    Console.WriteLine(string.Format("{0} ({1}/{2})", combined, i + 1, ct));
                    try
                    {
                        archive.Replace(i, File.ReadAllBytes(combined));
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("Could not read replacement file");
                        Console.WriteLine("Details: " + ex.Message);
                        Environment.Exit(-1);
                    }
                }
            }

            Console.WriteLine("");

            Console.WriteLine("Writing back files...");
            var fivePercent = ct / 20;
            if (fivePercent == 0) fivePercent = 1;
            try
            {
                var task2 = archive.SaveAsync(newArchivePath, new Progress<int>((i) =>
                {
                    if (i % fivePercent == 0) Console.Write(".");
                }));
                task2.Wait();
            }
            catch (IOException ex)
            {
                Console.WriteLine("");
                Console.WriteLine("Could not write new archive");
                Console.WriteLine("Details: " + ex.Message);
                Environment.Exit(-1);
            }
            Console.WriteLine("");
        }
    }
}
