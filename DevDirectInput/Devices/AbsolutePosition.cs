namespace DevDirectInput.Devices
{
    public interface IAbsolutePosition
    {
        public int X { get; }
        public int Y { get; }
    }

    public class AbsolutePosition : IAbsolutePosition
    {
        public int X { get; }
        public int Y { get; }

        public AbsolutePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}