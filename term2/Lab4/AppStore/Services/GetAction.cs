using System;

namespace Project.Services.UndoRedo
{
    public class GetAction : IUndoRedoAction
    {
        private readonly Action _undo;
        private readonly Action _redo;

        public string Name => "Get";

        public GetAction(Action undo, Action redo)
        {
            _undo = undo;
            _redo = redo;
        }

        public void Undo() => _undo();

        public void Redo() => _redo();
    }
}
