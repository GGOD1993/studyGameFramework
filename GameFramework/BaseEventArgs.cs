﻿namespace GameFramework
{
    public abstract class BaseEventArgs : GameFrameworkEventArgs
    {
        public abstract int Id
        {
            get;
        }

        public abstract void Clear();
    }
}
