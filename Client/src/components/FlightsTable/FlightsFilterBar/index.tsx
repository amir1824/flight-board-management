import { useState, useEffect } from "react";
import { TextField, MenuItem, Button, Paper, Stack } from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import type { RootState } from "../../../store/store";
import { flightsUiActions } from "../../../store/flightsUiSlice";

const statuses = ["Scheduled", "Boarding", "Departed", "Landed"];

interface FlightsFilterBarProps {
  onAddFlight: () => void;
}

export const FlightsFilterBar = ({ onAddFlight }: FlightsFilterBarProps) => {
  const dispatch = useDispatch();
  const { destination: destValue, status: statusValue } = useSelector(
    (state: RootState) => state.flightsUi
  );

  const [destination, setDestination] = useState(destValue);

  useEffect(() => {
    const timeout = setTimeout(() => {
      dispatch(flightsUiActions.setDestination(destination));
    }, 400);
    return () => clearTimeout(timeout);
  }, [destination, dispatch]);

  return (
    <Paper sx={{ p: 2, mb: 2 }} elevation={0}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        alignItems="center"
        justifyContent="space-between"
      >
        <Stack
          direction={{ xs: "column", sm: "row" }}
          spacing={2}
          flex={1}
          alignItems="center"
        >
          <TextField
            label="Filter by destination"
            variant="outlined"
            size="small"
            value={destination}
            onChange={(e) => setDestination(e.target.value)}
            sx={{ minWidth: 180 }}
          />

          <TextField
            select
            label="Status"
            variant="outlined"
            size="small"
            value={statusValue}
            onChange={(e) =>
              dispatch(flightsUiActions.setStatus(e.target.value as any))
            }
            sx={{ minWidth: 180 }}
          >
            {statuses.map((status) => (
              <MenuItem key={status} value={status}>
                {status}
              </MenuItem>
            ))}
          </TextField>

          <Button
            variant="outlined"
            onClick={() => dispatch(flightsUiActions.clearFilters())}
            sx={{ minWidth: 120 }}
          >
            Clear
          </Button>
        </Stack>

        <Button
          sx={{ minWidth: 120 }}
          variant="contained"
          color="primary"
          onClick={onAddFlight}
        >
          Add Flight
        </Button>
      </Stack>
    </Paper>
  );
};
