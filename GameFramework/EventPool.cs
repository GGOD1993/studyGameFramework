using System;
using System.Collections.Generic;

namespace GameFramework
{
    internal sealed partial class EventPool<T>where T : BaseEventArgs
    {
        private readonly Dictionary<int, EventHandler<T>> m_EventHandlers;
        private readonly Queue<Event> m_Events;
        private readonly EventPoolMode m_EventPoolMode;

        public EventPool(EventPoolMode mode)
        {
            m_EventHandlers = new Dictionary<int, EventHandler<T>>();
            m_Events = new Queue<Event>();
            m_EventPoolMode = mode;
        }

        public int Count
        {
            get
            {
                return m_Events.Count;
            }
        }

        public void Update(float elapseSeconds, float readElapseSeconds)
        {
            while (m_Events.Count > 0)
            {
                Event e = null;
                lock (m_Events)
                {
                    e = m_Events.Dequeue();
                }

                HandlerEvent(e.Sender, e.EventArgs);
            }
        }

        public void Shutdown()
        {
            Clear();
            m_EventHandlers.Clear();
        }

        public void Clear()
        {
            lock (m_Events)
            {
                m_Events.Clear();
            }
        }

        public bool Check(int id, EventHandler<T> handler)
        {
            if(handler == null)
            {
                throw new GameFrameworkException("Event handler is invalid");
            }

            EventHandler<T> handlers = null;
            if (!m_EventHandlers.TryGetValue(id, out handlers))
            {
                return false;
            }

            if (handlers == null)
            {
                return false;
            }

            foreach(EventHandler<T> i in handlers.GetInvocationList())
            {
                if (i == handler)
                {
                    return false;
                }
            }

            return false;
        }

        public void Subscribe(int id, EventHandler<T> handler)
        {
            if(handler == null)
            {
                throw new GameFrameworkException("Event handler is invalid.");
            }

            EventHandler<T> eventHandler = null;
            if (!m_EventHandlers.TryGetValue(id, out eventHandler) || eventHandler == null)
            {
                m_EventHandlers[id] = handler;
            } else if ((m_EventPoolMode & EventPoolMode.AllowMultiHandler) == 0)
            {
                throw new GameFrameworkException(string.Format("Event '{0}' not allow multi handler", id.ToString()));
            } else if ((m_EventPoolMode & EventPoolMode.AllowDuplicateHandler) == 0 && Check(id, handler))
            {
                throw new GameFrameworkException(string.Format("Event '{0}' not allow duplicate handler", id.ToString()));
            } else
            {
                eventHandler += handler;
                m_EventHandlers[id] = eventHandler;
            }
        }

        public void Unsubscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new GameFrameworkException("Event handler is invalid");
            }

            if (m_EventHandlers.ContainsKey(id))
            {
                m_EventHandlers[id] -= handler;
            }
        }

        public void Fire(object sender, T e)
        {
            Event eventNode = new Event(sender, e);
            lock (m_Events)
            {
                m_Events.Enqueue(eventNode);
            }
        }

        public void FireNow(object sender, T e)
        {
            HandlerEvent(sender, e);
        }

        private void HandlerEvent(object sender, T e)
        {
            EventHandler<T> handlers = null;
            if (m_EventHandlers.TryGetValue(e.Id, out handlers))
            {
                if (handlers != null)
                {
                    handlers(sender, e);
                    return;
                }
            }

            if ((m_EventPoolMode & EventPoolMode.AllowNoHandler) == 0)
            {
                throw new GameFrameworkException(string.Format("Event '{0}' not allow no handler.", e.Id.ToString()));
            }
        }
    }
}
