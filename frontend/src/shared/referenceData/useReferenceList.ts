import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { createReferenceListApi } from './referenceListApi'
import type { CreateReferenceListItemInput } from './types'

export function useReferenceList(queryKey: string, basePath: string) {
  const api = createReferenceListApi(basePath)

  return useQuery({
    queryKey: [queryKey],
    queryFn: api.getAll,
  })
}

export function useCreateReferenceListItem(queryKey: string, basePath: string) {
  const api = createReferenceListApi(basePath)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: CreateReferenceListItemInput) => api.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [queryKey] })
    },
  })
}

export function useUpdateReferenceListItem(queryKey: string, basePath: string) {
  const api = createReferenceListApi(basePath)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, input }: { id: string; input: CreateReferenceListItemInput }) => api.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [queryKey] })
    },
  })
}
