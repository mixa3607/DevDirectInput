namespace DevDirectInput.Enums
{
    /// <summary>
    /// from kernel source code
    /// </summary>
    public enum EEventType : uint
    {
        EvSyn = 0x00,
        EvKey = 0x01,
        EvRel = 0x02,
        EvAbs = 0x03,
        EvMsc = 0x04,
        EvSw = 0x05,
        EvLed = 0x11,
        EvSnd = 0x12,
        EvRep = 0x14,
        EvFf = 0x15,
        EvPwr = 0x16,
        EvFfStatus = 0x17,
        EvMax = 0x1f,
        EvCnt = (EvMax + 1),
    }
}