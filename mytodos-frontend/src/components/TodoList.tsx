import type { Todo } from '../types/todo'
import TodoItem from './TodoItem.tsx'

interface TodoListProps {
  todos: Todo[]
  onToggleComplete: (todo: Todo) => void
  onEdit: (todo: Todo) => void
  onDelete: (id: string) => void
}

function TodoList({ todos, onToggleComplete, onEdit, onDelete }: TodoListProps) {
  if (todos.length === 0) {
    return (
      <div className="empty-state">
        <p className="empty-state-title">Nothing here yet</p>
        <p className="empty-state-subtitle">Add your first todo above to get started.</p>
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
