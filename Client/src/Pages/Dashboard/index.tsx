import { useState, type FC } from "react";
import { RecentFlightsCards } from "../../components/RecentFlights";
import { FlightsTable } from "../../components/FlightsTable";
import styles from "./index.module.scss";
import { useDeleteFlight, useFlightsList } from "../../api/flightsQueries";
import { useSelector } from "react-redux";
import { DialogLayout } from "../../components/DialogLayout";
import FlightsForm from "../../components/FlightsForm";
import { useFlightsRealtime } from "../../api/signalR/useFlightsRealtime";
import type { RootState } from "../../store/store";
import { Typography } from "@mui/material";

export const Dashboard: FC = () => {
  useFlightsRealtime();

  const { destination, status, page, pageSize } = useSelector(
    (s: RootState) => s.flightsUi
  );

  const { data = [], isLoading } = useFlightsList(
    destination || status ? "search" : "all",
    { page, pageSize, destination, status }
  );

  const del = useDeleteFlight();

  const [open, setOpen] = useState(false);
  const handleClose = () => setOpen(false);
  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <Typography variant="h3" fontWeight={600}>
          Flight Dashboard
        </Typography>
        {/* <RecentFlightsCards flights={data} /> */}
      </div>
      {isLoading ? (
        "Loading…"
      ) : (
        <FlightsTable
          flights={data}
          onAddFlight={() => setOpen(true)}
          onDelete={(id) => del.mutate(id)}
        />
      )}

      <DialogLayout open={open} onClose={handleClose} title="Add Flight">
        <FlightsForm  onAfterSubmit={handleClose}/>
      </DialogLayout>
    </div>
  );
};
