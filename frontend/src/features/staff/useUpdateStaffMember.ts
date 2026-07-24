import { useMutation, useQueryClient } from '@tanstack/react-query'
import { staffApi } from './staffApi'
import type { CreateStaffMemberInput } from './types'

export function useUpdateStaffMember() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, input }: { id: string; input: CreateStaffMemberInput }) => staffApi.update(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['staff'] })
    },
  })
}
