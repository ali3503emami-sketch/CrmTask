import { useMutation, useQueryClient } from '@tanstack/react-query'
import { staffApi } from './staffApi'

export function useCreateStaffMember() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: staffApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['staff'] })
    },
  })
}
