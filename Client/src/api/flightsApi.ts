import axios from "axios";
import dayjs from "dayjs";
import type { CreateFlightDto, FlightModel, FlightStatus } from "../store/dbModels/FlightModel";

const http = axios.create({ baseURL: "/api" });

function mapFlight(f: FlightModel): FlightModel {
  return {
    ...f,
    departureTime: dayjs(f.departureTime),
    createdAt: dayjs(f.createdAt),
    updatedAt: dayjs(f.updatedAt),
  };
}

export async function getFlights(params?: { page?: number; pageSize?: number }) {
  const { data } = await http.get<FlightModel[]>("/Flights", { params });
  return data.map(mapFlight);
}

export async function searchFlights(params: { status?: string; destination?: string }) {
  const { data } = await http.get<FlightModel[]>("/Flights/search", { params });
  return data.map(mapFlight);
}

export async function addFlight(body: CreateFlightDto) {
  const { data } = await http.post<FlightModel>("/Flights", body);
  return mapFlight(data);
}

export async function deleteFlight(id: number) {
  await http.delete(`/Flights/${id}`);
}
export async function getFlightStatus(id: number) {
  const { data } = await http.get<string>(`/Flights/${id}/status`);
  return data as FlightStatus; 
}