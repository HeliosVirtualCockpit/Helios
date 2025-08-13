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

using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using GadrocsWorkshop.Helios.Interfaces.Capabilities.ProfileAwareInterface;
using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
using GadrocsWorkshop.Helios.Json;
using GadrocsWorkshop.Helios.Util;
using GadrocsWorkshop.Helios.Windows;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace GadrocsWorkshop.Helios.UDPInterface
{
    public class BaseUDPInterface : HeliosInterface, ISoftInterface
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // const during lifetime, no access control required
        private readonly AsyncCallback _socketDataCallback;

        /// <summary>
        /// This is the locale of the lua exports program
        /// </summary>
        private static readonly System.Text.Encoding Iso88591 =
                System.Text.Encoding.GetEncoding("iso-8859-1");

        // events don't need access control
        // event to notify potentially other threads that the client connection has changed
        public event EventHandler<ClientChange> ClientChanged;

        /// <summary>
        /// accessed only by main thread
        /// </summary>
        private class MainThreadAccess
        {
            // ReSharper disable once InconsistentNaming this is not a constant, but a static value that varies by execution
            private static readonly int _id = System.Threading.Thread.CurrentThread.ManagedThreadId;

            private readonly NetworkFunctionCollection _functions = new NetworkFunctionCollection();

            private readonly Dictionary<string, NetworkFunction> _functionsById =
                new Dictionary<string, NetworkFunction>();

            private EndPoint _client;
            private string _clientID = ClientChange.NO_CLIENT;

            private HeliosTrigger _connectedTrigger;
            private HeliosTrigger _disconnectedTrigger;
            private HeliosTrigger _profileLoadedTrigger;

            private Timer _startuptimer;

            private readonly Dictionary<string, string> _networkAliases
                = new Dictionary<string, string>();

            public NetworkFunctionCollection Functions
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _functions;
                }
            }

            public Dictionary<string, NetworkFunction> FunctionsById
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _functionsById;
                }
            }

            public Dictionary<string, string> NetworkAliases
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _networkAliases;
                }
            }

            public EndPoint Client
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _client;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _client = value;
                }
            }

            public string ClientID
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _clientID;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _clientID = value;
                }
            }

            public HeliosTrigger ConnectedTrigger
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _connectedTrigger;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _connectedTrigger = value;
                }
            }

            public HeliosTrigger DisconnectedTrigger
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _disconnectedTrigger;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _disconnectedTrigger = value;
                }
            }

            public HeliosTrigger ProfileLoadedTrigger
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _profileLoadedTrigger;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _profileLoadedTrigger = value;
                }
            }

            public Timer StartupTimer
            {
                get
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    return _startuptimer;
                }
                set
                {
                    Debug.Assert(System.Threading.Thread.CurrentThread.ManagedThreadId == _id);
                    _startuptimer = value;
                }
            }
        }

        /// <summary>
        /// accessed by multiple threads (main and socket pool, or multiple socket pool)
        /// </summary>
        private class SharedAccess
        {
            // contention between main and socket threads due to read/write and error recovery open/close
            private Socket _socket;

            // pool of receive contexts
            private readonly Queue<ReceiveContext> _receiveContexts = new Queue<ReceiveContext>();
            private const int CACHED_RECEIVE_CONTEXTS = 16;

            private int _port = 9089;

            // lock on all the other fields, public access ok so we can lock larger
            // sections of code and rely on re-entrant locking to avoid deadlock
            public object Lock { get; } = new object();

            public bool Started
            {
                get
                {
                    lock (Lock)
                    {
                        return _socket != null;
                    }
                }
            }

            public Socket ServerSocket
            {
                get
                {
                    lock (Lock)
                    {
                        return _socket;
                    }
                }
                set
                {
                    lock (Lock)
                    {
                        _socket = value;
                    }
                }
            }

            public int Port
            {
                get
                {
                    lock (Lock)
                    {
                        return _port;
                    }
                }
                set
                {
                    lock (Lock)
                    {
                        _port = value;
                    }
                }
            }

            /// <summary>
            /// returns clean ReceiveContext or null
            /// </summary>
            /// <returns></returns>
            public ReceiveContext FetchReceiveContext()
            {
                lock (Lock)
                {
                    if (_socket == null || _receiveContexts.Count <= 0)
                    {
                        return null;
                    }

                    ReceiveContext context = _receiveContexts.Dequeue();
                    context.socket = _socket;
                    return context;
                }
            }

            /// <summary>
            /// adds a receive context back to the pool
            /// </summary>
            public void ReturnReceiveContext(ReceiveContext context)
            {
                context.Clear();
                lock (Lock)
                {
                    if (_receiveContexts.Count < CACHED_RECEIVE_CONTEXTS)
                    {
                        _receiveContexts.Enqueue(context);
                    }
                }
            }
        }

        private class SendContext
        {
            // buffer for datagram to be sent
            // NOTE: send size is entire buffer for now, since we don't reuse these
            public byte[] dataBuffer;
        }

        /// <summary>
        /// owned by the socket thread pool thread that is currently processing the receive operation, then
        /// ownership is handed off to main thread for final processing.  object is not reused (yet)
        /// </summary>
        private class ReceiveContext
        {
            public Socket socket;

            public class Message
            {
                // preallocated space
                public readonly byte[] data = new byte[2048];

                // fill level
                public int bytesReceived;

                // source of datagram received
                public EndPoint fromEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // tokens parsed out on socket thread pool to avoid loading main thread
                public readonly string[] tokens = new string[1024];
                public int tokenCount;

                public void Clear()
                {
                    fromEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    bytesReceived = 0;
                    tokenCount = 0;
                }
            }

            // buffers for datagrams received on one context switch
            // REVISIT: create an array of counters for sizes actually used, use to tune size under heavy load
            private readonly Message[] _messages = new Message[10];

            // number of buffers filled
            private int _messagesFilled;

            /// <summary>
            /// called before we are recycled to pool
            /// </summary>
            public void Clear()
            {
                _messagesFilled = 0;
                socket = null;
            }

            public int Length => _messagesFilled;

            public int Capacity => _messages.Length;

            public Message BeginWrite()
            {
                if (_messagesFilled >= _messages.Length)
                {
                    throw new IndexOutOfRangeException("logic error: attempt to fill receive context past capacity");
                }

                if (_messages[_messagesFilled] == null)
                {
                    // lazy allocate
                    _messages[_messagesFilled] = new Message();
                }
                else
                {
                    _messages[_messagesFilled].Clear();
                }

                return _messages[_messagesFilled];
            }

            public Message ContinueWrite(int index)
            {
                if (index != _messagesFilled)
                {
                    throw new IndexOutOfRangeException("logic error: attempt to continue write that is not current");
                }

                return _messages[index];
            }

            public void EndWrite()
            {
                _messagesFilled++;
            }

            public Message Read(int index)
            {
                if (index >= _messagesFilled)
                {
                    throw new IndexOutOfRangeException("logic error: attempt to read receive context past fill level");
                }

                return _messages[index];
            }
        }

        private readonly MainThreadAccess _main = new MainThreadAccess();
        private readonly SharedAccess _shared = new SharedAccess();

        /// <summary>
        /// we cache the main application dispatcher, because we use it
        /// on every received packet, so let's not keep fetching it from
        /// Application.Current
        /// </summary>
        private readonly Dispatcher _dispatcher;

        public BaseUDPInterface(string name)
            : base(name)
        {
            _dispatcher = System.Windows.Application.Current.Dispatcher;
            _socketDataCallback = OnDataReceived;

            _main.ConnectedTrigger = new HeliosTrigger(this, "", "", "Connected", "Fired on DCS connect.");
            Triggers.Add(_main.ConnectedTrigger);

            _main.DisconnectedTrigger = new HeliosTrigger(this, "", "", "Disconnected", "Fired on DCS disconnect.");
            Triggers.Add(_main.DisconnectedTrigger);

            _main.ProfileLoadedTrigger = new HeliosTrigger(this, "", "", "Profile Delay Start",
                "Fired 10 seconds after DCS profile is started.");
            Triggers.Add(_main.ProfileLoadedTrigger);

            _main.Functions.CollectionChanged += Functions_CollectionChanged;
        }

        private void Functions_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (NetworkFunction function in e.OldItems)
                {
                    Triggers.RemoveSlave(function.Triggers);
                    Actions.RemoveSlave(function.Actions);
                    Values.RemoveSlave(function.Values);

                    foreach (ExportDataElement element in function.DataElements)
                    {
                        if (_main.FunctionsById.ContainsKey(element.ID))
                        {
                            _main.FunctionsById.Remove(element.ID);
                        }
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                foreach (NetworkFunction function in e.NewItems)
                {
                    Triggers.AddSlave(function.Triggers);
                    Actions.AddSlave(function.Actions);
                    Values.AddSlave(function.Values);

                    foreach (ExportDataElement element in function.DataElements)
                    {
                        if (!_main.FunctionsById.ContainsKey(element.ID))
                        {
                            _main.FunctionsById.Add(element.ID, function);
                        }
                        else
                        {
                            Logger.Error("UDP interface created duplicate function ID. (Interface=\"" + Name +
                                         "\", Function ID=\"" + element.ID + "\")");
                        }
                    }
                }
            }
        }

        public int Port
        {
            get => _shared.Port;
            set
            {
                int oldValue;
                lock (_shared.Lock)
                {
                    oldValue = _shared.Port;
                    if (!_shared.Port.Equals(value))
                    {
                        _shared.Port = value;
                    }
                }

                if (!oldValue.Equals(value))
                {
                    OnPropertyChanged("Port", oldValue, value, false);
                }
            }
        }

        public NetworkFunctionCollection Functions => _main.Functions;

        /// <summary>
        /// Global set to disable reading of JSON files and instead use code definitions of interfaces,
        /// so we can generate the JSON.  This is temporary while we have the code in there and the JSON
        /// isn't final.
        ///
        /// XXX eliminate?
        /// </summary>
        public static bool IsWritingFunctionsToJson { get; set; }

        protected bool LoadFunctionsFromJson()
        {
            if (IsWritingFunctionsToJson)
            {
                return false;
            }

            // search for exact name and Existing type
            string jsonFileName = $"{TypeIdentifier}.hif.json";
            string jsonPath = InterfaceHeader.FindInterfaceFile(jsonFileName);
            if (null == jsonPath || !File.Exists(jsonPath))
            {
                Logger.Debug("requested soft interface definition {FileName} not found", jsonFileName);
                return false;
            }


            // load from Json
            // WARNING: this can be InterfaceType.Existing or InterfaceType.DCS, but either way we just use the functions
            InterfaceFile<NetworkFunction> loaded = InterfaceFile<NetworkFunction>.Load(this, jsonPath);

            // if we survive the loading, install all these functions
            InstallFunctions(loaded);
            Logger.Info($"Soft interface definition {jsonFileName} loaded from {Anonymizer.Anonymize(jsonPath)}. (Interface contained {loaded.Functions.Count()} Network Functions).");

            return true;
        }

        protected void InstallFunctions(InterfaceFile<NetworkFunction> loaded)
        {
            // WARNING: all base inherited functions will already exist and will need to be removed
            // in this implementation, because we are loading into an actual instance of the interface class
            Dictionary<string, NetworkFunction> predefined =
                new Dictionary<string, NetworkFunction>(Functions.ToDictionary(f => f.LocalKey, f => f));

            // add or replace functions
            foreach (NetworkFunction function in loaded.Functions
                .Where(f => f != null))
            {
                if (predefined.TryGetValue(function.LocalKey, out NetworkFunction oldFunction))
                {
                    // remove the implementation we inherited
                    Functions.Remove(oldFunction);

                    // only remove it once
                    predefined.Remove(function.LocalKey);
                }

                // now install the new implementation
                AddFunction(function);
            }
        }


        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);
            if (oldProfile != null)
            {
                oldProfile.ProfileStarted -= Profile_ProfileStarted;
                oldProfile.ProfileStopped -= Profile_ProfileStopped;
            }

            if (Profile != null)
            {
                Profile.ProfileStarted += Profile_ProfileStarted;
                Profile.ProfileStopped += Profile_ProfileStopped;
            }
        }

        private void WaitForData(Socket serverSocket)
        {
            ReceiveContext context = _shared.FetchReceiveContext() ?? new ReceiveContext { socket = serverSocket };
            do
            {
                Logger.Debug("UDP interface waiting for socket data on {Interface}.", Name);
                try
                {
                    ReceiveContext.Message message = context.BeginWrite();
                    _ = context.socket.BeginReceiveFrom(message.data, 0, message.data.Length, SocketFlags.None,
                        ref message.fromEndPoint, _socketDataCallback, context);
                    break;
                }
                catch (SocketException se)
                {
                    if (HandleSocketException(se, out context.socket))
                    {
                        // retry forever
                        continue;
                    }

                    Logger.Error(
                        "UDP interface unable to recover from socket reset, no longer receiving data. (Interface=\"" +
                        Name + "\")");
                    break;
                }
            } while (true);
        }

        /// <summary>
        /// socket thread pool callback
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnDataReceived(IAsyncResult asyncResult)
        {
            // WARNING: we must not throw out of this frame unless we are prepared to crash silently
            try
            {
                if (!_shared.Started)
                {
                    // ignore, we shut down since requesting receive
                    Logger.Info("UDP socket closed; stopping receiver");
                    return;
                }

                ReceiveContext context = asyncResult.AsyncState as ReceiveContext;
                try
                {
                    ReceiveContext.Message message = context.ContinueWrite(0);
                    message.bytesReceived = context.socket.EndReceiveFrom(asyncResult, ref message.fromEndPoint);
                    if (message.bytesReceived == 0)
                    {
                        // socket has shut down or closed but not deallocated
                        Logger.Info("UDP socket closing; stopping receiver");
                        return;
                    }

                    context.EndWrite();
                }
                catch (SocketException se)
                {
                    // NOTE: EndReceiveFrom isn't retriable, because the receive won't we valid after we reset socket
                    if (!HandleSocketException(se, out context.socket))
                    {
                        // no new receive attempt
                        return;
                    }

                    // recovered with probably a new socket
                }

                // drain the socket, as much as allowed, to share the context switch to main
                while ((context.socket.Available > 0) && (context.Length < context.Capacity))
                {
                    ReceiveContext.Message message = context.BeginWrite();
                    message.bytesReceived = 0;
                    try
                    {
                        message.bytesReceived = context.socket.ReceiveFrom(message.data, 0, message.data.Length,
                            SocketFlags.None, ref message.fromEndPoint);
                        if (message.bytesReceived > 0)
                        {
                            // if we did not receive anything or throw, we use this slot again
                            context.EndWrite();
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (SocketException se)
                    {
                        if (HandleSocketException(se, out context.socket))
                        {
                            // recovered with probably a new socket
                        }
                        else
                        {
                            // dead, stop trying to drain
                            break;
                        }
                    }
                }

                // save this, because we are recycling the context
                Socket contextSocket = context.socket;

                // it could be empty if all we did this iteration is throw and reset the socket 
                if (context.Length > 0)
                {
                    // offload parsing from main thread to socket thread pool, without lock held
                    ParseReceived(context);

                    // pass ownership to main thread, process synchronously
                    _dispatcher.Invoke(() => DispatchReceived(context),
                        System.Windows.Threading.DispatcherPriority.Send);
                }

                // check if we are still open
                Socket serverSocket = _shared.ServerSocket;
                if (serverSocket != contextSocket)
                {
                    Logger.Info("UDP socket closed while we were reading and dispatching; stopping receiver");
                    return;
                }

                // start next receive
                WaitForData(serverSocket);
            }
            catch (ObjectDisposedException)
            {
                // this can happen because Windows does not support UDP half-close to wake up the
                // async receiver, so we may actually try to read from the socket after it has been
                // hard closed
                Logger.Info("Ignoring UDP receiver failure after closing socket");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fatal error in UDP socket listening worker thread; program will exit");
                ExceptionViewer.DisplayException(ex);
                throw;
            }
        }

        private static void ParseReceived(ReceiveContext owned)
        {
            for (int messageIndex = 0; messageIndex < owned.Length; messageIndex++)
            {
                ReceiveContext.Message message = owned.Read(messageIndex);
                message.tokenCount = 0;
                int parseCount = message.bytesReceived - 1;
                int offset = 8;
                for (int scan = 9; scan < parseCount; scan++)
                {
                    if (message.data[scan] == 0x3a || message.data[scan] == 0x3d)
                    {
                        int size = scan - offset - 1;
                        message.tokens[message.tokenCount++] = Iso88591.GetString(message.data, offset + 1, size);
                        offset = scan;
                    }
                }

                message.tokens[message.tokenCount++] =
                    Iso88591.GetString(message.data, offset + 1, parseCount - offset - 1);
                if (message.tokenCount % 1 > 0)
                {
                    // don't allow odd number of tokens because a lot of the parsing code is unsafe
                    message.tokenCount--;
                }
            }
        }

        private void DispatchReceived(ReceiveContext owned)
        {
            if (owned.Length > 1)
            {
                Logger.Debug("received {MessageCount} UDP messages in batch", owned.Length);
            }

            // REVISIT: could skip ahead if this batch contains a client change
            for (int messageIndex = 0; messageIndex < owned.Length; messageIndex++)
            {
                ReceiveContext.Message message = owned.Read(messageIndex);
                // store address and port, since we need it for outgoing messages
                _main.Client = message.fromEndPoint;
                if (message.bytesReceived < 13)
                {
                    HandleShortMessage(message.data, message.bytesReceived);
                    continue;
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("UDP Interface received packet on {Interface}: {Packet}.", Name,
                        Iso88591.GetString(message.data, 0, message.bytesReceived));
                }

                // handle client restart or change in client
                string packetClientID = Iso88591.GetString(message.data, 0, 8);
                if (!_main.ClientID.Equals(packetClientID))
                {
                    Logger.Info("UDP interface new client connected, sending data reset command. (Interface=\"" + Name +
                                "\", Client=\"" + _main.Client + "\", Client ID=\"" + packetClientID + "\")");
                    string fromValue = _main.ClientID;
                    _main.ClientID = packetClientID;
                    OnClientChanged(fromValue, packetClientID);
                    SendData("R");
                }

                // tokenCount is already even at this point because we fix it up during pre-parsing
                Debug.Assert(message.tokenCount % 1 == 0);
                for (int tokenIndex = 0; tokenIndex < message.tokenCount; tokenIndex += 2)
                {
                    string id = message.tokens[tokenIndex];
                    if (DispatchReceived(id, message.tokens[tokenIndex + 1]))
                    {
                        continue;
                    }

                    if (id == "DISCONNECT")
                    {
                        // if DISCONNECT is formatted as a valid message with a value, we get here, otherwise we
                        // handled it in HandleShortMessage
                        Logger.Debug("UDP interface received disconnect message from simulator.");
                        _main.DisconnectedTrigger.FireTrigger(BindingValue.Empty);
                    }
                    else
                    {
                        OnUnrecognizedFunction(id, message.tokens[tokenIndex + 1]);
                    }
                }
            }

            _shared.ReturnReceiveContext(owned);
        }

        /// <summary>
        /// helper to dispatch or re-dispatch a value received from the network side
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public virtual bool DispatchReceived(string id, string value)
        {
            if (!_main.FunctionsById.TryGetValue(id, out NetworkFunction function))
            {
                // no handler for received id
                return false;
            }
            function.ProcessNetworkData(id, value);

            // pay the cost of looking in a usually empty alias table
            if (_main.NetworkAliases.TryGetValue(id, out string secondaryId) &&
                _main.FunctionsById.TryGetValue(secondaryId, out NetworkFunction secondaryFunction))
            {
                // dispatch to other function also
                secondaryFunction.ProcessNetworkData(secondaryId, value);
            }
            return true;
        }

        protected virtual void OnUnrecognizedFunction(string id, string value)
        {
            Logger.Warn($"UDP interface received data for missing function. (Key=\"{id}\")");
        }

        private void HandleShortMessage(byte[] dataBuffer, int receivedByteCount)
        {
            string RecString = Iso88591.GetString(dataBuffer, 0, receivedByteCount);
            // Special case for legacy Disconnect
            if (RecString.Contains("DISCONNECT"))
            {
                Logger.Info("UDP interface disconnect from Lua.");
                _main.DisconnectedTrigger.FireTrigger(BindingValue.Empty);
                return;
            }

            Logger.Warn("UDP interface short packet received. (Interface=\"" + Name + "\")");
        }

        // WARNING: called on both Main and Socket threads, depending on where the failure occurred
        private bool HandleSocketException(SocketException se, out Socket newSocket)
        {
            if ((SocketError)se.ErrorCode == SocketError.ConnectionReset)
            {
                Logger.Debug("UDP receiver socket reset");
                try
                {
                    CloseSocket();
                    newSocket = OpenSocket();
                }
                catch (SocketException secondException)
                {
                    Logger.Error("UDP interface threw exception (Interface=\"" + Name + "\")", se);
                    Logger.Error(
                        "UDP interface then threw exception reopening socket; cannot continue. (Interface=\"" + Name +
                        "\")", secondException);
                    newSocket = null;
                    return false;
                }

                return true;
            }

            Logger.Error("UDP interface threw unhandled exception handling socket reset. (Interface=\"" + Name + "\")",
                se);
            newSocket = null;
            return false;
        }

        public void SendData(string data)
        {
            try
            {
                if ((_main.Client != null) && (_main.ClientID != ClientChange.NO_CLIENT))
                {
                    Logger.Debug("UDP interface sending data on {Interface}: {Packet}.", Name, data);
                    SendContext context = new SendContext();
                    context.dataBuffer = Iso88591.GetBytes(data + "\n");
                    Socket socket = _shared.ServerSocket;
                    socket?.BeginSendTo(context.dataBuffer, 0, context.dataBuffer.Length, SocketFlags.None,
                        _main.Client, OnDataSent, context);
                }
            }
            catch (SocketException se)
            {
                // just ignore
                Logger.Debug(
                    $"UDP interface threw handled socket exception \"{se.Message}\" while sending data. (Interface=\"" +
                    Name + "\", Data=\"" + data + "\")");
                HandleSocketException(se, out Socket unused);
            }
            catch (Exception e)
            {
                Logger.Error(e, "UDP interface threw exception sending data on {Interface}.", Name);
            }
        }

        /// <summary>
        /// socket thread pool callback
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnDataSent(IAsyncResult asyncResult)
        {
            SendContext context = asyncResult.AsyncState as SendContext;
            // currently we don't need to do anything on send
            // we are only using the async send API in order to match async reads and writes,
            // because we don't want to be an atypical user of the API
        }

        // WARNING: called on both Main and Socket threads, depending on where a socket exception occurred
        private Socket OpenSocket()
        {
            EndPoint bindEndPoint = new IPEndPoint(IPAddress.Any, _shared.Port);
            Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            socket.ExclusiveAddressUse = false;
            socket.Bind(bindEndPoint);
            _shared.ServerSocket = socket;

            // activate receiver
            Logger.Debug("starting UDP receiver for newly opened socket");
            WaitForData(socket);

            // we need to return this to the caller, so they don't rely on _shared.ServerSocket, in case it immediately gets changed by another thread
            return socket;
        }

        private void CloseSocket()
        {
            Socket socket;
            Logger.Debug("removing UDP server socket from service");
            lock (_shared.Lock)
            {
                socket = _shared.ServerSocket;
                _shared.ServerSocket = null;
            }

            // hard closing socket because shutdown does not wake up async UDP receive on Windows
            Logger.Debug("closing UDP server socket");
            socket?.Close();
        }

        private void Profile_ProfileStarted(object sender, EventArgs e)
        {
            Logger.Debug("UDP interface {Interface} starting.", Name);

            // hook for descendants to initialize for a profile start before we receive traffic
            OnProfileStarted();

            // 10 seconds for Delayed Startup (fire only once)
            Timer timer = new Timer(10000) { AutoReset = false };
            timer.Elapsed += OnStartupTimer;
            _main.StartupTimer = timer;

            // start delayed start timer
            Logger.Debug("Starting startup timer.");
            timer.Start();

            // now go active
            try
            {
                _main.Client = new IPEndPoint(IPAddress.Any, 0);
                _main.ClientID = "";
                _ = OpenSocket();
            }
            catch (SocketException se)
            {
                // NOTE: we continue to run even if we can't open sockets
                Logger.Error("UDP interface startup error. (Interface=\"" + Name + "\")");
                Logger.Error("UDP Socket Exception on Profile Start.  " + se.Message, se);
            }
        }

        private void Profile_ProfileStopped(object sender, EventArgs e)
        {
            CloseSocket();
            if (_main.StartupTimer != null)
            {
                _main.StartupTimer.Stop();
            }

            // hook for descendants
            OnProfileStopped();
        }

        /// <summary>
        /// timer thread callback
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnStartupTimer(object source, ElapsedEventArgs e)
        {
            // sync notify
            _dispatcher.Invoke(OnDelayedStartup);
        }

        private void OnDelayedStartup()
        {
            Logger.Debug("Delayed startup timer triggered.");
            _main.ProfileLoadedTrigger.FireTrigger(BindingValue.Empty);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            // No Op
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            // No Op
        }

        protected void AddFunction(NetworkFunction function)
        {
            Functions.Add(function);
            if (GlobalOptions.HasLogDCSFunctionDictionary)
            {
                DCSFunction dcsFunction = function as DCSFunction;
                if (dcsFunction != null)
                {
                    foreach (ExportDataElement de in dcsFunction.DataElements)
                    {
                        Logger.Info($"{dcsFunction.SourceInterface.Name} | {dcsFunction.DeviceName} | {dcsFunction.Name} | Arg = ({de.ID})");
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            foreach (NetworkFunction function in Functions)
            {
                function.Reset();
            }

            SendData("R");
        }

        public bool CanSend => (_shared.Started && (_main.Client != null) && (_main.ClientID.Length > 0));

        protected virtual void OnProfileStarted()
        {
            // no code in base implementation
        }

        protected virtual void OnProfileStopped()
        {
            // no code in base implementation
        }

        protected virtual void OnClientChanged(string fromValue, string toValue)
        {
            _main.ConnectedTrigger.FireTrigger(BindingValue.Empty);
            ClientChanged?.Invoke(this, new ClientChange { FromOpaqueHandle = fromValue, ToOpaqueHandle = toValue });
        }

        /// <summary>
        /// dispatch messages received with either of the given IDs to any functions currently registered for either of them
        /// </summary>
        /// <param name="firstId"></param>
        /// <param name="secondId"></param>
        protected void DuplicateReceivedNetworkData(string firstId, string secondId)
        {
            _main.NetworkAliases.Add(firstId, secondId);
            _main.NetworkAliases.Add(secondId, firstId);
        }

        public Type ResolveFunctionType(string typeName) => this.ResolveFunctionType(typeName, "");
        public Type ResolveFunctionType(string typeName, string assemblyName)
        {
            Type type = null;
            // this will intentionally break attempts to instantiate things outside the prefix
            if (!typeName.StartsWith(NetworkFunction.OMITTED_PREFIX))
            {
                typeName = $"{NetworkFunction.OMITTED_PREFIX}{typeName}";
            }
            type = Type.GetType(typeName);
            if (type == null)
            {
                type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(typeName)).Where(t => t != null).First();
            }
            return type;
        }
    }
}