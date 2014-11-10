using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZodiacGlass
{
    class ValueChangedEventArgs<T> : EventArgs
    {
        private readonly T oldValue;
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
