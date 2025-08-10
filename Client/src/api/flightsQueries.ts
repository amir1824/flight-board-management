import { useMutation, useQuery,keepPreviousData } from "@tanstack/react-query";
import {
  addFlight,
  deleteFlight,
  getFlights,
  searchFlights,
} from "./flightsApi";
import type { FlightModel } from "../store/dbModels/FlightModel";

interface FlightsListParams {
  page?: number;
  pageSize?: number;
  status?: string;
  destination?: string;
} 

function normalizeFlights(res: unknown): FlightModel[] {
  if (Array.isArray(res)) return res;
  if (
    res &&
    typeof res === "object" &&
    Array.isArray((res as { items?: unknown[] }).items)
  ) {
    return (res as { items: FlightModel[] }).items;
  }
  return [];
}

export function useFlightsList(key: string, params?: FlightsListParams) {
  const hasFilters = Boolean(params?.status || params?.destination);

  return useQuery<FlightModel[]>({
    queryKey: ["flights", key, params],
    queryFn: () =>
      hasFilters
        ? searchFlights({
            status: params?.status,
            destination: params?.destination,
          })
        : getFlights({
            page: params?.page,
            pageSize: params?.pageSize,
          }),
    select: normalizeFlights,
    refetchOnWindowFocus: false,
    placeholderData:keepPreviousData
  });
}

export function useAddFlight() {
  return useMutation({
    mutationFn: addFlight,
  });
}

export function useDeleteFlight() {
  return useMutation({
    mutationFn: deleteFlight,
  });
}
