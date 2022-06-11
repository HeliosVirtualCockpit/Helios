// Copyright 2021 Ammo Goettsch
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
using System.Linq;
using System.Windows.Threading;

namespace GadrocsWorkshop.Helios.Interfaces.Keyboard.Input
{
    class KeyboardInput: IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // REVISIT: devices for specific modifiers could be configurable
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable intended to become configurable
        private readonly string[] _modifierSetDeviceNames;

        private readonly Dictionary<string, KeyTriggers> _modifierSetDevices;

        private readonly System.Threading.ManualResetEvent _stoppingEvent = new System.Threading.ManualResetEvent(false);

        private readonly int _retryInterval = 1000;

        private System.Threading.Thread _thread;

        public KeyboardInput(HeliosObject parent)
        {
            // this set of modifiers seems mostly unused in DCS and BMS, so it is a good initial candidate for direct support
            _modifierSetDeviceNames = new [] { "LeftControl LeftShift" };

            // create devices for specific sets of modifiers
            _modifierSetDevices = _modifierSetDeviceNames.ToDictionary(modifiers => modifiers, modifiers => (KeyTriggers) new KeyTriggersFixed(parent, modifiers));

            // base device for keys with any set of modifiers as the value
            BaseDevice = new KeyTriggersWithModifiers(parent);
        }

        /// <summary>
        /// start thread and begin listening for keyboard input
        /// </summary>
        public void Start()
        {
            _stoppingEvent?.Reset();
            _thread = new System.Threading.Thread(ThreadFunction);
            _thread.Start();
        }

        /// <summary>
        /// stop any thread running and free up resources, may be called without Start having been called
        /// and may be called any number of times
        /// </summary>
        public void Stop()
        {
            _stoppingEvent?.Set();
            _thread?.Join();
            _thread = null;
        }

        // enumerate all triggers for installation into parent
        public IEnumerable<HeliosTrigger> Triggers => BaseDevice.Triggers
                .Concat(_modifierSetDevices.SelectMany(pair => pair.Value.Triggers));

        private KeyTriggers BaseDevice { get; }

        private void ThreadFunction() {
            // loop to reconnect to keyboard input
            while (true)
            {
                try
                {
                    SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
                    SharpDX.DirectInput.Keyboard keyboard = new SharpDX.DirectInput.Keyboard(directInput);
                    System.Threading.EventWaitHandle eventHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "keyboard input");
                    keyboard.SetNotification(eventHandle);
                    keyboard.Acquire();
                    Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
                    try
                    {
                        // wait for either stopping or a change in the keyboard state, forever
                        System.Threading.WaitHandle[] anyEvent = { _stoppingEvent, eventHandle };
                        while (0 != System.Threading.WaitHandle.WaitAny(anyEvent))
                        {
                            // eventHandle is set, so read new keyboard state
                            SharpDX.DirectInput.KeyboardState state = keyboard.GetCurrentState();
                            string modifierString = KeyTriggers.ConstructModifierString(state.PressedKeys);

                            foreach (SharpDX.DirectInput.Key key in state.PressedKeys.Where(key =>
                                !KeyTriggers.Modifiers.Contains(key)))
                            {
                                dispatcher.BeginInvoke(DispatcherPriority.Input, new Action<string, SharpDX.DirectInput.Key>(DispatchKey), modifierString, key);
                            }

                            eventHandle.Reset();
                        }

                        // if we ever exit the worker loop without throwing, we are stopping
                        return;
                    }
                    finally
                    {
                        keyboard.Unacquire();
                        keyboard.Dispose();
                        directInput.Dispose();
                    }
                }
                catch (SharpDX.SharpDXException ex)
                {
                    Logger.Error(ex, "Keyboard Interface failed; reinitializing after time out of {MilliSeconds} ms", _retryInterval);
                    if (_stoppingEvent.WaitOne(_retryInterval))
                    {
                        // stop requested
                        return;
                    };
                    // retry after time out by looping
                }
            }
        }

        /// <summary>
        /// dispatch key press, called on UI Main Thread via dispatcher
        /// </summary>
        /// <param name="modifierString"></param>
        /// <param name="key"></param>
        private void DispatchKey(string modifierString, SharpDX.DirectInput.Key key)
        {
            if (_modifierSetDevices.TryGetValue(modifierString, out KeyTriggers device))
            {
                // if modifierString is the name of a device, fire the key on that
                device.FireTrigger(key, new BindingValue(true));
            }
            else
            {
                // else fire key on the base device with the modifier string as the value
                BaseDevice.FireTrigger(key, new BindingValue(modifierString));
            }

            /*
                REVISIT we need to remember keys and modifiers that are currently pressed so we can calculate and fire release events.
                It is unclear what to do about modifiers being released, since we don't really know which modifier combinations
                are of interest.
            */
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
