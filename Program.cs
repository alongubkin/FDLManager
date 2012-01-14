using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using NDesk.Options;

namespace FDLManager
{
    internal class Program
    {
        private string _destinationDirectory;
        private string _gameDirectoryName;
        private string _listFileName;
        private MD5 _md5 = MD5.Create();
        private List<string> _sourceDirectories = new List<string>();
        private List<string> _unnecssaryFiles = new List<string>();

        private void AddFile(string path)
        {
            string fileNameInDestination = this.GetFileNameInDestination(path);

            if (!Directory.Exists(Path.GetDirectoryName(fileNameInDestination)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileNameInDestination));
            }

            string contents = this.GetMd5Sum(File.ReadAllBytes(path));

            using (BZip2OutputStream stream = new BZip2OutputStream(new FileStream(fileNameInDestination + ".bz2", FileMode.Create, FileAccess.Write)))
            {
                byte[] buffer = File.ReadAllBytes(path);
                stream.Write(buffer, 0, buffer.Length);
            }

            File.WriteAllText(this.GetDestinationHashLocation(fileNameInDestination), contents);
        }

        private string GetDestinationHashLocation(string dest)
        {
            string directoryName = Path.GetDirectoryName(dest);
            string fileName = Path.GetFileName(dest);

            return Path.Combine(directoryName, string.Format("{0}.hash.txt", fileName));
        }

        private string GetFileNameInDestination(string path)
        {
            return Path.Combine(this._destinationDirectory, this.GetRelativePath(path).Substring(1));
        }

        private string GetMd5Sum(byte[] buffer)
        {
            byte[] buffer2 = this._md5.ComputeHash(buffer);
            var builder = new StringBuilder();

            for (int i = 0; i < buffer2.Length; i++)
            {
                builder.Append(buffer2[i].ToString("X2"));
            }

            return builder.ToString();
        }

        private string GetRelativePath(string path)
        {
            var builder = new StringBuilder();

            foreach (string part in path.Split(new char[] { Path.DirectorySeparatorChar }))
            {
                if (part == this._gameDirectoryName)
                {
                    return builder.ToString();
                }

                builder.Append(Path.DirectorySeparatorChar);
                builder.Append(part);
            }

            return string.Empty;
        }

        private static void Main(string[] args)
        {
            var program = new Program();

            new OptionSet()
                .Add("g|game=", new Action<string>((v) => { program._gameDirectoryName = v; }))
                .Add("d|dest=", new Action<string>((v) => { program._destinationDirectory = v; }))
                .Add("s|source=", (v) => {
                    if (Directory.Exists(v))
                    {
                        program._sourceDirectories.Add(v);
                    }
                    else
                    {
                        Console.WriteLine("Source directory {0} doesn't exists", v);
                    }
                })
                .Add("l|list=", new Action<string>((v) => { program._listFileName = v; }))
                .Parse(args);

            program.ScanAll();
        }

        private void ScanAll()
        {
            if (string.IsNullOrEmpty(this._listFileName))
            {
                this.ScanAndCopy(false);
            }
            else if (File.Exists(this._listFileName))
            {
                this.ScanAndCopy(true);
            }
            else
            {
                this.ScanAndAdd();
            }
        }

        private void ScanAndAdd()
        {
            using (StreamWriter writer = new StreamWriter(this._listFileName))
            {
                foreach (string directory in this._sourceDirectories)
                {
                    this.ScanAndAdd(directory, writer);
                }
            }
        }

        private void ScanAndAdd(string path, StreamWriter streamWriter)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                string relativePath = this.GetRelativePath(file);

                if (!this._unnecssaryFiles.Contains(relativePath))
                {
                    this._unnecssaryFiles.Add(relativePath);
                    streamWriter.WriteLine(relativePath);
                }
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                this.ScanAndAdd(directory, streamWriter);
            }
        }

        private void ScanAndCopy(bool list)
        {
            if (list)
            {
                this._unnecssaryFiles = new List<string>(File.ReadAllLines(this._listFileName));
            }

            foreach (string directory in this._sourceDirectories)
            {
                Console.WriteLine("Scanning {0}.", directory);
                this.ScanAndCopy(directory);
            }
        }

        private void ScanAndCopy(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (!this._unnecssaryFiles.Contains(this.GetRelativePath(file)))
                {
                    string fileNameInDestination = this.GetFileNameInDestination(file);
                    string dest = fileNameInDestination + ".bz2";

                    if (File.Exists(dest))
                    {
                        var hash = this.GetMd5Sum(File.ReadAllBytes(file));
                        var destinationHashLocation = this.GetDestinationHashLocation(fileNameInDestination);
                        
                        if (File.Exists(destinationHashLocation))
                        {
                            var text = File.ReadAllText(destinationHashLocation);

                            if (hash != text)
                            {
                                Console.WriteLine("Updating file: {0}.", file, hash, text);

                                File.Delete(dest);
                                File.Delete(this.GetDestinationHashLocation(fileNameInDestination));

                                this.AddFile(file);
                            }
                        }
                        else
                        {
                            File.Delete(dest);
                            this.AddFile(file);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Copying file: {0}.", file);
                        this.AddFile(file);
                    }
                }
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                this.ScanAndCopy(directory);
            }
        }
    }
}

