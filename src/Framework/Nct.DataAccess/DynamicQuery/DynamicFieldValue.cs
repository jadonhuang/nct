using System;
using System.Text.Json.Serialization;

namespace Nct.DataAccess.DynamicQuery
{


    [JsonConverter(typeof(FieldValueJsonConverter))]
    public struct DynamicFieldValue : IConvertible
    {


        private readonly static object Sentinel_Integer = new object();
        private readonly static object Sentinel_Double = new object();
        private readonly static object Sentinel_Bool = new object();
        private readonly static object Sentinel_DateTime = new object();
        private static DateTime UnixTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);




        private readonly object _objectValue;
        private readonly long _intValue;

        public bool IsNull => _objectValue == null;



        public static DynamicFieldValue Null { get; } = new DynamicFieldValue();

        public DynamicFieldValue(long intValue, object value)
        {
            _intValue = intValue;
            _objectValue = value;

        }




        private double ValueDouble
        {
            get => BitConverter.Int64BitsToDouble(_intValue);
        }

        private DateTime ValueDateTime
        {
            get => UnixTimestamp.AddMilliseconds(_intValue).ToLocalTime();
        }

        private long ValueInt64
        {

            get => _intValue;
        }

        //long

        public static long operator +(DynamicFieldValue value0, long value)
        {
            return (long)value0 + value;

        }

        public static long operator -(DynamicFieldValue value0, long value)
        {
            return (long)value0 - value;

        }

        public static long operator *(DynamicFieldValue value0, long value)
        {
            return (long)value0 * value;

        }

        public static long operator /(DynamicFieldValue value0, long value)
        {
            return (long)value0/value;

        }


        public static bool operator >(DynamicFieldValue value0, long value)
        {
            return (decimal)value0 > value;

        }

        public static bool operator <(DynamicFieldValue value0, long value)
        {
            return (decimal)value0 < value;

        }

        public static bool operator >=(DynamicFieldValue value0, long value)
        {
            return (decimal)value0 >= value;

        }

        public static bool operator <=(DynamicFieldValue value0, long value)
        {
            return (decimal)value0 <= value;

        }


        public static bool operator ==(DynamicFieldValue value0, long value)
        {
            return (long)value0 == value;

        }

        public static bool operator !=(DynamicFieldValue value0, long value)
        {
            return (long)value0 != value;

        }


        //string

        public static string operator +(DynamicFieldValue value0, string value)
        {
            return (string)value0 + value;

        }

      

        public static bool operator ==(DynamicFieldValue value0, string value)
        {
            return (string)value0 == value;

        }

        public static bool operator !=(DynamicFieldValue value0, string value)
        {
            return (string)value0 != value;

        }

        //datetime
        public static bool operator >(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 > value;

        }

        public static bool operator <(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 < value;

        }

        public static bool operator >=(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 >= value;

        }

        public static bool operator <=(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 <= value;

        }


        public static bool operator ==(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 == value;

        }

        public static bool operator !=(DynamicFieldValue value0, DateTime value)
        {
            return (DateTime)value0 != value;

        }

        //decimal


        public static decimal operator +(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 + value;

        }

        public static decimal operator -(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 - value;

        }


        public static decimal operator *(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 * value;

        }

        public static decimal operator /(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 / value;

        }


        public static bool operator >(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 > value;

        }

        public static bool operator <(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 < value;

        }

        public static bool operator >=(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 >= value;

        }

        public static bool operator <=(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 <= value;

        }


        public static bool operator ==(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 == value;

        }

        public static bool operator !=(DynamicFieldValue value0, decimal value)
        {
            return (decimal)value0 != value;

        }


        //double


        public static double operator +(DynamicFieldValue value0, double value)
        {
            return (double)value0 + value;

        }

        public static double operator -(DynamicFieldValue value0, double value)
        {
            return (double)value0 -value;

        }

        public static double operator *(DynamicFieldValue value0, double value)
        {
            return (double)value0 * value;

        }

        public static double operator /(DynamicFieldValue value0, double value)
        {
            return (double)value0 / value;

        }



        public static bool operator >(DynamicFieldValue value0, double value)
        {
            return (double)value0 > value;

        }

