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
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Patching.DCS.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GadrocsWorkshop.Helios.Patching.DCS
{
    /// <summary>
    /// Helios interface for the installation of DCS patches related to exported viewports
    /// </summary>
    [HeliosInterface("Patching.DCS.AdditionalViewports", "DCS Additional Viewports", typeof(AdditionalViewportsEditor),
        Factory = typeof(UniqueHeliosInterfaceFactory))]
    public class AdditionalViewports : HeliosInterface, IReadyCheck, IViewportProvider, IStatusReportNotify,
        IExtendedDescription
    {
        #region Private

        private readonly HashSet<IStatusReportObserver> _observers = new HashSet<IStatusReportObserver>();
        
        #endregion

        public AdditionalViewports() : base("DCS Additional Viewports")
        {
            // no code
        }

        protected override void AttachToProfileOnMainThread()
        {
            base.AttachToProfileOnMainThread();
            ViewportPatches = new DCSPatchInstallation(this, "Viewports", "viewport");
            CommunityPatches = new DCSPatchInstallation(this, "DCS Community", "viewport", DCSIntallationType.DCS_Community);
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            base.DetachFromProfileOnMainThread(oldProfile);
            ViewportPatches.Dispose();
            ViewportPatches = null;
            CommunityPatches.Dispose();
            CommunityPatches = null;
        }

        public DCSPatchInstallation ViewportPatches { get; private set; }

        public DCSPatchInstallation CommunityPatches { get; private set; }

        public override void ReadXml(XmlReader reader)
        {
            // no code
        }

        public override void WriteXml(XmlWriter writer)
        {
            // no code
        }

        #region IExtendedDescription

        public string Description =>
            "Utility interface that applies patches to DCS installation files to create additional exported viewports.";

        public string RemovalNarrative =>
            "Delete this interface to no longer let Helios manage viewport patches in DCS.";

        #endregion

        #region IReadyCheck

        public IEnumerable<StatusReportItem> PerformReadyCheck()
        {
            foreach (StatusReportItem item in ViewportPatches.PerformReadyCheck().ToList().Concat(CommunityPatches.PerformReadyCheck().ToList()))
            {
                yield return item;
            }
            yield break;
        }

        #endregion

        #region IStatusReportNotify

        public void Subscribe(IStatusReportObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IStatusReportObserver observer)
        {
            _observers.Remove(observer);
        }

        public void InvalidateStatusReport()
        {
            if (_observers.Count < 1)
            {
                return;
            }

            IList<StatusReportItem> newReport = PerformReadyCheck().ToList();
            PublishStatusReport(newReport);
        }

        public void PublishStatusReport(IList<StatusReportItem> statusReport)
        {
            foreach (IStatusReportObserver observer in _observers)
            {
                observer.ReceiveStatusReport(Name,
                    Description,
                    statusReport);
            }
        }

        #endregion

        #region IViewportProvider

        // For now, this assumes all patches are either included or not (just checks for presence of interface.)        
        public bool IsViewportAvailable(string viewportName) => true;

        #endregion
    }
}