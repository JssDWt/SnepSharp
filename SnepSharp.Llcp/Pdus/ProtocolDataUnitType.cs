using System;
namespace SnepSharp.Llcp.Pdus
{
    public enum ProtocolDataUnitType
    {
        Symmetry = 0x00,
        ParameterExchange = 0x01,
        AggregatedFrame = 0x02,
        UnnumberedInformation = 0x03,
        Connect = 0x04,
        Disconnect = 0x05,
        ConnectionComplete = 0x06,
        DisconnectedMode = 0x07,
        FrameReject = 0x08,
        Reserved1 = 0x09,
        Reserved2 = 0x0A,
        Reserved3 = 0x0B,
        Information = 0x0C,
        ReceiveReady = 0x0D,
        ReceiveNotReady = 0x0E,
        Reserved4 = 0x0F
    }
}
