import type { Todo } from '../types/todo'

interface TodoItemProps {
  todo: Todo
  onToggleComplete: (todo: Todo) => void
  onEdit: (todo: Todo) => void
  onDelete: (id: string) => void
}

function formatDueDate(dueDate: string): string {
  const [year, month, day] = dueDate.slice(0, 10).split('-').map(Number)
  return new Date(year, month - 1, day).toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

function isOverdue(dueDate: string, isComplete: boolean): boolean {
  if (isComplete) {
    return false
  }
  const [year, month, day] = dueDate.slice(0, 10).split('-').map(Number)
  const due = new Date(year, month - 1, day)
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return due < today
}

function TodoItem({ todo, onToggleComplete, onEdit, onDelete }: TodoItemProps) {
  const overdue = todo.dueDate ? isOverdue(todo.dueDate, todo.isComplete) : false

  return (
    <li className={`todo-item${todo.isComplete ? ' completed' : ''}`}>
      <button
        type="button"
        className="todo-checkbox"
        role="checkbox"
        aria-checked={todo.isComplete}
        aria-label={todo.isComplete ? 'Mark as incomplete' : 'Mark as complete'}
        onClick={() => onToggleComplete(todo)}
      >
        {todo.isComplete && (
          <svg viewBox="0 0 16 16" width="11" height="11" aria-hidden="true">
            <path
              d="M2 8.5 6 12l8-8"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.2"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        )}
      </button>

      <div className="todo-content">
        <span className="todo-title">{todo.title}</span>
        {todo.dueDate && (
          <span className={`todo-due-date${overdue ? ' overdue' : ''}`}>
            {overdue ? 'Overdue · ' : 'Due '}
            {formatDueDate(todo.dueDate)}
          </span>
        )}
      </div>

      <div className="todo-actions">
        <button type="button" className="icon-btn" onClick={() => onEdit(todo)} aria-label="Edit todo">
          <svg viewBox="0 0 20 20" width="16" height="16" aria-hidden="true">
            <path
              d="M13.5 3.5 16.5 6.5 7 16H4v-3z"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinejoin="round"
            />
          </svg>
        </button>
        <button
          type="button"
          className="icon-btn icon-btn-danger"
          onClick={() => onDelete(todo.id)}
          aria-label="Delete todo"
        >
          <svg viewBox="0 0 20 20" width="16" height="16" aria-hidden="true">
            <path
              d="M5 6h10M8 6V4.5h4V6M6 6l.6 9.5a1 1 0 0 0 1 .9h4.8a1 1 0 0 0 1-.9L14 6"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </button>
      </div>
    </li>
  )
}

export default TodoItem
