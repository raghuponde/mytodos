import type { Todo, TodoStatusFilter } from '../types/todo'
import TodoItem from './TodoItem.tsx'

interface TodoListProps {
  todos: Todo[]
  statusFilter: TodoStatusFilter
  onToggleComplete: (todo: Todo) => void
  onEdit: (todo: Todo) => void
  onDelete: (id: string) => void
}

const EMPTY_STATE_SUBTITLE: Record<TodoStatusFilter, string> = {
  all: 'Add your first todo above to get started.',
  pending: 'No pending todos — nice work!',
  completed: 'No completed todos yet.',
}

function TodoList({ todos, statusFilter, onToggleComplete, onEdit, onDelete }: TodoListProps) {
  if (todos.length === 0) {
    return (
      <div className="empty-state">
        <p className="empty-state-title">Nothing here yet</p>
        <p className="empty-state-subtitle">{EMPTY_STATE_SUBTITLE[statusFilter]}</p>
      </div>
    )
  }

  return (
    <ul className="todo-list">
      {todos.map((todo) => (
        <TodoItem
          key={todo.id}
          todo={todo}
          onToggleComplete={onToggleComplete}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </ul>
  )
}

export default TodoList
