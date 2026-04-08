using System;

namespace Project.Services.UndoRedo
{
    public class OpenAction : IUndoRedoAction
    {
        private readonly Action _undo;
        private readonly Action _redo;

        public string Name => "Open";

        public OpenAction(Action undo, Action redo)
        {
            _undo = undo;
            _redo = redo;
        }

        public void Undo() => _undo();
        public void Redo() => _redo();
    }
}ƒ