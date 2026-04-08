namespace Project.Services.UndoRedo
{
    public interface IUndoRedoAction
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
}
