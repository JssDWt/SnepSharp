namespace Snep
{
    /// <summary>
    /// Enumaration containing snep response status codes.
    /// </summary>
    public enum SnepResponseCode
    {
        Continue = 0x80,
        Success = 0x81,
        NotFound = 0xC0,
        ExcessData = 0xC1,
        BadRequest = 0xC2,
        NotImplemented = 0xE0,
        UnsopportedVersion = 0xE1,
        Reject = 0xFF
    }
}
