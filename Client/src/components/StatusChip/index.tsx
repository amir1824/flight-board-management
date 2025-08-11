import { Chip } from "@mui/material";
export type FlightStatus = "Scheduled" | "Boarding" | "Departed" | "Landed";

const color = (s: FlightStatus): "default" | "warning" | "info" | "success" => {
  switch (s) {
    case "Boarding": return "warning";
    case "Departed": return "info";
    case "Landed":   return "success";
    default:         return "default";
  }
};

export function StatusChip({ status }: { status: FlightStatus }) {
  return <Chip size="small" label={status}   color={color(status)} />;
}
