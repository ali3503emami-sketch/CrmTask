import { httpClient } from '../api/httpClient'
import type { CreateReferenceListItemInput, ReferenceListItem } from './types'

/**
 * Positions, Customer Categories, and Activity Fields are three identical-shape
 * REST resources (mirrors the backend's shared ReferenceListControllerBase) —
 * one factory instead of tripling this file for each.
 */
export function createReferenceListApi(basePath: string) {
  return {
    getAll: () => httpClient.get<ReferenceListItem[]>(basePath),
    create: (input: CreateReferenceListItemInput) => httpClient.post<ReferenceListItem>(basePath, input),
  }
}
