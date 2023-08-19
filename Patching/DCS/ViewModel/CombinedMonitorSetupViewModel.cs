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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Windows;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    /// <summary>
    /// view model for DCS Monitor Setup interaction with the list of separated monitor setups versus a single combined monitor setup
    /// </summary>
    public partial class CombinedMonitorSetupViewModel : HeliosViewModel<MonitorSetup>
    {
        /// <summary>
        /// backing field for property CombineCommand, contains
        /// command handlers for "add monitor setup to combined monitor setup"
        /// </summary>
        private ICommand _combineCommand;

        /// <summary>
        /// backing field for property ExcludeCommand, contains
        /// command handlers for "remove monitor setup from set of combined monitor setups"
        /// </summary>
        private ICommand _excludeCommand;

        /// <summary>
        /// backing field for property DeleteCommand, contains
        /// command handlers for "delete generated monitor setup file"
        /// </summary>
        private ICommand _deleteCommand;

        public CombinedMonitorSetupViewModel(MonitorSetup data) : base(data)
        {
            Combined = new ObservableCollection<ViewportSetupFileViewModel>();
            Excluded = new ObservableCollection<ViewportSetupFileViewModel>();

            bool found = false;
            foreach (string name in Data.Combined.KnownViewportSetupNames)
            {
                ViewportSetupFileViewModel model = new ViewportSetupFileViewModel(name, Data.Combined.Load(name));
                if (model.ProfileName == Data.CurrentProfileName)
                {
                    found = true;
                    ConfigureCurrentProfile(model);
                }

                if (Data.Combined.IsCombined(model.ProfileName))
                {
                    AddCombined(model);
                }
                else
                {
                    AddExcluded(model);
                }
            }

            // add ourselves at the end of the appropriate list, if not already there
            if (!found)
            {
                AddCurrentProfile();
            }

            // register for changes
            Data.PropertyChanged += Data_PropertyChanged;
            Data.UpdatedViewports += Data_UpdatedViewports;
        }

        internal void Dispose()
        {
            Data.PropertyChanged -= Data_PropertyChanged;
            Data.UpdatedViewports -= Data_UpdatedViewports;
            Combined.Clear();
            Excluded.Clear();
        }

        private void Data_UpdatedViewports(object sender, MonitorSetup.UpdatedViewportsEventArgs e)
        {
            if (e.LocalViewports == null)
            {
                // this happens when we can't even calculate these, so we update nothing
                return;
            }

            CurrentViewportSetup.SetData(e.LocalViewports);

            // recalculate every status, in case of new or resolved conflicts
            CalculateStatus();
        }

        private void CalculateStatus()
        {
            // REVISIT PERFORMANCE this is n-squared times log-n, where n = total viewports in the system
            foreach (ViewportSetupFileViewModel loaded in Combined)
            {
                CalculateStatus(loaded);
            }

            foreach (ViewportSetupFileViewModel loaded in Excluded)
            {
                CalculateStatus(loaded);
            }
        }

        private void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Data.CurrentProfileName):
                    HandleProfileNameChange();
                    Data.InvalidateStatusReport();
                    break;
            }
        }

        /// <summary>
        /// called when the current profile has been saved under a different name
        /// </summary>
        private void HandleProfileNameChange()
        {
            string oldName = CurrentViewportSetup.ProfileName;
            string newName = Data.CurrentProfileName;
            Debug.Assert(oldName != newName);

            ResetCurrentMonitorSetupSelection();
            ViewportSetupFileViewModel model = Combined.FirstOrDefault(m => m.ProfileName == newName);
            if (model != null)
            {
                ConfigureCurrentProfile(model);

                // after fixups, check if in the correct list
                if (Data.Combined.IsCombined(model.ProfileName))
                {
                    return;
                }

                Combined.Remove(model);
                AddExcluded(model);
                return;
            }

            model = Excluded.FirstOrDefault(m => m.ProfileName == newName);
            if (model != null)
            {
                ConfigureCurrentProfile(model);

                // after fixups, check if in the correct list
                if (!Data.Combined.IsCombined(model.ProfileName))
                {
                    return;
                }

                Excluded.Remove(model);
                AddCombined(model);
                return;
            }

            // not found
            AddCurrentProfile();
        }

        private void ResetCurrentMonitorSetupSelection()
        {
            if (CurrentViewportSetup.ProfileName == null)
            {
                // special entry for unsaved profile, just remove it
                if (Combined.Contains(CurrentViewportSetup))
                {
                    Combined.Remove(CurrentViewportSetup);
                }
                else if (Excluded.Contains(CurrentViewportSetup))
                {
                    Excluded.Remove(CurrentViewportSetup);
                }
            }
            else
            {
                // keep it, but this is no longer the special entry for current
                CurrentViewportSetup.IsCurrentProfile = false;
            }
        }

        private void ConfigureCurrentProfile(ViewportSetupFileViewModel model)
        {
            model.IsCurrentProfile = true;
            CurrentViewportSetup = model;

            // now fix up inconsistencies
            if (Data.Combined.IsCombined(model.ProfileName) == Data.GenerateCombined)
            {
                return;
            }

            if (Data.GenerateCombined)
            {
                Data.Combined.SetCombined(model.ProfileName);
            }
            else
            {
                Data.Combined.SetExcluded(model.ProfileName);
            }
        }

        private void AddCurrentProfile()
        { 
            // create new item
            CurrentViewportSetup =
                new ViewportSetupFileViewModel(Data.CurrentProfileName, Data.Combined.Load(Data.CurrentProfileName))
                {
                    IsCurrentProfile = true
                };
            if (Data.GenerateCombined)
            {
                Data.Combined.SetCombined(Data.CurrentProfileName);
                AddCombined(CurrentViewportSetup);
            }
            else
            {
                Data.Combined.SetExcluded(Data.CurrentProfileName);
                AddExcluded(CurrentViewportSetup);
            }
        }


        /// <summary>
        /// check if viewport data is available
        /// calculate conflicts with the models currently in Combined set and report the first one
        /// </summary>
        /// <param name="model"></param>
        private void CalculateStatus(ViewportSetupFileViewModel model)
        {
            if (!model.Data.Exists)
            {
                // the local profile may not have data yet, but that is ok.  for any merged profiles, we need the data
                if (!model.IsCurrentProfile)
                {
                    model.Status = ViewportSetupFileStatus.NotGenerated;
                    model.ProblemShortDescription = "needs configuration";
                    model.ProblemNarrative =
                        $"DCS Monitor Setup has to be configured in profile '{model.ProfileName}' before it can be combined";
                    return;
                }
            }
            else
            {
                // check compatibility
                if (model.Data.MonitorLayoutKey != Data.MonitorLayoutKey)
                {
                    model.Status = ViewportSetupFileStatus.OutOfDate;
                    model.ProblemShortDescription = "needs configuration for new monitor layout";
                    model.ProblemNarrative =
                        $"DCS Monitor Setup should be configured in profile '{model.ProfileName}' to adjust to the current monitor layout";
                    return;
                }

                // search for conflicts
                foreach (KeyValuePair<string, Rect> viewport in model.Data.Viewports)
                {
                    foreach (ViewportSetupFileViewModel existing in Combined)
                    {

                        if (existing.Data == null)
                        {
                            continue;
                        }

                        if (!existing.Data.Viewports.TryGetValue(viewport.Key, out Rect existingRect))
                        {
                            continue;
                        }

                        if (existingRect.Equals(viewport.Value))
                        {
                            continue;
                        }

                        model.Status = ViewportSetupFileStatus.Conflict;
                        model.ProblemShortDescription = $"conflicts with {existing.ProfileName}";
                        model.ProblemNarrative =
                            $"profile '{existing.ProfileName}' defines the viewport '{viewport.Key}' at a different screen location";
                        return;
                    }
                }
            }

            // none found, we are good
            model.Status = ViewportSetupFileStatus.OK;
            model.ProblemShortDescription = null;
            model.ProblemNarrative = null;
        }

        private void Combine(ViewportSetupFileViewModel model)
        {
            Excluded.Remove(model);
            Data.Combined.SetCombined(model.ProfileName);
            if (model == CurrentViewportSetup)
            {
                using (new HeliosUndoBatch())
                {
                    Data.GenerateCombined = true;
                    ConfigManager.UndoManager.AddUndoItem(new UndoCombine(this, model));
                }
            }

            AddCombined(model);
            Data.InvalidateStatusReport();
        }

        private void Exclude(ViewportSetupFileViewModel model)
        {
            Combined.Remove(model);
            Data.Combined.SetExcluded(model.ProfileName);
            if (model == CurrentViewportSetup)
            {
                using (new HeliosUndoBatch())
                {
                    Data.GenerateCombined = false;
                    ConfigManager.UndoManager.AddUndoItem(new UndoExclude(this, model));
                }
            }

            AddExcluded(model);
            Data.InvalidateStatusReport();
        }

        private void Delete(ViewportSetupFileViewModel model)
        {
            Data.Combined.Delete(model.ProfileName);
            Excluded.Remove(model);
            Data.InvalidateStatusReport();
        }

        private void AddExcluded(ViewportSetupFileViewModel model)
        {
            Excluded.Add(model);
            CalculateStatus();
        }

        private void AddCombined(ViewportSetupFileViewModel model)
        {
            Combined.Add(model);
            CalculateStatus();
        }


        /// reference to the only set of viewports we update right now
        /// </summary>
        internal ViewportSetupFileViewModel CurrentViewportSetup { get; private set; }

        public ObservableCollection<ViewportSetupFileViewModel> Combined
        {
            get => (ObservableCollection<ViewportSetupFileViewModel>) GetValue(CombinedProperty);
            set => SetValue(CombinedProperty, value);
        }

        public static readonly DependencyProperty CombinedProperty =
            DependencyProperty.Register("Combined", typeof(ObservableCollection<ViewportSetupFileViewModel>),
                typeof(MonitorSetupViewModel), new PropertyMetadata(null));

        public ObservableCollection<ViewportSetupFileViewModel> Excluded
        {
            get => (ObservableCollection<ViewportSetupFileViewModel>) GetValue(ExcludedProperty);
            set => SetValue(ExcludedProperty, value);
        }

        public static readonly DependencyProperty ExcludedProperty =
            DependencyProperty.Register("Excluded", typeof(ObservableCollection<ViewportSetupFileViewModel>),
                typeof(MonitorSetupViewModel), new PropertyMetadata(null));

        /// <summary>
        /// command handlers for "add monitor setup to combined monitor setup"
        /// </summary>
        public ICommand CombineCommand
        {
            get
            {
                _combineCommand = _combineCommand ?? new RelayCommand(
                    parameter => { Combine((ViewportSetupFileViewModel) parameter); },
                    parameter =>
                        parameter != null && ((ViewportSetupFileViewModel) parameter).Status !=
                        ViewportSetupFileStatus.NotGenerated);
                return _combineCommand;
            }
        }

        /// <summary>
        /// command handlers for "remove monitor setup from set of combined monitor setups"
        /// </summary>
        public ICommand ExcludeCommand
        {
            get
            {
                _excludeCommand = _excludeCommand ?? new RelayCommand(parameter =>
                {
                    Exclude((ViewportSetupFileViewModel) parameter);
                });
                return _excludeCommand;
            }
        }

        /// <summary>
        /// command handlers for "delete generated monitor setup file"
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                _deleteCommand = _deleteCommand ?? new RelayCommand(
                    parameter => { Delete((ViewportSetupFileViewModel) parameter); },
                    parameter =>
                        parameter != null && !((ViewportSetupFileViewModel) parameter).IsCurrentProfile
                );
                return _deleteCommand;
            }
        }

        private class UndoExclude : IUndoItem
        {
            private readonly CombinedMonitorSetupViewModel _parent;
            private readonly ViewportSetupFileViewModel _model;

            public UndoExclude(CombinedMonitorSetupViewModel parent, ViewportSetupFileViewModel model)
            {
                _parent = parent;
                _model = model;
            }

            public void Do()
            {
                if (!_parent.Combined.Any())
                {
                    // parent is disposed
                    return;
                }

                _parent.Exclude(_model);
            }

            public void Undo()
            {
                if (!_parent.Excluded.Any())
                {
                    // parent is disposed
                    return;
                }

                _parent.Combine(_model);
            }
        }

        private class UndoCombine : IUndoItem
        {
            private readonly CombinedMonitorSetupViewModel _parent;
            private readonly ViewportSetupFileViewModel _model;

            public UndoCombine(CombinedMonitorSetupViewModel parent, ViewportSetupFileViewModel model)
            {
                _parent = parent;
                _model = model;
            }

            public void Do()
            {
                if (!_parent.Excluded.Any())
                {
                    // parent is disposed
                    return;
                }

                _parent.Combine(_model);
            }

            public void Undo()
            {
                if (!_parent.Combined.Any())
                {
                    // parent is disposed
                    return;
                }

                _parent.Exclude(_model);
            }
        }
    }
}