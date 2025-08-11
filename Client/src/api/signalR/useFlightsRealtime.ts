import { HubConnectionBuilder } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import dayjs from "dayjs";
import utc from "dayjs/plugin/utc";
import type { FlightModel, FlightStatus } from "../../store/dbModels/FlightModel";

dayjs.extend(utc);

const FLIGHTS_KEY = ["flights"];

type FlightDto = {
  id: number;
  flightNumber: string;
  destination: string;
  departureTime: string;     
  gate?: string;
  status: FlightStatus;      
  createdAt?: string;
  updatedAt?: string;
};

  interface Window {
    __signalr?: {
      connected?: boolean;
      lastAdded?: FlightModel;
    };
  }

const mapFlight = (f: FlightDto): FlightModel => {
  const base = {
    id: f.id,
    flightNumber: f.flightNumber,
    destination: f.destination,
    departureTime: dayjs.utc(f.departureTime),    
    gate: f.gate || "", 
    status: f.status,                           
  };

  return {
    ...base,
    ...(f.createdAt && { createdAt: dayjs.utc(f.createdAt) }),
    ...(f.updatedAt && { updatedAt: dayjs.utc(f.updatedAt) }),
  } as FlightModel;
};

export function useFlightsRealtime() {
  const qc = useQueryClient();

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl("/hubs/flights")
      .build();

    const updateCache = (fn: (arr: FlightModel[]) => FlightModel[]) => {
      qc.setQueriesData<FlightModel[]>(
        { queryKey: FLIGHTS_KEY, exact: false, type: "all" },
        (old) => fn(Array.isArray(old) ? old : [])
      );
    };

    connection.on("FlightAdded", (dto: FlightDto) => {
      const flight = mapFlight(dto);
      updateCache(arr => (arr.some(f => f.id === flight.id) ? arr : [flight, ...arr]));
      (window as any).__signalr = { ...(window as any).__signalr, lastAdded: flight };
    });

    connection.on("FlightUpdated", (dto: FlightDto) => {
      const flight = mapFlight(dto);
      updateCache(arr => arr.map(f => (f.id === flight.id ? flight : f)));
    });

    connection.on("FlightDeleted", (id: number) => {
      updateCache(arr => arr.filter(f => f.id !== id));
    });

    connection.on("FlightStatusChanged", ({ id, status }: { id: number; status: FlightStatus }) => {
      updateCache(arr => arr.map(f => (f.id === id ? { ...f, status } : f)));
    });

    connection.onreconnected(() => {
      qc.invalidateQueries({ queryKey: FLIGHTS_KEY });
    });

    (async () => {
      try {
        await connection.start();
        (window as Window).__signalr = { ...(window as Window).__signalr, connected: true };
      } catch (err) {
        console.error(err);
      }
    })();

    return () => { void connection.stop(); };
  }, [qc]);
}
