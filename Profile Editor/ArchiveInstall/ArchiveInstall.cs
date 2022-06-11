// Copyright 2020 Ammo Goettsch
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Windows;
using Newtonsoft.Json;

namespace GadrocsWorkshop.Helios.ProfileEditor.ArchiveInstall
{
    internal class ArchiveInstall : IInstallation2
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// the full path to the archive file
        /// </summary>
        private readonly string _archivePath;

        /// <summary>
        /// how to decide whether to overwrite files (other than Profiles) that already exist
        /// </summary>
        private OverwriteStrategy _defaultOverwriteStrategy = OverwriteStrategy.ReadOnlyFiles;

        /// <summary>
        /// UI element that will host our modal dialogs
        /// </summary>
        private readonly IInputElement _host;

        /// <summary>
        /// well known name of a manifest file to place in the root of the zip
        /// </summary>
        private const string MANIFEST_PATH = "Profile16.json";

        /// <summary>
        /// relative paths within the archive that we will not extract
        /// </summary>
        private readonly HashSet<string> _pathExclusions =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public ArchiveInstall(string archivePath, IInputElement host)
        {
            _archivePath = archivePath;
            _host = host;
        }

        /// <summary>
        /// discovered profiles in the archive, if not excluded
        /// </summary>
        public List<string> ProfilePaths { get; } = new List<string>();

        /// <summary>
        /// true if there is an obvious choice of which profile to configure after installation
        /// </summary>
        public bool HasMainProfile => ProfilePaths.Count == 1;

        /// <summary>
        /// only valid if HasMainProfile is true
        /// </summary>
        public string MainProfilePath => ProfilePaths.FirstOrDefault();

        public string ArchivePath => Path.GetFileName(_archivePath);

