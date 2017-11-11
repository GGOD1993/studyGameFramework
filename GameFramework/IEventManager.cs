using System;

namespace GameFramework.Event
{
    public interface IEventManager
    {
        int Count
        {
            get;
        }

        bool Check(int id, EventHandler<GameEventArgs> handler);
        void Subscribe(int id, EventHandler<GameEventArgs> handler);
        void Unsubscribe(int id, EventHandler<GameEventArgs> handler);
        void Fire(object sender, GameEventArgs e);
        void FireNow(object sender, GameEventArgs e);
    }
}