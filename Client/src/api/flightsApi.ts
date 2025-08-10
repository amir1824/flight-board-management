import axios from "axios";
import dayjs from "dayjs";
import type { CreateFlightDto, FlightModel, FlightStatus } from "../store/dbModels/FlightModel";

export interface Flight {
  Id: number;
  FlightNumber: string;
  Destination: string;
  DepartureTime: string; 
  Gate: string;
  Status: "Scheduled" | "Boarding" | "Departed" | "Landed";
}

const http = axios.create({ baseURL: "/api" });

function mapFlight(f: any): FlightModel {
  return {
    ...f,
    departureTime: dayjs(f.departureTime),
    createdAt: dayjs(f.createdAt),
    updatedAt: dayjs(f.updatedAt),
  };
}

export async function getFlights(params?: { page?: number; pageSize?: number }) {
  const { data } = await http.get<Flight[]>("/Flights", { params });
  return data.map(mapFlight);
}

export async function searchFlights(params: { status?: string; destination?: string }) {
  const { data } = await http.get<Flight[]>("/Flights/search", { params });
  return data.map(mapFlight);
}

export async function addFlight(body: CreateFlightDto) {
  const { data } = await http.post<Flight>("/Flights", body);
  return mapFlight(data);
}

export async function deleteFlight(id: number) {
  await http.delete(`/Flights/${id}`);
}
export async function getFlightStatus(id: number) {
  const { data } = await http.get<string>(`/Flights/${id}/status`);
  return data as FlightStatus; 
}