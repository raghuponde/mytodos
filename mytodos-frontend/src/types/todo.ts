export interface Todo {
  id: string
  title: string
  dueDate: string | null
  isComplete: boolean
  createdAt: string
}

export interface CreateTodoRequest {
  title: string
  dueDate?: string | null
}

export interface UpdateTodoRequest {
  title: string
  dueDate?: string | null
  isComplete: boolean
}
