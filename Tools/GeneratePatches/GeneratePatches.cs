// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CommandLine;
using DiffMatchPatch;
using GadrocsWorkshop.Helios.Patching.DCS;
using LibGit2Sharp;
using Newtonsoft.Json;
using ToolsCommon;
using Diff = DiffMatchPatch.Diff;
using Patch = DiffMatchPatch.Patch;

namespace GeneratePatches
{
    internal class GeneratePatches
    {
        public class CommonOptions
        {
            [Option('d', "dcsroot", Required = true, HelpText = "Full path to DCS installation on which to operate.")]
            public string DcsRoot { get; set; }
        }

        [Verb("init",
            HelpText =
                "Initialize a git repo in the DCS installation and save all configuration lua under git for comparison later.")]
        public class InitOptions : CommonOptions
        {
            // nothing
        }

        [Verb("generate",
            HelpText =
                "Generate patches for all differences between current working directory and last git commit of DCS files.")]
        public class GenerateOptions : CommonOptions
        {
            [Option('p', "patchset", Required = false, Default = "Viewports",
                HelpText = "Name of patch set to generate.")]
            public string PatchSet { get; set; }

            [Option('o', "outputpath", Required = false, Default = null,
                HelpText =
                    "Path to 'Patches' folder.  If not specified, will find nearest folder 'Patching\\Patches' in directory tree above.")]
            public string OutputPath { get; set; }

            [Option('m', "setmargin", Required = false, Default = 4,
                HelpText =
                "value to set the patch-margin size to (default is 4) which may need to be increased if there are multiple changes on a single line.  Typically the need for increasing this is seen when the patch applies, but does not verify.")]
            public int SetMargin { get; set; }
        }
        private class AutoUpdateConfig
        {
            [JsonProperty("version")] public string Version { get; private set; }
        }

        private static void Main(string[] args)
        {
            ParserResult<object> result = Parser.Default.ParseArguments<InitOptions, GenerateOptions>(args)
                .WithParsed<InitOptions>(options => { InitializeRepo(options.DcsRoot); })
                .WithParsed<GenerateOptions>(options =>
                {
                    UpdatePatches(options.DcsRoot, options.PatchSet, options.OutputPath, options.SetMargin);
                });
        }

        private static void UpdatePatches(string dcsRoot, string patchSet, string outputPath, int setMargin)
        {
            // determine DCS version
            string autoUpdatePath = Path.Combine(dcsRoot, "autoupdate.cfg");
            string versionString = JsonConvert.DeserializeObject<AutoUpdateConfig>(File.ReadAllText(autoUpdatePath))
                .Version;
            string dcsVersion = PatchVersion.SortableString(versionString);

            // now build patches based on files changed in repo
            string patchesPath = outputPath ?? FileSystem.FindNearestDirectory("Patching\\Patches");
            Console.WriteLine($"writing patches for {dcsVersion} to {patchesPath}");
            int nCount = 0;
            using (Repository repo = new Repository(dcsRoot))
            {
                foreach (StatusEntry item in repo.RetrieveStatus(new StatusOptions()))
                {
                    if (item.State == FileStatus.ModifiedInWorkdir)
                    {
                        string source;
                        string target;
                        Debug.WriteLine(item.FilePath);
                        Blob blob = repo.Head.Tip[item.FilePath].Target as Blob;
                        using (var content = new StreamReader(blob.GetContentStream(), Encoding.UTF8))
                        {
                            source = content.ReadToEnd();
                        }

                        string workingPath = Path.Combine(repo.Info.WorkingDirectory, item.FilePath);
                        using (var content = new StreamReader(workingPath, Encoding.UTF8))
                        {
                            target = content.ReadToEnd();
                        }

                        string patchPath = Path.Combine(patchesPath, "DCS", dcsVersion, patchSet,
                            $"{item.FilePath}.gpatch");
                        string reversePath = Path.Combine(patchesPath, "DCS", dcsVersion, patchSet,
                            $"{item.FilePath}.grevert");
                        string outputDirectoryPath = Path.GetDirectoryName(patchPath);
                        if (outputDirectoryPath != null && !Directory.Exists(outputDirectoryPath))
                        {
                            Directory.CreateDirectory(outputDirectoryPath);
                        }

                        Console.WriteLine($"writing patch {patchPath}");
                        WritePatch(source, target, patchPath, setMargin);
                        WritePatch(target, source, reversePath, setMargin);
                        nCount++;
                    }
                }
            }

            if (nCount == 0)
            {
                Console.WriteLine($"no patch files written, does {dcsRoot} not have any current changes in the working directory?");
            }
        }

        private static void WritePatch(string source, string target, string outputPath, int setMargin)
        {
            // NOTE: do our own diffs so we just do semantic cleanup. 
            // We don't want to optimize for efficiency.
            diff_match_patch googleDiff = new diff_match_patch()
            {
                Patch_Margin = (short) setMargin,
                //Diff_EditCost = 16,
            };
            List<Diff> diffs = googleDiff.diff_main(source, target);
            googleDiff.diff_cleanupSemantic(diffs);

            // write patch
            List<Patch> patches = googleDiff.patch_make(diffs);
            using (StreamWriter streamWriter = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                string patchText = googleDiff.patch_toText(patches);
                streamWriter.Write(patchText);
            }
        }

        private static void InitializeRepo(string dcsRoot)
        {
            Console.WriteLine($"Initializing git repo at '{dcsRoot}' and checking in lua files");
            if (!Directory.Exists(Path.Combine(dcsRoot, ".git")))
            {
                Repository.Init(dcsRoot);
            }
            InitializeGitConfig(dcsRoot, "gitignore");
            InitializeGitConfig(dcsRoot, "gitattributes");

            using (Repository repo = new Repository(dcsRoot))
            {
                Commands.Stage(repo, "*");
                Signature author = new Signature("Helios GeneratePatches", "@anonymous", DateTime.Now);
                try
                {
                    Commit commit = repo.Commit("Initial checkin of clean files", author, author);
                }
                catch (EmptyCommitException)
                {
                    Console.WriteLine("all non-ignored files are already committed");
                }
            }
        }

        private static void InitializeGitConfig(string dcsRoot, string configFile)
        {
            string configPath = Path.Combine(dcsRoot, $".{configFile}");
            if (File.Exists(configPath))
            {
                return;
            }

            string sourcePath = FileSystem.FindNearestDirectory("Tools\\GeneratePatches\\Setup\\DCS") + configFile;
            string config = File.ReadAllText(sourcePath);
            File.WriteAllText(configPath, config);
        }
    }
}