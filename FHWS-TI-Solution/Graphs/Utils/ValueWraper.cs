using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphs.Utils
{
    class ValueWraper<T> : PropertyChangedBase
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set { _value = value; OnNotifyPropertyChanged(); }
        }

        public ValueWraper(T value)
        {
            _value = value;
        }

        public ValueWraper()
        {
            _value = default(T);
        }
    }
}
