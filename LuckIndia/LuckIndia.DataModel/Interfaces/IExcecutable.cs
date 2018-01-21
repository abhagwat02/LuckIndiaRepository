namespace LuckIndia.DataModel.Interfaces

{
    interface IExecutable
    {
        int Priority { get; }

        void Execute();
    }
}