        public static bool operator <(DynamicFieldValue value0, double value)
        {
            return (double)value0 < value;

        }

        public static bool operator >=(DynamicFieldValue value0, double value)
        {
            return (double)value0 >= value;

        }

        public static bool operator <=(DynamicFieldValue value0, double value)
        {
            return (double)value0 <= value;

        }


        public static bool operator ==(DynamicFieldValue value0, double value)
        {
            return (double)value0 == value;

        }

        public static bool operator !=(DynamicFieldValue value0, double value)
        {
            return (double)value0 != value;

        }




        internal enum ValueType
        {
            Null, Int64, Double, String, Bool, DateTime
        }

        internal ValueType Type
        {
            get
            {
                var objectValue = _objectValue;

                if (objectValue == null) return ValueType.Null;
                if (objectValue == Sentinel_Integer) return ValueType.Int64;
                if (objectValue == Sentinel_Double) return ValueType.Double;
                if (objectValue is string) return ValueType.String;
                if (objectValue == Sentinel_Bool) return ValueType.Bool;
                if (objectValue == Sentinel_DateTime) return ValueType.DateTime;

                throw new InvalidOperationException("Unknown type");
            }
        }

        private DynamicFieldValue Simplify()
        {

            switch (Type)
            {
                case ValueType.String:
                    string s = (string)_objectValue;
                    return s;
                case ValueType.Int64:
                case ValueType.Bool:
                    return _intValue;
                
                  

            }
            return this;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override string ToString()
        {
            if (IsNull) return null;
            switch (Type)
            {
                case ValueType.DateTime:
                    return ValueDateTime.ToString();
                case ValueType.Double:
                    return ValueDouble.ToString();
                case ValueType.Int64:
                    return ValueInt64.ToString();

                case ValueType.Bool:
                    return ((bool)this).ToString();
            }

            return _objectValue?.ToString();


        }




        public static explicit operator int(DynamicFieldValue value)
        {
            return checked((int)(long)value);
        }

        public static explicit operator long(DynamicFieldValue value)
        {

            value = value.Simplify();
            switch (value.Type)
            {
                case ValueType.Null:
                    return 0;
                case ValueType.Int64:
                    return value.ValueInt64;

            }
            throw new InvalidCastException($"Unable to cast from {value.Type} to long: '{value}'");
        }


        public static explicit operator double(DynamicFieldValue value)
        {
            value = value.Simplify();
            switch (value.Type)
            {
                case ValueType.Null:
                    return 0;
                case ValueType.Int64:
                    return value.ValueInt64;

                case ValueType.Double:
                    return value.ValueDouble;
            }
            throw new InvalidCastException($"Unable to cast from {value.Type} to double: '{value}'");
        }

        public static explicit operator decimal(DynamicFieldValue value)
        {
            value = value.Simplify();
            switch (value.Type)
            {
                case ValueType.Null:
                    return 0;
                case ValueType.Int64:
                    return value.ValueInt64;

                case ValueType.Double:
                    return (decimal)value.ValueDouble;
            }
            throw new InvalidCastException($"Unable to cast from {value.Type} to decimal: '{value}'");
        }


        public static explicit operator string(DynamicFieldValue value)
        {
            switch (value.Type)
            {
                case ValueType.Null: return null;
                case ValueType.Double:
                    return value.ValueDouble.ToString();
                case ValueType.Int64:
                    return value.ValueInt64.ToString();
                default:
                    return (string)value._objectValue;
            }
        }

        public static explicit operator bool(DynamicFieldValue value)
        {
            switch ((long)value)
            {
                case 0: return false;
                case 1: return true;
                default: throw new InvalidCastException();
            }
        }


        public static explicit operator DateTime(DynamicFieldValue value)
        {
            switch (value.Type)
            {
                case ValueType.Null: return DateTime.MinValue;
                case ValueType.DateTime:
                    return value.ValueDateTime;
                default: throw new InvalidCastException();
            }
        }

        public static explicit operator DateTime? (DynamicFieldValue value)
        {
            return value.IsNull ? (DateTime?)null : (DateTime)value;
        }



        public static explicit operator double? (DynamicFieldValue value) => value.IsNull ? (double?)null : (double)value;


        public static explicit operator decimal? (DynamicFieldValue value) => value.IsNull ? (decimal?)null : (decimal)value;


        public static explicit operator long? (DynamicFieldValue value) => value.IsNull ? (long?)null : (long)value;


        public static explicit operator int? (DynamicFieldValue value) => value.IsNull ? (int?)null : (int)value;


        public static explicit operator bool? (DynamicFieldValue value) => value.IsNull ? (bool?)null : (bool)value;









        public static implicit operator DynamicFieldValue(bool value)
        {

            return new DynamicFieldValue(value ? 1 : 0, Sentinel_Bool);
        }

        public static implicit operator DynamicFieldValue(bool? value)
        {

            if (value == null) return Null;

            return value.GetValueOrDefault();
        }


        public static implicit operator DynamicFieldValue(DateTime value)
        {

            var timestam = (long)value.ToUniversalTime().Subtract(UnixTimestamp).TotalMilliseconds;

            return new DynamicFieldValue(timestam, Sentinel_DateTime);
        }

        public static implicit operator DynamicFieldValue(DateTime? value)
        {
            if (value == null) return Null;

            return value.GetValueOrDefault();
        }



        public static implicit operator DynamicFieldValue(int value)
        {

            return new DynamicFieldValue(value, Sentinel_Integer);
        }

        public static implicit operator DynamicFieldValue(int? value)
        {

            if (value == null) return Null;

            return value.GetValueOrDefault();
        }



        public static implicit operator DynamicFieldValue(long value)
        {

            return new DynamicFieldValue(value, Sentinel_Integer);
        }



        public static implicit operator DynamicFieldValue(long? value)
        {

            if (value == null) return Null;

            return value.GetValueOrDefault();
        }

        public static implicit operator DynamicFieldValue(double value)
        {

            try
            {
                var i64 = (long)value;
                if (value == i64) return new DynamicFieldValue(i64, Sentinel_Integer);
            }
            catch { }
             return new DynamicFieldValue(BitConverter.DoubleToInt64Bits(value), Sentinel_Double);


        }

        public static implicit operator DynamicFieldValue(double? value)
        {

            if (value == null) return Null;

            return value.GetValueOrDefault();
        }

        public static implicit operator DynamicFieldValue(decimal value)
        {

            try
            {
                var i64 = (long)value;
                if (value == i64) return new DynamicFieldValue(i64, Sentinel_Integer);
            }
            catch { }
            return new DynamicFieldValue(BitConverter.DoubleToInt64Bits(Convert.ToDouble(value)), Sentinel_Double);
        }

        public static implicit operator DynamicFieldValue(decimal? value)
        {

            if (value == null) return Null;

            return value.GetValueOrDefault();
        }


        public static implicit operator DynamicFieldValue(string value)
        {

            return new DynamicFieldValue(0, value);
        }




        public TypeCode GetTypeCode()
        {
            switch (Type)
            {
                case ValueType.Int64:
                    return TypeCode.Int64;
                case ValueType.Double:
                    return TypeCode.Double;
                case ValueType.Bool:
                    return TypeCode.Boolean;
                case ValueType.DateTime:
                    return TypeCode.DateTime;

            }

            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return (bool)this;
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)(long)this;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)(long)this;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)(long)this;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)(long)this;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)(long)this;
        }


        public float ToSingle(IFormatProvider provider)
        {

            return (float)(double)this;
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        public string ToString(IFormatProvider provider)
        {
            return (string)this;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null) throw new ArgumentNullException(nameof(conversionType));


            switch (System.Type.GetTypeCode(conversionType))
            {
                case TypeCode.Boolean: return (bool)this;
                case TypeCode.DateTime: return ToDateTime(provider);
                case TypeCode.Decimal: return (decimal)this;
                case TypeCode.Double: return (double)this;
                case TypeCode.Int16: return (short)this;
                case TypeCode.Int32: return (int)this;
                case TypeCode.Int64: return (long)this;
                case TypeCode.Single: return (float)this;
                case TypeCode.String: return (string)this;
                case TypeCode.UInt16: return checked((ushort)(uint)this);
                case TypeCode.UInt32: return (uint)this;
                case TypeCode.UInt64: return (long)this;
                case TypeCode.Object: return this;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
