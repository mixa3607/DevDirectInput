// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace DevDirectInput.Enums
{
    /// <summary>
    /// from kernel source code
    /// </summary>
    public enum EEventCode : uint
    {
        //sync codes
        SynReport = 0,
        SynConfig = 1,
        SynMtReport = 2,
        SynDropped = 3,
        SynMax = 0xf,
        SynCnt = (SynMax + 1),


        //Absolute axes
        AbsX = 0x00,
        AbsY = 0x01,
        AbsZ = 0x02,
        AbsRx = 0x03,
        AbsRy = 0x04,
        AbsRz = 0x05,
        AbsThrottle = 0x06,
        AbsRudder = 0x07,
        AbsWheel = 0x08,
        AbsGas = 0x09,
        AbsBrake = 0x0a,
        AbsHat0x = 0x10,
        AbsHat0y = 0x11,
        AbsHat1x = 0x12,
        AbsHat1y = 0x13,
        AbsHat2x = 0x14,
        AbsHat2y = 0x15,
        AbsHat3x = 0x16,
        AbsHat3y = 0x17,
        AbsPressure = 0x18,
        AbsDistance = 0x19,
        AbsTiltX = 0x1a,
        AbsTiltY = 0x1b,
        AbsToolWidth = 0x1c,
        AbsVolume = 0x20,
        AbsMisc = 0x28,

        //mt
        AbsReserved = 0x2e,
        AbsMtSlot = 0x2f,
        AbsMtTouchMajor = 0x30,
        AbsMtTouchMinor = 0x31,
        AbsMtWidthMajor = 0x32,
        AbsMtWidthMinor = 0x33,
        AbsMtOrientation = 0x34,
        AbsMtPositionX = 0x35,
        AbsMtPositionY = 0x36,
        AbsMtToolType = 0x37,
        AbsMtBlobId = 0x38,
        AbsMtTrackingId = 0x39,
        AbsMtPressure = 0x3a,
        AbsMtDistance = 0x3b,
        AbsMtToolX = 0x3c,
        AbsMtToolY = 0x3d,
        AbsMax = 0x3f,
        AbsCnt = (AbsMax + 1),


        BtnDigi = 0x140,
        BtnToolPen = 0x140,
        BtnToolRubber = 0x141,
        BtnToolBrush = 0x142,
        BtnToolPencil = 0x143,
        BtnToolAirbrush = 0x144,
        BtnToolFinger = 0x145,
        BtnToolMouse = 0x146,
        BtnToolLens = 0x147,
        BtnToolQuinttap = 0x148,
        BtnStylus3 = 0x149,
        BtnTouch = 0x14a,
        BtnStylus = 0x14b,
        BtnStylus2 = 0x14c,
        BtnToolDoubletap = 0x14d,
        BtnToolTripletap = 0x14e,
        BtnToolQuadtap = 0x14f,
    }
}