namespace NetCord
{
    public class LogType
    {
        public static readonly LogType
            Gateway = new("Gateway"),
            Exception = new("Exception");

        private readonly string _value;

        private LogType(string s)
        {
            _value = s;
        }

        public override string ToString() => _value;

        public static bool operator ==(LogType logType, LogType logType2) => logType.Equals(logType2);
        public static bool operator !=(LogType logType, LogType logType2) => !(logType == logType2);

        public override bool Equals(object obj) => _value == ((LogType)obj)._value;

        public override int GetHashCode() => HashCode.Combine(_value);
    }
}
