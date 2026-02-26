// Tipos compartidos (DTOs del backend)
export interface User {
  id: number
  email: string
  name: string
  role: string
}

export interface Game {
  id: number
  name: string
  imageUrl: string | null
  year: number | null
  description: string | null
  createdAt: string
}

export interface Character {
  id: number
  name: string
  gameId: number
  gameName?: string | null
  imageUrl: string | null
  description: string | null
  createdAt: string
}
