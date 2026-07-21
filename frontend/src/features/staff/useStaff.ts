import { useQuery } from '@tanstack/react-query'
import { staffApi } from './staffApi'

export function useStaff() {
  return useQuery({
    queryKey: ['staff'],
    queryFn: staffApi.getActive,
  })
}