        public InstallationResult Install(IInstallationCallbacks2 callbacks)
        {
            ProfileManifest16 manifest = null;
            try
            {
                using (ZipArchive archive = ZipFile.Open(_archivePath, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry manifestEntry = archive.Entries.FirstOrDefault(entry =>
                        entry.FullName.Equals(MANIFEST_PATH, StringComparison.CurrentCultureIgnoreCase));
                    if (manifestEntry != null)
                    {
                        manifest = LoadManifest(manifestEntry);
                        if (manifest == null)
                        {
                            // this cannot happen because we would get a parse failure?  maybe empty file?
                            throw new Exception($"JSON manifest {MANIFEST_PATH} could not be parsed");
                        }

                        // see if we are allowed to install this package
                        if (!CheckVersionRequirements(manifest, out IList<StatusReportItem> problems))
                        {
                            if (RunningVersion.IsDevelopmentPrototype ||
                                ConfigManager.SettingsManager.LoadSetting("ArchiveInstall", "VersionOverride", false))
                            {
                                InstallationPromptResult result = callbacks.DangerPrompt($"{ArchivePath} Installation",
                                    "Installation requirements are not met.  Do you want to install anyway?", problems);
                                if (result == InstallationPromptResult.Cancel)
                                {
                                    return InstallationResult.Canceled;
                                }
                            }
                            else
                            {
                                callbacks.Failure($"{ArchivePath} cannot be installed", "Installation requirements are not met", problems);
                                return InstallationResult.Canceled;
                            }
                        }

                        // present welcome screen, if applicable
                        if (!ShowWelcome(callbacks, manifest))
                        {
                            callbacks.Failure($"{ArchivePath} installation canceled", "Installation canceled by user", new List<StatusReportItem>());
                            return InstallationResult.Canceled;
                        }

                        // process selection and build exclusion list
                        foreach (Choice choice in manifest.Choices)
                        {
                            // filter options based on version requirements
                            List<StatusReportItem> details = new List<StatusReportItem>();
                            foreach (Option option in choice.Options)
                            {
                                if (!CheckVersionRequirements(option, out IList<StatusReportItem> optionDetails))
                                {
                                    // this option is not allowed
                                    details.AddRange(optionDetails);
                                    option.IsValid = false;
                                    option.ValidityNarrative = "Version requirements not met";
                                }
                            }

                            // check if we still have viable choices
                            if (!choice.Options.Any())
                            {
                                callbacks.Failure($"{ArchivePath} cannot be installed", "None of the options for a required choice are valid for your installation.", details);
                                return InstallationResult.Fatal;
                            }

                            // fix up dialog if not specified
                            choice.Message = choice.Message ?? "The Profile Archive being installed contains multiple versions of some of its files.  Please choose one version to install:";

                            // wait for response
                            Option selected = PresentChoice(choice);

                            // check if dialog closed or canceled
                            if (selected == null)
                            {
                                return InstallationResult.Canceled;
                            }

                            // process results
                            if (selected?.PathExclusions != null)
                            {
                                foreach (string path in selected.PathExclusions)
                                {
                                    _pathExclusions.Add(path);
                                }
                            }
                        }
                    }

                    // first check if we have a whole lot of writable files, in which case ask the user if the overwrite strategy should be changed
                    CheckForFirstArchiveInstall(archive);

                    // now unpack everything
                    IList<StatusReportItem> report = archive.Entries
                        .Where(NotFilteredVerbose)
                        .Select(Unpack)
                        .Where(item => item != null)
                        .ToList();

                    callbacks.Success($"Installed {ArchivePath}",
                        $"Files were installed into {Anonymizer.Anonymize(ConfigManager.DocumentPath)}", report);
                }

                return InstallationResult.Success;
            }
            catch (Exception ex)
            {
                callbacks.Failure($"Failed to install {ArchivePath}",
                    $"Attempt to install Helios Profile from '{Anonymizer.Anonymize(_archivePath)}' failed",
                    ReportException(ex));
                return InstallationResult.Fatal;
            }
        }

        private bool ShowWelcome(IInstallationCallbacks2 callbacks, ProfileManifest16 manifest)
        {
            if (string.IsNullOrEmpty(manifest.Description) && (manifest.Info == null || !manifest.Info.Any()))
            {
                // nothing to present, continue
                return true;
            }

            List<StructuredInfo> info = new List<StructuredInfo>();
            
            // add info that has its own properties
            if (manifest.Authors != null)
            {
                info.AddRange(manifest.Authors.Select(manifestAuthor => new StructuredInfo("Author", manifestAuthor)));
            }
            if (!string.IsNullOrEmpty(manifest.License))
            {
                info.Add(new StructuredInfo("License", manifest.License));
            }
            if (!string.IsNullOrEmpty(manifest.Version))
            {
                info.Add(new StructuredInfo("Version", manifest.Version));
            }

            // gather structured info
            if (null != manifest.Info)
            {
                info.AddRange(manifest.Info);
            }

            return callbacks.DangerPrompt(
                $"{ArchivePath} Installation", 
                $"Helios is about to install: {manifest.Description ?? ArchivePath}", 
                info,
                new List<StatusReportItem>()) == InstallationPromptResult.Ok;
        }

        private bool CheckVersionRequirements(ProfileManifest16 manifest, out IList<StatusReportItem> problems)
        {
            List<StatusReportItem> report = new List<StatusReportItem>();

            if (manifest.VersionsRequired == null)
            {
                problems = report;
                return true;
            }
            foreach (VersionRequired versionRequired in manifest.VersionsRequired)
            {
                if (!FetchVersion(versionRequired.Product, out Version version))
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"Helios does not recognize product '{versionRequired.Product}' which is required by this archive",
                        Recommendation =
                            "find a version of this archive for the Helios distribution that you have or manually edit the archive contents at your own risk",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                    continue;
                }
                if (versionRequired.Minimum != null && version < versionRequired.Minimum)
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"This archive requires {versionRequired.Product} version {versionRequired.Minimum} or higher. You have version {version}.",
                        Recommendation =
                            $"upgrade {versionRequired.Product} or find a version of this archive for the {versionRequired.Product} version you have",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                    continue;
                }
                if (versionRequired.Maximum != null && version > versionRequired.Maximum)
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"This archive requires {versionRequired.Product} version {versionRequired.Maximum} or lower. You have version {version}.",
                        Recommendation =
                            $"find a version of this archive for the {versionRequired.Product} version you have",
                        Severity = StatusReportItem.SeverityCode.Warning,
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                }
            }

            // return success only if we reported nothing
            problems = report;
            return !report.Any();
        }

        private bool CheckVersionRequirements(Option option, out IList<StatusReportItem> problems)
        {
            List<StatusReportItem> report = new List<StatusReportItem>();

            if (option.VersionsRequired == null)
            {
                problems = report;
                return true;
            }
            foreach (VersionRequired versionRequired in option.VersionsRequired)
            {
                if (!FetchVersion(versionRequired.Product, out Version version))
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"Helios does not recognize product '{versionRequired.Product}' which is required by {option.Description ?? "one of the options"} ",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                    continue;
                }
                if (versionRequired.Minimum != null && version < versionRequired.Minimum)
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"{option.Description ?? "One of the options"} requires {versionRequired.Product} version {versionRequired.Minimum} or higher. You have version {version}.",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                    continue;
                }
                if (versionRequired.Maximum != null && version > versionRequired.Maximum)
                {
                    report.Add(new StatusReportItem
                    {
                        Status =
                            $"{option.Description ?? "One of the options"} requires {versionRequired.Product} version {versionRequired.Maximum} or lower. You have version {version}.",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                }
            }

            // return success only if we reported nothing
            problems = report;
            return !report.Any();
        }

        private bool FetchVersion(string versionRequiredProduct, out Version version)
        {
            switch (versionRequiredProduct)
            {
                case "Helios":
                    version = RunningVersion.FromHeliosAssembly();
                    return true;
                default:
                    version = null;
                    return false;
            }
        }

        private void CheckForFirstArchiveInstall(ZipArchive archive)
        {
            int numExisting = 0;
            int numReadOnly = archive.Entries
                .Where(NotFiltered)
                .Select(entry => CheckExpectedReadonly(ref numExisting, entry))
                .Where(path => path != null)
                .Count(IsReadOnly);
            if (numExisting == 0 || numReadOnly >= numExisting * 0.9)
            {
                return;
            }

            // have never used an archive before, so our files are not marked read only, we should probably replace all of them
            Option overwrite = new Option
            {
                Description = "Overwrite existing files with fresh versions from archive"
            };
            Choice choice = new Choice
            {
                Message =
                    "It seems like some files included in the current archive were previously installed without using an archive, such as by copying.  Please make sure you have saved any of these files you may have changed (this is not common) and choose one of the following:",
                Options = new List<Option>
                {
                    overwrite,
                    new Option
                    {
                        Description = "Leave all existing files alone, because you modified them intentionally"
                    }
                }
            };
            if (overwrite == PresentChoice(choice))
            {
                _defaultOverwriteStrategy = OverwriteStrategy.Always;
            }
        }

        private string CheckExpectedReadonly(ref int numExisting, ZipArchiveEntry entry)
        {
            if (!ResolveArchiveEntry(entry, out string unpackedPath, out OverwriteStrategy overwriteStrategy,
                out bool _))
            {
                return null;
            }

            if (overwriteStrategy != OverwriteStrategy.ReadOnlyFiles)
            {
                // these are not expected to be readonly overwritable files
                return null;
            }

            if (!File.Exists(unpackedPath))
            {
                return null;
            }

            numExisting++;
            return unpackedPath;
        }

        private static bool IsReadOnly(string path) => File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly);

        private bool NotFiltered(ZipArchiveEntry entry) => !_pathExclusions.Contains(entry.FullName);

        private bool NotFilteredVerbose(ZipArchiveEntry entry)
        {
            if (!_pathExclusions.Contains(entry.FullName))
            {
                return true;
            }

            // filtered
            Logger.Info(
                "skipping profile archive entry {Path} because it is excluded via the user choices in the manifest {Manifest}",
                entry.FullName, MANIFEST_PATH);
            return false;
        }

        private Option PresentChoice(Choice choice)
        {
            ChoiceModel choiceModel = new ChoiceModel(choice);
            ShowModalParameter dialogParameter = new ShowModalParameter
            {
                Content = choiceModel
            };

            // run the dialog to completion
            Dialog.ShowModalCommand.Execute(dialogParameter, _host);
            return choiceModel.Selected;
        }

        private static ProfileManifest16 LoadManifest(ZipArchiveEntry manifestEntry)
        {
            ProfileManifest16 manifest;
            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader input = new StreamReader(manifestEntry.Open()))
            using (JsonTextReader jsonTextReader = new JsonTextReader(input))
            {
                manifest = serializer.Deserialize<ProfileManifest16>(jsonTextReader);
            }

            return manifest;
        }

        private List<string> PathSegments(string path) => EnumeratePathSegments(path).ToList();

        private IEnumerable<string> EnumeratePathSegments(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                yield break;
            }

            string lastSegment = Path.GetFileName(path);
            string parent = Path.GetDirectoryName(path);

            foreach (string prefix in EnumeratePathSegments(parent))
            {
                yield return prefix;
            }

            if (!string.IsNullOrEmpty(lastSegment))
            {
                yield return lastSegment;
            }
        }

        private StatusReportItem Unpack(ZipArchiveEntry entry)
        {
            if (!ResolveArchiveEntry(entry, out string unpackedPath, out OverwriteStrategy overwriteStrategy, out bool isProfile))
            {
                return null;
            }

            if (isProfile)
            {
                ProfilePaths.Add(unpackedPath);
            }

            if (File.Exists(unpackedPath))
            {
                FileInfo info = new FileInfo(unpackedPath);

                switch (overwriteStrategy)
                {
                    case OverwriteStrategy.Never:
                        return new StatusReportItem
                        {
                            Status =
                                $"not unpacking file '{entry.FullName}' because it already exists and we do not overwrite this type of file"
                        };
                    case OverwriteStrategy.ReadOnlyFiles:
                        // make sure file is tagged unmodified via readonly flag
                        if (!info.IsReadOnly)
                        {
                            return new StatusReportItem
                            {
                                Status =
                                    $"not unpacking file '{entry.FullName}' because it has been made writable by the user and therefore could be changed"
                            };
                        }

                        // overwrite authorized, check if we want to
                        if (info.LastWriteTimeUtc >= entry.LastWriteTime.UtcDateTime)
                        {
                            // already up to date
                            return null;
                        }

                        break;
                    case OverwriteStrategy.Always:
                        // overwrite without checking
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // clear readonly flag
                File.SetAttributes(unpackedPath, File.GetAttributes(unpackedPath) & ~FileAttributes.ReadOnly);
            }

            // overwrite checks done, write the file regardless
            using (Stream read = entry.Open())
            {
                using (Stream write = new FileStream(unpackedPath, FileMode.Create))
                {
                    read.CopyTo(write);
                }
            }
            File.SetLastWriteTimeUtc(unpackedPath, entry.LastWriteTime.UtcDateTime);
            if (!isProfile)
            {
                File.SetAttributes(unpackedPath, FileAttributes.ReadOnly);
            }

            return null;
        }

        private bool ResolveArchiveEntry(ZipArchiveEntry entry, out string unpackedPath,
            out OverwriteStrategy overwriteStrategy, out bool isProfile)
        {
            if (!GetPathSegments(entry, out List<string> pathSegments))
            {
                unpackedPath = null;
                overwriteStrategy = OverwriteStrategy.Never;
                isProfile = false;
                return false;
            }

            if (pathSegments[1].Equals("Profiles", StringComparison.InvariantCultureIgnoreCase))
            {
                // install a profile
                string profileFileName = pathSegments[2];
                unpackedPath = Path.Combine(ConfigManager.DocumentPath, "Profiles", profileFileName);
                overwriteStrategy = OverwriteStrategy.Never;
                isProfile = true;
            }
            else
            {
                // install another type of file
                string relativePath = string.Join(Path.DirectorySeparatorChar.ToString(), pathSegments.Skip(1));
                unpackedPath = Path.Combine(ConfigManager.DocumentPath, relativePath);
                string unpackedDirectory = Path.GetDirectoryName(unpackedPath);
                if (null != unpackedDirectory && !Directory.Exists(unpackedDirectory))
                {
                    Directory.CreateDirectory(unpackedDirectory);
                }

                overwriteStrategy = _defaultOverwriteStrategy;
                isProfile = false;
            }

            return true;
        }

        private bool GetPathSegments(ZipArchiveEntry entry, out List<string> validPathSegments)
        {
            if (entry.Length < 1)
            {
                // directory entry or empty file
                Logger.Debug("skipping profile archive entry {Path} without content", entry.FullName);
                validPathSegments = null;
                return false;
            }

            List<string>pathSegments = PathSegments(entry.FullName);
            if (pathSegments.Count < 3)
            {
                Logger.Debug(
                    "skipping profile archive entry {Path} because it it not within the supported directory structure",
                    entry.FullName);
                validPathSegments = null;
                return false;
            }

            if ((pathSegments[1].Equals("Helios", StringComparison.InvariantCultureIgnoreCase)
                && !pathSegments[0].Equals("Helios", StringComparison.InvariantCultureIgnoreCase)) 
                ||
                (pathSegments[1].Equals("HeliosDev", StringComparison.InvariantCultureIgnoreCase)
                && !pathSegments[0].Equals("HeliosDev", StringComparison.InvariantCultureIgnoreCase)))
            {
                // Helios folder is nested in profile name folder or similar
                pathSegments.RemoveAt(0);
            }

            // check again if we have enough path left
            if (pathSegments.Count < 3)
            {
                Logger.Debug(
                    "skipping profile archive entry {Path} because it it not within the supported directory structure (can't use Helios root)",
                    entry.FullName);
                validPathSegments = null;
                return false;
            }

            if (!(pathSegments[0].Equals("Helios", StringComparison.InvariantCultureIgnoreCase) || pathSegments[0].Equals("HeliosDev", StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.Debug(
                    "skipping profile archive entry {Path} because it it not within the supported directory structure (must use Helios folder)",
                    entry.FullName);
                validPathSegments = null;
                return false;
            }

            validPathSegments = pathSegments;
            return true;
        }

        private IList<StatusReportItem> ReportException(Exception ex) =>
            new List<StatusReportItem>
            {
                new StatusReportItem
                {
                    Status = $"Unexpected error from attempt to install Helios Archive '{_archivePath}'",
                    Recommendation =
                        "Make sure the specified file is a valid archive.  If this error persists, report it to the developer of this Profile or file a bug against Helios.",
                    Severity = StatusReportItem.SeverityCode.Error,
                    Code = $"{ex}{Environment.NewLine}{ex.StackTrace}"
                }
            };
    }
}