namespace ZodiacGlass
{
    using System;
    using System.Diagnostics;

    public class ValueChangedEventArgs<T> : EventArgs
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T oldValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T newValue;

        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public T OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        public T NewValue
        {
            get
            {
                return this.newValue;
            }
        }
    }
}
