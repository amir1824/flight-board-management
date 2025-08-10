import { type FC, useMemo } from "react"
import type { Provider } from "./provider"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

export const QueryProvider: FC<Provider> = ({ children }) => {
  const queryClient = useMemo(() => new QueryClient({
    defaultOptions: {
      queries: {
        refetchOnWindowFocus: false
      }
    }
  }), []);

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  )
}