using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace BlankApp
{

    class Grill
    {
        private const String deviceName = "iGrill_mini";
        private BluetoothLEDevice bleDevice;
        public Grill()
        {


        }
        public async Task<Boolean> Init()
        {
            DeviceInformationCollection deviceInformationCollection = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());
            DeviceInformation deviceInformation = deviceInformationCollection.First(x => deviceName.Equals(x.Name));
            bleDevice = await BluetoothLEDevice.FromIdAsync(deviceInformation.Id);
            bleDevice.ConnectionStatusChanged += bleDevice_ConnectionStatusChanged;
            return true;
        }

        void bleDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            System.Diagnostics.Debug.WriteLine("connection status changed! " + args);
        }

        public List<String> GetCapabilities()
        {
            List<string> serviceList = new List<string>();
            foreach (var service in bleDevice.GattServices)
            {
                switch (service.Uuid.ToString())
                {
                    case "00001811-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("AlertNotification");
                        break;
                    case "0000180f-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("Battery");
                        break;
                    case "00001810-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("BloodPressure");
                        break;
                    case "00001805-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CurrentTime");
                        break;
                    case "00001818-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CyclingPower");
                        break;
                    case "00001816-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CyclingSpeedAndCadence");
                        break;
                    case "0000180a-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("DeviceInformation");
                        break;
                    case "00001800-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("GenericAccess");
                        break;
                    case "00001801-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("GenericAttribute");
                        break;
                    case "00001808-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("Glucose");
                        break;
                    case "00001809-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HealthThermometer");
                        break;
                    case "0000180d-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HeartRate");
                        break;
                    case "00001812-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HumanInterfaceDevice");
                        break;
                    case "00001802-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ImmediateAlert");
                        break;
                    case "00001803-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("LinkLoss");
                        break;
                    case "00001819-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("LocationAndNavigation");
                        break;
                    case "00001807-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("NextDstChange");
                        break;
                    case "0000180e-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("PhoneAlertStatus");
                        break;
                    case "00001806-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ReferenceTimeUpdate");
                        break;
                    case "00001814-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("RunningSpeedAndCadence");
                        break;
                    case "00001813-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ScanParameters");
                        break;
                    case "00001804-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("TxPower");
                        break;
                    default:
                        break;
                }
            }
            return serviceList;
        }

        public async Task<int> GetBatteryLevel()
        {
            var immediateAlertService = bleDevice.GetGattService(GattServiceUuids.Battery);
            var characteristics = immediateAlertService.GetCharacteristics(GattCharacteristicUuids.BatteryLevel).First();
            GattReadResult x = await characteristics.ReadValueAsync(BluetoothCacheMode.Uncached);
            byte[] byteArray = new byte[x.Value.Length];
            DataReader.FromBuffer(x.Value).ReadBytes(byteArray);
            int batterylevel = byteArray[0];
            return batterylevel;
        }

        /*
         * 512 - Generic Tag
         * https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.gap.appearance.xml
         */
        public async Task<int> GetAppearance()
        {
            var genericAccessService = bleDevice.GetGattService(GattServiceUuids.GenericAccess);
            var characteristics = genericAccessService.GetCharacteristics(GattCharacteristicUuids.GapAppearance).First();
            GattReadResult x = await characteristics.ReadValueAsync(BluetoothCacheMode.Uncached);
            byte[] byteArray = new byte[x.Value.Length];
            DataReader.FromBuffer(x.Value).ReadBytes(byteArray);
            return BitConverter.ToInt16(byteArray, 0);

        }


        /*
         * 
         * https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.gap.device_name.xml
         */
        public async Task<string> GetDeviceName()
        {
            var genericAccessService = bleDevice.GetGattService(GattServiceUuids.GenericAccess);
            var characteristics = genericAccessService.GetCharacteristics(GattCharacteristicUuids.GapDeviceName).First();
            GattReadResult x = await characteristics.ReadValueAsync(BluetoothCacheMode.Uncached);
            byte[] byteArray = new byte[x.Value.Length];
            DataReader.FromBuffer(x.Value).ReadBytes(byteArray);
            return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

        }


        public async Task<string> GetGenericAttribute()
        {
            var hmm = bleDevice.GattServices;
            /*   foreach(var a in hmm)
               {
                   System.Diagnostics.Debug.WriteLine("a: " + a.Uuid);
               }
             */
            var service = bleDevice.GetGattService(Guid.Parse("63c70000-4a82-4261-95ff-92cf32477861"));
            var caracs = service.GetAllCharacteristics();
           
            foreach (GattCharacteristic k in caracs)
            {
                System.Diagnostics.Debug.WriteLine("x: " + k.Uuid + " , " + k.CharacteristicProperties + ", attributehandle: " + k.AttributeHandle);
            }

            Guid id = Guid.Parse("06ef0003-2e06-4b79-9e33-fce2c42805ec");
            var lol = service.GetCharacteristics(id);
            System.Diagnostics.Debug.WriteLine("--->zcount: " + lol.Count());
            var characteristics = service.GetCharacteristics(id).First();
            try
            {
                characteristics.ProtectionLevel = GattProtectionLevel.EncryptionAndAuthenticationRequired;

               // GattReadClientCharacteristicConfigurationDescriptorResult stat = await characteristics.ReadClientCharacteristicConfigurationDescriptorAsync();
                
                //GattCommunicationStatus stat = await characteristics.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
               //System.Diagnostics.Debug.WriteLine("stass: " + stat);
               // characteristics.ValueChanged += characteristics_ValueChanged;
                
                var x = await characteristics.ReadValueAsync(BluetoothCacheMode.Uncached);
                if (x.Status == GattCommunicationStatus.Success)
                {
                    byte[] byteArray = new byte[x.Value.Length];
                    DataReader.FromBuffer(x.Value).ReadBytes(byteArray);
                    System.Diagnostics.Debug.WriteLine("string value: " + Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
                    System.Diagnostics.Debug.WriteLine("uint16 value: " + BitConverter.ToUInt16(byteArray, 0));
                    System.Diagnostics.Debug.WriteLine("uint32 value: " + BitConverter.ToUInt32(byteArray, 0));
                    System.Diagnostics.Debug.WriteLine("int16 value: " + BitConverter.ToInt16(byteArray, 0));
                    System.Diagnostics.Debug.WriteLine("int32 value: " + BitConverter.ToInt32(byteArray, 0));
                    System.Diagnostics.Debug.WriteLine("slutt");
                }
                   
   
                return "lol";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("message: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("stack: " + ex.StackTrace);
          
            }
            return "lol";

        }

        void characteristics_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("value changed!");
        }

    }
}


