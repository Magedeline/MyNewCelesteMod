namespace DesoloZantas.Core.Core.Player
{
    // REMOVE THIS CLASS IF DUPLICATE
    public class IngesteStateMachine
    {
        private class StateCallbacks
        {
            public Func<int> Update { get; set; }
            public object Coroutine { get; set; }
            public Action Begin { get; set; }
            public Action End { get; set; }

            public StateCallbacks(Func<int> update, object coroutine, Action begin, Action end)
            {
                Update = update;
                Coroutine = coroutine;
                Begin = begin;
                End = end;
            }
        }

        private Dictionary<int, StateCallbacks> stateCallbacks;
        private Dictionary<int, string> stateNames;
        public static global::Celeste.Facings Facing;

        public int State { get; set; }
        public bool Locked { get; set; }

        public enum Facings
        {
            Left,
            Right
        }

        public void SetCallbacks(int state, Func<int> update, object coroutine, Action begin, Action end)
        {
            // Store the callbacks for the specified state
            if (stateCallbacks == null) 
                stateCallbacks = new Dictionary<int, StateCallbacks>();

            stateCallbacks[state] = new StateCallbacks(update, coroutine, begin, end);
        }

        public void SetStateName(int state, string name)
        {
            if (stateCallbacks == null) 
                throw new InvalidOperationException("State callbacks are not initialized.");

            if (!stateCallbacks.ContainsKey(state))
                throw new ArgumentException($"State {state} is not registered.", nameof(state));

            // Assuming state names are stored in a dictionary for lookup
            if (stateNames == null) 
                stateNames = new Dictionary<int, string>();

            stateNames[state] = name;
        }

        public void SetCallbacks(int state, Func<int> update, Func<IEnumerator> coroutine, Action begin)
        {
            if (stateCallbacks == null) 
                stateCallbacks = new Dictionary<int, StateCallbacks>();

            stateCallbacks[state] = new StateCallbacks(update, coroutine, begin, null);
        }

        public void SetCallbacks(int state, Func<int> update, Action begin)
        {
            if (stateCallbacks == null) 
                stateCallbacks = new Dictionary<int, StateCallbacks>();

            stateCallbacks[state] = new StateCallbacks(update, null, begin, null);
        }

        public void SetCallbacks(int state, Func<int> update)
        {
            if (stateCallbacks == null) 
                stateCallbacks = new Dictionary<int, StateCallbacks>();

            stateCallbacks[state] = new StateCallbacks(update, null, null, null);
        }
    }
}



