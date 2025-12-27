namespace DesoloZantas.Core.Core
{
    public partial class ConcreteMapDataProcessor
    {
        public ConcreteMapDataProcessor()
        {
        }

        public virtual Dictionary<string, Action<BinaryPacker.Element>> Init()
        {
            return new Dictionary<string, Action<BinaryPacker.Element>>();
        }

        public virtual void Reset()
        {
        }

        public virtual void End()
        {
        }

        public virtual void Run(string stepName, BinaryPacker.Element el)
        {
        }
    }
}



