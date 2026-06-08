namespace Billy.Api.Repositories
{
    public abstract partial class Base<T, TRoot>
        where T : class, IEntity
        where TRoot : class, new()
    {
        internal abstract class SideloadDescriptor(string includeProperty)
        {

            public string IncludeProperty { get; set; } = includeProperty;

            internal abstract void ApplyAll(TRoot root, IList<T> items);
        }


        private sealed class SingleSideloadDescriptor<S>(
            Func<TRoot, IEnumerable<S>?> getCandidates,
            Func<T, string?> getId,
            Action<T, S> apply, string includeProperty) : SideloadDescriptor(includeProperty)
            where S : class, IEntity
        {
            internal override void ApplyAll(TRoot root, IList<T> items)
            {
                var dict = getCandidates(root)
                    ?.Where(s => s.Id is not null)
                    .ToDictionary(s => s.Id!) ?? [];
                foreach (var item in items)
                {
                    var id = getId(item);
                    if (id is not null && dict.TryGetValue(id, out var match))
                        apply(item, match);
                }
            }
        }

        private sealed class ListSideloadDescriptor<S>(
            Func<TRoot, IEnumerable<S>?> getCandidates,
            Func<T, IEnumerable<string>?> getIds,
            Action<T, S> append,
            string includeProperty) : SideloadDescriptor(includeProperty)
            where S : class, IEntity
        {
            internal override void ApplyAll(TRoot root, IList<T> items)
            {
                var dict = getCandidates(root)
                    ?.Where(s => s.Id is not null)
                    .ToDictionary(s => s.Id!) ?? [];
                foreach (var item in items)
                {
                    var ids = getIds(item);
                    if (ids is null) continue;
                    foreach (var id in ids)
                        if (dict.TryGetValue(id, out var match))
                            append(item, match);
                }
            }
        }


    }
}
