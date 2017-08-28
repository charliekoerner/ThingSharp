using LifxMvc.Domain;
using LifxMvc.Services;
using LifxMvc.Services.UdpHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThingSharp.Drivers
{
    public class LifxDriver
    {
        private DiscoveryService mDiscoveryService;
        private BulbService mBulbService;

        public LifxDriver(IPAddress localEndpoint)
        {
            mDiscoveryService = new DiscoveryService(localEndpoint);
            mBulbService = new BulbService();
        }

        public bool IsBulbOffline(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return b.isOffline;
        }

        //********************************************************************
        // Discovery
        //********************************************************************

        public bool DiscoverBulbs()
        {
            bool isDiscoveryStarted = false;
            try
            {
                // Tell the Lifx Service to start discovering bulbs
                mDiscoveryService.DiscoverAsync();
                isDiscoveryStarted = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
                isDiscoveryStarted = false;
            }

            return isDiscoveryStarted;
        }
        //-------------------------------------------------------------
        public List<Object> GetNewBulbs()
        {
            // Get a list of all the newly discovered bulbs
            List<IBulb> bulbs = mDiscoveryService.GetDiscoveredBulbs();
            List<object> bulbIds = new List<object>();
            foreach (IBulb bulb in bulbs)
            {
                bulbIds.Add(bulb);
            }
            
            return bulbIds;
        }
        //-------------------------------------------------------------
        public void StopDiscovery()
        {
            // Tell the service to stop all discovery threads
            mDiscoveryService.StopDiscovery();
        }
        //-------------------------------------------------------------

        //********************************************************************
        // Label - Get
        //********************************************************************

        public string GetBulbLabel(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            mBulbService.GetLabel(b);
            return b.Label;
        }
        //-------------------------------------------------------------
        public string GetBulbObjectLabel(Object bulb)
        {
            IBulb b = (IBulb)bulb;

            if (String.IsNullOrEmpty(b.Label))
            {
                System.Diagnostics.Debug.WriteLine("----- Bulb Label not set ----");
            }

            return b.Label;
        }
        //-------------------------------------------------------------
        public string GetBulbEndPoint(Object bulb)
        {
            IBulb b = (IBulb)bulb;
            return b.IPEndPoint.ToString();
        }
        //-------------------------------------------------------------


        //********************************************************************
        // Power - Get/Set
        //********************************************************************

        public bool? GetBulbPower(Object bulb)
        {
            return mBulbService.LightGetPower((IBulb)bulb);
        }
        //-------------------------------------------------------------
        public bool SetBulbPower(Object bulb, object state)
        {
            return mBulbService.LightSetPower((IBulb)bulb, (bool)state);
        }
        //-------------------------------------------------------------


        //********************************************************************
        // Color - Get/Set
        //********************************************************************

        public String GetBulbColor(Object bulb)
        {
            String color = mBulbService.LightGetColor((IBulb)bulb);

            if(color != null)
            {
                color = String.Format("#{0}", color);
            }

            return color;
        }
        //-------------------------------------------------------------
        public object SetBulbColor(Object bulb, object value)
        {
            System.Drawing.Color color = ParseColor((string)value);
            return mBulbService.LightSetColor((IBulb)bulb, color);
        }
        //-------------------------------------------------------------
        System.Drawing.Color ParseColor(string color)
        {
            // Remove '#' from front if present
            int argb = int.Parse(color.Replace("#", ""), System.Globalization.NumberStyles.HexNumber);

            // Add Alpha to RGB value if missing
            if(color.Length == 6)
            {
                color = "FF" + color;
            }

            return System.Drawing.Color.FromArgb(argb);            
        }
        //-------------------------------------------------------------


        //********************************************************************
        // Brightness - Get/Set
        //********************************************************************

        public ushort? GetBulbBrightness(Object bulb)
        {
            ushort? brightness = mBulbService.LightGetBrightness((IBulb)bulb);

            if (brightness != null)
            {
                // Convert brightness to Percentage value
                brightness = (ushort)Math.Round(((float)brightness / 65535.0) * 100.0, 0);
            }

            return brightness;
        }
        //-------------------------------------------------------------
        public object SetBulbBrightness(Object bulb, object value)
        {
            ushort brightness = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (brightness < 0)
                return false;

            // convert from % to value in range of 0 to 65535
            brightness = brightness > (ushort)100 ? (ushort)100 : brightness;
            brightness = brightness < (ushort)0 ? (ushort)0 : brightness;
            brightness = (ushort)Math.Round(((float)brightness / 100.0) * 65535.0, 0);

            return mBulbService.LightSetBrightness((IBulb)bulb, brightness);
        }
        //-------------------------------------------------------------


        //********************************************************************
        // Saturation - Get/Set
        //********************************************************************

        public ushort? GetBulbSaturation(Object bulb)
        {
            ushort? saturation = mBulbService.LightGetSaturation((IBulb)bulb);

            if (saturation != null)
            {
                // Convert saturation to Percentage value
                saturation = (ushort)Math.Round(((float)saturation / 65535.0) * 100.0, 0);
            }

            return saturation;
        }
        //-------------------------------------------------------------
        public object SetBulbSaturation(Object bulb, object value)
        {
            ushort saturation = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (saturation < 0)
                return false;

            // convert from % to value in range of 0 to 65535
            saturation = saturation > (ushort)100 ? (ushort)100 : saturation;
            saturation = saturation < (ushort)0 ? (ushort)0 : saturation;
            saturation = (ushort)Math.Round(((float)saturation / 100.0) * 65535.0, 0);

            return mBulbService.LightSetSaturation((IBulb)bulb, saturation);
        }
        //-------------------------------------------------------------


        //********************************************************************
        // Temperature (Kelvin) - Get/Set
        //********************************************************************

        public ushort? GetBulbKelvin(Object bulb)
        {
            return mBulbService.LightGetKelvin((IBulb)bulb);
        }
        //-------------------------------------------------------------
        public object SetBulbKelvin(Object bulb, object value)
        {
            ushort kelvin = Convert.ToUInt16(value);

            // If the conversion to an int failed, then return false
            if (kelvin < 0)
                return false;

            // convert from % to value in range of 0 to 65535
            kelvin = kelvin > (ushort)6500 ? (ushort)6500 : kelvin;
            kelvin = kelvin < (ushort)2700 ? (ushort)2700 : kelvin;

            return mBulbService.LightSetKelvin((IBulb)bulb, kelvin);
        }
        //-------------------------------------------------------------
    }

}
