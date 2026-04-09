using System;

namespace Project.Services
{
    /// <summary>Установка приложения. Undo → удалить. Redo → установить снова.</summary>
    public class GetAction : IUndoRedoAction
    {
        public string Name => "Get";
        private readonly Action _undo;
        private readonly Action _redo;

        public GetAction(Action undo, Action redo)
        {
            _undo = undo;
            _redo = redo;
        }

        public void Undo() => _undo();

        public void Redo() => _redo();
    }

    /// <summary>Удаление приложения. Undo → установить снова. Redo → удалить.</summary>
    public class OpenAction : IUndoRedoAction
    {
        public string Name => "Open";
        private readonly Action _undo;
        private readonly Action _redo;

        public OpenAction(Action undo, Action redo)
        {
            _undo = undo;
            _redo = redo;
        }

        public void Undo() => _undo();

        public void Redo() => _redo();
    }
}
