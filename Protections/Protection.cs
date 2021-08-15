namespace LoGiC.NET.Protections
{
    public abstract class Protection
    {
        public string Name { get; set; }

        public abstract void Execute();
    }
}
