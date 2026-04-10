namespace Billy.Api.Utils
{
    public sealed class BillyDebugLog
    {
        private readonly object syncRoot = new();
        private readonly List<string> entries = [];

        public IReadOnlyList<string> Entries
        {
            get
            {
                lock (syncRoot)
                {
                    return [.. entries];
                }
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                entries.Clear();
            }
        }

        public void Write(string message)
        {
            lock (syncRoot)
            {
                entries.Add(message);
            }
        }

        public override string ToString()
        {
            lock (syncRoot)
            {
                return string.Join(Environment.NewLine + Environment.NewLine, entries);
            }
        }
    }
}
