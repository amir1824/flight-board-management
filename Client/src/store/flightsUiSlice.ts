import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

export type FlightStatus = "Scheduled" | "Boarding" | "Departed" | "Landed" | "";

interface FlightsUiState {
  destination: string;
  status: FlightStatus | "";
  page: number;
  pageSize: number;
  sortBy: "DepartureTime" | "FlightNumber" | "";
  sortDir: "asc" | "desc";
}

const initialState: FlightsUiState = {
  destination: "",
  status: "",
  page: 1,
  pageSize: 50,
  sortBy: "",
  sortDir: "asc",
};

const flightsUiSlice = createSlice({
  name: "flightsUi",
  initialState,
  reducers: {
    setDestination: (s, a: PayloadAction<string>) => { s.destination = a.payload; s.page = 1; },
    setStatus: (s, a: PayloadAction<FlightsUiState["status"]>) => { s.status = a.payload; s.page = 1; },
    clearFilters: (s) => { s.destination = ""; s.status = ""; s.page = 1; },
    setPage: (s, a: PayloadAction<number>) => { s.page = Math.max(1, a.payload); },
    setPageSize: (s, a: PayloadAction<number>) => { s.pageSize = Math.max(1, a.payload); s.page = 1; },
    setSort: (s, a: PayloadAction<{ by: FlightsUiState["sortBy"]; dir: FlightsUiState["sortDir"] }>) => {
      s.sortBy = a.payload.by; s.sortDir = a.payload.dir;
    },
  },
});

export const flightsUiActions = flightsUiSlice.actions;
export default flightsUiSlice.reducer;
