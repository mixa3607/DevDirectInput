using System.Collections.Generic;
using DevDirectInput.Devices.Events;

namespace DevDirectInput.Replay
{
    public class InputTimeline<T> where T: IInputEvent
    {
        private readonly List<List<T>?> _timeline = new List<List<T>?>();

        public void Add(int tick, T tEvent)
        {
            while (tick >= _timeline.Count)
            {
                _timeline.Add(new List<T>());
            }

            #pragma warning disable CS8602 // list increased to required size in 12 line
            _timeline[tick].Add(tEvent);
            #pragma warning restore CS8602
        }

        public List<List<T>?> GetEventsLists()
        {
            return _timeline;
        }
    }
}