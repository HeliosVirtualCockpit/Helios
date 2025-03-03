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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Patching.DCS;
using GadrocsWorkshop.Helios.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GadrocsWorkshop.Helios.Patching
{
    /// <summary>
    /// a context for the installation of a specific patch set to a set of patch destinations
    ///
    /// NOTE: view model functionality directly integrated to avoid tons of glue
    /// </summary>
    public class PatchInstallation: DependencyObject, IInstallation
    {
        public event EventHandler PatchesChanged;

        private readonly Dictionary<string, PatchApplication> _destinations;
        private readonly string _patchSetDescription;
        private bool _isAdvancedWarningSuppressed = false;
        private readonly DCSIntallationType _dcsInstallationType;

        public PatchInstallation(
            Dictionary<string, PatchApplication> destinations, 
            string patchSet,
            string patchSetDescription,
            DCSIntallationType dcsInstallationType = DCSIntallationType.DCS)
        {
            _destinations = destinations;
            _ = patchSet;
            _patchSetDescription = patchSetDescription;
            _dcsInstallationType = dcsInstallationType;

            PatchedPaths = new ObservableCollection<PatchedPath>();

            // load patchExclusions from settings instead of caching them, so we don't have different code paths from elevated binary
            HashSet<string> patchExclusions = LoadPatchExclusions();

            // check if all selected patches are installed
            foreach (PatchApplication destination in _destinations.Values)
            {
                destination.PatchExclusions = patchExclusions;
                destination.CheckApplied();
            }

            // update overall status
            UpdateStatus();
        }

        public void OnEnabled(string key)
        {
            PatchApplication destinationPatches = _destinations[key];
            destinationPatches.Enabled = true;
            destinationPatches.CheckApplied();
            UpdateStatus();
        }

        public void OnDisabled(string key)
        {
            _destinations[key].Enabled = false;
            UpdateStatus();
        }

        public void OnRemoved(string key)
        {
            _destinations.Remove(key);
            UpdateStatus();
        }

        public void OnAdded(string key, PatchApplication destinationPatches)
        {
            destinationPatches.CheckApplied();
            _destinations[key] = destinationPatches;
            UpdateStatus();
        }

        public void Reload(Dictionary<string, PatchApplication> effectiveDestinations)
        {
            // load patchExclusions from settings instead of caching them, so we don't have different code paths from elevated binary
            HashSet<string> patchExclusions = LoadPatchExclusions();

            // rebuild completely
            _destinations.Clear();
            foreach (KeyValuePair<string, PatchApplication> keyValuePair in effectiveDestinations)
            {
                keyValuePair.Value.PatchExclusions = patchExclusions;
                keyValuePair.Value.CheckApplied();
                _destinations[keyValuePair.Key] = keyValuePair.Value;
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            // calculate overall status
            StatusCodes newStatus = StatusCodes.NotApplicable;
            bool someUpToDate = false;
            bool statusFound = false;
            bool someEnabled = false;

            // update set of patched paths
            // REVISIT: accumulate flags for each path, whether patch was from documents or program files?
            SortedSet<string> allPaths = new SortedSet<string>();

            foreach (PatchApplication destination in _destinations.Values)
            {
                // accumulate set of paths that exist in any destination's selected patch set
                foreach (string path in destination.Patches.PatchedPaths)
                {
                    allPaths.Add(path);
                }

                if (statusFound)
                {
                    // already found terminal status, just iterating for paths at this point
                    continue;
                }

                if (!destination.Enabled)
                {
                    // does not count for status
                    continue;
                }

                someEnabled = true;
                switch (destination.Status)
                {
                    case StatusCodes.Unknown:
                    case StatusCodes.OutOfDate:
                        newStatus = StatusCodes.OutOfDate;
                        // don't end iteration, need to check for failures
                        break;
                    case StatusCodes.UpToDate:
                        // don't end iteration, need to check for failures and out of date things
                        someUpToDate = true;
                        break;
                    case StatusCodes.Incompatible:
                        // any destination being incompatible counts
                        newStatus = StatusCodes.Incompatible;
                        statusFound = true;
                        break;
                    case StatusCodes.ResetRequired:
                        // any destination being dirty counts
                        newStatus = StatusCodes.ResetRequired;
                        statusFound = true;
                        break;
                    case StatusCodes.NotApplicable:
                        // this never changes the status; ignore
                        break;
                    // ReSharper disable RedundantCaseLabel documenting intent and triggering unhandled case warning when something is added later
                    case StatusCodes.NoLocations:
                    case StatusCodes.ResetMonitorsRequired:
                    case StatusCodes.ProfileSaveRequired:
                    // ReSharper enable RedundantCaseLabel
                    default:
                        throw new ArgumentOutOfRangeException(nameof(destination.Status), destination.Status, null);
                }
            }

            if (newStatus == StatusCodes.NotApplicable)
            {
                if (someUpToDate)
                {
                    // at least some patches were applied
                    newStatus = StatusCodes.UpToDate;
                } 
                else if (!someEnabled)
                {
                    newStatus = StatusCodes.NoLocations;
                }
            } 
            
            // update the collection view minimally to invalidate the fewest UI elements
            UpdatePatchedPaths(allPaths);

            // show new status
            Status = newStatus;
        }

        private void UpdatePatchedPaths(SortedSet<string> allPaths)
        {
            // master/update because we build the ObservableCollection in order also
            // and we want to strictly minimize the changes in the observable collection
            int masterCount = PatchedPaths.Count;
            int masterIndex = 0;

            foreach (string update in allPaths)
            {
                if (masterIndex == masterCount)
                {
                    // ran our of master records, all updates go in
                    PatchedPaths.Insert(masterIndex, CreatePatchedPath(update));
                    masterIndex++;
                    masterCount++;
                    continue;
                }

                // scan up to the current update
                while (masterIndex<masterCount)
                {
                    PatchedPath master = PatchedPaths[masterIndex];
                    int comparison = string.Compare(master.Path, update, StringComparison.Ordinal);
                    if (comparison < 0)
                    {
                        // master item is no longer present
                        PatchedPaths.RemoveAt(masterIndex);
                        masterCount--;
                        DisposePatchedPath(master);
                        continue;
                    }

                    if (comparison == 0)
                    {
                        // found existing item, advance update and master
                        masterIndex++;
                        break;
                    }

                    // insert update item before master and keep considering the same master record
                    // (which is now one further to the right)
                    PatchedPaths.Insert(masterIndex, CreatePatchedPath(update));
                    masterIndex++;
                    masterCount++;
                    break;
                }
            }
        }

        private void DisposePatchedPath(PatchedPath patchedPath)
        {
            patchedPath.PropertyChanged -= PatchedPath_PropertyChanged;
        }

        private PatchedPath CreatePatchedPath(string path)
        {
            PatchedPath patchedPath = new PatchedPath(path);
            patchedPath.PropertyChanged += PatchedPath_PropertyChanged;
            return patchedPath;
        }

        private void PatchedPath_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PatchedPath.Allowed))
            {
                if(sender is PatchedPath patchedPath && e is PropertyNotificationEventArgs args)
                {
                    patchedPath.IsWarningSuppressed = _isAdvancedWarningSuppressed;
                    _isAdvancedWarningSuppressed = (bool) args.NewValue;
                }
                // need to check patch status
                PatchesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public InstallationResult Install(IInstallationCallbacks callbacks)
        {
            List<StatusReportItem> results = new List<StatusReportItem>();
            bool failed = false;
            bool imprecise = false;

            // load patchExclusions from settings instead of caching them, so we don't have different code paths from elevated binary
            HashSet<string> patchExclusions = LoadPatchExclusions();

            // simulate patches and collect any errors
            foreach (PatchApplication item in _destinations.Values)
            {
                if (!item.Enabled)
                {
                    continue;
                }

                item.PatchExclusions = patchExclusions;

                foreach (StatusReportItem result in item.Patches.SimulateApply(item.Destination, patchExclusions))
                {
                    result.Log(ConfigManager.LogManager);
                    results.Add(result);
                    if (result.Severity >= StatusReportItem.SeverityCode.Error)
                    {
                        // add additional context information we know about
                        results.Add(new StatusReportItem
                        {
                            Severity = StatusReportItem.SeverityCode.Error,
                            Status = $"{item.Destination.Description} could not be patched using version {item.SelectedVersion} of the patches.  You may need a newer distribution of Helios or of the patches.",
                            Recommendation = item.Destination.FailedPatchRecommendation
                        });

                        failed = true;
                        item.Status = StatusCodes.Incompatible;
                    }

                    if (result.Severity >= StatusReportItem.SeverityCode.Warning)
                    {
                        imprecise = true;
                    }
                }
            }

            if (failed)
            {
                UpdateStatus(); 
                callbacks.Failure($"{_patchSetDescription} installation would fail", "Some patches would fail to apply.  No patches have been applied.", results);
                return InstallationResult.Fatal;
            }

            if (imprecise)
            {
                InstallationPromptResult response = callbacks.DangerPrompt(
                    $"{_patchSetDescription} installation may have risks",
                    $"{_patchSetDescription} installation can continue, but some target files have changed since these patches were created.",
                    results);
                if (response == InstallationPromptResult.Cancel)
                {
                    return InstallationResult.Canceled;
                }
            }

            // apply patches
            foreach (PatchApplication item in _destinations.Values)
            {
                if (!item.Enabled)
                {
                    // destination is not selected for writing
                    continue;
                }

                // apply patches directly or via elevated process, as appropriate
                IList<StatusReportItem> applyResults = item.UseRemote ? item.RemoteApply() : item.Apply();
                foreach (StatusReportItem result in applyResults)
                {
                    result.Log(ConfigManager.LogManager);
                    results.Add(result);
                    if (result.Severity >= StatusReportItem.SeverityCode.Error)
                    {
                        failed = true;
                    }
                }
            }

            UpdateStatus();
            if (failed)
            {
                // XXX need to revert any patches that were installed, if we can, and add to result report
                callbacks.Failure($"{_patchSetDescription} installation failed", "Some patches failed to be written", results);
                return InstallationResult.Fatal;
            }

            if (Status != StatusCodes.UpToDate)
            {
                // installation completed, but not everything was done
                callbacks.Failure($"{_patchSetDescription} installation incomplete", "Not all patches were installed", results);
                return InstallationResult.Canceled;
            }

            callbacks.Success($"{_patchSetDescription} installation success", "All patches installed successfully",
                results);
            return InstallationResult.Success;
        }

        public InstallationResult Uninstall(IInstallationCallbacks callbacks)
        {
            List<StatusReportItem> results = new List<StatusReportItem>();
            bool failed = false;
            string message = "";

            // load patchExclusions from settings instead of caching them, so we don't have different code paths from elevated binary
            HashSet<string> patchExclusions = LoadPatchExclusions();

            // revert patches, by either restoring original file or reverse patching
            foreach (PatchApplication item in _destinations.Values)
            {
                if (!item.Enabled)
                {
                    continue;
                }

                // revert directly or via elevated process, as appropriate
                item.PatchExclusions = patchExclusions;
                IList<StatusReportItem> revertResults = item.UseRemote ? item.RemoteRevert() : item.Revert();
                foreach (StatusReportItem result in revertResults)
                {
                    result.Log(ConfigManager.LogManager);
                    results.Add(result);
                    if (result.Severity < StatusReportItem.SeverityCode.Error)
                    {
                        continue;
                    }

                    if (!failed)
                    {
                        // keep first message
                        message = $"Reverting patches in {item.Destination.LongDescription} failed\n{result.Status}\n{result.Recommendation}";
                    }
                    failed = true;
                }

                // ReSharper disable once InvertIf early exit on error condition should have a block
                if (failed)
                {
                    if (item.PatchSet == "Viewports")
                    {
                        // give up and just direct the user to fix one installation this time, meaning they may have to do this multiple times
                        message +=
                            $"\nPlease execute 'bin\\dcs_updater.exe repair' in your {item.Destination.LongDescription} to restore to original files";
                    }
                    break;
                }
            }

            UpdateStatus();

            if (failed)
            {
                callbacks.Failure($"{_patchSetDescription} revert failed", message, results);
                return InstallationResult.Fatal;
            }

            // special check that is not handled by UpdateStatus, because we are technically in a state that just means
            // we have to reinstall some patches, but this still means something went wrong in reverting patches, such as
            // user cancellation
            if (_destinations.Values.Any(d => d.Enabled && 
                d.Status != StatusCodes.OutOfDate && 
                d.Status != StatusCodes.NotApplicable))
            {
                // revert completed, but not everything was done
                callbacks.Failure($"{_patchSetDescription} removal incomplete", "Some patches were not reverted", results);
                return InstallationResult.Canceled;
            }

            callbacks.Success($"{_patchSetDescription} removal success", "All patches reverted successfully",
                results);
            return InstallationResult.Success;
        }

        public static HashSet<string> LoadPatchExclusions()
        {
            return new HashSet<string>(PatchedPath.ExcludedPaths);
        }

        #region Properties

        /// <summary>
        /// Patch installation status
        /// </summary>
        public StatusCodes Status
        {
            get => (StatusCodes) GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StatusCodes), typeof(PatchInstallation),
                new PropertyMetadata(StatusCodes.OutOfDate));

        /// <summary>
        /// currently known patched paths in any of the current patch destinations, and their filtering status
        /// </summary>
        public ObservableCollection<PatchedPath> PatchedPaths
        {
            get => (ObservableCollection<PatchedPath>)GetValue(PatchedPathsProperty);
            set => SetValue(PatchedPathsProperty, value);
        }

        public static readonly DependencyProperty PatchedPathsProperty =
            DependencyProperty.Register("PatchedPaths", typeof(ObservableCollection<PatchedPath>), typeof(PatchInstallation), new PropertyMetadata(null));

        #endregion
    }
}