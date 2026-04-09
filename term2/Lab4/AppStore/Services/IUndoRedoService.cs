namespace Project.Services
{
    public interface IUndoRedoService
    {
        bool CanUndo { get; }
        bool CanRedo { get; }
        public void Push(IUndoRedoAction action);
        public void Undo();
        public void Redo();
        public void Clear();
    }
}
