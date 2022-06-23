//  Copyright 2019 Piet Van Nes
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
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using CommandLine;

namespace GadrocsWorkshop.Helios.KeyPressReceiver
{
  static class KeyPressReceiver
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static Mutex mutex = new Mutex(false, "Helios.KeypressReceiver");
    [STAThread]
    static void Main(string[] args)
    {
      // Only allow one instance of the Receiver - added due to user error
      if (!mutex.WaitOne(TimeSpan.FromSeconds(2), false))
      {
        MessageBox.Show("Another instance is already running.\r\nApplication shutting down.", "Helios Keypress Receiver", MessageBoxButtons.OK);
        return;
      }
      try
      {
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(RunOptions).WithNotParsed(HandleParseError);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new KeyPressReceiverForm());
      } catch (ArgumentException e)
            {
                Console.WriteLine($"Keypress Receiver Error:  Command line input argument error {e}.");
            }
      finally
      {
        mutex.ReleaseMutex();
      }
    }
    static void RunOptions(CommandLineOptions opts)
        {
            Properties.Settings.Default.ServerAddress = opts.ServerAddress;
            Properties.Settings.Default.ServerPort = opts.ServerPort;
        }
    static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach(Error err in errs)
            {
                throw new ArgumentException($"Keypress Receiver Command line input argument error {err}");
            }
        }
    }
}
