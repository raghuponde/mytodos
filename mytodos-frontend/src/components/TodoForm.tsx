import { useState, type FormEvent } from 'react'
import type { CreateTodoRequest, Todo } from '../types/todo'

interface TodoFormProps {
  initialValues: Todo | null
  onSubmit: (values: CreateTodoRequest) => void | Promise<void>
  onCancel: (() => void) | null
}

function toDateInputValue(dateString: string | null | undefined): string {
  if (!dateString) {
    return ''
  }
  return dateString.slice(0, 10)
}

function TodoForm({ initialValues, onSubmit, onCancel }: TodoFormProps) {
  const [title, setTitle] = useState(initialValues?.title ?? '')
  const [dueDate, setDueDate] = useState(toDateInputValue(initialValues?.dueDate))
  const [validationError, setValidationError] = useState<string | null>(null)

  const isEditing = Boolean(initialValues)

  function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault()
    const trimmedTitle = title.trim()
    if (!trimmedTitle) {
      setValidationError('Title is required.')
      return
    }
    setValidationError(null)
    onSubmit({ title: trimmedTitle, dueDate: dueDate || null })
    if (!isEditing) {
      setTitle('')
      setDueDate('')
    }
  }

  return (
    <form className="todo-form" onSubmit={handleSubmit}>
      <div className="todo-form-fields">
        <input
          className="todo-form-title"
          type="text"
          placeholder="What needs to be done?"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          autoFocus={isEditing}
        />
        <input
          className="todo-form-date"
          type="date"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
          aria-label="Due date"
        />
        <div className="todo-form-actions">
          <button type="submit" className="btn btn-primary">
            {isEditing ? 'Save' : 'Add todo'}
          </button>
          {isEditing && onCancel && (
            <button type="button" className="btn btn-ghost" onClick={onCancel}>
              Cancel
            </button>
          )}
        </div>
      </div>
      {validationError && <p className="form-error">{validationError}</p>}
    </form>
  )
}

export default TodoForm
