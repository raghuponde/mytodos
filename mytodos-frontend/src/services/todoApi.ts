import axios from 'axios'
import type { CreateTodoRequest, Todo, UpdateTodoRequest } from '../types/todo'

const baseURL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000/api'

const api = axios.create({ baseURL })

function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { message?: string; title?: string } | undefined
    if (data?.message) {
      return data.message
    }
    if (data?.title) {
      return data.title
    }
    if (error.message) {
      return error.message
    }
  }
  return 'Something went wrong. Please try again.'
}

export async function getTodos(): Promise<Todo[]> {
  try {
    const response = await api.get<Todo[]>('/todos')
    return response.data
  } catch (error) {
    throw new Error(extractErrorMessage(error))
  }
}

export async function createTodo(request: CreateTodoRequest): Promise<Todo> {
  try {
    const response = await api.post<Todo>('/todos', request)
    return response.data
  } catch (error) {
    throw new Error(extractErrorMessage(error))
  }
}

export async function updateTodo(id: string, request: UpdateTodoRequest): Promise<Todo> {
  try {
    const response = await api.put<Todo>(`/todos/${id}`, request)
    return response.data
  } catch (error) {
    throw new Error(extractErrorMessage(error))
  }
}

export async function deleteTodo(id: string): Promise<void> {
  try {
    await api.delete(`/todos/${id}`)
  } catch (error) {
    throw new Error(extractErrorMessage(error))
  }
}
