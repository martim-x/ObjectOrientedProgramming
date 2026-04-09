using System.Collections.Generic;
using System.Linq;

namespace Project.Services
{
    public class UndoRedoService : IUndoRedoService
    {
        private const int MaxActions = 10;

        private readonly Stack<IUndoRedoAction> _undoStack = new();
        private readonly Stack<IUndoRedoAction> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Сохранить уже выполненное действие в историю.
        /// Caller сам вызывает repo-метод ДО Push.
        /// </summary>
        public void Push(IUndoRedoAction action)
        {
            _undoStack.Push(action);
            _redoStack.Clear();

            if (_undoStack.Count > MaxActions)
            {
                var items = _undoStack.ToArray(); // [новый, ..., старый]
                _undoStack.Clear();
                for (int i = MaxActions - 1; i >= 0; i--)
                    _undoStack.Push(items[i]);
            }
        }

        public void Undo()
        {
            if (!CanUndo)
                return;
            var action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;
            var action = _redoStack.Pop();
            action.Redo();
            _undoStack.Push(action);
        }

        /// <summary>Очистить историю при logout — данные одного пользователя не видит другой.</summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
