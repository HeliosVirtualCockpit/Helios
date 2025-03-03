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
using System.Linq;
using GadrocsWorkshop.Helios.Patching.DCS;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Patching
{
    /// <summary>
    /// context for the application a particular set of patches to a particular patch destination
    /// </summary>
    [DebuggerDisplay("Patches for {Destination} with status {Status}")]
    public class PatchApplication
    {
        public IPatchDestinationWritable Destination { get; }
        public PatchList Patches { get; }
        public string PatchSet { get; }
        public string[] PatchesRoots { get; }
        public string SelectedVersion { get; }
        public bool Enabled { get; internal set; }
        public bool UseRemote { get; }
        public StatusCodes Status { get; internal set; }
        public DCSIntallationType DCSIntallationType { get; }

        public HashSet<string> PatchExclusions { get; internal set; } = new HashSet<string>();

        public PatchApplication(IPatchDestinationWritable destination, bool enabled, bool useRemote, string patchSet, DCSIntallationType installationType,
            params string[] patchesRoots)
        {
            PatchSet = patchSet;
            PatchesRoots = patchesRoots;
            Destination = destination;
            Enabled = enabled;
            UseRemote = useRemote;
            DCSIntallationType = installationType;

            PatchList patches = new PatchList();
            string selectedVersion = null;
            foreach (string patchesRoot in PatchesRoots)
            {
                string previouslySelectedVersion = selectedVersion;
                PatchList loaded = Destination.SelectPatches(patchesRoot, ref selectedVersion, PatchSet, installationType);
                if(installationType == DCSIntallationType.DCS)
                {
                    if (previouslySelectedVersion != null &&
                        selectedVersion != null &&
                        string.Compare(selectedVersion, previouslySelectedVersion, StringComparison.InvariantCulture) > 0)
                    {
                        // replaced with higher version
                        patches = new PatchList();
                    }
                } else
                {
                    selectedVersion = "";
                }

                patches.Merge(loaded);
            }

            Patches = patches;
            SelectedVersion = selectedVersion;
            Status = StatusCodes.Unknown;
        }

        internal void CheckApplied()
        {
            bool notInstalled = false;
            bool installed = false;
            IList<StatusReportItem> verified = Patches.Verify(Destination, PatchExclusions).ToList();
            foreach (StatusReportItem result in verified)
            {
                // don't log these results, because Verify considers being out of date to be an error
                if (result.Severity > StatusReportItem.SeverityCode.Warning)
                {
                    // errors indicate patch needs work
                    notInstalled = true;
                }
                else if (result.Flags.HasFlag(StatusReportItem.StatusFlags.ConfigurationUpToDate))
                {
                    // patch is installed
                    installed = true;
                }
                // Note: ignore not applicable / excluded
            }

            if (!verified.Any())
            {
                Status = StatusCodes.NotApplicable;
            }
            else if (installed && notInstalled)
            {
                Status = StatusCodes.ResetRequired;
            }
            else if (installed)
            {
                Status = StatusCodes.UpToDate;
            }
            else
            {
                Status = StatusCodes.OutOfDate;
            }
        }

        internal IList<StatusReportItem> RemoteApply()
        {
            if (Status == StatusCodes.NotApplicable)
            {
                // there is nothing to do, so don't run the remote patch utility
                return new List<StatusReportItem>
                {
                    new StatusReportItem
                    {
                        Status = $"No applicable patches for {Anonymizer.Anonymize(Destination.LongDescription)} since none of the patched files are present",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    }
                };
            }

            if (string.IsNullOrEmpty(SelectedVersion))
            {
                return new List<StatusReportItem>
                {
                    new StatusReportItem
                    {
                        Status = $"No compatible patch set version available for {Anonymizer.Anonymize(Destination.LongDescription)}",
                        Recommendation =
                            $"Upgrade Helios to support {Anonymizer.Anonymize(Destination.LongDescription)} or install patches in user documents",
                        Severity = StatusReportItem.SeverityCode.Error
                    }
                };
            }

            IList<StatusReportItem> results = Destination.RemoteApply(PatchesRoots, SelectedVersion, PatchSet);
            return ScanResults(results);
        }

        internal IList<StatusReportItem> RemoteRevert()
        {
            if (Status == StatusCodes.NotApplicable)
            {
                // there is nothing to do, so don't run the remote patch/revert utility
                return new List<StatusReportItem>
                {
                    new StatusReportItem
                    {
                        Status = $"No applicable patches to revert for {Anonymizer.Anonymize(Destination.LongDescription)} since none of the patched files are present",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    }
                };
            }

            if (string.IsNullOrEmpty(SelectedVersion))
            {
                return new List<StatusReportItem>
                {
                    new StatusReportItem
                    {
                        Status = $"Nothing to revert for for {Anonymizer.Anonymize(Destination.LongDescription)}",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    }
                };
            }

            IList<StatusReportItem> results = Destination.RemoteRevert(PatchesRoots, SelectedVersion, PatchSet);
            return ScanResults(results);
        }

        internal IList<StatusReportItem> Apply()
        {
            IList<StatusReportItem> results = Patches.Apply(Destination, PatchExclusions).ToList();
            return ScanResults(results);
        }

        internal IList<StatusReportItem> Revert()
        {
            IList<StatusReportItem> results = Patches.Revert(Destination, PatchExclusions).ToList();
            return ScanResults(results);
        }

        private IList<StatusReportItem> ScanResults(IList<StatusReportItem> results)
        {
            if (results.Any(result => result.Severity >= StatusReportItem.SeverityCode.Error))
            {
                // fatal error
                Status = StatusCodes.Incompatible;
                return results;
            }

            // nothing fatal found, check files to set new status
            CheckApplied();
            return results;
        }
    }
}