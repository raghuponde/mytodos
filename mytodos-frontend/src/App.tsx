import { useEffect, useState } from 'react'
import TodoList from './components/TodoList.tsx'
import TodoForm from './components/TodoForm.tsx'
import { createTodo, deleteTodo, getTodos, updateTodo } from './services/todoApi.ts'
import type { CreateTodoRequest, Todo, TodoStatusFilter } from './types/todo'

const STATUS_FILTERS: { value: TodoStatusFilter; label: string }[] = [
  { value: 'all', label: 'All' },
  { value: 'pending', label: 'Pending' },
  { value: 'completed', label: 'Completed' },
]

function matchesFilter(todo: Todo, filter: TodoStatusFilter): boolean {
  if (filter === 'pending') return !todo.isComplete
  if (filter === 'completed') return todo.isComplete
  return true
}

function App() {
  const [todos, setTodos] = useState<Todo[]>([])
  const [statusFilter, setStatusFilter] = useState<TodoStatusFilter>('all')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [editingTodo, setEditingTodo] = useState<Todo | null>(null)

  useEffect(() => {
    setLoading(true)
    getTodos(statusFilter)
      .then(setTodos)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [statusFilter])

  async function handleSubmit(values: CreateTodoRequest) {
    setError(null)
    try {
      if (editingTodo) {
        const updated = await updateTodo(editingTodo.id, {
          ...values,
          isComplete: editingTodo.isComplete,
        })
        setTodos((prev) =>
          matchesFilter(updated, statusFilter)
            ? prev.map((t) => (t.id === updated.id ? updated : t))
            : prev.filter((t) => t.id !== updated.id),
        )
        setEditingTodo(null)
      } else {
        const created = await createTodo(values)
        setTodos((prev) => (matchesFilter(created, statusFilter) ? [...prev, created] : prev))
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.')
    }
  }

  async function handleToggleComplete(todo: Todo) {
    setError(null)
    try {
      const updated = await updateTodo(todo.id, {
        title: todo.title,
        dueDate: todo.dueDate,
        isComplete: !todo.isComplete,
      })
      setTodos((prev) =>
        matchesFilter(updated, statusFilter)
          ? prev.map((t) => (t.id === updated.id ? updated : t))
          : prev.filter((t) => t.id !== updated.id),
      )
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.')
    }
  }

  async function handleDelete(id: string) {
    if (!window.confirm('Delete this todo?')) {
      return
    }
    setError(null)
    try {
      await deleteTodo(id)
      setTodos((prev) => prev.filter((t) => t.id !== id))
      if (editingTodo?.id === id) {
        setEditingTodo(null)
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong.')
    }
  }

  const completedCount = todos.filter((t) => t.isComplete).length

  return (
    <div className="app">
      <header className="app-header">
        <div>
          <h1>MyTodos</h1>
          <p className="app-subtitle">A simple place to track what needs doing.</p>
        </div>
        {!loading && statusFilter === 'all' && todos.length > 0 && (
          <div className="stats-badge">
            {completedCount} / {todos.length} done
          </div>
        )}
      </header>

      <div className="status-filter" role="tablist" aria-label="Filter todos by status">
        {STATUS_FILTERS.map(({ value, label }) => (
          <button
            key={value}
            type="button"
            role="tab"
            aria-selected={statusFilter === value}
            className={`status-filter-btn${statusFilter === value ? ' active' : ''}`}
            onClick={() => setStatusFilter(value)}
          >
            {label}
          </button>
        ))}
      </div>

      <TodoForm
        key={editingTodo?.id ?? 'new'}
        initialValues={editingTodo}
        onSubmit={handleSubmit}
        onCancel={editingTodo ? () => setEditingTodo(null) : null}
      />

      {error && (
        <div className="error-banner" role="alert">
          <span>{error}</span>
          <button type="button" className="error-dismiss" onClick={() => setError(null)} aria-label="Dismiss">
            ×
          </button>
        </div>
      )}

      {loading ? (
        <p className="loading-text">Loading todos…</p>
      ) : (
        <TodoList
          todos={todos}
          statusFilter={statusFilter}
          onToggleComplete={handleToggleComplete}
          onEdit={setEditingTodo}
          onDelete={handleDelete}
        />
      )}
    </div>
  )
}

export default App
