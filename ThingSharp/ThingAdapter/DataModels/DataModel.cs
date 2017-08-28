using System;
using ThingSharp.Types;

namespace SorisAdapter
{
    public class LIFX_WIFI_LIGHT : Thing
    {
        public LIFX_WIFI_LIGHT(Uri uri)
            : base(uri)
        {
            CC_OMName = "LIFX_WIFI_LIGHT";

            mColor = new Property<string>(uri, "Color") { Name = "Color", Writable = false };
            AddProperty(mColor);

            mSaturation = new Property<uint>(uri, "Saturation") { Name = "Saturation", Writable = true, Min = 0, Max = 100, Units = "%" };
            AddProperty(mSaturation);

            mBrightness = new Property<uint>(uri, "Brightness") { Name = "Brightness", Writable = true, Min = 0, Max = 100, Units = "%" };
            AddProperty(mBrightness);

            mKelvin = new Property<uint>(uri, "Kelvin") { Name = "Kelvin", Writable = true, Min = 2500, Max = 9000, Units = "°" };
            AddProperty(mKelvin);

            mPower = new Property<bool>(uri, "Power") { Name = "Power", Writable = true };
            AddProperty(mPower);

            mLabel = new Property<string>(uri, "Label") { Name = "Label", Writable = false };
            AddProperty(mLabel);

            mInfrared = new Property<uint>(uri, "Infrared") { Name = "Infrared", Writable = true, Min = 0, Max = 100, Units = "%" };
            AddProperty(mInfrared);

        }

        Property<string> mColor;
        public Property<string> Color { get { return mColor; } }

        Property<uint> mSaturation;
        public Property<uint> Saturation { get { return mSaturation; } }

        Property<uint> mBrightness;
        public Property<uint> Brightness { get { return mBrightness; } }

        Property<uint> mKelvin;
        public Property<uint> Kelvin { get { return mKelvin; } }

        Property<bool> mPower;
        public Property<bool> Power { get { return mPower; } }

        Property<string> mLabel;
        public Property<string> Label { get { return mLabel; } }

        Property<uint> mInfrared;
        public Property<uint> Infrared { get { return mInfrared; } }

    }
}
