﻿using System.Diagnostics;
using System.Threading.Tasks;

using Rnet.Profiles.Media;

namespace Rnet.Profiles.Russound
{

    /// <summary>
    /// Implements <see cref="IZoneAudio"/> for a zone on a Russound controller.
    /// </summary>
    class RussoundZoneAudioProfile : ZoneProfileBase, IZoneAudio
    {

        RnetDataHandle runHandle;
        RnetDataHandle zoneHandle;

        RnetDataHandle powerHandle;
        Power power;

        RnetDataHandle volumeHandle;
        int volume;

        RnetDataHandle bassHandle;
        int bass;

        RnetDataHandle trebleHandle;
        int treble;

        RnetDataHandle loudnessHandle;
        Loudness loudness;

        RnetDataHandle balanceHandle;
        int balance;

        RnetDataHandle partyModeHandle;
        PartyMode partyMode;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="zone"></param>
        public RussoundZoneAudioProfile(RnetZone zone)
            : base(zone)
        {
            Trace.TraceInformation("RussoundZoneAudioProfile:ctor Id={0}", (int)zone.Id);
            runHandle =
                Zone.Controller[2, 0];
            zoneHandle =
                Zone.Controller[2, 0, Zone.Id, 7];
            powerHandle =
                Zone.Controller[2, 0, Zone.Id, 6];
            volumeHandle =
                Zone.Controller[2, 0, Zone.Id, 1];
            bassHandle =
                Zone.Controller[2, 0, Zone.Id, 0, 0];
            trebleHandle =
                Zone.Controller[2, 0, Zone.Id, 0, 1];
            loudnessHandle =
                Zone.Controller[2, 0, Zone.Id, 0, 2];
            balanceHandle =
                Zone.Controller[2, 0, Zone.Id, 0, 3];
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await zoneHandle.Subscribe(d =>
                ReceiveZone(d));
            await powerHandle.Subscribe(d =>
                ReceivePower(d[0]));
            await volumeHandle.Subscribe(d =>
                ReceiveVolume(d[0]));
            await bassHandle.Subscribe(d =>
                ReceiveBass(d[0]));
            await trebleHandle.Subscribe(d =>
                ReceiveTreble(d[0]));
            await loudnessHandle.Subscribe(d =>
                ReceiveLoudness(d[0]));
            await balanceHandle.Subscribe(d =>
                ReceiveBalance(d[0]));
        }

        /// <summary>
        /// Receives all the zone data.
        /// </summary>
        /// <param name="d"></param>
        void ReceiveZone(byte[] d)
        {
            ReceivePower(d[0]);
            ReceiveVolume(d[2]);
            ReceiveBass(d[3]);
            ReceiveTreble(d[4]);
            ReceiveLoudness(d[5]);
            ReceiveBalance(d[6]);
            ReceivePartyMode(d[9]);
        }

        public Power Power
        {
            get { return power; }
            set { power = value; RaisePropertyChanged("Power"); ChangePower(); }
        }

        void ReceivePower(byte value)
        {
            power = (Power)value;
            RaisePropertyChanged("Power");
        }

        public async void ChangePower()
        {
            await powerHandle.SendEvent(RnetEvent.ZoneOnOff, (ushort)power, Zone.Id);
        }

        public async void PowerToggle()
        {
            await powerHandle.SendEvent(RnetEvent.AllZonesOnOff, (int)(power == Power.On ? Power.Off : Power.On));
        }

        public async void PowerOn()
        {
            await powerHandle.SendEvent(RnetEvent.ZoneOnOff, (ushort)Power.On, Zone.Id);
        }

        public async void PowerOff()
        {
            await powerHandle.SendEvent(RnetEvent.ZoneOnOff, (ushort)Power.On, Zone.Id);
        }

        public int Volume
        {
            get { return volume; }
            set { volume = value; RaisePropertyChanged("Volume"); ChangeVolume(); }
        }

        void ReceiveVolume(byte value)
        {
            volume = value * 2;
            RaisePropertyChanged("Volume");
        }

        async void ChangeVolume()
        {
            await runHandle.SendEvent(RnetEvent.SetZoneVolume, volume / 2, Zone.Id);
            await volumeHandle.Refresh();
        }

        public async void VolumeUp()
        {
            await volumeHandle.SendEvent(RnetEvent.Plus);
        }

        public async void VolumeDown()
        {
            await volumeHandle.SendEvent(RnetEvent.Minus);
        }

        public int Bass
        {
            get { return bass; }
            set { bass = value; RaisePropertyChanged("Bass"); ChangeBass(); }
        }

        void ReceiveBass(byte value)
        {
            bass = value - 10;
            RaisePropertyChanged("Bass");
        }

        async void ChangeBass()
        {
            await bassHandle.Write((byte)(bass + 10));
        }

        public async void BassUp()
        {
            await bassHandle.SendEvent(RnetEvent.Plus);
        }

        public async void BassDown()
        {
            await bassHandle.SendEvent(RnetEvent.Minus);
        }

        public int Treble
        {
            get { return treble; }
            set { treble = value; RaisePropertyChanged("Treble"); ChangeTreble(); }
        }

        void ReceiveTreble(byte value)
        {
            treble = value - 10;
            RaisePropertyChanged("Treble");
        }

        async void ChangeTreble()
        {
            await trebleHandle.Write((byte)(treble + 10));
        }

        public async void TrebleUp()
        {
            await trebleHandle.SendEvent(RnetEvent.Plus);
        }

        public async void TrebleDown()
        {
            await trebleHandle.SendEvent(RnetEvent.Minus);
        }

        public Loudness Loudness
        {
            get { return loudness; }
            set { loudness = value; RaisePropertyChanged("Loudness"); ChangeLoudness(); }
        }

        void ReceiveLoudness(byte value)
        {
            loudness = (Loudness)value;
            RaisePropertyChanged("Loudness");
        }

        async void ChangeLoudness()
        {
            await loudnessHandle.Write((byte)loudness);
        }

        public int Balance
        {
            get { return balance; }
            set { balance = value; RaisePropertyChanged("Balance"); ChangeBalance(); }
        }

        void ReceiveBalance(byte value)
        {
            balance = value - 10;
            RaisePropertyChanged("Balance");
        }

        async void ChangeBalance()
        {
            await balanceHandle.Write((byte)(balance + 10));
        }

        public async void BalanceLeft()
        {
            await balanceHandle.SendEvent(RnetEvent.Minus);
        }

        public async void BalanceRight()
        {
            await balanceHandle.SendEvent(RnetEvent.Plus);
        }

        public PartyMode PartyMode
        {
            get { return partyMode; }
            set { partyMode = value; RaisePropertyChanged("PartyMode"); ChangePartyMode(); }
        }

        void ReceivePartyMode(byte value)
        {
            partyMode = (PartyMode)value;
            RaisePropertyChanged("PartyMode");
        }

        async void ChangePartyMode()
        {
            await partyModeHandle.Write((byte)partyMode);
        }

    }

}