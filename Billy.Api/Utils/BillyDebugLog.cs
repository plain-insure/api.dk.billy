namespace Billy.Api.Utils
{
    /// <summary>
    /// Thread-safe in-memory sink for Billy API debug output. Pass an instance to
    /// <see cref="ClientExtensions.CreateBillyClient(string, BillyDebugLog)"/> to capture raw
    /// request and response text for diagnostics.
    /// </summary>
    public sealed class BillyDebugLog
    {
        private readonly object syncRoot = new();
        private readonly List<string> entries = [];

        /// <summary>All log entries captured since the last <see cref="Clear"/> call.</summary>
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

        /// <summary>Removes all captured log entries.</summary>
        public void Clear()
        {
            lock (syncRoot)
            {
                entries.Clear();
            }
        }

        /// <summary>Appends <paramref name="message"/> to the log. Called automatically by the HTTP interceptor.</summary>
        public void Write(string message)
        {
            lock (syncRoot)
            {
                entries.Add(message);
            }
        }

        /// <summary>Returns all captured entries joined by double newlines.</summary>
        public override string ToString()
        {
            lock (syncRoot)
            {
                return string.Join(Environment.NewLine + Environment.NewLine, entries);
            }
        }
    }
}
