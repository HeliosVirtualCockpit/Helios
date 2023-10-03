//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors
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

using System.Linq;
using System.Xml;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Common;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Interfaces.Profile
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [HeliosInterface("Helios.Base.ProfileInterface", "Profile", typeof(ProfileInterfaceEditor), typeof(UniqueHeliosInterfaceFactory), AutoAdd = true)]
    public class ProfileInterface : HeliosInterfaceWithXml<ProfileSettings>, IStatusReportNotify, IResetMonitorsObserver, IReadyCheck
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // failed image load paths we know about, so that we can recognize when a load success changes the status
        private HashSet<string> _failedImagePaths;
        private bool _dirty;

        /// <summary>
        /// backing field for property Status, contains
        /// the current configuration status of the profile interface, as of the last status report
        /// </summary>
        private StatusCodes _status;

        // our implementation of IStatusReportNotify
        private readonly StatusReportNotifyAsyncOnce _statusReportNotify;

        // definition for profile related triggers
        private readonly HeliosTrigger _profileStartedTrigger;
        private readonly HeliosTrigger _profileResetTrigger;
        private readonly HeliosTrigger _profileStoppedTrigger;

        public ProfileInterface()
            : base("Profile")
        {
            HeliosAction resetAction = new HeliosAction(this, "", "", "reset", "Resets the profile to default state.");
            resetAction.Execute += new HeliosActionHandler(ResetAction_Execute);
            Actions.Add(resetAction);

            HeliosAction stopAction = new HeliosAction(this, "", "", "stop", "Stops the profile from running.");
            stopAction.Execute += new HeliosActionHandler(StopAction_Execute);
            Actions.Add(stopAction);

            HeliosAction showControlCenter = new HeliosAction(this, "", "", "show control center", "Shows the control center.");
            showControlCenter.Execute += new HeliosActionHandler(ShowAction_Execute);
            Actions.Add(showControlCenter);

            HeliosAction hideControlCenter = new HeliosAction(this, "", "", "hide control center", "Shows the control center.");
            hideControlCenter.Execute += new HeliosActionHandler(HideAction_Execute);
            Actions.Add(hideControlCenter);

            HeliosAction changeControlCenter = new HeliosAction(this, "", "", "change control center profile", "Changes the control center profile.", "Profile name",
BindingValueUnits.Text);
            changeControlCenter.Execute += new HeliosActionHandler(ChangeProfileAction_Execute);
            Actions.Add(changeControlCenter);

            HeliosAction launchApplication = new HeliosAction(this, "", "", "launch application", "This functionality has moved to Process Control interface", "This action will be ignored.", BindingValueUnits.Text);
            launchApplication.Execute += LaunchApplication_Execute;
            Actions.Add(launchApplication);

            _profileStartedTrigger = new HeliosTrigger(this, "", "", "Started", "Fired when a profile is started.");
            Triggers.Add(_profileStartedTrigger);

           _profileResetTrigger = new HeliosTrigger(this, "", "", "Reset", "Fired when a profile has been reset.");
            Triggers.Add(_profileResetTrigger);

            _profileStoppedTrigger = new HeliosTrigger(this, "", "", "Stopped", "Fired when a profile is stopped.");
            Triggers.Add(_profileStoppedTrigger);
            
            _statusReportNotify = new StatusReportNotifyAsyncOnce(CreateStatusReport, () => "Profile Interface", () => "Interface to Helios itself.");
        }

        protected override void AttachToProfileOnMainThread()
        {
            base.AttachToProfileOnMainThread();
            if (!(ConfigManager.ImageManager is IImageManager2 images))
            {
                return;
            }

            _failedImagePaths = new HashSet<string>();
            images.ReplayCurrentFailures(Images_ImageLoadFailure);

            images.ImageLoadSuccess += Images_ImageLoadSuccess;
            images.ImageLoadFailure += Images_ImageLoadFailure;
            Profile.PropertyChanged += Profile_PropertyChanged;
            Profile.ProfileStarted += Profile_ProfileStarted;
            Profile.ProfileStopped += Profile_ProfileStopped;

            ConfigManager.UndoManager.Empty += UndoManager_Empty;
            ConfigManager.UndoManager.NonEmpty += UndoManager_NonEmpty;
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            base.DetachFromProfileOnMainThread(oldProfile);
            if (!(ConfigManager.ImageManager is IImageManager2 images))
            {
                return;
            }

            oldProfile.ProfileStopped -= Profile_ProfileStopped;
            oldProfile.ProfileStarted -= Profile_ProfileStarted;
            oldProfile.PropertyChanged -= Profile_PropertyChanged;
            images.ImageLoadSuccess -= Images_ImageLoadSuccess;
            images.ImageLoadFailure -= Images_ImageLoadFailure;
            ConfigManager.UndoManager.Empty -= UndoManager_Empty;
            ConfigManager.UndoManager.NonEmpty -= UndoManager_NonEmpty;
        }

        // WARNING: ReadXml is not usually called because we are usually an empty XML element (default configuration)

        public override void WriteXml(XmlWriter writer)
        {
            if (Model.IsDefault)
            {
                // keep profile compatible with previous Helios if no new feature was used
                return;
            }
            base.WriteXml(writer);
        }

        private void UndoManager_NonEmpty(object sender, EventArgs e)
        {
            _dirty = true;
            InvalidateStatusReport();
        }

        private void UndoManager_Empty(object sender, EventArgs e)
        {
            _dirty = false;
            InvalidateStatusReport();
        }

        private void Profile_ProfileStarted(object sender, EventArgs e)
        {
            _profileStartedTrigger.FireTrigger(BindingValue.Empty);
        }

        private void Profile_ProfileStopped(object sender, EventArgs e)
        {
            _profileStoppedTrigger.FireTrigger(BindingValue.Empty);
        }

        private void Profile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Profile.Path))
            {
                InvalidateStatusReport();
            }
        }

        protected override void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.Model_PropertyChanged(sender, e); 
            InvalidateStatusReport();
        }

        public override void Reset()
        {
            base.Reset();
            _profileResetTrigger.FireTrigger(BindingValue.Empty);
        }

        private void Images_ImageLoadFailure(object sender, ImageLoadEventArgs e)
        {
            if (_failedImagePaths == null)
            {
                // this is normal state during profile loading
                return;
            }

            // track in case we turn reporting back on
            _failedImagePaths.Add(e.Path);

            if (Model.IgnoreMissingImages)
            {
                // suppressed
                return;
            }

            // log as warning if not suppressed (to raise caution) and report in status report, because we only logged missing user images at info level during deserializaton
            Logger.Warn("an image referenced in the profile {ProfileName} could not be loaded from {Path}",
                Profile?.Name, e.Path);
            InvalidateStatusReport();
        }

        private void Images_ImageLoadSuccess(object sender, ImageLoadEventArgs e)
        {
            if (_failedImagePaths?.Remove(e.Path) ?? false)
            {
                InvalidateStatusReport();
            }
        }

        private void LaunchApplication_Execute(object action, HeliosActionEventArgs e)
        {
            _ = action;
            _ = e;
            Logger.Error("The ability to launch programs with a Helios profile has been moved to the Process Control Interface for security reasons.  Please install this interface, give it permission in its editor, and then use its bindings if you intend to use this functionality.");
        }

        private void HideAction_Execute(object action, HeliosActionEventArgs e)
        {
            Profile?.HideControlCenter();
        }

        private void ShowAction_Execute(object action, HeliosActionEventArgs e)
        {
            Profile?.ShowControlCenter();
        }

        private void StopAction_Execute(object action, HeliosActionEventArgs e)
        {
            Profile?.Stop();
        }

        private void ResetAction_Execute(object action, HeliosActionEventArgs e)
        {
            Profile?.Reset();
        }

        private void ChangeProfileAction_Execute(object action, HeliosActionEventArgs e)
        {
            Profile?.ChangeProfileControlCenter(e);
        }

        public void Subscribe(IStatusReportObserver observer)
        {
            _statusReportNotify.Subscribe(observer);
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            _statusReportNotify.Unsubscribe(observer);
        }

        public void InvalidateStatusReport()
        {
            _statusReportNotify.InvalidateStatusReport();
        }

        private void CreateStatusReport()
        {
            IList<StatusReportItem> newReport = new List<StatusReportItem>();
            CheckProfile(newReport);
            CheckResetMonitors(newReport);
            CheckMissingimages(newReport);
            CheckDirty(newReport);
            PublishStatusReport(newReport);
        }

        private void CheckDirty(IList<StatusReportItem> newReport)
        {
            if (_dirty)
            {
                newReport.Add(new StatusReportItem
                {
                    Status = "Profile needs to be saved in order to be used by Helios Control Center",
                    Recommendation = "Save the profile before trying to use it in Helios Control Center",
                    Link = StatusReportItem.ProfileEditor,
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Flags = StatusReportItem.StatusFlags.DoNotDisturb
                });
            }
        }

        private void CheckProfile(IList<StatusReportItem> newReport)
        {
            if (Profile == null)
            {
                // this is probably never displayed
                newReport.Add(new StatusReportItem
                {
                    Status = "No profile loaded"
                });
                return;
            }

            if (string.IsNullOrEmpty(Profile.Path))
            {
                newReport.Add(new StatusReportItem
                {
                    Status = $"Profile is not yet named",
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                });
                return;
            }

            newReport.Add(new StatusReportItem
            {
                Status = $"Profile is '{Profile.Path}'",
                Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
            });
        }

        private void CheckMissingimages(IList<StatusReportItem> newReport)
        {
            if (Model.IgnoreMissingImages)
            {
                if (_failedImagePaths.Any())
                {
                    newReport.Add(new StatusReportItem
                    {
                        Status = $"Missing images in this Profile are ignored due to a setting in the Profile Interface",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate
                    });
                }

                // suppress reporting
                return;
            }
            foreach (string path in _failedImagePaths)
            {
                newReport.Add(new StatusReportItem
                {
                    Status = $"An image used in this Profile's controls could not be loaded from '{path}'",
                    Recommendation = "Install missing images or correct image paths in controls, then reload profile.",
                    Severity = StatusReportItem.SeverityCode.Warning,
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                });
            }
        }

        private void CheckResetMonitors(IList<StatusReportItem> newReport)
        {
            if (Profile == null)
            {
                return;
            }

            if (Profile.IsValidMonitorLayout)
            {
                Status = StatusCodes.UpToDate;
                newReport.Add(new StatusReportItem
                {
                    Status = "The monitor arrangement saved in this profile matches this computer",
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                });
            }
            else
            {
                Status = StatusCodes.ResetMonitorsRequired;
                newReport.Add(new StatusReportItem
                {
                    Status = "The monitor arrangement saved in this profile does not match this computer",
                    Recommendation = "Perform Reset Monitors from the Profile menu",
                    Severity = StatusReportItem.SeverityCode.Error,
                    Link = StatusReportItem.ProfileEditor
                });

                foreach (Monitor display in Profile.CheckedDisplays)
                {
                    newReport.Add(new StatusReportItem
                    {
                        Status = $"Windows reports an attached display of size {display.Width}x{display.Height} at ({display.Left}, {display.Top})",
                        Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                    });
                }
            }

            foreach (Monitor monitor in Profile.Monitors)
            {
                newReport.Add(new StatusReportItem
                {
                    Status = $"The profile declares a monitor of size {monitor.Width}x{monitor.Height} at ({monitor.Left}, {monitor.Top})",
                    Flags = StatusReportItem.StatusFlags.ConfigurationUpToDate | StatusReportItem.StatusFlags.Verbose
                });
            }
        }

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            _statusReportNotify.PublishStatusReport(statusReport);
        }

        public void NotifyResetMonitorsStarting()
        {
            // no code
        }

        public void NotifyResetMonitorsComplete()
        {
            InvalidateStatusReport();
        }

        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            // NOTE: we don't have to check monitor reset because Control Center already does that
            // NOTE: we don't have to check missing images because they will log warnings 
            yield break;
        }

        /// <summary>
        /// the current configuration status of the profile interface, as of the last status report
        /// </summary>
        public StatusCodes Status
        {
            get => _status;
            set
            {
                if (_status == value) return;
                StatusCodes oldValue = _status;
                _status = value;

                // status change must not make the profile dirty, so we fire it as a no-undo event
                OnPropertyChanged("Status", oldValue, value, false);
            }
        }
    }
}
