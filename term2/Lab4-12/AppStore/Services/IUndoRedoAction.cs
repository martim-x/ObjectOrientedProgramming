namespace Project.Services
{
    public interface IUndoRedoAction
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
}
