namespace ScriptableArchitecture.Data
{
    public interface IStats 
    {
        public void SetStats(IStats newStats);

        public void AddStats(IStats otherStats);

        public void ClampStats();
    }
}