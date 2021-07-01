﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;
using Plugin.BLE;
using System.Diagnostics;

namespace BLETest.Droid
{
    [Activity(Label = "BLETest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private readonly string[] Permissions =
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessBackgroundLocation,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        private IAdapter adapter { get; set; }
        private IBluetoothLE ble { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CheckPermissions();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            ble.StateChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
            };

            adapter.DeviceDiscovered += (s, a) =>
            {
                System.Diagnostics.Debug.WriteLine("Device Discovered!!!!! " + a.Device.Name);
            };

            adapter.ScanTimeoutElapsed += async (a, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Rescanning...");
                await adapter.StartScanningForDevicesAsync();
            };

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            // scan for devices
            adapter.StartScanningForDevicesAsync();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void CheckPermissions()
        {
            bool minimumPermissionsGranted = true;

            foreach (string permission in Permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    minimumPermissionsGranted = false;
                }
            }

            // If any of the minimum permissions aren't granted, we request them from the user
            if (!minimumPermissionsGranted)
            {
                RequestPermissions(Permissions, 0);
            }
        }
    }
}