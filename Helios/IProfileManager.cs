//  Copyright 2014 Craig Courtney
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

namespace GadrocsWorkshop.Helios
{
    using System.Windows.Threading;
    using System.Windows.Media.Effects;

    public interface IProfileManager
    {
        bool SaveProfile(HeliosProfile profile);

        /// <summary>
        /// load entire profile synchronously on an arbitrary thread (no longer supported)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        [Obsolete("Load profile on load thread is no longer allowed.  Iterate progress on main thread via IProfileManager2 or load on main thread synchronously via IProfileManager2")]
        HeliosProfile LoadProfile(string path, Dispatcher dispatcher);
    }

    public interface IProfileManager2: IProfileManager
    {
        /// <summary>
        /// load entire profile synchronously on main thread
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        HeliosProfile LoadProfile(string path);

        /// <summary>
        /// Begin loading a profile on main thread and defer all heavy loading work to returned enumeration.
        /// By iterating this enumeration, caller can step execute the loading without blocking for long
        /// periods of time.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loadingWork">iteration of progress strings that step executes the load process</param>
        /// <returns>a loaded profile or null</returns>
        HeliosProfile LoadProfile(string path, out IEnumerable<string> loadingWork);
        /// <summary>
        /// Required here so that the ShaderEffect can be accessed by visuals and gauges.
        /// </summary>
        ShaderEffect CurrentEffect { get; set; }
    }
}
