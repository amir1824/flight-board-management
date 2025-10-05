import type { Dayjs } from "dayjs";
import type { CRUDModel } from "./baseModels/CRUDModel";

export type FlightStatus = "Scheduled" | "Boarding" | "Departed" | "Landed";

export interface FlightModel extends CRUDModel {
  flightNumber: string;
  destination: string;
  departureTime: Dayjs;
  gate: string;
  status: FlightStatus;
 
}

export type CreateFlightDto = {
  flightNumber: string;
  destination: string;
  departureTime: string;
  gate?: string;
  status?: FlightStatus; 
};