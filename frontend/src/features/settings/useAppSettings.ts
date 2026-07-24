import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from './settingsApi'
import type { UpdateAppSettingsInput } from './types'

export function useAppSettings() {
  return useQuery({
    queryKey: ['settings'],
    queryFn: settingsApi.get,
  })
}

export function useUpdateAppSettings() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (input: UpdateAppSettingsInput) => settingsApi.update(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['settings'] })
    },
  })
}
