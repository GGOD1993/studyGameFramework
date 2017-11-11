namespace GameFramework
{
    internal interface ITaskAgent<T> where T : ITask
    {
        T Task
        {
            get;
        }

        void Initialize();

        void Update(float elapseSeconds, float readElapseSeconds);

        void Shutdown();

        void Start(T task);

        void Reset();
    }
}