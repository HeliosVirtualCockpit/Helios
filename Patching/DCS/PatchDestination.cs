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
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Util.DCS;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    [DebuggerDisplay("{" + nameof(_dcsInstallationType) + "} in {" + nameof(_dcsRoot) + "}")]
    public class PatchDestination : IPatchDestinationWritable
    {
        private readonly string _dcsRoot;
        private readonly string _targetRoot;
        private readonly string _installationType;
        private readonly DCSIntallationType _dcsInstallationType;
        private readonly InstallationLocation _location;
        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        public PatchDestination(InstallationLocation location, DCSIntallationType installationType = DCSIntallationType.DCS)
        {
            _dcsInstallationType = installationType;
            _installationType = installationType.ToString("G").Replace('_',' ');
            _location = location;
            switch (installationType)
            {
                case DCSIntallationType.DCS_Community:
                    _targetRoot = location.SavedGamesPath;
                    _dcsRoot = location.Path;
                    break;
                case DCSIntallationType.DCS:
                default:
                    _dcsRoot = _targetRoot = location.Path;
                    break;
            }
            try
            {
                Version = PatchVersion.SortableString(location.Version);
            } catch (Exception ex)
            {
                ConfigManager.LogManager.LogError($"The version number \"{location.Version}\" read from \"autoupdate.cfg\" in DCS installation \"{_dcsRoot}\" was either corrupt, or in a format unknown to Helios.  Run DCS repair (slow mode) or reinstall the DCS installation.", ex);
                Version = "000_000_00000_00000_00000";
                DisplayVersion = "INVALID_AUTOUPDATE.CFG_VERSION";
                return;
            }
            DisplayVersion = location.Version;
        }

        public string Description => $"{_installationType} {DisplayVersion}";
        public string LongDescription => $"{_installationType} installation in '{_targetRoot}'";
        public string FailedPatchRecommendation => _installationType == "DCS" ? "Please make sure you do not have viewport mods installed and run a DCS Repair (slow mode) to clean up DCS." : "Please make sure you have not made changes to the community mods installed.  Consider re-installing the mod.";

        public string RootFolder => _dcsRoot;
        public string TargetRootFolder => _targetRoot;

        public string Version { get; }
        public string DisplayVersion { get; }

        public bool TryGetSource(string targetPath, out string source)
        {
            string path = LocateFile(targetPath);
            if (!File.Exists(path))
            {
                source = null;
                return false;
            }

            using (StreamReader streamReader = new StreamReader(path, Utf8WithoutBom))
            {
                source = streamReader.ReadToEnd();
                return true;
            }
        }

        public bool TrySaveOriginal(string targetPath)
        {
            string backupAbsolute = LocateFile(BackupPath(targetPath));
            if (File.Exists(backupAbsolute))
            {
                return true;
            }

            File.Copy(LocateFile(targetPath), backupAbsolute, true);
            return true;
        }

        private string LocateFile(string targetPath) => Path.Combine(_targetRoot, targetPath);

        // REVISIT what sort of locking can we reasonably do?
        public bool TryLock() => true;

        // REVISIT what sort of locking can we reasonably do?
        public bool TryUnlock() => true;

        public bool TryWritePatched(string targetPath, string patched)
        {
            string path = LocateFile(targetPath);
            if (!File.Exists(path))
            {
                ConfigManager.LogManager.LogError(
                    $"attempt to write back a patched file to a non-existing location: '{path}'");
                return false;
            }

            try
            {
                // delete file in case of hard link, so we get a new, unlinked file
                File.Delete(path);

                // now write as UTF-8 (no magic tag)
                using (StreamWriter streamWriter = new StreamWriter(path, false, Utf8WithoutBom))
                {
                    streamWriter.Write(patched);
                    return true;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ConfigManager.LogManager.LogError($"permission denied trying to write patched file: '{path}'", ex);
                return false;
            }
        }

        public bool TryRestoreOriginal(string targetPath)
        {
            string backupAbsolute = LocateFile(BackupPath(targetPath));
            if (!File.Exists(backupAbsolute))
            {
                return false;
            }

            string path = LocateFile(targetPath);
            try
            {
                File.Copy(backupAbsolute, path, true);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                ConfigManager.LogManager.LogError($"permission denied trying to write reverted file: '{path}'", ex);
                return false;
            }
        }

        private string BackupPath(string targetPath) => $"{targetPath}.{Version}";

        public PatchList SelectPatches(string patchesRoot, ref string selectedVersion, string patchSet, DCSIntallationType intallationType = DCSIntallationType.DCS)
        {
            if (!Directory.Exists(patchesRoot))
            {
                return new PatchList();
            }

            string candidateVersion = "";
            string candidatePatchSetPath = "";
            foreach (string versionPath in Directory.EnumerateDirectories(patchesRoot, "???_???_?????_*",
                SearchOption.TopDirectoryOnly))
            {
                string directoryVersion = Path.GetFileName(versionPath);
                // ReSharper disable once AssignNullToNotNullAttribute versionPath is returned from directory enumeration and can't be null
                string patchSetPath = Path.Combine(versionPath, patchSet);
                if (!Directory.Exists(patchSetPath))
                {
                    // patch set not included in this update
                    continue;
                }

                if (string.Compare(directoryVersion, Version, StringComparison.InvariantCulture) > 0)
                {
                    // patches are only for later version of DCS
                    continue;
                }

                if (string.Compare(directoryVersion, candidateVersion, StringComparison.InvariantCulture) <= 0)
                {
                    // we already have a better match
                    continue;
                }

                if (selectedVersion != null && string.Compare(directoryVersion, selectedVersion, StringComparison.InvariantCulture) <= 0)
                {
                    // we found a better match in another location
                    continue;
                }

                candidateVersion = directoryVersion;
                candidatePatchSetPath = patchSetPath;
            }

            if (candidatePatchSetPath == "" && _dcsInstallationType == DCSIntallationType.DCS)
            {
                if (selectedVersion != null)
                {
                    ConfigManager.LogManager.LogInfo(
                        $"no additional {patchSet} patches for DCS {DisplayVersion} from {Anonymizer.Anonymize(patchesRoot)} based on selected patch version {selectedVersion}");
                }
                else
                {
                    ConfigManager.LogManager.LogInfo(
                        $"current version of DCS {DisplayVersion} is not supported by any installed {patchSet} patch set from {Anonymizer.Anonymize(patchesRoot)}");
                }

                return new PatchList();
            }
            
            if (_dcsInstallationType == DCSIntallationType.DCS_Community)
            {
                candidatePatchSetPath = patchesRoot;
                candidateVersion = "";
            }

            if (_dcsInstallationType == DCSIntallationType.DCS)
            {
                ConfigManager.LogManager.LogInfo(
                    $"loading {patchSet} patches for DCS {DisplayVersion} from {Anonymizer.Anonymize(patchesRoot)} using version {candidateVersion} of the patches");
            }
            else
            {
                ConfigManager.LogManager.LogInfo(
                    $"loading {patchSet} patches for DCS {DisplayVersion} from {Anonymizer.Anonymize(patchesRoot)} using unversioned patches");
            }

            selectedVersion = candidateVersion;

            return PatchList.Load(candidatePatchSetPath, _targetRoot);
                    }

        private IList<StatusReportItem> ExecuteRemote(string[] patchesRoots, string selectedVersion, string patchSet, string command)
        {
            IEnumerable<string> args = new[] { "-h", $"\"{ConfigManager.DocumentPath}\"", "-d", $"\"{_targetRoot}\"", command }
                .Concat(patchesRoots.Select(root => $"\"{Path.Combine(root, selectedVersion, patchSet)}\""));
            string myDirectory = Directory.GetParent(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)).FullName;
            string executablePath = Path.Combine(myDirectory ?? "", "HeliosPatching.exe");
            using (ElevatedProcess elevated = new ElevatedProcess(executablePath, args))
            {
                if (!elevated.TryExecute())
                {
                    return new StatusReportItem
                    {
                        Status =
                            $"Running the HeliosPatching.exe utility as administrator either failed or was canceled by the user.  No changes will be written to {LongDescription}",
                        Recommendation = "Try the patching operation again and give permission to execute HeliosPatching.exe as administrator",
                        Link = StatusReportItem.ProfileEditor,
                        Severity = StatusReportItem.SeverityCode.Warning
                        // NOTE: configuration out of date indicates that this can be corrected by trying again
                    }.AsReport();
                }
                return elevated.ReadResults();
            }
        }

        public IList<StatusReportItem> RemoteApply(string[] patchesRoots, string selectedVersion, string patchSet)
        {
            return ExecuteRemote(patchesRoots, selectedVersion, patchSet, "apply");
        }

        public IList<StatusReportItem> RemoteRevert(string[] patchesRoots, string selectedVersion, string patchSet)
        {
            return ExecuteRemote(patchesRoots, selectedVersion, patchSet, "revert");
        }
    }
}