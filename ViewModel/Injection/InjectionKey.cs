namespace DoenaSoft.FolderSize.ViewModel.Injection;

internal sealed class InjectionKey : Tuple<Type, string>
{
    public Type Type => this.Item1;

    public string Id => this.Item2;

    public InjectionKey(Type type, string id) : base(type, id)
    {
    }
}