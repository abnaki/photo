using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace photo.exif
{
    public static class Converter
    {
        public static Dictionary<int, ExifItem> Items = Init();

        public static Dictionary<int, ExifItem> Init()
        {
            return TextHelper.GetItems().ToDictionary(x=>x.Id);
        } 

        public static ExifItem ConvertTo(this PropertyItem item)
        {
            ExifItem result;
            if (!Items.TryGetValue(item.Id, out result))
            {
                result = new ExifItem
                             {
                                 Id = item.Id
                             };
            }

            result.Value = item.Value.ConvertTo((ExifType)item.Type, item.Len);
            result.Length = item.Len;

            return result;
        }

        public static object ConvertTo(this byte[] bytes, ExifType type, int len)
        {
            switch (type)
            {
                case ExifType.Byte:
                    return bytes;
                case ExifType.String:
                    return Encoding.ASCII.GetString(bytes);
                case ExifType.UInt16:
                    return BitConverter.ToUInt16(bytes.Safe(2), 0);
                case ExifType.UInt32:
                    return BitConverter.ToUInt32(bytes.Safe(4), 0);
                case ExifType.URational:
                    if (len > 8)
                        return URationalArray(bytes, len);
                    else
                        return URationalFromBytes(bytes, 0);

                case ExifType.Object:
                    return bytes;
                case ExifType.Int32:
                    return BitConverter.ToInt32(bytes.Safe(4), 0);
                case ExifType.Long:
                    return BitConverter.ToInt64(bytes.Safe(8), 0);
                case ExifType.Rational:
                    if (len > 8)
                        return RationalArray(bytes, len);
                    else
                        return RationalFromBytes(bytes, 0);

                default:
                    return bytes;
            }
        }

        static URational URationalFromBytes(byte[] bytes, int offset)
        {
            return new URational
            {
                Denominator = BitConverter.ToUInt32(bytes, offset + 4),
                Numerator = BitConverter.ToUInt32(bytes, offset)
            };
        }

        static URational[] URationalArray(byte[] bytes, int overallLength)
        {
            int n = overallLength / 8;

            return Enumerable.Range(0, n).Select(k => URationalFromBytes(bytes, 8 * k)).ToArray();
        }

        static Rational RationalFromBytes(byte[] bytes, int offset)
        {
            return new Rational
            {
                Denominator = BitConverter.ToInt32(bytes, 0),
                Numerator = BitConverter.ToInt32(bytes, 4)
            };
        }

        static Rational[] RationalArray(byte[] bytes, int overallLength)
        {
            int n = overallLength / 8;

            return Enumerable.Range(0, n).Select(k => RationalFromBytes(bytes, 8 * k)).ToArray();
        }

        public static byte[] Safe(this byte[] bytes, int minimun)
        {
            if (bytes.Length >= minimun) return bytes;

            var safe = new byte[minimun];
            bytes.CopyTo(safe, minimun - bytes.Length);
            return safe;
        }
    }
}
