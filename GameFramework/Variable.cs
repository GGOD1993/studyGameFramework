using System;

namespace GameFramework
{
    public abstract class Variable
    {
        protected Variable()
        {

        }

        public abstract Type Type
        {
            get;
        }

        public abstract object GetValue();

        public abstract void SetValue(object value);

        public abstract void Reset();
    }
}