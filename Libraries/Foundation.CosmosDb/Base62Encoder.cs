using System.Text;

namespace Foundation.CosmosDb;

public static class Base62Encoder
{
    private const uint Radix = 62;
    private const ulong Carry = 297528130221121800; // 2^64 / 62
    private const ulong CarryRemainder = 16;        // 2^64 % 62
    private const string Symbols = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string OutputTemplate = "0000000000000000000000"; // 16 bytes takes up 22 base62 symbols

    public static string ToBase62(this Guid guid)
    {
        return guid.ToByteArray().ToBase62(true); // It's important to set the leading zero to true, since we rely on the length of 22 chars for id validations
    }

    private static string ToBase62(this byte[] bytes, bool zeroPadding = false)
    {
        if (bytes.Length != 16)
            throw new ArgumentOutOfRangeException(nameof(bytes), "Input must be 16 bytes");

        if (!BitConverter.IsLittleEndian)
            throw new Exception("Untested on big endian architecture");

        var lower = BitConverter.ToUInt64(bytes, 0);
        var upper = BitConverter.ToUInt64(bytes, 8);

        var sb = new StringBuilder(OutputTemplate);
        var pos = OutputTemplate.Length;
        uint remainder;
        while (upper != 0)
        {
            DivRem(upper, lower, out upper, out lower, out remainder);
            sb[--pos] = Symbols[(int)remainder];
        }

        do
        {
            DivRem(lower, out lower, out remainder);
            sb[--pos] = Symbols[(int)remainder];
        } while (lower != 0);

        if (zeroPadding) pos = 0;
        return sb.ToString(pos, OutputTemplate.Length - pos);
    }

    private static void DivRem(ulong num, out ulong quot, out uint rem)
    {
        quot = num / Radix;
        rem = (uint)(num - (Radix * quot));
    }

    private static void DivRem(ulong numUpper64, ulong numLower64, out ulong quotUpper, out ulong quotLower, out uint rem)
    {
        DivRem(numUpper64, out quotUpper, out var remUpper);
        DivRem(numLower64, out quotLower, out var remLower);

        // take the upper remainder, and incorporate it into the other lower quotient/lower remainder/output remainder
        remLower += (uint)(remUpper * CarryRemainder); // max value = 61 + 61*16 = 1037
        DivRem(remLower, out var remLowerQuot, out rem);

        // at this point the max values are:
        //   quotientLower: 2^64-17 / 62, which is 297528130221121799 (any more than 2^64-17 and remainderLower will be under 61)
        //   remainderUpper * carry: 61 * 297528130221121800 which is 18149215943488429800
        //   remainderLowerQuotient = 1037 / 62 = 16 
        quotLower += remUpper * Carry;  // max value is now 18446744073709551599
        quotLower += remLowerQuot;  // max value is now 18446744073709551615. So no overflow.
    }
}

public static class Id
{
    public static string NewBase62Id => Guid.CreateVersion7().ToBase62();
}