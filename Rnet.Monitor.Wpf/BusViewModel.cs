﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Threading;

using ReactiveUI;

namespace Rnet.Monitor.Wpf
{

    public class BusViewModel : ReactiveObject
    {

        Dispatcher dispatcher;
        RnetDevice selectedDevice;
        RnetDeviceData selectedDataItem;

        public BusViewModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;

            SentMessages = new ObservableCollection<RnetMessage>();
            ReceivedMessages = new ObservableCollection<RnetMessage>();

            Bus = new RnetBus(new RnetTcpConnection(IPAddress.Parse("72.181.255.134"), 9999));
            Bus.ConnectionStateChanged += Bus_ConnectionStateChanged;
            Bus.MessageSent += (s, a) => SentMessages.Add(a.Message);
            Bus.MessageReceived += (s, a) => ReceivedMessages.Add(a.Message);
            Bus.Error += Bus_Error;

            var canStart = this.WhenAny(i => i.Client.State, i => i.Value == RnetClientState.Stopped);
            StartCommand = new ReactiveCommand(canStart);
            StartCommand.RegisterAsyncAction(i => dispatcher.Invoke(() => Bus.Start()));

            var canStop = this.WhenAny(i => i.Client.State, i => i.Value == RnetClientState.Started);
            StopCommand = new ReactiveCommand(canStop);
            StopCommand.RegisterAsyncAction(i => dispatcher.Invoke(() => Bus.Stop()));

            var canProbeDevice = this.WhenAny(i => i.SelectedDevice, i => i.Value != null);
            ProbeDeviceCommand = new ReactiveCommand(canProbeDevice);
            ProbeDeviceCommand.RegisterAsyncAction(i => dispatcher.Invoke(() => DiscoverDeviceData(SelectedDevice)));

            var canSetData = this.WhenAny(i => i.SelectedDataItem, i => i.Value != null);
            SetDataCommand = new ReactiveCommand(canSetData);
            SetDataCommand.RegisterAsyncAction(i => dispatcher.Invoke(() => SelectedDataItem.SetBufferAsync(new byte[] { 0x00 })));

            // wrap devices in synchronized collection
            Devices = Bus.Devices;

            selectedDataItemViewModel = this.ObservableForProperty(i => i.SelectedDataItem)
                .Select(i => i.Value != null ? new DataItemViewModel(i.Value) : null)
                .ToProperty(this, i => i.SelectedDataItemViewModel);
        }

        void Bus_Error(object sender, RnetClientErrorEventArgs e)
        {
            throw e.Exception;
        }

        void Bus_ConnectionStateChanged(object sender, RnetConnectionStateEventArgs args)
        {
            if (args.State == RnetConnectionState.Open)
                DiscoverDevices();
        }

        public ObservableCollection<RnetMessage> SentMessages { get; private set; }

        public ObservableCollection<RnetMessage> ReceivedMessages { get; private set; }

        public RnetClient Client { get; private set; }

        public RnetBus Bus { get; private set; }

        public ReactiveCommand StartCommand { get; private set; }

        public ReactiveCommand StopCommand { get; private set; }

        public ReactiveCommand ProbeDeviceCommand { get; private set; }

        public ReactiveCommand SetDataCommand { get; private set; }

        public IEnumerable<RnetDevice> Devices { get; private set; }

        public RnetDevice SelectedDevice
        {
            get { return selectedDevice; }
            set { this.RaiseAndSetIfChanged(ref selectedDevice, value); }
        }

        public RnetDeviceData SelectedDataItem
        {
            get { return selectedDataItem; }
            set { this.RaiseAndSetIfChanged(ref selectedDataItem, value); }
        }

        ObservableAsPropertyHelper<object> selectedDataItemViewModel;
        public object SelectedDataItemViewModel
        {
            get { return selectedDataItemViewModel.Value; }
        }

        async void DiscoverDevices()
        {
            await Task.WhenAll(GetDevices());
        }

        IEnumerable<Task<RnetDevice>> GetDevices()
        {
            for (int i = 0; i < 6; i++)
                yield return GetDeviceAsync(new RnetDeviceId(i, 0, RnetKeypadId.Controller));
        }

        async void DiscoverDeviceData(RnetDevice device)
        {
            for (byte i = 0; i < 8; i++)
                await DiscoverDeviceData(device, new RnetPath(i));
        }

        async Task DiscoverDeviceData(RnetDevice device, RnetPath path)
        {
            var d = await GetDataItemAsync(device, path.ToArray());
            if (d != null && d.Data != null && path.Length < 8)
                for (byte i = 0; i < 8; i++)
                    await DiscoverDeviceData(device, path.Navigate(i));
        }

        IEnumerable<RnetPath> GetPaths(RnetPath path, int maxDepth)
        {
            if (path.Length > maxDepth)
                yield break;

            yield return path;

            for (byte i = 0; i < 8; i++)
                foreach (var p in GetPaths(path.Navigate(i), maxDepth))
                    yield return p;
        }

        async Task<RnetDevice> GetDeviceAsync(RnetDeviceId deviceId)
        {
            try
            {
                return await Bus.Devices.GetAsync(deviceId);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        async Task<RnetDeviceDirectory> GetDataItemAsync(RnetDevice device, params byte[] path)
        {
            try
            {
                return await device.Directories.GetAsync(new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token, path);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

    }

}