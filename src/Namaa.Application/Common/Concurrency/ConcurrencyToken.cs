namespace Namaa.Application.Common.Concurrency;

public static class ConcurrencyToken
{
    public static string Encode(byte[] rowVersion)
    {
        ArgumentNullException.ThrowIfNull(rowVersion);
        return Convert.ToBase64String(rowVersion);
    }

    public static bool TryDecode(string? value, out byte[] rowVersion)
    {
        rowVersion = Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var buffer = new byte[value.Length];
        if (!Convert.TryFromBase64String(value, buffer, out var bytesWritten) || bytesWritten != 8)
        {
            return false;
        }

        rowVersion = buffer[..bytesWritten];
        return true;
    }
}
