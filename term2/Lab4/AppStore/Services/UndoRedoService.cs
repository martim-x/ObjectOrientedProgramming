using System;
using System.Collections.Generic;
using System.Linq;
using Project.Data;

namespace Project.Services.UndoRedo
{
    public class UndoRedoService
    {
        private const int MaxActionsPerUser = 2;

        private readonly Dictionary<Guid, Stack<IUndoRedoAction>> _undoStacks = new();
        private readonly Dictionary<Guid, Stack<IUndoRedoAction>> _redoStacks = new();

        private Stack<IUndoRedoAction> GetUndoStack(Guid userId)
        {
            if (!_undoStacks.TryGetValue(userId, out var stack))
            {
                stack = new Stack<IUndoRedoAction>();
                _undoStacks[userId] = stack;
            }

            return stack;
        }

        private Stack<IUndoRedoAction> GetRedoStack(Guid userId)
        {
            if (!_redoStacks.TryGetValue(userId, out var stack))
            {
                stack = new Stack<IUndoRedoAction>();
                _redoStacks[userId] = stack;
            }

            return stack;
        }

        public void PushAction(Guid userId, IUndoRedoAction action)
        {
            var undo = GetUndoStack(userId);
            var redo = GetRedoStack(userId);

            undo.Push(action);
            redo.Clear();

            while (undo.Count > MaxActionsPerUser)
            {
                var temp = undo.Reverse().Skip(1).Reverse().ToArray();
                undo.Clear();
                foreach (var item in temp)
                    undo.Push(item);
            }
        }

        public bool CanUndo(Guid userId) =>
            _undoStacks.TryGetValue(userId, out var stack) && stack.Count > 0;

        public bool CanRedo(Guid userId) =>
            _redoStacks.TryGetValue(userId, out var stack) && stack.Count > 0;

        public void Undo(Guid userId)
        {
            if (!CanUndo(userId))
                return;

            var undo = GetUndoStack(userId);
            var redo = GetRedoStack(userId);

            var action = undo.Pop();
            action.Undo();
            redo.Push(action);
        }

        public void Redo(Guid userId)
        {
            if (!CanRedo(userId))
                return;

            var undo = GetUndoStack(userId);
            var redo = GetRedoStack(userId);

            var action = redo.Pop();
            action.Redo();
            undo.Push(action);
        }
    }
}
