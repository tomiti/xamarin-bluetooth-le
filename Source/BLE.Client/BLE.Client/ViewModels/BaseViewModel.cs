﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace BLE.Client.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        protected readonly IAdapter Adapter;
        protected const string DeviceIdKey = "DeviceIdNavigationKey";
        protected const string ServiceIdKey = "ServiceIdNavigationKey";
        protected const string CharacteristicIdKey = "CharacteristicIdNavigationKey";

        public BaseViewModel(IAdapter adapter)
        {
            Adapter = adapter;
        }

        public virtual void Resume()
        {
            Mvx.Trace("Resume {0}", GetType().Name);
        }

        public virtual void Suspend()
        {
            Mvx.Trace("Suspend {0}", GetType().Name);
        }

        protected override void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            Bundle = parameters;
        }

        protected IMvxBundle Bundle { get; private set; }

        protected IDevice GetDeviceFromBundle(IMvxBundle parameters)
        {
            if (!parameters.Data.ContainsKey(DeviceIdKey)) return null;
            var deviceId = parameters.Data[DeviceIdKey];

            return Adapter.ConnectedDevices.FirstOrDefault(d => d.Id.ToString().Equals(deviceId));

        }

        protected Task<IService> GetServiceFromBundleAsync(IMvxBundle parameters)
        {

            var device = GetDeviceFromBundle(parameters);
            if (device == null || !parameters.Data.ContainsKey(ServiceIdKey))
            {
                return Task.FromResult<IService>(null);
            }

            var serviceId = parameters.Data[ServiceIdKey];
            return device.GetServiceAsync(Guid.Parse(serviceId));
        }

        protected async Task<ICharacteristic> GetCharacteristicFromBundleAsync(IMvxBundle parameters)
        {
            var service = await GetServiceFromBundleAsync(parameters);
            if (service == null || !parameters.Data.ContainsKey(CharacteristicIdKey))
            {
                return null;
            }

            var characteristicId = parameters.Data[CharacteristicIdKey];
            return await service.GetCharacteristicAsync(Guid.Parse(characteristicId));
        }
    }
}