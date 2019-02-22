namespace Snep
{
    /// <summary>
    /// Enumeration containing snep request codes.
    /// </summary>
    internal enum SnepRequestCode
    {
        Continue = 0x00,
        Get = 0x01,
        Put = 0x02,
        Reject = 0x7F,
    }
}
